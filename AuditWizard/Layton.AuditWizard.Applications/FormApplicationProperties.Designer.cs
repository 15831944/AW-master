namespace Layton.AuditWizard.Applications
{
	partial class FormApplicationProperties
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApplicationProperties));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click here to change the publisher of this application.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Current application publisher - click \'...\' to select a different publisher.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo3 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Click here to change the publisher of this application.", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.tbVersion = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblAliasDescription = new System.Windows.Forms.Label();
            this.bnBrowseAlias = new System.Windows.Forms.Button();
            this.lblAliasedTo = new System.Windows.Forms.Label();
            this.tbAliasedApplication = new System.Windows.Forms.TextBox();
            this.bnSetPublisher = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPublisher = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.notesControl = new Layton.AuditWizard.Common.NotesControl();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.documentsControl = new Layton.AuditWizard.Common.DocumentsControl();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.bnCancel = new System.Windows.Forms.Button();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.dataTabControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.bnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            this.ultraTabPageControl2.SuspendLayout();
            this.ultraTabPageControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataTabControl)).BeginInit();
            this.dataTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Applications.Properties.Resources.application_global_properties_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(326, 427);
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.ultraGroupBox1);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 23);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(753, 334);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.ultraGroupBox1.Appearance = appearance1;
            this.ultraGroupBox1.Controls.Add(this.tbVersion);
            this.ultraGroupBox1.Controls.Add(this.label3);
            this.ultraGroupBox1.Controls.Add(this.lblAliasDescription);
            this.ultraGroupBox1.Controls.Add(this.bnBrowseAlias);
            this.ultraGroupBox1.Controls.Add(this.lblAliasedTo);
            this.ultraGroupBox1.Controls.Add(this.tbAliasedApplication);
            this.ultraGroupBox1.Controls.Add(this.bnSetPublisher);
            this.ultraGroupBox1.Controls.Add(this.label1);
            this.ultraGroupBox1.Controls.Add(this.tbPublisher);
            this.ultraGroupBox1.Controls.Add(this.tbName);
            this.ultraGroupBox1.Controls.Add(this.label2);
            this.ultraGroupBox1.Location = new System.Drawing.Point(12, 14);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(724, 292);
            this.ultraGroupBox1.TabIndex = 21;
            this.ultraGroupBox1.Text = "Basic Information";
            // 
            // tbVersion
            // 
            this.tbVersion.BackColor = System.Drawing.SystemColors.Window;
            this.tbVersion.Location = new System.Drawing.Point(118, 86);
            this.tbVersion.Name = "tbVersion";
            this.tbVersion.ReadOnly = true;
            this.tbVersion.Size = new System.Drawing.Size(259, 21);
            this.tbVersion.TabIndex = 33;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(22, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Version:";
            // 
            // lblAliasDescription
            // 
            this.lblAliasDescription.BackColor = System.Drawing.Color.Transparent;
            this.lblAliasDescription.Location = new System.Drawing.Point(7, 143);
            this.lblAliasDescription.Name = "lblAliasDescription";
            this.lblAliasDescription.Size = new System.Drawing.Size(711, 61);
            this.lblAliasDescription.TabIndex = 31;
            this.lblAliasDescription.Text = resources.GetString("lblAliasDescription.Text");
            // 
            // bnBrowseAlias
            // 
            this.bnBrowseAlias.Location = new System.Drawing.Point(385, 207);
            this.bnBrowseAlias.Name = "bnBrowseAlias";
            this.bnBrowseAlias.Size = new System.Drawing.Size(38, 24);
            this.bnBrowseAlias.TabIndex = 30;
            this.bnBrowseAlias.Text = "...";
            ultraToolTipInfo1.ToolTipText = "Click here to change the publisher of this application.";
            this.ultraToolTipManager1.SetUltraToolTip(this.bnBrowseAlias, ultraToolTipInfo1);
            this.bnBrowseAlias.UseVisualStyleBackColor = true;
            this.bnBrowseAlias.Click += new System.EventHandler(this.bnBrowseAlias_Click);
            // 
            // lblAliasedTo
            // 
            this.lblAliasedTo.AutoSize = true;
            this.lblAliasedTo.BackColor = System.Drawing.Color.Transparent;
            this.lblAliasedTo.Location = new System.Drawing.Point(22, 213);
            this.lblAliasedTo.Name = "lblAliasedTo";
            this.lblAliasedTo.Size = new System.Drawing.Size(71, 13);
            this.lblAliasedTo.TabIndex = 28;
            this.lblAliasedTo.Text = "Aliased To:";
            // 
            // tbAliasedApplication
            // 
            this.tbAliasedApplication.BackColor = System.Drawing.SystemColors.Window;
            this.tbAliasedApplication.Location = new System.Drawing.Point(118, 210);
            this.tbAliasedApplication.Name = "tbAliasedApplication";
            this.tbAliasedApplication.ReadOnly = true;
            this.tbAliasedApplication.Size = new System.Drawing.Size(259, 21);
            this.tbAliasedApplication.TabIndex = 29;
            ultraToolTipInfo2.ToolTipText = "Current application publisher - click \'...\' to select a different publisher.";
            this.ultraToolTipManager1.SetUltraToolTip(this.tbAliasedApplication, ultraToolTipInfo2);
            // 
            // bnSetPublisher
            // 
            this.bnSetPublisher.Location = new System.Drawing.Point(385, 29);
            this.bnSetPublisher.Name = "bnSetPublisher";
            this.bnSetPublisher.Size = new System.Drawing.Size(38, 24);
            this.bnSetPublisher.TabIndex = 27;
            this.bnSetPublisher.Text = "...";
            ultraToolTipInfo3.ToolTipText = "Click here to change the publisher of this application.";
            this.ultraToolTipManager1.SetUltraToolTip(this.bnSetPublisher, ultraToolTipInfo3);
            this.bnSetPublisher.UseVisualStyleBackColor = true;
            this.bnSetPublisher.Click += new System.EventHandler(this.bnSetPublisher_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(22, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Publisher:";
            // 
            // tbPublisher
            // 
            this.tbPublisher.BackColor = System.Drawing.SystemColors.Window;
            this.tbPublisher.Location = new System.Drawing.Point(118, 32);
            this.tbPublisher.Name = "tbPublisher";
            this.tbPublisher.ReadOnly = true;
            this.tbPublisher.Size = new System.Drawing.Size(259, 21);
            this.tbPublisher.TabIndex = 24;
            // 
            // tbName
            // 
            this.tbName.BackColor = System.Drawing.SystemColors.Window;
            this.tbName.Location = new System.Drawing.Point(118, 59);
            this.tbName.Name = "tbName";
            this.tbName.ReadOnly = true;
            this.tbName.Size = new System.Drawing.Size(259, 21);
            this.tbName.TabIndex = 26;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(22, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Application:";
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.notesControl);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(753, 334);
            // 
            // notesControl
            // 
            this.notesControl.BackColor = System.Drawing.Color.Transparent;
            this.notesControl.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.notesControl.Location = new System.Drawing.Point(3, 5);
            this.notesControl.Name = "notesControl";
            this.notesControl.Size = new System.Drawing.Size(747, 327);
            this.notesControl.TabIndex = 0;
            // 
            // ultraTabPageControl3
            // 
            this.ultraTabPageControl3.Controls.Add(this.documentsControl);
            this.ultraTabPageControl3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(753, 334);
            // 
            // documentsControl
            // 
            this.documentsControl.BackColor = System.Drawing.Color.Transparent;
            this.documentsControl.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.documentsControl.Location = new System.Drawing.Point(12, 14);
            this.documentsControl.Name = "documentsControl";
            this.documentsControl.Size = new System.Drawing.Size(726, 275);
            this.documentsControl.TabIndex = 0;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "All files|*.*";
            this.openFileDialog1.Title = "Select Document";
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(679, 397);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(92, 24);
            this.bnCancel.TabIndex = 22;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // dataTabControl
            // 
            this.dataTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.dataTabControl.Appearance = appearance2;
            this.dataTabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.dataTabControl.Controls.Add(this.ultraTabPageControl1);
            this.dataTabControl.Controls.Add(this.ultraTabPageControl2);
            this.dataTabControl.Controls.Add(this.ultraTabPageControl3);
            this.dataTabControl.Location = new System.Drawing.Point(12, 20);
            this.dataTabControl.Name = "dataTabControl";
            this.dataTabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.dataTabControl.Size = new System.Drawing.Size(757, 360);
            this.dataTabControl.TabIndex = 25;
            appearance3.Image = global::Layton.AuditWizard.Applications.Properties.Resources.application_16;
            ultraTab1.Appearance = appearance3;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "Basic Information";
            ultraTab1.ToolTipText = "Display basic information about this instance of the application";
            appearance4.Image = global::Layton.AuditWizard.Applications.Properties.Resources.notes_16;
            ultraTab2.Appearance = appearance4;
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "Notes";
            ultraTab2.ToolTipText = "Allow a date stamped list of notes to be maintained for this instance of the appl" +
                "ication.";
            appearance5.Image = global::Layton.AuditWizard.Applications.Properties.Resources.Documents_16;
            ultraTab3.Appearance = appearance5;
            ultraTab3.TabPage = this.ultraTabPageControl3;
            ultraTab3.Text = "Documents";
            ultraTab3.ToolTipText = "Allows one or more documents to be associated with this instance of the applicati" +
                "on";
            this.dataTabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(753, 334);
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(587, 398);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(86, 23);
            this.bnOK.TabIndex = 26;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // FormApplicationProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(785, 548);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.dataTabControl);
            this.Controls.Add(this.bnCancel);
            this.Name = "FormApplicationProperties";
            this.Text = "Application Properties";
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.dataTabControl, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            this.ultraTabPageControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            this.ultraTabPageControl2.ResumeLayout(false);
            this.ultraTabPageControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataTabControl)).EndInit();
            this.dataTabControl.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Button bnCancel;
		private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
		private Infragistics.Win.UltraWinTabControl.UltraTabControl dataTabControl;
		private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
		private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
		private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
		private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
		private System.Windows.Forms.Button bnSetPublisher;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbPublisher;
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblAliasDescription;
		private System.Windows.Forms.Button bnBrowseAlias;
		private System.Windows.Forms.Label lblAliasedTo;
        private System.Windows.Forms.TextBox tbAliasedApplication;
        private Layton.AuditWizard.Common.DocumentsControl documentsControl;
        private System.Windows.Forms.TextBox tbVersion;
        private System.Windows.Forms.Label label3;
        private Layton.AuditWizard.Common.NotesControl notesControl;
        private System.Windows.Forms.Button bnOK;
	}
}