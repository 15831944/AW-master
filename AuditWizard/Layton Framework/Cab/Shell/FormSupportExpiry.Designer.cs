namespace AuditWizardv8
{
    partial class FormSupportExpiry
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
            this.checkBoxNotify = new System.Windows.Forms.CheckBox();
            this.labelMessageText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkBoxNotify
            // 
            this.checkBoxNotify.AutoSize = true;
            this.checkBoxNotify.Location = new System.Drawing.Point(369, 69);
            this.checkBoxNotify.Name = "checkBoxNotify";
            this.checkBoxNotify.Size = new System.Drawing.Size(149, 17);
            this.checkBoxNotify.TabIndex = 0;
            this.checkBoxNotify.Text = "Don\'t notify me again";
            this.checkBoxNotify.UseVisualStyleBackColor = true;
            // 
            // labelMessageText
            // 
            this.labelMessageText.AutoSize = true;
            this.labelMessageText.Location = new System.Drawing.Point(22, 26);
            this.labelMessageText.Name = "labelMessageText";
            this.labelMessageText.Size = new System.Drawing.Size(41, 13);
            this.labelMessageText.TabIndex = 1;
            this.labelMessageText.Text = "label1";
            // 
            // FormSupportExpiry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(530, 97);
            this.Controls.Add(this.labelMessageText);
            this.Controls.Add(this.checkBoxNotify);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSupportExpiry";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AuditWizard Support Expiry Reminder";
            this.Load += new System.EventHandler(this.FormSupportExpiry_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSupportExpiry_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxNotify;
        private System.Windows.Forms.Label labelMessageText;
    }
}