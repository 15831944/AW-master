namespace Layton.AuditWizard.Applications
{
    partial class ReportProgress
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
            this.pbReportProgress = new System.Windows.Forms.ProgressBar();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblStuff = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // pbReportProgress
            // 
            this.pbReportProgress.Location = new System.Drawing.Point(12, 60);
            this.pbReportProgress.Name = "pbReportProgress";
            this.pbReportProgress.Size = new System.Drawing.Size(455, 23);
            this.pbReportProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbReportProgress.TabIndex = 0;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Location = new System.Drawing.Point(12, 23);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(513, 13);
            this.lblDescription.TabIndex = 2;
            this.lblDescription.Text = "Please wait whilst the report data is generated. This process may take a few mome" +
                "nts...";
            // 
            // lblStuff
            // 
            this.lblStuff.AutoSize = true;
            this.lblStuff.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.lblStuff.Location = new System.Drawing.Point(24, 106);
            this.lblStuff.Name = "lblStuff";
            this.lblStuff.Size = new System.Drawing.Size(0, 13);
            this.lblStuff.TabIndex = 3;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            // 
            // ReportProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 149);
            this.Controls.Add(this.lblStuff);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.pbReportProgress);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReportProgress";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Processing report...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbReportProgress;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblStuff;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}