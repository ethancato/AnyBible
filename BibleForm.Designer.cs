namespace Core_Bible
{
    partial class BibleForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BibleForm));
            this.panel2 = new System.Windows.Forms.Panel();
            this.readingChecklistButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.zoomCombo = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.darkModeCheckbox = new System.Windows.Forms.CheckBox();
            this.inlineModeCheckbox = new System.Windows.Forms.CheckBox();
            this.translationCombo = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cotdButton = new System.Windows.Forms.Button();
            this.votdButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel3 = new System.Windows.Forms.Panel();
            this.treeViewBible = new System.Windows.Forms.TreeView();
            this.webBrowserContent = new System.Windows.Forms.WebBrowser();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.readingChecklistButton);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.zoomCombo);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.translationCombo);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.menuStrip1);
            this.panel2.Name = "panel2";
            // 
            // readingChecklistButton
            // 
            resources.ApplyResources(this.readingChecklistButton, "readingChecklistButton");
            this.readingChecklistButton.Name = "readingChecklistButton";
            this.readingChecklistButton.UseVisualStyleBackColor = true;
            this.readingChecklistButton.Click += new System.EventHandler(this.readingChecklistButton_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // zoomCombo
            // 
            resources.ApplyResources(this.zoomCombo, "zoomCombo");
            this.zoomCombo.FormattingEnabled = true;
            this.zoomCombo.Name = "zoomCombo";
            this.zoomCombo.SelectedIndexChanged += new System.EventHandler(this.zoomCombo_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.darkModeCheckbox);
            this.groupBox3.Controls.Add(this.inlineModeCheckbox);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // darkModeCheckbox
            // 
            resources.ApplyResources(this.darkModeCheckbox, "darkModeCheckbox");
            this.darkModeCheckbox.Name = "darkModeCheckbox";
            this.darkModeCheckbox.UseVisualStyleBackColor = true;
            this.darkModeCheckbox.CheckedChanged += new System.EventHandler(this.darkModeCheckbox_CheckedChanged);
            // 
            // inlineModeCheckbox
            // 
            resources.ApplyResources(this.inlineModeCheckbox, "inlineModeCheckbox");
            this.inlineModeCheckbox.Name = "inlineModeCheckbox";
            this.inlineModeCheckbox.UseVisualStyleBackColor = true;
            this.inlineModeCheckbox.CheckedChanged += new System.EventHandler(this.inlineModeCheckbox_CheckedChanged);
            // 
            // translationCombo
            // 
            resources.ApplyResources(this.translationCombo, "translationCombo");
            this.translationCombo.FormattingEnabled = true;
            this.translationCombo.Name = "translationCombo";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.cotdButton);
            this.groupBox2.Controls.Add(this.votdButton);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // cotdButton
            // 
            resources.ApplyResources(this.cotdButton, "cotdButton");
            this.cotdButton.Name = "cotdButton";
            this.cotdButton.UseVisualStyleBackColor = true;
            // 
            // votdButton
            // 
            resources.ApplyResources(this.votdButton, "votdButton");
            this.votdButton.Name = "votdButton";
            this.votdButton.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.treeViewBible);
            this.panel3.Name = "panel3";
            // 
            // treeViewBible
            // 
            resources.ApplyResources(this.treeViewBible, "treeViewBible");
            this.treeViewBible.Name = "treeViewBible";
            // 
            // webBrowserContent
            // 
            resources.ApplyResources(this.webBrowserContent, "webBrowserContent");
            this.webBrowserContent.Name = "webBrowserContent";
            this.webBrowserContent.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // BibleForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.webBrowserContent);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BibleForm";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.WebBrowser webBrowserContent;
        private System.Windows.Forms.TreeView treeViewBible;
        private System.Windows.Forms.Button cotdButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button votdButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ComboBox translationCombo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox inlineModeCheckbox;
        private System.Windows.Forms.CheckBox darkModeCheckbox;
        private System.Windows.Forms.ComboBox zoomCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button readingChecklistButton;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}