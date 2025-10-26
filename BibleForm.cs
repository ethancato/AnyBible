using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Core_Bible
{
    using System.Runtime.InteropServices;

    [ComImport, Guid("0002DF05-0000-0000-C000-000000000046"),
     InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
     TypeLibType(TypeLibTypeFlags.FHidden)]
    interface IWebBrowser2
    {
        [DispId(100)] void GoBack();
        [DispId(101)] void GoForward();
        [DispId(102)] void GoHome();
        [DispId(103)] void GoSearch();
        [DispId(104)]
        void Navigate([In] string Url, [In] ref object Flags,
            [In] ref object TargetFrameName, [In] ref object PostData,
            [In] ref object Headers);
        [DispId(300)] object Document { get; }
        [DispId(63)]
        void ExecWB(int cmdID, int cmdexecopt,
            ref object pvaIn, ref object pvaOut);
    }

    public partial class BibleForm : Form
    {
        private XmlDocument bibleXml;
        private Random random = new Random();
        private bool restoredPage = false;
        private int currentZoom = 100;

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

        public BibleForm()
        {
            InitializeComponent();

            this.Load += BibleForm_Load;
            this.FormClosing += BibleForm_FormClosing;

            treeViewBible.AfterSelect += TreeViewBible_AfterSelect;
            translationCombo.SelectedIndexChanged += translationCombo_SelectedIndexChanged;
            darkModeCheckbox.CheckedChanged += darkModeCheckbox_CheckedChanged;
            inlineModeCheckbox.CheckedChanged += inlineModeCheckbox_CheckedChanged;
            zoomCombo.SelectedIndexChanged += zoomCombo_SelectedIndexChanged;
            webBrowserContent.DocumentCompleted += webBrowserContent_DocumentCompleted;
            votdButton.Click += votdButton_Click;
            cotdButton.Click += cotdButton_Click;

            zoomCombo.Items.AddRange(new object[] { "50%", "75%", "100%", "125%", "150%", "175%", "200%" });

            LoadActiveTranslations();
        }

        private void BibleForm_Load(object sender, EventArgs e)
        {
            // Restore settings
            currentZoom = Properties.Settings.Default.LastZoomLevel > 0 ? Properties.Settings.Default.LastZoomLevel : 100;
            darkModeCheckbox.Checked = Properties.Settings.Default.LastDarkMode;
            inlineModeCheckbox.Checked = Properties.Settings.Default.LastInlineMode;

            string zoomStr = currentZoom + "%";
            if (zoomCombo.Items.Contains(zoomStr)) zoomCombo.SelectedItem = zoomStr;
            else zoomCombo.SelectedItem = "100%";

            LoadDefaultBible();
            // delay restoring last page until after load
            this.BeginInvoke((MethodInvoker)delegate { RestoreLastPage(); });
        }

        private void BibleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.LastZoomLevel = currentZoom;
            Properties.Settings.Default.LastDarkMode = darkModeCheckbox.Checked;
            Properties.Settings.Default.LastInlineMode = inlineModeCheckbox.Checked;
            Properties.Settings.Default.Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Settings s = new Settings())
            {
                if (s.ShowDialog() == DialogResult.OK)
                {
                    LoadActiveTranslations();
                    string defaultBible = Properties.Settings.Default.DefaultBible;
                    if (defaultBible != null && defaultBible != "")
                    {
                        string path = Application.StartupPath + "\\Bibles\\" + defaultBible + ".xml";
                        if (File.Exists(path))
                        {
                            LoadBibleXml(path);
                            PopulateTreeView();
                        }
                    }
                }
            }
        }

        private void LoadDefaultBible()
        {
            string def = Properties.Settings.Default.DefaultBible;
            if (def == null || def == "") return;
            string path = Application.StartupPath + "\\Bibles\\" + def + ".xml";
            if (File.Exists(path))
            {
                LoadBibleXml(path);
                PopulateTreeView();
            }
        }

        private void RestoreLastPage()
        {
            if (restoredPage) return;

            string last = Properties.Settings.Default.LastOpenedPage;
            if (last == null || last == "") return;

            string[] parts = last.Split('|');
            if (parts.Length != 3) return;

            string bibleName = parts[0];
            string bookName = parts[1];
            string chapterNumber = parts[2];
            string path = Application.StartupPath + "\\Bibles\\" + bibleName + ".xml";

            if (!File.Exists(path)) return;

            try
            {
                LoadBibleXml(path);
                PopulateTreeView();

                TreeNode node = FindChapterNode(bookName, chapterNumber);
                if (node != null)
                    ExpandAndSelectNode(node);

                restoredPage = true;
            }
            catch { }
        }

        private void LoadBibleXml(string path)
        {
            bibleXml = new XmlDocument();
            bibleXml.Load(path);
        }

        private void PopulateTreeView()
        {
            treeViewBible.Nodes.Clear();

            if (bibleXml == null || bibleXml.DocumentElement == null)
                return;

            string trans = bibleXml.DocumentElement.Attributes["translation"]?.Value ?? "Unknown";
            TreeNode transNode = new TreeNode(trans);

            XmlNodeList testaments = bibleXml.SelectNodes("/bible/testament");
            foreach (XmlNode testament in testaments)
            {
                TreeNode testNode = new TreeNode(testament.Attributes["name"]?.Value ?? "Unknown Testament");

                foreach (XmlNode book in testament.SelectNodes("book"))
                {
                    int bookNum = int.Parse(book.Attributes["number"].Value);
                    string bookName = (bookNum >= 1 && bookNum <= BookNames.Length)
                        ? BookNames[bookNum - 1] : "Unknown Book";

                    TreeNode bookNode = new TreeNode(bookName);

                    foreach (XmlNode chapter in book.SelectNodes("chapter"))
                    {
                        string cnum = chapter.Attributes["number"]?.Value ?? "Unknown";
                        TreeNode cNode = new TreeNode("Chapter " + cnum);
                        cNode.Tag = new ChapterData(chapter, bookName);
                        bookNode.Nodes.Add(cNode);
                    }

                    testNode.Nodes.Add(bookNode);
                }

                transNode.Nodes.Add(testNode);
            }

            treeViewBible.Nodes.Add(transNode);
        }

        private void TreeViewBible_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is ChapterData data)
            {
                string html = GenerateChapterHtml(data.ChapterNode, data.BookName);
                webBrowserContent.DocumentText = html;

                string bibleName = bibleXml.DocumentElement.Attributes["translation"]?.Value ?? "";
                string chapterNum = data.ChapterNode.Attributes["number"]?.Value ?? "1";

                if (bibleName != "")
                {
                    Properties.Settings.Default.LastOpenedPage = bibleName + "|" + data.BookName + "|" + chapterNum;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private string GenerateChapterHtml(XmlNode chapterNode, string bookName)
        {
            string cnum = chapterNode.Attributes["number"]?.Value ?? "Unknown";
            bool dark = darkModeCheckbox.Checked;
            bool inline = inlineModeCheckbox.Checked;

            string bg = dark ? "rgb(27,30,33)" : "white";
            string text = dark ? "rgb(230,230,230)" : "black";
            string font = dark ? "Helvetica" : "Times New Roman";
            string type = dark ? "sans-serif" : "serif";

            StringWriter sw = new StringWriter();
            sw.WriteLine("<!DOCTYPE html><html><head><meta charset='UTF-8'>");
            sw.WriteLine("<style>");
            sw.WriteLine("body { background-color:" + bg + "; color:" + text + "; margin:20px; font-family:'" + font + "', " + type + "; line-height:1.6; }");
            sw.WriteLine(".inline p { display:inline; margin:0; }");
            sw.WriteLine(".inline sup { font-size:smaller; vertical-align:super; }");
            sw.WriteLine("</style></head><body>");
            sw.WriteLine("<h1>" + bookName + " " + cnum + "</h1>");

            if (inline)
            {
                sw.Write("<div class='inline'>");
                foreach (XmlNode v in chapterNode.SelectNodes("verse"))
                {
                    string num = v.Attributes["number"]?.Value ?? "";
                    string txt = v.InnerText ?? "";
                    sw.Write("<p><sup>" + num + "</sup> " + txt + " </p>");
                }
                sw.Write("</div>");
            }
            else
            {
                foreach (XmlNode v in chapterNode.SelectNodes("verse"))
                {
                    string num = v.Attributes["number"]?.Value ?? "";
                    string txt = v.InnerText ?? "";
                    sw.WriteLine("<p><strong>" + num + "</strong> " + txt + "</p>");
                }
            }

            sw.WriteLine("</body></html>");
            return sw.ToString();
        }

        private void votdButton_Click(object sender, EventArgs e)
        {
            if (bibleXml == null) return;
            XmlNodeList verses = bibleXml.SelectNodes("//verse");
            if (verses == null || verses.Count == 0) return;

            XmlNode verse = verses[random.Next(verses.Count)];
            XmlNode chapter = verse.ParentNode;
            XmlNode book = chapter.ParentNode;

            string vNum = verse.Attributes["number"]?.Value ?? "?";
            string vText = verse.InnerText ?? "";
            string cNum = chapter.Attributes["number"]?.Value ?? "?";
            int bNum = int.Parse(book.Attributes["number"].Value);
            string bName = (bNum >= 1 && bNum <= BookNames.Length) ? BookNames[bNum - 1] : "Unknown";

            TreeNode node = FindChapterNode(bName, cNum);
            if (node != null) ExpandAndSelectNode(node);

            MessageBox.Show(bName + " " + cNum + ":" + vNum + "\n\n" + vText, "Verse of the Day");
        }

        private void cotdButton_Click(object sender, EventArgs e)
        {
            if (bibleXml == null) return;
            XmlNodeList chapters = bibleXml.SelectNodes("//chapter");
            if (chapters == null || chapters.Count == 0) return;

            XmlNode chapter = chapters[random.Next(chapters.Count)];
            XmlNode book = chapter.ParentNode;
            int bNum = int.Parse(book.Attributes["number"].Value);
            string bName = (bNum >= 1 && bNum <= BookNames.Length) ? BookNames[bNum - 1] : "Unknown";
            string cNum = chapter.Attributes["number"]?.Value ?? "?";

            TreeNode node = FindChapterNode(bName, cNum);
            if (node != null) ExpandAndSelectNode(node);
        }

        private void zoomCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = zoomCombo.SelectedItem.ToString().Replace("%", "");
            int zoom;
            if (int.TryParse(selected, out zoom))
            {
                currentZoom = zoom;
                ApplyZoom();
            }
        }

        private void ApplyZoom()
        {
            try
            {
                if (webBrowserContent.Document == null || webBrowserContent.Document.Body == null)
                    return;
                string script = "document.body.style.zoom='" + currentZoom + "%';";
                webBrowserContent.Document.InvokeScript("execScript", new object[] { script, "JavaScript" });
            }
            catch { }
        }

        private void webBrowserContent_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ApplyZoom();
        }

        private void darkModeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ApplyDarkMode(this, darkModeCheckbox.Checked);
            if (treeViewBible.SelectedNode != null && treeViewBible.SelectedNode.Tag is ChapterData data)
                webBrowserContent.DocumentText = GenerateChapterHtml(data.ChapterNode, data.BookName);
        }

        private void inlineModeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LastInlineMode = inlineModeCheckbox.Checked;
            Properties.Settings.Default.Save();

            if (treeViewBible.SelectedNode != null && treeViewBible.SelectedNode.Tag is ChapterData data)
                webBrowserContent.DocumentText = GenerateChapterHtml(data.ChapterNode, data.BookName);
        }

        private void ApplyDarkMode(Control parent, bool enabled)
        {
            Color bg = enabled ? Color.FromArgb(34, 40, 49) : SystemColors.Control;
            Color fg = enabled ? Color.FromArgb(230, 230, 230) : SystemColors.ControlText;

            parent.BackColor = bg;
            parent.ForeColor = fg;

            foreach (Control c in parent.Controls)
                ApplyDarkMode(c, enabled);
        }

        private void LoadActiveTranslations()
        {
            translationCombo.Items.Clear();
            translationCombo.SelectedIndex = -1;

            string dir = Application.StartupPath + "\\Bibles";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string[] active = {
                Properties.Settings.Default.Bible1,
                Properties.Settings.Default.Bible2,
                Properties.Settings.Default.Bible3,
                Properties.Settings.Default.Bible4,
                Properties.Settings.Default.Bible5
            };

            string def = Properties.Settings.Default.DefaultBible;
            for (int i = 0; i < active.Length; i++)
            {
                if (active[i] != null && active[i] != "")
                {
                    string path = dir + "\\" + active[i] + ".xml";
                    if (File.Exists(path))
                    {
                        string item = active[i];
                        if (string.Compare(item, def, true) == 0)
                            item += " (Default)";
                        translationCombo.Items.Add(item);
                    }
                }
            }

            for (int i = 0; i < translationCombo.Items.Count; i++)
            {
                string t = translationCombo.Items[i].ToString();
                string pure = t.Replace(" (Default)", "");
                if (string.Compare(pure, def, true) == 0)
                {
                    translationCombo.SelectedIndex = i;
                    break;
                }
            }
        }

        private void translationCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (translationCombo.SelectedItem == null) return;

            string selected = translationCombo.SelectedItem.ToString().Replace(" (Default)", "");
            string path = Application.StartupPath + "\\Bibles\\" + selected + ".xml";
            if (File.Exists(path))
            {
                LoadBibleXml(path);
                PopulateTreeView();
            }
            else MessageBox.Show("Bible file not found: " + path, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private TreeNode FindChapterNode(string bookName, string cNum)
        {
            foreach (TreeNode root in treeViewBible.Nodes)
                foreach (TreeNode t in root.Nodes)
                    foreach (TreeNode b in t.Nodes)
                        if (b.Text.Equals(bookName, StringComparison.OrdinalIgnoreCase))
                            foreach (TreeNode c in b.Nodes)
                                if (c.Text.Equals("Chapter " + cNum, StringComparison.OrdinalIgnoreCase))
                                    return c;
            return null;
        }

        private void ExpandAndSelectNode(TreeNode n)
        {
            n.Parent?.Expand();
            n.Parent?.Parent?.Expand();
            n.EnsureVisible();
            treeViewBible.SelectedNode = n;
            n.Expand();
        }

        public class ChapterData
        {
            public XmlNode ChapterNode;
            public string BookName;
            public ChapterData(XmlNode node, string name)
            {
                ChapterNode = node;
                BookName = name;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();

            aboutBox.ShowDialog();
        }

        private void readingChecklistButton_Click(object sender, EventArgs e)
        {
            using (var f = new ReadingChecklistForm())
                f.ShowDialog(this);
        }
    }
}
