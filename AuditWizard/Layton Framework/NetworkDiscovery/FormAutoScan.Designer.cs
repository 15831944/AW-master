namespace Layton.NetworkDiscovery
{
    partial class FormAutoScan
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
            this.ubAutoScanSettings = new Infragistics.Win.Misc.UltraGroupBox();
            this.tbScanInterval = new System.Windows.Forms.TextBox();
            this.cbScanInterval = new System.Windows.Forms.ComboBox();
            this.lblScanInterval = new System.Windows.Forms.Label();
            this.cbEnableScan = new System.Windows.Forms.CheckBox();
            this.cbDeployAgent = new System.Windows.Forms.CheckBox();
            this.lbAutoScanDesc = new Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel();
            this.bnApply = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ubAutoScanSettings)).BeginInit();
            this.ubAutoScanSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // ubAutoScanSettings
            // 
            this.ubAutoScanSettings.Controls.Add(this.tbScanInterval);
            this.ubAutoScanSettings.Controls.Add(this.cbScanInterval);
            this.ubAutoScanSettings.Controls.Add(this.lblScanInterval);
            this.ubAutoScanSettings.Controls.Add(this.cbEnableScan);
            this.ubAutoScanSettings.Controls.Add(this.cbDeployAgent);
            this.ubAutoScanSettings.Controls.Add(this.lbAutoScanDesc);
            this.ubAutoScanSettings.Location = new System.Drawing.Point(14, 26);
            this.ubAutoScanSettings.Name = "ubAutoScanSettings";
            this.ubAutoScanSettings.Size = new System.Drawing.Size(464, 254);
            this.ubAutoScanSettings.TabIndex = 63;
            this.ubAutoScanSettings.Text = "Auto-Discovery Settings";
            // 
            // tbScanInterval
            // 
            this.tbScanInterval.Enabled = false;
            this.tbScanInterval.Location = new System.Drawing.Point(9, 145);
            this.tbScanInterval.Name = "tbScanInterval";
            this.tbScanInterval.Size = new System.Drawing.Size(39, 21);
            this.tbScanInterval.TabIndex = 6;
            this.tbScanInterval.Text = "8";
            this.tbScanInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbScanInterval.TextChanged += new System.EventHandler(this.tbScanInterval_TextChanged);
            // 
            // cbScanInterval
            // 
            this.cbScanInterval.Enabled = false;
            this.cbScanInterval.FormattingEnabled = true;
            this.cbScanInterval.Items.AddRange(new object[] {
            "minutes",
            "hours",
            "days"});
            this.cbScanInterval.Location = new System.Drawing.Point(54, 145);
            this.cbScanInterval.Name = "cbScanInterval";
            this.cbScanInterval.Size = new System.Drawing.Size(140, 21);
            this.cbScanInterval.TabIndex = 5;
            this.cbScanInterval.SelectedIndexChanged += new System.EventHandler(this.cbScanInterval_SelectedIndexChanged);
            // 
            // lblScanInterval
            // 
            this.lblScanInterval.AutoSize = true;
            this.lblScanInterval.Enabled = false;
            this.lblScanInterval.Location = new System.Drawing.Point(7, 119);
            this.lblScanInterval.Name = "lblScanInterval";
            this.lblScanInterval.Size = new System.Drawing.Size(125, 13);
            this.lblScanInterval.TabIndex = 4;
            this.lblScanInterval.Text = "Perform scan every:";
            // 
            // cbEnableScan
            // 
            this.cbEnableScan.AutoSize = true;
            this.cbEnableScan.Location = new System.Drawing.Point(10, 80);
            this.cbEnableScan.Name = "cbEnableScan";
            this.cbEnableScan.Size = new System.Drawing.Size(156, 17);
            this.cbEnableScan.TabIndex = 3;
            this.cbEnableScan.Text = "Enable Auto-Discovery";
            this.cbEnableScan.UseVisualStyleBackColor = true;
            this.cbEnableScan.CheckedChanged += new System.EventHandler(this.cbEnableScan_CheckedChanged);
            // 
            // cbDeployAgent
            // 
            this.cbDeployAgent.AutoSize = true;
            this.cbDeployAgent.Enabled = false;
            this.cbDeployAgent.Location = new System.Drawing.Point(10, 186);
            this.cbDeployAgent.Name = "cbDeployAgent";
            this.cbDeployAgent.Size = new System.Drawing.Size(314, 17);
            this.cbDeployAgent.TabIndex = 2;
            this.cbDeployAgent.Text = "Deploy AuditAgent to newly discovered computers";
            this.cbDeployAgent.UseVisualStyleBackColor = true;
            this.cbDeployAgent.CheckedChanged += new System.EventHandler(this.cbDeployAgent_CheckedChanged);
            // 
            // lbAutoScanDesc
            // 
            this.lbAutoScanDesc.Location = new System.Drawing.Point(10, 46);
            this.lbAutoScanDesc.Name = "lbAutoScanDesc";
            this.lbAutoScanDesc.Size = new System.Drawing.Size(448, 38);
            this.lbAutoScanDesc.TabIndex = 1;
            this.lbAutoScanDesc.TabStop = true;
            this.lbAutoScanDesc.Value = "Configure AuditWizard to automatically discover new assets on your network.";
            // 
            // bnApply
            // 
            this.bnApply.Enabled = false;
            this.bnApply.Location = new System.Drawing.Point(391, 295);
            this.bnApply.Name = "bnApply";
            this.bnApply.Size = new System.Drawing.Size(87, 23);
            this.bnApply.TabIndex = 11;
            this.bnApply.Text = "Apply";
            this.bnApply.UseVisualStyleBackColor = true;
            this.bnApply.Click += new System.EventHandler(this.bnApply_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Location = new System.Drawing.Point(297, 295);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(87, 23);
            this.bnCancel.TabIndex = 10;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // bnOK
            // 
            this.bnOK.Location = new System.Drawing.Point(202, 295);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(87, 23);
            this.bnOK.TabIndex = 9;
            this.bnOK.Text = "OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // FormAutoScan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(490, 330);
            this.Controls.Add(this.bnApply);
            this.Controls.Add(this.ubAutoScanSettings);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormAutoScan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto-Discovery Settings";
            this.Load += new System.EventHandler(this.Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ubAutoScanSettings)).EndInit();
            this.ubAutoScanSettings.ResumeLayout(false);
            this.ubAutoScanSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ubAutoScanSettings;
        private System.Windows.Forms.TextBox tbScanInterval;
        private System.Windows.Forms.ComboBox cbScanInterval;
        private System.Windows.Forms.Label lblScanInterval;
        private System.Windows.Forms.CheckBox cbEnableScan;
        private System.Windows.Forms.CheckBox cbDeployAgent;
        private Infragistics.Win.FormattedLinkLabel.UltraFormattedLinkLabel lbAutoScanDesc;
        private System.Windows.Forms.Button bnApply;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnOK;
    }
}