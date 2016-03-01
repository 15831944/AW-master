namespace Layton.AuditWizard.Applications
{
    partial class OSTabView
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
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OSTabView));
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("OperatingSystems", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("OSObject");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Licenses");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Installed Count");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Installed Variance");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status");
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.headerGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.headerLabel = new Infragistics.Win.Misc.UltraLabel();
            this.OSGridView = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.OSMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portableDocumentFormatPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLPaperSpecificationXPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newLicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OSDataSet = new System.Data.DataSet();
            this.OSDataTable = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).BeginInit();
            this.headerGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OSGridView)).BeginInit();
            this.OSMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OSDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OSDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.headerGroupBox);
            this.mainSplitContainer.Panel1MinSize = 80;
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.OSGridView);
            this.mainSplitContainer.Size = new System.Drawing.Size(955, 401);
            this.mainSplitContainer.SplitterDistance = 80;
            this.mainSplitContainer.SplitterWidth = 1;
            this.mainSplitContainer.TabIndex = 3;
            // 
            // headerGroupBox
            // 
            this.headerGroupBox.Controls.Add(this.headerLabel);
            this.headerGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerGroupBox.Location = new System.Drawing.Point(0, 0);
            this.headerGroupBox.Name = "headerGroupBox";
            this.headerGroupBox.Size = new System.Drawing.Size(955, 80);
            this.headerGroupBox.TabIndex = 3;
            this.headerGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // headerLabel
            // 
            appearance4.BackColor = System.Drawing.Color.Transparent;
            appearance4.Image = global::Layton.AuditWizard.Applications.Properties.Resources.os_96;
            appearance4.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance4.TextHAlignAsString = "Center";
            appearance4.TextVAlignAsString = "Middle";
            this.headerLabel.Appearance = appearance4;
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ImageSize = new System.Drawing.Size(72, 72);
            this.headerLabel.Location = new System.Drawing.Point(7, 3);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.headerLabel.Size = new System.Drawing.Size(244, 72);
            this.headerLabel.TabIndex = 3;
            this.headerLabel.Text = "Operating System";
            // 
            // OSGridView
            // 
            this.OSGridView.ContextMenuStrip = this.OSMenuStrip;
            this.OSGridView.DataSource = this.OSDataSet;
            appearance3.BackColor = System.Drawing.Color.White;
            appearance3.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.OSGridView.DisplayLayout.AddNewBox.Appearance = appearance3;
            this.OSGridView.DisplayLayout.AddNewBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance7.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance7.ImageBackground")));
            appearance7.ImageBackgroundAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            appearance7.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance7.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.OSGridView.DisplayLayout.AddNewBox.ButtonAppearance = appearance7;
            this.OSGridView.DisplayLayout.AddNewBox.ButtonConnectorColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.OSGridView.DisplayLayout.AddNewBox.ButtonStyle = Infragistics.Win.UIElementButtonStyle.FlatBorderless;
            appearance8.BackColor = System.Drawing.Color.White;
            this.OSGridView.DisplayLayout.Appearance = appearance8;
            this.OSGridView.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn1.Width = 59;
            appearance50.Image = global::Layton.AuditWizard.Applications.Properties.Resources.os_16;
            ultraGridColumn2.CellAppearance = appearance50;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 201;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 210;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 161;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Width = 194;
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn6.Width = 168;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6});
            ultraGridBand1.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            ultraGridBand1.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            ultraGridBand1.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance51.FontData.SizeInPoints = 8F;
            ultraGridBand1.Override.HeaderAppearance = appearance51;
            this.OSGridView.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            appearance10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance10.FontData.Name = "Trebuchet MS";
            appearance10.FontData.SizeInPoints = 9F;
            appearance10.ForeColor = System.Drawing.Color.White;
            appearance10.TextHAlignAsString = "Right";
            this.OSGridView.DisplayLayout.CaptionAppearance = appearance10;
            this.OSGridView.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.OSGridView.DisplayLayout.FixedHeaderOffImage = ((System.Drawing.Image)(resources.GetObject("OSGridView.DisplayLayout.FixedHeaderOffImage")));
            this.OSGridView.DisplayLayout.FixedHeaderOnImage = ((System.Drawing.Image)(resources.GetObject("OSGridView.DisplayLayout.FixedHeaderOnImage")));
            this.OSGridView.DisplayLayout.FixedRowOffImage = ((System.Drawing.Image)(resources.GetObject("OSGridView.DisplayLayout.FixedRowOffImage")));
            this.OSGridView.DisplayLayout.FixedRowOnImage = ((System.Drawing.Image)(resources.GetObject("OSGridView.DisplayLayout.FixedRowOnImage")));
            appearance11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            appearance11.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(185)))), ((int)(((byte)(235)))));
            appearance11.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance11.FontData.BoldAsString = "True";
            appearance11.FontData.Name = "Verdana";
            appearance11.FontData.SizeInPoints = 10F;
            appearance11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(127)))), ((int)(((byte)(177)))));
            this.OSGridView.DisplayLayout.GroupByBox.Appearance = appearance11;
            this.OSGridView.DisplayLayout.GroupByBox.BandLabelBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.OSGridView.DisplayLayout.GroupByBox.ButtonBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance12.BackColor = System.Drawing.Color.Transparent;
            appearance12.FontData.Name = "Verdana";
            this.OSGridView.DisplayLayout.GroupByBox.PromptAppearance = appearance12;
            this.OSGridView.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.OSGridView.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.OSGridView.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;
            this.OSGridView.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance13.BackColor = System.Drawing.Color.Transparent;
            this.OSGridView.DisplayLayout.Override.CardAreaAppearance = appearance13;
            appearance32.BorderColor = System.Drawing.Color.Transparent;
            appearance32.FontData.Name = "Verdana";
            this.OSGridView.DisplayLayout.Override.CellAppearance = appearance32;
            appearance33.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance33.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance33.ImageBackground")));
            appearance33.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance33.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.OSGridView.DisplayLayout.Override.CellButtonAppearance = appearance33;
            this.OSGridView.DisplayLayout.Override.CellPadding = 4;
            appearance34.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(251)))));
            this.OSGridView.DisplayLayout.Override.FilterCellAppearance = appearance34;
            appearance35.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance35.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance35.ImageBackground")));
            appearance35.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            this.OSGridView.DisplayLayout.Override.FilterClearButtonAppearance = appearance35;
            appearance36.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(251)))));
            appearance36.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
            this.OSGridView.DisplayLayout.Override.FilterRowPromptAppearance = appearance36;
            this.OSGridView.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance37.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(255)))));
            appearance37.BackColor2 = System.Drawing.Color.White;
            appearance37.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance37.FontData.BoldAsString = "True";
            appearance37.FontData.Name = "Verdana";
            appearance37.FontData.SizeInPoints = 8F;
            appearance37.ForeColor = System.Drawing.Color.DimGray;
            appearance37.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance37.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance37.TextHAlignAsString = "Center";
            this.OSGridView.DisplayLayout.Override.HeaderAppearance = appearance37;
            appearance38.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.OSGridView.DisplayLayout.Override.RowAlternateAppearance = appearance38;
            appearance39.BorderColor = System.Drawing.Color.LightGray;
            this.OSGridView.DisplayLayout.Override.RowAppearance = appearance39;
            appearance40.BackColor = System.Drawing.Color.White;
            this.OSGridView.DisplayLayout.Override.RowSelectorAppearance = appearance40;
            this.OSGridView.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance41.BorderColor = System.Drawing.Color.Transparent;
            appearance41.ForeColor = System.Drawing.Color.Black;
            this.OSGridView.DisplayLayout.Override.SelectedCellAppearance = appearance41;
            appearance42.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(223)))));
            appearance42.BorderColor = System.Drawing.Color.Transparent;
            appearance42.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance42.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 4);
            appearance42.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.OSGridView.DisplayLayout.Override.SelectedRowAppearance = appearance42;
            appearance43.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 4, 2, 4);
            appearance43.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.Appearance = appearance43;
            appearance44.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance44.ImageBackground")));
            appearance44.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(3, 2, 3, 2);
            scrollBarLook1.AppearanceHorizontal = appearance44;
            appearance45.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance45.ImageBackground")));
            appearance45.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 3, 2, 3);
            appearance45.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.AppearanceVertical = appearance45;
            appearance46.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance46.ImageBackground")));
            appearance46.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(0, 2, 0, 1);
            scrollBarLook1.TrackAppearanceHorizontal = appearance46;
            appearance47.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance47.ImageBackground")));
            appearance47.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 0, 1, 0);
            appearance47.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.TrackAppearanceVertical = appearance47;
            this.OSGridView.DisplayLayout.ScrollBarLook = scrollBarLook1;
            this.OSGridView.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.OSGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OSGridView.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.OSGridView.Location = new System.Drawing.Point(0, 0);
            this.OSGridView.Name = "OSGridView";
            this.OSGridView.Size = new System.Drawing.Size(955, 320);
            this.OSGridView.TabIndex = 2;
            this.OSGridView.Text = "Grid Caption Area";
            this.OSGridView.UseAppStyling = false;
            this.OSGridView.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.OSGridView.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.OSGridView_InitializeRow);
            // 
            // OSMenuStrip
            // 
            this.OSMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportDataToolStripMenuItem,
            this.newLicenseToolStripMenuItem});
            this.OSMenuStrip.Name = "OSMenuStrip";
            this.OSMenuStrip.Size = new System.Drawing.Size(169, 48);
            // 
            // exportDataToolStripMenuItem
            // 
            this.exportDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excelToolStripMenuItem,
            this.portableDocumentFormatPDFToolStripMenuItem,
            this.xMLPaperSpecificationXPSToolStripMenuItem});
            this.exportDataToolStripMenuItem.Image = global::Layton.AuditWizard.Applications.Properties.Resources.export_16;
            this.exportDataToolStripMenuItem.Name = "exportDataToolStripMenuItem";
            this.exportDataToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.exportDataToolStripMenuItem.Text = "&Export Data to...";
            // 
            // excelToolStripMenuItem
            // 
            this.excelToolStripMenuItem.Image = global::Layton.AuditWizard.Applications.Properties.Resources.excel_32;
            this.excelToolStripMenuItem.Name = "excelToolStripMenuItem";
            this.excelToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.excelToolStripMenuItem.Text = "&Excel";
            this.excelToolStripMenuItem.Click += new System.EventHandler(this.ExportXlsToolStripMenuItem_Click);
            // 
            // portableDocumentFormatPDFToolStripMenuItem
            // 
            this.portableDocumentFormatPDFToolStripMenuItem.Image = global::Layton.AuditWizard.Applications.Properties.Resources.pdf_32;
            this.portableDocumentFormatPDFToolStripMenuItem.Name = "portableDocumentFormatPDFToolStripMenuItem";
            this.portableDocumentFormatPDFToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.portableDocumentFormatPDFToolStripMenuItem.Text = "Portable Document Format (PDF)";
            this.portableDocumentFormatPDFToolStripMenuItem.Click += new System.EventHandler(this.ExportPDFToolStripMenuItem_Click);
            // 
            // xMLPaperSpecificationXPSToolStripMenuItem
            // 
            this.xMLPaperSpecificationXPSToolStripMenuItem.Image = global::Layton.AuditWizard.Applications.Properties.Resources.xps_32;
            this.xMLPaperSpecificationXPSToolStripMenuItem.Name = "xMLPaperSpecificationXPSToolStripMenuItem";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.xMLPaperSpecificationXPSToolStripMenuItem.Text = "XML Paper Specification (XPS)";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Click += new System.EventHandler(this.ExportXPSToolStripMenuItem_Click);
            // 
            // newLicenseToolStripMenuItem
            // 
            this.newLicenseToolStripMenuItem.Image = global::Layton.AuditWizard.Applications.Properties.Resources.license_add_16;
            this.newLicenseToolStripMenuItem.Name = "newLicenseToolStripMenuItem";
            this.newLicenseToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.newLicenseToolStripMenuItem.Text = "New License";
            this.newLicenseToolStripMenuItem.Click += new System.EventHandler(this.newLicenseToolStripMenuItem_Click);
            // 
            // OSDataSet
            // 
            this.OSDataSet.DataSetName = "OSDataSet";
            this.OSDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.OSDataTable});
            // 
            // OSDataTable
            // 
            this.OSDataTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5,
            this.dataColumn6});
            this.OSDataTable.MinimumCapacity = 0;
            this.OSDataTable.TableName = "OperatingSystems";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "OSObject";
            this.dataColumn1.DataType = typeof(object);
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "Name";
            // 
            // dataColumn3
            // 
            this.dataColumn3.ColumnName = "Licenses";
            // 
            // dataColumn4
            // 
            this.dataColumn4.ColumnName = "Installed Count";
            // 
            // dataColumn5
            // 
            this.dataColumn5.ColumnName = "Installed Variance";
            // 
            // dataColumn6
            // 
            this.dataColumn6.ColumnName = "Status";
            // 
            // OSTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainSplitContainer);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "OSTabView";
            this.Size = new System.Drawing.Size(955, 401);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            this.mainSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).EndInit();
            this.headerGroupBox.ResumeLayout(false);
            this.headerGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OSGridView)).EndInit();
            this.OSMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OSDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OSDataTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.SplitContainer mainSplitContainer;
		private Infragistics.Win.Misc.UltraGroupBox headerGroupBox;
		private Infragistics.Win.Misc.UltraLabel headerLabel;
		private System.Data.DataSet OSDataSet;
		private System.Data.DataTable OSDataTable;
		private System.Data.DataColumn dataColumn1;
		private System.Data.DataColumn dataColumn2;
		private System.Data.DataColumn dataColumn3;
		private System.Data.DataColumn dataColumn4;
		private System.Data.DataColumn dataColumn5;
		private System.Data.DataColumn dataColumn6;
		private Infragistics.Win.UltraWinGrid.UltraGrid OSGridView;
		private System.Windows.Forms.ContextMenuStrip OSMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem exportDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem excelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem portableDocumentFormatPDFToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem xMLPaperSpecificationXPSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newLicenseToolStripMenuItem;

	}
}
