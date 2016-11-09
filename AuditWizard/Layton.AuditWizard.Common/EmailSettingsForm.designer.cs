namespace Layton.AuditWizard.Common
{
    partial class EmailSettingsForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.authCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.userTextBox = new System.Windows.Forms.TextBox();
            this.passTextBox = new System.Windows.Forms.TextBox();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.sSLCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Common.Properties.Resources.email_settings_footer;
            this.footerPictureBox.Location = new System.Drawing.Point(51, 232);
            this.footerPictureBox.Size = new System.Drawing.Size(312, 94);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.okButton.BackColor = System.Drawing.Color.Transparent;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(168, 197);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(87, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = false;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.BackColor = System.Drawing.Color.Transparent;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(262, 197);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = false;
            // 
            // authCheckBox
            // 
            this.authCheckBox.AutoSize = true;
            this.authCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.authCheckBox.Location = new System.Drawing.Point(28, 113);
            this.authCheckBox.Name = "authCheckBox";
            this.authCheckBox.Size = new System.Drawing.Size(306, 17);
            this.authCheckBox.TabIndex = 2;
            this.authCheckBox.Text = "My outgoing SMTP server requires authentication";
            this.authCheckBox.UseVisualStyleBackColor = false;
            this.authCheckBox.CheckedChanged += new System.EventHandler(this.authCheckBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Outgoing SMTP Email Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(48, 142);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "User Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(48, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Password:";
            // 
            // userTextBox
            // 
            this.userTextBox.Enabled = false;
            this.userTextBox.Location = new System.Drawing.Point(132, 139);
            this.userTextBox.Name = "userTextBox";
            this.userTextBox.Size = new System.Drawing.Size(217, 21);
            this.userTextBox.TabIndex = 3;
            // 
            // passTextBox
            // 
            this.passTextBox.Enabled = false;
            this.passTextBox.Location = new System.Drawing.Point(132, 161);
            this.passTextBox.Name = "passTextBox";
            this.passTextBox.Size = new System.Drawing.Size(217, 21);
            this.passTextBox.TabIndex = 4;
            this.passTextBox.UseSystemPasswordChar = true;
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(204, 40);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(44, 21);
            this.portTextBox.TabIndex = 1;
            this.portTextBox.Text = "25";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(24, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(168, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Outgoing SMTP Server Port:";
            // 
            // sSLCheckBox
            // 
            this.sSLCheckBox.AutoSize = true;
            this.sSLCheckBox.Location = new System.Drawing.Point(28, 78);
            this.sSLCheckBox.Name = "sSLCheckBox";
            this.sSLCheckBox.Size = new System.Drawing.Size(124, 17);
            this.sSLCheckBox.TabIndex = 18;
            this.sSLCheckBox.Text = "SSL / TSL";
            this.sSLCheckBox.UseVisualStyleBackColor = true;
            // 
            // EmailSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(363, 325);
            this.Controls.Add(this.sSLCheckBox);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.passTextBox);
            this.Controls.Add(this.userTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.authCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EmailSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Email Settings";
            this.Controls.SetChildIndex(this.okButton, 0);
            this.Controls.SetChildIndex(this.cancelButton, 0);
            this.Controls.SetChildIndex(this.authCheckBox, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.userTextBox, 0);
            this.Controls.SetChildIndex(this.passTextBox, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.portTextBox, 0);
            this.Controls.SetChildIndex(this.sSLCheckBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox authCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox userTextBox;
        private System.Windows.Forms.TextBox passTextBox;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox sSLCheckBox;
    }
}