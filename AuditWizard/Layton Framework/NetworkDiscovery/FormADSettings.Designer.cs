namespace Layton.NetworkDiscovery
{
    partial class FormADSettings
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ugbCustomLocations = new Infragistics.Win.Misc.UltraGroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbReadStrings = new System.Windows.Forms.ListBox();
            this.bnRemoveRead = new System.Windows.Forms.Button();
            this.bnAddRead = new System.Windows.Forms.Button();
            this.tbReadString = new System.Windows.Forms.TextBox();
            this.rbCustomLocation = new System.Windows.Forms.RadioButton();
            this.rbRootLocation = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnApply = new System.Windows.Forms.Button();
            this.bnOK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugbCustomLocations)).BeginInit();
            this.ugbCustomLocations.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ugbCustomLocations);
            this.groupBox1.Controls.Add(this.rbCustomLocation);
            this.groupBox1.Controls.Add(this.rbRootLocation);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(491, 452);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Active Directory Settings";
            // 
            // ugbCustomLocations
            // 
            this.ugbCustomLocations.Controls.Add(this.label2);
            this.ugbCustomLocations.Controls.Add(this.label3);
            this.ugbCustomLocations.Controls.Add(this.label4);
            this.ugbCustomLocations.Controls.Add(this.lbReadStrings);
            this.ugbCustomLocations.Controls.Add(this.bnRemoveRead);
            this.ugbCustomLocations.Controls.Add(this.bnAddRead);
            this.ugbCustomLocations.Controls.Add(this.tbReadString);
            this.ugbCustomLocations.Enabled = false;
            this.ugbCustomLocations.Location = new System.Drawing.Point(27, 135);
            this.ugbCustomLocations.Name = "ugbCustomLocations";
            this.ugbCustomLocations.Size = new System.Drawing.Size(439, 311);
            this.ugbCustomLocations.TabIndex = 15;
            this.ugbCustomLocations.Text = "Custom Location Strings";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 243);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "New location";
            // 
            // lbReadStrings
            // 
            this.lbReadStrings.BackColor = System.Drawing.SystemColors.Window;
            this.lbReadStrings.FormattingEnabled = true;
            this.lbReadStrings.HorizontalScrollbar = true;
            this.lbReadStrings.Location = new System.Drawing.Point(9, 52);
            this.lbReadStrings.Name = "lbReadStrings";
            this.lbReadStrings.Size = new System.Drawing.Size(411, 147);
            this.lbReadStrings.Sorted = true;
            this.lbReadStrings.TabIndex = 7;
            this.lbReadStrings.SelectedIndexChanged += new System.EventHandler(this.lbReadStrings_SelectedIndexChanged);
            // 
            // bnRemoveRead
            // 
            this.bnRemoveRead.Enabled = false;
            this.bnRemoveRead.Location = new System.Drawing.Point(102, 276);
            this.bnRemoveRead.Name = "bnRemoveRead";
            this.bnRemoveRead.Size = new System.Drawing.Size(75, 23);
            this.bnRemoveRead.TabIndex = 6;
            this.bnRemoveRead.Text = "Remove";
            this.bnRemoveRead.UseVisualStyleBackColor = true;
            this.bnRemoveRead.Click += new System.EventHandler(this.bnRemoveRead_Click);
            // 
            // bnAddRead
            // 
            this.bnAddRead.Location = new System.Drawing.Point(21, 276);
            this.bnAddRead.Name = "bnAddRead";
            this.bnAddRead.Size = new System.Drawing.Size(75, 23);
            this.bnAddRead.TabIndex = 4;
            this.bnAddRead.Text = "Add";
            this.bnAddRead.UseVisualStyleBackColor = true;
            this.bnAddRead.Click += new System.EventHandler(this.bnAddRead_Click);
            // 
            // tbReadString
            // 
            this.tbReadString.Location = new System.Drawing.Point(102, 240);
            this.tbReadString.Name = "tbReadString";
            this.tbReadString.Size = new System.Drawing.Size(322, 21);
            this.tbReadString.TabIndex = 1;
            // 
            // rbCustomLocation
            // 
            this.rbCustomLocation.AutoSize = true;
            this.rbCustomLocation.Location = new System.Drawing.Point(10, 100);
            this.rbCustomLocation.Name = "rbCustomLocation";
            this.rbCustomLocation.Size = new System.Drawing.Size(120, 17);
            this.rbCustomLocation.TabIndex = 2;
            this.rbCustomLocation.Text = "Custom Location";
            this.rbCustomLocation.UseVisualStyleBackColor = true;
            this.rbCustomLocation.CheckedChanged += new System.EventHandler(this.rbCustomLocation_CheckedChanged);
            // 
            // rbRootLocation
            // 
            this.rbRootLocation.AutoSize = true;
            this.rbRootLocation.Checked = true;
            this.rbRootLocation.Location = new System.Drawing.Point(10, 68);
            this.rbRootLocation.Name = "rbRootLocation";
            this.rbRootLocation.Size = new System.Drawing.Size(102, 17);
            this.rbRootLocation.TabIndex = 1;
            this.rbRootLocation.TabStop = true;
            this.rbRootLocation.Text = "Root Location";
            this.rbRootLocation.UseVisualStyleBackColor = true;
            this.rbRootLocation.CheckedChanged += new System.EventHandler(this.rbRootLocation_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(246, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select type of Active Directory discovery:";
            // 
            // bnCancel
            // 
            this.bnCancel.Location = new System.Drawing.Point(347, 483);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 13;
            this.bnCancel.Text = "Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // bnApply
            // 
            this.bnApply.Enabled = false;
            this.bnApply.Location = new System.Drawing.Point(428, 483);
            this.bnApply.Name = "bnApply";
            this.bnApply.Size = new System.Drawing.Size(75, 23);
            this.bnApply.TabIndex = 14;
            this.bnApply.Text = "Apply";
            this.bnApply.UseVisualStyleBackColor = true;
            this.bnApply.Click += new System.EventHandler(this.bnApply_Click);
            // 
            // bnOK
            // 
            this.bnOK.Location = new System.Drawing.Point(266, 483);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(75, 23);
            this.bnOK.TabIndex = 12;
            this.bnOK.Text = "OK";
            this.bnOK.UseVisualStyleBackColor = true;
            this.bnOK.Click += new System.EventHandler(this.bnOK_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 215);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Enter custom location string:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(191, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Current custom location strings:";
            // 
            // FormADSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(515, 518);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnApply);
            this.Controls.Add(this.bnOK);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormADSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Active Directory Settings";
            this.Load += new System.EventHandler(this.Form_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugbCustomLocations)).EndInit();
            this.ugbCustomLocations.ResumeLayout(false);
            this.ugbCustomLocations.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbCustomLocation;
        private System.Windows.Forms.RadioButton rbRootLocation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnApply;
        private System.Windows.Forms.Button bnOK;
        private Infragistics.Win.Misc.UltraGroupBox ugbCustomLocations;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lbReadStrings;
        private System.Windows.Forms.Button bnRemoveRead;
        private System.Windows.Forms.Button bnAddRead;
        private System.Windows.Forms.TextBox tbReadString;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}