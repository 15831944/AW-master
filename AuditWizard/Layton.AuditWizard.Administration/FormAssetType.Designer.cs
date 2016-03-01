namespace Layton.AuditWizard.Administration
{
	partial class FormAssetType
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbIconFile = new System.Windows.Forms.TextBox();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.bnBrowse = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tbCategory = new System.Windows.Forms.TextBox();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.assettype_properties_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(57, 215);
            this.footerPictureBox.Size = new System.Drawing.Size(447, 120);
            // 
            // ultraLabel1
            // 
            appearance3.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel1.Appearance = appearance3;
            this.ultraLabel1.Location = new System.Drawing.Point(14, 63);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(117, 23);
            this.ultraLabel1.TabIndex = 10;
            this.ultraLabel1.Text = "Name:";
            // 
            // ultraLabel2
            // 
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel2.Appearance = appearance2;
            this.ultraLabel2.Location = new System.Drawing.Point(14, 92);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(117, 23);
            this.ultraLabel2.TabIndex = 11;
            this.ultraLabel2.Text = "Icon Name:";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(138, 60);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(353, 21);
            this.tbName.TabIndex = 12;
            // 
            // tbIconFile
            // 
            this.tbIconFile.BackColor = System.Drawing.SystemColors.Window;
            this.tbIconFile.Location = new System.Drawing.Point(138, 89);
            this.tbIconFile.Name = "tbIconFile";
            this.tbIconFile.ReadOnly = true;
            this.tbIconFile.Size = new System.Drawing.Size(175, 21);
            this.tbIconFile.TabIndex = 13;
            this.tbIconFile.TextChanged += new System.EventHandler(this.tbIconFile_TextChanged);
            // 
            // pbIcon
            // 
            this.pbIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbIcon.Location = new System.Drawing.Point(364, 86);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(37, 32);
            this.pbIcon.TabIndex = 14;
            this.pbIcon.TabStop = false;
            // 
            // bnBrowse
            // 
            this.bnBrowse.Location = new System.Drawing.Point(321, 89);
            this.bnBrowse.Name = "bnBrowse";
            this.bnBrowse.Size = new System.Drawing.Size(36, 23);
            this.bnBrowse.TabIndex = 15;
            this.bnBrowse.Text = "...";
            this.bnBrowse.UseVisualStyleBackColor = true;
            this.bnBrowse.Click += new System.EventHandler(this.bnBrowse_Click);
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(309, 176);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 16;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(404, 176);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 17;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "*.png";
            this.openFileDialog.Filter = "Image Files (*.bmp,*.gif,*.jpg, *.png)|*.bmp;*.gif;*.jpg;*.png|All files|*.*||";
            this.openFileDialog.FilterIndex = 3;
            // 
            // tbCategory
            // 
            this.tbCategory.BackColor = System.Drawing.SystemColors.Window;
            this.tbCategory.Location = new System.Drawing.Point(138, 34);
            this.tbCategory.Name = "tbCategory";
            this.tbCategory.ReadOnly = true;
            this.tbCategory.Size = new System.Drawing.Size(353, 21);
            this.tbCategory.TabIndex = 19;
            // 
            // ultraLabel3
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel3.Appearance = appearance1;
            this.ultraLabel3.Location = new System.Drawing.Point(14, 37);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(117, 23);
            this.ultraLabel3.TabIndex = 18;
            this.ultraLabel3.Text = "Category:";
            // 
            // FormAssetType
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(504, 336);
            this.Controls.Add(this.tbCategory);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnBrowse);
            this.Controls.Add(this.pbIcon);
            this.Controls.Add(this.tbIconFile);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormAssetType";
            this.Text = "New Asset Type";
            this.Controls.SetChildIndex(this.ultraLabel1, 0);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.ultraLabel2, 0);
            this.Controls.SetChildIndex(this.tbName, 0);
            this.Controls.SetChildIndex(this.tbIconFile, 0);
            this.Controls.SetChildIndex(this.pbIcon, 0);
            this.Controls.SetChildIndex(this.bnBrowse, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.ultraLabel3, 0);
            this.Controls.SetChildIndex(this.tbCategory, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.TextBox tbIconFile;
		private System.Windows.Forms.PictureBox pbIcon;
		private System.Windows.Forms.Button bnBrowse;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.TextBox tbCategory;
		private Infragistics.Win.Misc.UltraLabel ultraLabel3;
	}
}