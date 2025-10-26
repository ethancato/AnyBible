using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Core_Bible
{
    public partial class Settings : Form
    {
        private List<string> allTranslations = new List<string>();
        private List<string> activeTranslations = new List<string>();
        private string defaultBible = "";
        private string bibleDir;

        public Settings()
        {
            InitializeComponent();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            bibleDir = baseDir.TrimEnd('\\') + "\\Bibles";
            if (!Directory.Exists(bibleDir))
                Directory.CreateDirectory(bibleDir);



            LoadSettings();
        }

        // -----------------------------
        // Load/Refresh
        // -----------------------------
        private void LoadSettings()
        {
            translationList.Items.Clear();
            activeList.Items.Clear();
            allTranslations.Clear();
            activeTranslations.Clear();

            defaultBible = Properties.Settings.Default.DefaultBible;

            // All files from Bibles folder
            if (Directory.Exists(bibleDir))
            {
                string[] files = Directory.GetFiles(bibleDir, "*.xml");
                for (int i = 0; i < files.Length; i++)
                    allTranslations.Add(Path.GetFileNameWithoutExtension(files[i]));
            }

            // Active from settings
            for (int i = 1; i <= 5; i++)
            {
                string val = (string)Properties.Settings.Default["Bible" + i];
                if (!IsNullOrWhiteSpace(val))
                    activeTranslations.Add(val);
            }

            RefreshLists();
        }

        private void RefreshLists()
        {
            translationList.Items.Clear();
            activeList.Items.Clear();

            // Non-active list
            for (int i = 0; i < allTranslations.Count; i++)
            {
                string t = allTranslations[i];
                bool isActive = false;
                for (int j = 0; j < activeTranslations.Count; j++)
                {
                    if (string.Compare(activeTranslations[j], t, true) == 0)
                    {
                        isActive = true;
                        break;
                    }
                }
                if (!isActive)
                    translationList.Items.Add(t);
            }

            // Active list (+ mark default)
            for (int i = 0; i < activeTranslations.Count; i++)
            {
                string a = activeTranslations[i];
                if (string.Compare(a, defaultBible, true) == 0)
                    activeList.Items.Add(a + " (Default)");
                else
                    activeList.Items.Add(a);
            }
        }

        // -----------------------------
        // Import (copy file into .\Bibles)
        // -----------------------------
        private void importButton_Click(object sender, EventArgs e)
        {
            importButton.Enabled = false;
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Bible XML|*.xml";
                ofd.Title = "Import Bible Translation";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string src = ofd.FileName;
                    string name = Path.GetFileNameWithoutExtension(src);
                    string dest = bibleDir + "\\" + name + ".xml";

                    if (File.Exists(dest))
                    {
                        if (MessageBox.Show("Bible '" + name + "' already exists. Overwrite?",
                                            "Confirm Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                            return;
                        try { File.Delete(dest); } catch { }
                    }

                    File.Copy(src, dest, true);

                    if (!ContainsString(allTranslations, name))
                        allTranslations.Add(name);

                    // If it wasn't active, ensure it’s visible in inactive list
                    if (!ListBoxContains(translationList, name) && !ListBoxContains(activeList, name) && !ListBoxContains(activeList, name + " (Default)"))
                        translationList.Items.Add(name);

                    MessageBox.Show("Bible imported into: " + dest, "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error importing Bible: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                importButton.Enabled = true;
                RefreshLists();
            }
        }

        // -----------------------------
        // Delete (remove from .\Bibles and from settings if active/default)
        // -----------------------------
        private void deleteButton_Click(object sender, EventArgs e)
        {
            string selected = null;

            if (translationList.SelectedItem != null)
                selected = translationList.SelectedItem.ToString();
            else if (activeList.SelectedItem != null)
                selected = activeList.SelectedItem.ToString().Replace(" (Default)", "");

            if (IsNullOrWhiteSpace(selected))
            {
                MessageBox.Show("Select a Bible to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Delete '" + selected + "' from the application Bibles folder?",
                                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            string path = bibleDir + "\\" + selected + ".xml";
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not delete file: " + path + "\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Remove from lists
            RemoveString(allTranslations, selected);
            RemoveString(activeTranslations, selected);

            // Clear default if that was it
            if (string.Compare(defaultBible, selected, true) == 0)
            {
                defaultBible = "";
                Properties.Settings.Default.DefaultBible = "";
            }

            // Clear any Bible1..Bible5 slots matching this
            for (int i = 1; i <= 5; i++)
            {
                string slot = (string)Properties.Settings.Default["Bible" + i];
                if (string.Compare(slot, selected, true) == 0)
                    Properties.Settings.Default["Bible" + i] = "";
            }

            Properties.Settings.Default.Save();

            RefreshLists();

            MessageBox.Show("Bible '" + selected + "' deleted from the application folder.", "Deleted",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // -----------------------------
        // Set Default (mark in active list)
        // -----------------------------
        private void defaultButton_Click(object sender, EventArgs e)
        {
            if (activeList.SelectedItem == null)
                return;

            string selected = activeList.SelectedItem.ToString().Replace(" (Default)", "");
            defaultBible = selected;
            Properties.Settings.Default.DefaultBible = selected;
            Properties.Settings.Default.Save();
            RefreshLists();
        }

        // -----------------------------
        // Move to Active
        // -----------------------------
        private void activeButton_Click(object sender, EventArgs e)
        {
            if (translationList.SelectedItem == null)
                return;

            if (activeTranslations.Count >= 5)
            {
                MessageBox.Show("You can only have 5 active translations.", "Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selected = translationList.SelectedItem.ToString();
            if (!ContainsString(activeTranslations, selected))
                activeTranslations.Add(selected);

            translationList.Items.Remove(selected);
            RefreshLists();
        }

        // -----------------------------
        // Move to Inactive
        // -----------------------------
        private void inactiveButton_Click(object sender, EventArgs e)
        {
            if (activeList.SelectedItem == null)
                return;

            string selected = activeList.SelectedItem.ToString().Replace(" (Default)", "");
            RemoveString(activeTranslations, selected);

            if (string.Compare(selected, defaultBible, true) == 0)
            {
                defaultBible = "";
                Properties.Settings.Default.DefaultBible = "";
                Properties.Settings.Default.Save();
            }

            RefreshLists();
        }

        // -----------------------------
        // OK / Cancel
        // -----------------------------
        private void okButton_Click(object sender, EventArgs e)
        {
            // Persist active list into Bible1..Bible5
            for (int i = 1; i <= 5; i++)
            {
                string val = "";
                if (i <= activeTranslations.Count)
                    val = activeTranslations[i - 1];
                Properties.Settings.Default["Bible" + i] = val;
            }

            Properties.Settings.Default.DefaultBible = defaultBible;
            Properties.Settings.Default.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // -----------------------------
        // Clear only settings (keep files)
        // -----------------------------
        private void clearsettingsButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear all saved Bible settings (active slots + default)?\nFiles will remain.",
                                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            Properties.Settings.Default.DefaultBible = "";
            for (int i = 1; i <= 5; i++)
                Properties.Settings.Default["Bible" + i] = "";

            Properties.Settings.Default.Save();

            defaultBible = "";
            activeTranslations.Clear();

            RefreshLists();

            MessageBox.Show("Settings cleared. Files are unchanged.", "Done",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // -----------------------------
        // Clear ALL: files + settings
        // -----------------------------
        private void clearallButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete ALL Bible files from the application folder and clear settings?",
                                "Confirm Delete All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                if (Directory.Exists(bibleDir))
                {
                    string[] files = Directory.GetFiles(bibleDir, "*.xml");
                    for (int i = 0; i < files.Length; i++)
                    {
                        try { File.Delete(files[i]); } catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting files: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Clear all in-memory and settings
            allTranslations.Clear();
            activeTranslations.Clear();
            defaultBible = "";

            Properties.Settings.Default.DefaultBible = "";
            for (int i = 1; i <= 5; i++)
                Properties.Settings.Default["Bible" + i] = "";
            Properties.Settings.Default.Save();

            RefreshLists();

            MessageBox.Show("All Bible files deleted and settings cleared.", "Complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // -----------------------------
        // Helpers
        // -----------------------------
        private static bool IsNullOrWhiteSpace(string s)
        {
            if (s == null) return true;
            for (int i = 0; i < s.Length; i++)
                if (!char.IsWhiteSpace(s[i])) return false;
            return true;
        }

        private static bool ContainsString(List<string> list, string value)
        {
            for (int i = 0; i < list.Count; i++)
                if (string.Compare(list[i], value, true) == 0) return true;
            return false;
        }

        private static void RemoveString(List<string> list, string value)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (string.Compare(list[i], value, true) == 0) list.RemoveAt(i);
        }

        private static bool ListBoxContains(ListBox box, string value)
        {
            for (int i = 0; i < box.Items.Count; i++)
                if (string.Compare(box.Items[i].ToString(), value, true) == 0) return true;
            return false;
        }
    }
}
