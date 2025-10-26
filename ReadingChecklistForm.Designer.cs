namespace Core_Bible
{
    partial class ReadingChecklistForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReadingChecklistForm));
            this.rcTranslationCombo = new System.Windows.Forms.ComboBox();
            this.rcBooksList = new System.Windows.Forms.ListBox();
            this.rcChaptersCheckedList = new System.Windows.Forms.CheckedListBox();
            this.rcProgressBar = new System.Windows.Forms.ProgressBar();
            this.rcCounterLabel = new System.Windows.Forms.Label();
            this.rcPercentLabel = new System.Windows.Forms.Label();
            this.rcResetButton = new System.Windows.Forms.Button();
            this.rcRefreshButton = new System.Windows.Forms.Button();
            this.rcClearAllButton = new System.Windows.Forms.Button();
            this.rcMarkAllButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rcTranslationCombo
            // 
            this.rcTranslationCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rcTranslationCombo.FormattingEnabled = true;
            this.rcTranslationCombo.Location = new System.Drawing.Point(16, 8);
            this.rcTranslationCombo.Name = "rcTranslationCombo";
            this.rcTranslationCombo.Size = new System.Drawing.Size(408, 33);
            this.rcTranslationCombo.TabIndex = 0;
            // 
            // rcBooksList
            // 
            this.rcBooksList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rcBooksList.FormattingEnabled = true;
            this.rcBooksList.ItemHeight = 15;
            this.rcBooksList.Location = new System.Drawing.Point(16, 64);
            this.rcBooksList.Name = "rcBooksList";
            this.rcBooksList.Size = new System.Drawing.Size(208, 199);
            this.rcBooksList.TabIndex = 1;
            // 
            // rcChaptersCheckedList
            // 
            this.rcChaptersCheckedList.CheckOnClick = true;
            this.rcChaptersCheckedList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rcChaptersCheckedList.FormattingEnabled = true;
            this.rcChaptersCheckedList.Location = new System.Drawing.Point(256, 64);
            this.rcChaptersCheckedList.Name = "rcChaptersCheckedList";
            this.rcChaptersCheckedList.Size = new System.Drawing.Size(168, 196);
            this.rcChaptersCheckedList.TabIndex = 2;
            // 
            // rcProgressBar
            // 
            this.rcProgressBar.Location = new System.Drawing.Point(16, 304);
            this.rcProgressBar.Name = "rcProgressBar";
            this.rcProgressBar.Size = new System.Drawing.Size(408, 40);
            this.rcProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.rcProgressBar.TabIndex = 7;
            // 
            // rcCounterLabel
            // 
            this.rcCounterLabel.AutoSize = true;
            this.rcCounterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rcCounterLabel.Location = new System.Drawing.Point(16, 360);
            this.rcCounterLabel.Name = "rcCounterLabel";
            this.rcCounterLabel.Size = new System.Drawing.Size(51, 20);
            this.rcCounterLabel.TabIndex = 8;
            this.rcCounterLabel.Text = "label1";
            // 
            // rcPercentLabel
            // 
            this.rcPercentLabel.AutoSize = true;
            this.rcPercentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rcPercentLabel.Location = new System.Drawing.Point(360, 360);
            this.rcPercentLabel.Name = "rcPercentLabel";
            this.rcPercentLabel.Size = new System.Drawing.Size(60, 24);
            this.rcPercentLabel.TabIndex = 9;
            this.rcPercentLabel.Text = "label1";
            // 
            // rcResetButton
            // 
            this.rcResetButton.Image = global::Core_Bible.Properties.Resources.delete;
            this.rcResetButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rcResetButton.Location = new System.Drawing.Point(352, 272);
            this.rcResetButton.Name = "rcResetButton";
            this.rcResetButton.Size = new System.Drawing.Size(72, 23);
            this.rcResetButton.TabIndex = 10;
            this.rcResetButton.Text = "Reset All";
            this.rcResetButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rcResetButton.UseVisualStyleBackColor = true;
            // 
            // rcRefreshButton
            // 
            this.rcRefreshButton.Image = global::Core_Bible.Properties.Resources.arrow_refresh;
            this.rcRefreshButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rcRefreshButton.Location = new System.Drawing.Point(256, 272);
            this.rcRefreshButton.Name = "rcRefreshButton";
            this.rcRefreshButton.Size = new System.Drawing.Size(75, 23);
            this.rcRefreshButton.TabIndex = 5;
            this.rcRefreshButton.Text = "Refresh";
            this.rcRefreshButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rcRefreshButton.UseVisualStyleBackColor = true;
            // 
            // rcClearAllButton
            // 
            this.rcClearAllButton.Image = global::Core_Bible.Properties.Resources.book_delete;
            this.rcClearAllButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rcClearAllButton.Location = new System.Drawing.Point(136, 272);
            this.rcClearAllButton.Name = "rcClearAllButton";
            this.rcClearAllButton.Size = new System.Drawing.Size(88, 23);
            this.rcClearAllButton.TabIndex = 4;
            this.rcClearAllButton.Text = "Clear Book";
            this.rcClearAllButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rcClearAllButton.UseVisualStyleBackColor = true;
            // 
            // rcMarkAllButton
            // 
            this.rcMarkAllButton.Image = global::Core_Bible.Properties.Resources.flag_blue;
            this.rcMarkAllButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.rcMarkAllButton.Location = new System.Drawing.Point(16, 272);
            this.rcMarkAllButton.Name = "rcMarkAllButton";
            this.rcMarkAllButton.Size = new System.Drawing.Size(112, 23);
            this.rcMarkAllButton.TabIndex = 3;
            this.rcMarkAllButton.Text = "Mark Book Read";
            this.rcMarkAllButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rcMarkAllButton.UseVisualStyleBackColor = true;
            // 
            // ReadingChecklistForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 393);
            this.Controls.Add(this.rcResetButton);
            this.Controls.Add(this.rcPercentLabel);
            this.Controls.Add(this.rcCounterLabel);
            this.Controls.Add(this.rcProgressBar);
            this.Controls.Add(this.rcRefreshButton);
            this.Controls.Add(this.rcClearAllButton);
            this.Controls.Add(this.rcMarkAllButton);
            this.Controls.Add(this.rcChaptersCheckedList);
            this.Controls.Add(this.rcBooksList);
            this.Controls.Add(this.rcTranslationCombo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReadingChecklistForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Reading Checklist";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox rcTranslationCombo;
        private System.Windows.Forms.ListBox rcBooksList;
        private System.Windows.Forms.CheckedListBox rcChaptersCheckedList;
        private System.Windows.Forms.Button rcMarkAllButton;
        private System.Windows.Forms.Button rcClearAllButton;
        private System.Windows.Forms.Button rcRefreshButton;
        private System.Windows.Forms.ProgressBar rcProgressBar;
        private System.Windows.Forms.Label rcCounterLabel;
        private System.Windows.Forms.Label rcPercentLabel;
        private System.Windows.Forms.Button rcResetButton;
    }
}