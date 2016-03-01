namespace Layton.AuditWizard.Administration
{
	partial class FormAlertDefinition
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.tbName = new System.Windows.Forms.TextBox();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cbEmailAlert = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.alert_create_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(-1, 215);
            this.footerPictureBox.Size = new System.Drawing.Size(447, 120);
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(140, 26);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(292, 21);
            this.tbName.TabIndex = 23;
            this.tbName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            // 
            // ultraLabel3
            // 
            appearance3.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel3.Appearance = appearance3;
            this.ultraLabel3.Location = new System.Drawing.Point(16, 29);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(117, 23);
            this.ultraLabel3.TabIndex = 22;
            this.ultraLabel3.Text = "Name of Alert:";
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(345, 186);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 21;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Enabled = false;
            this.bnOK.Location = new System.Drawing.Point(251, 186);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 20;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // ultraGroupBox1
            // 
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.ultraGroupBox1.Appearance = appearance2;
            this.ultraGroupBox1.Controls.Add(this.cbEmailAlert);
            this.ultraGroupBox1.Location = new System.Drawing.Point(16, 67);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(416, 83);
            this.ultraGroupBox1.TabIndex = 24;
            this.ultraGroupBox1.Text = "Alert Actions";
            // 
            // cbEmailAlert
            // 
            this.cbEmailAlert.AutoSize = true;
            this.cbEmailAlert.Location = new System.Drawing.Point(22, 41);
            this.cbEmailAlert.Name = "cbEmailAlert";
            this.cbEmailAlert.Size = new System.Drawing.Size(131, 17);
            this.cbEmailAlert.TabIndex = 1;
            this.cbEmailAlert.Text = "Email Alert Details";
            this.cbEmailAlert.UseVisualStyleBackColor = true;
            // 
            // FormAlertDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 335);
            this.Controls.Add(this.ultraGroupBox1);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormAlertDefinition";
            this.Text = "Create Alert Definition";
            this.Load += new System.EventHandler(this.FormAlertDefinition_Load);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.Controls.SetChildIndex(this.tbName, 0);
            this.Controls.SetChildIndex(this.ultraGroupBox1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbName;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
		private System.Windows.Forms.CheckBox cbEmailAlert;
	}
}