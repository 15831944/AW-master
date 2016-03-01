namespace Layton.AuditWizard.Applications
{
    partial class InstancesTabView
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
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstancesTabView));
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Applications", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Asset");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Version");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Serial Number");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CD Key");
            Infragistics.Win.Appearance appearance52 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
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
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.headerGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.headerLabel = new Infragistics.Win.Misc.UltraLabel();
            this.instancesGridView = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portableDocumentFormatPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLPaperSpecificationXPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instancesDataSet = new System.Data.DataSet();
            this.applicationsDataTable = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).BeginInit();
            this.headerGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.instancesGridView)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.instancesDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.applicationsDataTable)).BeginInit();
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
            this.mainSplitContainer.Panel2.Controls.Add(this.instancesGridView);
            this.mainSplitContainer.Size = new System.Drawing.Size(884, 364);
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
            this.headerGroupBox.Size = new System.Drawing.Size(884, 80);
            this.headerGroupBox.TabIndex = 3;
            this.headerGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
            // 
            // headerLabel
            // 
            appearance4.BackColor = System.Drawing.Color.Transparent;
            appearance4.Image = global::Layton.AuditWizard.Applications.Properties.Resources.application_instance_72;
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
            this.headerLabel.Size = new System.Drawing.Size(269, 72);
            this.headerLabel.TabIndex = 3;
            this.headerLabel.Text = "Application Instance";
            // 
            // instancesGridView
            // 
            this.instancesGridView.ContextMenuStrip = this.contextMenuStrip;
            this.instancesGridView.DataSource = this.instancesDataSet;
            appearance3.BackColor = System.Drawing.Color.White;
            appearance3.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.instancesGridView.DisplayLayout.AddNewBox.Appearance = appearance3;
            this.instancesGridView.DisplayLayout.AddNewBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance5.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance5.ImageBackground")));
            appearance5.ImageBackgroundAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
            appearance5.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance5.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.instancesGridView.DisplayLayout.AddNewBox.ButtonAppearance = appearance5;
            this.instancesGridView.DisplayLayout.AddNewBox.ButtonConnectorColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            this.instancesGridView.DisplayLayout.AddNewBox.ButtonStyle = Infragistics.Win.UIElementButtonStyle.FlatBorderless;
            appearance6.BackColor = System.Drawing.Color.White;
            this.instancesGridView.DisplayLayout.Appearance = appearance6;
            this.instancesGridView.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 212;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 212;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 229;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Width = 210;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4});
            ultraGridBand1.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            ultraGridBand1.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            ultraGridBand1.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance52.FontData.SizeInPoints = 8F;
            ultraGridBand1.Override.HeaderAppearance = appearance52;
            appearance53.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            appearance53.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            appearance53.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            ultraGridBand1.Override.RowAlternateAppearance = appearance53;
            this.instancesGridView.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            appearance9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance9.FontData.Name = "Trebuchet MS";
            appearance9.FontData.SizeInPoints = 9F;
            appearance9.ForeColor = System.Drawing.Color.White;
            appearance9.TextHAlignAsString = "Right";
            this.instancesGridView.DisplayLayout.CaptionAppearance = appearance9;
            this.instancesGridView.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.instancesGridView.DisplayLayout.FixedHeaderOffImage = ((System.Drawing.Image)(resources.GetObject("instancesGridView.DisplayLayout.FixedHeaderOffImage")));
            this.instancesGridView.DisplayLayout.FixedHeaderOnImage = ((System.Drawing.Image)(resources.GetObject("instancesGridView.DisplayLayout.FixedHeaderOnImage")));
            this.instancesGridView.DisplayLayout.FixedRowOffImage = ((System.Drawing.Image)(resources.GetObject("instancesGridView.DisplayLayout.FixedRowOffImage")));
            this.instancesGridView.DisplayLayout.FixedRowOnImage = ((System.Drawing.Image)(resources.GetObject("instancesGridView.DisplayLayout.FixedRowOnImage")));
            appearance10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
            appearance10.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(185)))), ((int)(((byte)(235)))));
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance10.FontData.BoldAsString = "True";
            appearance10.FontData.Name = "Verdana";
            appearance10.FontData.SizeInPoints = 10F;
            appearance10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(127)))), ((int)(((byte)(177)))));
            this.instancesGridView.DisplayLayout.GroupByBox.Appearance = appearance10;
            this.instancesGridView.DisplayLayout.GroupByBox.BandLabelBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.instancesGridView.DisplayLayout.GroupByBox.ButtonBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            appearance12.BackColor = System.Drawing.Color.Transparent;
            appearance12.FontData.Name = "Verdana";
            this.instancesGridView.DisplayLayout.GroupByBox.PromptAppearance = appearance12;
            this.instancesGridView.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.instancesGridView.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.instancesGridView.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;
            this.instancesGridView.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance13.BackColor = System.Drawing.Color.Transparent;
            this.instancesGridView.DisplayLayout.Override.CardAreaAppearance = appearance13;
            appearance14.BorderColor = System.Drawing.Color.Transparent;
            appearance14.FontData.Name = "Verdana";
            this.instancesGridView.DisplayLayout.Override.CellAppearance = appearance14;
            appearance15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance15.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance15.ImageBackground")));
            appearance15.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            appearance15.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.instancesGridView.DisplayLayout.Override.CellButtonAppearance = appearance15;
            this.instancesGridView.DisplayLayout.Override.CellPadding = 4;
            appearance16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(251)))));
            this.instancesGridView.DisplayLayout.Override.FilterCellAppearance = appearance16;
            appearance17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance17.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance17.ImageBackground")));
            appearance17.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(6, 3, 6, 3);
            this.instancesGridView.DisplayLayout.Override.FilterClearButtonAppearance = appearance17;
            appearance18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(251)))), ((int)(((byte)(251)))));
            appearance18.BackColorAlpha = Infragistics.Win.Alpha.Opaque;
            this.instancesGridView.DisplayLayout.Override.FilterRowPromptAppearance = appearance18;
            this.instancesGridView.DisplayLayout.Override.FilterUIType = Infragistics.Win.UltraWinGrid.FilterUIType.FilterRow;
            appearance19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(236)))), ((int)(((byte)(255)))));
            appearance19.BackColor2 = System.Drawing.Color.White;
            appearance19.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance19.FontData.BoldAsString = "True";
            appearance19.FontData.Name = "Verdana";
            appearance19.FontData.SizeInPoints = 8F;
            appearance19.ForeColor = System.Drawing.Color.DimGray;
            appearance19.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance19.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance19.TextHAlignAsString = "Center";
            this.instancesGridView.DisplayLayout.Override.HeaderAppearance = appearance19;
            appearance20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.instancesGridView.DisplayLayout.Override.RowAlternateAppearance = appearance20;
            appearance21.BorderColor = System.Drawing.Color.LightGray;
            this.instancesGridView.DisplayLayout.Override.RowAppearance = appearance21;
            appearance22.BackColor = System.Drawing.Color.White;
            this.instancesGridView.DisplayLayout.Override.RowSelectorAppearance = appearance22;
            this.instancesGridView.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance23.BorderColor = System.Drawing.Color.Transparent;
            appearance23.ForeColor = System.Drawing.Color.Black;
            this.instancesGridView.DisplayLayout.Override.SelectedCellAppearance = appearance23;
            appearance24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(243)))), ((int)(((byte)(223)))));
            appearance24.BorderColor = System.Drawing.Color.Transparent;
            appearance24.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(27)))), ((int)(((byte)(85)))));
            appearance24.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(1, 1, 1, 4);
            appearance24.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            this.instancesGridView.DisplayLayout.Override.SelectedRowAppearance = appearance24;
            appearance25.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 4, 2, 4);
            appearance25.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.Appearance = appearance25;
            appearance26.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance26.ImageBackground")));
            appearance26.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(3, 2, 3, 2);
            scrollBarLook1.AppearanceHorizontal = appearance26;
            appearance27.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance27.ImageBackground")));
            appearance27.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 3, 2, 3);
            appearance27.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.AppearanceVertical = appearance27;
            appearance50.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance50.ImageBackground")));
            appearance50.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(0, 2, 0, 1);
            scrollBarLook1.TrackAppearanceHorizontal = appearance50;
            appearance51.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance51.ImageBackground")));
            appearance51.ImageBackgroundStretchMargins = new Infragistics.Win.ImageBackgroundStretchMargins(2, 0, 1, 0);
            appearance51.ImageBackgroundStyle = Infragistics.Win.ImageBackgroundStyle.Stretched;
            scrollBarLook1.TrackAppearanceVertical = appearance51;
            this.instancesGridView.DisplayLayout.ScrollBarLook = scrollBarLook1;
            this.instancesGridView.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.instancesGridView.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.instancesGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.instancesGridView.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.instancesGridView.Location = new System.Drawing.Point(0, 0);
            this.instancesGridView.Name = "instancesGridView";
            this.instancesGridView.Size = new System.Drawing.Size(884, 283);
            this.instancesGridView.TabIndex = 2;
            this.instancesGridView.Text = "Grid Caption Area";
            this.instancesGridView.UseAppStyling = false;
            this.instancesGridView.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportDataToolStripMenuItem});
            this.contextMenuStrip.Name = "applicationsMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(169, 26);
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
            // instancesDataSet
            // 
            this.instancesDataSet.DataSetName = "InstancesDataSet";
            this.instancesDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.applicationsDataTable});
            // 
            // applicationsDataTable
            // 
            this.applicationsDataTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4});
            this.applicationsDataTable.MinimumCapacity = 0;
            this.applicationsDataTable.TableName = "Applications";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "Asset";
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "Version";
            // 
            // dataColumn3
            // 
            this.dataColumn3.ColumnName = "Serial Number";
            // 
            // dataColumn4
            // 
            this.dataColumn4.ColumnName = "CD Key";
            // 
            // InstancesTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainSplitContainer);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "InstancesTabView";
            this.Size = new System.Drawing.Size(884, 364);
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            this.mainSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.headerGroupBox)).EndInit();
            this.headerGroupBox.ResumeLayout(false);
            this.headerGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.instancesGridView)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.instancesDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.applicationsDataTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.SplitContainer mainSplitContainer;
		private Infragistics.Win.Misc.UltraGroupBox headerGroupBox;
		private Infragistics.Win.Misc.UltraLabel headerLabel;
		private System.Data.DataSet instancesDataSet;
		private System.Data.DataTable applicationsDataTable;
		private Infragistics.Win.UltraWinGrid.UltraGrid instancesGridView;
		private System.Data.DataColumn dataColumn1;
		private System.Data.DataColumn dataColumn2;
		private System.Data.DataColumn dataColumn3;
		private System.Data.DataColumn dataColumn4;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem exportDataToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem excelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem portableDocumentFormatPDFToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem xMLPaperSpecificationXPSToolStripMenuItem;

	}
}
