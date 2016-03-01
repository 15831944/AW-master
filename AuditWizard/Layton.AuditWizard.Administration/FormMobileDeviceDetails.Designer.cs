namespace Layton.AuditWizard.Administration
{
	partial class FormMobileDeviceDetails
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.ultraGridPrintDocument1 = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.panelFiles = new System.Windows.Forms.Panel();
            this.bnEditFile = new Infragistics.Win.Misc.UltraButton();
            this.bnDeleteFile = new Infragistics.Win.Misc.UltraButton();
            this.bnAddFile = new Infragistics.Win.Misc.UltraButton();
            this.lbFiles = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbFilesAll = new System.Windows.Forms.RadioButton();
            this.rbFilesSpecified = new System.Windows.Forms.RadioButton();
            this.rbFilesNone = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            this.panelFiles.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.mobiledevice_settings_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(219, 246);
            this.footerPictureBox.Size = new System.Drawing.Size(447, 120);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(14, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(422, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Specify which Files on Mobile Devices are to be scanned by AuditWizard.";
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(537, 191);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 19;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(442, 191);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 18;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // panelFiles
            // 
            this.panelFiles.BackColor = System.Drawing.Color.Transparent;
            this.panelFiles.Controls.Add(this.bnEditFile);
            this.panelFiles.Controls.Add(this.bnDeleteFile);
            this.panelFiles.Controls.Add(this.bnAddFile);
            this.panelFiles.Controls.Add(this.lbFiles);
            this.panelFiles.Location = new System.Drawing.Point(180, 64);
            this.panelFiles.Name = "panelFiles";
            this.panelFiles.Size = new System.Drawing.Size(444, 117);
            this.panelFiles.TabIndex = 21;
            // 
            // bnEditFile
            // 
            appearance3.Image = global::Layton.AuditWizard.Administration.Properties.Resources.Edit;
            this.bnEditFile.Appearance = appearance3;
            this.bnEditFile.Location = new System.Drawing.Point(350, 76);
            this.bnEditFile.Name = "bnEditFile";
            this.bnEditFile.Size = new System.Drawing.Size(87, 30);
            this.bnEditFile.TabIndex = 21;
            this.bnEditFile.Text = "&Edit";
            this.bnEditFile.Click += new System.EventHandler(this.bnEditFile_Click);
            // 
            // bnDeleteFile
            // 
            appearance2.Image = global::Layton.AuditWizard.Administration.Properties.Resources.Delete;
            this.bnDeleteFile.Appearance = appearance2;
            this.bnDeleteFile.Location = new System.Drawing.Point(350, 40);
            this.bnDeleteFile.Name = "bnDeleteFile";
            this.bnDeleteFile.Size = new System.Drawing.Size(87, 30);
            this.bnDeleteFile.TabIndex = 20;
            this.bnDeleteFile.Text = "&Delete";
            this.bnDeleteFile.Click += new System.EventHandler(this.bnDeleteFile_Click);
            // 
            // bnAddFile
            // 
            appearance1.Image = global::Layton.AuditWizard.Administration.Properties.Resources.add16;
            this.bnAddFile.Appearance = appearance1;
            this.bnAddFile.Location = new System.Drawing.Point(350, 4);
            this.bnAddFile.Name = "bnAddFile";
            this.bnAddFile.Size = new System.Drawing.Size(87, 30);
            this.bnAddFile.TabIndex = 19;
            this.bnAddFile.Text = "&Add";
            this.bnAddFile.Click += new System.EventHandler(this.bnAddFile_Click);
            // 
            // lbFiles
            // 
            this.lbFiles.FormattingEnabled = true;
            this.lbFiles.Location = new System.Drawing.Point(6, 4);
            this.lbFiles.Name = "lbFiles";
            this.lbFiles.Size = new System.Drawing.Size(336, 108);
            this.lbFiles.TabIndex = 18;
            this.lbFiles.SelectedIndexChanged += new System.EventHandler(this.lbFiles_SelectedIndexChanged);
            this.lbFiles.DoubleClick += new System.EventHandler(this.lbFiles_DoubleClick);
            this.lbFiles.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lbFiles_KeyPress);
            this.lbFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbFiles_KeyDown);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.rbFilesAll);
            this.panel2.Controls.Add(this.rbFilesSpecified);
            this.panel2.Controls.Add(this.rbFilesNone);
            this.panel2.Location = new System.Drawing.Point(17, 64);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(155, 85);
            this.panel2.TabIndex = 23;
            // 
            // rbFilesAll
            // 
            this.rbFilesAll.AutoSize = true;
            this.rbFilesAll.BackColor = System.Drawing.Color.Transparent;
            this.rbFilesAll.Location = new System.Drawing.Point(3, 53);
            this.rbFilesAll.Name = "rbFilesAll";
            this.rbFilesAll.Size = new System.Drawing.Size(68, 17);
            this.rbFilesAll.TabIndex = 16;
            this.rbFilesAll.Tag = "3";
            this.rbFilesAll.Text = "All Files";
            this.rbFilesAll.UseVisualStyleBackColor = false;
            this.rbFilesAll.CheckedChanged += new System.EventHandler(this.rbFiles_CheckedChanged);
            // 
            // rbFilesSpecified
            // 
            this.rbFilesSpecified.AutoSize = true;
            this.rbFilesSpecified.BackColor = System.Drawing.Color.Transparent;
            this.rbFilesSpecified.Checked = true;
            this.rbFilesSpecified.Location = new System.Drawing.Point(3, 30);
            this.rbFilesSpecified.Name = "rbFilesSpecified";
            this.rbFilesSpecified.Size = new System.Drawing.Size(106, 17);
            this.rbFilesSpecified.TabIndex = 15;
            this.rbFilesSpecified.TabStop = true;
            this.rbFilesSpecified.Tag = "2";
            this.rbFilesSpecified.Text = "Specified Files";
            this.rbFilesSpecified.UseVisualStyleBackColor = false;
            this.rbFilesSpecified.CheckedChanged += new System.EventHandler(this.rbFiles_CheckedChanged);
            // 
            // rbFilesNone
            // 
            this.rbFilesNone.AutoSize = true;
            this.rbFilesNone.BackColor = System.Drawing.Color.Transparent;
            this.rbFilesNone.Location = new System.Drawing.Point(3, 7);
            this.rbFilesNone.Name = "rbFilesNone";
            this.rbFilesNone.Size = new System.Drawing.Size(69, 17);
            this.rbFilesNone.TabIndex = 13;
            this.rbFilesNone.Tag = "0";
            this.rbFilesNone.Text = "No Files";
            this.rbFilesNone.UseVisualStyleBackColor = false;
            this.rbFilesNone.CheckedChanged += new System.EventHandler(this.rbFiles_CheckedChanged);
            // 
            // FormMobileDeviceDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 365);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelFiles);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormMobileDeviceDetails";
            this.Text = "Mobile Device Audit Details";
            this.Load += new System.EventHandler(this.FormMobileDeviceDetails_Load);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.panelFiles, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            this.panelFiles.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Panel panelFiles;
		private Infragistics.Win.Misc.UltraButton bnEditFile;
		private Infragistics.Win.Misc.UltraButton bnDeleteFile;
		private Infragistics.Win.Misc.UltraButton bnAddFile;
		private System.Windows.Forms.ListBox lbFiles;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RadioButton rbFilesAll;
		private System.Windows.Forms.RadioButton rbFilesSpecified;
		private System.Windows.Forms.RadioButton rbFilesNone;
	}
}