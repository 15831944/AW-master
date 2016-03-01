namespace Layton.AuditWizard.Administration
{
	partial class FormFtpSettingsBackup
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
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.gbFtpDetails = new Infragistics.Win.Misc.UltraGroupBox();
            this.cbFTPType = new System.Windows.Forms.ComboBox();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.panelFTPCredentials = new System.Windows.Forms.Panel();
            this.tbFtpPassword = new System.Windows.Forms.TextBox();
            this.tbFtpUser = new System.Windows.Forms.TextBox();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.cbLoginAnonymous = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.tbFtpPort = new System.Windows.Forms.NumericUpDown();
            this.tbFtpDefaultDirectory = new System.Windows.Forms.TextBox();
            this.tbFtpSiteName = new System.Windows.Forms.TextBox();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbFtpDetails)).BeginInit();
            this.gbFtpDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            this.panelFTPCredentials.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbFtpPort)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.ftp_settings_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(159, 424);
            this.footerPictureBox.Size = new System.Drawing.Size(380, 120);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(438, 384);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 33;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(343, 384);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 32;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // gbFtpDetails
            // 
            appearance9.BackColor = System.Drawing.Color.Transparent;
            this.gbFtpDetails.Appearance = appearance9;
            this.gbFtpDetails.Controls.Add(this.cbFTPType);
            this.gbFtpDetails.Controls.Add(this.ultraLabel6);
            this.gbFtpDetails.Controls.Add(this.ultraGroupBox1);
            this.gbFtpDetails.Controls.Add(this.tbFtpPort);
            this.gbFtpDetails.Controls.Add(this.tbFtpDefaultDirectory);
            this.gbFtpDetails.Controls.Add(this.tbFtpSiteName);
            this.gbFtpDetails.Controls.Add(this.ultraLabel3);
            this.gbFtpDetails.Controls.Add(this.ultraLabel2);
            this.gbFtpDetails.Controls.Add(this.ultraLabel1);
            this.gbFtpDetails.Location = new System.Drawing.Point(14, 22);
            this.gbFtpDetails.Name = "gbFtpDetails";
            this.gbFtpDetails.Size = new System.Drawing.Size(511, 344);
            this.gbFtpDetails.TabIndex = 35;
            this.gbFtpDetails.Text = "Connection Details";
            // 
            // cbFTPType
            // 
            this.cbFTPType.FormattingEnabled = true;
            this.cbFTPType.Items.AddRange(new object[] {
            "FTP",
            "SFTP"});
            this.cbFTPType.Location = new System.Drawing.Point(199, 26);
            this.cbFTPType.Name = "cbFTPType";
            this.cbFTPType.Size = new System.Drawing.Size(90, 21);
            this.cbFTPType.TabIndex = 27;
            this.cbFTPType.SelectedIndexChanged += new System.EventHandler(this.cbFTPType_SelectedIndexChanged);
            // 
            // ultraLabel6
            // 
            appearance3.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel6.Appearance = appearance3;
            this.ultraLabel6.Location = new System.Drawing.Point(22, 32);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(138, 18);
            this.ultraLabel6.TabIndex = 26;
            this.ultraLabel6.Text = "Connection Type:";
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.panelFTPCredentials);
            this.ultraGroupBox1.Controls.Add(this.cbLoginAnonymous);
            this.ultraGroupBox1.Location = new System.Drawing.Point(12, 154);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(482, 169);
            this.ultraGroupBox1.TabIndex = 25;
            this.ultraGroupBox1.Text = "Login Credentials";
            // 
            // panelFTPCredentials
            // 
            this.panelFTPCredentials.Controls.Add(this.tbFtpPassword);
            this.panelFTPCredentials.Controls.Add(this.tbFtpUser);
            this.panelFTPCredentials.Controls.Add(this.ultraLabel5);
            this.panelFTPCredentials.Controls.Add(this.ultraLabel4);
            this.panelFTPCredentials.Enabled = false;
            this.panelFTPCredentials.Location = new System.Drawing.Point(37, 59);
            this.panelFTPCredentials.Name = "panelFTPCredentials";
            this.panelFTPCredentials.Size = new System.Drawing.Size(425, 93);
            this.panelFTPCredentials.TabIndex = 12;
            // 
            // tbFtpPassword
            // 
            this.tbFtpPassword.Location = new System.Drawing.Point(105, 41);
            this.tbFtpPassword.Name = "tbFtpPassword";
            this.tbFtpPassword.PasswordChar = '*';
            this.tbFtpPassword.Size = new System.Drawing.Size(293, 21);
            this.tbFtpPassword.TabIndex = 17;
            // 
            // tbFtpUser
            // 
            this.tbFtpUser.Location = new System.Drawing.Point(105, 15);
            this.tbFtpUser.Name = "tbFtpUser";
            this.tbFtpUser.Size = new System.Drawing.Size(293, 21);
            this.tbFtpUser.TabIndex = 16;
            // 
            // ultraLabel5
            // 
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel5.Appearance = appearance2;
            this.ultraLabel5.Location = new System.Drawing.Point(10, 44);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(87, 23);
            this.ultraLabel5.TabIndex = 15;
            this.ultraLabel5.Text = "Password:";
            // 
            // ultraLabel4
            // 
            appearance6.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel4.Appearance = appearance6;
            this.ultraLabel4.Location = new System.Drawing.Point(10, 18);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(87, 23);
            this.ultraLabel4.TabIndex = 14;
            this.ultraLabel4.Text = "User ID:";
            // 
            // cbLoginAnonymous
            // 
            appearance4.BackColor = System.Drawing.Color.Transparent;
            this.cbLoginAnonymous.Appearance = appearance4;
            this.cbLoginAnonymous.BackColor = System.Drawing.Color.Transparent;
            this.cbLoginAnonymous.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbLoginAnonymous.Checked = true;
            this.cbLoginAnonymous.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLoginAnonymous.Location = new System.Drawing.Point(19, 33);
            this.cbLoginAnonymous.Name = "cbLoginAnonymous";
            this.cbLoginAnonymous.Size = new System.Drawing.Size(400, 20);
            this.cbLoginAnonymous.TabIndex = 11;
            this.cbLoginAnonymous.Text = "Login as Anonymous";
            this.cbLoginAnonymous.CheckedChanged += new System.EventHandler(this.cbLoginAnonymous_CheckedChanged);
            // 
            // tbFtpPort
            // 
            this.tbFtpPort.Location = new System.Drawing.Point(199, 80);
            this.tbFtpPort.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.tbFtpPort.Name = "tbFtpPort";
            this.tbFtpPort.Size = new System.Drawing.Size(90, 21);
            this.tbFtpPort.TabIndex = 24;
            this.tbFtpPort.Value = new decimal(new int[] {
            21,
            0,
            0,
            0});
            // 
            // tbFtpDefaultDirectory
            // 
            this.tbFtpDefaultDirectory.Location = new System.Drawing.Point(199, 107);
            this.tbFtpDefaultDirectory.Name = "tbFtpDefaultDirectory";
            this.tbFtpDefaultDirectory.Size = new System.Drawing.Size(248, 21);
            this.tbFtpDefaultDirectory.TabIndex = 23;
            // 
            // tbFtpSiteName
            // 
            this.tbFtpSiteName.Location = new System.Drawing.Point(199, 53);
            this.tbFtpSiteName.Name = "tbFtpSiteName";
            this.tbFtpSiteName.Size = new System.Drawing.Size(248, 21);
            this.tbFtpSiteName.TabIndex = 22;
            // 
            // ultraLabel3
            // 
            appearance7.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel3.Appearance = appearance7;
            this.ultraLabel3.Location = new System.Drawing.Point(22, 110);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(117, 18);
            this.ultraLabel3.TabIndex = 21;
            this.ultraLabel3.Text = "Default Directory:";
            // 
            // ultraLabel2
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel2.Appearance = appearance1;
            this.ultraLabel2.Location = new System.Drawing.Point(22, 82);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(117, 19);
            this.ultraLabel2.TabIndex = 20;
            this.ultraLabel2.Text = "Port:";
            // 
            // ultraLabel1
            // 
            appearance5.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel1.Appearance = appearance5;
            this.ultraLabel1.Location = new System.Drawing.Point(22, 56);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(138, 18);
            this.ultraLabel1.TabIndex = 19;
            this.ultraLabel1.Text = "Site Name / Address:";
            // 
            // FormFtpSettingsBackup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(540, 546);
            this.Controls.Add(this.gbFtpDetails);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormFtpSettingsBackup";
            this.Text = "Copy Data File Settings";
            this.Load += new System.EventHandler(this.FormFtpSettings_Load);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.gbFtpDetails, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbFtpDetails)).EndInit();
            this.gbFtpDetails.ResumeLayout(false);
            this.gbFtpDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.panelFTPCredentials.ResumeLayout(false);
            this.panelFTPCredentials.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbFtpPort)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
		private Infragistics.Win.Misc.UltraGroupBox gbFtpDetails;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
		private System.Windows.Forms.Panel panelFTPCredentials;
		private System.Windows.Forms.TextBox tbFtpPassword;
		private System.Windows.Forms.TextBox tbFtpUser;
		private Infragistics.Win.Misc.UltraLabel ultraLabel5;
		private Infragistics.Win.Misc.UltraLabel ultraLabel4;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbLoginAnonymous;
		private System.Windows.Forms.NumericUpDown tbFtpPort;
		private System.Windows.Forms.TextBox tbFtpDefaultDirectory;
		private System.Windows.Forms.TextBox tbFtpSiteName;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.ComboBox cbFTPType;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
	}
}
