namespace Layton.AuditWizard.AuditTrail
{
    partial class AuditTrailTabView
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuditTrailTabView));
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("AuditTrailEntries", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("dataObject");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("date");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("computer");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("username");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Action");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("oldvalue");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("newvalue");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("licensetype");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.headerGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.headerLabel = new Infragistics.Win.Misc.UltraLabel();
            this.auditTrailGridView = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.auditTrailDataSet = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.date = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.dataColumn8 = new System.Data.DataColumn();
            this.dataColumn9 = new System.Data.DataColumn();
            this.auditTrailMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portableDocumentFormatPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLPaperSpecificationXPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataColumn1 = new System.Data.DataColumn();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).BeginInit();
            this.headerGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.auditTrailGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.auditTrailDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            this.auditTrailMenuStrip.SuspendLayout();
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
            this.mainSplitContainer.Panel2.Controls.Add(this.auditTrailGridView);
            this.mainSplitContainer.Size = new System.Drawing.Size(1090, 430);
            this.mainSplitContainer.SplitterDistance = 80;
            this.mainSplitContainer.SplitterWidth = 1;
            this.mainSplitContainer.TabIndex = 4;
            // 
            // headerGroupBox
            // 
            this.headerGroupBox.Controls.Add(this.headerLabel);
            this.headerGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerGroupBox.Location = new System.Drawing.Point(0, 0);
            this.headerGroupBox.Name = "headerGroupBox";
            this.headerGroupBox.Size = new System.Drawing.Size(1090, 80);
            this.headerGroupBox.TabIndex = 4;
            this.headerGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            this.headerGroupBox.SizeChanged += new System.EventHandler(this.headerGroupBox_SizeChanged);
            // 
            // headerLabel
            // 
            appearance4.BackColor = System.Drawing.Color.Transparent;
            appearance4.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.AuditTrail_72;
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
            this.headerLabel.Size = new System.Drawing.Size(174, 72);
            this.headerLabel.TabIndex = 3;
            this.headerLabel.Text = "Audit Trail";
            // 
            // auditTrailGridView
            // 
            this.auditTrailGridView.DataSource = this.auditTrailDataSet;
            appearance1.BackColor = System.Drawing.Color.White;
            appearance1.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.auditTrailGridView.DisplayLayout.AddNewBox.Appearance = appearance1;
            this.auditTrailGridView.DisplayLayout.AddNewBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance2.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance2.ImageBackground")));
            appearance2.ImageBackgroundAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            appearance2.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance2.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.auditTrailGridView.DisplayLayout.AddNewBox.ButtonAppearance = appearance2;
            this.auditTrailGridView.DisplayLayout.AddNewBox.ButtonConnectorColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.auditTrailGridView.DisplayLayout.AddNewBox.ButtonStyle = Infragistics.Win.UIElementButtonStyle.FlatBorderless;
            appearance3.BackColor = System.Drawing.Color.White;
            this.auditTrailGridView.DisplayLayout.Appearance = appearance3;
            this.auditTrailGridView.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 124;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 157;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 139;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Width = 134;
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn6.Width = 125;
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn7.Width = 127;
            ultraGridColumn8.Header.VisiblePosition = 7;
            ultraGridColumn8.Width = 133;
            ultraGridColumn9.Header.VisiblePosition = 8;
            ultraGridColumn9.Width = 130;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9});
            this.auditTrailGridView.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            appearance5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance5.FontData.Name = "Trebuchet MS";
            appearance5.FontData.SizeInPoints = 9F;
            appearance5.ForeColor = System.Drawing.Color.White;
            appearance5.TextHAlignAsString = "Right";
            this.auditTrailGridView.DisplayLayout.CaptionAppearance = appearance5;
            this.auditTrailGridView.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.auditTrailGridView.DisplayLayout.FixedHeaderOffImage = ((System.Drawing.Image)(resources.GetObject("auditTrailGridView.DisplayLayout.FixedHeaderOffImage")));
            this.auditTrailGridView.DisplayLayout.FixedHeaderOnImage = ((System.Drawing.Image)(resources.GetObject("auditTrailGridView.DisplayLayout.FixedHeaderOnImage")));
            this.auditTrailGridView.DisplayLayout.FixedRowOffImage = ((System.Drawing.Image)(resources.GetObject("auditTrailGridView.DisplayLayout.FixedRowOffImage")));
            this.auditTrailGridView.DisplayLayout.FixedRowOnImage = ((System.Drawing.Image)(resources.GetObject("auditTrailGridView.DisplayLayout.FixedRowOnImage")));
            appearance6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            appearance6.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(185)))), ((int)(((byte)(235)))));
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance6.FontData.BoldAsString = "True";
            appearance6.FontData.Name = "Verdana";
            appearance6.FontData.SizeInPoints = 10F;
            appearance6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(127)))), ((int)(((byte)(177)))));
            this.auditTrailGridView.DisplayLayout.GroupByBox.Appearance = appearance6;
            this.auditTrailGridView.DisplayLayout.GroupByBox.BandLabelBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.auditTrailGridView.DisplayLayout.GroupByBox.ButtonBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance7.BackColor = System.Drawing.Color.Transparent;
            appearance7.FontData.Name = "Verdana";
            this.auditTrailGridView.DisplayLayout.GroupByBox.PromptAppearance = appearance7;
            this.auditTrailGridView.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.WithinGroup;
            this.auditTrailGridView.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.auditTrailGridView.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.auditTrailGridView.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;
            this.auditTrailGridView.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance8.BackColor = System.Drawing.Color.Transparent;
            this.auditTrailGridView.DisplayLayout.Override.CardAreaAppearance = appearance8;
            appearance9.BorderColor = System.Drawing.Color.Transparent;
            appearance9.FontData.Name = "Verdana";
            this.auditTrailGridView.DisplayLayout.Override.CellAppearance = appearance9;
            appearance10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance10.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance10.ImageBackground")));
            appearance10.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance10.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.auditTrailGridView.DisplayLayout.Override.CellButtonAppearance = appearance10;
            this.auditTrailGridView.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.auditTrailGridView.DisplayLayout.Override.CellPadding = 4;
            appearance11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(251)))));
            this.auditTrailGridView.DisplayLayout.Override.FilterCellAppearance = appearance11;
            appearance12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance12.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance12.ImageBackground")));
            appearance12.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            this.auditTrailGridView.DisplayLayout.Override.FilterClearButtonAppearance = appearance12;
            appearance13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(251)))));
            appearance13.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
            this.auditTrailGridView.DisplayLayout.Override.FilterRowPromptAppearance = appearance13;
            this.auditTrailGridView.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(255)))));
            appearance14.BackColor2 = System.Drawing.Color.White;
            appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance14.FontData.BoldAsString = "True";
            appearance14.FontData.Name = "Verdana";
            appearance14.FontData.SizeInPoints = 8F;
            appearance14.ForeColor = System.Drawing.Color.DimGray;
            appearance14.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance14.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance14.TextHAlignAsString = "Center";
            this.auditTrailGridView.DisplayLayout.Override.HeaderAppearance = appearance14;
            appearance15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.auditTrailGridView.DisplayLayout.Override.RowAlternateAppearance = appearance15;
            appearance16.BorderColor = System.Drawing.Color.LightGray;
            this.auditTrailGridView.DisplayLayout.Override.RowAppearance = appearance16;
            appearance17.BackColor = System.Drawing.Color.White;
            this.auditTrailGridView.DisplayLayout.Override.RowSelectorAppearance = appearance17;
            this.auditTrailGridView.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance18.BorderColor = System.Drawing.Color.Transparent;
            appearance18.ForeColor = System.Drawing.Color.Black;
            this.auditTrailGridView.DisplayLayout.Override.SelectedCellAppearance = appearance18;
            appearance19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(223)))));
            appearance19.BorderColor = System.Drawing.Color.Transparent;
            appearance19.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance19.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 4);
            appearance19.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.auditTrailGridView.DisplayLayout.Override.SelectedRowAppearance = appearance19;
            appearance20.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 4, 2, 4);
            appearance20.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.Appearance = appearance20;
            appearance21.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance21.ImageBackground")));
            appearance21.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(3, 2, 3, 2);
            scrollBarLook1.AppearanceHorizontal = appearance21;
            appearance22.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance22.ImageBackground")));
            appearance22.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 3, 2, 3);
            appearance22.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.AppearanceVertical = appearance22;
            appearance23.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance23.ImageBackground")));
            appearance23.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(0, 2, 0, 1);
            scrollBarLook1.TrackAppearanceHorizontal = appearance23;
            appearance24.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance24.ImageBackground")));
            appearance24.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 0, 1, 0);
            appearance24.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.TrackAppearanceVertical = appearance24;
            scrollBarLook1.ViewStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarViewStyle.Outlook2007;
            this.auditTrailGridView.DisplayLayout.ScrollBarLook = scrollBarLook1;
            this.auditTrailGridView.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.auditTrailGridView.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.auditTrailGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.auditTrailGridView.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.auditTrailGridView.Location = new System.Drawing.Point(0, 0);
            this.auditTrailGridView.Name = "auditTrailGridView";
            this.auditTrailGridView.Size = new System.Drawing.Size(1090, 349);
            this.auditTrailGridView.TabIndex = 0;
            this.auditTrailGridView.Text = "Grid Caption Area";
            this.auditTrailGridView.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            // 
            // auditTrailDataSet
            // 
            this.auditTrailDataSet.DataSetName = "auditTrailDataSet";
            this.auditTrailDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1});
            // 
            // dataTable1
            // 
            this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.date,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn7,
            this.dataColumn8,
            this.dataColumn9});
            this.dataTable1.TableName = "AuditTrailEntries";
            // 
            // date
            // 
            this.date.Caption = "Date";
            this.date.ColumnName = "date";
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Asset";
            this.dataColumn2.ColumnName = "computer";
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Username";
            this.dataColumn3.ColumnName = "username";
            // 
            // dataColumn5
            // 
            this.dataColumn5.Caption = "Action";
            this.dataColumn5.ColumnName = "Action";
            // 
            // dataColumn6
            // 
            this.dataColumn6.Caption = "Name";
            this.dataColumn6.ColumnName = "name";
            // 
            // dataColumn7
            // 
            this.dataColumn7.Caption = "Previous Value";
            this.dataColumn7.ColumnName = "oldvalue";
            // 
            // dataColumn8
            // 
            this.dataColumn8.Caption = "New Value";
            this.dataColumn8.ColumnName = "newvalue";
            // 
            // dataColumn9
            // 
            this.dataColumn9.Caption = "Item";
            this.dataColumn9.ColumnName = "licensetype";
            // 
            // auditTrailMenuStrip
            // 
            this.auditTrailMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportDataToolStripMenuItem,
            this.deleteItemsToolStripMenuItem});
            this.auditTrailMenuStrip.Name = "applicationsMenuStrip";
            this.auditTrailMenuStrip.Size = new System.Drawing.Size(169, 48);
            // 
            // exportDataToolStripMenuItem
            // 
            this.exportDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excelToolStripMenuItem,
            this.portableDocumentFormatPDFToolStripMenuItem,
            this.xMLPaperSpecificationXPSToolStripMenuItem});
            this.exportDataToolStripMenuItem.Name = "exportDataToolStripMenuItem";
            this.exportDataToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.exportDataToolStripMenuItem.Text = "&Export Data to...";
            // 
            // excelToolStripMenuItem
            // 
            this.excelToolStripMenuItem.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.excel_32;
            this.excelToolStripMenuItem.Name = "excelToolStripMenuItem";
            this.excelToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.excelToolStripMenuItem.Text = "&Excel";
            this.excelToolStripMenuItem.Click += new System.EventHandler(this.ExportXlsToolStripMenuItem_Click);
            // 
            // portableDocumentFormatPDFToolStripMenuItem
            // 
            this.portableDocumentFormatPDFToolStripMenuItem.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.pdf_32;
            this.portableDocumentFormatPDFToolStripMenuItem.Name = "portableDocumentFormatPDFToolStripMenuItem";
            this.portableDocumentFormatPDFToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.portableDocumentFormatPDFToolStripMenuItem.Text = "Portable Document Format (PDF)";
            this.portableDocumentFormatPDFToolStripMenuItem.Click += new System.EventHandler(this.ExportPDFToolStripMenuItem_Click);
            // 
            // xMLPaperSpecificationXPSToolStripMenuItem
            // 
            this.xMLPaperSpecificationXPSToolStripMenuItem.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.xps_32;
            this.xMLPaperSpecificationXPSToolStripMenuItem.Name = "xMLPaperSpecificationXPSToolStripMenuItem";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.xMLPaperSpecificationXPSToolStripMenuItem.Text = "XML Paper Specification (XPS)";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Click += new System.EventHandler(this.ExportXPSToolStripMenuItem_Click);
            // 
            // deleteItemsToolStripMenuItem
            // 
            this.deleteItemsToolStripMenuItem.Image = global::Layton.AuditWizard.AuditTrail.Properties.Resources.cross;
            this.deleteItemsToolStripMenuItem.Name = "deleteItemsToolStripMenuItem";
            this.deleteItemsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.deleteItemsToolStripMenuItem.Text = "&Delete Item(s)";
            this.deleteItemsToolStripMenuItem.Click += new System.EventHandler(this.deleteItemsToolStripMenuItem_Click);
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnMapping = System.Data.MappingType.Hidden;
            this.dataColumn1.ColumnName = "dataObject";
            this.dataColumn1.DataType = typeof(object);
            // 
            // AuditTrailTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainSplitContainer);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "AuditTrailTabView";
            this.Size = new System.Drawing.Size(1090, 430);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            this.mainSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).EndInit();
            this.headerGroupBox.ResumeLayout(false);
            this.headerGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.auditTrailGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.auditTrailDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            this.auditTrailMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.SplitContainer mainSplitContainer;
		private Infragistics.Win.UltraWinGrid.UltraGrid auditTrailGridView;
		private System.Data.DataSet auditTrailDataSet;
		private System.Data.DataTable dataTable1;
		private System.Windows.Forms.ContextMenuStrip auditTrailMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem exportDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem excelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem portableDocumentFormatPDFToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem xMLPaperSpecificationXPSToolStripMenuItem;
		private Infragistics.Win.Misc.UltraGroupBox headerGroupBox;
		private Infragistics.Win.Misc.UltraLabel headerLabel;
		private System.Data.DataColumn dataColumn1;
		private System.Data.DataColumn date;
		private System.Data.DataColumn dataColumn2;
		private System.Data.DataColumn dataColumn3;
		private System.Data.DataColumn dataColumn5;
		private System.Data.DataColumn dataColumn6;
		private System.Data.DataColumn dataColumn7;
        private System.Data.DataColumn dataColumn8;
		private System.Windows.Forms.ToolStripMenuItem deleteItemsToolStripMenuItem;
        private System.Data.DataColumn dataColumn9;

	}
}
