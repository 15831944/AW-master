namespace Layton.AuditWizard.Administration
{
	partial class FormSoftwareDetails
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
			this.label10 = new System.Windows.Forms.Label();
			this.cbApplications = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
			this.cbOperatingSystem = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
			this.bnCancel = new System.Windows.Forms.Button();
			this.bnOK = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbApplications)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOperatingSystem)).BeginInit();
			this.SuspendLayout();
			// 
			// footerPictureBox
			// 
			this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.software_settings_corner;
			this.footerPictureBox.Location = new System.Drawing.Point(3, 161);
			// 
			// label10
			// 
			this.label10.BackColor = System.Drawing.Color.Transparent;
			this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label10.Location = new System.Drawing.Point(14, 21);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(422, 32);
			this.label10.TabIndex = 25;
			this.label10.Text = "Select the Software Categories to be audited.";
			// 
			// cbApplications
			// 
			this.cbApplications.BackColor = System.Drawing.Color.Transparent;
			this.cbApplications.BackColorInternal = System.Drawing.Color.Transparent;
			this.cbApplications.Checked = true;
			this.cbApplications.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbApplications.Location = new System.Drawing.Point(17, 56);
			this.cbApplications.Name = "cbApplications";
			this.cbApplications.Size = new System.Drawing.Size(304, 20);
			this.cbApplications.TabIndex = 26;
			this.cbApplications.Text = "Installed Applications";
			// 
			// cbOperatingSystem
			// 
			this.cbOperatingSystem.BackColor = System.Drawing.Color.Transparent;
			this.cbOperatingSystem.BackColorInternal = System.Drawing.Color.Transparent;
			this.cbOperatingSystem.Checked = true;
			this.cbOperatingSystem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbOperatingSystem.Location = new System.Drawing.Point(17, 82);
			this.cbOperatingSystem.Name = "cbOperatingSystem";
			this.cbOperatingSystem.Size = new System.Drawing.Size(140, 20);
			this.cbOperatingSystem.TabIndex = 27;
			this.cbOperatingSystem.Text = "Operating System";
			// 
			// bnCancel
			// 
			this.bnCancel.Location = new System.Drawing.Point(351, 132);
			this.bnCancel.Name = "bnCancel";
			this.bnCancel.Size = new System.Drawing.Size(87, 23);
			this.bnCancel.TabIndex = 31;
			this.bnCancel.Text = "&Cancel";
			this.bnCancel.UseVisualStyleBackColor = true;
			// 
			// bnOK
			// 
			this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bnOK.Location = new System.Drawing.Point(258, 132);
			this.bnOK.Name = "bnOK";
			this.bnOK.Size = new System.Drawing.Size(87, 23);
			this.bnOK.TabIndex = 30;
			this.bnOK.Text = "&OK";
			this.bnOK.UseVisualStyleBackColor = true;
			this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
			// 
			// FormSoftwareDetails
			// 
			this.AcceptButton = this.bnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.CancelButton = this.bnCancel;
			this.ClientSize = new System.Drawing.Size(450, 281);
			this.Controls.Add(this.bnCancel);
			this.Controls.Add(this.bnOK);
			this.Controls.Add(this.cbOperatingSystem);
			this.Controls.Add(this.cbApplications);
			this.Controls.Add(this.label10);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "FormSoftwareDetails";
			this.Text = "Select Software Items to Audit";
			this.Load += new System.EventHandler(this.FormSoftwareDetails_Load);
			this.Controls.SetChildIndex(this.footerPictureBox, 0);
			this.Controls.SetChildIndex(this.label10, 0);
			this.Controls.SetChildIndex(this.cbApplications, 0);
			this.Controls.SetChildIndex(this.cbOperatingSystem, 0);
			this.Controls.SetChildIndex(this.bnOK, 0);
			this.Controls.SetChildIndex(this.bnCancel, 0);
			((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbApplications)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOperatingSystem)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label10;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbApplications;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbOperatingSystem;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
	}
}
