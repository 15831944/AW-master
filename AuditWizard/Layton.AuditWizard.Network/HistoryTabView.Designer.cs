namespace Layton.AuditWizard.Network
{
    partial class HistoryTabView
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryTabView));
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("historyData", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DataObject");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Date", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Operation");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("User");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.headerGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.headerLabel = new Infragistics.Win.Misc.UltraLabel();
            this.historyGridView = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.groupMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portableDocumentFormatPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLPaperSpecificationXPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.historyDataSet = new System.Data.DataSet();
            this.historyDataTable = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).BeginInit();
            this.headerGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.historyGridView)).BeginInit();
            this.groupMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.historyDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.historyDataTable)).BeginInit();
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
            this.mainSplitContainer.Panel2.Controls.Add(this.historyGridView);
            this.mainSplitContainer.Size = new System.Drawing.Size(1032, 482);
            this.mainSplitContainer.SplitterDistance = 80;
            this.mainSplitContainer.SplitterWidth = 1;
            this.mainSplitContainer.TabIndex = 2;
            // 
            // headerGroupBox
            // 
            this.headerGroupBox.Controls.Add(this.headerLabel);
            this.headerGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerGroupBox.Location = new System.Drawing.Point(0, 0);
            this.headerGroupBox.Name = "headerGroupBox";
            this.headerGroupBox.Size = new System.Drawing.Size(1032, 80);
            this.headerGroupBox.TabIndex = 3;
            this.headerGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            this.headerGroupBox.SizeChanged += new System.EventHandler(this.headerGroupBox_SizeChanged);
            // 
            // headerLabel
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            appearance1.Image = ((object)(resources.GetObject("appearance1.Image")));
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.headerLabel.Appearance = appearance1;
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ImageSize = new System.Drawing.Size(72, 72);
            this.headerLabel.Location = new System.Drawing.Point(7, 5);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.headerLabel.Size = new System.Drawing.Size(276, 72);
            this.headerLabel.TabIndex = 3;
            this.headerLabel.Text = "Asset Change History";
            // 
            // historyGridView
            // 
            this.historyGridView.AllowDrop = true;
            this.historyGridView.ContextMenuStrip = this.groupMenuStrip;
            this.historyGridView.DataSource = this.historyDataSet;
            appearance15.BackColor = System.Drawing.Color.White;
            appearance15.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            appearance15.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.historyGridView.DisplayLayout.AddNewBox.Appearance = appearance15;
            this.historyGridView.DisplayLayout.AddNewBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance16.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance16.ImageBackground")));
            appearance16.ImageBackgroundAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            appearance16.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance16.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.historyGridView.DisplayLayout.AddNewBox.ButtonAppearance = appearance16;
            this.historyGridView.DisplayLayout.AddNewBox.ButtonConnectorColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.historyGridView.DisplayLayout.AddNewBox.ButtonStyle = Infragistics.Win.UIElementButtonStyle.FlatBorderless;
            appearance17.BackColor = System.Drawing.Color.White;
            this.historyGridView.DisplayLayout.Appearance = appearance17;
            this.historyGridView.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.Format = "MM/dd/yyyy hh:mm:ss tt";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTimeWithoutDropDown;
            ultraGridColumn2.Width = 248;
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn3.Width = 557;
            ultraGridColumn4.Header.VisiblePosition = 2;
            ultraGridColumn4.Width = 206;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4});
            ultraGridBand1.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            ultraGridBand1.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            ultraGridBand1.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance2.FontData.SizeInPoints = 8F;
            ultraGridBand1.Override.HeaderAppearance = appearance2;
            appearance3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            appearance3.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            ultraGridBand1.Override.RowAlternateAppearance = appearance3;
            this.historyGridView.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            appearance20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance20.FontData.Name = "Trebuchet MS";
            appearance20.FontData.SizeInPoints = 9F;
            appearance20.ForeColor = System.Drawing.Color.White;
            appearance20.TextHAlignAsString = "Right";
            this.historyGridView.DisplayLayout.CaptionAppearance = appearance20;
            this.historyGridView.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.historyGridView.DisplayLayout.FixedHeaderOffImage = ((System.Drawing.Image)(resources.GetObject("historyGridView.DisplayLayout.FixedHeaderOffImage")));
            this.historyGridView.DisplayLayout.FixedHeaderOnImage = ((System.Drawing.Image)(resources.GetObject("historyGridView.DisplayLayout.FixedHeaderOnImage")));
            this.historyGridView.DisplayLayout.FixedRowOffImage = ((System.Drawing.Image)(resources.GetObject("historyGridView.DisplayLayout.FixedRowOffImage")));
            this.historyGridView.DisplayLayout.FixedRowOnImage = ((System.Drawing.Image)(resources.GetObject("historyGridView.DisplayLayout.FixedRowOnImage")));
            appearance21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            appearance21.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(185)))), ((int)(((byte)(235)))));
            appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance21.FontData.BoldAsString = "True";
            appearance21.FontData.Name = "Verdana";
            appearance21.FontData.SizeInPoints = 10F;
            appearance21.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(127)))), ((int)(((byte)(177)))));
            this.historyGridView.DisplayLayout.GroupByBox.Appearance = appearance21;
            this.historyGridView.DisplayLayout.GroupByBox.BandLabelBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.historyGridView.DisplayLayout.GroupByBox.ButtonBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance22.BackColor = System.Drawing.Color.Transparent;
            appearance22.FontData.Name = "Verdana";
            this.historyGridView.DisplayLayout.GroupByBox.PromptAppearance = appearance22;
            this.historyGridView.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.historyGridView.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.historyGridView.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;
            this.historyGridView.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance23.BackColor = System.Drawing.Color.Transparent;
            this.historyGridView.DisplayLayout.Override.CardAreaAppearance = appearance23;
            appearance24.BorderColor = System.Drawing.Color.Transparent;
            appearance24.FontData.Name = "Verdana";
            this.historyGridView.DisplayLayout.Override.CellAppearance = appearance24;
            appearance25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance25.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance25.ImageBackground")));
            appearance25.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance25.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.historyGridView.DisplayLayout.Override.CellButtonAppearance = appearance25;
            this.historyGridView.DisplayLayout.Override.CellPadding = 4;
            appearance26.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(251)))));
            this.historyGridView.DisplayLayout.Override.FilterCellAppearance = appearance26;
            appearance27.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance27.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance27.ImageBackground")));
            appearance27.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            this.historyGridView.DisplayLayout.Override.FilterClearButtonAppearance = appearance27;
            appearance28.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(251)))));
            appearance28.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
            this.historyGridView.DisplayLayout.Override.FilterRowPromptAppearance = appearance28;
            this.historyGridView.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance29.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(255)))));
            appearance29.BackColor2 = System.Drawing.Color.White;
            appearance29.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance29.FontData.BoldAsString = "True";
            appearance29.FontData.Name = "Verdana";
            appearance29.FontData.SizeInPoints = 8F;
            appearance29.ForeColor = System.Drawing.Color.DimGray;
            appearance29.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance29.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance29.TextHAlignAsString = "Center";
            this.historyGridView.DisplayLayout.Override.HeaderAppearance = appearance29;
            appearance30.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.historyGridView.DisplayLayout.Override.RowAlternateAppearance = appearance30;
            appearance31.BorderColor = System.Drawing.Color.LightGray;
            this.historyGridView.DisplayLayout.Override.RowAppearance = appearance31;
            appearance32.BackColor = System.Drawing.Color.White;
            this.historyGridView.DisplayLayout.Override.RowSelectorAppearance = appearance32;
            this.historyGridView.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance33.BorderColor = System.Drawing.Color.Transparent;
            appearance33.ForeColor = System.Drawing.Color.Black;
            this.historyGridView.DisplayLayout.Override.SelectedCellAppearance = appearance33;
            appearance44.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(223)))));
            appearance44.BorderColor = System.Drawing.Color.Transparent;
            appearance44.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance44.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 4);
            appearance44.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.historyGridView.DisplayLayout.Override.SelectedRowAppearance = appearance44;
            appearance45.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 4, 2, 4);
            appearance45.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.Appearance = appearance45;
            appearance46.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance46.ImageBackground")));
            appearance46.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(3, 2, 3, 2);
            scrollBarLook1.AppearanceHorizontal = appearance46;
            appearance47.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance47.ImageBackground")));
            appearance47.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 3, 2, 3);
            appearance47.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.AppearanceVertical = appearance47;
            appearance48.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance48.ImageBackground")));
            appearance48.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(0, 2, 0, 1);
            scrollBarLook1.TrackAppearanceHorizontal = appearance48;
            appearance49.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance49.ImageBackground")));
            appearance49.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 0, 1, 0);
            appearance49.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.TrackAppearanceVertical = appearance49;
            this.historyGridView.DisplayLayout.ScrollBarLook = scrollBarLook1;
            this.historyGridView.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.historyGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyGridView.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.historyGridView.Location = new System.Drawing.Point(0, 0);
            this.historyGridView.Name = "historyGridView";
            this.historyGridView.Size = new System.Drawing.Size(1032, 401);
            this.historyGridView.TabIndex = 0;
            this.historyGridView.Text = "Grid Caption Area";
            this.historyGridView.UseAppStyling = false;
            this.historyGridView.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.historyGridView.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.historyGridView_InitializeRow);
            // 
            // groupMenuStrip
            // 
            this.groupMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportDataToolStripMenuItem});
            this.groupMenuStrip.Name = "applicationsMenuStrip";
            this.groupMenuStrip.Size = new System.Drawing.Size(169, 26);
            // 
            // exportDataToolStripMenuItem
            // 
            this.exportDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excelToolStripMenuItem,
            this.portableDocumentFormatPDFToolStripMenuItem,
            this.xMLPaperSpecificationXPSToolStripMenuItem});
            this.exportDataToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.export_16;
            this.exportDataToolStripMenuItem.Name = "exportDataToolStripMenuItem";
            this.exportDataToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.exportDataToolStripMenuItem.Text = "&Export Data to...";
            // 
            // excelToolStripMenuItem
            // 
            this.excelToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.excel_32;
            this.excelToolStripMenuItem.Name = "excelToolStripMenuItem";
            this.excelToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.excelToolStripMenuItem.Text = "&Excel";
            this.excelToolStripMenuItem.Click += new System.EventHandler(this.ExportXlsToolStripMenuItem_Click);
            // 
            // portableDocumentFormatPDFToolStripMenuItem
            // 
            this.portableDocumentFormatPDFToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.pdf_32;
            this.portableDocumentFormatPDFToolStripMenuItem.Name = "portableDocumentFormatPDFToolStripMenuItem";
            this.portableDocumentFormatPDFToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.portableDocumentFormatPDFToolStripMenuItem.Text = "Portable Document Format (PDF)";
            this.portableDocumentFormatPDFToolStripMenuItem.Click += new System.EventHandler(this.ExportPDFToolStripMenuItem_Click);
            // 
            // xMLPaperSpecificationXPSToolStripMenuItem
            // 
            this.xMLPaperSpecificationXPSToolStripMenuItem.Image = global::Layton.AuditWizard.Network.Properties.Resources.xps_32;
            this.xMLPaperSpecificationXPSToolStripMenuItem.Name = "xMLPaperSpecificationXPSToolStripMenuItem";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.xMLPaperSpecificationXPSToolStripMenuItem.Text = "XML Paper Specification (XPS)";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Click += new System.EventHandler(this.ExportXPSToolStripMenuItem_Click);
            // 
            // historyDataSet
            // 
            this.historyDataSet.DataSetName = "HistoryDataSet";
            this.historyDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.historyDataTable});
            // 
            // historyDataTable
            // 
            this.historyDataTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn4,
            this.dataColumn5});
            this.historyDataTable.MinimumCapacity = 0;
            this.historyDataTable.TableName = "historyData";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "DataObject";
            this.dataColumn1.DataType = typeof(object);
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "Date";
            this.dataColumn2.DataType = typeof(System.DateTime);
            // 
            // dataColumn4
            // 
            this.dataColumn4.ColumnName = "Operation";
            // 
            // dataColumn5
            // 
            this.dataColumn5.ColumnName = "User";
            // 
            // HistoryTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainSplitContainer);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "HistoryTabView";
            this.Size = new System.Drawing.Size(1032, 482);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            this.mainSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).EndInit();
            this.headerGroupBox.ResumeLayout(false);
            this.headerGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.historyGridView)).EndInit();
            this.groupMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.historyDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.historyDataTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private Infragistics.Win.Misc.UltraGroupBox headerGroupBox;
		private Infragistics.Win.Misc.UltraLabel headerLabel;
        private System.Data.DataSet historyDataSet;
		private System.Data.DataTable historyDataTable;
		private Infragistics.Win.UltraWinGrid.UltraGrid historyGridView;
		private System.Windows.Forms.ContextMenuStrip groupMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem exportDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem excelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem portableDocumentFormatPDFToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem xMLPaperSpecificationXPSToolStripMenuItem;
		private System.Data.DataColumn dataColumn1;
		private System.Data.DataColumn dataColumn2;
		private System.Data.DataColumn dataColumn4;
		private System.Data.DataColumn dataColumn5;

    }
}
