namespace Layton.AuditWizard.Administration
{
	partial class DatabaseTabView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseTabView));
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.gbDatabaseMaintenance = new System.Windows.Forms.GroupBox();
            this.bnDatabaseMaintenance = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.ubChangeDB = new Infragistics.Win.Misc.UltraGroupBox();
            this.lbDBDetails = new System.Windows.Forms.Label();
            this.lbDBType = new System.Windows.Forms.Label();
            this.btnChangeDB = new System.Windows.Forms.Button();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.bnImportHistory = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.bnImportPicklists = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.bnImportUserData = new System.Windows.Forms.Button();
            this.bnInportAssets = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.bnPurgeNow = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.nupAssets = new System.Windows.Forms.NumericUpDown();
            this.cbAssetsUnits = new System.Windows.Forms.ComboBox();
            this.nupInternet = new System.Windows.Forms.NumericUpDown();
            this.cbInternetUnits = new System.Windows.Forms.ComboBox();
            this.nupHistory = new System.Windows.Forms.NumericUpDown();
            this.cbHistoryUnits = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbEnablePurge = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.databaseTabControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.headerGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.headerLabel = new Infragistics.Win.Misc.UltraLabel();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.footerPictureBox = new System.Windows.Forms.PictureBox();
            this.ultraTabPageControl1.SuspendLayout();
            this.gbDatabaseMaintenance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ubChangeDB)).BeginInit();
            this.ubChangeDB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupAssets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupInternet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.databaseTabControl)).BeginInit();
            this.databaseTabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).BeginInit();
            this.headerGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.AutoScroll = true;
            this.ultraTabPageControl1.Controls.Add(this.gbDatabaseMaintenance);
            this.ultraTabPageControl1.Controls.Add(this.ubChangeDB);
            this.ultraTabPageControl1.Controls.Add(this.ultraGroupBox2);
            this.ultraTabPageControl1.Controls.Add(this.ultraGroupBox1);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(0, 0);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(817, 669);
            // 
            // gbDatabaseMaintenance
            // 
            this.gbDatabaseMaintenance.Controls.Add(this.bnDatabaseMaintenance);
            this.gbDatabaseMaintenance.Controls.Add(this.label9);
            this.gbDatabaseMaintenance.Location = new System.Drawing.Point(19, 570);
            this.gbDatabaseMaintenance.Name = "gbDatabaseMaintenance";
            this.gbDatabaseMaintenance.Size = new System.Drawing.Size(778, 96);
            this.gbDatabaseMaintenance.TabIndex = 2;
            this.gbDatabaseMaintenance.TabStop = false;
            this.gbDatabaseMaintenance.Text = "Database Maintenance";
            // 
            // bnDatabaseMaintenance
            // 
            this.bnDatabaseMaintenance.Location = new System.Drawing.Point(35, 56);
            this.bnDatabaseMaintenance.Name = "bnDatabaseMaintenance";
            this.bnDatabaseMaintenance.Size = new System.Drawing.Size(156, 23);
            this.bnDatabaseMaintenance.TabIndex = 11;
            this.bnDatabaseMaintenance.Text = "Enter SQL statements";
            this.bnDatabaseMaintenance.UseVisualStyleBackColor = true;
            this.bnDatabaseMaintenance.Click += new System.EventHandler(this.bnDatabaseMaintenance_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 30);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(447, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Click button to enter and run SQL statements on your AuditWizard database:";
            // 
            // ubChangeDB
            // 
            this.ubChangeDB.Controls.Add(this.lbDBDetails);
            this.ubChangeDB.Controls.Add(this.lbDBType);
            this.ubChangeDB.Controls.Add(this.btnChangeDB);
            this.ubChangeDB.Location = new System.Drawing.Point(19, 17);
            this.ubChangeDB.Name = "ubChangeDB";
            this.ubChangeDB.Size = new System.Drawing.Size(780, 133);
            this.ubChangeDB.TabIndex = 1;
            this.ubChangeDB.Text = "Change Database";
            // 
            // lbDBDetails
            // 
            this.lbDBDetails.AutoSize = true;
            this.lbDBDetails.Location = new System.Drawing.Point(37, 64);
            this.lbDBDetails.Name = "lbDBDetails";
            this.lbDBDetails.Size = new System.Drawing.Size(0, 13);
            this.lbDBDetails.TabIndex = 6;
            // 
            // lbDBType
            // 
            this.lbDBType.AutoSize = true;
            this.lbDBType.Location = new System.Drawing.Point(34, 33);
            this.lbDBType.Name = "lbDBType";
            this.lbDBType.Size = new System.Drawing.Size(199, 13);
            this.lbDBType.TabIndex = 5;
            this.lbDBType.Text = "You are currently connected to... ";
            // 
            // btnChangeDB
            // 
            this.btnChangeDB.Location = new System.Drawing.Point(35, 94);
            this.btnChangeDB.Name = "btnChangeDB";
            this.btnChangeDB.Size = new System.Drawing.Size(122, 23);
            this.btnChangeDB.TabIndex = 1;
            this.btnChangeDB.Text = "Change Database";
            this.btnChangeDB.UseVisualStyleBackColor = true;
            this.btnChangeDB.Click += new System.EventHandler(this.btnChangeDB_Click);
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.label8);
            this.ultraGroupBox2.Controls.Add(this.bnImportHistory);
            this.ultraGroupBox2.Controls.Add(this.label7);
            this.ultraGroupBox2.Controls.Add(this.bnImportPicklists);
            this.ultraGroupBox2.Controls.Add(this.label6);
            this.ultraGroupBox2.Controls.Add(this.label5);
            this.ultraGroupBox2.Controls.Add(this.bnImportUserData);
            this.ultraGroupBox2.Controls.Add(this.bnInportAssets);
            this.ultraGroupBox2.Controls.Add(this.label4);
            this.ultraGroupBox2.Location = new System.Drawing.Point(17, 356);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(780, 208);
            this.ultraGroupBox2.TabIndex = 0;
            this.ultraGroupBox2.Text = "Import && Export";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(19, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(742, 42);
            this.label8.TabIndex = 0;
            this.label8.Text = resources.GetString("label8.Text");
            // 
            // bnImportHistory
            // 
            this.bnImportHistory.Location = new System.Drawing.Point(314, 171);
            this.bnImportHistory.Name = "bnImportHistory";
            this.bnImportHistory.Size = new System.Drawing.Size(87, 23);
            this.bnImportHistory.TabIndex = 8;
            this.bnImportHistory.Text = "Import";
            this.bnImportHistory.UseVisualStyleBackColor = true;
            this.bnImportHistory.Click += new System.EventHandler(this.bnImportHistory_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(34, 176);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(239, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Click here to IMPORT Asset History Data";
            // 
            // bnImportPicklists
            // 
            this.bnImportPicklists.Location = new System.Drawing.Point(314, 113);
            this.bnImportPicklists.Name = "bnImportPicklists";
            this.bnImportPicklists.Size = new System.Drawing.Size(87, 23);
            this.bnImportPicklists.TabIndex = 4;
            this.bnImportPicklists.Text = "Import";
            this.bnImportPicklists.UseVisualStyleBackColor = true;
            this.bnImportPicklists.Click += new System.EventHandler(this.bnImportPicklists_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(34, 118);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(240, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Click here to IMPORT Picklists and Items";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(274, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Click here to IMPORT User Defined Data Fields";
            // 
            // bnImportUserData
            // 
            this.bnImportUserData.Location = new System.Drawing.Point(314, 142);
            this.bnImportUserData.Name = "bnImportUserData";
            this.bnImportUserData.Size = new System.Drawing.Size(87, 23);
            this.bnImportUserData.TabIndex = 6;
            this.bnImportUserData.Text = "Import";
            this.bnImportUserData.UseVisualStyleBackColor = true;
            this.bnImportUserData.Click += new System.EventHandler(this.bnImportUserData_Click);
            // 
            // bnInportAssets
            // 
            this.bnInportAssets.Location = new System.Drawing.Point(314, 84);
            this.bnInportAssets.Name = "bnInportAssets";
            this.bnInportAssets.Size = new System.Drawing.Size(87, 23);
            this.bnInportAssets.TabIndex = 2;
            this.bnInportAssets.Text = "Import";
            this.bnInportAssets.UseVisualStyleBackColor = true;
            this.bnInportAssets.Click += new System.EventHandler(this.bnImportAssets_click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(257, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Click here to IMPORT Assets and Locations:";
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.bnPurgeNow);
            this.ultraGroupBox1.Controls.Add(this.label3);
            this.ultraGroupBox1.Controls.Add(this.nupAssets);
            this.ultraGroupBox1.Controls.Add(this.cbAssetsUnits);
            this.ultraGroupBox1.Controls.Add(this.nupInternet);
            this.ultraGroupBox1.Controls.Add(this.cbInternetUnits);
            this.ultraGroupBox1.Controls.Add(this.nupHistory);
            this.ultraGroupBox1.Controls.Add(this.cbHistoryUnits);
            this.ultraGroupBox1.Controls.Add(this.label2);
            this.ultraGroupBox1.Controls.Add(this.label1);
            this.ultraGroupBox1.Controls.Add(this.cbEnablePurge);
            this.ultraGroupBox1.Location = new System.Drawing.Point(17, 156);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(780, 194);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "Database Purging";
            // 
            // bnPurgeNow
            // 
            this.bnPurgeNow.Location = new System.Drawing.Point(37, 160);
            this.bnPurgeNow.Name = "bnPurgeNow";
            this.bnPurgeNow.Size = new System.Drawing.Size(87, 23);
            this.bnPurgeNow.TabIndex = 1;
            this.bnPurgeNow.Text = "Purge &Now";
            this.bnPurgeNow.UseVisualStyleBackColor = true;
            this.bnPurgeNow.Click += new System.EventHandler(this.bnPurgeNow_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(187, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Purge Non-Audited Assets After";
            // 
            // nupAssets
            // 
            this.nupAssets.Location = new System.Drawing.Point(228, 122);
            this.nupAssets.Name = "nupAssets";
            this.nupAssets.Size = new System.Drawing.Size(72, 21);
            this.nupAssets.TabIndex = 9;
            // 
            // cbAssetsUnits
            // 
            this.cbAssetsUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAssetsUnits.FormattingEnabled = true;
            this.cbAssetsUnits.Items.AddRange(new object[] {
            "Days",
            "Months",
            "Years",
            "Never"});
            this.cbAssetsUnits.Location = new System.Drawing.Point(314, 121);
            this.cbAssetsUnits.Name = "cbAssetsUnits";
            this.cbAssetsUnits.Size = new System.Drawing.Size(124, 21);
            this.cbAssetsUnits.TabIndex = 10;
            this.cbAssetsUnits.SelectedIndexChanged += new System.EventHandler(this.cbAssetsUnits_SelectedIndexChanged);
            // 
            // nupInternet
            // 
            this.nupInternet.Location = new System.Drawing.Point(228, 95);
            this.nupInternet.Name = "nupInternet";
            this.nupInternet.Size = new System.Drawing.Size(72, 21);
            this.nupInternet.TabIndex = 6;
            // 
            // cbInternetUnits
            // 
            this.cbInternetUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInternetUnits.FormattingEnabled = true;
            this.cbInternetUnits.Items.AddRange(new object[] {
            "Days",
            "Months",
            "Years",
            "Never"});
            this.cbInternetUnits.Location = new System.Drawing.Point(314, 94);
            this.cbInternetUnits.Name = "cbInternetUnits";
            this.cbInternetUnits.Size = new System.Drawing.Size(124, 21);
            this.cbInternetUnits.TabIndex = 7;
            this.cbInternetUnits.SelectedIndexChanged += new System.EventHandler(this.cbInternetUnits_SelectedIndexChanged);
            // 
            // nupHistory
            // 
            this.nupHistory.Location = new System.Drawing.Point(228, 69);
            this.nupHistory.Name = "nupHistory";
            this.nupHistory.Size = new System.Drawing.Size(72, 21);
            this.nupHistory.TabIndex = 3;
            // 
            // cbHistoryUnits
            // 
            this.cbHistoryUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHistoryUnits.FormattingEnabled = true;
            this.cbHistoryUnits.Items.AddRange(new object[] {
            "Days",
            "Months",
            "Years",
            "Never"});
            this.cbHistoryUnits.Location = new System.Drawing.Point(314, 68);
            this.cbHistoryUnits.Name = "cbHistoryUnits";
            this.cbHistoryUnits.Size = new System.Drawing.Size(124, 21);
            this.cbHistoryUnits.TabIndex = 4;
            this.cbHistoryUnits.SelectedIndexChanged += new System.EventHandler(this.cbHistoryUnits_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(172, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Purge Internet Records After";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Purge Asset History After";
            // 
            // cbEnablePurge
            // 
            this.cbEnablePurge.Location = new System.Drawing.Point(37, 33);
            this.cbEnablePurge.Name = "cbEnablePurge";
            this.cbEnablePurge.Size = new System.Drawing.Size(251, 20);
            this.cbEnablePurge.TabIndex = 0;
            this.cbEnablePurge.Text = "Enable Automatic Database Purging";
            // 
            // databaseTabControl
            // 
            this.databaseTabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.databaseTabControl.Controls.Add(this.ultraTabPageControl1);
            this.databaseTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databaseTabControl.Location = new System.Drawing.Point(0, 80);
            this.databaseTabControl.Name = "databaseTabControl";
            this.databaseTabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.databaseTabControl.Size = new System.Drawing.Size(817, 669);
            this.databaseTabControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
            this.databaseTabControl.TabIndex = 3;
            this.databaseTabControl.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.TopLeft;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "Database";
            this.databaseTabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(817, 669);
            // 
            // headerGroupBox
            // 
            this.headerGroupBox.Controls.Add(this.headerLabel);
            this.headerGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerGroupBox.Location = new System.Drawing.Point(0, 0);
            this.headerGroupBox.Name = "headerGroupBox";
            this.headerGroupBox.Size = new System.Drawing.Size(817, 80);
            this.headerGroupBox.TabIndex = 8;
            this.headerGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // headerLabel
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            appearance1.Image = global::Layton.AuditWizard.Administration.Properties.Resources.database_72;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.headerLabel.Appearance = appearance1;
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ImageSize = new System.Drawing.Size(72, 72);
            this.headerLabel.Location = new System.Drawing.Point(6, 3);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.headerLabel.Size = new System.Drawing.Size(288, 72);
            this.headerLabel.TabIndex = 5;
            this.headerLabel.Text = "Database Maintenance";
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "csv";
            this.openFileDialog.Filter = "Comma Delimited Files (*.csv)|*.csv|All files|*.*||\"";
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.footerPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.email_settings_footer;
            this.footerPictureBox.Location = new System.Drawing.Point(510, 440);
            this.footerPictureBox.Name = "footerPictureBox";
            this.footerPictureBox.Size = new System.Drawing.Size(312, 92);
            this.footerPictureBox.TabIndex = 7;
            this.footerPictureBox.TabStop = false;
            // 
            // DatabaseTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.databaseTabControl);
            this.Controls.Add(this.headerGroupBox);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DatabaseTabView";
            this.Size = new System.Drawing.Size(817, 749);
            this.Leave += new System.EventHandler(this.Leave_Control);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.gbDatabaseMaintenance.ResumeLayout(false);
            this.gbDatabaseMaintenance.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ubChangeDB)).EndInit();
            this.ubChangeDB.ResumeLayout(false);
            this.ubChangeDB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupAssets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupInternet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.databaseTabControl)).EndInit();
            this.databaseTabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).EndInit();
            this.headerGroupBox.ResumeLayout(false);
            this.headerGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private Infragistics.Win.UltraWinTabControl.UltraTabControl databaseTabControl;
		private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
		private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
		private System.Windows.Forms.PictureBox footerPictureBox;
		private Infragistics.Win.Misc.UltraGroupBox headerGroupBox;
		private Infragistics.Win.Misc.UltraLabel headerLabel;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
		private System.Windows.Forms.NumericUpDown nupAssets;
		private System.Windows.Forms.ComboBox cbAssetsUnits;
		private System.Windows.Forms.NumericUpDown nupInternet;
		private System.Windows.Forms.ComboBox cbInternetUnits;
		private System.Windows.Forms.NumericUpDown nupHistory;
		private System.Windows.Forms.ComboBox cbHistoryUnits;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbEnablePurge;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button bnPurgeNow;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.Button bnImportUserData;
		private System.Windows.Forms.Button bnInportAssets;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Button bnImportPicklists;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button bnImportHistory;
		private System.Windows.Forms.Label label7;
        private Infragistics.Win.Misc.UltraGroupBox ubChangeDB;
        private System.Windows.Forms.Button btnChangeDB;
        private System.Windows.Forms.Label lbDBType;
        private System.Windows.Forms.Label lbDBDetails;
        private System.Windows.Forms.GroupBox gbDatabaseMaintenance;
        private System.Windows.Forms.Button bnDatabaseMaintenance;
        private System.Windows.Forms.Label label9;
    }
}
