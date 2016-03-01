namespace Layton.NetworkDiscovery
{
    partial class NetworkDiscoveryTabView
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.label2 = new System.Windows.Forms.Label();
            this.lblComputers = new System.Windows.Forms.Label();
            this.lbPrinters = new System.Windows.Forms.Label();
            this.lblNetworkDevices = new System.Windows.Forms.Label();
            this.lblComputerCount = new System.Windows.Forms.Label();
            this.lblPrinterCount = new System.Windows.Forms.Label();
            this.lblNetworkDeviceCount = new System.Windows.Forms.Label();
            this.pbDiscoveryProgress = new System.Windows.Forms.ProgressBar();
            this.gbNetDiscovery = new System.Windows.Forms.GroupBox();
            this.lbStatusText = new System.Windows.Forms.Label();
            this.lbDiscProg = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.lbDiscoverdAssetCount = new System.Windows.Forms.Label();
            this.headerGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.headerLabel = new Infragistics.Win.Misc.UltraLabel();
            this.gbNetDiscovery.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).BeginInit();
            this.headerGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 234);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(171, 21);
            this.label2.TabIndex = 8;
            this.label2.Text = "Total Assets Discovered:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblComputers
            // 
            this.lblComputers.AutoSize = true;
            this.lblComputers.BackColor = System.Drawing.Color.Transparent;
            this.lblComputers.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblComputers.Location = new System.Drawing.Point(16, 93);
            this.lblComputers.Name = "lblComputers";
            this.lblComputers.Size = new System.Drawing.Size(75, 13);
            this.lblComputers.TabIndex = 16;
            this.lblComputers.Text = "Computers:";
            // 
            // lbPrinters
            // 
            this.lbPrinters.AutoSize = true;
            this.lbPrinters.BackColor = System.Drawing.Color.Transparent;
            this.lbPrinters.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPrinters.Location = new System.Drawing.Point(16, 133);
            this.lbPrinters.Name = "lbPrinters";
            this.lbPrinters.Size = new System.Drawing.Size(56, 13);
            this.lbPrinters.TabIndex = 17;
            this.lbPrinters.Text = "Printers:";
            // 
            // lblNetworkDevices
            // 
            this.lblNetworkDevices.AutoSize = true;
            this.lblNetworkDevices.BackColor = System.Drawing.Color.Transparent;
            this.lblNetworkDevices.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNetworkDevices.Location = new System.Drawing.Point(16, 173);
            this.lblNetworkDevices.Name = "lblNetworkDevices";
            this.lblNetworkDevices.Size = new System.Drawing.Size(108, 13);
            this.lblNetworkDevices.TabIndex = 18;
            this.lblNetworkDevices.Text = "Network Devices:";
            // 
            // lblComputerCount
            // 
            this.lblComputerCount.AutoSize = true;
            this.lblComputerCount.BackColor = System.Drawing.Color.Transparent;
            this.lblComputerCount.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblComputerCount.Location = new System.Drawing.Point(211, 93);
            this.lblComputerCount.Name = "lblComputerCount";
            this.lblComputerCount.Size = new System.Drawing.Size(15, 13);
            this.lblComputerCount.TabIndex = 19;
            this.lblComputerCount.Text = "0";
            // 
            // lblPrinterCount
            // 
            this.lblPrinterCount.AutoSize = true;
            this.lblPrinterCount.BackColor = System.Drawing.Color.Transparent;
            this.lblPrinterCount.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrinterCount.Location = new System.Drawing.Point(211, 133);
            this.lblPrinterCount.Name = "lblPrinterCount";
            this.lblPrinterCount.Size = new System.Drawing.Size(15, 13);
            this.lblPrinterCount.TabIndex = 20;
            this.lblPrinterCount.Text = "0";
            // 
            // lblNetworkDeviceCount
            // 
            this.lblNetworkDeviceCount.AutoSize = true;
            this.lblNetworkDeviceCount.BackColor = System.Drawing.Color.Transparent;
            this.lblNetworkDeviceCount.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNetworkDeviceCount.Location = new System.Drawing.Point(211, 173);
            this.lblNetworkDeviceCount.Name = "lblNetworkDeviceCount";
            this.lblNetworkDeviceCount.Size = new System.Drawing.Size(15, 13);
            this.lblNetworkDeviceCount.TabIndex = 21;
            this.lblNetworkDeviceCount.Text = "0";
            // 
            // pbDiscoveryProgress
            // 
            this.pbDiscoveryProgress.Location = new System.Drawing.Point(19, 320);
            this.pbDiscoveryProgress.Maximum = 252;
            this.pbDiscoveryProgress.Name = "pbDiscoveryProgress";
            this.pbDiscoveryProgress.Size = new System.Drawing.Size(425, 23);
            this.pbDiscoveryProgress.Step = 1;
            this.pbDiscoveryProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbDiscoveryProgress.TabIndex = 23;
            // 
            // gbNetDiscovery
            // 
            this.gbNetDiscovery.Controls.Add(this.lbStatusText);
            this.gbNetDiscovery.Controls.Add(this.lbDiscProg);
            this.gbNetDiscovery.Controls.Add(this.lbStatus);
            this.gbNetDiscovery.Controls.Add(this.lbDiscoverdAssetCount);
            this.gbNetDiscovery.Controls.Add(this.pbDiscoveryProgress);
            this.gbNetDiscovery.Controls.Add(this.lblComputers);
            this.gbNetDiscovery.Controls.Add(this.label2);
            this.gbNetDiscovery.Controls.Add(this.lblNetworkDeviceCount);
            this.gbNetDiscovery.Controls.Add(this.lbPrinters);
            this.gbNetDiscovery.Controls.Add(this.lblPrinterCount);
            this.gbNetDiscovery.Controls.Add(this.lblNetworkDevices);
            this.gbNetDiscovery.Controls.Add(this.lblComputerCount);
            this.gbNetDiscovery.Location = new System.Drawing.Point(6, 106);
            this.gbNetDiscovery.Name = "gbNetDiscovery";
            this.gbNetDiscovery.Size = new System.Drawing.Size(469, 367);
            this.gbNetDiscovery.TabIndex = 24;
            this.gbNetDiscovery.TabStop = false;
            this.gbNetDiscovery.Text = "Network Discovery Results";
            // 
            // lbStatusText
            // 
            this.lbStatusText.AutoSize = true;
            this.lbStatusText.BackColor = System.Drawing.Color.Transparent;
            this.lbStatusText.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatusText.Location = new System.Drawing.Point(211, 36);
            this.lbStatusText.Name = "lbStatusText";
            this.lbStatusText.Size = new System.Drawing.Size(0, 13);
            this.lbStatusText.TabIndex = 27;
            // 
            // lbDiscProg
            // 
            this.lbDiscProg.AutoSize = true;
            this.lbDiscProg.BackColor = System.Drawing.Color.Transparent;
            this.lbDiscProg.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDiscProg.Location = new System.Drawing.Point(16, 300);
            this.lbDiscProg.Name = "lbDiscProg";
            this.lbDiscProg.Size = new System.Drawing.Size(118, 13);
            this.lbDiscProg.TabIndex = 26;
            this.lbDiscProg.Text = "Discovery Progress";
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.BackColor = System.Drawing.Color.Transparent;
            this.lbStatus.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatus.Location = new System.Drawing.Point(16, 36);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(52, 13);
            this.lbStatus.TabIndex = 25;
            this.lbStatus.Text = "Status:";
            // 
            // lbDiscoverdAssetCount
            // 
            this.lbDiscoverdAssetCount.AutoSize = true;
            this.lbDiscoverdAssetCount.BackColor = System.Drawing.Color.Transparent;
            this.lbDiscoverdAssetCount.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDiscoverdAssetCount.Location = new System.Drawing.Point(211, 238);
            this.lbDiscoverdAssetCount.Name = "lbDiscoverdAssetCount";
            this.lbDiscoverdAssetCount.Size = new System.Drawing.Size(15, 13);
            this.lbDiscoverdAssetCount.TabIndex = 24;
            this.lbDiscoverdAssetCount.Text = "0";
            // 
            // headerGroupBox
            // 
            this.headerGroupBox.Controls.Add(this.headerLabel);
            this.headerGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerGroupBox.Location = new System.Drawing.Point(0, 0);
            this.headerGroupBox.Name = "headerGroupBox";
            this.headerGroupBox.Size = new System.Drawing.Size(785, 80);
            this.headerGroupBox.TabIndex = 25;
            this.headerGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // headerLabel
            // 
            appearance2.BackColor = System.Drawing.Color.Transparent;
            appearance2.Image = global::Layton.NetworkDiscovery.Properties.Resources.network_discovery96;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance2.TextHAlignAsString = "Center";
            appearance2.TextVAlignAsString = "Middle";
            this.headerLabel.Appearance = appearance2;
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ImageSize = new System.Drawing.Size(72, 72);
            this.headerLabel.Location = new System.Drawing.Point(6, 3);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.headerLabel.Size = new System.Drawing.Size(378, 72);
            this.headerLabel.TabIndex = 5;
            this.headerLabel.Text = "Network Discovery";
            // 
            // NetworkDiscoveryTabView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.headerGroupBox);
            this.Controls.Add(this.gbNetDiscovery);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "NetworkDiscoveryTabView";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Size = new System.Drawing.Size(785, 572);
            this.gbNetDiscovery.ResumeLayout(false);
            this.gbNetDiscovery.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).EndInit();
            this.headerGroupBox.ResumeLayout(false);
            this.headerGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblComputers;
        private System.Windows.Forms.Label lbPrinters;
        private System.Windows.Forms.Label lblNetworkDevices;
        private System.Windows.Forms.Label lblComputerCount;
        private System.Windows.Forms.Label lblPrinterCount;
        private System.Windows.Forms.Label lblNetworkDeviceCount;
        private System.Windows.Forms.ProgressBar pbDiscoveryProgress;
        private System.Windows.Forms.GroupBox gbNetDiscovery;
        private System.Windows.Forms.Label lbDiscProg;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Label lbDiscoverdAssetCount;
        private System.Windows.Forms.Label lbStatusText;
        private Infragistics.Win.Misc.UltraGroupBox headerGroupBox;
        private Infragistics.Win.Misc.UltraLabel headerLabel;


    }
}
