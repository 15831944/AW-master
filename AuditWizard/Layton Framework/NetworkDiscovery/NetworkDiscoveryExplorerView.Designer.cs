namespace Layton.NetworkDiscovery
{
    partial class NetworkDiscoveryExplorerView
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
            this.label1 = new System.Windows.Forms.Label();
            this.adRadioButton = new System.Windows.Forms.RadioButton();
            this.netbiosRadioButton = new System.Windows.Forms.RadioButton();
            this.tcpipRadioButton = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.discoverButton = new System.Windows.Forms.Button();
            this.snmpRadioButton = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(237, 48);
            this.label1.TabIndex = 3;
            this.label1.Text = "Network Discovery will find computers on your network allowing you to use them th" +
                "roughout the application";
            // 
            // adRadioButton
            // 
            this.adRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.adRadioButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.adRadioButton.Checked = true;
            this.adRadioButton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.adRadioButton.Image = global::Layton.NetworkDiscovery.Properties.Resources.activedirectory_netdisc32;
            this.adRadioButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.adRadioButton.Location = new System.Drawing.Point(3, 160);
            this.adRadioButton.Name = "adRadioButton";
            this.adRadioButton.Size = new System.Drawing.Size(233, 40);
            this.adRadioButton.TabIndex = 0;
            this.adRadioButton.TabStop = true;
            this.adRadioButton.Text = "Import From Active Directory";
            this.adRadioButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.adRadioButton.UseVisualStyleBackColor = false;
            // 
            // netbiosRadioButton
            // 
            this.netbiosRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.netbiosRadioButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.netbiosRadioButton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.netbiosRadioButton.Image = global::Layton.NetworkDiscovery.Properties.Resources.netbios_netdisc32;
            this.netbiosRadioButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.netbiosRadioButton.Location = new System.Drawing.Point(3, 206);
            this.netbiosRadioButton.Name = "netbiosRadioButton";
            this.netbiosRadioButton.Size = new System.Drawing.Size(222, 40);
            this.netbiosRadioButton.TabIndex = 1;
            this.netbiosRadioButton.Text = "Discover via NetBIOS";
            this.netbiosRadioButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.netbiosRadioButton.UseVisualStyleBackColor = false;
            // 
            // tcpipRadioButton
            // 
            this.tcpipRadioButton.BackColor = System.Drawing.Color.Transparent;
            this.tcpipRadioButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tcpipRadioButton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcpipRadioButton.Image = global::Layton.NetworkDiscovery.Properties.Resources.tcp_ip_netdisc32;
            this.tcpipRadioButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tcpipRadioButton.Location = new System.Drawing.Point(3, 252);
            this.tcpipRadioButton.Name = "tcpipRadioButton";
            this.tcpipRadioButton.Size = new System.Drawing.Size(222, 40);
            this.tcpipRadioButton.TabIndex = 2;
            this.tcpipRadioButton.Text = "Find via TCP/IP";
            this.tcpipRadioButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.tcpipRadioButton.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Image = global::Layton.NetworkDiscovery.Properties.Resources.ipaddressrange96;
            this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(237, 102);
            this.label2.TabIndex = 6;
            this.label2.Text = "Network    \r\nDiscovery";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // discoverButton
            // 
            this.discoverButton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.discoverButton.Location = new System.Drawing.Point(149, 339);
            this.discoverButton.Name = "discoverButton";
            this.discoverButton.Size = new System.Drawing.Size(87, 23);
            this.discoverButton.TabIndex = 7;
            this.discoverButton.Text = "Discover";
            this.discoverButton.UseVisualStyleBackColor = true;
            this.discoverButton.Click += new System.EventHandler(this.discoverButton_Click);
            // 
            // snmpRadioButton
            // 
            this.snmpRadioButton.AutoSize = true;
            this.snmpRadioButton.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.snmpRadioButton.Location = new System.Drawing.Point(5, 301);
            this.snmpRadioButton.Name = "snmpRadioButton";
            this.snmpRadioButton.Size = new System.Drawing.Size(152, 17);
            this.snmpRadioButton.TabIndex = 8;
            this.snmpRadioButton.TabStop = true;
            this.snmpRadioButton.Text = "Find devices via SNMP";
            this.snmpRadioButton.UseVisualStyleBackColor = true;
            this.snmpRadioButton.Image = global::Layton.NetworkDiscovery.Properties.Resources.tcp_ip_netdisc32;
            this.snmpRadioButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.snmpRadioButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // NetworkDiscoveryExplorerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Controls.Add(this.snmpRadioButton);
            this.Controls.Add(this.discoverButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tcpipRadioButton);
            this.Controls.Add(this.netbiosRadioButton);
            this.Controls.Add(this.adRadioButton);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "NetworkDiscoveryExplorerView";
            this.Size = new System.Drawing.Size(247, 413);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton adRadioButton;
        private System.Windows.Forms.RadioButton netbiosRadioButton;
        private System.Windows.Forms.RadioButton tcpipRadioButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button discoverButton;
        private System.Windows.Forms.RadioButton snmpRadioButton;





    }
}
