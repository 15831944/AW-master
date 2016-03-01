namespace Layton.AuditWizard.Overview
{
    partial class OverviewTabView
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
            this.ultraGridBagLayoutManager1 = new Infragistics.Win.Misc.UltraGridBagLayoutManager(this.components);
            this.tabControlDrilldown = new System.Windows.Forms.TabControl();
            this.contextMenuStripCloseTab = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllTabsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPageDashboard = new System.Windows.Forms.TabPage();
            this.widgetMatrixView = new WidgetMatrixView();
            this.contextMenuStripExportData = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portableDocumentFormatPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xMLPaperSpecificationXPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutManager1)).BeginInit();
            this.tabControlDrilldown.SuspendLayout();
            this.contextMenuStripCloseTab.SuspendLayout();
            this.tabPageDashboard.SuspendLayout();
            this.contextMenuStripExportData.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlDrilldown
            // 
            this.tabControlDrilldown.ContextMenuStrip = this.contextMenuStripCloseTab;
            this.tabControlDrilldown.Controls.Add(this.tabPageDashboard);
            this.tabControlDrilldown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlDrilldown.Location = new System.Drawing.Point(0, 0);
            this.tabControlDrilldown.Name = "tabControlDrilldown";
            this.tabControlDrilldown.SelectedIndex = 0;
            this.tabControlDrilldown.Size = new System.Drawing.Size(902, 630);
            this.tabControlDrilldown.TabIndex = 1;
            this.tabControlDrilldown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControlDrilldown_MouseDown);
            // 
            // contextMenuStripCloseTab
            // 
            this.contextMenuStripCloseTab.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem,
            this.closeAllToolStripMenuItem,
            this.closeAllTabsToolStripMenuItem});
            this.contextMenuStripCloseTab.Name = "contextMenuStripCloseTab";
            this.contextMenuStripCloseTab.Size = new System.Drawing.Size(167, 70);
            this.contextMenuStripCloseTab.MouseUp += new System.Windows.Forms.MouseEventHandler(this.contextMenuStripCloseTab_MouseUp);
            this.contextMenuStripCloseTab.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripCloseTab_Opening);
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
            // tabPageDashboard
            // 
            this.tabPageDashboard.Controls.Add(this.widgetMatrixView);
            this.tabPageDashboard.Location = new System.Drawing.Point(4, 22);
            this.tabPageDashboard.Name = "tabPageDashboard";
            this.tabPageDashboard.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDashboard.Size = new System.Drawing.Size(894, 604);
            this.tabPageDashboard.TabIndex = 0;
            this.tabPageDashboard.Text = "Dashboard View";
            this.tabPageDashboard.UseVisualStyleBackColor = true;
            // 
            // widgetMatrixView
            // 
            this.widgetMatrixView.BackColor = System.Drawing.Color.LightSteelBlue;
            this.widgetMatrixView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.widgetMatrixView.Location = new System.Drawing.Point(3, 3);
            this.widgetMatrixView.Name = "widgetMatrixView";
            this.widgetMatrixView.Size = new System.Drawing.Size(888, 598);
            this.widgetMatrixView.TabIndex = 0;
            // 
            // contextMenuStripExportData
            // 
            this.contextMenuStripExportData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportDataToolStripMenuItem});
            this.contextMenuStripExportData.Name = "applicationsMenuStrip";
            this.contextMenuStripExportData.Size = new System.Drawing.Size(169, 26);
            // 
            // exportDataToolStripMenuItem
            // 
            this.exportDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.excelToolStripMenuItem,
            this.portableDocumentFormatPDFToolStripMenuItem,
            this.xMLPaperSpecificationXPSToolStripMenuItem});
            this.exportDataToolStripMenuItem.Image = global::Layton.AuditWizard.Overview.Properties.Resources.export_16;
            this.exportDataToolStripMenuItem.Name = "exportDataToolStripMenuItem";
            this.exportDataToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.exportDataToolStripMenuItem.Text = "&Export Data to...";
            // 
            // excelToolStripMenuItem
            // 
            this.excelToolStripMenuItem.Image = global::Layton.AuditWizard.Overview.Properties.Resources.excel_32;
            this.excelToolStripMenuItem.Name = "excelToolStripMenuItem";
            this.excelToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.excelToolStripMenuItem.Text = "&Excel";
            this.excelToolStripMenuItem.Click += new System.EventHandler(this.ExportXlsToolStripMenuItem_Click);
            // 
            // portableDocumentFormatPDFToolStripMenuItem
            // 
            this.portableDocumentFormatPDFToolStripMenuItem.Image = global::Layton.AuditWizard.Overview.Properties.Resources.pdf_32;
            this.portableDocumentFormatPDFToolStripMenuItem.Name = "portableDocumentFormatPDFToolStripMenuItem";
            this.portableDocumentFormatPDFToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.portableDocumentFormatPDFToolStripMenuItem.Text = "Portable Document Format (PDF)";
            this.portableDocumentFormatPDFToolStripMenuItem.Click += new System.EventHandler(this.ExportPDFToolStripMenuItem_Click);
            // 
            // xMLPaperSpecificationXPSToolStripMenuItem
            // 
            this.xMLPaperSpecificationXPSToolStripMenuItem.Image = global::Layton.AuditWizard.Overview.Properties.Resources.xps_32;
            this.xMLPaperSpecificationXPSToolStripMenuItem.Name = "xMLPaperSpecificationXPSToolStripMenuItem";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.xMLPaperSpecificationXPSToolStripMenuItem.Text = "XML Paper Specification (XPS)";
            this.xMLPaperSpecificationXPSToolStripMenuItem.Click += new System.EventHandler(this.ExportXPSToolStripMenuItem_Click);
            // 
            // OverviewTabView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(246)))), ((int)(((byte)(254)))));
            this.Controls.Add(this.tabControlDrilldown);
            this.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.Name = "OverviewTabView";
            this.Size = new System.Drawing.Size(902, 630);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutManager1)).EndInit();
            this.tabControlDrilldown.ResumeLayout(false);
            this.contextMenuStripCloseTab.ResumeLayout(false);
            this.tabPageDashboard.ResumeLayout(false);
            this.contextMenuStripExportData.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGridBagLayoutManager ultraGridBagLayoutManager1;
        private System.Windows.Forms.TabControl tabControlDrilldown;
        private System.Windows.Forms.TabPage tabPageDashboard;
        private WidgetMatrixView widgetMatrixView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCloseTab;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllTabsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripExportData;
        private System.Windows.Forms.ToolStripMenuItem exportDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem excelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem portableDocumentFormatPDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xMLPaperSpecificationXPSToolStripMenuItem;

	}
}
