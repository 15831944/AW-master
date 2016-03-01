namespace Layton.AuditWizard.Network
{
    partial class UserDefinedDataTabView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserDefinedDataTabView));
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("auditedData", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Column1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Column2");
            Infragistics.Win.Appearance appearance52 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            this.groupMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editUserDataFieldsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portableDocumentFormatPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLPaperSpecificationXPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.headerGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.headerLabel = new Infragistics.Win.Misc.UltraLabel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.auditGridView = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.auditDataSet = new System.Data.DataSet();
            this.auditedDataTable = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.groupMenuStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).BeginInit();
            this.headerGroupBox.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.auditGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.auditDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.auditedDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // groupMenuStrip
            // 
            this.groupMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editUserDataFieldsToolStripMenuItem,
            this.exportDataToolStripMenuItem});
            this.groupMenuStrip.Name = "applicationsMenuStrip";
            this.groupMenuStrip.Size = new System.Drawing.Size(185, 48);
            // 
            // editUserDataFieldsToolStripMenuItem
            // 
            this.editUserDataFieldsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editUserDataFieldsToolStripMenuItem.Image")));
            this.editUserDataFieldsToolStripMenuItem.Name = "editUserDataFieldsToolStripMenuItem";
            this.editUserDataFieldsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.editUserDataFieldsToolStripMenuItem.Text = "Edit &User Data Fields";
            this.editUserDataFieldsToolStripMenuItem.Click += new System.EventHandler(this.editUserDataFieldsToolStripMenuItem_Click);
            // 
            // exportDataToolStripMenuItem
            // 
            this.exportDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excelToolStripMenuItem,
            this.portableDocumentFormatPDFToolStripMenuItem,
            this.xMLPaperSpecificationXPSToolStripMenuItem});
            this.exportDataToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.export_16;
            this.exportDataToolStripMenuItem.Name = "exportDataToolStripMenuItem";
            this.exportDataToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exportDataToolStripMenuItem.Text = "&Export Data to...";
            // 
            // excelToolStripMenuItem
            // 
            this.excelToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.excel_32;
            this.excelToolStripMenuItem.Name = "excelToolStripMenuItem";
            this.excelToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.excelToolStripMenuItem.Text = "&Excel";
            // 
            // portableDocumentFormatPDFToolStripMenuItem
            // 
            this.portableDocumentFormatPDFToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.pdf_32;
            this.portableDocumentFormatPDFToolStripMenuItem.Name = "portableDocumentFormatPDFToolStripMenuItem";
            this.portableDocumentFormatPDFToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.portableDocumentFormatPDFToolStripMenuItem.Text = "Portable Document Format (PDF)";
            // 
            // xMLPaperSpecificationXPSToolStripMenuItem
            // 
            this.xMLPaperSpecificationXPSToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.xps_32;
            this.xMLPaperSpecificationXPSToolStripMenuItem.Name = "xMLPaperSpecificationXPSToolStripMenuItem";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.xMLPaperSpecificationXPSToolStripMenuItem.Text = "XML Paper Specification (XPS)";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.headerGroupBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1032, 80);
            this.panel1.TabIndex = 1;
            // 
            // headerGroupBox
            // 
            this.headerGroupBox.Controls.Add(this.headerLabel);
            this.headerGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerGroupBox.Location = new System.Drawing.Point(0, 0);
            this.headerGroupBox.Name = "headerGroupBox";
            this.headerGroupBox.Size = new System.Drawing.Size(1032, 80);
            this.headerGroupBox.TabIndex = 4;
            this.headerGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // headerLabel
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            appearance1.Image = global::Layton.AuditWizard.Network.Properties.Resources.UserData_72;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.headerLabel.Appearance = appearance1;
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ImageSize = new System.Drawing.Size(72, 72);
            this.headerLabel.Location = new System.Drawing.Point(7, 2);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.headerLabel.Size = new System.Drawing.Size(246, 72);
            this.headerLabel.TabIndex = 5;
            this.headerLabel.Text = "User Defined Data";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.auditGridView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 80);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1032, 402);
            this.panel2.TabIndex = 2;
            // 
            // auditGridView
            // 
            this.auditGridView.AllowDrop = true;
            this.auditGridView.ContextMenuStrip = this.groupMenuStrip;
            this.auditGridView.DataSource = this.auditDataSet;
            appearance9.BackColor = System.Drawing.Color.White;
            appearance9.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.auditGridView.DisplayLayout.AddNewBox.Appearance = appearance9;
            this.auditGridView.DisplayLayout.AddNewBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance10.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance10.ImageBackground")));
            appearance10.ImageBackgroundAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            appearance10.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance10.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.auditGridView.DisplayLayout.AddNewBox.ButtonAppearance = appearance10;
            this.auditGridView.DisplayLayout.AddNewBox.ButtonConnectorColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.auditGridView.DisplayLayout.AddNewBox.ButtonStyle = Infragistics.Win.UIElementButtonStyle.FlatBorderless;
            appearance11.BackColor = System.Drawing.Color.White;
            this.auditGridView.DisplayLayout.Appearance = appearance11;
            this.auditGridView.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 667;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 344;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2});
            ultraGridBand1.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            ultraGridBand1.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            ultraGridBand1.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance52.FontData.SizeInPoints = 8F;
            ultraGridBand1.Override.HeaderAppearance = appearance52;
            appearance53.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            appearance53.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            appearance53.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            ultraGridBand1.Override.RowAlternateAppearance = appearance53;
            this.auditGridView.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            appearance12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance12.FontData.Name = "Trebuchet MS";
            appearance12.FontData.SizeInPoints = 9F;
            appearance12.ForeColor = System.Drawing.Color.White;
            appearance12.TextHAlignAsString = "Right";
            this.auditGridView.DisplayLayout.CaptionAppearance = appearance12;
            this.auditGridView.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.auditGridView.DisplayLayout.FixedHeaderOffImage = ((System.Drawing.Image)(resources.GetObject("auditGridView.DisplayLayout.FixedHeaderOffImage")));
            this.auditGridView.DisplayLayout.FixedHeaderOnImage = ((System.Drawing.Image)(resources.GetObject("auditGridView.DisplayLayout.FixedHeaderOnImage")));
            this.auditGridView.DisplayLayout.FixedRowOffImage = ((System.Drawing.Image)(resources.GetObject("auditGridView.DisplayLayout.FixedRowOffImage")));
            this.auditGridView.DisplayLayout.FixedRowOnImage = ((System.Drawing.Image)(resources.GetObject("auditGridView.DisplayLayout.FixedRowOnImage")));
            appearance13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            appearance13.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(185)))), ((int)(((byte)(235)))));
            appearance13.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance13.FontData.BoldAsString = "True";
            appearance13.FontData.Name = "Verdana";
            appearance13.FontData.SizeInPoints = 10F;
            appearance13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(127)))), ((int)(((byte)(177)))));
            this.auditGridView.DisplayLayout.GroupByBox.Appearance = appearance13;
            this.auditGridView.DisplayLayout.GroupByBox.BandLabelBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.auditGridView.DisplayLayout.GroupByBox.ButtonBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance14.BackColor = System.Drawing.Color.Transparent;
            appearance14.FontData.Name = "Verdana";
            this.auditGridView.DisplayLayout.GroupByBox.PromptAppearance = appearance14;
            this.auditGridView.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.auditGridView.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.auditGridView.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;
            this.auditGridView.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance15.BackColor = System.Drawing.Color.Transparent;
            this.auditGridView.DisplayLayout.Override.CardAreaAppearance = appearance15;
            appearance16.BorderColor = System.Drawing.Color.Transparent;
            appearance16.FontData.Name = "Verdana";
            this.auditGridView.DisplayLayout.Override.CellAppearance = appearance16;
            appearance17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance17.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance17.ImageBackground")));
            appearance17.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance17.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.auditGridView.DisplayLayout.Override.CellButtonAppearance = appearance17;
            this.auditGridView.DisplayLayout.Override.CellPadding = 4;
            appearance18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(251)))));
            this.auditGridView.DisplayLayout.Override.FilterCellAppearance = appearance18;
            appearance19.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance19.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance19.ImageBackground")));
            appearance19.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            this.auditGridView.DisplayLayout.Override.FilterClearButtonAppearance = appearance19;
            appearance20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(251)))));
            appearance20.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
            this.auditGridView.DisplayLayout.Override.FilterRowPromptAppearance = appearance20;
            this.auditGridView.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(255)))));
            appearance21.BackColor2 = System.Drawing.Color.White;
            appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance21.FontData.BoldAsString = "True";
            appearance21.FontData.Name = "Verdana";
            appearance21.FontData.SizeInPoints = 8F;
            appearance21.ForeColor = System.Drawing.Color.DimGray;
            appearance21.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance21.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance21.TextHAlignAsString = "Center";
            this.auditGridView.DisplayLayout.Override.HeaderAppearance = appearance21;
            appearance22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.auditGridView.DisplayLayout.Override.RowAlternateAppearance = appearance22;
            appearance23.BorderColor = System.Drawing.Color.LightGray;
            this.auditGridView.DisplayLayout.Override.RowAppearance = appearance23;
            appearance24.BackColor = System.Drawing.Color.White;
            this.auditGridView.DisplayLayout.Override.RowSelectorAppearance = appearance24;
            this.auditGridView.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance25.BorderColor = System.Drawing.Color.Transparent;
            appearance25.ForeColor = System.Drawing.Color.Black;
            this.auditGridView.DisplayLayout.Override.SelectedCellAppearance = appearance25;
            appearance26.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(223)))));
            appearance26.BorderColor = System.Drawing.Color.Transparent;
            appearance26.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance26.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 4);
            appearance26.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.auditGridView.DisplayLayout.Override.SelectedRowAppearance = appearance26;
            appearance27.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 4, 2, 4);
            appearance27.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.Appearance = appearance27;
            appearance28.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance28.ImageBackground")));
            appearance28.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(3, 2, 3, 2);
            scrollBarLook1.AppearanceHorizontal = appearance28;
            appearance29.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance29.ImageBackground")));
            appearance29.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 3, 2, 3);
            appearance29.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.AppearanceVertical = appearance29;
            appearance50.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance50.ImageBackground")));
            appearance50.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(0, 2, 0, 1);
            scrollBarLook1.TrackAppearanceHorizontal = appearance50;
            appearance51.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance51.ImageBackground")));
            appearance51.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 0, 1, 0);
            appearance51.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.TrackAppearanceVertical = appearance51;
            this.auditGridView.DisplayLayout.ScrollBarLook = scrollBarLook1;
            this.auditGridView.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.auditGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.auditGridView.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.auditGridView.Location = new System.Drawing.Point(0, 0);
            this.auditGridView.Name = "auditGridView";
            this.auditGridView.Size = new System.Drawing.Size(1032, 402);
            this.auditGridView.TabIndex = 1;
            this.auditGridView.Text = "Grid Caption Area";
            this.auditGridView.UseAppStyling = false;
            this.auditGridView.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            // 
            // auditDataSet
            // 
            this.auditDataSet.DataSetName = "AuditDataSet";
            this.auditDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.auditedDataTable});
            // 
            // auditedDataTable
            // 
            this.auditedDataTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2});
            this.auditedDataTable.MinimumCapacity = 0;
            this.auditedDataTable.TableName = "auditedData";
            // 
            // dataColumn1
            // 
            this.dataColumn1.Caption = "User Data Field Name";
            this.dataColumn1.ColumnName = "Column1";
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Value";
            this.dataColumn2.ColumnName = "Column2";
            // 
            // UserDefinedDataTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "UserDefinedDataTabView";
            this.Size = new System.Drawing.Size(1032, 482);
            this.groupMenuStrip.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).EndInit();
            this.headerGroupBox.ResumeLayout(false);
            this.headerGroupBox.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.auditGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.auditDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.auditedDataTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.ContextMenuStrip groupMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem exportDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem excelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem portableDocumentFormatPDFToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem xMLPaperSpecificationXPSToolStripMenuItem;
		private System.Windows.Forms.Panel panel1;
		private Infragistics.Win.Misc.UltraGroupBox headerGroupBox;
		private Infragistics.Win.Misc.UltraLabel headerLabel;
		private System.Windows.Forms.Panel panel2;
		private Infragistics.Win.UltraWinGrid.UltraGrid auditGridView;
		private System.Data.DataSet auditDataSet;
		private System.Data.DataTable auditedDataTable;
		private System.Data.DataColumn dataColumn1;
		private System.Data.DataColumn dataColumn2;
		private System.Windows.Forms.ToolStripMenuItem editUserDataFieldsToolStripMenuItem;

    }
}
