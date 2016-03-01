namespace Layton.AuditWizard.Overview
{
	partial class FormEmailConfiguration
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
            this.emailConfigurationControl = new Layton.AuditWizard.Common.EmailConfigurationControl();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Overview.Properties.Resources.email_settings_footer;
            this.footerPictureBox.Location = new System.Drawing.Point(472, 347);
            this.footerPictureBox.Size = new System.Drawing.Size(447, 120);
            // 
            // emailConfigurationControl
            // 
            this.emailConfigurationControl.BackColor = System.Drawing.Color.Transparent;
            this.emailConfigurationControl.Location = new System.Drawing.Point(14, 21);
            this.emailConfigurationControl.Name = "emailConfigurationControl";
            this.emailConfigurationControl.Size = new System.Drawing.Size(777, 274);
            this.emailConfigurationControl.TabIndex = 10;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(605, 318);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 11;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(700, 318);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 12;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // FormEmailConfiguration
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(801, 467);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.emailConfigurationControl);
            this.Controls.Add(this.bnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormEmailConfiguration";
            this.Text = "Email Configuration";
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.emailConfigurationControl, 0);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Layton.AuditWizard.Common.EmailConfigurationControl emailConfigurationControl;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
	}
}