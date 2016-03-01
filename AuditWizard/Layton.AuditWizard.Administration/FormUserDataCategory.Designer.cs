namespace Layton.AuditWizard.Administration
{
	partial class FormUserDataCategory
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
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.tbCategoryName = new System.Windows.Forms.TextBox();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tbAppliesTo = new System.Windows.Forms.TextBox();
            this.lblAppliesTo = new Infragistics.Win.Misc.UltraLabel();
            this.bnSelect = new System.Windows.Forms.Button();
            this.bnBrowse = new System.Windows.Forms.Button();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.tbIconFile = new System.Windows.Forms.TextBox();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            appearance3.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel1.Appearance = appearance3;
            this.ultraLabel1.Location = new System.Drawing.Point(18, 28);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(102, 18);
            this.ultraLabel1.TabIndex = 10;
            this.ultraLabel1.Text = "Category Name:";
            // 
            // tbCategoryName
            // 
            this.tbCategoryName.Location = new System.Drawing.Point(126, 25);
            this.tbCategoryName.Name = "tbCategoryName";
            this.tbCategoryName.Size = new System.Drawing.Size(225, 21);
            this.tbCategoryName.TabIndex = 12;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(303, 192);
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
            this.bnCancel.Location = new System.Drawing.Point(398, 192);
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
            // tbAppliesTo
            // 
            this.tbAppliesTo.BackColor = System.Drawing.SystemColors.Window;
            this.tbAppliesTo.Location = new System.Drawing.Point(126, 54);
            this.tbAppliesTo.Name = "tbAppliesTo";
            this.tbAppliesTo.ReadOnly = true;
            this.tbAppliesTo.Size = new System.Drawing.Size(225, 21);
            this.tbAppliesTo.TabIndex = 19;
            // 
            // lblAppliesTo
            // 
            appearance5.BackColor = System.Drawing.Color.Transparent;
            this.lblAppliesTo.Appearance = appearance5;
            this.lblAppliesTo.Location = new System.Drawing.Point(18, 57);
            this.lblAppliesTo.Name = "lblAppliesTo";
            this.lblAppliesTo.Size = new System.Drawing.Size(71, 18);
            this.lblAppliesTo.TabIndex = 18;
            this.lblAppliesTo.Text = "Applies To:";
            // 
            // bnSelect
            // 
            this.bnSelect.Location = new System.Drawing.Point(368, 49);
            this.bnSelect.Name = "bnSelect";
            this.bnSelect.Size = new System.Drawing.Size(36, 23);
            this.bnSelect.TabIndex = 20;
            this.bnSelect.Text = "...";
            this.bnSelect.UseVisualStyleBackColor = true;
            this.bnSelect.Click += new System.EventHandler(this.bnSelect_Click);
            // 
            // bnBrowse
            // 
            this.bnBrowse.Location = new System.Drawing.Point(368, 78);
            this.bnBrowse.Name = "bnBrowse";
            this.bnBrowse.Size = new System.Drawing.Size(36, 23);
            this.bnBrowse.TabIndex = 27;
            this.bnBrowse.Text = "...";
            this.bnBrowse.UseVisualStyleBackColor = true;
            this.bnBrowse.Click += new System.EventHandler(this.bnBrowse_Click);
            // 
            // pbIcon
            // 
            this.pbIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbIcon.Location = new System.Drawing.Point(412, 71);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(37, 32);
            this.pbIcon.TabIndex = 26;
            this.pbIcon.TabStop = false;
            // 
            // tbIconFile
            // 
            this.tbIconFile.BackColor = System.Drawing.SystemColors.Window;
            this.tbIconFile.Location = new System.Drawing.Point(126, 82);
            this.tbIconFile.Name = "tbIconFile";
            this.tbIconFile.ReadOnly = true;
            this.tbIconFile.Size = new System.Drawing.Size(225, 21);
            this.tbIconFile.TabIndex = 25;
            this.tbIconFile.TextChanged += new System.EventHandler(this.tbIconFile_TextChanged);
            // 
            // ultraLabel5
            // 
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel5.Appearance = appearance2;
            this.ultraLabel5.Location = new System.Drawing.Point(18, 86);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(80, 20);
            this.ultraLabel5.TabIndex = 24;
            this.ultraLabel5.Text = "Icon Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(244, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "Enter details for a user-defined category:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pbIcon);
            this.groupBox1.Controls.Add(this.ultraLabel1);
            this.groupBox1.Controls.Add(this.bnBrowse);
            this.groupBox1.Controls.Add(this.tbCategoryName);
            this.groupBox1.Controls.Add(this.lblAppliesTo);
            this.groupBox1.Controls.Add(this.tbIconFile);
            this.groupBox1.Controls.Add(this.tbAppliesTo);
            this.groupBox1.Controls.Add(this.ultraLabel5);
            this.groupBox1.Controls.Add(this.bnSelect);
            this.groupBox1.Location = new System.Drawing.Point(16, 49);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(469, 124);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            // 
            // FormUserDataCategory
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(508, 242);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "FormUserDataCategory";
            this.Text = "New User Data Category";
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.Misc.UltraLabel ultraLabel1;
		private System.Windows.Forms.TextBox tbCategoryName;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.TextBox tbAppliesTo;
		private Infragistics.Win.Misc.UltraLabel lblAppliesTo;
        private System.Windows.Forms.Button bnSelect;
		private System.Windows.Forms.Button bnBrowse;
		private System.Windows.Forms.PictureBox pbIcon;
		private System.Windows.Forms.TextBox tbIconFile;
		private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
	}
}