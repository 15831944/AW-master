namespace Layton.AuditWizard.Common
{
	partial class FormAuditWizardServiceControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAuditWizardServiceControl));
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            this.pbServiceStatus = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.panelLogonCredentials = new System.Windows.Forms.Panel();
            this.panelRunSystemAs = new System.Windows.Forms.Panel();
            this.tbConfirmPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.bnBrowse = new Infragistics.Win.Misc.UltraButton();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbLocalSystem = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.label4 = new System.Windows.Forms.Label();
            this.bnStart = new System.Windows.Forms.Button();
            this.bnStop = new System.Windows.Forms.Button();
            this.bnRemove = new System.Windows.Forms.Button();
            this.bnViewLog = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            this.panelLogonCredentials.SuspendLayout();
            this.panelRunSystemAs.SuspendLayout();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Common.Properties.Resources.aw_service_control_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(35, 426);
            // 
            // pbServiceStatus
            // 
            this.pbServiceStatus.AutoSize = true;
            this.pbServiceStatus.BorderShadowColor = System.Drawing.Color.Empty;
            this.pbServiceStatus.Image = ((object)(resources.GetObject("pbServiceStatus.Image")));
            this.pbServiceStatus.Location = new System.Drawing.Point(357, 95);
            this.pbServiceStatus.Name = "pbServiceStatus";
            this.pbServiceStatus.Size = new System.Drawing.Size(66, 18);
            this.pbServiceStatus.TabIndex = 42;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(14, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(336, 20);
            this.label7.TabIndex = 41;
            this.label7.Text = "The AuditWizard Service is currently";
            // 
            // ultraGroupBox1
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.ultraGroupBox1.Appearance = appearance1;
            this.ultraGroupBox1.Controls.Add(this.panelLogonCredentials);
            this.ultraGroupBox1.Controls.Add(this.label4);
            this.ultraGroupBox1.Location = new System.Drawing.Point(17, 119);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(457, 223);
            this.ultraGroupBox1.TabIndex = 43;
            this.ultraGroupBox1.Text = "Run Service As";
            // 
            // panelLogonCredentials
            // 
            this.panelLogonCredentials.Controls.Add(this.panelRunSystemAs);
            this.panelLogonCredentials.Controls.Add(this.cbLocalSystem);
            this.panelLogonCredentials.Location = new System.Drawing.Point(10, 67);
            this.panelLogonCredentials.Name = "panelLogonCredentials";
            this.panelLogonCredentials.Size = new System.Drawing.Size(440, 150);
            this.panelLogonCredentials.TabIndex = 43;
            // 
            // panelRunSystemAs
            // 
            this.panelRunSystemAs.Controls.Add(this.tbConfirmPassword);
            this.panelRunSystemAs.Controls.Add(this.label5);
            this.panelRunSystemAs.Controls.Add(this.bnBrowse);
            this.panelRunSystemAs.Controls.Add(this.tbPassword);
            this.panelRunSystemAs.Controls.Add(this.tbUsername);
            this.panelRunSystemAs.Controls.Add(this.label2);
            this.panelRunSystemAs.Controls.Add(this.label1);
            this.panelRunSystemAs.Enabled = false;
            this.panelRunSystemAs.Location = new System.Drawing.Point(21, 29);
            this.panelRunSystemAs.Name = "panelRunSystemAs";
            this.panelRunSystemAs.Size = new System.Drawing.Size(412, 118);
            this.panelRunSystemAs.TabIndex = 3;
            // 
            // tbConfirmPassword
            // 
            this.tbConfirmPassword.Location = new System.Drawing.Point(120, 75);
            this.tbConfirmPassword.Name = "tbConfirmPassword";
            this.tbConfirmPassword.Size = new System.Drawing.Size(187, 21);
            this.tbConfirmPassword.TabIndex = 4;
            this.tbConfirmPassword.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Confirm Password:";
            // 
            // bnBrowse
            // 
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bnBrowse.Appearance = appearance5;
            this.bnBrowse.Location = new System.Drawing.Point(315, 14);
            this.bnBrowse.Name = "bnBrowse";
            this.bnBrowse.Size = new System.Drawing.Size(87, 23);
            this.bnBrowse.TabIndex = 10;
            this.bnBrowse.TabStop = false;
            this.bnBrowse.Text = "&Browse";
            this.bnBrowse.Click += new System.EventHandler(this.bnBrowse_Click);
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(120, 45);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(187, 21);
            this.tbPassword.TabIndex = 3;
            this.tbPassword.UseSystemPasswordChar = true;
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(120, 14);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(187, 21);
            this.tbUsername.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username:";
            // 
            // cbLocalSystem
            // 
            this.cbLocalSystem.Checked = true;
            this.cbLocalSystem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLocalSystem.Location = new System.Drawing.Point(3, 3);
            this.cbLocalSystem.Name = "cbLocalSystem";
            this.cbLocalSystem.Size = new System.Drawing.Size(140, 20);
            this.cbLocalSystem.TabIndex = 2;
            this.cbLocalSystem.Text = "Local System";
            this.cbLocalSystem.CheckedChanged += new System.EventHandler(this.cbLocalSystem_CheckedChanged);
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(7, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(429, 35);
            this.label4.TabIndex = 42;
            this.label4.Text = "Please note that the Service MUST be removed before changing the Logon credential" +
                "s.";
            // 
            // bnStart
            // 
            this.bnStart.Location = new System.Drawing.Point(63, 348);
            this.bnStart.Name = "bnStart";
            this.bnStart.Size = new System.Drawing.Size(87, 23);
            this.bnStart.TabIndex = 44;
            this.bnStart.Text = "&Start";
            this.bnStart.UseVisualStyleBackColor = true;
            this.bnStart.Click += new System.EventHandler(this.bnStart_Click);
            // 
            // bnStop
            // 
            this.bnStop.Location = new System.Drawing.Point(157, 348);
            this.bnStop.Name = "bnStop";
            this.bnStop.Size = new System.Drawing.Size(87, 23);
            this.bnStop.TabIndex = 45;
            this.bnStop.Text = "S&top";
            this.bnStop.UseVisualStyleBackColor = true;
            this.bnStop.Click += new System.EventHandler(this.bnStop_Click);
            // 
            // bnRemove
            // 
            this.bnRemove.Location = new System.Drawing.Point(252, 348);
            this.bnRemove.Name = "bnRemove";
            this.bnRemove.Size = new System.Drawing.Size(87, 23);
            this.bnRemove.TabIndex = 46;
            this.bnRemove.Text = "&Remove";
            this.bnRemove.UseVisualStyleBackColor = true;
            this.bnRemove.Click += new System.EventHandler(this.bnRemove_Click);
            // 
            // bnViewLog
            // 
            this.bnViewLog.Location = new System.Drawing.Point(346, 348);
            this.bnViewLog.Name = "bnViewLog";
            this.bnViewLog.Size = new System.Drawing.Size(87, 23);
            this.bnViewLog.TabIndex = 47;
            this.bnViewLog.Text = "&View Log";
            this.bnViewLog.UseVisualStyleBackColor = true;
            this.bnViewLog.Click += new System.EventHandler(this.bnViewLog_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(387, 396);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 50;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOK
            // 
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(293, 396);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 49;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(14, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(454, 68);
            this.label3.TabIndex = 51;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // FormAuditWizardServiceControl
            // 
            this.AcceptButton = this.bnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(482, 546);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnViewLog);
            this.Controls.Add(this.bnRemove);
            this.Controls.Add(this.bnStop);
            this.Controls.Add(this.bnStart);
            this.Controls.Add(this.ultraGroupBox1);
            this.Controls.Add(this.pbServiceStatus);
            this.Controls.Add(this.label7);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormAuditWizardServiceControl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AuditWizard Service Control";
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.label7, 0);
            this.Controls.SetChildIndex(this.pbServiceStatus, 0);
            this.Controls.SetChildIndex(this.ultraGroupBox1, 0);
            this.Controls.SetChildIndex(this.bnStart, 0);
            this.Controls.SetChildIndex(this.bnStop, 0);
            this.Controls.SetChildIndex(this.bnRemove, 0);
            this.Controls.SetChildIndex(this.bnViewLog, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.panelLogonCredentials.ResumeLayout(false);
            this.panelRunSystemAs.ResumeLayout(false);
            this.panelRunSystemAs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinEditors.UltraPictureBox pbServiceStatus;
		private System.Windows.Forms.Label label7;
		private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
		private System.Windows.Forms.Button bnStart;
		private System.Windows.Forms.Button bnStop;
		private System.Windows.Forms.Button bnRemove;
		private System.Windows.Forms.Button bnViewLog;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panelLogonCredentials;
		private System.Windows.Forms.Panel panelRunSystemAs;
		private Infragistics.Win.Misc.UltraButton bnBrowse;
		private System.Windows.Forms.TextBox tbPassword;
		private System.Windows.Forms.TextBox tbUsername;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private Infragistics.Win.UltraWinEditors.UltraCheckEditor cbLocalSystem;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbConfirmPassword;
		private System.Windows.Forms.Label label5;
	}
}