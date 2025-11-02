using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Core_Bible
{
    public partial class NotepadForm : Form
    {
        private string _currentPath = null;
        private bool _isDirty = false;
        private string _lastFind = "";

        // Expected designer controls:
        // RichTextBox noteBox
        // MenuStrip menuStrip1 (optional)
        //   ToolStripToolStripMenuItem openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem (optional)
        //   ToolStripToolStripMenuItem undoToolStripMenuItem, redoToolStripMenuItem, cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, deleteToolStripMenuItem, selectAllToolStripMenuItem (optional)
        //   ToolStripToolStripMenuItem findToolStripMenuItem, replaceToolStripMenuItem, goToToolStripMenuItem (optional)
        //   ToolStripToolStripMenuItem fontToolStripMenuItem (Format -> Font…) (optional)
        // ToolStrip formatToolStrip (Dock=Bottom) with:
        //   ToolStripButton boldButton, italicButton, underlineButton
        //   ToolStripButton bulletButton, numberButton
        //   ToolStripButton increaseFontButton, decreaseFontButton
        // Optional edit toolbar buttons:
        //   ToolStripButton undoButton, redoButton, cutButton, copyButton, pasteButton, deleteButton

        public NotepadForm()
        {
            InitializeComponent();

            this.Text = "Untitled - Notes";

            if (noteBox != null)
            {
                noteBox.AcceptsTab = true;
                noteBox.ShortcutsEnabled = true;
                noteBox.WordWrap = true;
                noteBox.ScrollBars = RichTextBoxScrollBars.Both;
                noteBox.LanguageOption = RichTextBoxLanguageOptions.AutoFont;
                noteBox.HideSelection = false;

                noteBox.TextChanged += (s, e) => MarkDirty(true);
                noteBox.SelectionChanged += noteBox_SelectionChanged;
                noteBox.KeyDown += noteBox_KeyDown; // Ctrl+F/H/G even if menu items removed
            }

            WireMenuHandlersAndShortcuts();
            WireToolStripHandlers();
            SyncToolbarTogglesFromSelection();
        }

        // -------- Dark mode hook from parent --------
        public void ApplyDarkMode(bool enabled)
        {
            Color back = enabled ? Color.FromArgb(27, 30, 33) : SystemColors.Window;
            Color fore = enabled ? Color.FromArgb(230, 230, 230) : SystemColors.WindowText;
            Color chromeBack = enabled ? Color.FromArgb(34, 40, 49) : SystemColors.Control;
            Color chromeFore = enabled ? Color.FromArgb(230, 230, 230) : SystemColors.ControlText;

            this.BackColor = chromeBack; this.ForeColor = chromeFore;
            if (menuStrip1 != null) { menuStrip1.BackColor = chromeBack; menuStrip1.ForeColor = chromeFore; }
            if (formatToolStrip != null) { formatToolStrip.BackColor = chromeBack; formatToolStrip.ForeColor = chromeFore; }
            if (noteBox != null) { noteBox.BackColor = back; noteBox.ForeColor = fore; }
        }

        // -------- File ops --------
        private void OpenFile()
        {
            if (!PromptSaveIfDirty()) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Rich Text (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                ofd.Title = "Open";
                if (ofd.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    if (string.Equals(Path.GetExtension(ofd.FileName), ".rtf", StringComparison.OrdinalIgnoreCase))
                        noteBox.LoadFile(ofd.FileName, RichTextBoxStreamType.RichText);
                    else
                        noteBox.LoadFile(ofd.FileName, RichTextBoxStreamType.PlainText);

                    _currentPath = ofd.FileName;
                    MarkDirty(false);
                    UpdateTitle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Failed to open file:\n" + ex.Message, "Open",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveFile()
        {
            if (string.IsNullOrEmpty(_currentPath)) { SaveFileAs(); return; }

            try
            {
                if (string.Equals(Path.GetExtension(_currentPath), ".rtf", StringComparison.OrdinalIgnoreCase))
                    noteBox.SaveFile(_currentPath, RichTextBoxStreamType.RichText);
                else
                    noteBox.SaveFile(_currentPath, RichTextBoxStreamType.PlainText);

                MarkDirty(false);
                UpdateTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to save:\n" + ex.Message, "Save",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveFileAs()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Rich Text (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                sfd.Title = "Save As";
                if (!string.IsNullOrEmpty(_currentPath))
                {
                    sfd.InitialDirectory = Path.GetDirectoryName(_currentPath);
                    sfd.FileName = Path.GetFileName(_currentPath);
                }

                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    _currentPath = sfd.FileName;
                    if (string.Equals(Path.GetExtension(_currentPath), ".rtf", StringComparison.OrdinalIgnoreCase))
                        noteBox.SaveFile(_currentPath, RichTextBoxStreamType.RichText);
                    else
                        noteBox.SaveFile(_currentPath, RichTextBoxStreamType.PlainText);

                    MarkDirty(false);
                    UpdateTitle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Failed to save:\n" + ex.Message, "Save As",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!PromptSaveIfDirty()) { e.Cancel = true; return; }
            base.OnFormClosing(e);
        }

        private bool PromptSaveIfDirty()
        {
            if (!_isDirty) return true;
            var dr = MessageBox.Show(this, "Save changes to this document?", "Confirm",
                                     MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Cancel) return false;
            if (dr == DialogResult.Yes) SaveFile();
            return true;
        }

        private void MarkDirty(bool dirty)
        {
            _isDirty = dirty;
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            string name = string.IsNullOrEmpty(_currentPath) ? "Untitled" : Path.GetFileName(_currentPath);
            this.Text = (_isDirty ? "*" : "") + name + " - Notes";
        }

        // -------- Edit ops --------
        private void DoUndo() { if (noteBox.CanUndo) noteBox.Undo(); }
        private void DoRedo() { if (noteBox.CanRedo) noteBox.Redo(); }
        private void DoCut() { noteBox.Cut(); }
        private void DoCopy() { noteBox.Copy(); }
        private void DoPaste() { noteBox.Paste(); MarkDirty(true); }
        private void DoDelete()
        {
            if (noteBox.SelectionLength > 0)
            {
                noteBox.SelectedText = "";
                MarkDirty(true);
            }
            else
            {
                int i = noteBox.SelectionStart;
                if (i < noteBox.TextLength)
                {
                    noteBox.SelectionStart = i;
                    noteBox.SelectionLength = 1;
                    noteBox.SelectedText = "";
                    MarkDirty(true);
                }
            }
        }
        private void DoSelectAll() { noteBox.SelectAll(); }

        private void DoFind()
        {
            string term;
            if (!ShowInput("Find", "Enter text to find:", _lastFind, out term)) return;
            if (term == null || term.Length == 0) return;
            _lastFind = term;

            int start = noteBox.SelectionStart + noteBox.SelectionLength;
            if (start < 0 || start >= noteBox.TextLength) start = 0;

            int idx = IndexOfIgnoreCase(noteBox.Text, term, start);
            if (idx < 0 && start > 0)
                idx = IndexOfIgnoreCase(noteBox.Text, term, 0);

            if (idx >= 0)
            {
                noteBox.SelectionStart = idx;
                noteBox.SelectionLength = term.Length;
                noteBox.ScrollToCaret();
            }
            else
            {
                MessageBox.Show(this, "Text not found.", "Find",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DoReplace()
        {
            string findText, replaceText; bool replaceAll;
            if (!ShowReplace("Replace", "Find what:", "Replace with:", _lastFind, "",
                             out findText, out replaceText, out replaceAll)) return;
            if (findText == null || findText.Length == 0) return;
            _lastFind = findText;

            if (replaceAll)
            {
                // Plain-text replace-all (warn if RTF)
                bool rtfDoc = noteBox.Rtf != null && noteBox.Rtf.StartsWith(@"{\rtf");
                if (rtfDoc)
                {
                    var dr = MessageBox.Show(this, "Replace all may affect rich formatting. Continue?",
                                             "Replace All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dr != DialogResult.Yes) return;
                }

                string src = noteBox.Text;
                string replaced = ReplaceCaseInsensitive(src, findText, replaceText ?? "");
                if (!object.ReferenceEquals(src, replaced))
                {
                    int selStart = noteBox.SelectionStart;
                    noteBox.Text = replaced;
                    noteBox.SelectionStart = Math.Min(selStart, noteBox.TextLength);
                    noteBox.SelectionLength = 0;
                    noteBox.ScrollToCaret();
                    MarkDirty(true);
                }
                return;
            }

            // Replace selection or next occurrence
            if (noteBox.SelectionLength > 0 &&
                string.Compare(noteBox.SelectedText, findText, true) == 0)
            {
                noteBox.SelectedText = replaceText ?? "";
                MarkDirty(true);
                return;
            }

            int from = noteBox.SelectionStart + noteBox.SelectionLength;
            if (from >= noteBox.TextLength) from = 0;

            int idx = IndexOfIgnoreCase(noteBox.Text, findText, from);
            if (idx < 0 && from > 0)
                idx = IndexOfIgnoreCase(noteBox.Text, findText, 0);

            if (idx >= 0)
            {
                noteBox.SelectionStart = idx;
                noteBox.SelectionLength = findText.Length;
                noteBox.SelectedText = replaceText ?? "";
                MarkDirty(true);
            }
            else
            {
                MessageBox.Show(this, "Text not found.", "Replace",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DoGoTo()
        {
            string input;
            if (!ShowInput("Go To Line", "Enter line number (1..N):", "", out input)) return;
            int line;
            if (!int.TryParse(input, out line) || line <= 0) line = 1;

            int total = (noteBox.Lines != null) ? noteBox.Lines.Length : 1;
            if (line > total) line = total;

            int idx = noteBox.GetFirstCharIndexFromLine(line - 1);
            if (idx < 0) idx = noteBox.TextLength;

            noteBox.SelectionStart = idx;
            noteBox.SelectionLength = 0;
            noteBox.ScrollToCaret();
        }

        // -------- Formatting (selection-only) --------
        private void ToggleBold() { ToggleStyleOnSelection(FontStyle.Bold); }
        private void ToggleItalic() { ToggleStyleOnSelection(FontStyle.Italic); }
        private void ToggleUnderline() { ToggleStyleOnSelection(FontStyle.Underline); }

        private void ToggleStyleOnSelection(FontStyle style)
        {
            if (noteBox.SelectionLength == 0) { System.Media.SystemSounds.Beep.Play(); return; }

            Font baseFont = noteBox.SelectionFont ?? noteBox.Font;
            FontStyle newStyle = ((baseFont.Style & style) == style)
                ? (baseFont.Style & ~style)
                : (baseFont.Style | style);

            noteBox.SelectionFont = new Font(baseFont, newStyle);
            MarkDirty(true);
            SyncToolbarTogglesFromSelection();
        }

        private void IncreaseFont()
        {
            if (noteBox.SelectionLength == 0) { System.Media.SystemSounds.Beep.Play(); return; }
            Font f = noteBox.SelectionFont ?? noteBox.Font;
            float size = Math.Max(6f, Math.Min(200f, f.Size + 1f));
            noteBox.SelectionFont = new Font(f.FontFamily, size, f.Style);
            MarkDirty(true);
            SyncToolbarTogglesFromSelection();
        }

        private void DecreaseFont()
        {
            if (noteBox.SelectionLength == 0) { System.Media.SystemSounds.Beep.Play(); return; }
            Font f = noteBox.SelectionFont ?? noteBox.Font;
            float size = Math.Max(6f, Math.Min(200f, f.Size - 1f));
            noteBox.SelectionFont = new Font(f.FontFamily, size, f.Style);
            MarkDirty(true);
            SyncToolbarTogglesFromSelection();
        }

        private void ToggleBullets()
        {
            noteBox.BulletIndent = 16;
            noteBox.SelectionBullet = !noteBox.SelectionBullet;
            MarkDirty(true);
        }

        // Adds/removes numeric prefixes (1. , 2. , …) at line starts without nuking formatting.
        private void ToggleNumbering()
        {
            int selStart = noteBox.SelectionStart;
            int selEnd = selStart + noteBox.SelectionLength;

            int firstLine = noteBox.GetLineFromCharIndex(selStart);
            int lastLine = noteBox.GetLineFromCharIndex(Math.Max(selEnd - 1, selStart));

            if (noteBox.Lines == null || noteBox.Lines.Length == 0) return;

            Regex numRx = new Regex(@"^\s*(\d+)[\.\)]\s+");

            bool allNumbered = true;
            int line;
            for (line = firstLine; line <= lastLine; line++)
            {
                int lineStart = noteBox.GetFirstCharIndexFromLine(line);
                if (lineStart < 0) continue;

                int lineEnd = (line == noteBox.Lines.Length - 1)
                    ? noteBox.TextLength
                    : noteBox.GetFirstCharIndexFromLine(line + 1);

                int len = Math.Max(0, lineEnd - lineStart);
                noteBox.Select(lineStart, len);
                string lineText = (noteBox.SelectedText ?? "").Replace("\r", "").Replace("\n", "");

                if (lineText.Length == 0) continue;
                if (!numRx.IsMatch(lineText)) { allNumbered = false; break; }
            }

            noteBox.SuspendLayout();
            try
            {
                if (allNumbered)
                {
                    for (line = firstLine; line <= lastLine; line++)
                    {
                        int lineStart = noteBox.GetFirstCharIndexFromLine(line);
                        if (lineStart < 0) continue;

                        int lineEnd = (line == noteBox.Lines.Length - 1)
                            ? noteBox.TextLength
                            : noteBox.GetFirstCharIndexFromLine(line + 1);

                        int len = Math.Max(0, lineEnd - lineStart);
                        noteBox.Select(lineStart, len);
                        string lineText = (noteBox.SelectedText ?? "").Replace("\r", "").Replace("\n", "");
                        if (lineText.Length == 0) continue;

                        Match m = numRx.Match(lineText);
                        if (m.Success)
                        {
                            int removeLen = m.Length;
                            noteBox.Select(lineStart, removeLen);
                            noteBox.SelectedText = "";
                        }
                    }
                }
                else
                {
                    int n = 1;
                    for (line = firstLine; line <= lastLine; line++)
                    {
                        int lineStart = noteBox.GetFirstCharIndexFromLine(line);
                        if (lineStart < 0) continue;

                        int lineEnd = (line == noteBox.Lines.Length - 1)
                            ? noteBox.TextLength
                            : noteBox.GetFirstCharIndexFromLine(line + 1);

                        int len = Math.Max(0, lineEnd - lineStart);
                        noteBox.Select(lineStart, len);
                        string lineText = (noteBox.SelectedText ?? "").Replace("\r", "").Replace("\n", "");

                        if (lineText.Length == 0) continue;
                        if (numRx.IsMatch(lineText)) continue;

                        string prefix = n.ToString() + ". ";
                        noteBox.Select(lineStart, 0);
                        noteBox.SelectedText = prefix;
                        n++;
                    }
                }
            }
            finally
            {
                noteBox.Select(selStart, Math.Max(0, noteBox.SelectionLength));
                noteBox.ScrollToCaret();
                noteBox.ResumeLayout();
                MarkDirty(true);
            }
        }

        // -------- Selection sync --------
        private void noteBox_SelectionChanged(object sender, EventArgs e)
        {
            SyncToolbarTogglesFromSelection();
        }

        private void SyncToolbarTogglesFromSelection()
        {
            try
            {
                Font f = noteBox.SelectionFont ?? noteBox.Font;
                if (boldButton != null) boldButton.Checked = (f.Style & FontStyle.Bold) == FontStyle.Bold;
                if (italicButton != null) italicButton.Checked = (f.Style & FontStyle.Italic) == FontStyle.Italic;
                if (underlineButton != null) underlineButton.Checked = (f.Style & FontStyle.Underline) == FontStyle.Underline;
            }
            catch { }
        }

        // -------- Menus / toolbar wiring --------
        private void WireMenuHandlersAndShortcuts()
        {
            // File
            if (openToolStripMenuItem != null) { openToolStripMenuItem.Click += delegate { OpenFile(); }; openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O; }
            if (saveToolStripMenuItem != null) { saveToolStripMenuItem.Click += delegate { SaveFile(); }; saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S; }
            if (saveAsToolStripMenuItem != null) { saveAsToolStripMenuItem.Click += delegate { SaveFileAs(); }; saveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S; }

            // Edit
            if (undoToolStripMenuItem != null) { undoToolStripMenuItem.Click += delegate { DoUndo(); }; undoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z; }
            if (redoToolStripMenuItem != null) { redoToolStripMenuItem.Click += delegate { DoRedo(); }; redoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y; }
            if (cutToolStripMenuItem != null) { cutToolStripMenuItem.Click += delegate { DoCut(); }; cutToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X; }
            if (copyToolStripMenuItem != null) { copyToolStripMenuItem.Click += delegate { DoCopy(); }; copyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C; }
            if (pasteToolStripMenuItem != null) { pasteToolStripMenuItem.Click += delegate { DoPaste(); }; pasteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V; }
            if (deleteToolStripMenuItem != null) { deleteToolStripMenuItem.Click += delegate { DoDelete(); }; deleteToolStripMenuItem.ShortcutKeys = Keys.Delete; }
            if (selectAllToolStripMenuItem != null) { selectAllToolStripMenuItem.Click += delegate { DoSelectAll(); }; selectAllToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.A; }

            // Find/Replace/GoTo
            if (findToolStripMenuItem != null) { findToolStripMenuItem.Click += delegate { DoFind(); }; findToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.F; }
            if (replaceToolStripMenuItem != null) { replaceToolStripMenuItem.Click += delegate { DoReplace(); }; replaceToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.H; }
            if (goToToolStripMenuItem != null) { goToToolStripMenuItem.Click += delegate { DoGoTo(); }; goToToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.G; }

            // Format -> Font…
            if (fontToolStripMenuItem != null) { fontToolStripMenuItem.Click += delegate { DoFontDialog(); }; }
        }

        private void WireToolStripHandlers()
        {
            if (formatToolStrip != null) formatToolStrip.Dock = DockStyle.Bottom;

            if (boldButton != null) boldButton.Click += delegate { ToggleBold(); };
            if (italicButton != null) italicButton.Click += delegate { ToggleItalic(); };
            if (underlineButton != null) underlineButton.Click += delegate { ToggleUnderline(); };

            if (increaseFontButton != null) increaseFontButton.Click += delegate { IncreaseFont(); };
            if (decreaseFontButton != null) decreaseFontButton.Click += delegate { DecreaseFont(); };

            if (bulletButton != null) bulletButton.Click += delegate { ToggleBullets(); };
            if (numberButton != null) numberButton.Click += delegate { ToggleNumbering(); };
        }

        // Shortcuts if menu items removed
        private void noteBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F) { DoFind(); e.SuppressKeyPress = true; }
            else if (e.Control && e.KeyCode == Keys.H) { DoReplace(); e.SuppressKeyPress = true; }
            else if (e.Control && e.KeyCode == Keys.G) { DoGoTo(); e.SuppressKeyPress = true; }
        }

        // -------- Font dialog (selection if any; otherwise control font) --------
        private void DoFontDialog()
        {
            using (FontDialog fd = new FontDialog())
            {
                try
                {
                    if (noteBox.SelectionLength > 0 && noteBox.SelectionFont != null)
                        fd.Font = noteBox.SelectionFont;
                    else
                        fd.Font = noteBox.Font;
                }
                catch
                {
                    fd.Font = noteBox.Font;
                }

                fd.ShowColor = false;
                if (fd.ShowDialog(this) == DialogResult.OK)
                {
                    if (noteBox.SelectionLength > 0)
                        noteBox.SelectionFont = fd.Font;
                    else
                        noteBox.Font = fd.Font;

                    MarkDirty(true);
                    SyncToolbarTogglesFromSelection();
                }
            }
        }

        // -------- small inline dialogs --------
        private bool ShowInput(string title, string prompt, string initial, out string text)
        {
            text = initial ?? "";
            using (Form f = new Form())
            {
                f.Text = title;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MinimizeBox = false; f.MaximizeBox = false;
                f.StartPosition = FormStartPosition.CenterParent;
                f.ClientSize = new Size(360, 120);
                f.ShowInTaskbar = false;

                Label l = new Label();
                l.Text = prompt; l.AutoSize = true; l.Location = new Point(12, 12);

                TextBox t = new TextBox();
                t.Text = initial ?? ""; t.Location = new Point(15, 40); t.Width = 330;

                System.Windows.Forms.Button ok = new System.Windows.Forms.Button();
                ok.Text = "OK"; ok.DialogResult = DialogResult.OK; ok.Location = new Point(190, 80);

                System.Windows.Forms.Button cancel = new System.Windows.Forms.Button();
                cancel.Text = "Cancel"; cancel.DialogResult = DialogResult.Cancel; cancel.Location = new Point(270, 80);

                f.Controls.AddRange(new Control[] { l, t, ok, cancel });
                f.AcceptButton = ok; f.CancelButton = cancel;

                if (f.ShowDialog(this) == DialogResult.OK) { text = t.Text; return true; }
                return false;
            }
        }

        private bool ShowReplace(string title, string promptFind, string promptReplace,
                                 string initialFind, string initialReplace,
                                 out string findText, out string replaceText, out bool replaceAll)
        {
            findText = initialFind ?? "";
            replaceText = initialReplace ?? "";
            replaceAll = false;

            using (Form f = new Form())
            {
                f.Text = title;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MinimizeBox = false; f.MaximizeBox = false;
                f.StartPosition = FormStartPosition.CenterParent;
                f.ClientSize = new Size(400, 170);
                f.ShowInTaskbar = false;

                Label lf = new Label { Text = promptFind, AutoSize = true, Location = new Point(12, 14) };
                TextBox tf = new TextBox { Text = findText, Location = new Point(110, 12), Width = 270 };

                Label lr = new Label { Text = promptReplace, AutoSize = true, Location = new Point(12, 50) };
                TextBox tr = new TextBox { Text = replaceText, Location = new Point(110, 48), Width = 270 };

                // Fully-qualify to avoid ambiguous CheckBox type
                System.Windows.Forms.CheckBox cbAll = new System.Windows.Forms.CheckBox
                {
                    Text = "Replace all",
                    AutoSize = true,
                    Location = new Point(110, 78)
                };

                System.Windows.Forms.Button bok = new System.Windows.Forms.Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(290, 115) };
                System.Windows.Forms.Button bcancel = new System.Windows.Forms.Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(320, 115) };

                f.Controls.AddRange(new Control[] { lf, tf, lr, tr, cbAll, bok, bcancel });
                f.AcceptButton = bok; f.CancelButton = bcancel;

                DialogResult dr = f.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    findText = tf.Text;
                    replaceText = tr.Text;
                    replaceAll = cbAll.Checked;
                    return true;
                }
                return false;
            }
        }

        // -------- helpers --------
        private static string ReplaceCaseInsensitive(string input, string find, string replacement)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(find)) return input;
            StringBuilder sb = new StringBuilder(input.Length);
            int prev = 0, idx;
            while ((idx = IndexOfIgnoreCase(input, find, prev)) >= 0)
            {
                sb.Append(input, prev, idx - prev);
                sb.Append(replacement ?? "");
                prev = idx + find.Length;
            }
            sb.Append(input, prev, input.Length - prev);
            return sb.ToString();
        }

        private static int IndexOfIgnoreCase(string text, string value, int startIndex)
        {
            // .NET 2.0 friendly
            return text.IndexOf(value, startIndex, StringComparison.OrdinalIgnoreCase);
        }
    }
}
