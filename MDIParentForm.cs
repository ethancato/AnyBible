using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Core_Bible
{
    public partial class MDIParentForm : Form
    {
        // Existing controls expected (from Designer):
        // CheckBox  darkModeCheckbox
        // CheckBox  inlineModeCheckbox
        // ComboBox  translationCombo
        // ComboBox  zoomCombo
        // TreeView  treeViewBible
        // Button    votdButton
        // Button    cotdButton
        // MenuItems (if present): readingChecklistToolStripMenuItem, settingsToolStripMenuItem, aboutToolStripMenuItem, exitToolStripMenuItem

        private bool globalDarkMode = false;
        private bool globalInlineMode = false;
        private bool _suppressUiSync = false;

        private Color _mdiBackOriginal = SystemColors.AppWorkspace;
        private bool _mdiBackCaptured = false;

        private static readonly string[] BookNames = {
            "Genesis","Exodus","Leviticus","Numbers","Deuteronomy",
            "Joshua","Judges","Ruth","1 Samuel","2 Samuel",
            "1 Kings","2 Kings","1 Chronicles","2 Chronicles","Ezra",
            "Nehemiah","Esther","Job","Psalms","Proverbs",
            "Ecclesiastes","Song of Solomon","Isaiah","Jeremiah","Lamentations",
            "Ezekiel","Daniel","Hosea","Joel","Amos",
            "Obadiah","Jonah","Micah","Nahum","Habakkuk",
            "Zephaniah","Haggai","Zechariah","Malachi",
            "Matthew","Mark","Luke","John","Acts",
            "Romans","1 Corinthians","2 Corinthians","Galatians","Ephesians",
            "Philippians","Colossians","1 Thessalonians","2 Thessalonians","1 Timothy",
            "2 Timothy","Titus","Philemon","Hebrews","James",
            "1 Peter","2 Peter","1 John","2 John","3 John",
            "Jude","Revelation"
        };

        public MDIParentForm()
        {
            InitializeComponent();

            this.IsMdiContainer = true;
            this.WindowState = FormWindowState.Maximized;

            this.Load += MDIParentForm_Load;
            this.Shown += MDIParentForm_Shown;
            this.MdiChildActivate += MDIParentForm_MdiChildActivate;
            this.FormClosing += MDIParentForm_FormClosing;

            this.Shown += (s, e) =>
            {
                this.BeginInvoke((MethodInvoker)(() =>
                {
                    var child = CreateChild();
                    string def = Properties.Settings.Default.DefaultBible;
                    if (!string.IsNullOrEmpty(def)) child.LoadBibleByTranslationName(def);
                    child.OpenGenesis1();   // force Genesis 1 at startup
                    child.BringToFront();
                }));
            };

            CaptureMdiClientBackColorOnce();

            // Hook control events if they exist
            if (darkModeCheckbox != null) darkModeCheckbox.CheckedChanged += darkModeCheckbox_CheckedChanged;
            if (inlineModeCheckbox != null) inlineModeCheckbox.CheckedChanged += inlineModeCheckbox_CheckedChanged;

            if (translationCombo != null) translationCombo.SelectedIndexChanged += translationCombo_SelectedIndexChanged;
            if (zoomCombo != null) zoomCombo.SelectedIndexChanged += zoomCombo_SelectedIndexChanged;

            if (treeViewBible != null) treeViewBible.AfterSelect += treeViewBible_AfterSelect;

            if (votdButton != null) votdButton.Click += votdButton_Click;
            if (cotdButton != null) cotdButton.Click += cotdButton_Click;

            // Menu items (if they exist in your designer)
            var mi = this.MainMenuStrip;
            if (this.settingsToolStripMenuItem != null) this.settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            if (this.aboutToolStripMenuItem != null) this.aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            if (this.exitToolStripMenuItem != null) this.exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            if (this.readingChecklistToolStripMenuItem != null) this.readingChecklistToolStripMenuItem.Click += readingChecklistToolStripMenuItem_Click;

            if (zoomCombo != null && zoomCombo.Items.Count == 0)
            {
                zoomCombo.Items.AddRange(new object[] { "50%", "75%", "100%", "125%", "150%", "175%", "200%" });
            }
        }

        private void MDIParentForm_Load(object sender, EventArgs e)
        {
            // Create an initial child so parent UI has a target
            PassageChildForm child = CreateChild();
            child.Show();

            // Restore persisted globals
            globalDarkMode = Properties.Settings.Default.LastDarkMode;
            globalInlineMode = Properties.Settings.Default.LastInlineMode;

            if (darkModeCheckbox != null) darkModeCheckbox.Checked = globalDarkMode;
            if (inlineModeCheckbox != null) inlineModeCheckbox.Checked = globalInlineMode;

            // Restore parent zoom combo to persisted zoom
            if (zoomCombo != null)
            {
                int z = Properties.Settings.Default.LastZoomLevel;
                if (z < 50 || z > 200) z = 100;
                string want = z.ToString() + "%";
                int zi = FindComboIndex(zoomCombo, want);
                if (zi >= 0) zoomCombo.SelectedIndex = zi;
                else zoomCombo.SelectedIndex = 2; // 100%
            }
            child.OpenGenesis1();
            SyncUiFromActiveChild();
            PopulateTranslationsComboFromSettings();

            PopulateTreeForActiveChild();

            ApplyParentChromeDark(globalDarkMode);

            PopulateTranslationsComboFromSettings();

            // If there is a saved LastOpenedPage, try to restore it into the active child
            string last = Properties.Settings.Default.LastOpenedPage;
            if (!IsNullOrWhiteSpace(last))
            {
                string[] parts = last.Split('|');
                if (parts != null && parts.Length == 3)
                {
                    string def = Properties.Settings.Default.DefaultBible;
                    if (!string.IsNullOrEmpty(def))
                        child.LoadBibleByTranslationName(def);
                    child.OpenGenesis1();   // forces Genesis 1
                    child.BringToFront();   // ensure it’s the active visible child

                }
            }
            else
            {
                // No saved page; ensure something renders if a default is available
                string def = Properties.Settings.Default.DefaultBible;
                if (!string.IsNullOrEmpty(def))
                    child.LoadBibleByTranslationName(def);
                child.OpenGenesis1();   // forces Genesis 1
                child.BringToFront();   // ensure it’s the active visible child

            }

            SyncUiFromActiveChild();
            PopulateTreeForActiveChild();
        }

        private void MDIParentForm_Shown(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void MDIParentForm_MdiChildActivate(object sender, EventArgs e)
        {
            SyncUiFromActiveChild();
            PopulateTreeForActiveChild();
        }

        private PassageChildForm CreateChild()
        {
            PassageChildForm child = new PassageChildForm();
            child.MdiParent = this;

            // Apply persisted zoom to the child
            int z = Properties.Settings.Default.LastZoomLevel;
            if (z < 50 || z > 200) z = 100;
            child.SetZoomPercent(z);

            // Apply globals
            child.ApplyGlobals(globalDarkMode, globalInlineMode);

            // Reasonable placement
            int offset = this.MdiChildren.Length * 24;
            child.StartPosition = FormStartPosition.Manual;
            child.Location = new Point(8 + offset, 8 + offset);
            if (child.Width < 800 || child.Height < 600)
                child.Size = new Size(Math.Max(child.Width, 800), Math.Max(child.Height, 600));

            return child;
        }

        private void MDIParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.LastDarkMode = globalDarkMode;
            Properties.Settings.Default.LastInlineMode = globalInlineMode;

            PassageChildForm child = this.ActiveMdiChild as PassageChildForm;
            if (child != null)
            {
                Properties.Settings.Default.LastZoomLevel = child.ZoomPercent;
            }
            Properties.Settings.Default.Save();
        }

        // ===== Global toggles =====
        private void darkModeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (darkModeCheckbox == null) return;
            globalDarkMode = darkModeCheckbox.Checked;

            Properties.Settings.Default.LastDarkMode = globalDarkMode;
            Properties.Settings.Default.Save();

            ApplyParentChromeDark(globalDarkMode);
            ApplyGlobalsToAllChildren();
        }

        private void inlineModeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (inlineModeCheckbox == null) return;
            globalInlineMode = inlineModeCheckbox.Checked;

            Properties.Settings.Default.LastInlineMode = globalInlineMode;
            Properties.Settings.Default.Save();

            ApplyGlobalsToAllChildren();
        }

        // Parent chrome theming, but keep MDI client background unchanged
        private void CaptureMdiClientBackColorOnce()
        {
            if (_mdiBackCaptured) return;
            for (int i = 0; i < this.Controls.Count; i++)
            {
                MdiClient mc = this.Controls[i] as MdiClient;
                if (mc != null)
                {
                    _mdiBackOriginal = mc.BackColor;
                    _mdiBackCaptured = true;
                    break;
                }
            }
        }

        private void RestoreMdiClientBackColor()
        {
            for (int i = 0; i < this.Controls.Count; i++)
            {
                MdiClient mc = this.Controls[i] as MdiClient;
                if (mc != null)
                {
                    mc.BackColor = _mdiBackOriginal;
                    mc.ForeColor = SystemColors.ControlText;
                }
            }
        }

        private void ApplyParentChromeDark(bool enabled)
        {
            Color back = enabled ? Color.FromArgb(34, 40, 49) : SystemColors.Control;       // control backgrounds
            Color fore = enabled ? Color.FromArgb(230, 230, 230) : SystemColors.ControlText; // text

            ApplyDarkRecursive(this, back, fore);
            RestoreMdiClientBackColor(); // keep MDI surface default
        }

        private void ApplyDarkRecursive(Control root, Color back, Color fore)
        {
            if (root is MdiClient) return; // do not recolor MDI background

            // Don't touch the form's own BackColor (so system borders/menu keep look),
            // but do theme its child controls.
            if (!(root is Form))
            {
                try { root.BackColor = back; } catch { }
                try { root.ForeColor = fore; } catch { }
            }

            for (int i = 0; i < root.Controls.Count; i++)
                ApplyDarkRecursive(root.Controls[i], back, fore);
        }

        private void ApplyGlobalsToAllChildren()
        {
            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                PassageChildForm pc = this.MdiChildren[i] as PassageChildForm;
                if (pc != null)
                {
                    pc.ApplyGlobals(globalDarkMode, globalInlineMode);
                    continue;
                }
                ReadingChecklistForm rc = this.MdiChildren[i] as ReadingChecklistForm;
                if (rc != null)
                {
                    rc.ApplyDarkMode(globalDarkMode);
                }
            }
        }

        // ===== Per-active-child controls =====
        private void translationCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressUiSync) return;
            if (translationCombo == null || translationCombo.SelectedItem == null) return;

            PassageChildForm child = this.ActiveMdiChild as PassageChildForm;
            if (child == null) return;

            string tName = translationCombo.SelectedItem.ToString();
            if (child.LoadBibleByTranslationName(tName))
            {
                if (IsNullOrWhiteSpace(child.CurrentBookName)) child.CurrentBookName = "Genesis";
                if (IsNullOrWhiteSpace(child.CurrentChapterNumber)) child.CurrentChapterNumber = "1";
                child.RenderCurrentChapter();
                PopulateTreeForActiveChild();
            }
            else
            {
                MessageBox.Show("Bible not found: " + tName, "Error");
            }
        }

        private void zoomCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressUiSync) return;
            if (zoomCombo == null || zoomCombo.SelectedItem == null) return;

            PassageChildForm child = this.ActiveMdiChild as PassageChildForm;
            if (child == null) return;

            string sel = zoomCombo.SelectedItem.ToString().Replace("%", "");
            int z;
            if (int.TryParse(sel, out z))
            {
                child.SetZoomPercent(z);
                Properties.Settings.Default.LastZoomLevel = z;
                Properties.Settings.Default.Save();
            }
        }

        // ===== Navigation from tree (active child) =====
        private void treeViewBible_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewBible == null || e == null || e.Node == null) return;

            PassageChildForm child = this.ActiveMdiChild as PassageChildForm;
            if (child == null)
            {
                child = CreateChild();
                child.Show();
            }

            TreeNode node = e.Node;
            string bookName = null;
            string chapterNumber = null;

            if (node.Parent != null && node.Parent.Parent != null && node.Text.StartsWith("Chapter "))
            {
                chapterNumber = node.Text.Substring("Chapter ".Length);
                bookName = node.Parent.Text;
            }

            if (!IsNullOrWhiteSpace(bookName) && !IsNullOrWhiteSpace(chapterNumber))
            {
                child.CurrentBookName = bookName;
                child.CurrentChapterNumber = chapterNumber;
                child.RenderCurrentChapter();
            }
        }

        // ===== Actions =====
        private void votdButton_Click(object sender, EventArgs e)
        {
            PassageChildForm child = this.ActiveMdiChild as PassageChildForm;
            if (child == null) { MessageBox.Show("Open a passage window first."); return; }

            string refText, verseText;
            if (child.TryRandomVerse(out refText, out verseText))
            {
                // Set the active window to that chapter
                int space = refText.LastIndexOf(' ');
                if (space > 0 && space + 1 < refText.Length)
                {
                    string bname = refText.Substring(0, space);
                    string rest = refText.Substring(space + 1);
                    int colon = rest.IndexOf(':');
                    string chn = (colon > 0) ? rest.Substring(0, colon) : rest;

                    child.CurrentBookName = bname;
                    child.CurrentChapterNumber = chn;
                    child.RenderCurrentChapter();
                }

                MessageBox.Show(refText + "\n\n" + verseText, "Verse of the Day");

            }
        }

        private void cotdButton_Click(object sender, EventArgs e)
        {
            PassageChildForm child = this.ActiveMdiChild as PassageChildForm;
            if (child == null) { MessageBox.Show("Open a passage window first."); return; }
            string refText;
            child.TryRandomChapter(out refText);
        }

        // ===== Sync UI from active child =====
        private void SyncUiFromActiveChild()
        {
            _suppressUiSync = true;
            try
            {
                PassageChildForm child = this.ActiveMdiChild as PassageChildForm;

                if (darkModeCheckbox != null) darkModeCheckbox.Checked = globalDarkMode;
                if (inlineModeCheckbox != null) inlineModeCheckbox.Checked = globalInlineMode;

                PopulateTranslationsComboFromSettings();

                if (child != null)
                {
                    if (translationCombo != null && !IsNullOrWhiteSpace(child.TranslationName))
                    {
                        int idx = FindComboIndex(translationCombo, child.TranslationName);
                        if (idx >= 0) translationCombo.SelectedIndex = idx;
                    }

                    if (zoomCombo != null)
                    {
                        string want = child.ZoomPercent.ToString() + "%";
                        int zi = FindComboIndex(zoomCombo, want);
                        if (zi >= 0) zoomCombo.SelectedIndex = zi;
                    }
                }
            }
            finally
            {
                _suppressUiSync = false;
            }
        }

        private void PopulateTranslationsComboFromSettings()
        {
            if (translationCombo == null) return;

            _suppressUiSync = true;
            try
            {
                translationCombo.Items.Clear();

                string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                string biblesDir = baseDir + "\\Bibles";
                if (!Directory.Exists(biblesDir)) Directory.CreateDirectory(biblesDir);

                string[] actives = {
                    Properties.Settings.Default.Bible1,
                    Properties.Settings.Default.Bible2,
                    Properties.Settings.Default.Bible3,
                    Properties.Settings.Default.Bible4,
                    Properties.Settings.Default.Bible5
                };

                for (int i = 0; i < actives.Length; i++)
                {
                    string name = actives[i];
                    if (IsNullOrWhiteSpace(name)) continue;

                    string path = biblesDir + "\\" + name + ".xml";
                    if (File.Exists(path))
                        translationCombo.Items.Add(name);
                }

                if (translationCombo.Items.Count > 0 && translationCombo.SelectedIndex < 0)
                    translationCombo.SelectedIndex = 0;
            }
            finally
            {
                _suppressUiSync = false;
            }
        }

        private void PopulateTreeForActiveChild()
        {
            if (treeViewBible == null) return;

            treeViewBible.Nodes.Clear();

            PassageChildForm child = this.ActiveMdiChild as PassageChildForm;
            if (child == null || child.BibleXml == null || child.BibleXml.DocumentElement == null)
                return;

            string tName = child.TranslationName;
            if (IsNullOrWhiteSpace(tName)) tName = "Translation";

            TreeNode root = new TreeNode(tName);

            XmlNodeList testaments = child.BibleXml.SelectNodes("/bible/testament");
            if (testaments != null)
            {
                for (int ti = 0; ti < testaments.Count; ti++)
                {
                    XmlNode testament = testaments[ti];
                    string testamentName = (testament.Attributes != null && testament.Attributes["name"] != null)
                        ? testament.Attributes["name"].Value : "Testament";
                    TreeNode tnode = new TreeNode(testamentName);

                    XmlNodeList books = testament.SelectNodes("book");
                    for (int bi = 0; bi < books.Count; bi++)
                    {
                        XmlNode book = books[bi];
                        string numStr = (book.Attributes != null && book.Attributes["number"] != null)
                            ? book.Attributes["number"].Value : "";
                        int bnum = 0; int.TryParse(numStr, out bnum);
                        string bname = (bnum >= 1 && bnum <= BookNames.Length) ? BookNames[bnum - 1] : "Book " + numStr;
                        TreeNode bnode = new TreeNode(bname);

                        XmlNodeList chapters = book.SelectNodes("chapter");
                        for (int ci = 0; ci < chapters.Count; ci++)
                        {
                            XmlNode ch = chapters[ci];
                            string chNum = (ch.Attributes != null && ch.Attributes["number"] != null)
                                ? ch.Attributes["number"].Value : (ci + 1).ToString();
                            TreeNode cnode = new TreeNode("Chapter " + chNum);
                            bnode.Nodes.Add(cnode);
                        }

                        tnode.Nodes.Add(bnode);
                    }

                    root.Nodes.Add(tnode);
                }
            }

            treeViewBible.Nodes.Add(root);
            root.Expand();
        }

        // ===== Menus =====
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Settings dlg = new Settings())
            {
                DialogResult dr = dlg.ShowDialog(this);
                if (dr == DialogResult.OK)
                {
                    PopulateTranslationsComboFromSettings();

                    string desired = Properties.Settings.Default.DefaultBible;
                    if (translationCombo != null && translationCombo.Items.Count > 0)
                    {
                        int idx = FindComboIndex(translationCombo, desired);
                        translationCombo.SelectedIndex = (idx >= 0) ? idx : 0;
                    }

                    // Re-render child/children with new settings
                    ApplyGlobalsToAllChildren();

                    PassageChildForm child = this.ActiveMdiChild as PassageChildForm;
                    if (child != null) child.RenderCurrentChapter();

                    PopulateTreeForActiveChild();
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void newBibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PassageChildForm child = CreateChild();
            child.Show();
        }
        private void readingChecklistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadingChecklistForm rc = null;
            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                rc = this.MdiChildren[i] as ReadingChecklistForm;
                if (rc != null) break;
            }
            if (rc == null)
            {
                rc = new ReadingChecklistForm();
                rc.MdiParent = this;
                rc.Show();
            }
            rc.ApplyDarkMode(globalDarkMode);
            rc.BringToFront();
        }

        // ===== Helpers =====
        private int FindComboIndex(ComboBox combo, string value)
        {
            if (combo == null) return -1;
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (string.Compare(combo.Items[i].ToString(), value, true) == 0)
                    return i;
            }
            return -1;
        }

        private static bool IsNullOrWhiteSpace(string s)
        {
            if (s == null) return true;
            for (int i = 0; i < s.Length; i++)
                if (!char.IsWhiteSpace(s[i])) return false;
            return true;
        }

        private void passageSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PassageLookup passageLookup = new PassageLookup();
            passageLookup.ShowDialog(this);
        }

        private void notepadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var np = new NotepadForm { MdiParent = this };
            np.ApplyDarkMode(globalDarkMode);
            np.Show();

        }
    }
}
