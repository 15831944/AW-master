namespace Layton.AuditWizard.Reports
{
    partial class FormComplianceReport
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn1 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 0");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn2 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 1");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn ultraListViewSubItemColumn3 = new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn("SubItemColumn 2");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormComplianceReport));
            this.btnDeleteTrigger = new System.Windows.Forms.Button();
            this.lvTriggers = new Infragistics.Win.UltraWinListView.UltraListView();
            this.bnRunComplianceReport = new System.Windows.Forms.Button();
            this.bnSaveComplianceReport = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.lbReportName = new System.Windows.Forms.Label();
            this.tbReportName = new System.Windows.Forms.TextBox();
            this.gb1 = new System.Windows.Forms.GroupBox();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.lbBooleanOperator = new Infragistics.Win.Misc.UltraLabel();
            this.btnEditComplianceField = new System.Windows.Forms.Button();
            this.cbBooleanValue = new System.Windows.Forms.ComboBox();
            this.lbAddField = new Infragistics.Win.Misc.UltraLabel();
            this.lblUnits = new System.Windows.Forms.Label();
            this.lbComplianceField = new Infragistics.Win.Misc.UltraLabel();
            this.tbSelectedComplianceField = new System.Windows.Forms.TextBox();
            this.cbConditions = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.lbConditions = new Infragistics.Win.Misc.UltraLabel();
            this.cbValuePicker = new System.Windows.Forms.ComboBox();
            this.lbValue = new Infragistics.Win.Misc.UltraLabel();
            this.bnAddTrigger = new System.Windows.Forms.Button();
            this.selectedFields = new Layton.AuditWizard.Common.SelectAuditedDataFieldsControl();
            ((System.ComponentModel.ISupportInitialize)(this.lvTriggers)).BeginInit();
            this.gb1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbConditions)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDeleteTrigger
            // 
            this.btnDeleteTrigger.Location = new System.Drawing.Point(805, 548);
            this.btnDeleteTrigger.Name = "btnDeleteTrigger";
            this.btnDeleteTrigger.Size = new System.Drawing.Size(163, 23);
            this.btnDeleteTrigger.TabIndex = 7;
            this.btnDeleteTrigger.Text = "Delete Compliance Field";
            this.btnDeleteTrigger.UseVisualStyleBackColor = true;
            this.btnDeleteTrigger.Click += new System.EventHandler(this.btnDeleteTrigger_Click);
            // 
            // lvTriggers
            // 
            this.lvTriggers.ItemSettings.AllowEdit = Infragistics.Win.DefaultableBoolean.False;
            this.lvTriggers.Location = new System.Drawing.Point(13, 387);
            appearance1.TextHAlignAsString = "Center";
            this.lvTriggers.MainColumn.HeaderAppearance = appearance1;
            this.lvTriggers.MainColumn.Text = "Compliance Field";
            this.lvTriggers.MainColumn.VisiblePositionInDetailsView = 1;
            this.lvTriggers.MainColumn.Width = 400;
            this.lvTriggers.Name = "lvTriggers";
            this.lvTriggers.Size = new System.Drawing.Size(955, 155);
            appearance2.TextHAlignAsString = "Center";
            ultraListViewSubItemColumn1.HeaderAppearance = appearance2;
            ultraListViewSubItemColumn1.Key = "SubItemColumn 0";
            ultraListViewSubItemColumn1.Text = "Condition";
            ultraListViewSubItemColumn1.VisiblePositionInDetailsView = 2;
            ultraListViewSubItemColumn1.Width = 122;
            appearance3.TextHAlignAsString = "Center";
            ultraListViewSubItemColumn2.HeaderAppearance = appearance3;
            ultraListViewSubItemColumn2.Key = "SubItemColumn 1";
            ultraListViewSubItemColumn2.Text = "Value";
            ultraListViewSubItemColumn2.VisiblePositionInDetailsView = 3;
            ultraListViewSubItemColumn2.Width = 375;
            appearance4.TextHAlignAsString = "Center";
            ultraListViewSubItemColumn3.HeaderAppearance = appearance4;
            ultraListViewSubItemColumn3.Key = "SubItemColumn 2";
            ultraListViewSubItemColumn3.Text = "And/Or";
            ultraListViewSubItemColumn3.VisiblePositionInDetailsView = 0;
            ultraListViewSubItemColumn3.Width = 55;
            this.lvTriggers.SubItemColumns.AddRange(new Infragistics.Win.UltraWinListView.UltraListViewSubItemColumn[] {
            ultraListViewSubItemColumn1,
            ultraListViewSubItemColumn2,
            ultraListViewSubItemColumn3});
            this.lvTriggers.TabIndex = 6;
            this.lvTriggers.Text = "ultraListView2";
            this.lvTriggers.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.Details;
            this.lvTriggers.ItemSelectionChanged += new Infragistics.Win.UltraWinListView.ItemSelectionChangedEventHandler(this.lvTriggers_ItemSelectionChanged);
            // 
            // bnRunComplianceReport
            // 
            this.bnRunComplianceReport.Location = new System.Drawing.Point(850, 609);
            this.bnRunComplianceReport.Name = "bnRunComplianceReport";
            this.bnRunComplianceReport.Size = new System.Drawing.Size(70, 23);
            this.bnRunComplianceReport.TabIndex = 10;
            this.bnRunComplianceReport.Text = "Run";
            this.bnRunComplianceReport.UseVisualStyleBackColor = true;
            this.bnRunComplianceReport.Click += new System.EventHandler(this.bnRunComplianceReport_Click);
            // 
            // bnSaveComplianceReport
            // 
            this.bnSaveComplianceReport.Location = new System.Drawing.Point(775, 609);
            this.bnSaveComplianceReport.Name = "bnSaveComplianceReport";
            this.bnSaveComplianceReport.Size = new System.Drawing.Size(70, 23);
            this.bnSaveComplianceReport.TabIndex = 9;
            this.bnSaveComplianceReport.Text = "Save";
            this.bnSaveComplianceReport.UseVisualStyleBackColor = true;
            this.bnSaveComplianceReport.Click += new System.EventHandler(this.bnSaveComplianceReport_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Location = new System.Drawing.Point(925, 609);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(70, 23);
            this.bnCancel.TabIndex = 11;
            this.bnCancel.Text = "Close";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // lbReportName
            // 
            this.lbReportName.AutoSize = true;
            this.lbReportName.Location = new System.Drawing.Point(429, 613);
            this.lbReportName.Name = "lbReportName";
            this.lbReportName.Size = new System.Drawing.Size(86, 13);
            this.lbReportName.TabIndex = 60;
            this.lbReportName.Text = "Report name:";
            // 
            // tbReportName
            // 
            this.tbReportName.Location = new System.Drawing.Point(521, 610);
            this.tbReportName.Name = "tbReportName";
            this.tbReportName.Size = new System.Drawing.Size(248, 21);
            this.tbReportName.TabIndex = 8;
            this.tbReportName.TextChanged += new System.EventHandler(this.tbReportName_TextChanged);
            // 
            // gb1
            // 
            this.gb1.Controls.Add(this.btnUp);
            this.gb1.Controls.Add(this.btnDown);
            this.gb1.Controls.Add(this.lbBooleanOperator);
            this.gb1.Controls.Add(this.btnEditComplianceField);
            this.gb1.Controls.Add(this.cbBooleanValue);
            this.gb1.Controls.Add(this.btnDeleteTrigger);
            this.gb1.Controls.Add(this.lvTriggers);
            this.gb1.Controls.Add(this.lbAddField);
            this.gb1.Controls.Add(this.lblUnits);
            this.gb1.Controls.Add(this.lbComplianceField);
            this.gb1.Controls.Add(this.tbSelectedComplianceField);
            this.gb1.Controls.Add(this.cbConditions);
            this.gb1.Controls.Add(this.lbConditions);
            this.gb1.Controls.Add(this.cbValuePicker);
            this.gb1.Controls.Add(this.lbValue);
            this.gb1.Controls.Add(this.bnAddTrigger);
            this.gb1.Controls.Add(this.selectedFields);
            this.gb1.Location = new System.Drawing.Point(14, 12);
            this.gb1.Name = "gb1";
            this.gb1.Size = new System.Drawing.Size(981, 577);
            this.gb1.TabIndex = 66;
            this.gb1.TabStop = false;
            this.gb1.Text = "Define compliance fields";
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(13, 548);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(86, 23);
            this.btnUp.TabIndex = 79;
            this.btnUp.Text = "Move Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(105, 548);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(86, 23);
            this.btnDown.TabIndex = 67;
            this.btnDown.Text = "Move Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // lbBooleanOperator
            // 
            appearance12.BackColor = System.Drawing.Color.Transparent;
            this.lbBooleanOperator.Appearance = appearance12;
            this.lbBooleanOperator.Location = new System.Drawing.Point(418, 173);
            this.lbBooleanOperator.Name = "lbBooleanOperator";
            this.lbBooleanOperator.Size = new System.Drawing.Size(102, 18);
            this.lbBooleanOperator.TabIndex = 78;
            this.lbBooleanOperator.Text = "Boolean operator";
            // 
            // btnEditComplianceField
            // 
            this.btnEditComplianceField.Location = new System.Drawing.Point(636, 548);
            this.btnEditComplianceField.Name = "btnEditComplianceField";
            this.btnEditComplianceField.Size = new System.Drawing.Size(163, 23);
            this.btnEditComplianceField.TabIndex = 8;
            this.btnEditComplianceField.Text = "Edit Compliance Field";
            this.btnEditComplianceField.UseVisualStyleBackColor = true;
            this.btnEditComplianceField.Click += new System.EventHandler(this.btnEditComplianceField_Click);
            // 
            // cbBooleanValue
            // 
            this.cbBooleanValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBooleanValue.Enabled = false;
            this.cbBooleanValue.FormattingEnabled = true;
            this.cbBooleanValue.Items.AddRange(new object[] {
            "And",
            "Or"});
            this.cbBooleanValue.Location = new System.Drawing.Point(546, 170);
            this.cbBooleanValue.Name = "cbBooleanValue";
            this.cbBooleanValue.Size = new System.Drawing.Size(53, 21);
            this.cbBooleanValue.TabIndex = 77;
            // 
            // lbAddField
            // 
            appearance5.BackColor = System.Drawing.Color.Transparent;
            this.lbAddField.Appearance = appearance5;
            this.lbAddField.Location = new System.Drawing.Point(418, 220);
            this.lbAddField.Name = "lbAddField";
            this.lbAddField.Size = new System.Drawing.Size(102, 18);
            this.lbAddField.TabIndex = 76;
            this.lbAddField.Text = "Add field";
            // 
            // lblUnits
            // 
            this.lblUnits.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblUnits.AutoSize = true;
            this.lblUnits.Location = new System.Drawing.Point(944, 126);
            this.lblUnits.Name = "lblUnits";
            this.lblUnits.Size = new System.Drawing.Size(24, 13);
            this.lblUnits.TabIndex = 75;
            this.lblUnits.Text = "MB";
            this.lblUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbComplianceField
            // 
            appearance11.BackColor = System.Drawing.Color.Transparent;
            this.lbComplianceField.Appearance = appearance11;
            this.lbComplianceField.Location = new System.Drawing.Point(418, 32);
            this.lbComplianceField.Name = "lbComplianceField";
            this.lbComplianceField.Size = new System.Drawing.Size(102, 18);
            this.lbComplianceField.TabIndex = 74;
            this.lbComplianceField.Text = "Compliance field";
            // 
            // tbSelectedComplianceField
            // 
            this.tbSelectedComplianceField.Location = new System.Drawing.Point(546, 29);
            this.tbSelectedComplianceField.Name = "tbSelectedComplianceField";
            this.tbSelectedComplianceField.ReadOnly = true;
            this.tbSelectedComplianceField.Size = new System.Drawing.Size(422, 21);
            this.tbSelectedComplianceField.TabIndex = 68;
            // 
            // cbConditions
            // 
            this.cbConditions.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cbConditions.Location = new System.Drawing.Point(546, 75);
            this.cbConditions.Name = "cbConditions";
            this.cbConditions.Size = new System.Drawing.Size(150, 22);
            this.cbConditions.TabIndex = 69;
            // 
            // lbConditions
            // 
            appearance10.BackColor = System.Drawing.Color.Transparent;
            this.lbConditions.Appearance = appearance10;
            this.lbConditions.Location = new System.Drawing.Point(418, 79);
            this.lbConditions.Name = "lbConditions";
            this.lbConditions.Size = new System.Drawing.Size(108, 18);
            this.lbConditions.TabIndex = 72;
            this.lbConditions.Text = "Select a condition";
            // 
            // cbValuePicker
            // 
            this.cbValuePicker.FormattingEnabled = true;
            this.cbValuePicker.Location = new System.Drawing.Point(546, 123);
            this.cbValuePicker.Name = "cbValuePicker";
            this.cbValuePicker.Size = new System.Drawing.Size(369, 21);
            this.cbValuePicker.TabIndex = 70;
            // 
            // lbValue
            // 
            appearance9.BackColor = System.Drawing.Color.Transparent;
            this.lbValue.Appearance = appearance9;
            this.lbValue.Location = new System.Drawing.Point(418, 126);
            this.lbValue.Name = "lbValue";
            this.lbValue.Size = new System.Drawing.Size(122, 18);
            this.lbValue.TabIndex = 73;
            this.lbValue.Text = "Enter/Select a value";
            // 
            // bnAddTrigger
            // 
            this.bnAddTrigger.Location = new System.Drawing.Point(546, 215);
            this.bnAddTrigger.Name = "bnAddTrigger";
            this.bnAddTrigger.Size = new System.Drawing.Size(164, 23);
            this.bnAddTrigger.TabIndex = 71;
            this.bnAddTrigger.Text = "Add Compliance Field";
            this.bnAddTrigger.UseVisualStyleBackColor = true;
            this.bnAddTrigger.Click += new System.EventHandler(this.bnAddTrigger_Click);
            // 
            // selectedFields
            // 
            this.selectedFields.AlertableItemsOnly = true;
            this.selectedFields.AllowExpandApplications = true;
            this.selectedFields.AllowInternetSelection = false;
            this.selectedFields.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.selectedFields.Location = new System.Drawing.Point(13, 29);
            this.selectedFields.Name = "selectedFields";
            this.selectedFields.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.Standard;
            this.selectedFields.ReportSpecificItemsShow = true;
            this.selectedFields.SelectedItems = ((System.Collections.Generic.List<string>)(resources.GetObject("selectedFields.SelectedItems")));
            this.selectedFields.Size = new System.Drawing.Size(387, 345);
            this.selectedFields.TabIndex = 1;
            this.selectedFields.AfterSelect += new Layton.AuditWizard.Common.AfterSelectEventHandler(this.selectedFields_AfterSelect);
            // 
            // FormComplianceReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1007, 644);
            this.Controls.Add(this.bnSaveComplianceReport);
            this.Controls.Add(this.bnRunComplianceReport);
            this.Controls.Add(this.lbReportName);
            this.Controls.Add(this.tbReportName);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.gb1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormComplianceReport";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configure Compliance Report";
            this.Load += new System.EventHandler(this.FormComplianceReport_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormComplianceReport_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.lvTriggers)).EndInit();
            this.gb1.ResumeLayout(false);
            this.gb1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbConditions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDeleteTrigger;
        private Infragistics.Win.UltraWinListView.UltraListView lvTriggers;
        private Layton.AuditWizard.Common.SelectAuditedDataFieldsControl selectedFields;
        private System.Windows.Forms.Button bnRunComplianceReport;
        private System.Windows.Forms.Button bnSaveComplianceReport;
        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Label lbReportName;
        private System.Windows.Forms.TextBox tbReportName;
        private System.Windows.Forms.GroupBox gb1;
        private System.Windows.Forms.Button btnEditComplianceField;
        private System.Windows.Forms.ComboBox cbBooleanValue;
        private Infragistics.Win.Misc.UltraLabel lbAddField;
        private System.Windows.Forms.Label lblUnits;
        private Infragistics.Win.Misc.UltraLabel lbComplianceField;
        private System.Windows.Forms.TextBox tbSelectedComplianceField;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cbConditions;
        private Infragistics.Win.Misc.UltraLabel lbConditions;
        private Infragistics.Win.Misc.UltraLabel lbValue;
        private System.Windows.Forms.Button bnAddTrigger;
        private Infragistics.Win.Misc.UltraLabel lbBooleanOperator;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.ComboBox cbValuePicker;

    }
}