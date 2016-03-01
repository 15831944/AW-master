namespace Layton.AuditWizard.Network
{
	partial class FormFindAsset
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
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 0");
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn2 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 1");
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn3 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 2");
            this.lvResults = new Infragistics.Win.UltraWinListView.UltraListView();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnFind = new System.Windows.Forms.Button();
            this.tbFindAssetName = new System.Windows.Forms.TextBox();
            this.lbWildcardInfo = new System.Windows.Forms.Label();
            this.checkBoxAssetNames = new System.Windows.Forms.CheckBox();
            this.checkBoxInstalledApplications = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxUserDefined = new System.Windows.Forms.CheckBox();
            this.checkBoxMACAddress = new System.Windows.Forms.CheckBox();
            this.checkBoxIPAddress = new System.Windows.Forms.CheckBox();
            this.checkBoxSerialNumber = new System.Windows.Forms.CheckBox();
            this.checkBoxAssetModel = new System.Windows.Forms.CheckBox();
            this.checkBoxAssetMake = new System.Windows.Forms.CheckBox();
            this.checkBoxAssetTag = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxInternet = new System.Windows.Forms.CheckBox();
            this.checkBoxScannedFiles = new System.Windows.Forms.CheckBox();
            this.checkBoxHardware = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.selectLocationsControl = new Layton.AuditWizard.Common.SelectLocationsControl();
            this.lblResults = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.lvResults)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvResults
            // 
            this.lvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvResults.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            this.lvResults.ItemSettings.DefaultImage = global::Layton.AuditWizard.Network.Properties.Resources.computer16;
            this.lvResults.ItemSettings.HideSelection = false;
            this.lvResults.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.Single;
            this.lvResults.Location = new System.Drawing.Point(12, 414);
            this.lvResults.MainColumn.AllowSorting = Infragistics.Win.DefaultableBoolean.True;
            this.lvResults.MainColumn.Sorting = Infragistics.Win.UltraWinListView.Sorting.Ascending;
            this.lvResults.MainColumn.Text = "Asset Name";
            this.lvResults.MainColumn.Width = 200;
            this.lvResults.Name = "lvResults";
            this.lvResults.Size = new System.Drawing.Size(978, 276);
            ultraListViewSubItemColumn1.Key = "SubItemColumn 0";
            ultraListViewSubItemColumn1.Text = "Location";
            ultraListViewSubItemColumn1.Width = 280;
            ultraListViewSubItemColumn2.Key = "SubItemColumn 1";
            ultraListViewSubItemColumn2.Text = "Found In";
            ultraListViewSubItemColumn2.Width = 160;
            ultraListViewSubItemColumn3.Key = "SubItemColumn 2";
            ultraListViewSubItemColumn3.Text = "Item";
            ultraListViewSubItemColumn3.Width = 332;
            this.lvResults.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1,
            ultraListViewSubItemColumn2,
            ultraListViewSubItemColumn3});
            this.lvResults.TabIndex = 10;
            this.lvResults.Text = "ultraListView1";
            this.lvResults.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvResults.ViewSettingsDetails.ColumnAutoSizeMode = Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.VisibleItems;
            this.lvResults.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.lvResults_ItemSelectionChanged);
            this.lvResults.ItemDoubleClick += new Infragistics.Win.UltraWinListView.ItemDoubleClickEventHandler(this.lvResults_ItemDoubleClick);
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Enabled = false;
            this.bnOK.Location = new System.Drawing.Point(801, 696);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(92, 24);
            this.bnOK.TabIndex = 23;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(900, 696);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(92, 24);
            this.bnCancel.TabIndex = 24;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnFind
            // 
            this.bnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnFind.Enabled = false;
            this.bnFind.Location = new System.Drawing.Point(473, 61);
            this.bnFind.Name = "bnFind";
            this.bnFind.Size = new System.Drawing.Size(65, 24);
            this.bnFind.TabIndex = 28;
            this.bnFind.Text = "&Find";
            this.bnFind.UseVisualStyleBackColor = true;
            this.bnFind.Click += new System.EventHandler(this.bnFind_Click);
            // 
            // tbFindAssetName
            // 
            this.tbFindAssetName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFindAssetName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Layton.AuditWizard.Network.Properties.Settings.Default, "LastSearchString", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbFindAssetName.Location = new System.Drawing.Point(20, 63);
            this.tbFindAssetName.Name = "tbFindAssetName";
            this.tbFindAssetName.Size = new System.Drawing.Size(442, 21);
            this.tbFindAssetName.TabIndex = 27;
            this.tbFindAssetName.Text = global::Layton.AuditWizard.Network.Properties.Settings.Default.LastSearchString;
            this.tbFindAssetName.TextChanged += new System.EventHandler(this.tbFindAssetName_TextChanged);
            this.tbFindAssetName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbFindAssetName_KeyPress);
            // 
            // lbWildcardInfo
            // 
            this.lbWildcardInfo.AutoSize = true;
            this.lbWildcardInfo.Location = new System.Drawing.Point(17, 37);
            this.lbWildcardInfo.Name = "lbWildcardInfo";
            this.lbWildcardInfo.Size = new System.Drawing.Size(414, 13);
            this.lbWildcardInfo.TabIndex = 29;
            this.lbWildcardInfo.Text = "If you wish to perform a wildcard search, please use the \'%\' character.";
            // 
            // checkBoxAssetNames
            // 
            this.checkBoxAssetNames.AutoSize = true;
            this.checkBoxAssetNames.Location = new System.Drawing.Point(21, 31);
            this.checkBoxAssetNames.Name = "checkBoxAssetNames";
            this.checkBoxAssetNames.Size = new System.Drawing.Size(94, 17);
            this.checkBoxAssetNames.TabIndex = 30;
            this.checkBoxAssetNames.Text = "Asset Name";
            this.checkBoxAssetNames.UseVisualStyleBackColor = true;
            // 
            // checkBoxInstalledApplications
            // 
            this.checkBoxInstalledApplications.AutoSize = true;
            this.checkBoxInstalledApplications.Location = new System.Drawing.Point(21, 121);
            this.checkBoxInstalledApplications.Name = "checkBoxInstalledApplications";
            this.checkBoxInstalledApplications.Size = new System.Drawing.Size(147, 17);
            this.checkBoxInstalledApplications.TabIndex = 31;
            this.checkBoxInstalledApplications.Text = "Installed Applications";
            this.checkBoxInstalledApplications.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxUserDefined);
            this.groupBox1.Controls.Add(this.checkBoxMACAddress);
            this.groupBox1.Controls.Add(this.checkBoxIPAddress);
            this.groupBox1.Controls.Add(this.checkBoxSerialNumber);
            this.groupBox1.Controls.Add(this.checkBoxAssetModel);
            this.groupBox1.Controls.Add(this.checkBoxAssetMake);
            this.groupBox1.Controls.Add(this.checkBoxAssetTag);
            this.groupBox1.Controls.Add(this.checkBoxAssetNames);
            this.groupBox1.Location = new System.Drawing.Point(21, 140);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 167);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Asset Details";
            // 
            // checkBoxUserDefined
            // 
            this.checkBoxUserDefined.AutoSize = true;
            this.checkBoxUserDefined.Location = new System.Drawing.Point(157, 121);
            this.checkBoxUserDefined.Name = "checkBoxUserDefined";
            this.checkBoxUserDefined.Size = new System.Drawing.Size(136, 17);
            this.checkBoxUserDefined.TabIndex = 37;
            this.checkBoxUserDefined.Text = "User Defined Fields";
            this.checkBoxUserDefined.UseVisualStyleBackColor = true;
            // 
            // checkBoxMACAddress
            // 
            this.checkBoxMACAddress.AutoSize = true;
            this.checkBoxMACAddress.Location = new System.Drawing.Point(157, 91);
            this.checkBoxMACAddress.Name = "checkBoxMACAddress";
            this.checkBoxMACAddress.Size = new System.Drawing.Size(102, 17);
            this.checkBoxMACAddress.TabIndex = 36;
            this.checkBoxMACAddress.Text = "MAC Address";
            this.checkBoxMACAddress.UseVisualStyleBackColor = true;
            // 
            // checkBoxIPAddress
            // 
            this.checkBoxIPAddress.AutoSize = true;
            this.checkBoxIPAddress.Location = new System.Drawing.Point(157, 61);
            this.checkBoxIPAddress.Name = "checkBoxIPAddress";
            this.checkBoxIPAddress.Size = new System.Drawing.Size(88, 17);
            this.checkBoxIPAddress.TabIndex = 35;
            this.checkBoxIPAddress.Text = "IP Address";
            this.checkBoxIPAddress.UseVisualStyleBackColor = true;
            // 
            // checkBoxSerialNumber
            // 
            this.checkBoxSerialNumber.AutoSize = true;
            this.checkBoxSerialNumber.Location = new System.Drawing.Point(157, 31);
            this.checkBoxSerialNumber.Name = "checkBoxSerialNumber";
            this.checkBoxSerialNumber.Size = new System.Drawing.Size(108, 17);
            this.checkBoxSerialNumber.TabIndex = 34;
            this.checkBoxSerialNumber.Text = "Serial Number";
            this.checkBoxSerialNumber.UseVisualStyleBackColor = true;
            // 
            // checkBoxAssetModel
            // 
            this.checkBoxAssetModel.AutoSize = true;
            this.checkBoxAssetModel.Location = new System.Drawing.Point(21, 121);
            this.checkBoxAssetModel.Name = "checkBoxAssetModel";
            this.checkBoxAssetModel.Size = new System.Drawing.Size(94, 17);
            this.checkBoxAssetModel.TabIndex = 33;
            this.checkBoxAssetModel.Text = "Asset Model";
            this.checkBoxAssetModel.UseVisualStyleBackColor = true;
            // 
            // checkBoxAssetMake
            // 
            this.checkBoxAssetMake.AutoSize = true;
            this.checkBoxAssetMake.Location = new System.Drawing.Point(21, 91);
            this.checkBoxAssetMake.Name = "checkBoxAssetMake";
            this.checkBoxAssetMake.Size = new System.Drawing.Size(91, 17);
            this.checkBoxAssetMake.TabIndex = 32;
            this.checkBoxAssetMake.Text = "Asset Make";
            this.checkBoxAssetMake.UseVisualStyleBackColor = true;
            // 
            // checkBoxAssetTag
            // 
            this.checkBoxAssetTag.AutoSize = true;
            this.checkBoxAssetTag.Location = new System.Drawing.Point(21, 61);
            this.checkBoxAssetTag.Name = "checkBoxAssetTag";
            this.checkBoxAssetTag.Size = new System.Drawing.Size(82, 17);
            this.checkBoxAssetTag.TabIndex = 31;
            this.checkBoxAssetTag.Text = "Asset Tag";
            this.checkBoxAssetTag.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxInternet);
            this.groupBox2.Controls.Add(this.checkBoxScannedFiles);
            this.groupBox2.Controls.Add(this.checkBoxInstalledApplications);
            this.groupBox2.Controls.Add(this.checkBoxHardware);
            this.groupBox2.Location = new System.Drawing.Point(338, 140);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 167);
            this.groupBox2.TabIndex = 38;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Other";
            // 
            // checkBoxInternet
            // 
            this.checkBoxInternet.AutoSize = true;
            this.checkBoxInternet.Location = new System.Drawing.Point(21, 91);
            this.checkBoxInternet.Name = "checkBoxInternet";
            this.checkBoxInternet.Size = new System.Drawing.Size(175, 17);
            this.checkBoxInternet.TabIndex = 32;
            this.checkBoxInternet.Text = "Internet History / Cookies";
            this.checkBoxInternet.UseVisualStyleBackColor = true;
            // 
            // checkBoxScannedFiles
            // 
            this.checkBoxScannedFiles.AutoSize = true;
            this.checkBoxScannedFiles.Location = new System.Drawing.Point(21, 61);
            this.checkBoxScannedFiles.Name = "checkBoxScannedFiles";
            this.checkBoxScannedFiles.Size = new System.Drawing.Size(104, 17);
            this.checkBoxScannedFiles.TabIndex = 31;
            this.checkBoxScannedFiles.Text = "Scanned Files";
            this.checkBoxScannedFiles.UseVisualStyleBackColor = true;
            // 
            // checkBoxHardware
            // 
            this.checkBoxHardware.AutoSize = true;
            this.checkBoxHardware.Location = new System.Drawing.Point(21, 31);
            this.checkBoxHardware.Name = "checkBoxHardware";
            this.checkBoxHardware.Size = new System.Drawing.Size(118, 17);
            this.checkBoxHardware.TabIndex = 30;
            this.checkBoxHardware.Text = "Hardware Items";
            this.checkBoxHardware.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.lbWildcardInfo);
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Controls.Add(this.bnFind);
            this.groupBox3.Controls.Add(this.tbFindAssetName);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(978, 377);
            this.groupBox3.TabIndex = 39;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search Criteria";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.selectLocationsControl);
            this.groupBox4.Location = new System.Drawing.Point(592, 24);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(380, 336);
            this.groupBox4.TabIndex = 39;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Search Scope";
            // 
            // selectLocationsControl
            // 
            this.selectLocationsControl.BackColor = System.Drawing.Color.White;
            this.selectLocationsControl.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.selectLocationsControl.Location = new System.Drawing.Point(19, 20);
            this.selectLocationsControl.Name = "selectLocationsControl";
            this.selectLocationsControl.ShowByDomain = false;
            this.selectLocationsControl.Size = new System.Drawing.Size(342, 304);
            this.selectLocationsControl.TabIndex = 28;
            // 
            // labelResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Location = new System.Drawing.Point(12, 702);
            this.lblResults.Name = "labelResults";
            this.lblResults.Size = new System.Drawing.Size(41, 13);
            this.lblResults.TabIndex = 40;
            this.lblResults.Text = "Found";
            this.lblResults.Visible = false;
            // 
            // FormFindAsset
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.lblResults);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.lvResults);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFindAsset";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search AuditWizard";
            this.Load += new System.EventHandler(this.FormFindAsset_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lvResults)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinListView.UltraListView lvResults;
		private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.TextBox tbFindAssetName;
		private System.Windows.Forms.Button bnFind;
        private System.Windows.Forms.Label lbWildcardInfo;
        private System.Windows.Forms.CheckBox checkBoxAssetNames;
        private System.Windows.Forms.CheckBox checkBoxInstalledApplications;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxAssetTag;
        private System.Windows.Forms.CheckBox checkBoxMACAddress;
        private System.Windows.Forms.CheckBox checkBoxIPAddress;
        private System.Windows.Forms.CheckBox checkBoxSerialNumber;
        private System.Windows.Forms.CheckBox checkBoxAssetModel;
        private System.Windows.Forms.CheckBox checkBoxAssetMake;
        private System.Windows.Forms.CheckBox checkBoxUserDefined;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxInternet;
        private System.Windows.Forms.CheckBox checkBoxScannedFiles;
        private System.Windows.Forms.CheckBox checkBoxHardware;
        private System.Windows.Forms.GroupBox groupBox3;
        private Layton.AuditWizard.Common.SelectLocationsControl selectLocationsControl;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lblResults;
	}
}
