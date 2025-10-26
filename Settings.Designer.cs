namespace Core_Bible
{
    partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.activeList = new System.Windows.Forms.ListBox();
            this.translationList = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.clearsettingsButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.clearallButton = new System.Windows.Forms.Button();
            this.inactiveButton = new System.Windows.Forms.Button();
            this.activeButton = new System.Windows.Forms.Button();
            this.defaultButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(504, 328);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.clearallButton);
            this.tabPage1.Controls.Add(this.inactiveButton);
            this.tabPage1.Controls.Add(this.activeButton);
            this.tabPage1.Controls.Add(this.activeList);
            this.tabPage1.Controls.Add(this.defaultButton);
            this.tabPage1.Controls.Add(this.deleteButton);
            this.tabPage1.Controls.Add(this.importButton);
            this.tabPage1.Controls.Add(this.translationList);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(496, 302);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Translations";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // activeList
            // 
            this.activeList.FormattingEnabled = true;
            this.activeList.Location = new System.Drawing.Point(256, 40);
            this.activeList.Name = "activeList";
            this.activeList.Size = new System.Drawing.Size(232, 212);
            this.activeList.TabIndex = 4;
            // 
            // translationList
            // 
            this.translationList.FormattingEnabled = true;
            this.translationList.Location = new System.Drawing.Point(8, 40);
            this.translationList.Name = "translationList";
            this.translationList.Size = new System.Drawing.Size(232, 212);
            this.translationList.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.clearsettingsButton);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(496, 302);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Other";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // clearsettingsButton
            // 
            this.clearsettingsButton.ForeColor = System.Drawing.Color.Red;
            this.clearsettingsButton.Location = new System.Drawing.Point(192, 152);
            this.clearsettingsButton.Name = "clearsettingsButton";
            this.clearsettingsButton.Size = new System.Drawing.Size(104, 23);
            this.clearsettingsButton.TabIndex = 0;
            this.clearsettingsButton.Text = "Clear Settings";
            this.clearsettingsButton.UseVisualStyleBackColor = true;
            this.clearsettingsButton.Click += new System.EventHandler(this.clearsettingsButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(424, 328);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(344, 328);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // clearallButton
            // 
            this.clearallButton.Image = global::Core_Bible.Properties.Resources.delete;
            this.clearallButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.clearallButton.Location = new System.Drawing.Point(168, 8);
            this.clearallButton.Name = "clearallButton";
            this.clearallButton.Size = new System.Drawing.Size(72, 24);
            this.clearallButton.TabIndex = 7;
            this.clearallButton.Text = "Clear All";
            this.clearallButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.clearallButton.UseVisualStyleBackColor = true;
            this.clearallButton.Click += new System.EventHandler(this.clearallButton_Click);
            // 
            // inactiveButton
            // 
            this.inactiveButton.Image = global::Core_Bible.Properties.Resources.flag_yellow;
            this.inactiveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inactiveButton.Location = new System.Drawing.Point(360, 256);
            this.inactiveButton.Name = "inactiveButton";
            this.inactiveButton.Size = new System.Drawing.Size(128, 23);
            this.inactiveButton.TabIndex = 6;
            this.inactiveButton.Text = "Move to Non-Active";
            this.inactiveButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.inactiveButton.UseVisualStyleBackColor = true;
            this.inactiveButton.Click += new System.EventHandler(this.inactiveButton_Click);
            // 
            // activeButton
            // 
            this.activeButton.Image = global::Core_Bible.Properties.Resources.flag_green;
            this.activeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.activeButton.Location = new System.Drawing.Point(8, 256);
            this.activeButton.Name = "activeButton";
            this.activeButton.Size = new System.Drawing.Size(104, 23);
            this.activeButton.TabIndex = 5;
            this.activeButton.Text = "Move to Active";
            this.activeButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.activeButton.UseVisualStyleBackColor = true;
            this.activeButton.Click += new System.EventHandler(this.activeButton_Click);
            // 
            // defaultButton
            // 
            this.defaultButton.Image = global::Core_Bible.Properties.Resources.flag_purple;
            this.defaultButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.defaultButton.Location = new System.Drawing.Point(256, 8);
            this.defaultButton.Name = "defaultButton";
            this.defaultButton.Size = new System.Drawing.Size(72, 24);
            this.defaultButton.TabIndex = 3;
            this.defaultButton.Text = "Default";
            this.defaultButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.defaultButton.UseVisualStyleBackColor = true;
            this.defaultButton.Click += new System.EventHandler(this.defaultButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Image = global::Core_Bible.Properties.Resources.book_delete;
            this.deleteButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.deleteButton.Location = new System.Drawing.Point(88, 8);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(72, 24);
            this.deleteButton.TabIndex = 2;
            this.deleteButton.Text = "Delete";
            this.deleteButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // importButton
            // 
            this.importButton.Image = global::Core_Bible.Properties.Resources.book_add;
            this.importButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.importButton.Location = new System.Drawing.Point(8, 8);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(72, 24);
            this.importButton.TabIndex = 1;
            this.importButton.Text = "Import";
            this.importButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 360);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.ListBox translationList;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button defaultButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button inactiveButton;
        private System.Windows.Forms.Button activeButton;
        private System.Windows.Forms.ListBox activeList;
        private System.Windows.Forms.Button clearsettingsButton;
        private System.Windows.Forms.Button clearallButton;
    }
}