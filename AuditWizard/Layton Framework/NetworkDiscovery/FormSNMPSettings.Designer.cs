namespace Layton.NetworkDiscovery
{
    partial class FormSNMPSettings
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
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn2 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 0");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            this.tcpipListView = new Infragistics.Win.UltraWinListView.UltraListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bnEditIP = new System.Windows.Forms.Button();
            this.bnRemoveIP = new System.Windows.Forms.Button();
            this.bnAddIP = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbReadStrings = new System.Windows.Forms.ListBox();
            this.bnRemoveRead = new System.Windows.Forms.Button();
            this.bnAddRead = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbReadString = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tcpipListView)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcpipListView
            // 
            appearance6.BackColor = System.Drawing.Color.White;
            this.tcpipListView.Appearance = appearance6;
            this.tcpipListView.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            this.tcpipListView.ItemSettings.DefaultImage = global::Layton.NetworkDiscovery.Properties.Resources.ipaddressrange16;
            this.tcpipListView.Location = new System.Drawing.Point(14, 91);
            this.tcpipListView.MainColumn.Text = "Start IP Address";
            this.tcpipListView.MainColumn.Width = 189;
            this.tcpipListView.Name = "tcpipListView";
            this.tcpipListView.Size = new System.Drawing.Size(380, 158);
            ultraListViewSubItemColumn2.Key = "SubItemColumn 0";
            ultraListViewSubItemColumn2.Text = "End IP Address";
            ultraListViewSubItemColumn2.Width = 189;
            this.tcpipListView.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn2});
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
            this.tcpipListView.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.tcpipListView_SelectChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.bnEditIP);
            this.groupBox1.Controls.Add(this.bnRemoveIP);
            this.groupBox1.Controls.Add(this.bnAddIP);
            this.groupBox1.Controls.Add(this.tcpipListView);
            this.groupBox1.Location = new System.Drawing.Point(15, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(413, 300);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IP Address Range";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(286, 39);
            this.label1.TabIndex = 12;
            this.label1.Text = "Enter a range of IP addresses to scan. \r\n\r\nUncheck a range to exclude it from fut" +
                "ure scans.";
            // 
            // bnEditIP
            // 
            this.bnEditIP.Enabled = false;
            this.bnEditIP.Location = new System.Drawing.Point(319, 264);
            this.bnEditIP.Name = "bnEditIP";
            this.bnEditIP.Size = new System.Drawing.Size(75, 23);
            this.bnEditIP.TabIndex = 8;
            this.bnEditIP.Text = "Edit";
            this.bnEditIP.UseVisualStyleBackColor = true;
            this.bnEditIP.Click += new System.EventHandler(this.bnEditIP_Click);
            // 
            // bnRemoveIP
            // 
            this.bnRemoveIP.Location = new System.Drawing.Point(238, 264);
            this.bnRemoveIP.Name = "bnRemoveIP";
            this.bnRemoveIP.Size = new System.Drawing.Size(75, 23);
            this.bnRemoveIP.TabIndex = 9;
            this.bnRemoveIP.Text = "Remove";
            this.bnRemoveIP.UseVisualStyleBackColor = true;
            this.bnRemoveIP.Click += new System.EventHandler(this.bnRemoveIP_Click);
            // 
            // bnAddIP
            // 
            this.bnAddIP.Location = new System.Drawing.Point(157, 264);
            this.bnAddIP.Name = "bnAddIP";
            this.bnAddIP.Size = new System.Drawing.Size(75, 23);
            this.bnAddIP.TabIndex = 10;
            this.bnAddIP.Text = "Add";
            this.bnAddIP.UseVisualStyleBackColor = true;
            this.bnAddIP.Click += new System.EventHandler(this.bnAddIP_Click);
            // 
            // bnOK
            // 
            this.bnOK.Location = new System.Drawing.Point(272, 594);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(75, 23);
            this.bnOK.TabIndex = 5;
            this.bnOK.Text = "OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Location = new System.Drawing.Point(353, 594);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 6;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.label2);
            this.ultraGroupBox2.Controls.Add(this.label4);
            this.ultraGroupBox2.Controls.Add(this.lbReadStrings);
            this.ultraGroupBox2.Controls.Add(this.bnRemoveRead);
            this.ultraGroupBox2.Controls.Add(this.bnAddRead);
            this.ultraGroupBox2.Controls.Add(this.label3);
            this.ultraGroupBox2.Controls.Add(this.tbReadString);
            this.ultraGroupBox2.Location = new System.Drawing.Point(15, 318);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(413, 270);
            this.ultraGroupBox2.TabIndex = 3;
            this.ultraGroupBox2.Text = "Read Community Strings";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 199);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "New string";
            // 
            // lbReadStrings
            // 
            this.lbReadStrings.FormattingEnabled = true;
            this.lbReadStrings.Location = new System.Drawing.Point(14, 60);
            this.lbReadStrings.Name = "lbReadStrings";
            this.lbReadStrings.Size = new System.Drawing.Size(379, 95);
            this.lbReadStrings.Sorted = true;
            this.lbReadStrings.TabIndex = 7;
            this.lbReadStrings.SelectedIndexChanged += new System.EventHandler(this.lbReadStrings_SelectedIndexChanged);
            // 
            // bnRemoveRead
            // 
            this.bnRemoveRead.Enabled = false;
            this.bnRemoveRead.Location = new System.Drawing.Point(94, 233);
            this.bnRemoveRead.Name = "bnRemoveRead";
            this.bnRemoveRead.Size = new System.Drawing.Size(75, 23);
            this.bnRemoveRead.TabIndex = 6;
            this.bnRemoveRead.Text = "Remove";
            this.bnRemoveRead.UseVisualStyleBackColor = true;
            this.bnRemoveRead.Click += new System.EventHandler(this.bnRemoveRead_Click);
            // 
            // bnAddRead
            // 
            this.bnAddRead.Location = new System.Drawing.Point(13, 233);
            this.bnAddRead.Name = "bnAddRead";
            this.bnAddRead.Size = new System.Drawing.Size(75, 23);
            this.bnAddRead.TabIndex = 4;
            this.bnAddRead.Text = "Add";
            this.bnAddRead.UseVisualStyleBackColor = true;
            this.bnAddRead.Click += new System.EventHandler(this.bnAddRead_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 170);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(217, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Enter SNMP read community strings:";
            // 
            // tbReadString
            // 
            this.tbReadString.Location = new System.Drawing.Point(99, 196);
            this.tbReadString.Name = "tbReadString";
            this.tbReadString.Size = new System.Drawing.Size(294, 21);
            this.tbReadString.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(231, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Existing SNMP read community strings:";
            // 
            // FormSNMPSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(447, 629);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ultraGroupBox2);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSNMPSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SNMP Settings";
            this.Load += new System.EventHandler(this.Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tcpipListView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinListView.UltraListView tcpipListView;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bnOK;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnEditIP;
        private System.Windows.Forms.Button bnRemoveIP;
        private System.Windows.Forms.Button bnAddIP;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lbReadStrings;
        private System.Windows.Forms.Button bnRemoveRead;
        private System.Windows.Forms.Button bnAddRead;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbReadString;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}