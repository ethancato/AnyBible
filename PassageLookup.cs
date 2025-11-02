using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Core_Bible
{
    public partial class PassageLookup : Form
    {
        public PassageLookup()
        {
            InitializeComponent();

            if (this.searchButton != null)
                this.searchButton.Click += searchButton_Click;
        }

        // Optional: let parent dark-mode this form too
        public void ApplyDarkMode(bool enabled)
        {
            System.Drawing.Color back = enabled ? System.Drawing.Color.FromArgb(34, 40, 49) : System.Drawing.SystemColors.Control;
            System.Drawing.Color fore = enabled ? System.Drawing.Color.FromArgb(230, 230, 230) : System.Drawing.SystemColors.ControlText;

            this.BackColor = back;
            this.ForeColor = fore;

            for (int i = 0; i < this.Controls.Count; i++)
            {
                try { this.Controls[i].BackColor = back; } catch { }
                try { this.Controls[i].ForeColor = fore; } catch { }
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (lookupTextBox == null) return;
            string raw = (lookupTextBox.Text ?? "").Trim();
            if (raw.Length == 0) return;

            string book;
            int sc; int? sv;
            int ec; int? ev;
            string err;

            if (!TryParseReference(raw, out book, out sc, out sv, out ec, out ev, out err))
            {
                MessageBox.Show(err, "Invalid Reference");
                return;
            }

            // Active MDI child must be a PassageChildForm
            PassageChildForm child = GetActivePassageChild();
            if (child == null)
            {
                MessageBox.Show("Open a Bible window first.", "No Active Window");
                return;
            }

            // Ensure child's Bible is loaded (keep whatever translation it already has)
            if (child.BibleXml == null)
            {
                string def = Properties.Settings.Default.DefaultBible;
                if (!string.IsNullOrEmpty(def)) child.LoadBibleByTranslationName(def);
            }

            // Render whole passage on one page
            child.RenderPassage(book, sc, sv, ec, ev);
            this.Hide();
        }

        private PassageChildForm GetActivePassageChild()
        {
            MDIParentForm host = this.MdiParent as MDIParentForm;
            if (host != null)
            {
                return host.ActiveMdiChild as PassageChildForm;
            }

            // Fallback: locate MDI parent by open forms
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {
                MDIParentForm p = Application.OpenForms[i] as MDIParentForm;
                if (p != null) return p.ActiveMdiChild as PassageChildForm;
            }
            return null;
        }

        // --- Parsing ---

        // Accepts: "Book C", "Book C:V", "Book C:V-V2", "Book C-D", "Book C:V-D",
        // "Book C:V-D:E"
        private bool TryParseReference(
    string input,
    out string book, out int sChapter, out int? sVerse,
    out int eChapter, out int? eVerse,
    out string error)
        {
            book = ""; sChapter = 1; sVerse = null; eChapter = 1; eVerse = null; error = null;

            // Normalize whitespace and dashes
            string s = Regex.Replace(input ?? "", @"\s+", " ").Trim();
            s = s.Replace('–', '-').Replace('—', '-'); // en/em dash -> hyphen

            // Canonical book names
            string[] Books = {
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

            // Longest book-name match at start
            string matchedBook = null; int matchedLen = -1;
            for (int i = 0; i < Books.Length; i++)
            {
                string b = Books[i];
                if (s.Length >= b.Length &&
                    string.Compare(s, 0, b, 0, b.Length, true) == 0 &&
                    (s.Length == b.Length || s[b.Length] == ' '))
                {
                    if (b.Length > matchedLen) { matchedBook = b; matchedLen = b.Length; }
                }
            }
            if (matchedBook == null) { error = "Could not recognize the book name."; return false; }

            book = matchedBook;
            string rest = s.Length > matchedLen ? s.Substring(matchedLen).Trim() : "";
            if (rest.Length == 0) { error = "Add a chapter/verse (e.g., \"John 3:16\")."; return false; }

            int P(string v) { int x; int.TryParse(v, out x); return x; }
            void Norm(ref int sc, ref int? sv, ref int ec, ref int? ev)
            {
                if (ec < sc || (ec == sc && (ev ?? 0) < (sv ?? 0)))
                {
                    int sc2 = sc, ec2 = ec; int? sv2 = sv, ev2 = ev;
                    sc = ec2; ec = sc2; sv = ev2; ev = sv2;
                }
            }

            Match m;

            // 1) C:V - D:E
            m = Regex.Match(rest, @"^(?<sc>\d+):(?<sv>\d+)\s*-\s*(?<ec>\d+):(?<ev>\d+)$");
            if (m.Success)
            {
                sChapter = P(m.Groups["sc"].Value);
                sVerse = P(m.Groups["sv"].Value);
                eChapter = P(m.Groups["ec"].Value);
                eVerse = P(m.Groups["ev"].Value);
                Norm(ref sChapter, ref sVerse, ref eChapter, ref eVerse);
                return true;
            }

            // 2) C:V - V2  (same chapter)  <-- moved BEFORE C:V - D
            m = Regex.Match(rest, @"^(?<sc>\d+):(?<sv>\d+)\s*-\s*(?<ev>\d+)$");
            if (m.Success)
            {
                sChapter = P(m.Groups["sc"].Value);
                sVerse = P(m.Groups["sv"].Value);
                eChapter = sChapter;
                eVerse = P(m.Groups["ev"].Value);
                Norm(ref sChapter, ref sVerse, ref eChapter, ref eVerse);
                return true;
            }

            // 3) C:V - D  (cross-chapter to end of chapter D)
            m = Regex.Match(rest, @"^(?<sc>\d+):(?<sv>\d+)\s*-\s*(?<ec>\d+)$");
            if (m.Success)
            {
                sChapter = P(m.Groups["sc"].Value);
                sVerse = P(m.Groups["sv"].Value);
                eChapter = P(m.Groups["ec"].Value);
                eVerse = null; // will be interpreted as "to end of chapter eChapter"
                Norm(ref sChapter, ref sVerse, ref eChapter, ref eVerse);
                return true;
            }

            // 4) C - D:V
            m = Regex.Match(rest, @"^(?<sc>\d+)\s*-\s*(?<ec>\d+):(?<ev>\d+)$");
            if (m.Success)
            {
                sChapter = P(m.Groups["sc"].Value);
                sVerse = null;
                eChapter = P(m.Groups["ec"].Value);
                eVerse = P(m.Groups["ev"].Value);
                Norm(ref sChapter, ref sVerse, ref eChapter, ref eVerse);
                return true;
            }

            // 5) C:V
            m = Regex.Match(rest, @"^(?<sc>\d+):(?<sv>\d+)$");
            if (m.Success)
            {
                sChapter = P(m.Groups["sc"].Value);
                sVerse = P(m.Groups["sv"].Value);
                eChapter = sChapter; eVerse = sVerse;
                return true;
            }

            // 6) C - D
            m = Regex.Match(rest, @"^(?<sc>\d+)\s*-\s*(?<ec>\d+)$");
            if (m.Success)
            {
                sChapter = P(m.Groups["sc"].Value);
                sVerse = null;
                eChapter = P(m.Groups["ec"].Value);
                eVerse = null;
                Norm(ref sChapter, ref sVerse, ref eChapter, ref eVerse);
                return true;
            }

            // 7) C
            m = Regex.Match(rest, @"^(?<sc>\d+)$");
            if (m.Success)
            {
                sChapter = P(m.Groups["sc"].Value);
                sVerse = null;
                eChapter = sChapter; eVerse = null;
                return true;
            }

            error = "Reference not understood.";
            return false;
        }



        private static int ParseInt(string s)
        {
            int v = 0; int.TryParse(s, out v); return v;
        }

        private static void NormalizeRanges(ref int sc, ref int? sv, ref int ec, ref int? ev)
        {
            // Ensure start <= end (basic swap if needed)
            if (ec < sc || (ec == sc && (ev ?? 0) < (sv ?? 0)))
            {
                int sc2 = sc; int ec2 = ec; int? sv2 = sv; int? ev2 = ev;
                sc = ec2; ec = sc2; sv = ev2; ev = sv2;
            }
        }
    }
}
