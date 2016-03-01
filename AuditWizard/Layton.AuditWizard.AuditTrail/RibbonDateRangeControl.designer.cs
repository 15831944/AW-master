namespace Layton.AuditWizard.AuditTrail
{
    partial class RibbonDateRangeControl
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
            Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton dateButton2 = new Infragistics.Win.UltraWinSchedule.CalendarCombo.DateButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.endDateCombo = new Infragistics.Win.UltraWinSchedule.UltraCalendarCombo();
            this.startDateCombo = new Infragistics.Win.UltraWinSchedule.UltraCalendarCombo();
            ((System.ComponentModel.ISupportInitialize)(this.endDateCombo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startDateCombo)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(7, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "To:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(7, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "From:";
            // 
            // endDateCombo
            // 
            this.endDateCombo.BackColor = System.Drawing.SystemColors.Window;
            this.endDateCombo.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2007RibbonButton;
            this.endDateCombo.DateButtons.Add(dateButton1);
            this.endDateCombo.Location = new System.Drawing.Point(52, 36);
            this.endDateCombo.Name = "endDateCombo";
            this.endDateCombo.NonAutoSizeHeight = 21;
            this.endDateCombo.Size = new System.Drawing.Size(134, 21);
            this.endDateCombo.TabIndex = 13;
            this.endDateCombo.Value = "";
            this.endDateCombo.ValueChanged += new System.EventHandler(this.endDateCombo_ValueChanged);
            // 
            // startDateCombo
            // 
            this.startDateCombo.BackColor = System.Drawing.SystemColors.Window;
            this.startDateCombo.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2007RibbonButton;
            this.startDateCombo.DateButtons.Add(dateButton2);
            this.startDateCombo.Location = new System.Drawing.Point(52, 7);
            this.startDateCombo.Name = "startDateCombo";
            this.startDateCombo.NonAutoSizeHeight = 21;
            this.startDateCombo.Size = new System.Drawing.Size(134, 21);
            this.startDateCombo.TabIndex = 12;
            this.startDateCombo.Value = "";
            this.startDateCombo.ValueChanged += new System.EventHandler(this.startDateCombo_ValueChanged);
            // 
            // RibbonDateRangeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.endDateCombo);
            this.Controls.Add(this.startDateCombo);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "RibbonDateRangeControl";
            this.Size = new System.Drawing.Size(203, 64);
            this.Load += new System.EventHandler(this.RibbonDateRangeControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.endDateCombo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startDateCombo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo endDateCombo;
        private Infragistics.Win.UltraWinSchedule.UltraCalendarCombo startDateCombo;
    }
}
