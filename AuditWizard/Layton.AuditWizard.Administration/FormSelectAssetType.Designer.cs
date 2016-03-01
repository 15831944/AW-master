namespace Layton.AuditWizard.Administration
{
	partial class FormSelectAssetType
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
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            this.label1 = new System.Windows.Forms.Label();
            this.tvAssetTypes = new Infragistics.Win.UltraWinTree.UltraTree();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.tbSelectedType = new System.Windows.Forms.TextBox();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvAssetTypes)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.select_assettype_properties_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(31, 477);
            this.footerPictureBox.Size = new System.Drawing.Size(390, 120);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(14, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(388, 39);
            this.label1.TabIndex = 10;
            this.label1.Text = "Select an asset type to which this user defined data category will be restricted." +
                "  Only assets with the selected type will display this category of user data.";
            // 
            // tvAssetTypes
            // 
            this.tvAssetTypes.Location = new System.Drawing.Point(17, 74);
            this.tvAssetTypes.Name = "tvAssetTypes";
            _override1.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.Standard;
            _override1.SelectionType = Infragistics.Win.UltraWinTree.SelectType.Single;
            this.tvAssetTypes.Override = _override1;
            this.tvAssetTypes.Size = new System.Drawing.Size(385, 299);
            this.tvAssetTypes.TabIndex = 11;
            this.tvAssetTypes.AfterSelect += new Infragistics.Win.UltraWinTree.AfterNodeSelectEventHandler(this.tvAssetTypes_AfterSelect);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Location = new System.Drawing.Point(17, 395);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(131, 23);
            this.ultraLabel1.TabIndex = 14;
            this.ultraLabel1.Text = "Selected Asset Type:";
            // 
            // tbSelectedType
            // 
            this.tbSelectedType.Location = new System.Drawing.Point(154, 392);
            this.tbSelectedType.Name = "tbSelectedType";
            this.tbSelectedType.ReadOnly = true;
            this.tbSelectedType.Size = new System.Drawing.Size(248, 21);
            this.tbSelectedType.TabIndex = 15;
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(315, 448);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 19;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Enabled = false;
            this.bnOK.Location = new System.Drawing.Point(221, 448);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 18;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // FormSelectAssetType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(421, 597);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.tbSelectedType);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tvAssetTypes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormSelectAssetType";
            this.Text = "Select Asset Type";
            this.Load += new System.EventHandler(this.FormSelectAssetType_Load);
            this.Controls.SetChildIndex(this.tvAssetTypes, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.Controls.SetChildIndex(this.tbSelectedType, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvAssetTypes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
        private Infragistics.Win.UltraWinTree.UltraTree tvAssetTypes;
		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private System.Windows.Forms.TextBox tbSelectedType;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
	}
}
