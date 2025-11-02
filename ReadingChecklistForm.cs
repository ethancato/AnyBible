using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Core_Bible
{
    public partial class ReadingChecklistForm : Form
    {
        // --- REQUIRED CONTROLS (add these in the Designer) ---
        // ComboBox        rcTranslationCombo   (DropDownStyle = DropDownList)
        // ListBox         rcBooksList
        // CheckedListBox  rcChaptersCheckedList (CheckOnClick = true)
        // Button          rcMarkAllButton
        // Button          rcClearAllButton
        // Button          rcRefreshButton
        // Button          rcResetButton
        // Button          rcCloseButton
        // Label           rcCounterLabel
        // ProgressBar     rcProgressBar
        // Label           rcPercentLabel

        private string biblesDir;
        private XmlDocument bibleXml;
        private string currentTranslationName = "";
        private bool _suppressEvents = false;          // prevent handlers while populating
        private bool _shownCongratsAllThisSession = false;
        private bool _shownWelcomeNTOnceThisSession = false;

        // Persisted as semicolon-separated keys: "Translation|Book|Chapter"
        private Dictionary<string, bool> readingProgress =
            new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        // 66-book canonical names (book @number is 1..66)
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

        public void ApplyDarkMode(bool enabled)
        {
            Color back = enabled ? Color.FromArgb(34, 40, 49) : SystemColors.Control;
            Color fore = enabled ? Color.FromArgb(230, 230, 230) : SystemColors.ControlText;

            this.BackColor = back;
            this.ForeColor = fore;

            for (int i = 0; i < this.Controls.Count; i++)
            {
                Control c = this.Controls[i];
                try { c.BackColor = back; } catch { }
                try { c.ForeColor = fore; } catch { }
            }

            // ProgressBar color (works when VisualStyles are off)
            if (this.rcProgressBar != null)
            {
                this.rcProgressBar.ForeColor = enabled
                    ? Color.FromArgb(255, 135, 40)  // dark mode bar color
                    : Color.FromArgb(0, 120, 215);  // light/default bar color
            }
        }

        protected override void OnShown(System.EventArgs e)
        {
            base.OnShown(e);
            // initialize to current global state if you have it; otherwise set light by default
            ApplyDarkMode(false);
        }


        public ReadingChecklistForm()
        {
            InitializeComponent();

            // Prepare Bible folder
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            biblesDir = baseDir.TrimEnd('\\') + "\\Bibles";
            if (!Directory.Exists(biblesDir)) Directory.CreateDirectory(biblesDir);

            // Hook events (once)
            rcTranslationCombo.SelectedIndexChanged += rcTranslationCombo_SelectedIndexChanged;
            rcBooksList.SelectedIndexChanged += rcBooksList_SelectedIndexChanged;
            rcChaptersCheckedList.ItemCheck += rcChaptersCheckedList_ItemCheck;
            rcChaptersCheckedList.SelectedIndexChanged += rcChaptersCheckedList_SelectedIndexChanged; // NT welcome
            rcMarkAllButton.Click += rcMarkAllButton_Click;
            rcClearAllButton.Click += rcClearAllButton_Click;
            rcRefreshButton.Click += rcRefreshButton_Click;
            rcResetButton.Click += rcResetButton_Click;

            // Defer population until handle exists
            this.Load += ReadingChecklistForm_Load;
        }

        private void ReadingChecklistForm_Load(object sender, EventArgs e)
        {
            LoadReadingProgress();
            LoadTranslationsIntoCombo();

            // Preselect default translation if present
            string def = Properties.Settings.Default.DefaultBible;
            if (!IsNullOrWhiteSpace(def))
            {
                int idx = FindComboIndex(rcTranslationCombo, def);
                if (idx >= 0) rcTranslationCombo.SelectedIndex = idx;
            }
            if (rcTranslationCombo.SelectedIndex < 0 && rcTranslationCombo.Items.Count > 0)
                rcTranslationCombo.SelectedIndex = 0;

            UpdateProgressUI();
        }

        // ===================== UI EVENTS =====================

        private void rcRefreshButton_Click(object sender, EventArgs e)
        {
            LoadReadingProgress();
            LoadTranslationsIntoCombo();

            if (rcTranslationCombo.Items.Count > 0 && rcTranslationCombo.SelectedIndex < 0)
                rcTranslationCombo.SelectedIndex = 0;

            UpdateProgressUI();
        }

        private void rcResetButton_Click(object sender, EventArgs e)
        {
            DialogResult d1 = MessageBox.Show(
                "This will clear all chapter progress for every translation. Continue?",
                "Reset Progress", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d1 != DialogResult.Yes) return;

            DialogResult d2 = MessageBox.Show(
                "Are you absolutely sure? This cannot be undone.",
                "Confirm Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (d2 != DialogResult.Yes) return;

            readingProgress.Clear();
            SaveReadingProgress();

            // Refresh current lists
            LoadChaptersForSelectedBook();
            UpdateProgressUI();

            // Allow congrats/welcome again in this session
            _shownCongratsAllThisSession = false;
            _shownWelcomeNTOnceThisSession = false;

            MessageBox.Show("Reading progress has been reset.", "Reset Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void rcTranslationCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;
            if (rcTranslationCombo.SelectedItem == null) return;

            string tName = rcTranslationCombo.SelectedItem.ToString();
            string xmlPath = biblesDir + "\\" + tName + ".xml";
            if (!File.Exists(xmlPath))
            {
                MessageBox.Show("Bible file not found: " + xmlPath, "Missing File");
                rcBooksList.Items.Clear();
                rcChaptersCheckedList.Items.Clear();
                bibleXml = null;
                currentTranslationName = "";
                UpdateProgressUI();
                return;
            }

            try
            {
                bibleXml = new XmlDocument();
                bibleXml.Load(xmlPath);

                string xmlTrans = "";
                if (bibleXml.DocumentElement != null && bibleXml.DocumentElement.Attributes["translation"] != null)
                    xmlTrans = bibleXml.DocumentElement.Attributes["translation"].Value;

                // Prefer XML translation attribute if present
                currentTranslationName = IsNullOrWhiteSpace(xmlTrans) ? tName : xmlTrans;

                PopulateBooks();

                // Auto-select first book
                if (rcBooksList.Items.Count > 0)
                {
                    _suppressEvents = true;
                    rcBooksList.SelectedIndex = 0;
                    _suppressEvents = false;
                    LoadChaptersForSelectedBook();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Bible: " + ex.Message, "Error");
                bibleXml = null;
                currentTranslationName = "";
            }

            UpdateProgressUI();
        }

        private void rcBooksList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;
            LoadChaptersForSelectedBook();
            UpdateProgressUI();
        }

        // Welcome to NT when user selects Matthew 1 (selection, not just check)
        private void rcChaptersCheckedList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;
            if (_shownWelcomeNTOnceThisSession) return;
            if (rcBooksList.SelectedItem == null || rcChaptersCheckedList.SelectedItem == null) return;

            string book = rcBooksList.SelectedItem.ToString();
            string display = rcChaptersCheckedList.SelectedItem.ToString(); // "Chapter N"
            string chNum = ExtractChapterNumber(display);

            if (string.Compare(book, "Matthew", true) == 0 && chNum == "1")
            {
                _shownWelcomeNTOnceThisSession = true;
                MessageBox.Show("Welcome to the New Testament.", "New Testament",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rcChaptersCheckedList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_suppressEvents) return;
            if (rcBooksList.SelectedItem == null) return;

            string selBook = rcBooksList.SelectedItem.ToString();
            string display = rcChaptersCheckedList.Items[e.Index].ToString(); // "Chapter N"
            string chNum = ExtractChapterNumber(display);
            string key = MakeKey(currentTranslationName, selBook, chNum);

            bool wasChecked = readingProgress.ContainsKey(key) && readingProgress[key];
            bool willBeChecked = (e.NewValue == CheckState.Checked);

            // Update dictionary according to new state
            if (willBeChecked)
            {
                if (!readingProgress.ContainsKey(key)) readingProgress.Add(key, true);
                else readingProgress[key] = true;
            }
            else
            {
                if (readingProgress.ContainsKey(key)) readingProgress.Remove(key);
            }

            SaveReadingProgress();

            // After commit, update UI and possibly show messages
            MethodInvoker postCommit = delegate
            {
                UpdateProgressUI();

                // If we just newly completed a chapter (transition from unchecked -> checked),
                // check for book completion and entire Bible completion.
                if (!wasChecked && willBeChecked)
                {
                    if (IsBookComplete(selBook))
                    {
                        MessageBox.Show("You've completed the book of " + selBook + ".",
                            "Book Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    if (IsBibleComplete() && !_shownCongratsAllThisSession)
                    {
                        _shownCongratsAllThisSession = true;
                        MessageBox.Show("Congratulations! You've completed reading the entire Bible.",
                            "All Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };

            if (this.IsHandleCreated) this.BeginInvoke(postCommit);
            else postCommit();
        }

        private void rcMarkAllButton_Click(object sender, EventArgs e)
        {
            if (rcBooksList.SelectedItem == null) return;
            string selBook = rcBooksList.SelectedItem.ToString();

            _suppressEvents = true;
            for (int i = 0; i < rcChaptersCheckedList.Items.Count; i++)
            {
                string display = rcChaptersCheckedList.Items[i].ToString();
                string chNum = ExtractChapterNumber(display);
                string key = MakeKey(currentTranslationName, selBook, chNum);

                if (!readingProgress.ContainsKey(key)) readingProgress.Add(key, true);
                else readingProgress[key] = true;

                rcChaptersCheckedList.SetItemChecked(i, true);
            }
            _suppressEvents = false;

            SaveReadingProgress();
            UpdateProgressUI();

            // Feedback
            if (IsBookComplete(selBook))
            {
                MessageBox.Show("You've completed the book of " + selBook + ".",
                    "Book Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (IsBibleComplete() && !_shownCongratsAllThisSession)
            {
                _shownCongratsAllThisSession = true;
                MessageBox.Show("Congratulations! You've completed reading the entire Bible.",
                    "All Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void rcClearAllButton_Click(object sender, EventArgs e)
        {
            if (rcBooksList.SelectedItem == null) return;
            string selBook = rcBooksList.SelectedItem.ToString();

            _suppressEvents = true;
            for (int i = 0; i < rcChaptersCheckedList.Items.Count; i++)
            {
                string display = rcChaptersCheckedList.Items[i].ToString();
                string chNum = ExtractChapterNumber(display);
                string key = MakeKey(currentTranslationName, selBook, chNum);

                if (readingProgress.ContainsKey(key)) readingProgress.Remove(key);
                rcChaptersCheckedList.SetItemChecked(i, false);
            }
            _suppressEvents = false;

            SaveReadingProgress();
            UpdateProgressUI();
        }

        // ===================== POPULATION =====================

        private void LoadTranslationsIntoCombo()
        {
            string keep = rcTranslationCombo.SelectedItem == null ? "" : rcTranslationCombo.SelectedItem.ToString();

            _suppressEvents = true;
            rcTranslationCombo.Items.Clear();

            string[] files = Directory.GetFiles(biblesDir, "*.xml");
            for (int i = 0; i < files.Length; i++)
            {
                string name = Path.GetFileNameWithoutExtension(files[i]);
                rcTranslationCombo.Items.Add(name);
            }

            if (keep.Length > 0)
            {
                int idx = FindComboIndex(rcTranslationCombo, keep);
                if (idx >= 0) rcTranslationCombo.SelectedIndex = idx;
            }
            _suppressEvents = false;
        }

        private void PopulateBooks()
        {
            _suppressEvents = true;
            rcBooksList.Items.Clear();
            rcChaptersCheckedList.Items.Clear();
            _suppressEvents = false;

            if (bibleXml == null || bibleXml.DocumentElement == null) return;

            XmlNodeList testaments = bibleXml.SelectNodes("/bible/testament");
            if (testaments == null) return;

            for (int ti = 0; ti < testaments.Count; ti++)
            {
                XmlNode testament = testaments[ti];
                XmlNodeList books = testament.SelectNodes("book");
                for (int bi = 0; bi < books.Count; bi++)
                {
                    XmlNode book = books[bi];
                    string numStr = (book.Attributes["number"] != null) ? book.Attributes["number"].Value : "";
                    int bnum = 0;
                    int.TryParse(numStr, out bnum);
                    string bname = (bnum >= 1 && bnum <= BookNames.Length) ? BookNames[bnum - 1] : "Book " + numStr;

                    if (!ListBoxContains(rcBooksList, bname))
                        rcBooksList.Items.Add(bname);
                }
            }
        }

        private void LoadChaptersForSelectedBook()
        {
            _suppressEvents = true;
            rcChaptersCheckedList.Items.Clear();

            if (bibleXml == null || rcBooksList.SelectedItem == null)
            {
                _suppressEvents = false;
                return;
            }

            string selBookName = rcBooksList.SelectedItem.ToString();
            int bnum = FindBookNumberByName(selBookName);
            if (bnum <= 0)
            {
                _suppressEvents = false;
                return;
            }

            XmlNode bookNode = bibleXml.SelectSingleNode("//book[@number='" + bnum.ToString() + "']");
            if (bookNode == null)
            {
                _suppressEvents = false;
                return;
            }

            XmlNodeList chapters = bookNode.SelectNodes("chapter");
            for (int i = 0; i < chapters.Count; i++)
            {
                XmlNode ch = chapters[i];
                string chNum = (ch.Attributes["number"] != null) ? ch.Attributes["number"].Value : (i + 1).ToString();
                string display = "Chapter " + chNum;
                int idx = rcChaptersCheckedList.Items.Add(display);

                string key = MakeKey(currentTranslationName, selBookName, chNum);
                if (readingProgress.ContainsKey(key) && readingProgress[key])
                    rcChaptersCheckedList.SetItemChecked(idx, true);
            }
            _suppressEvents = false;
        }

        // ===================== PROGRESS / CHECKS =====================

        private void UpdateProgressUI()
        {
            int total = CountTotalChaptersForCurrentTranslation();
            int read = CountReadChaptersForCurrentTranslation();

            if (total < 1) total = 1; // guard

            rcCounterLabel.Text = "Read: " + read + " / " + total;

            rcProgressBar.Minimum = 0;
            rcProgressBar.Maximum = total;
            rcProgressBar.Value = (read <= total) ? read : total;

            double pct = (read * 100.0) / total;
            rcPercentLabel.Text = pct.ToString("0.0") + "%";
        }

        private int CountTotalChaptersForCurrentTranslation()
        {
            if (bibleXml == null || bibleXml.DocumentElement == null) return 0;
            XmlNodeList chapters = bibleXml.SelectNodes("//chapter");
            return (chapters != null) ? chapters.Count : 0;
        }

        private int CountReadChaptersForCurrentTranslation()
        {
            if (bibleXml == null || bibleXml.DocumentElement == null) return 0;
            int read = 0;

            XmlNodeList books = bibleXml.SelectNodes("//book");
            if (books == null) return 0;

            for (int i = 0; i < books.Count; i++)
            {
                XmlNode book = books[i];
                string numStr = (book.Attributes["number"] != null) ? book.Attributes["number"].Value : "";
                int bnum = 0; int.TryParse(numStr, out bnum);
                string bookName = MapBookNumberToName(bnum);

                XmlNodeList chapters = book.SelectNodes("chapter");
                for (int c = 0; c < chapters.Count; c++)
                {
                    XmlNode ch = chapters[c];
                    string chNum = (ch.Attributes["number"] != null) ? ch.Attributes["number"].Value : (c + 1).ToString();
                    string key = MakeKey(currentTranslationName, bookName, chNum);
                    if (readingProgress.ContainsKey(key) && readingProgress[key]) read++;
                }
            }
            return read;
        }

        private bool IsBookComplete(string bookName)
        {
            if (bibleXml == null || bibleXml.DocumentElement == null) return false;

            int bnum = FindBookNumberByName(bookName);
            if (bnum <= 0) return false;

            XmlNode book = bibleXml.SelectSingleNode("//book[@number='" + bnum.ToString() + "']");
            if (book == null) return false;

            XmlNodeList chapters = book.SelectNodes("chapter");
            if (chapters == null || chapters.Count == 0) return false;

            for (int i = 0; i < chapters.Count; i++)
            {
                XmlNode ch = chapters[i];
                string chNum = (ch.Attributes["number"] != null) ? ch.Attributes["number"].Value : (i + 1).ToString();
                string key = MakeKey(currentTranslationName, bookName, chNum);
                if (!(readingProgress.ContainsKey(key) && readingProgress[key]))
                    return false;
            }
            return true;
        }

        private bool IsBibleComplete()
        {
            if (bibleXml == null || bibleXml.DocumentElement == null) return false;

            XmlNodeList books = bibleXml.SelectNodes("//book");
            if (books == null || books.Count == 0) return false;

            for (int i = 0; i < books.Count; i++)
            {
                XmlNode book = books[i];
                string numStr = (book.Attributes["number"] != null) ? book.Attributes["number"].Value : "";
                int bnum = 0; int.TryParse(numStr, out bnum);
                string bookName = MapBookNumberToName(bnum);

                XmlNodeList chapters = book.SelectNodes("chapter");
                if (chapters == null || chapters.Count == 0) return false;

                for (int c = 0; c < chapters.Count; c++)
                {
                    XmlNode ch = chapters[c];
                    string chNum = (ch.Attributes["number"] != null) ? ch.Attributes["number"].Value : (c + 1).ToString();
                    string key = MakeKey(currentTranslationName, bookName, chNum);
                    if (!(readingProgress.ContainsKey(key) && readingProgress[key]))
                        return false;
                }
            }
            return true;
        }

        // ===================== PERSISTENCE =====================

        private void LoadReadingProgress()
        {
            readingProgress.Clear();
            string raw = Properties.Settings.Default.ReadingProgress;
            if (IsNullOrWhiteSpace(raw)) return;

            string[] parts = raw.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                if (!readingProgress.ContainsKey(parts[i]))
                    readingProgress.Add(parts[i], true);
            }
        }

        private void SaveReadingProgress()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (string k in readingProgress.Keys)
            {
                if (readingProgress[k])
                {
                    if (sb.Length > 0) sb.Append(';');
                    sb.Append(k);
                }
            }
            Properties.Settings.Default.ReadingProgress = sb.ToString();
            Properties.Settings.Default.Save();
        }

        // ===================== HELPERS =====================

        private int FindComboIndex(ComboBox combo, string value)
        {
            for (int i = 0; i < combo.Items.Count; i++)
                if (string.Compare(combo.Items[i].ToString(), value, true) == 0) return i;
            return -1;
        }

        private bool ListBoxContains(ListBox lb, string text)
        {
            for (int i = 0; i < lb.Items.Count; i++)
                if (string.Compare(lb.Items[i].ToString(), text, true) == 0) return true;
            return false;
        }

        private int FindBookNumberByName(string bookName)
        {
            for (int i = 0; i < BookNames.Length; i++)
                if (string.Compare(BookNames[i], bookName, true) == 0) return i + 1;
            return -1;
        }

        private string MapBookNumberToName(int bnum)
        {
            if (bnum >= 1 && bnum <= BookNames.Length) return BookNames[bnum - 1];
            return "Book " + bnum.ToString();
        }

        private string ExtractChapterNumber(string display)
        {
            // "Chapter 12" -> "12"
            if (display == null) return "1";
            int idx = display.LastIndexOf(' ');
            if (idx >= 0 && idx < display.Length - 1)
                return display.Substring(idx + 1);
            return display;
        }

        private string MakeKey(string translation, string bookName, string chapterNumber)
        {
            if (translation == null) translation = "";
            if (bookName == null) bookName = "";
            if (chapterNumber == null) chapterNumber = "";
            return translation.Trim() + "|" + bookName.Trim() + "|" + chapterNumber.Trim();
        }

        // .NET 2.0-safe replacement
        private static bool IsNullOrWhiteSpace(string s)
        {
            if (s == null) return true;
            for (int i = 0; i < s.Length; i++)
                if (!char.IsWhiteSpace(s[i])) return false;
            return true;
        }
    }
}
