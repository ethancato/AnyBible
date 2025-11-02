namespace Core_Bible
{
    partial class PassageChildForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PassageChildForm));
            this.webBrowserContent = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowserContent
            // 
            this.webBrowserContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserContent.Location = new System.Drawing.Point(0, 0);
            this.webBrowserContent.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserContent.Name = "webBrowserContent";
            this.webBrowserContent.Size = new System.Drawing.Size(800, 450);
            this.webBrowserContent.TabIndex = 0;
            // 
            // PassageChildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.webBrowserContent);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "PassageChildForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "PassageChildForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowserContent;
    }
}