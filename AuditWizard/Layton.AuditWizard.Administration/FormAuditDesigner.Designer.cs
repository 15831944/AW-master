namespace Layton.AuditWizard.Administration
{
	partial class FormAuditDesigner
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.cbAllowCancel = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.gbAssetInformation = new Infragistics.Win.Misc.UltraGroupBox();
            this.cbAssetSerial = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cbAssetModel = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cbAssetMake = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cbAssetCategory = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cbDisplayBasicInformation = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cbDisplayAssetLocation = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cbAssetDataScreens = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbAssetInformation)).BeginInit();
            this.gbAssetInformation.SuspendLayout();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.audit_screendesigner_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(14, 364);
            this.footerPictureBox.Size = new System.Drawing.Size(384, 120);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(299, 335);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 48;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(204, 335);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 47;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // cbAllowCancel
            // 
            this.cbAllowCancel.BackColor = System.Drawing.Color.Transparent;
            this.cbAllowCancel.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbAllowCancel.Location = new System.Drawing.Point(14, 25);
            this.cbAllowCancel.Name = "cbAllowCancel";
            this.cbAllowCancel.Size = new System.Drawing.Size(243, 20);
            this.cbAllowCancel.TabIndex = 49;
            this.cbAllowCancel.Text = "Allow the User to Cancel the Audit";
            // 
            // gbAssetInformation
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.gbAssetInformation.Appearance = appearance1;
            this.gbAssetInformation.Controls.Add(this.cbAssetSerial);
            this.gbAssetInformation.Controls.Add(this.ultraLabel1);
            this.gbAssetInformation.Controls.Add(this.cbAssetModel);
            this.gbAssetInformation.Controls.Add(this.cbAssetMake);
            this.gbAssetInformation.Controls.Add(this.cbAssetCategory);
            this.gbAssetInformation.Location = new System.Drawing.Point(14, 68);
            this.gbAssetInformation.Name = "gbAssetInformation";
            this.gbAssetInformation.Size = new System.Drawing.Size(372, 170);
            this.gbAssetInformation.TabIndex = 52;
            // 
            // cbAssetSerial
            // 
            this.cbAssetSerial.BackColor = System.Drawing.Color.Transparent;
            this.cbAssetSerial.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbAssetSerial.Checked = true;
            this.cbAssetSerial.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAssetSerial.Location = new System.Drawing.Point(48, 52);
            this.cbAssetSerial.Name = "cbAssetSerial";
            this.cbAssetSerial.Size = new System.Drawing.Size(243, 20);
            this.cbAssetSerial.TabIndex = 55;
            this.cbAssetSerial.Text = "Asset Serial Number";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Location = new System.Drawing.Point(7, 20);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(409, 23);
            this.ultraLabel1.TabIndex = 54;
            this.ultraLabel1.Text = "Allow the user to enter data into the following checked fields";
            // 
            // cbAssetModel
            // 
            this.cbAssetModel.BackColor = System.Drawing.Color.Transparent;
            this.cbAssetModel.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbAssetModel.Checked = true;
            this.cbAssetModel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAssetModel.Location = new System.Drawing.Point(48, 130);
            this.cbAssetModel.Name = "cbAssetModel";
            this.cbAssetModel.Size = new System.Drawing.Size(243, 20);
            this.cbAssetModel.TabIndex = 52;
            this.cbAssetModel.Text = "Asset Model";
            // 
            // cbAssetMake
            // 
            this.cbAssetMake.BackColor = System.Drawing.Color.Transparent;
            this.cbAssetMake.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbAssetMake.Checked = true;
            this.cbAssetMake.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAssetMake.Location = new System.Drawing.Point(48, 104);
            this.cbAssetMake.Name = "cbAssetMake";
            this.cbAssetMake.Size = new System.Drawing.Size(243, 20);
            this.cbAssetMake.TabIndex = 51;
            this.cbAssetMake.Text = "Asset Manufacturer";
            // 
            // cbAssetCategory
            // 
            this.cbAssetCategory.BackColor = System.Drawing.Color.Transparent;
            this.cbAssetCategory.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbAssetCategory.Checked = true;
            this.cbAssetCategory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAssetCategory.Location = new System.Drawing.Point(48, 78);
            this.cbAssetCategory.Name = "cbAssetCategory";
            this.cbAssetCategory.Size = new System.Drawing.Size(243, 20);
            this.cbAssetCategory.TabIndex = 50;
            this.cbAssetCategory.Text = "Asset Category";
            // 
            // cbDisplayBasicInformation
            // 
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.cbDisplayBasicInformation.Appearance = appearance2;
            this.cbDisplayBasicInformation.BackColor = System.Drawing.Color.Transparent;
            this.cbDisplayBasicInformation.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbDisplayBasicInformation.Checked = true;
            this.cbDisplayBasicInformation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDisplayBasicInformation.Location = new System.Drawing.Point(24, 58);
            this.cbDisplayBasicInformation.Name = "cbDisplayBasicInformation";
            this.cbDisplayBasicInformation.Size = new System.Drawing.Size(259, 20);
            this.cbDisplayBasicInformation.TabIndex = 53;
            this.cbDisplayBasicInformation.Text = "Display Basic Asset Information Screen";
            this.cbDisplayBasicInformation.CheckedChanged += new System.EventHandler(this.cbDisplayBasicInformation_CheckedChanged);
            // 
            // cbDisplayAssetLocation
            // 
            this.cbDisplayAssetLocation.BackColor = System.Drawing.Color.Transparent;
            this.cbDisplayAssetLocation.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbDisplayAssetLocation.Checked = true;
            this.cbDisplayAssetLocation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDisplayAssetLocation.Location = new System.Drawing.Point(14, 253);
            this.cbDisplayAssetLocation.Name = "cbDisplayAssetLocation";
            this.cbDisplayAssetLocation.Size = new System.Drawing.Size(209, 20);
            this.cbDisplayAssetLocation.TabIndex = 55;
            this.cbDisplayAssetLocation.Text = "Display Asset Location Screen";
            this.cbDisplayAssetLocation.CheckedChanged += new System.EventHandler(this.cbDisplayAssetLocation_CheckedChanged);
            // 
            // cbAssetDataScreens
            // 
            this.cbAssetDataScreens.BackColor = System.Drawing.Color.Transparent;
            this.cbAssetDataScreens.BackColorInternal = System.Drawing.Color.Transparent;
            this.cbAssetDataScreens.Checked = true;
            this.cbAssetDataScreens.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAssetDataScreens.Location = new System.Drawing.Point(14, 288);
            this.cbAssetDataScreens.Name = "cbAssetDataScreens";
            this.cbAssetDataScreens.Size = new System.Drawing.Size(269, 20);
            this.cbAssetDataScreens.TabIndex = 56;
            this.cbAssetDataScreens.Text = "Display User Defined Data Screen(s)";
            // 
            // FormAuditDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 485);
            this.Controls.Add(this.cbAssetDataScreens);
            this.Controls.Add(this.cbDisplayAssetLocation);
            this.Controls.Add(this.cbDisplayBasicInformation);
            this.Controls.Add(this.cbAllowCancel);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.gbAssetInformation);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormAuditDesigner";
            this.Text = "Interactive Audit Designer";
            this.Load += new System.EventHandler(this.FormAuditDesigner_Load);
            this.Controls.SetChildIndex(this.gbAssetInformation, 0);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.cbAllowCancel, 0);
            this.Controls.SetChildIndex(this.cbDisplayBasicInformation, 0);
            this.Controls.SetChildIndex(this.cbDisplayAssetLocation, 0);
            this.Controls.SetChildIndex(this.cbAssetDataScreens, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbAssetInformation)).EndInit();
            this.gbAssetInformation.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbAllowCancel;
		private Infragistics.Win.Misc.UltraGroupBox gbAssetInformation;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbAssetModel;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbAssetMake;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbAssetCategory;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbDisplayBasicInformation;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbDisplayAssetLocation;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbAssetDataScreens;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbAssetSerial;
	}
}