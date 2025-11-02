namespace Core_Bible
{
    partial class MDIParentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MDIParentForm));
            this.panel2 = new System.Windows.Forms.Panel();
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
            this.newBibleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readingChecklistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeViewBible = new System.Windows.Forms.TreeView();
            this.passageSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notepadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.zoomCombo);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.translationCombo);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.menuStrip1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1023, 88);
            this.panel2.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(200, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Zoom:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(280, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Bible Translation:";
            // 
            // zoomCombo
            // 
            this.zoomCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.zoomCombo.FormattingEnabled = true;
            this.zoomCombo.Location = new System.Drawing.Point(200, 48);
            this.zoomCombo.Name = "zoomCombo";
            this.zoomCombo.Size = new System.Drawing.Size(72, 26);
            this.zoomCombo.TabIndex = 4;
            this.zoomCombo.SelectedIndexChanged += new System.EventHandler(this.zoomCombo_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.darkModeCheckbox);
            this.groupBox3.Controls.Add(this.inlineModeCheckbox);
            this.groupBox3.Location = new System.Drawing.Point(8, 32);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(184, 48);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "View";
            // 
            // darkModeCheckbox
            // 
            this.darkModeCheckbox.AutoSize = true;
            this.darkModeCheckbox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.darkModeCheckbox.Location = new System.Drawing.Point(8, 16);
            this.darkModeCheckbox.Name = "darkModeCheckbox";
            this.darkModeCheckbox.Size = new System.Drawing.Size(79, 17);
            this.darkModeCheckbox.TabIndex = 5;
            this.darkModeCheckbox.Text = "Dark Mode";
            this.darkModeCheckbox.UseVisualStyleBackColor = true;
            this.darkModeCheckbox.CheckedChanged += new System.EventHandler(this.darkModeCheckbox_CheckedChanged);
            // 
            // inlineModeCheckbox
            // 
            this.inlineModeCheckbox.AutoSize = true;
            this.inlineModeCheckbox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.inlineModeCheckbox.Location = new System.Drawing.Point(96, 16);
            this.inlineModeCheckbox.Name = "inlineModeCheckbox";
            this.inlineModeCheckbox.Size = new System.Drawing.Size(81, 17);
            this.inlineModeCheckbox.TabIndex = 4;
            this.inlineModeCheckbox.Text = "Inline Mode";
            this.inlineModeCheckbox.UseVisualStyleBackColor = true;
            this.inlineModeCheckbox.CheckedChanged += new System.EventHandler(this.inlineModeCheckbox_CheckedChanged);
            // 
            // translationCombo
            // 
            this.translationCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.translationCombo.FormattingEnabled = true;
            this.translationCombo.Location = new System.Drawing.Point(280, 48);
            this.translationCombo.Name = "translationCombo";
            this.translationCombo.Size = new System.Drawing.Size(200, 26);
            this.translationCombo.TabIndex = 3;
            this.translationCombo.SelectedIndexChanged += new System.EventHandler(this.translationCombo_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cotdButton);
            this.groupBox2.Controls.Add(this.votdButton);
            this.groupBox2.Location = new System.Drawing.Point(847, 32);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(168, 48);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Randomly Selected";
            // 
            // cotdButton
            // 
            this.cotdButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cotdButton.Location = new System.Drawing.Point(88, 16);
            this.cotdButton.Name = "cotdButton";
            this.cotdButton.Size = new System.Drawing.Size(72, 23);
            this.cotdButton.TabIndex = 0;
            this.cotdButton.Text = "Chapter";
            this.cotdButton.UseVisualStyleBackColor = true;
            // 
            // votdButton
            // 
            this.votdButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.votdButton.Location = new System.Drawing.Point(16, 16);
            this.votdButton.Name = "votdButton";
            this.votdButton.Size = new System.Drawing.Size(56, 23);
            this.votdButton.TabIndex = 0;
            this.votdButton.Text = "Verse";
            this.votdButton.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(1021, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newBibleToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newBibleToolStripMenuItem
            // 
            this.newBibleToolStripMenuItem.Name = "newBibleToolStripMenuItem";
            this.newBibleToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.newBibleToolStripMenuItem.Text = "New Bible";
            this.newBibleToolStripMenuItem.Click += new System.EventHandler(this.newBibleToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readingChecklistToolStripMenuItem,
            this.passageSearchToolStripMenuItem,
            this.notepadToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // readingChecklistToolStripMenuItem
            // 
            this.readingChecklistToolStripMenuItem.Name = "readingChecklistToolStripMenuItem";
            this.readingChecklistToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.readingChecklistToolStripMenuItem.Text = "Reading Checklist";
            this.readingChecklistToolStripMenuItem.Click += new System.EventHandler(this.readingChecklistToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // treeViewBible
            // 
            this.treeViewBible.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeViewBible.Location = new System.Drawing.Point(0, 88);
            this.treeViewBible.Name = "treeViewBible";
            this.treeViewBible.Size = new System.Drawing.Size(166, 528);
            this.treeViewBible.TabIndex = 3;
            // 
            // passageSearchToolStripMenuItem
            // 
            this.passageSearchToolStripMenuItem.Name = "passageSearchToolStripMenuItem";
            this.passageSearchToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.passageSearchToolStripMenuItem.Text = "Passage Search";
            this.passageSearchToolStripMenuItem.Click += new System.EventHandler(this.passageSearchToolStripMenuItem_Click);
            // 
            // notepadToolStripMenuItem
            // 
            this.notepadToolStripMenuItem.Name = "notepadToolStripMenuItem";
            this.notepadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.notepadToolStripMenuItem.Text = "Notepad";
            this.notepadToolStripMenuItem.Click += new System.EventHandler(this.notepadToolStripMenuItem_Click);
            // 
            // MDIParentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 616);
            this.Controls.Add(this.treeViewBible);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "MDIParentForm";
            this.Text = "AnyBible";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox zoomCombo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox darkModeCheckbox;
        private System.Windows.Forms.CheckBox inlineModeCheckbox;
        private System.Windows.Forms.ComboBox translationCombo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cotdButton;
        private System.Windows.Forms.Button votdButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.TreeView treeViewBible;
        private System.Windows.Forms.ToolStripMenuItem newBibleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readingChecklistToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem passageSearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notepadToolStripMenuItem;
    }
}