namespace Layton.AuditWizard.Reports
{
    partial class FormCustomReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCustomReport));
            this.lvSelectedItems = new Infragistics.Win.UltraWinListView.UltraListView();
            this.cbAssetRegister = new System.Windows.Forms.CheckBox();
            this.gb4 = new System.Windows.Forms.GroupBox();
            this.bnRunComplianceReport = new System.Windows.Forms.Button();
            this.bnSaveComplianceReport = new System.Windows.Forms.Button();
            this.lbReportName = new System.Windows.Forms.Label();
            this.tbCustomReportName = new System.Windows.Forms.TextBox();
            this.bnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.selectAuditedDataFieldsControl = new Layton.AuditWizard.Common.SelectAuditedDataFieldsControl();
            ((System.ComponentModel.ISupportInitialize)(this.lvSelectedItems)).BeginInit();
            this.gb4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvSelectedItems
            // 
            this.lvSelectedItems.Location = new System.Drawing.Point(387, 26);
            this.lvSelectedItems.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvSelectedItems.Name = "lvSelectedItems";
            this.lvSelectedItems.Size = new System.Drawing.Size(350, 424);
            this.lvSelectedItems.TabIndex = 2;
            this.lvSelectedItems.Text = "ultraListView1";
            this.lvSelectedItems.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.lvSelectedItems.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
            this.lvSelectedItems.ViewSettingsList.MultiColumn = false;
            // 
            // cbAssetRegister
            // 
            this.cbAssetRegister.AutoSize = true;
            this.cbAssetRegister.Location = new System.Drawing.Point(524, 463);
            this.cbAssetRegister.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbAssetRegister.Name = "cbAssetRegister";
            this.cbAssetRegister.Size = new System.Drawing.Size(213, 17);
            this.cbAssetRegister.TabIndex = 3;
            this.cbAssetRegister.Text = "Display results grouped by asset";
            this.cbAssetRegister.UseVisualStyleBackColor = true;
            // 
            // gb4
            // 
            this.gb4.Controls.Add(this.bnRunComplianceReport);
            this.gb4.Controls.Add(this.bnSaveComplianceReport);
            this.gb4.Controls.Add(this.lbReportName);
            this.gb4.Controls.Add(this.tbCustomReportName);
            this.gb4.Location = new System.Drawing.Point(12, 530);
            this.gb4.Name = "gb4";
            this.gb4.Size = new System.Drawing.Size(754, 68);
            this.gb4.TabIndex = 69;
            this.gb4.TabStop = false;
            this.gb4.Text = "Save and/or run report";
            // 
            // bnRunComplianceReport
            // 
            this.bnRunComplianceReport.Location = new System.Drawing.Point(678, 30);
            this.bnRunComplianceReport.Name = "bnRunComplianceReport";
            this.bnRunComplianceReport.Size = new System.Drawing.Size(70, 23);
            this.bnRunComplianceReport.TabIndex = 6;
            this.bnRunComplianceReport.Text = "Run";
            this.bnRunComplianceReport.UseVisualStyleBackColor = true;
            this.bnRunComplianceReport.Click += new System.EventHandler(this.bnRunCustomReport_Click);
            // 
            // bnSaveComplianceReport
            // 
            this.bnSaveComplianceReport.Location = new System.Drawing.Point(602, 30);
            this.bnSaveComplianceReport.Name = "bnSaveComplianceReport";
            this.bnSaveComplianceReport.Size = new System.Drawing.Size(70, 23);
            this.bnSaveComplianceReport.TabIndex = 5;
            this.bnSaveComplianceReport.Text = "Save";
            this.bnSaveComplianceReport.UseVisualStyleBackColor = true;
            this.bnSaveComplianceReport.Click += new System.EventHandler(this.buttonSaveAs_Click);
            // 
            // lbReportName
            // 
            this.lbReportName.AutoSize = true;
            this.lbReportName.Location = new System.Drawing.Point(10, 35);
            this.lbReportName.Name = "lbReportName";
            this.lbReportName.Size = new System.Drawing.Size(86, 13);
            this.lbReportName.TabIndex = 60;
            this.lbReportName.Text = "Report name:";
            // 
            // tbCustomReportName
            // 
            this.tbCustomReportName.Location = new System.Drawing.Point(102, 31);
            this.tbCustomReportName.Name = "tbCustomReportName";
            this.tbCustomReportName.Size = new System.Drawing.Size(468, 21);
            this.tbCustomReportName.TabIndex = 4;
            this.tbCustomReportName.TextChanged += new System.EventHandler(this.tbCustomReportName_TextChanged);
            // 
            // bnCancel
            // 
            this.bnCancel.Location = new System.Drawing.Point(690, 608);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(70, 23);
            this.bnCancel.TabIndex = 7;
            this.bnCancel.Text = "Close";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.selectAuditedDataFieldsControl);
            this.groupBox1.Controls.Add(this.lvSelectedItems);
            this.groupBox1.Controls.Add(this.cbAssetRegister);
            this.groupBox1.Location = new System.Drawing.Point(12, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(754, 486);
            this.groupBox1.TabIndex = 70;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Define custom report fields";
            // 
            // selectAuditedDataFieldsControl
            // 
            this.selectAuditedDataFieldsControl.AlertableItemsOnly = true;
            this.selectAuditedDataFieldsControl.AllowExpandApplications = true;
            this.selectAuditedDataFieldsControl.AllowInternetSelection = false;
            this.selectAuditedDataFieldsControl.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.selectAuditedDataFieldsControl.Location = new System.Drawing.Point(17, 26);
            this.selectAuditedDataFieldsControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.selectAuditedDataFieldsControl.Name = "selectAuditedDataFieldsControl";
            this.selectAuditedDataFieldsControl.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBoxTriState;
            this.selectAuditedDataFieldsControl.ReportSpecificItemsShow = true;
            this.selectAuditedDataFieldsControl.SelectedItems = ((System.Collections.Generic.List<string>)(resources.GetObject("selectAuditedDataFieldsControl.SelectedItems")));
            this.selectAuditedDataFieldsControl.Size = new System.Drawing.Size(350, 424);
            this.selectAuditedDataFieldsControl.TabIndex = 1;
            this.selectAuditedDataFieldsControl.CheckChanged += new Layton.AuditWizard.Common.CheckChangedEventHandler(this.selectAuditedDataFieldsControl_CheckChanged);
            // 
            // FormCustomReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(778, 639);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gb4);
            this.Controls.Add(this.bnCancel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCustomReport";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create new custom report";
            this.Load += new System.EventHandler(this.FormCustomReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lvSelectedItems)).EndInit();
            this.gb4.ResumeLayout(false);
            this.gb4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinListView.UltraListView lvSelectedItems;
        private Layton.AuditWizard.Common.SelectAuditedDataFieldsControl selectAuditedDataFieldsControl;
        private System.Windows.Forms.CheckBox cbAssetRegister;
        private System.Windows.Forms.GroupBox gb4;
        private System.Windows.Forms.Button bnRunComplianceReport;
        private System.Windows.Forms.Button bnSaveComplianceReport;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Label lbReportName;
        private System.Windows.Forms.TextBox tbCustomReportName;
        private System.Windows.Forms.GroupBox groupBox1;

    }
}