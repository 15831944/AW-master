namespace Layton.NetworkDiscovery
{
    partial class FormTcpipSettings
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
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 0");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bnEditIP = new System.Windows.Forms.Button();
            this.bnRemoveIP = new System.Windows.Forms.Button();
            this.bnAddIP = new System.Windows.Forms.Button();
            this.tcpipListView = new Infragistics.Win.UltraWinListView.UltraListView();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcpipListView)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.bnEditIP);
            this.groupBox1.Controls.Add(this.bnRemoveIP);
            this.groupBox1.Controls.Add(this.bnAddIP);
            this.groupBox1.Controls.Add(this.tcpipListView);
            this.groupBox1.Location = new System.Drawing.Point(12, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(401, 303);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IP Address Range";
            // 
            // bnEditIP
            // 
            this.bnEditIP.Enabled = false;
            this.bnEditIP.Location = new System.Drawing.Point(320, 274);
            this.bnEditIP.Name = "bnEditIP";
            this.bnEditIP.Size = new System.Drawing.Size(75, 23);
            this.bnEditIP.TabIndex = 8;
            this.bnEditIP.Text = "Edit";
            this.bnEditIP.UseVisualStyleBackColor = true;
            this.bnEditIP.Click += new System.EventHandler(this.bnEditIP_Click);
            // 
            // bnRemoveIP
            // 
            this.bnRemoveIP.Location = new System.Drawing.Point(239, 274);
            this.bnRemoveIP.Name = "bnRemoveIP";
            this.bnRemoveIP.Size = new System.Drawing.Size(75, 23);
            this.bnRemoveIP.TabIndex = 9;
            this.bnRemoveIP.Text = "Remove";
            this.bnRemoveIP.UseVisualStyleBackColor = true;
            this.bnRemoveIP.Click += new System.EventHandler(this.bnRemoveIP_Click);
            // 
            // bnAddIP
            // 
            this.bnAddIP.Location = new System.Drawing.Point(158, 274);
            this.bnAddIP.Name = "bnAddIP";
            this.bnAddIP.Size = new System.Drawing.Size(75, 23);
            this.bnAddIP.TabIndex = 10;
            this.bnAddIP.Text = "Add";
            this.bnAddIP.UseVisualStyleBackColor = true;
            this.bnAddIP.Click += new System.EventHandler(this.bnAddIP_Click);
            // 
            // tcpipListView
            // 
            appearance6.BackColor = System.Drawing.Color.White;
            this.tcpipListView.Appearance = appearance6;
            this.tcpipListView.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            this.tcpipListView.ItemSettings.DefaultImage = global::Layton.NetworkDiscovery.Properties.Resources.ipaddressrange16;
            this.tcpipListView.Location = new System.Drawing.Point(12, 88);
            this.tcpipListView.MainColumn.Text = "Start IP Address";
            this.tcpipListView.MainColumn.Width = 189;
            this.tcpipListView.Name = "tcpipListView";
            this.tcpipListView.Size = new System.Drawing.Size(380, 180);
            ultraListViewSubItemColumn1.Key = "SubItemColumn 0";
            ultraListViewSubItemColumn1.Text = "End IP Address";
            ultraListViewSubItemColumn1.Width = 189;
            this.tcpipListView.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1});
            this.tcpipListView.TabIndex = 0;
            this.tcpipListView.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.tcpipListView.ViewSettingsDetails.AllowColumnMoving = false;
            this.tcpipListView.ViewSettingsDetails.AllowColumnSizing = false;
            this.tcpipListView.ViewSettingsDetails.AllowColumnSorting = false;
            this.tcpipListView.ViewSettingsDetails.CheckBoxStyle = Infragistics.Win.UltraWinListView.CheckBoxStyle.CheckBox;
            appearance4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(235)))));
            appearance4.BackColor2 = System.Drawing.Color.White;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.GlassTop20;
            this.tcpipListView.ViewSettingsDetails.ColumnHeaderAppearance = appearance4;
            this.tcpipListView.ViewSettingsDetails.ColumnHeaderStyle = Infragistics.Win.HeaderStyle.Standard;
            this.tcpipListView.ViewSettingsDetails.ColumnsShowSortIndicators = false;
            this.tcpipListView.ViewSettingsDetails.FullRowSelect = true;
            this.tcpipListView.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.TcpipListViewSelectChanged);
            // 
            // bnCancel
            // 
            this.bnCancel.Location = new System.Drawing.Point(338, 341);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 9;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // bnOK
            // 
            this.bnOK.Location = new System.Drawing.Point(257, 341);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(75, 23);
            this.bnOK.TabIndex = 8;
            this.bnOK.Text = "OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(286, 39);
            this.label1.TabIndex = 11;
            this.label1.Text = "Enter a range of IP addresses to scan. \r\n\r\nUncheck a range to exclude it from fut" +
                "ure scans.";
            // 
            // FormTcpipSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(425, 376);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTcpipSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TCP / IP Settings";
            this.Load += new System.EventHandler(this.Form_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcpipListView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bnEditIP;
        private System.Windows.Forms.Button bnRemoveIP;
        private System.Windows.Forms.Button bnAddIP;
        private Infragistics.Win.UltraWinListView.UltraListView tcpipListView;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.Label label1;
    }
}