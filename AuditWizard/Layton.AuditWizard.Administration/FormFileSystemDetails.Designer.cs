namespace Layton.AuditWizard.Administration
{
	partial class FormFileSystemDetails
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
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.ultraGridPrintDocument1 = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panelFolders = new System.Windows.Forms.Panel();
            this.bnEditFolder = new Infragistics.Win.Misc.UltraButton();
            this.bnDeleteFolder = new Infragistics.Win.Misc.UltraButton();
            this.bnAddFolder = new Infragistics.Win.Misc.UltraButton();
            this.lbFolders = new System.Windows.Forms.ListBox();
            this.panelFiles = new System.Windows.Forms.Panel();
            this.bnEditFile = new Infragistics.Win.Misc.UltraButton();
            this.bnDeleteFile = new Infragistics.Win.Misc.UltraButton();
            this.bnAddFile = new Infragistics.Win.Misc.UltraButton();
            this.lbFiles = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbFoldersSpecified = new System.Windows.Forms.RadioButton();
            this.rbFoldersAll = new System.Windows.Forms.RadioButton();
            this.rbFoldersNone = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbFilesAll = new System.Windows.Forms.RadioButton();
            this.rbFilesExecutables = new System.Windows.Forms.RadioButton();
            this.rbFilesSpecified = new System.Windows.Forms.RadioButton();
            this.rbFilesNone = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            this.panelFolders.SuspendLayout();
            this.panelFiles.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.filesystem_settings_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(219, 385);
            this.footerPictureBox.Size = new System.Drawing.Size(447, 120);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(14, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(385, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Specify which Folders and Files are to be scanned by AuditWizard.";
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(553, 347);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 19;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(458, 347);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 18;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(192, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Folders";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(192, 204);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Files";
            // 
            // panelFolders
            // 
            this.panelFolders.Controls.Add(this.bnEditFolder);
            this.panelFolders.Controls.Add(this.bnDeleteFolder);
            this.panelFolders.Controls.Add(this.bnAddFolder);
            this.panelFolders.Controls.Add(this.lbFolders);
            this.panelFolders.Location = new System.Drawing.Point(196, 70);
            this.panelFolders.Name = "panelFolders";
            this.panelFolders.Size = new System.Drawing.Size(444, 120);
            this.panelFolders.TabIndex = 20;
            // 
            // bnEditFolder
            // 
            appearance4.Image = global::Layton.AuditWizard.Administration.Properties.Resources.Edit;
            this.bnEditFolder.Appearance = appearance4;
            this.bnEditFolder.Location = new System.Drawing.Point(348, 75);
            this.bnEditFolder.Name = "bnEditFolder";
            this.bnEditFolder.Size = new System.Drawing.Size(87, 30);
            this.bnEditFolder.TabIndex = 12;
            this.bnEditFolder.Text = "&Edit";
            this.bnEditFolder.Click += new System.EventHandler(this.bnEditFolder_Click);
            // 
            // bnDeleteFolder
            // 
            appearance5.Image = global::Layton.AuditWizard.Administration.Properties.Resources.Delete;
            this.bnDeleteFolder.Appearance = appearance5;
            this.bnDeleteFolder.Location = new System.Drawing.Point(348, 39);
            this.bnDeleteFolder.Name = "bnDeleteFolder";
            this.bnDeleteFolder.Size = new System.Drawing.Size(87, 30);
            this.bnDeleteFolder.TabIndex = 11;
            this.bnDeleteFolder.Text = "&Delete";
            this.bnDeleteFolder.Click += new System.EventHandler(this.bnDeleteFolder_Click);
            // 
            // bnAddFolder
            // 
            appearance6.Image = global::Layton.AuditWizard.Administration.Properties.Resources.add16;
            this.bnAddFolder.Appearance = appearance6;
            this.bnAddFolder.Location = new System.Drawing.Point(348, 3);
            this.bnAddFolder.Name = "bnAddFolder";
            this.bnAddFolder.Size = new System.Drawing.Size(87, 30);
            this.bnAddFolder.TabIndex = 10;
            this.bnAddFolder.Text = "&Add";
            this.bnAddFolder.Click += new System.EventHandler(this.bnAddFolder_Click);
            // 
            // lbFolders
            // 
            this.lbFolders.FormattingEnabled = true;
            this.lbFolders.Location = new System.Drawing.Point(3, 3);
            this.lbFolders.Name = "lbFolders";
            this.lbFolders.Size = new System.Drawing.Size(336, 108);
            this.lbFolders.TabIndex = 9;
            this.lbFolders.DoubleClick += new System.EventHandler(this.lbFolders_DoubleClick);
            this.lbFolders.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lbFolders_KeyPress);
            this.lbFolders.SelectedValueChanged += new System.EventHandler(this.lbFolders_SelectedIndexChanged);
            this.lbFolders.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbFolders_KeyDown);
            // 
            // panelFiles
            // 
            this.panelFiles.Controls.Add(this.bnEditFile);
            this.panelFiles.Controls.Add(this.bnDeleteFile);
            this.panelFiles.Controls.Add(this.bnAddFile);
            this.panelFiles.Controls.Add(this.lbFiles);
            this.panelFiles.Location = new System.Drawing.Point(196, 220);
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
            // panel1
            // 
            this.panel1.Controls.Add(this.rbFoldersSpecified);
            this.panel1.Controls.Add(this.rbFoldersAll);
            this.panel1.Controls.Add(this.rbFoldersNone);
            this.panel1.Location = new System.Drawing.Point(34, 70);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(155, 100);
            this.panel1.TabIndex = 22;
            // 
            // rbFoldersSpecified
            // 
            this.rbFoldersSpecified.AutoSize = true;
            this.rbFoldersSpecified.BackColor = System.Drawing.Color.Transparent;
            this.rbFoldersSpecified.Checked = true;
            this.rbFoldersSpecified.Location = new System.Drawing.Point(3, 26);
            this.rbFoldersSpecified.Name = "rbFoldersSpecified";
            this.rbFoldersSpecified.Size = new System.Drawing.Size(132, 17);
            this.rbFoldersSpecified.TabIndex = 5;
            this.rbFoldersSpecified.TabStop = true;
            this.rbFoldersSpecified.Tag = "1";
            this.rbFoldersSpecified.Text = "Specified Folder(s)";
            this.rbFoldersSpecified.UseVisualStyleBackColor = false;
            this.rbFoldersSpecified.CheckedChanged += new System.EventHandler(this.rbFolders_CheckedChanged);
            // 
            // rbFoldersAll
            // 
            this.rbFoldersAll.AutoSize = true;
            this.rbFoldersAll.BackColor = System.Drawing.Color.Transparent;
            this.rbFoldersAll.Location = new System.Drawing.Point(3, 49);
            this.rbFoldersAll.Name = "rbFoldersAll";
            this.rbFoldersAll.Size = new System.Drawing.Size(84, 17);
            this.rbFoldersAll.TabIndex = 6;
            this.rbFoldersAll.Tag = "2";
            this.rbFoldersAll.Text = "All Folders";
            this.rbFoldersAll.UseVisualStyleBackColor = false;
            this.rbFoldersAll.CheckedChanged += new System.EventHandler(this.rbFolders_CheckedChanged);
            // 
            // rbFoldersNone
            // 
            this.rbFoldersNone.AutoSize = true;
            this.rbFoldersNone.BackColor = System.Drawing.Color.Transparent;
            this.rbFoldersNone.Location = new System.Drawing.Point(3, 3);
            this.rbFoldersNone.Name = "rbFoldersNone";
            this.rbFoldersNone.Size = new System.Drawing.Size(85, 17);
            this.rbFoldersNone.TabIndex = 4;
            this.rbFoldersNone.Tag = "0";
            this.rbFoldersNone.Text = "No Folders";
            this.rbFoldersNone.UseVisualStyleBackColor = false;
            this.rbFoldersNone.CheckedChanged += new System.EventHandler(this.rbFolders_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbFilesAll);
            this.panel2.Controls.Add(this.rbFilesExecutables);
            this.panel2.Controls.Add(this.rbFilesSpecified);
            this.panel2.Controls.Add(this.rbFilesNone);
            this.panel2.Location = new System.Drawing.Point(34, 220);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(155, 100);
            this.panel2.TabIndex = 23;
            // 
            // rbFilesAll
            // 
            this.rbFilesAll.AutoSize = true;
            this.rbFilesAll.BackColor = System.Drawing.Color.Transparent;
            this.rbFilesAll.Location = new System.Drawing.Point(3, 76);
            this.rbFilesAll.Name = "rbFilesAll";
            this.rbFilesAll.Size = new System.Drawing.Size(68, 17);
            this.rbFilesAll.TabIndex = 16;
            this.rbFilesAll.Tag = "3";
            this.rbFilesAll.Text = "All Files";
            this.rbFilesAll.UseVisualStyleBackColor = false;
            this.rbFilesAll.CheckedChanged += new System.EventHandler(this.rbFiles_CheckedChanged);
            // 
            // rbFilesExecutables
            // 
            this.rbFilesExecutables.AutoSize = true;
            this.rbFilesExecutables.BackColor = System.Drawing.Color.Transparent;
            this.rbFilesExecutables.Location = new System.Drawing.Point(3, 30);
            this.rbFilesExecutables.Name = "rbFilesExecutables";
            this.rbFilesExecutables.Size = new System.Drawing.Size(134, 17);
            this.rbFilesExecutables.TabIndex = 14;
            this.rbFilesExecutables.Tag = "1";
            this.rbFilesExecutables.Text = "All Executable Files";
            this.rbFilesExecutables.UseVisualStyleBackColor = false;
            this.rbFilesExecutables.CheckedChanged += new System.EventHandler(this.rbFiles_CheckedChanged);
            // 
            // rbFilesSpecified
            // 
            this.rbFilesSpecified.AutoSize = true;
            this.rbFilesSpecified.BackColor = System.Drawing.Color.Transparent;
            this.rbFilesSpecified.Checked = true;
            this.rbFilesSpecified.Location = new System.Drawing.Point(3, 53);
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
            // FormFileSystemDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 504);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelFiles);
            this.Controls.Add(this.panelFolders);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormFileSystemDetails";
            this.Text = "File System Audit Details";
            this.Load += new System.EventHandler(this.FormFileSystemDetails_Load);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.panelFolders, 0);
            this.Controls.SetChildIndex(this.panelFiles, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            this.panelFolders.ResumeLayout(false);
            this.panelFiles.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panelFolders;
		private Infragistics.Win.Misc.UltraButton bnEditFolder;
		private Infragistics.Win.Misc.UltraButton bnDeleteFolder;
		private Infragistics.Win.Misc.UltraButton bnAddFolder;
		private System.Windows.Forms.ListBox lbFolders;
		private System.Windows.Forms.Panel panelFiles;
		private Infragistics.Win.Misc.UltraButton bnEditFile;
		private Infragistics.Win.Misc.UltraButton bnDeleteFile;
		private Infragistics.Win.Misc.UltraButton bnAddFile;
		private System.Windows.Forms.ListBox lbFiles;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton rbFoldersSpecified;
		private System.Windows.Forms.RadioButton rbFoldersAll;
		private System.Windows.Forms.RadioButton rbFoldersNone;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RadioButton rbFilesAll;
		private System.Windows.Forms.RadioButton rbFilesExecutables;
		private System.Windows.Forms.RadioButton rbFilesSpecified;
		private System.Windows.Forms.RadioButton rbFilesNone;
	}
}