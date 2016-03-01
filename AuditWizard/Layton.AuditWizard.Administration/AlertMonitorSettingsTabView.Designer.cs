namespace Layton.AuditWizard.Administration
{
	partial class AlertMonitorSettingsTabView
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
			Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
			Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
			Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 0");
			Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn2 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 1");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlertMonitorSettingsTabView));
			this.headerGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
			this.headerLabel = new Infragistics.Win.Misc.UltraLabel();
			this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
			this.ultraGroupBox3 = new Infragistics.Win.Misc.UltraGroupBox();
			this.btnEditAlert = new System.Windows.Forms.Button();
			this.bnDelete = new System.Windows.Forms.Button();
			this.bnNew = new System.Windows.Forms.Button();
			this.lvAlertDefinitions = new Infragistics.Win.UltraWinListView.UltraListView();
			this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
			this.ultraFormattedLinkLabel1 = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
			this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
			this.gbFrequency = new System.Windows.Forms.GroupBox();
			this.panelEmailAt = new Infragistics.Win.Misc.UltraPanel();
			this.udteEmailAtTime = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
			this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
			this.dailyRadioButton = new System.Windows.Forms.RadioButton();
			this.hourlyRadioButton = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).BeginInit();
			this.headerGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox3)).BeginInit();
			this.ultraGroupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lvAlertDefinitions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
			this.ultraGroupBox2.SuspendLayout();
			this.gbFrequency.SuspendLayout();
			this.panelEmailAt.ClientArea.SuspendLayout();
			this.panelEmailAt.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.udteEmailAtTime)).BeginInit();
			this.SuspendLayout();
			// 
			// headerGroupBox
			// 
			this.headerGroupBox.Controls.Add(this.headerLabel);
			this.headerGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.headerGroupBox.Location = new System.Drawing.Point(0, 0);
			this.headerGroupBox.Name = "headerGroupBox";
			this.headerGroupBox.Size = new System.Drawing.Size(818, 80);
			this.headerGroupBox.TabIndex = 7;
			this.headerGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
			// 
			// headerLabel
			// 
			appearance1.BackColor = System.Drawing.Color.Transparent;
			appearance1.Image = global::Layton.AuditWizard.Administration.Properties.Resources.Alert_72;
			appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
			appearance1.TextHAlignAsString = "Center";
			appearance1.TextVAlignAsString = "Middle";
			this.headerLabel.Appearance = appearance1;
			this.headerLabel.AutoSize = true;
			this.headerLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.headerLabel.ImageSize = new System.Drawing.Size(72, 72);
			this.headerLabel.Location = new System.Drawing.Point(7, 5);
			this.headerLabel.Name = "headerLabel";
			this.headerLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.headerLabel.Size = new System.Drawing.Size(282, 72);
			this.headerLabel.TabIndex = 5;
			this.headerLabel.Text = "Alert Monitor Settings";
			// 
			// ultraGroupBox1
			// 
			this.ultraGroupBox1.Location = new System.Drawing.Point(0, 0);
			this.ultraGroupBox1.Name = "ultraGroupBox1";
			this.ultraGroupBox1.Size = new System.Drawing.Size(200, 110);
			this.ultraGroupBox1.TabIndex = 0;
			// 
			// ultraGroupBox3
			// 
			this.ultraGroupBox3.Controls.Add(this.btnEditAlert);
			this.ultraGroupBox3.Controls.Add(this.bnDelete);
			this.ultraGroupBox3.Controls.Add(this.bnNew);
			this.ultraGroupBox3.Controls.Add(this.lvAlertDefinitions);
			this.ultraGroupBox3.Controls.Add(this.ultraLabel1);
			this.ultraGroupBox3.Location = new System.Drawing.Point(7, 198);
			this.ultraGroupBox3.Name = "ultraGroupBox3";
			this.ultraGroupBox3.Size = new System.Drawing.Size(807, 257);
			this.ultraGroupBox3.TabIndex = 9;
			this.ultraGroupBox3.Text = "Alert Definitions";
			// 
			// btnEditAlert
			// 
			this.btnEditAlert.Enabled = false;
			this.btnEditAlert.Location = new System.Drawing.Point(130, 212);
			this.btnEditAlert.Name = "btnEditAlert";
			this.btnEditAlert.Size = new System.Drawing.Size(87, 23);
			this.btnEditAlert.TabIndex = 10;
			this.btnEditAlert.Text = "Edit";
			this.btnEditAlert.UseVisualStyleBackColor = true;
			this.btnEditAlert.Click += new System.EventHandler(this.btnEditAlert_Click);
			// 
			// bnDelete
			// 
			this.bnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bnDelete.Enabled = false;
			this.bnDelete.Location = new System.Drawing.Point(238, 212);
			this.bnDelete.Name = "bnDelete";
			this.bnDelete.Size = new System.Drawing.Size(87, 23);
			this.bnDelete.TabIndex = 6;
			this.bnDelete.Text = "&Delete";
			this.bnDelete.UseVisualStyleBackColor = true;
			this.bnDelete.Click += new System.EventHandler(this.bnDelete_Click);
			// 
			// bnNew
			// 
			this.bnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bnNew.Location = new System.Drawing.Point(23, 212);
			this.bnNew.Name = "bnNew";
			this.bnNew.Size = new System.Drawing.Size(87, 23);
			this.bnNew.TabIndex = 4;
			this.bnNew.Text = "&Create";
			this.bnNew.UseVisualStyleBackColor = true;
			this.bnNew.Click += new System.EventHandler(this.bnCreate_Click);
			// 
			// lvAlertDefinitions
			// 
			this.lvAlertDefinitions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lvAlertDefinitions.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
			appearance2.Image = global::Layton.AuditWizard.Administration.Properties.Resources.Alert_16;
			this.lvAlertDefinitions.ItemSettings.Appearance = appearance2;
			this.lvAlertDefinitions.Location = new System.Drawing.Point(23, 47);
			this.lvAlertDefinitions.MainColumn.Text = "Name";
			this.lvAlertDefinitions.MainColumn.Width = 150;
			this.lvAlertDefinitions.Name = "lvAlertDefinitions";
			this.lvAlertDefinitions.Size = new System.Drawing.Size(758, 149);
			ultraListViewSubItemColumn1.Key = "SubItemColumn 0";
			ultraListViewSubItemColumn1.Text = "Description";
			ultraListViewSubItemColumn1.Width = 347;
			ultraListViewSubItemColumn2.Key = "SubItemColumn 1";
			ultraListViewSubItemColumn2.Sorting = Infragistics.Win.UltraWinListView.Sorting.Descending;
			ultraListViewSubItemColumn2.Text = "Date Modified";
			ultraListViewSubItemColumn2.Width = 150;
			this.lvAlertDefinitions.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1,
            ultraListViewSubItemColumn2});
			this.lvAlertDefinitions.TabIndex = 0;
			this.lvAlertDefinitions.Text = "ultraListView2";
			this.lvAlertDefinitions.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
			this.lvAlertDefinitions.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.lvAlerts_ItemSelectionChanged);
			// 
			// ultraLabel1
			// 
			this.ultraLabel1.Location = new System.Drawing.Point(23, 29);
			this.ultraLabel1.Name = "ultraLabel1";
			this.ultraLabel1.Size = new System.Drawing.Size(194, 23);
			this.ultraLabel1.TabIndex = 1;
			this.ultraLabel1.Text = "Currently Defined Alerts";
			// 
			// ultraFormattedLinkLabel1
			// 
			this.ultraFormattedLinkLabel1.Location = new System.Drawing.Point(30, 95);
			this.ultraFormattedLinkLabel1.Name = "ultraFormattedLinkLabel1";
			this.ultraFormattedLinkLabel1.Size = new System.Drawing.Size(758, 93);
			this.ultraFormattedLinkLabel1.TabIndex = 10;
			this.ultraFormattedLinkLabel1.TabStop = true;
			this.ultraFormattedLinkLabel1.Value = resources.GetString("ultraFormattedLinkLabel1.Value");
			// 
			// ultraGroupBox2
			// 
			this.ultraGroupBox2.Controls.Add(this.gbFrequency);
			this.ultraGroupBox2.Location = new System.Drawing.Point(7, 465);
			this.ultraGroupBox2.Name = "ultraGroupBox2";
			this.ultraGroupBox2.Size = new System.Drawing.Size(804, 96);
			this.ultraGroupBox2.TabIndex = 11;
			this.ultraGroupBox2.Text = "Email Alerts";
			// 
			// gbFrequency
			// 
			this.gbFrequency.Controls.Add(this.panelEmailAt);
			this.gbFrequency.Controls.Add(this.dailyRadioButton);
			this.gbFrequency.Controls.Add(this.hourlyRadioButton);
			this.gbFrequency.Location = new System.Drawing.Point(23, 31);
			this.gbFrequency.Name = "gbFrequency";
			this.gbFrequency.Size = new System.Drawing.Size(514, 48);
			this.gbFrequency.TabIndex = 78;
			this.gbFrequency.TabStop = false;
			this.gbFrequency.Text = "Frequency";
			// 
			// panelEmailAt
			// 
			// 
			// panelEmailAt.ClientArea
			// 
			this.panelEmailAt.ClientArea.Controls.Add(this.udteEmailAtTime);
			this.panelEmailAt.ClientArea.Controls.Add(this.ultraLabel2);
			this.panelEmailAt.Location = new System.Drawing.Point(170, 9);
			this.panelEmailAt.Name = "panelEmailAt";
			this.panelEmailAt.Size = new System.Drawing.Size(132, 33);
			this.panelEmailAt.TabIndex = 4;
			this.panelEmailAt.Visible = false;
			// 
			// udteEmailAtTime
			// 
			this.udteEmailAtTime.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
			this.udteEmailAtTime.Location = new System.Drawing.Point(36, 10);
			this.udteEmailAtTime.MaskInput = "{LOC}hh:mm";
			this.udteEmailAtTime.Name = "udteEmailAtTime";
			this.udteEmailAtTime.Size = new System.Drawing.Size(69, 22);
			this.udteEmailAtTime.TabIndex = 3;
			this.udteEmailAtTime.ValueChanged += new System.EventHandler(this.udteEmailAtTime_ValueChanged);
			// 
			// ultraLabel2
			// 
			this.ultraLabel2.AutoSize = true;
			this.ultraLabel2.Location = new System.Drawing.Point(3, 12);
			this.ultraLabel2.Name = "ultraLabel2";
			this.ultraLabel2.Size = new System.Drawing.Size(15, 15);
			this.ultraLabel2.TabIndex = 2;
			this.ultraLabel2.Text = "at";
			// 
			// dailyRadioButton
			// 
			this.dailyRadioButton.AutoSize = true;
			this.dailyRadioButton.BackColor = System.Drawing.Color.Transparent;
			this.dailyRadioButton.Location = new System.Drawing.Point(101, 19);
			this.dailyRadioButton.Name = "dailyRadioButton";
			this.dailyRadioButton.Size = new System.Drawing.Size(48, 17);
			this.dailyRadioButton.TabIndex = 3;
			this.dailyRadioButton.Text = "Daily";
			this.dailyRadioButton.UseVisualStyleBackColor = false;
			this.dailyRadioButton.CheckedChanged += new System.EventHandler(this.hourlyRadioButton_CheckedChanged);
			// 
			// hourlyRadioButton
			// 
			this.hourlyRadioButton.AutoSize = true;
			this.hourlyRadioButton.BackColor = System.Drawing.Color.Transparent;
			this.hourlyRadioButton.Checked = true;
			this.hourlyRadioButton.Location = new System.Drawing.Point(32, 19);
			this.hourlyRadioButton.Name = "hourlyRadioButton";
			this.hourlyRadioButton.Size = new System.Drawing.Size(55, 17);
			this.hourlyRadioButton.TabIndex = 2;
			this.hourlyRadioButton.TabStop = true;
			this.hourlyRadioButton.Text = "Hourly";
			this.hourlyRadioButton.UseVisualStyleBackColor = false;
			this.hourlyRadioButton.CheckedChanged += new System.EventHandler(this.hourlyRadioButton_CheckedChanged);
			// 
			// AlertMonitorSettingsTabView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.ultraGroupBox2);
			this.Controls.Add(this.ultraFormattedLinkLabel1);
			this.Controls.Add(this.ultraGroupBox3);
			this.Controls.Add(this.headerGroupBox);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "AlertMonitorSettingsTabView";
			this.Size = new System.Drawing.Size(818, 561);
			((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).EndInit();
			this.headerGroupBox.ResumeLayout(false);
			this.headerGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox3)).EndInit();
			this.ultraGroupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lvAlertDefinitions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
			this.ultraGroupBox2.ResumeLayout(false);
			this.gbFrequency.ResumeLayout(false);
			this.gbFrequency.PerformLayout();
			this.panelEmailAt.ClientArea.ResumeLayout(false);
			this.panelEmailAt.ClientArea.PerformLayout();
			this.panelEmailAt.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.udteEmailAtTime)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

		private Infragistics.Win.Misc.UltraGroupBox headerGroupBox;
		private Infragistics.Win.Misc.UltraLabel headerLabel;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox3;
        private System.Windows.Forms.Button bnDelete;
        private System.Windows.Forms.Button bnNew;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.Button btnEditAlert;
        private Infragistics.Win.UltraWinListView.UltraListView lvAlertDefinitions;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel ultraFormattedLinkLabel1;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
		private System.Windows.Forms.GroupBox gbFrequency;
		private System.Windows.Forms.RadioButton dailyRadioButton;
		private System.Windows.Forms.RadioButton hourlyRadioButton;
		private Infragistics.Win.Misc.UltraPanel panelEmailAt;
		private Infragistics.Win.Misc.UltraLabel ultraLabel2;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor udteEmailAtTime;
    }
}
