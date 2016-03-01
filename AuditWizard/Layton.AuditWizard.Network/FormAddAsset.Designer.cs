namespace Layton.AuditWizard.Network
{
	partial class FormAddAsset
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbAssetName = new System.Windows.Forms.TextBox();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.labelLocation = new System.Windows.Forms.Label();
            this.cbAssetType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cbAssetCategory = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label3 = new System.Windows.Forms.Label();
            this.tbParentLocation = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.cbAssetType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAssetCategory)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(15, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Asset Name:";
            // 
            // tbAssetName
            // 
            this.tbAssetName.BackColor = System.Drawing.SystemColors.Window;
            this.tbAssetName.Location = new System.Drawing.Point(140, 46);
            this.tbAssetName.Name = "tbAssetName";
            this.tbAssetName.Size = new System.Drawing.Size(368, 21);
            this.tbAssetName.TabIndex = 3;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(317, 142);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(92, 24);
            this.bnOK.TabIndex = 8;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(416, 142);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(92, 24);
            this.bnCancel.TabIndex = 9;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(15, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Asset Type:";
            // 
            // labelLocation
            // 
            this.labelLocation.AutoSize = true;
            this.labelLocation.BackColor = System.Drawing.Color.Transparent;
            this.labelLocation.Location = new System.Drawing.Point(15, 23);
            this.labelLocation.Name = "labelLocation";
            this.labelLocation.Size = new System.Drawing.Size(59, 13);
            this.labelLocation.TabIndex = 0;
            this.labelLocation.Text = "Location:";
            // 
            // cbAssetType
            // 
            this.cbAssetType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cbAssetType.Location = new System.Drawing.Point(140, 99);
            this.cbAssetType.Name = "cbAssetType";
            this.cbAssetType.Size = new System.Drawing.Size(369, 22);
            this.cbAssetType.TabIndex = 7;
            // 
            // cbAssetCategory
            // 
            this.cbAssetCategory.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cbAssetCategory.Location = new System.Drawing.Point(140, 72);
            this.cbAssetCategory.Name = "cbAssetCategory";
            this.cbAssetCategory.Size = new System.Drawing.Size(369, 22);
            this.cbAssetCategory.TabIndex = 5;
            this.cbAssetCategory.ValueChanged += new System.EventHandler(this.cbAssetCategory_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(14, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Asset Category:";
            // 
            // tbParentLocation
            // 
            this.tbParentLocation.BackColor = System.Drawing.SystemColors.Window;
            this.tbParentLocation.Enabled = false;
            this.tbParentLocation.Location = new System.Drawing.Point(140, 20);
            this.tbParentLocation.Name = "tbParentLocation";
            this.tbParentLocation.ReadOnly = true;
            this.tbParentLocation.Size = new System.Drawing.Size(368, 21);
            this.tbParentLocation.TabIndex = 1;
            // 
            // FormAddAsset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(530, 178);
            this.Controls.Add(this.cbAssetCategory);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbAssetType);
            this.Controls.Add(this.labelLocation);
            this.Controls.Add(this.tbParentLocation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbAssetName);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnCancel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "FormAddAsset";
            this.Text = "Add Asset";
            this.Load += new System.EventHandler(this.FormAddAsset_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cbAssetType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbAssetCategory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbAssetName;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelLocation;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cbAssetType;
		private Infragistics.Win.UltraWinEditors.UltraComboEditor cbAssetCategory;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbParentLocation;
	}
}
