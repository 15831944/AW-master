namespace Layton.AuditWizard.AuditTrail
{
	partial class AuditTrailPurgeControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton1 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            this.purgeDateCombo = new Infragistics.Win.UltraWinSchedule.UltraCalendarCombo();
            this.label1 = new System.Windows.Forms.Label();
            this.bnPurge = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.purgeDateCombo)).BeginInit();
            this.SuspendLayout();
            // 
            // purgeDateCombo
            // 
            this.purgeDateCombo.BackColor = System.Drawing.SystemColors.Window;
            this.purgeDateCombo.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2007RibbonButton;
            this.purgeDateCombo.DateButtons.Add(dateButton1);
            this.purgeDateCombo.Location = new System.Drawing.Point(3, 44);
            this.purgeDateCombo.Name = "purgeDateCombo";
            this.purgeDateCombo.NonAutoSizeHeight = 21;
            this.purgeDateCombo.Size = new System.Drawing.Size(149, 21);
            this.purgeDateCombo.TabIndex = 15;
            this.purgeDateCombo.Value = "";
            // 
            // label1
            // 
            this.label1.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.clearlog32;
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 41);
            this.label1.TabIndex = 16;
            this.label1.Text = "Purge Data Before";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // bnPurge
            // 
            this.bnPurge.Location = new System.Drawing.Point(158, 42);
            this.bnPurge.Name = "bnPurge";
            this.bnPurge.Size = new System.Drawing.Size(75, 23);
            this.bnPurge.TabIndex = 17;
            this.bnPurge.Text = "&Purge";
            this.bnPurge.UseVisualStyleBackColor = true;
            this.bnPurge.Click += new System.EventHandler(this.bnPurge_Click);
            // 
            // AuditTrailPurgeControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.bnPurge);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.purgeDateCombo);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "AuditTrailPurgeControl";
            this.Size = new System.Drawing.Size(248, 75);
            ((System.ComponentModel.ISupportInitialize)(this.purgeDateCombo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo purgeDateCombo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button bnPurge;

	}
}
