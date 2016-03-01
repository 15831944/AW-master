namespace Layton.NetworkDiscovery
{
    partial class IpAddressRangeForm
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
            this.startIpAddress = new IPAddressControlLib.IPAddressControl();
            this.endIpAddress = new IPAddressControlLib.IPAddressControl();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.okButton = new Infragistics.Win.Misc.UltraButton();
            this.cancelButton = new Infragistics.Win.Misc.UltraButton();
            this.SuspendLayout();
            // 
            // startIpAddress
            // 
            this.startIpAddress.AutoHeight = true;
            this.startIpAddress.BackColor = System.Drawing.SystemColors.Window;
            this.startIpAddress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.startIpAddress.Location = new System.Drawing.Point(119, 47);
            this.startIpAddress.MinimumSize = new System.Drawing.Size(102, 21);
            this.startIpAddress.Name = "startIpAddress";
            this.startIpAddress.ReadOnly = false;
            this.startIpAddress.Size = new System.Drawing.Size(167, 21);
            this.startIpAddress.TabIndex = 0;
            this.startIpAddress.Text = "...";
            // 
            // endIpAddress
            // 
            this.endIpAddress.AutoHeight = true;
            this.endIpAddress.BackColor = System.Drawing.SystemColors.Window;
            this.endIpAddress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.endIpAddress.Location = new System.Drawing.Point(119, 73);
            this.endIpAddress.MinimumSize = new System.Drawing.Size(102, 21);
            this.endIpAddress.Name = "endIpAddress";
            this.endIpAddress.ReadOnly = false;
            this.endIpAddress.Size = new System.Drawing.Size(167, 21);
            this.endIpAddress.TabIndex = 1;
            this.endIpAddress.Text = "...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Please enter an IP address range:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(23, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Start Address:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(23, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "End Address:";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(148, 113);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(66, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(220, 113);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(66, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // IpAddressRangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(307, 149);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.endIpAddress);
            this.Controls.Add(this.startIpAddress);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IpAddressRangeForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Network Discovery Settings";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.IpAddressRangeForm_Paint);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IpAddressRangeForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private IPAddressControlLib.IPAddressControl startIpAddress;
        private IPAddressControlLib.IPAddressControl endIpAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private Infragistics.Win.Misc.UltraButton okButton;
        private Infragistics.Win.Misc.UltraButton cancelButton;
    }
}