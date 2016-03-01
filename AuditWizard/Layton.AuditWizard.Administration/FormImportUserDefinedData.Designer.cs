namespace Layton.AuditWizard.Administration
{
	partial class FormImportUserDefinedData
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
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn3 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("Category");
            this.lvRecords = new Infragistics.Win.UltraWinListView.UltraListView();
            this.label2 = new System.Windows.Forms.Label();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.importProgress = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvRecords)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.import_userdata_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(376, 437);
            this.footerPictureBox.Size = new System.Drawing.Size(447, 120);
            // 
            // lvRecords
            // 
            this.lvRecords.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvRecords.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.None;
            this.lvRecords.Location = new System.Drawing.Point(14, 54);
            this.lvRecords.MainColumn.Text = "Asset Name";
            this.lvRecords.MainColumn.VisiblePositionInDetailsView = 0;
            this.lvRecords.MainColumn.Width = 200;
            this.lvRecords.Name = "lvRecords";
            this.lvRecords.Size = new System.Drawing.Size(794, 312);
            ultraListViewSubItemColumn1.Key = "SubItemColumn 0";
            ultraListViewSubItemColumn1.Text = "Field Name";
            ultraListViewSubItemColumn1.VisiblePositionInDetailsView = 2;
            ultraListViewSubItemColumn1.Width = 200;
            ultraListViewSubItemColumn2.Key = "SubItemColumn 1";
            ultraListViewSubItemColumn2.Text = "Field Value";
            ultraListViewSubItemColumn2.VisiblePositionInDetailsView = 3;
            ultraListViewSubItemColumn2.Width = 200;
            ultraListViewSubItemColumn3.DataType = typeof(string);
            ultraListViewSubItemColumn3.Key = "Category";
            ultraListViewSubItemColumn3.VisiblePositionInDetailsView = 1;
            this.lvRecords.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1,
            ultraListViewSubItemColumn2,
            ultraListViewSubItemColumn3});
            this.lvRecords.TabIndex = 10;
            this.lvRecords.Text = "ultraListView1";
            this.lvRecords.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvRecords.ViewSettingsDetails.CheckBoxStyle = Infragistics.Win.UltraWinListView.CheckBoxStyle.CheckBox;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(14, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(794, 30);
            this.label2.TabIndex = 21;
            this.label2.Text = "Please check from the list below those entries that you would like to import into" +
                " AuditWizard.";
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(621, 407);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(92, 24);
            this.bnOK.TabIndex = 20;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(716, 407);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(92, 24);
            this.bnCancel.TabIndex = 19;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            // 
            // importProgress
            // 
            this.importProgress.Location = new System.Drawing.Point(14, 378);
            this.importProgress.Name = "importProgress";
            this.importProgress.Size = new System.Drawing.Size(794, 23);
            this.importProgress.TabIndex = 23;
            this.importProgress.Text = "[Percent]";
            // 
            // FormImportUserDefinedData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(822, 557);
            this.Controls.Add(this.importProgress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.lvRecords);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FormImportUserDefinedData";
            this.Text = "Import User Defined Data";
            this.Load += new System.EventHandler(this.FormImportUserDefinedData_Load);
            this.Controls.SetChildIndex(this.lvRecords, 0);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.importProgress, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvRecords)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.UltraWinListView.UltraListView lvRecords;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
		private Infragistics.Win.UltraWinProgressBar.UltraProgressBar importProgress;
	}
}
