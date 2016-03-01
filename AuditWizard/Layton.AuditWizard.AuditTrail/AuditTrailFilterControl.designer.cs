namespace Layton.AuditWizard.AuditTrail
{
    partial class AuditTrailFilterControl
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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.applicationComboBox = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.userComboBox = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.computerComboBox = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.applicationComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.computerComboBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.applicationhistory32;
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label3.Location = new System.Drawing.Point(320, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 41);
            this.label3.TabIndex = 12;
            this.label3.Text = "Applications";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.computerhistory32;
            this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.Location = new System.Drawing.Point(162, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 41);
            this.label2.TabIndex = 9;
            this.label2.Text = "Computers";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.reportuser32;
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 41);
            this.label1.TabIndex = 0;
            this.label1.Text = "Users";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // applicationComboBox
            // 
            this.applicationComboBox.DropDownListWidth = -1;
            this.applicationComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.applicationComboBox.Location = new System.Drawing.Point(323, 40);
            this.applicationComboBox.Name = "applicationComboBox";
            this.applicationComboBox.Size = new System.Drawing.Size(143, 22);
            this.applicationComboBox.TabIndex = 13;
            this.applicationComboBox.ValueChanged += new System.EventHandler(this.applicationComboBox_SelectedValueChanged);
            // 
            // userComboBox
            // 
            this.userComboBox.DropDownListWidth = -1;
            this.userComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.userComboBox.Location = new System.Drawing.Point(6, 40);
            this.userComboBox.Name = "userComboBox";
            this.userComboBox.Size = new System.Drawing.Size(143, 22);
            this.userComboBox.TabIndex = 14;
            this.userComboBox.ValueChanged += new System.EventHandler(this.usersComboBox_SelectedValueChanged);
            // 
            // computerComboBox
            // 
            this.computerComboBox.DropDownListWidth = -1;
            this.computerComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.computerComboBox.Location = new System.Drawing.Point(165, 40);
            this.computerComboBox.Name = "computerComboBox";
            this.computerComboBox.Size = new System.Drawing.Size(143, 22);
            this.computerComboBox.TabIndex = 15;
            this.computerComboBox.ValueChanged += new System.EventHandler(this.computerComboBox_SelectedValueChanged);
            // 
            // AuditTrailFilterControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.computerComboBox);
            this.Controls.Add(this.userComboBox);
            this.Controls.Add(this.applicationComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "AuditTrailFilterControl";
            this.Size = new System.Drawing.Size(505, 64);
            ((System.ComponentModel.ISupportInitialize)(this.applicationComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.computerComboBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor applicationComboBox;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor userComboBox;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor computerComboBox;
    }
}
