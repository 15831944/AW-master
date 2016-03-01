namespace Layton.AuditWizard.Common
{
	partial class FormAlertLog
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAlertLog));
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("alertsTable", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DataObject");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Date");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("type");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Item");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Previous Value");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("New Value");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Alert Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Asset Name");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            this.alertsGridView = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.operationsMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portableDocumentFormatPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLPaperSpecificationXPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alertsDataSet = new System.Data.DataSet();
            this.alertsTable = new System.Data.DataTable();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.dataColumn9 = new System.Data.DataColumn();
            this.dataColumn10 = new System.Data.DataColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.bnOK = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.cbShowNewAtStartup = new System.Windows.Forms.CheckBox();
            this.dtpStartDateTime = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.bnDelete = new System.Windows.Forms.Button();
            this.dataColumn1 = new System.Data.DataColumn();
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsGridView)).BeginInit();
            this.operationsMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertsDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpStartDateTime)).BeginInit();
            this.SuspendLayout();
            // 
            // footerPictureBox
            // 
            this.footerPictureBox.Image = global::Layton.AuditWizard.Common.Properties.Resources.alertlog_corner;
            this.footerPictureBox.Location = new System.Drawing.Point(517, 467);
            // 
            // alertsGridView
            // 
            this.alertsGridView.AllowDrop = true;
            this.alertsGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.alertsGridView.ContextMenuStrip = this.operationsMenuStrip;
            this.alertsGridView.DataSource = this.alertsDataSet;
            appearance3.BackColor = System.Drawing.Color.White;
            appearance3.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.alertsGridView.DisplayLayout.AddNewBox.Appearance = appearance3;
            this.alertsGridView.DisplayLayout.AddNewBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance4.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance4.ImageBackground")));
            appearance4.ImageBackgroundAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            appearance4.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance4.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.alertsGridView.DisplayLayout.AddNewBox.ButtonAppearance = appearance4;
            this.alertsGridView.DisplayLayout.AddNewBox.ButtonConnectorColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.alertsGridView.DisplayLayout.AddNewBox.ButtonStyle = Infragistics.Win.UIElementButtonStyle.FlatBorderless;
            appearance5.BackColor = System.Drawing.Color.White;
            this.alertsGridView.DisplayLayout.Appearance = appearance5;
            this.alertsGridView.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 102;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 129;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 96;
            ultraGridColumn5.Header.VisiblePosition = 8;
            ultraGridColumn5.Width = 130;
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn6.Width = 133;
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn7.Width = 132;
            ultraGridColumn8.Header.VisiblePosition = 4;
            ultraGridColumn8.Width = 96;
            ultraGridColumn9.Header.VisiblePosition = 7;
            ultraGridColumn9.Width = 101;
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
            ultraGridBand1.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            ultraGridBand1.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            ultraGridBand1.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance12.FontData.SizeInPoints = 8F;
            ultraGridBand1.Override.HeaderAppearance = appearance12;
            appearance27.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            appearance27.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            appearance27.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            ultraGridBand1.Override.RowAlternateAppearance = appearance27;
            this.alertsGridView.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            appearance6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance6.FontData.Name = "Trebuchet MS";
            appearance6.FontData.SizeInPoints = 9F;
            appearance6.ForeColor = System.Drawing.Color.White;
            appearance6.TextHAlignAsString = "Right";
            this.alertsGridView.DisplayLayout.CaptionAppearance = appearance6;
            this.alertsGridView.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.alertsGridView.DisplayLayout.FixedHeaderOffImage = ((System.Drawing.Image)(resources.GetObject("alertsGridView.DisplayLayout.FixedHeaderOffImage")));
            this.alertsGridView.DisplayLayout.FixedHeaderOnImage = ((System.Drawing.Image)(resources.GetObject("alertsGridView.DisplayLayout.FixedHeaderOnImage")));
            this.alertsGridView.DisplayLayout.FixedRowOffImage = ((System.Drawing.Image)(resources.GetObject("alertsGridView.DisplayLayout.FixedRowOffImage")));
            this.alertsGridView.DisplayLayout.FixedRowOnImage = ((System.Drawing.Image)(resources.GetObject("alertsGridView.DisplayLayout.FixedRowOnImage")));
            appearance7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            appearance7.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(185)))), ((int)(((byte)(235)))));
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance7.FontData.BoldAsString = "True";
            appearance7.FontData.Name = "Verdana";
            appearance7.FontData.SizeInPoints = 10F;
            appearance7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(127)))), ((int)(((byte)(177)))));
            this.alertsGridView.DisplayLayout.GroupByBox.Appearance = appearance7;
            this.alertsGridView.DisplayLayout.GroupByBox.BandLabelBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.alertsGridView.DisplayLayout.GroupByBox.ButtonBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance8.BackColor = System.Drawing.Color.Transparent;
            appearance8.FontData.Name = "Verdana";
            this.alertsGridView.DisplayLayout.GroupByBox.PromptAppearance = appearance8;
            this.alertsGridView.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.alertsGridView.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.alertsGridView.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;
            this.alertsGridView.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance9.BackColor = System.Drawing.Color.Transparent;
            this.alertsGridView.DisplayLayout.Override.CardAreaAppearance = appearance9;
            appearance10.BorderColor = System.Drawing.Color.Transparent;
            appearance10.FontData.Name = "Verdana";
            this.alertsGridView.DisplayLayout.Override.CellAppearance = appearance10;
            appearance11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance11.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance11.ImageBackground")));
            appearance11.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance11.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.alertsGridView.DisplayLayout.Override.CellButtonAppearance = appearance11;
            this.alertsGridView.DisplayLayout.Override.CellPadding = 4;
            appearance13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(251)))));
            this.alertsGridView.DisplayLayout.Override.FilterCellAppearance = appearance13;
            appearance14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance14.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance14.ImageBackground")));
            appearance14.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            this.alertsGridView.DisplayLayout.Override.FilterClearButtonAppearance = appearance14;
            appearance15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(251)))));
            appearance15.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
            this.alertsGridView.DisplayLayout.Override.FilterRowPromptAppearance = appearance15;
            this.alertsGridView.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(255)))));
            appearance16.BackColor2 = System.Drawing.Color.White;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance16.FontData.BoldAsString = "True";
            appearance16.FontData.Name = "Verdana";
            appearance16.FontData.SizeInPoints = 8F;
            appearance16.ForeColor = System.Drawing.Color.DimGray;
            appearance16.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance16.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance16.TextHAlignAsString = "Center";
            this.alertsGridView.DisplayLayout.Override.HeaderAppearance = appearance16;
            appearance17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.alertsGridView.DisplayLayout.Override.RowAlternateAppearance = appearance17;
            appearance18.BorderColor = System.Drawing.Color.LightGray;
            this.alertsGridView.DisplayLayout.Override.RowAppearance = appearance18;
            appearance19.BackColor = System.Drawing.Color.White;
            this.alertsGridView.DisplayLayout.Override.RowSelectorAppearance = appearance19;
            this.alertsGridView.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance20.BorderColor = System.Drawing.Color.Transparent;
            appearance20.ForeColor = System.Drawing.Color.Black;
            this.alertsGridView.DisplayLayout.Override.SelectedCellAppearance = appearance20;
            appearance21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(223)))));
            appearance21.BorderColor = System.Drawing.Color.Transparent;
            appearance21.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance21.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 4);
            appearance21.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.alertsGridView.DisplayLayout.Override.SelectedRowAppearance = appearance21;
            appearance22.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 4, 2, 4);
            appearance22.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.Appearance = appearance22;
            appearance23.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance23.ImageBackground")));
            appearance23.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(3, 2, 3, 2);
            scrollBarLook1.AppearanceHorizontal = appearance23;
            appearance24.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance24.ImageBackground")));
            appearance24.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 3, 2, 3);
            appearance24.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.AppearanceVertical = appearance24;
            appearance25.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance25.ImageBackground")));
            appearance25.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(0, 2, 0, 1);
            scrollBarLook1.TrackAppearanceHorizontal = appearance25;
            appearance26.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance26.ImageBackground")));
            appearance26.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 0, 1, 0);
            appearance26.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.TrackAppearanceVertical = appearance26;
            this.alertsGridView.DisplayLayout.ScrollBarLook = scrollBarLook1;
            this.alertsGridView.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.alertsGridView.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.alertsGridView.Location = new System.Drawing.Point(9, 86);
            this.alertsGridView.Name = "alertsGridView";
            this.alertsGridView.Size = new System.Drawing.Size(940, 321);
            this.alertsGridView.TabIndex = 10;
            this.alertsGridView.Text = "Grid Caption Area";
            this.alertsGridView.UseAppStyling = false;
            this.alertsGridView.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            // 
            // operationsMenuStrip
            // 
            this.operationsMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportDataToolStripMenuItem});
            this.operationsMenuStrip.Name = "OSMenuStrip";
            this.operationsMenuStrip.Size = new System.Drawing.Size(169, 48);
            // 
            // exportDataToolStripMenuItem
            // 
            this.exportDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excelToolStripMenuItem,
            this.portableDocumentFormatPDFToolStripMenuItem,
            this.xMLPaperSpecificationXPSToolStripMenuItem});
            this.exportDataToolStripMenuItem.Image = global::Layton.AuditWizard.Common.Properties.Resources.export_16;
            this.exportDataToolStripMenuItem.Name = "exportDataToolStripMenuItem";
            this.exportDataToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.exportDataToolStripMenuItem.Text = "&Export Data to...";
            // 
            // excelToolStripMenuItem
            // 
            this.excelToolStripMenuItem.Image = global::Layton.AuditWizard.Common.Properties.Resources.excel_16;
            this.excelToolStripMenuItem.Name = "excelToolStripMenuItem";
            this.excelToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.excelToolStripMenuItem.Text = "&Excel";
            this.excelToolStripMenuItem.Click += new System.EventHandler(this.ExportXlsToolStripMenuItem_Click);
            // 
            // portableDocumentFormatPDFToolStripMenuItem
            // 
            this.portableDocumentFormatPDFToolStripMenuItem.Image = global::Layton.AuditWizard.Common.Properties.Resources.pdf;
            this.portableDocumentFormatPDFToolStripMenuItem.Name = "portableDocumentFormatPDFToolStripMenuItem";
            this.portableDocumentFormatPDFToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.portableDocumentFormatPDFToolStripMenuItem.Text = "Portable Document Format (PDF)";
            this.portableDocumentFormatPDFToolStripMenuItem.Click += new System.EventHandler(this.ExportPDFToolStripMenuItem_Click);
            // 
            // xMLPaperSpecificationXPSToolStripMenuItem
            // 
            this.xMLPaperSpecificationXPSToolStripMenuItem.Image = global::Layton.AuditWizard.Common.Properties.Resources.xps_16;
            this.xMLPaperSpecificationXPSToolStripMenuItem.Name = "xMLPaperSpecificationXPSToolStripMenuItem";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.xMLPaperSpecificationXPSToolStripMenuItem.Text = "XML Paper Specification (XPS)";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Click += new System.EventHandler(this.ExportXPSToolStripMenuItem_Click);
            // 
            // alertsDataSet
            // 
            this.alertsDataSet.DataSetName = "NewDataSet";
            this.alertsDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.alertsTable});
            // 
            // alertsTable
            // 
            this.alertsTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn7,
            this.dataColumn9,
            this.dataColumn10});
            this.alertsTable.TableName = "alertsTable";
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Date of Alert";
            this.dataColumn2.ColumnName = "Date";
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Type";
            this.dataColumn3.ColumnName = "type";
            // 
            // dataColumn4
            // 
            this.dataColumn4.ColumnName = "Category";
            // 
            // dataColumn5
            // 
            this.dataColumn5.ColumnName = "Item";
            // 
            // dataColumn6
            // 
            this.dataColumn6.Caption = "Previous Value";
            this.dataColumn6.ColumnName = "Previous Value";
            // 
            // dataColumn7
            // 
            this.dataColumn7.ColumnName = "New Value";
            // 
            // dataColumn9
            // 
            this.dataColumn9.ColumnName = "Alert Name";
            // 
            // dataColumn10
            // 
            this.dataColumn10.ColumnName = "Asset Name";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(14, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(936, 27);
            this.label1.TabIndex = 11;
            this.label1.Text = "This view shows all of the Alerts which have been generated by AuditWizard since " +
                "the date and time specified.";
            // 
            // bnOK
            // 
            this.bnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnOK.Location = new System.Drawing.Point(758, 437);
            this.bnOK.Name = "bnOK";
            this.bnOK.Size = new System.Drawing.Size(92, 24);
            this.bnOK.TabIndex = 23;
            this.bnOK.Text = "&OK";
            this.bnOK.UseVisualStyleBackColor = true;
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(857, 437);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(92, 24);
            this.bnCancel.TabIndex = 24;
            this.bnCancel.Text = "&Cancel";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // timerRefresh
            // 
            this.timerRefresh.Interval = 5000;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(14, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Start Date\\Time:";
            // 
            // cbShowNewAtStartup
            // 
            this.cbShowNewAtStartup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbShowNewAtStartup.AutoSize = true;
            this.cbShowNewAtStartup.Checked = true;
            this.cbShowNewAtStartup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowNewAtStartup.Location = new System.Drawing.Point(9, 442);
            this.cbShowNewAtStartup.Name = "cbShowNewAtStartup";
            this.cbShowNewAtStartup.Size = new System.Drawing.Size(261, 17);
            this.cbShowNewAtStartup.TabIndex = 29;
            this.cbShowNewAtStartup.Text = "Show New Alerts on Entry to AuditWizard";
            this.cbShowNewAtStartup.UseVisualStyleBackColor = true;
            this.cbShowNewAtStartup.CheckedChanged += new System.EventHandler(this.cbAlwaysShowLog_CheckedChanged);
            // 
            // dtpStartDateTime
            // 
            this.dtpStartDateTime.Location = new System.Drawing.Point(134, 48);
            this.dtpStartDateTime.MaskInput = "{date} {time}";
            this.dtpStartDateTime.Name = "dtpStartDateTime";
            this.dtpStartDateTime.Size = new System.Drawing.Size(162, 22);
            this.dtpStartDateTime.TabIndex = 30;
            this.dtpStartDateTime.ValueChanged += new System.EventHandler(this.dtpStartDateTime_ValueChanged);
            // 
            // bnDelete
            // 
            this.bnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnDelete.Location = new System.Drawing.Point(633, 437);
            this.bnDelete.Name = "bnDelete";
            this.bnDelete.Size = new System.Drawing.Size(92, 24);
            this.bnDelete.TabIndex = 31;
            this.bnDelete.Text = "&Delete";
            this.bnDelete.UseVisualStyleBackColor = true;
            this.bnDelete.Click += new System.EventHandler(this.bnDelete_Click);
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "DataObject";
            this.dataColumn1.DataType = typeof(object);
            // 
            // FormAlertLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.ClientSize = new System.Drawing.Size(964, 587);
            this.Controls.Add(this.bnDelete);
            this.Controls.Add(this.dtpStartDateTime);
            this.Controls.Add(this.cbShowNewAtStartup);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bnOK);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.alertsGridView);
            this.Name = "FormAlertLog";
            this.Text = "AuditWizard Alerts Log";
            this.Load += new System.EventHandler(this.FormAlertLog_Load);
            this.Controls.SetChildIndex(this.footerPictureBox, 0);
            this.Controls.SetChildIndex(this.alertsGridView, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.bnCancel, 0);
            this.Controls.SetChildIndex(this.bnOK, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.cbShowNewAtStartup, 0);
            this.Controls.SetChildIndex(this.dtpStartDateTime, 0);
            this.Controls.SetChildIndex(this.bnDelete, 0);
            ((System.ComponentModel.ISupportInitialize)(this.footerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsGridView)).EndInit();
            this.operationsMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alertsDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.alertsTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpStartDateTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Infragistics.Win.UltraWinGrid.UltraGrid alertsGridView;
		private System.Data.DataSet alertsDataSet;
		private System.Data.DataTable alertsTable;
		private System.Data.DataColumn dataColumn1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button bnOK;
		private System.Windows.Forms.Button bnCancel;
		private System.Windows.Forms.Timer timerRefresh;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox cbShowNewAtStartup;
		private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtpStartDateTime;
		private System.Windows.Forms.ContextMenuStrip operationsMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem exportDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem excelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem portableDocumentFormatPDFToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem xMLPaperSpecificationXPSToolStripMenuItem;
		private System.Data.DataColumn dataColumn2;
		private System.Data.DataColumn dataColumn3;
		private System.Data.DataColumn dataColumn4;
		private System.Data.DataColumn dataColumn5;
		private System.Data.DataColumn dataColumn6;
		private System.Data.DataColumn dataColumn7;
		private System.Data.DataColumn dataColumn9;
		private System.Data.DataColumn dataColumn10;
		private System.Windows.Forms.Button bnDelete;
	}
}
