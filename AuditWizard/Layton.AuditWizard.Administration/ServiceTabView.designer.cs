namespace Layton.AuditWizard.Administration
{
	partial class ServiceTabView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceTabView));
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cbUsePing = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.bnAuditWizardServiceControl = new Infragistics.Win.Misc.UltraButton();
            this.pbAuditWizardServiceStatus = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.label12 = new System.Windows.Forms.Label();
            this.activityTabPage = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.emailButton = new System.Windows.Forms.Button();
            this.advancedButton = new System.Windows.Forms.Button();
            this.smtpTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.neverRadioButton = new System.Windows.Forms.RadioButton();
            this.monthlyRadioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.weeklyRadioButton = new System.Windows.Forms.RadioButton();
            this.dailyRadioButton = new System.Windows.Forms.RadioButton();
            this.instructionLabel = new Infragistics.Win.Misc.UltraLabel();
            this.footerPictureBox = new System.Windows.Forms.PictureBox();
            this.serviceTabControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.headerGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.headerLabel = new Infragistics.Win.Misc.UltraLabel();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            this.activityTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serviceTabControl)).BeginInit();
            this.serviceTabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).BeginInit();
            this.headerGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.AutoScroll = true;
            this.ultraTabPageControl1.Controls.Add(this.ultraGroupBox1);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(0, 0);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(817, 338);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.cbUsePing);
            this.ultraGroupBox1.Controls.Add(this.label4);
            this.ultraGroupBox1.Controls.Add(this.label14);
            this.ultraGroupBox1.Controls.Add(this.bnAuditWizardServiceControl);
            this.ultraGroupBox1.Controls.Add(this.pbAuditWizardServiceStatus);
            this.ultraGroupBox1.Controls.Add(this.label12);
            this.ultraGroupBox1.Location = new System.Drawing.Point(23, 30);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(776, 288);
            this.ultraGroupBox1.TabIndex = 53;
            this.ultraGroupBox1.Text = "AuditWizard Service Control";
            // 
            // cbUsePing
            // 
            this.cbUsePing.AutoSize = true;
            this.cbUsePing.Location = new System.Drawing.Point(47, 251);
            this.cbUsePing.Name = "cbUsePing";
            this.cbUsePing.Size = new System.Drawing.Size(331, 17);
            this.cbUsePing.TabIndex = 57;
            this.cbUsePing.Text = "Use a TCP/IP \'Ping\' to determine connectivity to a PC";
            this.cbUsePing.UseVisualStyleBackColor = true;
            this.cbUsePing.CheckStateChanged += new System.EventHandler(this.cbUsePing_CheckStateChanged);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(15, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(745, 79);
            this.label4.TabIndex = 56;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.Location = new System.Drawing.Point(40, 127);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(164, 13);
            this.label14.TabIndex = 55;
            this.label14.Text = "The AuditWizard Service is ";
            // 
            // bnAuditWizardServiceControl
            // 
            this.bnAuditWizardServiceControl.Location = new System.Drawing.Point(303, 122);
            this.bnAuditWizardServiceControl.Name = "bnAuditWizardServiceControl";
            this.bnAuditWizardServiceControl.Size = new System.Drawing.Size(107, 23);
            this.bnAuditWizardServiceControl.TabIndex = 54;
            this.bnAuditWizardServiceControl.Text = "Service &Control";
            this.bnAuditWizardServiceControl.Click += new System.EventHandler(this.bnAuditWizardServiceControl_Click);
            // 
            // pbAuditWizardServiceStatus
            // 
            this.pbAuditWizardServiceStatus.AutoSize = true;
            this.pbAuditWizardServiceStatus.BorderShadowColor = System.Drawing.Color.Empty;
            this.pbAuditWizardServiceStatus.Image = ((object)(resources.GetObject("pbAuditWizardServiceStatus.Image")));
            this.pbAuditWizardServiceStatus.Location = new System.Drawing.Point(213, 124);
            this.pbAuditWizardServiceStatus.Name = "pbAuditWizardServiceStatus";
            this.pbAuditWizardServiceStatus.Size = new System.Drawing.Size(66, 18);
            this.pbAuditWizardServiceStatus.TabIndex = 53;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Location = new System.Drawing.Point(15, 39);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(745, 78);
            this.label12.TabIndex = 52;
            this.label12.Text = resources.GetString("label12.Text");
            // 
            // activityTabPage
            // 
            this.activityTabPage.Controls.Add(this.emailButton);
            this.activityTabPage.Controls.Add(this.advancedButton);
            this.activityTabPage.Controls.Add(this.smtpTextBox);
            this.activityTabPage.Controls.Add(this.label3);
            this.activityTabPage.Controls.Add(this.emailTextBox);
            this.activityTabPage.Controls.Add(this.label2);
            this.activityTabPage.Controls.Add(this.neverRadioButton);
            this.activityTabPage.Controls.Add(this.monthlyRadioButton);
            this.activityTabPage.Controls.Add(this.label1);
            this.activityTabPage.Controls.Add(this.weeklyRadioButton);
            this.activityTabPage.Controls.Add(this.dailyRadioButton);
            this.activityTabPage.Controls.Add(this.instructionLabel);
            this.activityTabPage.Controls.Add(this.footerPictureBox);
            this.activityTabPage.Location = new System.Drawing.Point(-10000, -10000);
            this.activityTabPage.Name = "activityTabPage";
            this.activityTabPage.Size = new System.Drawing.Size(825, 535);
            // 
            // emailButton
            // 
            this.emailButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.emailButton.BackColor = System.Drawing.Color.Transparent;
            this.emailButton.Location = new System.Drawing.Point(316, 175);
            this.emailButton.Name = "emailButton";
            this.emailButton.Size = new System.Drawing.Size(75, 23);
            this.emailButton.TabIndex = 29;
            this.emailButton.Text = "Send Email";
            this.emailButton.UseVisualStyleBackColor = false;
            // 
            // advancedButton
            // 
            this.advancedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.advancedButton.BackColor = System.Drawing.Color.Transparent;
            this.advancedButton.Location = new System.Drawing.Point(397, 175);
            this.advancedButton.Name = "advancedButton";
            this.advancedButton.Size = new System.Drawing.Size(75, 23);
            this.advancedButton.TabIndex = 28;
            this.advancedButton.Text = "Advanced";
            this.advancedButton.UseVisualStyleBackColor = false;
            // 
            // smtpTextBox
            // 
            this.smtpTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.smtpTextBox.Location = new System.Drawing.Point(98, 149);
            this.smtpTextBox.Name = "smtpTextBox";
            this.smtpTextBox.Size = new System.Drawing.Size(374, 20);
            this.smtpTextBox.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "SMTP Host:";
            // 
            // emailTextBox
            // 
            this.emailTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.emailTextBox.Location = new System.Drawing.Point(98, 123);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.Size = new System.Drawing.Size(374, 20);
            this.emailTextBox.TabIndex = 25;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 126);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Email Address:";
            // 
            // neverRadioButton
            // 
            this.neverRadioButton.AutoSize = true;
            this.neverRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.neverRadioButton.Location = new System.Drawing.Point(267, 79);
            this.neverRadioButton.Name = "neverRadioButton";
            this.neverRadioButton.Size = new System.Drawing.Size(54, 17);
            this.neverRadioButton.TabIndex = 23;
            this.neverRadioButton.Text = "Never";
            this.neverRadioButton.UseVisualStyleBackColor = false;
            // 
            // monthlyRadioButton
            // 
            this.monthlyRadioButton.AutoSize = true;
            this.monthlyRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.monthlyRadioButton.Location = new System.Drawing.Point(204, 79);
            this.monthlyRadioButton.Name = "monthlyRadioButton";
            this.monthlyRadioButton.Size = new System.Drawing.Size(62, 17);
            this.monthlyRadioButton.TabIndex = 22;
            this.monthlyRadioButton.Text = "Monthly";
            this.monthlyRadioButton.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Frequency:";
            // 
            // weeklyRadioButton
            // 
            this.weeklyRadioButton.AutoSize = true;
            this.weeklyRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.weeklyRadioButton.Location = new System.Drawing.Point(139, 79);
            this.weeklyRadioButton.Name = "weeklyRadioButton";
            this.weeklyRadioButton.Size = new System.Drawing.Size(61, 17);
            this.weeklyRadioButton.TabIndex = 20;
            this.weeklyRadioButton.Text = "Weekly";
            this.weeklyRadioButton.UseVisualStyleBackColor = false;
            // 
            // dailyRadioButton
            // 
            this.dailyRadioButton.AutoSize = true;
            this.dailyRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.dailyRadioButton.Checked = true;
            this.dailyRadioButton.Location = new System.Drawing.Point(85, 79);
            this.dailyRadioButton.Name = "dailyRadioButton";
            this.dailyRadioButton.Size = new System.Drawing.Size(48, 17);
            this.dailyRadioButton.TabIndex = 19;
            this.dailyRadioButton.TabStop = true;
            this.dailyRadioButton.Text = "Daily";
            this.dailyRadioButton.UseVisualStyleBackColor = false;
            // 
            // instructionLabel
            // 
            this.instructionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.instructionLabel.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.instructionLabel.Location = new System.Drawing.Point(16, 14);
            this.instructionLabel.Name = "instructionLabel";
            this.instructionLabel.Size = new System.Drawing.Size(793, 69);
            this.instructionLabel.TabIndex = 17;
            this.instructionLabel.Text = resources.GetString("instructionLabel.Text");
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.footerPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.footerPictureBox.Image = global::Layton.AuditWizard.Administration.Properties.Resources.email_settings_footer;
            this.footerPictureBox.Location = new System.Drawing.Point(510, 440);
            this.footerPictureBox.Name = "footerPictureBox";
            this.footerPictureBox.Size = new System.Drawing.Size(312, 92);
            this.footerPictureBox.TabIndex = 7;
            this.footerPictureBox.TabStop = false;
            // 
            // serviceTabControl
            // 
            this.serviceTabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.serviceTabControl.Controls.Add(this.ultraTabPageControl1);
            this.serviceTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serviceTabControl.Location = new System.Drawing.Point(0, 80);
            this.serviceTabControl.Name = "serviceTabControl";
            this.serviceTabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.serviceTabControl.Size = new System.Drawing.Size(817, 338);
            this.serviceTabControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
            this.serviceTabControl.TabIndex = 3;
            this.serviceTabControl.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.TopLeft;
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "User Accounts";
            this.serviceTabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1});
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(817, 338);
            // 
            // headerGroupBox
            // 
            this.headerGroupBox.Controls.Add(this.headerLabel);
            this.headerGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerGroupBox.Location = new System.Drawing.Point(0, 0);
            this.headerGroupBox.Name = "headerGroupBox";
            this.headerGroupBox.Size = new System.Drawing.Size(817, 80);
            this.headerGroupBox.TabIndex = 7;
            this.headerGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // headerLabel
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            appearance1.Image = global::Layton.AuditWizard.Administration.Properties.Resources.aw_service_control_72;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.headerLabel.Appearance = appearance1;
            this.headerLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ImageSize = new System.Drawing.Size(72, 72);
            this.headerLabel.Location = new System.Drawing.Point(7, 5);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.headerLabel.Size = new System.Drawing.Size(295, 72);
            this.headerLabel.TabIndex = 5;
            this.headerLabel.Text = "AuditWizard Services";
            // 
            // ServiceTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.serviceTabControl);
            this.Controls.Add(this.headerGroupBox);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "ServiceTabView";
            this.Size = new System.Drawing.Size(817, 418);
            this.ultraTabPageControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            this.activityTabPage.ResumeLayout(false);
            this.activityTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serviceTabControl)).EndInit();
            this.serviceTabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).EndInit();
            this.headerGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

		private Infragistics.Win.UltraWinTabControl.UltraTabControl serviceTabControl;
		private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
		private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
		private Infragistics.Win.UltraWinTabControl.UltraTabPageControl activityTabPage;
		private System.Windows.Forms.PictureBox footerPictureBox;
		private System.Windows.Forms.Button emailButton;
		private System.Windows.Forms.Button advancedButton;
		private System.Windows.Forms.TextBox smtpTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox emailTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton neverRadioButton;
		private System.Windows.Forms.RadioButton monthlyRadioButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton weeklyRadioButton;
		private System.Windows.Forms.RadioButton dailyRadioButton;
		private Infragistics.Win.Misc.UltraLabel instructionLabel;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
		private System.Windows.Forms.Label label14;
		private Infragistics.Win.Misc.UltraButton bnAuditWizardServiceControl;
		private Infragistics.Win.UltraWinEditors.UltraPictureBox pbAuditWizardServiceStatus;
		private System.Windows.Forms.Label label12;
		private Infragistics.Win.Misc.UltraGroupBox headerGroupBox;
		private Infragistics.Win.Misc.UltraLabel headerLabel;
		private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbUsePing;
    }
}
