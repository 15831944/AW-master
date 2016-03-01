namespace Layton.AuditWizard.Network
{
	partial class FormUploadAudits
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo5 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click to browse for the audit data folder", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("This is the folder containing the audit files to be uploaded", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 0");
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn2 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("status");
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Upload the selected audit files", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Upload all audit files displayed", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Exit without uploading", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            this.bnBrowseSingle = new System.Windows.Forms.Button();
            this.tbAuditFolder = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lvAudits = new Infragistics.Win.UltraWinListView.UltraListView();
            this.lblAutoRefresh = new System.Windows.Forms.Label();
            this.bnUpload = new System.Windows.Forms.Button();
            this.bnUploadAll = new System.Windows.Forms.Button();
            this.bnClose = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.backgroundWorkerUpload = new System.ComponentModel.BackgroundWorker();
            this.bnCancel = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvAudits)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Network.Properties.Resources.upload_audits_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(102, 453);
            // 
            // bnBrowseSingle
            // 
            this.bnBrowseSingle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnBrowseSingle.Location = new System.Drawing.Point(497, 50);
            this.bnBrowseSingle.Name = "bnBrowseSingle";
            this.bnBrowseSingle.Size = new System.Drawing.Size(42, 23);
            this.bnBrowseSingle.TabIndex = 12;
            this.bnBrowseSingle.Text = "...";
            ultraToolTipInfo5.ToolTipText = "Click to browse for the audit data folder";
            this.ultraToolTipManager1.SetUltraToolTip(this.bnBrowseSingle, ultraToolTipInfo5);
            this.bnBrowseSingle.UseVisualStyleBackColor = true;
            this.bnBrowseSingle.Click += new System.EventHandler(this.bnBrowseSingle_Click);
            // 
            // tbAuditFolder
            // 
            this.tbAuditFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAuditFolder.Location = new System.Drawing.Point(12, 50);
            this.tbAuditFolder.Name = "tbAuditFolder";
            this.tbAuditFolder.Size = new System.Drawing.Size(479, 21);
            this.tbAuditFolder.TabIndex = 11;
            ultraToolTipInfo3.ToolTipText = "This is the folder containing the audit files to be uploaded";
            this.ultraToolTipManager1.SetUltraToolTip(this.tbAuditFolder, ultraToolTipInfo3);
            this.tbAuditFolder.TextChanged += new System.EventHandler(this.tbAuditFolder_TextChanged);
            this.tbAuditFolder.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbAuditFolder_KeyDown);
            this.tbAuditFolder.Leave += new System.EventHandler(this.tbAuditFolder_Leave);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Location = new System.Drawing.Point(9, 24);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(340, 13);
            this.lblTitle.TabIndex = 10;
            this.lblTitle.Text = "Please select the folder containing the audit files to upload";
            // 
            // lvAudits
            // 
            this.lvAudits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvAudits.ItemSettings.DefaultImage = global::Layton.AuditWizard.Network.Properties.Resources.audit_upload_16;
            this.lvAudits.Location = new System.Drawing.Point(12, 109);
            this.lvAudits.MainColumn.Text = "Audit File Name";
            this.lvAudits.MainColumn.Width = 195;
            this.lvAudits.Name = "lvAudits";
            this.lvAudits.Size = new System.Drawing.Size(527, 189);
            ultraListViewSubItemColumn1.Key = "SubItemColumn 0";
            ultraListViewSubItemColumn1.Text = "Asset Name";
            ultraListViewSubItemColumn1.Width = 170;
            ultraListViewSubItemColumn2.Key = "status";
            ultraListViewSubItemColumn2.Text = "Date of Audit";
            ultraListViewSubItemColumn2.Width = 160;
            this.lvAudits.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1,
            ultraListViewSubItemColumn2});
            this.lvAudits.TabIndex = 13;
            this.lvAudits.Text = "lvAudits";
            this.lvAudits.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvAudits.DoubleClick += new System.EventHandler(this.Form_DoubleClick);
            this.lvAudits.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.lvAudits_ItemSelectionChanged);
            // 
            // lblAutoRefresh
            // 
            this.lblAutoRefresh.AutoSize = true;
            this.lblAutoRefresh.BackColor = System.Drawing.Color.Transparent;
            this.lblAutoRefresh.Location = new System.Drawing.Point(9, 88);
            this.lblAutoRefresh.Name = "lblAutoRefresh";
            this.lblAutoRefresh.Size = new System.Drawing.Size(354, 13);
            this.lblAutoRefresh.TabIndex = 14;
            this.lblAutoRefresh.Text = "N.B. This list will auto-refresh when new audits are detected.";
            // 
            // bnUpload
            // 
            this.bnUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnUpload.Enabled = false;
            this.bnUpload.Location = new System.Drawing.Point(228, 423);
            this.bnUpload.Name = "bnUpload";
            this.bnUpload.Size = new System.Drawing.Size(99, 24);
            this.bnUpload.TabIndex = 15;
            this.bnUpload.Text = "&Upload";
            ultraToolTipInfo2.ToolTipText = "Upload the selected audit files";
            this.ultraToolTipManager1.SetUltraToolTip(this.bnUpload, ultraToolTipInfo2);
            this.bnUpload.UseVisualStyleBackColor = true;
            this.bnUpload.Click += new System.EventHandler(this.bnUpload_Click);
            // 
            // bnUploadAll
            // 
            this.bnUploadAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnUploadAll.Location = new System.Drawing.Point(334, 423);
            this.bnUploadAll.Name = "bnUploadAll";
            this.bnUploadAll.Size = new System.Drawing.Size(99, 24);
            this.bnUploadAll.TabIndex = 16;
            this.bnUploadAll.Text = "Upload &All";
            ultraToolTipInfo1.ToolTipText = "Upload all audit files displayed";
            this.ultraToolTipManager1.SetUltraToolTip(this.bnUploadAll, ultraToolTipInfo1);
            this.bnUploadAll.UseVisualStyleBackColor = true;
            this.bnUploadAll.Click += new System.EventHandler(this.bnUploadAll_Click);
            // 
            // bnClose
            // 
            this.bnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnClose.Location = new System.Drawing.Point(440, 423);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(99, 24);
            this.bnClose.TabIndex = 17;
            this.bnClose.Tag = "0";
            this.bnClose.Text = "&Close";
            this.bnClose.UseVisualStyleBackColor = true;
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select the Upload Folder";
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.DesktopDirectory;
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // backgroundWorkerUpload
            // 
            this.backgroundWorkerUpload.WorkerReportsProgress = true;
            this.backgroundWorkerUpload.WorkerSupportsCancellation = true;
            this.backgroundWorkerUpload.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerUpload_DoWork);
            this.backgroundWorkerUpload.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerUpload_RunWorkerCompleted);
            this.backgroundWorkerUpload.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerUpload_ProgressChanged);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(440, 423);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(99, 24);
            this.bnCancel.TabIndex = 19;
            this.bnCancel.Tag = "0";
            this.bnCancel.Text = "&Cancel";
            ultraToolTipInfo4.ToolTipText = "Exit without uploading";
            this.ultraToolTipManager1.SetUltraToolTip(this.bnCancel, ultraToolTipInfo4);
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Visible = false;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.BackColor = System.Drawing.Color.Transparent;
            this.lblProgress.Location = new System.Drawing.Point(9, 311);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(530, 40);
            this.lblProgress.TabIndex = 20;
            this.lblProgress.Text = "Status: Ready";
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // FormUploadAudits
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 574);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.bnClose);
            this.Controls.Add(this.bnUploadAll);
            this.Controls.Add(this.bnUpload);
            this.Controls.Add(this.lblAutoRefresh);
            this.Controls.Add(this.lvAudits);
            this.Controls.Add(this.bnBrowseSingle);
            this.Controls.Add(this.tbAuditFolder);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.bnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormUploadAudits";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Upload Audit Data";
            this.Load += new System.EventHandler(this.FormUploadAudits_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormUploadAudits_FormClosing);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.lblTitle, 0);
            this.Controls.SetChildIndex(this.tbAuditFolder, 0);
            this.Controls.SetChildIndex(this.bnBrowseSingle, 0);
            this.Controls.SetChildIndex(this.lvAudits, 0);
            this.Controls.SetChildIndex(this.lblAutoRefresh, 0);
            this.Controls.SetChildIndex(this.bnUpload, 0);
            this.Controls.SetChildIndex(this.bnUploadAll, 0);
            this.Controls.SetChildIndex(this.bnClose, 0);
            this.Controls.SetChildIndex(this.lblProgress, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvAudits)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bnBrowseSingle;
		private System.Windows.Forms.TextBox tbAuditFolder;
		private System.Windows.Forms.Label lblTitle;
		private Infragistics.Win.UltraWinListView.UltraListView lvAudits;
		private System.Windows.Forms.Label lblAutoRefresh;
		private System.Windows.Forms.Button bnUpload;
		private System.Windows.Forms.Button bnUploadAll;
		private System.Windows.Forms.Button bnClose;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.ComponentModel.BackgroundWorker backgroundWorkerUpload;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Label lblProgress;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
	}
}