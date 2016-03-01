namespace Layton.AuditWizard.Reports
{
    partial class FormAppByLocation
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
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Locations");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Applications");
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboBoxReportProfiles = new System.Windows.Forms.ComboBox();
			this.textBoxUserDefinedReportName = new System.Windows.Forms.TextBox();
			this.checkBoxSave = new System.Windows.Forms.CheckBox();
			this.treeViewLocations = new System.Windows.Forms.TreeView();
			this.treeViewApplications = new System.Windows.Forms.TreeView();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.checkBoxReportProfile = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.comboBoxReportProfiles);
			this.groupBox1.Controls.Add(this.textBoxUserDefinedReportName);
			this.groupBox1.Controls.Add(this.checkBoxSave);
			this.groupBox1.Controls.Add(this.treeViewLocations);
			this.groupBox1.Controls.Add(this.treeViewApplications);
			this.groupBox1.Location = new System.Drawing.Point(12, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(694, 387);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Select The Applications and Locations To be included in the Report";
			// 
			// comboBoxReportProfiles
			// 
			this.comboBoxReportProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxReportProfiles.FormattingEnabled = true;
			this.comboBoxReportProfiles.Location = new System.Drawing.Point(470, 17);
			this.comboBoxReportProfiles.Name = "comboBoxReportProfiles";
			this.comboBoxReportProfiles.Size = new System.Drawing.Size(205, 21);
			this.comboBoxReportProfiles.TabIndex = 4;
			this.comboBoxReportProfiles.Visible = false;
			this.comboBoxReportProfiles.SelectedIndexChanged += new System.EventHandler(this.comboBoxReportProfiles_SelectedIndexChanged);
			// 
			// textBoxUserDefinedReportName
			// 
			this.textBoxUserDefinedReportName.Enabled = false;
			this.textBoxUserDefinedReportName.Location = new System.Drawing.Point(293, 349);
			this.textBoxUserDefinedReportName.Name = "textBoxUserDefinedReportName";
			this.textBoxUserDefinedReportName.Size = new System.Drawing.Size(254, 20);
			this.textBoxUserDefinedReportName.TabIndex = 3;
			// 
			// checkBoxSave
			// 
			this.checkBoxSave.AutoSize = true;
			this.checkBoxSave.Location = new System.Drawing.Point(18, 352);
			this.checkBoxSave.Name = "checkBoxSave";
			this.checkBoxSave.Size = new System.Drawing.Size(264, 17);
			this.checkBoxSave.TabIndex = 2;
			this.checkBoxSave.Text = "Save this as User-Defined SQL Report with Name ";
			this.checkBoxSave.UseVisualStyleBackColor = true;
			this.checkBoxSave.CheckedChanged += new System.EventHandler(this.checkBoxSave_CheckedChanged);
			// 
			// treeViewLocations
			// 
			this.treeViewLocations.CheckBoxes = true;
			this.treeViewLocations.FullRowSelect = true;
			this.treeViewLocations.Location = new System.Drawing.Point(383, 44);
			this.treeViewLocations.Name = "treeViewLocations";
			treeNode1.Name = "Locations";
			treeNode1.Text = "Locations";
			this.treeViewLocations.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
			this.treeViewLocations.Size = new System.Drawing.Size(292, 292);
			this.treeViewLocations.TabIndex = 1;
			this.treeViewLocations.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewLocations_AfterCheck);
			// 
			// treeViewApplications
			// 
			this.treeViewApplications.CheckBoxes = true;
			this.treeViewApplications.FullRowSelect = true;
			this.treeViewApplications.Location = new System.Drawing.Point(18, 44);
			this.treeViewApplications.Name = "treeViewApplications";
			treeNode2.Name = "Applications";
			treeNode2.Text = "Applications";
			this.treeViewApplications.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
			this.treeViewApplications.Size = new System.Drawing.Size(352, 292);
			this.treeViewApplications.TabIndex = 0;
			this.treeViewApplications.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewApplications_AfterCheck);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(549, 403);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "&OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(631, 403);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// checkBoxReportProfile
			// 
			this.checkBoxReportProfile.AutoSize = true;
			this.checkBoxReportProfile.Location = new System.Drawing.Point(180, 29);
			this.checkBoxReportProfile.Name = "checkBoxReportProfile";
			this.checkBoxReportProfile.Size = new System.Drawing.Size(283, 17);
			this.checkBoxReportProfile.TabIndex = 3;
			this.checkBoxReportProfile.Text = "Get the item selections from the previous report profiles";
			this.checkBoxReportProfile.UseVisualStyleBackColor = true;
			this.checkBoxReportProfile.CheckedChanged += new System.EventHandler(this.checkBoxReportProfile_CheckedChanged);
			// 
			// FormAppByLocation
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(737, 433);
			this.Controls.Add(this.checkBoxReportProfile);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBox1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAppByLocation";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Application By Location";
			this.Load += new System.EventHandler(this.FormAppByLocation_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TreeView treeViewApplications;
        private System.Windows.Forms.TreeView treeViewLocations;
        private System.Windows.Forms.CheckBox checkBoxSave;
        private System.Windows.Forms.TextBox textBoxUserDefinedReportName;
        private System.Windows.Forms.CheckBox checkBoxReportProfile;
        private System.Windows.Forms.ComboBox comboBoxReportProfiles;
    }
}