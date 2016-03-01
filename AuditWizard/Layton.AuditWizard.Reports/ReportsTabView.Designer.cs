namespace Layton.AuditWizard.Reports
{
    partial class ReportsTabView
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
            this.cmbChartType = new System.Windows.Forms.ComboBox();
            this.contextMenuStripCloseTab = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllTabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resultsMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portableDocumentFormatPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLPaperSpecificationXPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.printPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.tabControlReport = new System.Windows.Forms.TabControl();
            this.ultraDockManager = new Infragistics.Win.UltraWinDock.UltraDockManager(this.components);
            this._ReportsTabViewUnpinnedTabAreaLeft = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._ReportsTabViewUnpinnedTabAreaRight = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._ReportsTabViewUnpinnedTabAreaTop = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._ReportsTabViewUnpinnedTabAreaBottom = new Infragistics.Win.UltraWinDock.UnpinnedTabArea();
            this._ReportsTabViewAutoHideControl = new Infragistics.Win.UltraWinDock.AutoHideControl();
            this.contextMenuStripCloseTab.SuspendLayout();
            this.resultsMenuStrip.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDockManager)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbChartType
            // 
            this.cmbChartType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbChartType.Location = new System.Drawing.Point(0, 0);
            this.cmbChartType.Name = "cmbChartType";
            this.cmbChartType.Size = new System.Drawing.Size(121, 21);
            this.cmbChartType.TabIndex = 0;
            // 
            // contextMenuStripCloseTab
            // 
            this.contextMenuStripCloseTab.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem,
            this.closeAllToolStripMenuItem,
            this.closeAllTabsToolStripMenuItem});
            this.contextMenuStripCloseTab.Name = "contextMenuStripCloseTab";
            this.contextMenuStripCloseTab.Size = new System.Drawing.Size(167, 70);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.closeAllToolStripMenuItem.Text = "Close All But This";
            this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.closeAllToolStripMenuItem_Click);
            // 
            // closeAllTabsToolStripMenuItem
            // 
            this.closeAllTabsToolStripMenuItem.Name = "closeAllTabsToolStripMenuItem";
            this.closeAllTabsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.closeAllTabsToolStripMenuItem.Text = "Close All";
            this.closeAllTabsToolStripMenuItem.Click += new System.EventHandler(this.closeAllTabsToolStripMenuItem_Click);
            // 
            // resultsMenuStrip
            // 
            this.resultsMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportDataToolStripMenuItem});
            this.resultsMenuStrip.Name = "applicationsMenuStrip";
            this.resultsMenuStrip.Size = new System.Drawing.Size(169, 48);
            // 
            // exportDataToolStripMenuItem
            // 
            this.exportDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excelToolStripMenuItem,
            this.portableDocumentFormatPDFToolStripMenuItem,
            this.xMLPaperSpecificationXPSToolStripMenuItem});
            this.exportDataToolStripMenuItem.Image = global::Layton.AuditWizard.Reports.Properties.Resources.export_16;
            this.exportDataToolStripMenuItem.Name = "exportDataToolStripMenuItem";
            this.exportDataToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.exportDataToolStripMenuItem.Text = "&Export Data to...";
            // 
            // excelToolStripMenuItem
            // 
            this.excelToolStripMenuItem.Image = global::Layton.AuditWizard.Reports.Properties.Resources.excel_32;
            this.excelToolStripMenuItem.Name = "excelToolStripMenuItem";
            this.excelToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.excelToolStripMenuItem.Text = "&Excel";
            this.excelToolStripMenuItem.Click += new System.EventHandler(this.ExportXlsToolStripMenuItem_Click);
            // 
            // portableDocumentFormatPDFToolStripMenuItem
            // 
            this.portableDocumentFormatPDFToolStripMenuItem.Image = global::Layton.AuditWizard.Reports.Properties.Resources.pdf_32;
            this.portableDocumentFormatPDFToolStripMenuItem.Name = "portableDocumentFormatPDFToolStripMenuItem";
            this.portableDocumentFormatPDFToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.portableDocumentFormatPDFToolStripMenuItem.Text = "Portable Document Format (PDF)";
            this.portableDocumentFormatPDFToolStripMenuItem.Click += new System.EventHandler(this.ExportPDFToolStripMenuItem_Click);
            // 
            // xMLPaperSpecificationXPSToolStripMenuItem
            // 
            this.xMLPaperSpecificationXPSToolStripMenuItem.Image = global::Layton.AuditWizard.Reports.Properties.Resources.xps_32;
            this.xMLPaperSpecificationXPSToolStripMenuItem.Name = "xMLPaperSpecificationXPSToolStripMenuItem";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.xMLPaperSpecificationXPSToolStripMenuItem.Text = "XML Paper Specification (XPS)";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Click += new System.EventHandler(this.ExportXPSToolStripMenuItem_Click);
            // 
            // gridPrintDocument
            // 
            this.gridPrintDocument.DocumentName = "Detailed Report";
            // 
            // printPreviewDialog
            // 
            this.printPreviewDialog.Document = this.gridPrintDocument;
            this.printPreviewDialog.Name = "gridPrintPreviewDialog";
            this.printPreviewDialog.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2007;
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 24);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.mainSplitContainer.Panel1Collapsed = true;
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.tabControlReport);
            this.mainSplitContainer.Size = new System.Drawing.Size(1105, 357);
            this.mainSplitContainer.SplitterDistance = 80;
            this.mainSplitContainer.SplitterWidth = 1;
            this.mainSplitContainer.TabIndex = 4;
            // 
            // tabControlReport
            // 
            this.tabControlReport.ContextMenuStrip = this.contextMenuStripCloseTab;
            this.tabControlReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlReport.Location = new System.Drawing.Point(0, 0);
            this.tabControlReport.Multiline = true;
            this.tabControlReport.Name = "tabControlReport";
            this.tabControlReport.SelectedIndex = 0;
            this.tabControlReport.Size = new System.Drawing.Size(1105, 357);
            this.tabControlReport.TabIndex = 3;
            // 
            // ultraDockManager
            // 
            this.ultraDockManager.HostControl = this;
            // 
            // _ReportsTabViewUnpinnedTabAreaLeft
            // 
            this._ReportsTabViewUnpinnedTabAreaLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this._ReportsTabViewUnpinnedTabAreaLeft.Font = new System.Drawing.Font("Verdana", 8.25F);
            this._ReportsTabViewUnpinnedTabAreaLeft.Location = new System.Drawing.Point(0, 24);
            this._ReportsTabViewUnpinnedTabAreaLeft.Name = "_ReportsTabViewUnpinnedTabAreaLeft";
            this._ReportsTabViewUnpinnedTabAreaLeft.Owner = this.ultraDockManager;
            this._ReportsTabViewUnpinnedTabAreaLeft.Size = new System.Drawing.Size(0, 357);
            this._ReportsTabViewUnpinnedTabAreaLeft.TabIndex = 5;
            // 
            // _ReportsTabViewUnpinnedTabAreaRight
            // 
            this._ReportsTabViewUnpinnedTabAreaRight.Dock = System.Windows.Forms.DockStyle.Right;
            this._ReportsTabViewUnpinnedTabAreaRight.Font = new System.Drawing.Font("Verdana", 8.25F);
            this._ReportsTabViewUnpinnedTabAreaRight.Location = new System.Drawing.Point(1105, 24);
            this._ReportsTabViewUnpinnedTabAreaRight.Name = "_ReportsTabViewUnpinnedTabAreaRight";
            this._ReportsTabViewUnpinnedTabAreaRight.Owner = this.ultraDockManager;
            this._ReportsTabViewUnpinnedTabAreaRight.Size = new System.Drawing.Size(0, 357);
            this._ReportsTabViewUnpinnedTabAreaRight.TabIndex = 6;
            // 
            // _ReportsTabViewUnpinnedTabAreaTop
            // 
            this._ReportsTabViewUnpinnedTabAreaTop.Dock = System.Windows.Forms.DockStyle.Top;
            this._ReportsTabViewUnpinnedTabAreaTop.Font = new System.Drawing.Font("Verdana", 8.25F);
            this._ReportsTabViewUnpinnedTabAreaTop.Location = new System.Drawing.Point(0, 24);
            this._ReportsTabViewUnpinnedTabAreaTop.Name = "_ReportsTabViewUnpinnedTabAreaTop";
            this._ReportsTabViewUnpinnedTabAreaTop.Owner = this.ultraDockManager;
            this._ReportsTabViewUnpinnedTabAreaTop.Size = new System.Drawing.Size(1105, 0);
            this._ReportsTabViewUnpinnedTabAreaTop.TabIndex = 7;
            // 
            // _ReportsTabViewUnpinnedTabAreaBottom
            // 
            this._ReportsTabViewUnpinnedTabAreaBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._ReportsTabViewUnpinnedTabAreaBottom.Font = new System.Drawing.Font("Verdana", 8.25F);
            this._ReportsTabViewUnpinnedTabAreaBottom.Location = new System.Drawing.Point(0, 381);
            this._ReportsTabViewUnpinnedTabAreaBottom.Name = "_ReportsTabViewUnpinnedTabAreaBottom";
            this._ReportsTabViewUnpinnedTabAreaBottom.Owner = this.ultraDockManager;
            this._ReportsTabViewUnpinnedTabAreaBottom.Size = new System.Drawing.Size(1105, 0);
            this._ReportsTabViewUnpinnedTabAreaBottom.TabIndex = 8;
            // 
            // _ReportsTabViewAutoHideControl
            // 
            this._ReportsTabViewAutoHideControl.Font = new System.Drawing.Font("Verdana", 8.25F);
            this._ReportsTabViewAutoHideControl.Location = new System.Drawing.Point(0, 0);
            this._ReportsTabViewAutoHideControl.Name = "_ReportsTabViewAutoHideControl";
            this._ReportsTabViewAutoHideControl.Owner = this.ultraDockManager;
            this._ReportsTabViewAutoHideControl.Size = new System.Drawing.Size(0, 0);
            this._ReportsTabViewAutoHideControl.TabIndex = 9;
            // 
            // ReportsTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._ReportsTabViewAutoHideControl);
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this._ReportsTabViewUnpinnedTabAreaTop);
            this.Controls.Add(this._ReportsTabViewUnpinnedTabAreaBottom);
            this.Controls.Add(this._ReportsTabViewUnpinnedTabAreaLeft);
            this.Controls.Add(this._ReportsTabViewUnpinnedTabAreaRight);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "ReportsTabView";
            this.Size = new System.Drawing.Size(1105, 381);
            this.contextMenuStripCloseTab.ResumeLayout(false);
            this.resultsMenuStrip.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            this.mainSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraDockManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip resultsMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem exportDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem excelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem portableDocumentFormatPDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xMLPaperSpecificationXPSToolStripMenuItem;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument gridPrintDocument;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog printPreviewDialog;
        //private Infragistics.Win.UltraWinGrid.UltraGrid reportGridView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCloseTab;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private System.Windows.Forms.ComboBox cmbChartType;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private Infragistics.Win.UltraWinDock.UltraDockManager ultraDockManager;
        private Infragistics.Win.UltraWinDock.AutoHideControl _ReportsTabViewAutoHideControl;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _ReportsTabViewUnpinnedTabAreaTop;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _ReportsTabViewUnpinnedTabAreaBottom;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _ReportsTabViewUnpinnedTabAreaLeft;
        private Infragistics.Win.UltraWinDock.UnpinnedTabArea _ReportsTabViewUnpinnedTabAreaRight;
        private System.Windows.Forms.TabControl tabControlReport;
        private System.Windows.Forms.ToolStripMenuItem closeAllTabsToolStripMenuItem;
    }
}
