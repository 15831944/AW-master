namespace Layton.AuditWizard.Administration
{
	partial class FormUserLocation
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo4 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Exit saving changes", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Exit discarding changes", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Name that will be associated with this type of license", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Name that will be associated with this type of license", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 0");
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbParent = new System.Windows.Forms.TextBox();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.tbChild = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.bnEdit = new Infragistics.Win.Misc.UltraButton();
            this.bnRemove = new Infragistics.Win.Misc.UltraButton();
            this.bnAdd = new Infragistics.Win.Misc.UltraButton();
            this.ulvTcpRanges = new Infragistics.Win.UltraWinListView.UltraListView();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ulvTcpRanges)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.userlocations_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(159, 370);
            this.footerPictureBox.Size = new System.Drawing.Size(390, 124);
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(349, 334);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(92, 24);
            this.bnOK.TabIndex = 5;
            this.bnOK.Text = "&OK";
            ultraToolTipInfo4.ToolTipText = "Exit saving changes";
            this.ultraToolTipManager1.SetUltraToolTip(this.bnOK, ultraToolTipInfo4);
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(444, 334);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(92, 24);
            this.bnCancel.TabIndex = 6;
            this.bnCancel.Text = "&Cancel";
            ultraToolTipInfo3.ToolTipText = "Exit discarding changes";
            this.ultraToolTipManager1.SetUltraToolTip(this.bnCancel, ultraToolTipInfo3);
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(14, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Parent Location:";
            // 
            // tbParent
            // 
            this.tbParent.Location = new System.Drawing.Point(159, 21);
            this.tbParent.Name = "tbParent";
            this.tbParent.ReadOnly = true;
            this.tbParent.Size = new System.Drawing.Size(377, 21);
            this.tbParent.TabIndex = 3;
            ultraToolTipInfo2.ToolTipText = "Name that will be associated with this type of license";
            this.ultraToolTipManager1.SetUltraToolTip(this.tbParent, ultraToolTipInfo2);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // tbChild
            // 
            this.tbChild.Location = new System.Drawing.Point(159, 57);
            this.tbChild.Name = "tbChild";
            this.tbChild.Size = new System.Drawing.Size(377, 21);
            this.tbChild.TabIndex = 1;
            ultraToolTipInfo1.ToolTipText = "Name that will be associated with this type of license";
            this.ultraToolTipManager1.SetUltraToolTip(this.tbChild, ultraToolTipInfo1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(14, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Location Name:";
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.bnEdit);
            this.ultraGroupBox1.Controls.Add(this.bnRemove);
            this.ultraGroupBox1.Controls.Add(this.bnAdd);
            this.ultraGroupBox1.Controls.Add(this.ulvTcpRanges);
            this.ultraGroupBox1.Location = new System.Drawing.Point(17, 101);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(519, 213);
            this.ultraGroupBox1.TabIndex = 4;
            this.ultraGroupBox1.Text = "TCP/IP Address Ranges for this Location";
            // 
            // bnEdit
            // 
            this.bnEdit.Enabled = false;
            this.bnEdit.Location = new System.Drawing.Point(420, 61);
            this.bnEdit.Name = "bnEdit";
            this.bnEdit.Size = new System.Drawing.Size(92, 24);
            this.bnEdit.TabIndex = 1;
            this.bnEdit.Text = "&Edit";
            this.bnEdit.Click += new System.EventHandler(this.bnEdit_Click);
            // 
            // bnRemove
            // 
            this.bnRemove.Enabled = false;
            this.bnRemove.Location = new System.Drawing.Point(420, 91);
            this.bnRemove.Name = "bnRemove";
            this.bnRemove.Size = new System.Drawing.Size(92, 24);
            this.bnRemove.TabIndex = 2;
            this.bnRemove.Text = "&Remove";
            this.bnRemove.Click += new System.EventHandler(this.bnRemove_Click);
            // 
            // bnAdd
            // 
            this.bnAdd.Location = new System.Drawing.Point(420, 31);
            this.bnAdd.Name = "bnAdd";
            this.bnAdd.Size = new System.Drawing.Size(92, 24);
            this.bnAdd.TabIndex = 0;
            this.bnAdd.Text = "&Add";
            this.bnAdd.Click += new System.EventHandler(this.bnAdd_Click);
            // 
            // ulvTcpRanges
            // 
            this.ulvTcpRanges.ItemSettings.DefaultImage = global::Layton.AuditWizard.Administration.Properties.Resources.ipaddress_16;
            this.ulvTcpRanges.Location = new System.Drawing.Point(17, 31);
            this.ulvTcpRanges.MainColumn.Text = "Starting IP Address";
            this.ulvTcpRanges.MainColumn.Width = 150;
            this.ulvTcpRanges.Name = "ulvTcpRanges";
            this.ulvTcpRanges.Size = new System.Drawing.Size(395, 163);
            ultraListViewSubItemColumn1.Key = "SubItemColumn 0";
            ultraListViewSubItemColumn1.Text = "Ending IP Address";
            ultraListViewSubItemColumn1.Width = 150;
            this.ulvTcpRanges.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1});
            this.ulvTcpRanges.TabIndex = 0;
            this.ulvTcpRanges.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.ulvTcpRanges.ViewSettingsDetails.ColumnAutoSizeMode = Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems;
            this.ulvTcpRanges.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.ulvTcpRanges_ItemSelectionChanged);
            // 
            // FormUserLocation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(551, 494);
            this.Controls.Add(this.ultraGroupBox1);
            this.Controls.Add(this.tbChild);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbParent);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnCancel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormUserLocation";
            this.Text = "Location Properties";
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.tbParent, 0);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.tbChild, 0);
            this.Controls.SetChildIndex(this.ultraGroupBox1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ulvTcpRanges)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbParent;
		private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
		private System.Windows.Forms.TextBox tbChild;
		private System.Windows.Forms.Label label2;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
		private Infragistics.Win.Misc.UltraButton bnRemove;
		private Infragistics.Win.Misc.UltraButton bnAdd;
		private Infragistics.Win.UltraWinListView.UltraListView ulvTcpRanges;
		private Infragistics.Win.Misc.UltraButton bnEdit;
	}
}