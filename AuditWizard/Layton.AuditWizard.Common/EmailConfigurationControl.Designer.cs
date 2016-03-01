namespace Layton.AuditWizard.Common
{
	partial class EmailConfigurationControl
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
            this.gbEmailOptions = new Infragistics.Win.Misc.UltraGroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gbEmailFormat = new System.Windows.Forms.GroupBox();
            this.rbText = new System.Windows.Forms.RadioButton();
            this.rbHtml = new System.Windows.Forms.RadioButton();
            this.gbFrequency = new System.Windows.Forms.GroupBox();
            this.weeklyRadioButton = new System.Windows.Forms.RadioButton();
            this.monthlyRadioButton = new System.Windows.Forms.RadioButton();
            this.dailyRadioButton = new System.Windows.Forms.RadioButton();
            this.neverRadioButton = new System.Windows.Forms.RadioButton();
            this.bnSaveEmailSettings = new System.Windows.Forms.Button();
            this.tbSendingEmailAddress = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.bnSendEmail = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tbRecipientEmailAddress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.smtpTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.gbEmailOptions)).BeginInit();
            this.gbEmailOptions.SuspendLayout();
            this.gbEmailFormat.SuspendLayout();
            this.gbFrequency.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbEmailOptions
            // 
            this.gbEmailOptions.Controls.Add(this.label1);
            this.gbEmailOptions.Controls.Add(this.gbEmailFormat);
            this.gbEmailOptions.Controls.Add(this.gbFrequency);
            this.gbEmailOptions.Controls.Add(this.bnSaveEmailSettings);
            this.gbEmailOptions.Controls.Add(this.tbSendingEmailAddress);
            this.gbEmailOptions.Controls.Add(this.label7);
            this.gbEmailOptions.Controls.Add(this.bnSendEmail);
            this.gbEmailOptions.Controls.Add(this.button2);
            this.gbEmailOptions.Controls.Add(this.label4);
            this.gbEmailOptions.Controls.Add(this.tbRecipientEmailAddress);
            this.gbEmailOptions.Controls.Add(this.label5);
            this.gbEmailOptions.Controls.Add(this.smtpTextBox);
            this.gbEmailOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbEmailOptions.Location = new System.Drawing.Point(0, 0);
            this.gbEmailOptions.Name = "gbEmailOptions";
            this.gbEmailOptions.Size = new System.Drawing.Size(642, 284);
            this.gbEmailOptions.TabIndex = 1;
            this.gbEmailOptions.Text = "Email Options";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(608, 39);
            this.label1.TabIndex = 79;
            this.label1.Text = "AuditWizard will send an email with a summary of your current software licensing " +
                "status.\r\n\r\nThe Emails are sent by the AuditWizard Service which will pick up any" +
                " changes made here automatically.";
            // 
            // gbEmailFormat
            // 
            this.gbEmailFormat.Controls.Add(this.rbText);
            this.gbEmailFormat.Controls.Add(this.rbHtml);
            this.gbEmailFormat.Location = new System.Drawing.Point(420, 88);
            this.gbEmailFormat.Name = "gbEmailFormat";
            this.gbEmailFormat.Size = new System.Drawing.Size(153, 49);
            this.gbEmailFormat.TabIndex = 78;
            this.gbEmailFormat.TabStop = false;
            this.gbEmailFormat.Text = "Email Format:";
            // 
            // rbText
            // 
            this.rbText.AutoSize = true;
            this.rbText.Location = new System.Drawing.Point(85, 20);
            this.rbText.Name = "rbText";
            this.rbText.Size = new System.Drawing.Size(50, 17);
            this.rbText.TabIndex = 76;
            this.rbText.Text = "Text";
            this.rbText.UseVisualStyleBackColor = true;
            // 
            // rbHtml
            // 
            this.rbHtml.AutoSize = true;
            this.rbHtml.Checked = true;
            this.rbHtml.Location = new System.Drawing.Point(24, 20);
            this.rbHtml.Name = "rbHtml";
            this.rbHtml.Size = new System.Drawing.Size(55, 17);
            this.rbHtml.TabIndex = 75;
            this.rbHtml.TabStop = true;
            this.rbHtml.Text = "HTML";
            this.rbHtml.UseVisualStyleBackColor = true;
            // 
            // gbFrequency
            // 
            this.gbFrequency.Controls.Add(this.weeklyRadioButton);
            this.gbFrequency.Controls.Add(this.monthlyRadioButton);
            this.gbFrequency.Controls.Add(this.dailyRadioButton);
            this.gbFrequency.Controls.Add(this.neverRadioButton);
            this.gbFrequency.Location = new System.Drawing.Point(41, 88);
            this.gbFrequency.Name = "gbFrequency";
            this.gbFrequency.Size = new System.Drawing.Size(359, 48);
            this.gbFrequency.TabIndex = 77;
            this.gbFrequency.TabStop = false;
            this.gbFrequency.Text = "Frequency";
            // 
            // weeklyRadioButton
            // 
            this.weeklyRadioButton.AutoSize = true;
            this.weeklyRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.weeklyRadioButton.Location = new System.Drawing.Point(101, 19);
            this.weeklyRadioButton.Name = "weeklyRadioButton";
            this.weeklyRadioButton.Size = new System.Drawing.Size(67, 17);
            this.weeklyRadioButton.TabIndex = 3;
            this.weeklyRadioButton.Text = "Weekly";
            this.weeklyRadioButton.UseVisualStyleBackColor = false;
            // 
            // monthlyRadioButton
            // 
            this.monthlyRadioButton.AutoSize = true;
            this.monthlyRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.monthlyRadioButton.Location = new System.Drawing.Point(183, 19);
            this.monthlyRadioButton.Name = "monthlyRadioButton";
            this.monthlyRadioButton.Size = new System.Drawing.Size(69, 17);
            this.monthlyRadioButton.TabIndex = 4;
            this.monthlyRadioButton.Text = "Monthly";
            this.monthlyRadioButton.UseVisualStyleBackColor = false;
            // 
            // dailyRadioButton
            // 
            this.dailyRadioButton.AutoSize = true;
            this.dailyRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.dailyRadioButton.Checked = true;
            this.dailyRadioButton.Location = new System.Drawing.Point(32, 19);
            this.dailyRadioButton.Name = "dailyRadioButton";
            this.dailyRadioButton.Size = new System.Drawing.Size(54, 17);
            this.dailyRadioButton.TabIndex = 2;
            this.dailyRadioButton.TabStop = true;
            this.dailyRadioButton.Text = "Daily";
            this.dailyRadioButton.UseVisualStyleBackColor = false;
            // 
            // neverRadioButton
            // 
            this.neverRadioButton.AutoSize = true;
            this.neverRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.neverRadioButton.Location = new System.Drawing.Point(267, 19);
            this.neverRadioButton.Name = "neverRadioButton";
            this.neverRadioButton.Size = new System.Drawing.Size(59, 17);
            this.neverRadioButton.TabIndex = 5;
            this.neverRadioButton.Text = "Never";
            this.neverRadioButton.UseVisualStyleBackColor = false;
            // 
            // bnSaveEmailSettings
            // 
            this.bnSaveEmailSettings.Location = new System.Drawing.Point(476, 244);
            this.bnSaveEmailSettings.Name = "bnSaveEmailSettings";
            this.bnSaveEmailSettings.Size = new System.Drawing.Size(97, 23);
            this.bnSaveEmailSettings.TabIndex = 73;
            this.bnSaveEmailSettings.Text = "Save Settings";
            this.bnSaveEmailSettings.UseVisualStyleBackColor = true;
            this.bnSaveEmailSettings.Click += new System.EventHandler(this.bnSaveEmailSettings_Click);
            // 
            // tbSendingEmailAddress
            // 
            this.tbSendingEmailAddress.Location = new System.Drawing.Point(274, 158);
            this.tbSendingEmailAddress.Name = "tbSendingEmailAddress";
            this.tbSendingEmailAddress.Size = new System.Drawing.Size(299, 21);
            this.tbSendingEmailAddress.TabIndex = 7;
            this.tbSendingEmailAddress.TextChanged += new System.EventHandler(this.tbSendingEmailAddress_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(45, 162);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(138, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Sender Email Address:";
            // 
            // bnSendEmail
            // 
            this.bnSendEmail.BackColor = System.Drawing.Color.Transparent;
            this.bnSendEmail.Enabled = false;
            this.bnSendEmail.Location = new System.Drawing.Point(383, 244);
            this.bnSendEmail.Name = "bnSendEmail";
            this.bnSendEmail.Size = new System.Drawing.Size(87, 23);
            this.bnSendEmail.TabIndex = 11;
            this.bnSendEmail.Text = "Send Email";
            this.bnSendEmail.UseVisualStyleBackColor = false;
            this.bnSendEmail.Click += new System.EventHandler(this.emailButton_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.Location = new System.Drawing.Point(48, 244);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "Advanced";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.advancedButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 214);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "SMTP Host:";
            // 
            // tbRecipientEmailAddress
            // 
            this.tbRecipientEmailAddress.Location = new System.Drawing.Point(274, 184);
            this.tbRecipientEmailAddress.Name = "tbRecipientEmailAddress";
            this.tbRecipientEmailAddress.Size = new System.Drawing.Size(299, 21);
            this.tbRecipientEmailAddress.TabIndex = 8;
            this.tbRecipientEmailAddress.TextChanged += new System.EventHandler(this.tbRecipientEmailAddress_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(45, 188);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(165, 13);
            this.label5.TabIndex = 72;
            this.label5.Text = "Recipient Email Address(s):";
            // 
            // smtpTextBox
            // 
            this.smtpTextBox.Location = new System.Drawing.Point(274, 210);
            this.smtpTextBox.Name = "smtpTextBox";
            this.smtpTextBox.Size = new System.Drawing.Size(299, 21);
            this.smtpTextBox.TabIndex = 10;
            this.smtpTextBox.TextChanged += new System.EventHandler(this.smtpTextBox_TextChanged);
            // 
            // EmailConfigurationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.gbEmailOptions);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "EmailConfigurationControl";
            this.Size = new System.Drawing.Size(642, 284);
            this.Leave += new System.EventHandler(this.EmailConfigurationControl_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.gbEmailOptions)).EndInit();
            this.gbEmailOptions.ResumeLayout(false);
            this.gbEmailOptions.PerformLayout();
            this.gbEmailFormat.ResumeLayout(false);
            this.gbEmailFormat.PerformLayout();
            this.gbFrequency.ResumeLayout(false);
            this.gbFrequency.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private Infragistics.Win.Misc.UltraGroupBox gbEmailOptions;
		private System.Windows.Forms.TextBox tbSendingEmailAddress;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button bnSendEmail;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbRecipientEmailAddress;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox smtpTextBox;
		private System.Windows.Forms.RadioButton neverRadioButton;
        private System.Windows.Forms.RadioButton dailyRadioButton;
		private System.Windows.Forms.RadioButton weeklyRadioButton;
        private System.Windows.Forms.RadioButton monthlyRadioButton;
        private System.Windows.Forms.Button bnSaveEmailSettings;
        private System.Windows.Forms.GroupBox gbFrequency;
        private System.Windows.Forms.RadioButton rbText;
        private System.Windows.Forms.RadioButton rbHtml;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbEmailFormat;
	}
}
