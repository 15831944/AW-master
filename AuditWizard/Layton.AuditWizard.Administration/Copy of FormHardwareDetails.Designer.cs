namespace Layton.AuditWizard.Administration
{
	partial class FormHardwareDetails
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
            this.cbPhysicalDisks = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cbNetworkDrives = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cbActiveSystemComponents = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cbSecuritySettings = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.cbGeneralSettings = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.hardware_settings_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(3, 256);
            this.footerPictureBox.Size = new System.Drawing.Size(447, 120);
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label10.Location = new System.Drawing.Point(14, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(422, 32);
            this.label10.TabIndex = 25;
            this.label10.Text = "Select the Hardware Categories to be audited.";
            // 
            // cbPhysicalDisks
            // 
            this.cbPhysicalDisks.BackColor = System.Drawing.Color.Transparent;
            this.cbPhysicalDisks.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbPhysicalDisks.Checked = true;
            this.cbPhysicalDisks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPhysicalDisks.Location = new System.Drawing.Point(17, 56);
            this.cbPhysicalDisks.Name = "cbPhysicalDisks";
            this.cbPhysicalDisks.Size = new System.Drawing.Size(304, 20);
            this.cbPhysicalDisks.TabIndex = 26;
            this.cbPhysicalDisks.Text = "Physical Disk Drives";
            // 
            // cbNetworkDrives
            // 
            this.cbNetworkDrives.BackColor = System.Drawing.Color.Transparent;
            this.cbNetworkDrives.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbNetworkDrives.Checked = true;
            this.cbNetworkDrives.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbNetworkDrives.Location = new System.Drawing.Point(17, 82);
            this.cbNetworkDrives.Name = "cbNetworkDrives";
            this.cbNetworkDrives.Size = new System.Drawing.Size(140, 20);
            this.cbNetworkDrives.TabIndex = 27;
            this.cbNetworkDrives.Text = "Network Drives";
            // 
            // cbActiveSystemComponents
            // 
            this.cbActiveSystemComponents.BackColor = System.Drawing.Color.Transparent;
            this.cbActiveSystemComponents.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbActiveSystemComponents.Checked = true;
            this.cbActiveSystemComponents.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbActiveSystemComponents.Location = new System.Drawing.Point(17, 108);
            this.cbActiveSystemComponents.Name = "cbActiveSystemComponents";
            this.cbActiveSystemComponents.Size = new System.Drawing.Size(419, 20);
            this.cbActiveSystemComponents.TabIndex = 28;
            this.cbActiveSystemComponents.Text = "Active System Components (System Services, Users, Processes)";
            // 
            // cbSecuritySettings
            // 
            this.cbSecuritySettings.BackColor = System.Drawing.Color.Transparent;
            this.cbSecuritySettings.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbSecuritySettings.Checked = true;
            this.cbSecuritySettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSecuritySettings.Location = new System.Drawing.Point(17, 134);
            this.cbSecuritySettings.Name = "cbSecuritySettings";
            this.cbSecuritySettings.Size = new System.Drawing.Size(399, 20);
            this.cbSecuritySettings.TabIndex = 29;
            this.cbSecuritySettings.Text = "Security Settings (Windows Firewall and Auto Update)";
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(349, 218);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 31;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(254, 218);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 30;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // cbGeneralSettings
            // 
            this.cbGeneralSettings.BackColor = System.Drawing.Color.Transparent;
            this.cbGeneralSettings.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbGeneralSettings.Checked = true;
            this.cbGeneralSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGeneralSettings.Location = new System.Drawing.Point(17, 160);
            this.cbGeneralSettings.Name = "cbGeneralSettings";
            this.cbGeneralSettings.Size = new System.Drawing.Size(399, 20);
            this.cbGeneralSettings.TabIndex = 32;
            this.cbGeneralSettings.Text = "Settings (Environment Strings)";
            // 
            // FormHardwareDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(450, 376);
            this.Controls.Add(this.cbGeneralSettings);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.cbSecuritySettings);
            this.Controls.Add(this.cbActiveSystemComponents);
            this.Controls.Add(this.cbNetworkDrives);
            this.Controls.Add(this.cbPhysicalDisks);
            this.Controls.Add(this.label10);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormHardwareDetails";
            this.Text = "Select Hardware Items to Audit";
            this.Load += new System.EventHandler(this.FormHardwareDetails_Load);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.label10, 0);
            this.Controls.SetChildIndex(this.cbPhysicalDisks, 0);
            this.Controls.SetChildIndex(this.cbNetworkDrives, 0);
            this.Controls.SetChildIndex(this.cbActiveSystemComponents, 0);
            this.Controls.SetChildIndex(this.cbSecuritySettings, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.cbGeneralSettings, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label10;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbPhysicalDisks;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbNetworkDrives;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbActiveSystemComponents;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbSecuritySettings;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbGeneralSettings;
	}
}
