using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Core_Bible
{
    public partial class PassageChildForm : Form
    {
        // Designer must already contain: WebBrowser webBrowserContent

        private XmlDocument _bibleXml;
        private readonly Random _rand = new Random();

        private string _translationName = "";
        private string _currentBookName = "";
        private string _currentChapterNumber = "1";
        private bool _darkMode = false;
        private bool _inlineMode = false;

        // We scale via font-size so wrapping stays correct.
        private int _zoomPercent = 100; // 50..200

        // Robust priming
        private bool _browserPrimed = false;
        private string _pendingHtml = null;

        public PassageChildForm()
        {
            InitializeComponent();

            if (webBrowserContent != null)
            {
                webBrowserContent.ScriptErrorsSuppressed = true;
                webBrowserContent.AllowWebBrowserDrop = false;
                webBrowserContent.IsWebBrowserContextMenuEnabled = false;
                try { webBrowserContent.Dock = DockStyle.Fill; } catch { }

                webBrowserContent.DocumentCompleted += WebBrowser_DocumentCompleted;

                // Prime immediately; we'll write after about:blank completes
                SafePrimeBrowser();
            }

            // restore last zoom
            int z = Properties.Settings.Default.LastZoomLevel;
            if (z < 50 || z > 200) z = 100;
            _zoomPercent = z;

            this.Load += PassageChildForm_Load;
            this.Shown += PassageChildForm_Shown;
        }

        private void PassageChildForm_Load(object sender, EventArgs e)
        {
            // No heavy work here. Priming already started in ctor.
        }

        private void PassageChildForm_Shown(object sender, EventArgs e)
        {
            // If prime didn’t happen for any reason, do it now.
            SafePrimeBrowser();
        }

        private void SafePrimeBrowser()
        {
            if (webBrowserContent == null || webBrowserContent.IsDisposed) return;
            // Only prime once
            if (!_browserPrimed)
                try { webBrowserContent.Navigate("about:blank"); } catch { }
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Mark primed on about:blank complete
            if (!_browserPrimed && e != null && e.Url != null &&
                e.Url.Scheme.Equals("about", StringComparison.OrdinalIgnoreCase))
            {
                _browserPrimed = true;

                // If we had pending HTML, write it now
                if (!string.IsNullOrEmpty(_pendingHtml))
                {
                    WriteHtmlIntoDocument(_pendingHtml);
                    _pendingHtml = null;
                }
                ApplyFontZoomToWebBrowser();
                return;
            }

            // For any later navigations (we always stay on about:blank), just re-apply zoom
            ApplyFontZoomToWebBrowser();
        }

        public XmlDocument BibleXml { get { return _bibleXml; } }
        public string TranslationName { get { return _translationName; } }
        public string CurrentBookName { get { return _currentBookName; } set { _currentBookName = value; } }
        public string CurrentChapterNumber { get { return _currentChapterNumber; } set { _currentChapterNumber = value; } }
        public int ZoomPercent { get { return _zoomPercent; } }

        public void ApplyGlobals(bool dark, bool inlineMode)
        {
            _darkMode = dark;
            _inlineMode = inlineMode;
            SafeRerender();
        }

        public bool LoadBibleByTranslationName(string translationName)
        {
            if (translationName == null) return false;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            string path = baseDir + "\\Bibles\\" + translationName + ".xml";
            if (!File.Exists(path)) return false;

            try
            {
                XmlDocument x = new XmlDocument();
                x.Load(path);
                _bibleXml = x;
                _translationName = translationName;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Force Genesis 1 (used by parent at startup).</summary>
        public void OpenGenesis1()
        {
            if (_bibleXml == null)
            {
                string def = Properties.Settings.Default.DefaultBible;
                if (!string.IsNullOrEmpty(def)) LoadBibleByTranslationName(def);
            }
            if (_bibleXml == null) return;

            _currentBookName = "Genesis";
            _currentChapterNumber = "1";
            RenderCurrentChapter();
        }

        public void RenderCurrentChapter()
        {
            if (_bibleXml == null)
            {
                string def = Properties.Settings.Default.DefaultBible;
                if (!string.IsNullOrEmpty(def)) LoadBibleByTranslationName(def);
            }
            if (_bibleXml == null) return;

            XmlNode chapNode = FindChapterNode(_currentBookName, _currentChapterNumber);
            if (chapNode == null)
            {
                _currentBookName = "Genesis";
                _currentChapterNumber = "1";
                chapNode = FindChapterNode(_currentBookName, _currentChapterNumber);
                if (chapNode == null) return;
            }

            string html = GenerateChapterHtml(chapNode, _currentBookName);
            RenderHtml(html);

            // Persist "Bible|Book|Chapter"
            Properties.Settings.Default.LastOpenedPage =
                _translationName + "|" + _currentBookName + "|" + _currentChapterNumber;
            Properties.Settings.Default.Save();
        }

        /// <summary>Render a multi-chapter passage on one page; keeps chapter headings in inline mode.</summary>
        public void RenderPassage(string bookName, int startChapter, int? startVerse, int endChapter, int? endVerse)
        {
            if (_bibleXml == null)
            {
                string def = Properties.Settings.Default.DefaultBible;
                if (!string.IsNullOrEmpty(def)) LoadBibleByTranslationName(def);
            }
            if (_bibleXml == null) return;

            if (endChapter < startChapter) { int t = endChapter; endChapter = startChapter; startChapter = t; }
            if (startChapter < 1) startChapter = 1;

            string titleRange = startChapter.ToString();
            if (startVerse.HasValue) titleRange += ":" + startVerse.Value.ToString();
            titleRange += "–" + endChapter.ToString();
            if (endVerse.HasValue) titleRange += ":" + endVerse.Value.ToString();

            string bodyColor = _darkMode ? "rgb(27,30,33)" : "white";
            string textColor = _darkMode ? "rgb(230,230,230)" : "black";
            string fontStack = _darkMode ? "Helvetica, Arial, sans-serif" : "'Times New Roman', serif";

            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html><html><head>");
            html.AppendLine("<meta http-equiv='X-UA-Compatible' content='IE=edge' />");
            html.AppendLine("<meta charset='UTF-8'>");
            html.AppendLine("<title>" + bookName + " " + titleRange + "</title>");
            html.AppendLine("<style>");
            html.AppendLine("html,body{margin:0;padding:0;}*{box-sizing:border-box;}");
            html.AppendLine("body{background-color:" + bodyColor + ";color:" + textColor + ";margin:20px;line-height:1.7;font-family:" + fontStack + ";white-space:normal;word-wrap:break-word;overflow-wrap:break-word;font-size:100%;}");
            html.AppendLine("#page{max-width:100%;overflow-x:hidden;}");
            html.AppendLine("h1,h2,h3{margin:0 0 8px 0;line-height:1.25;}");
            html.AppendLine(".inline p{display:inline;margin:0 .5em 0 0;white-space:normal;}");
            html.AppendLine(".inline sup{font-size:0.8em;line-height:1;vertical-align:super;}");
            html.AppendLine("</style>");
            html.AppendLine("</head><body><div id='page'>");
            html.AppendLine("<h1>" + bookName + "</h1>");
            html.AppendLine("<h2>" + titleRange + "</h2>");

            for (int ch = startChapter; ch <= endChapter; ch++)
            {
                XmlNode chapterNode = FindChapterNode(bookName, ch.ToString());
                if (chapterNode == null) break;

                int? fromV = (ch == startChapter) ? startVerse : null;
                int? toV = (ch == endChapter) ? endVerse : null;

                html.AppendLine("<h3>Chapter " + ch.ToString() + "</h3>");
                if (_inlineMode)
                {
                    html.AppendLine("<div class='inline'>");
                    ChapterToHtmlSegment(chapterNode, fromV, toV, html, inline: true);
                    html.AppendLine("</div>");
                }
                else
                {
                    ChapterToHtmlSegment(chapterNode, fromV, toV, html, inline: false);
                }
            }

            html.AppendLine("</div></body></html>");
            RenderHtml(html.ToString());

            // Persist a sensible “last position”: start of the range
            _currentBookName = bookName;
            _currentChapterNumber = startChapter.ToString();
            Properties.Settings.Default.LastOpenedPage = _translationName + "|" + _currentBookName + "|" + _currentChapterNumber;
            Properties.Settings.Default.Save();
        }

        // --- Robust HTML path ---
        private void RenderHtml(string html)
        {
            if (!_browserPrimed)
            {
                _pendingHtml = html; // will write on about:blank completion
                SafePrimeBrowser();
                return;
            }
            WriteHtmlIntoDocument(html);
        }

        private void WriteHtmlIntoDocument(string html)
        {
            if (webBrowserContent == null || webBrowserContent.IsDisposed) return;

            try
            {
                if (webBrowserContent.Document == null)
                {
                    // Ensure a document exists
                    webBrowserContent.Navigate("about:blank");
                    _pendingHtml = html;
                    return;
                }

                // New clean doc, then write
                webBrowserContent.Document.OpenNew(true);
                webBrowserContent.Document.Write(html);

                ApplyFontZoomToWebBrowser();
            }
            catch
            {
                // As a fallback, try DocumentText
                try
                {
                    webBrowserContent.DocumentText = html;
                }
                catch { /* swallow */ }
            }
        }

        private XmlNode FindChapterNode(string bookName, string chapterNum)
        {
            if (_bibleXml == null) return null;
            XmlNodeList books = _bibleXml.SelectNodes("//book");
            if (books == null) return null;

            for (int i = 0; i < books.Count; i++)
            {
                XmlNode book = books[i];
                string numStr = (book.Attributes != null && book.Attributes["number"] != null)
                                ? book.Attributes["number"].Value : "";
                int bnum = 0; int.TryParse(numStr, out bnum);
                string name = MapBookNumberToName(bnum);

                if (string.Compare(name, bookName, true) == 0)
                {
                    XmlNodeList chapters = book.SelectNodes("chapter");
                    for (int c = 0; c < chapters.Count; c++)
                    {
                        XmlNode ch = chapters[c];
                        string chNum = (ch.Attributes != null && ch.Attributes["number"] != null)
                                       ? ch.Attributes["number"].Value : (c + 1).ToString();
                        if (string.Compare(chNum, chapterNum, true) == 0) return ch;
                    }
                }
            }
            return null;
        }

        private static string MapBookNumberToName(int bnum)
        {
            string[] BookNames = {
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
            if (bnum >= 1 && bnum <= BookNames.Length) return BookNames[bnum - 1];
            return "Book " + bnum.ToString();
        }

        private void ChapterToHtmlSegment(XmlNode chapterNode, int? fromVerse, int? toVerse, StringBuilder html, bool inline)
        {
            XmlNodeList verses = chapterNode.SelectNodes("verse");
            if (verses == null || verses.Count == 0) return;

            int maxV = GetMaxVerseNumber(chapterNode);
            int startV = fromVerse.HasValue ? fromVerse.Value : 1;
            int endV = toVerse.HasValue ? toVerse.Value : maxV;
            if (endV < startV) { int t = endV; endV = startV; startV = t; }

            for (int i = 0; i < verses.Count; i++)
            {
                XmlNode v = verses[i];
                string vnStr = (v.Attributes != null && v.Attributes["number"] != null) ? v.Attributes["number"].Value : null;
                int vn = 0; int.TryParse(vnStr, out vn);
                if (vn == 0) vn = i + 1;

                if (vn < startV || vn > endV) continue;

                string vt = v.InnerText ?? "";

                if (inline)
                    html.Append("<p><sup>" + vn.ToString() + "</sup> " + vt + " </p>");
                else
                    html.Append("<p><strong>" + vn.ToString() + "</strong> " + vt + "</p>");
            }
        }

        private static int GetMaxVerseNumber(XmlNode chapterNode)
        {
            XmlNodeList verses = chapterNode.SelectNodes("verse");
            if (verses == null || verses.Count == 0) return 0;

            int max = 0;
            for (int i = 0; i < verses.Count; i++)
            {
                XmlNode v = verses[i];
                string vnStr = (v.Attributes != null && v.Attributes["number"] != null) ? v.Attributes["number"].Value : null;
                int vn = 0; int.TryParse(vnStr, out vn);
                if (vn == 0) vn = i + 1;
                if (vn > max) max = vn;
            }
            return max;
        }

        private string GenerateChapterHtml(XmlNode chapterNode, string bookName)
        {
            string chNum = (chapterNode.Attributes != null && chapterNode.Attributes["number"] != null)
                           ? chapterNode.Attributes["number"].Value : "1";

            string bodyColor = _darkMode ? "rgb(27,30,33)" : "white";
            string textColor = _darkMode ? "rgb(230,230,230)" : "black";
            string fontStack = _darkMode ? "Helvetica, Arial, sans-serif" : "'Times New Roman', serif";

            this.Text = bookName + " " + chNum + " - " + _translationName;

            var sw = new StringWriter();
            sw.WriteLine("<!DOCTYPE html>");
            sw.WriteLine("<html>");
            sw.WriteLine("<head>");
            sw.WriteLine("<meta http-equiv='X-UA-Compatible' content='IE=edge' />");
            sw.WriteLine("<meta charset='UTF-8'>");
            sw.WriteLine("<title>" + bookName + " " + chNum + "</title>");
            sw.WriteLine("<style>");
            sw.WriteLine("html,body{margin:0;padding:0;}*{box-sizing:border-box;}");
            sw.WriteLine("body{background-color:" + bodyColor + ";color:" + textColor + ";margin:20px;line-height:1.7;font-family:" + fontStack + ";white-space:normal;word-wrap:break-word;overflow-wrap:break-word;font-size:100%;}");
            sw.WriteLine("#page{max-width:100%;overflow-x:hidden;}");
            sw.WriteLine("h1,h2{margin:0 0 8px 0;line-height:1.25;}");
            sw.WriteLine(".inline p{display:inline;margin:0 .5em 0 0;white-space:normal;}");
            sw.WriteLine(".inline sup{font-size:0.8em;line-height:1;vertical-align:super;}");
            sw.WriteLine("</style>");
            sw.WriteLine("</head>");
            sw.WriteLine("<body><div id='page'>");
            sw.WriteLine("<h1>" + bookName + "</h1>");
            sw.WriteLine("<h2>Chapter " + chNum + "</h2>");

            if (_inlineMode)
            {
                sw.WriteLine("<div class='inline'>");
                XmlNodeList verses = chapterNode.SelectNodes("verse");
                for (int i = 0; i < verses.Count; i++)
                {
                    XmlNode v = verses[i];
                    string vn = (v.Attributes != null && v.Attributes["number"] != null) ? v.Attributes["number"].Value : "";
                    string vt = v.InnerText ?? "";
                    sw.Write("<p><sup>" + vn + "</sup> " + vt + " </p>");
                }
                sw.WriteLine("</div>");
            }
            else
            {
                XmlNodeList verses2 = chapterNode.SelectNodes("verse");
                for (int j = 0; j < verses2.Count; j++)
                {
                    XmlNode v2 = verses2[j];
                    string vn2 = (v2.Attributes != null && v2.Attributes["number"] != null) ? v2.Attributes["number"].Value : "";
                    string vt2 = v2.InnerText ?? "";
                    sw.WriteLine("<p><strong>" + vn2 + "</strong> " + vt2 + "</p>");
                }
            }

            sw.WriteLine("</div></body>");
            sw.WriteLine("</html>");
            return sw.ToString();
        }

        public void SetZoomPercent(int percent)
        {
            if (percent < 50) percent = 50;
            if (percent > 200) percent = 200;
            _zoomPercent = percent;

            Properties.Settings.Default.LastZoomLevel = _zoomPercent;
            Properties.Settings.Default.Save();

            ApplyFontZoomToWebBrowser();
        }

        /// <summary>Scale via CSS font-size so wrapping stays correct in IE WebBrowser.</summary>
        private void ApplyFontZoomToWebBrowser()
        {
            if (webBrowserContent == null || webBrowserContent.Document == null) return;
            try
            {
                string js = "document.body && (document.body.style.fontSize='" + _zoomPercent.ToString() + "%');";
                webBrowserContent.Document.InvokeScript("execScript", new object[] { js, "JavaScript" });
            }
            catch { /* ignore */ }
        }

        private void SafeRerender()
        {
            RenderCurrentChapter();
            ApplyFontZoomToWebBrowser();
        }

        // ---- Random helpers ----
        public bool TryRandomVerse(out string reference, out string verseText)
        {
            reference = ""; verseText = "";
            if (_bibleXml == null) return false;

            XmlNodeList allVerses = _bibleXml.SelectNodes("//verse");
            if (allVerses == null || allVerses.Count == 0) return false;

            XmlNode verse = allVerses[_rand.Next(allVerses.Count)];
            verseText = verse.InnerText ?? "";
            string verseNum = (verse.Attributes != null && verse.Attributes["number"] != null) ? verse.Attributes["number"].Value : "?";

            XmlNode chapter = verse.ParentNode;
            XmlNode book = (chapter != null) ? chapter.ParentNode : null;

            string chapterNum = (chapter != null && chapter.Attributes != null && chapter.Attributes["number"] != null)
                                ? chapter.Attributes["number"].Value : "?";
            string bookName = "Book";
            if (book != null && book.Attributes != null && book.Attributes["number"] != null)
            {
                int bnum = 0; int.TryParse(book.Attributes["number"].Value, out bnum);
                bookName = MapBookNumberToName(bnum);
            }

            reference = bookName + " " + chapterNum + ":" + verseNum;
            return true;
        }

        public bool TryRandomChapter(out string reference)
        {
            reference = "";
            if (_bibleXml == null) return false;

            XmlNodeList allChapters = _bibleXml.SelectNodes("//chapter");
            if (allChapters == null || allChapters.Count == 0) return false;

            XmlNode chapter = allChapters[_rand.Next(allChapters.Count)];
            XmlNode book = (chapter != null) ? chapter.ParentNode : null;

            string chapterNum = (chapter != null && chapter.Attributes != null && chapter.Attributes["number"] != null)
                                ? chapter.Attributes["number"].Value : "?";
            string bookName = "Book";
            if (book != null && book.Attributes != null && book.Attributes["number"] != null)
            {
                int bnum = 0; int.TryParse(book.Attributes["number"].Value, out bnum);
                bookName = MapBookNumberToName(bnum);
            }

            _currentBookName = bookName;
            _currentChapterNumber = chapterNum;
            RenderCurrentChapter();

            reference = bookName + " " + chapterNum;
            return true;
        }
    }
}
