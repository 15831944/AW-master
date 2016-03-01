namespace Layton.AuditWizard.Common
{
	partial class DocumentsControl
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
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 1");
            this.ultraGroupBox3 = new Infragistics.Win.Misc.UltraGroupBox();
            this.bnViewDocument = new System.Windows.Forms.Button();
            this.lvDocuments = new Infragistics.Win.UltraWinListView.UltraListView();
            this.bnDeleteDocument = new System.Windows.Forms.Button();
            this.bnAddDocument = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox3)).BeginInit();
            this.ultraGroupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lvDocuments)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox3
            // 
            this.ultraGroupBox3.Controls.Add(this.bnViewDocument);
            this.ultraGroupBox3.Controls.Add(this.lvDocuments);
            this.ultraGroupBox3.Controls.Add(this.bnDeleteDocument);
            this.ultraGroupBox3.Controls.Add(this.bnAddDocument);
            this.ultraGroupBox3.Location = new System.Drawing.Point(3, 3);
            this.ultraGroupBox3.Name = "ultraGroupBox3";
            this.ultraGroupBox3.Size = new System.Drawing.Size(715, 255);
            this.ultraGroupBox3.TabIndex = 26;
            this.ultraGroupBox3.Text = "Documents";
            // 
            // bnViewDocument
            // 
            this.bnViewDocument.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnViewDocument.Enabled = false;
            this.bnViewDocument.Location = new System.Drawing.Point(591, 88);
            this.bnViewDocument.Name = "bnViewDocument";
            this.bnViewDocument.Size = new System.Drawing.Size(92, 24);
            this.bnViewDocument.TabIndex = 33;
            this.bnViewDocument.Text = "&View";
            this.bnViewDocument.UseVisualStyleBackColor = true;
            this.bnViewDocument.Click += new System.EventHandler(this.bnViewDocument_Click);
            // 
            // lvDocuments
            // 
            this.lvDocuments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvDocuments.Location = new System.Drawing.Point(26, 28);
            this.lvDocuments.MainColumn.Text = "Name";
            this.lvDocuments.Name = "lvDocuments";
            this.lvDocuments.Size = new System.Drawing.Size(559, 204);
            ultraListViewSubItemColumn1.Key = "SubItemColumn 1";
            ultraListViewSubItemColumn1.Text = "Path";
            ultraListViewSubItemColumn1.Width = 200;
            this.lvDocuments.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1});
            this.lvDocuments.TabIndex = 32;
            this.lvDocuments.Text = "ultraListView1";
            this.lvDocuments.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvDocuments.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.lvDocuments_ItemSelectionChanged);
            // 
            // bnDeleteDocument
            // 
            this.bnDeleteDocument.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnDeleteDocument.Enabled = false;
            this.bnDeleteDocument.Location = new System.Drawing.Point(591, 58);
            this.bnDeleteDocument.Name = "bnDeleteDocument";
            this.bnDeleteDocument.Size = new System.Drawing.Size(92, 24);
            this.bnDeleteDocument.TabIndex = 31;
            this.bnDeleteDocument.Text = "&Delete";
            this.bnDeleteDocument.UseVisualStyleBackColor = true;
            this.bnDeleteDocument.Click += new System.EventHandler(this.bnDeleteDocument_Click);
            // 
            // bnAddDocument
            // 
            this.bnAddDocument.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnAddDocument.Location = new System.Drawing.Point(591, 28);
            this.bnAddDocument.Name = "bnAddDocument";
            this.bnAddDocument.Size = new System.Drawing.Size(92, 24);
            this.bnAddDocument.TabIndex = 29;
            this.bnAddDocument.Text = "&Add";
            this.bnAddDocument.UseVisualStyleBackColor = true;
            this.bnAddDocument.Click += new System.EventHandler(this.bnAddDocument_Click);
            // 
            // DocumentsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.ultraGroupBox3);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "DocumentsControl";
            this.Size = new System.Drawing.Size(726, 275);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox3)).EndInit();
            this.ultraGroupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lvDocuments)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox3;
		private System.Windows.Forms.Button bnViewDocument;
		private Infragistics.Win.UltraWinListView.UltraListView lvDocuments;
		private System.Windows.Forms.Button bnDeleteDocument;
		private System.Windows.Forms.Button bnAddDocument;

	}
}
