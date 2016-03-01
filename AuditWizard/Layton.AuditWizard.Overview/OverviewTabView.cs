using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.AuditWizard.Network;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;

namespace Layton.AuditWizard.Overview
{
    [SmartPart]
    public partial class OverviewTabView : UserControl, ILaytonView
    {
        private LaytonWorkItem workItem;
        private WidgetManager _widgetManager;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private UltraGrid _currentGrid;

        [InjectionConstructor]
        public OverviewTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();

            List<string> widgets = new List<string>();
            widgets.Add("ApplicationComplianceWidget");
            widgets.Add("ServerData Widget");
            widgets.Add("Inventory Widget");
            widgets.Add("News Feed Widget");

            // Create the Widget manager object
            _widgetManager = new WidgetManager(this.widgetMatrixView, this.workItem);

            foreach (string widget in widgets)
            {
                _widgetManager.DisplayWidget(widget);
            }
        }

        public void RefreshView()
        {
            base.Refresh();
            this.widgetMatrixView.RefreshWidgets();
            Initialize();
        }

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }

        private void Initialize()
        {
            OverviewWorkItemController wiController = WorkItem.Controller as OverviewWorkItemController;

            // refresh the widgets
            //this.widgetMatrixView.RefreshWidgets();
        }

        public void DisplayDrilldownTabView(string drilldownReportName)
        {
            TabPage drillDownTabPage = null;
            TabPage drillDownTabPageHw = null;
            string strReportName;
            strReportName = drilldownReportName.Substring(0, drilldownReportName.IndexOf(":"));

            foreach (TabPage tabPage in tabControlDrilldown.TabPages)
            {
                if (tabPage.Text == "Software")
                {
                    tabPage.Text = "Data View";
                }
                if (tabPage.Text == "Hardware")
                {
                    tabControlDrilldown.TabPages.Remove(tabPage);
                }
            }


            if (Control.ModifierKeys != Keys.Control)
            {
                foreach (TabPage tabPage in tabControlDrilldown.TabPages)
                {
                    if (tabPage.Text == "Data View")
                    {
                        drillDownTabPage = tabPage;

                        foreach (Control control in tabPage.Controls)
                        {
                            if (control.GetType() == typeof(UltraGrid))
                            {
                                _currentGrid = (UltraGrid)control;
                                break;
                            }
                        }

                        break;
                    }
                }
            }

            if (drillDownTabPage == null)
            {
                drillDownTabPage = new TabPage("Data View");
                drillDownTabPage.ContextMenuStrip = contextMenuStripExportData;

                _currentGrid = CreateNewGrid(drilldownReportName);
                drillDownTabPage.Controls.Add(_currentGrid);

                _currentGrid.DataSource = ReportDataDrilldown(drilldownReportName);

                tabControlDrilldown.TabPages.Add(drillDownTabPage);
            }

            else
            {
                _currentGrid.DataSource = ReportDataDrilldown(drilldownReportName);
            }

            tabControlDrilldown.SelectedTab = drillDownTabPage;
            if (strReportName == "Expired" || strReportName == "Expire Today" || strReportName == "Expire this Week" || strReportName == "Expire this Month")
            {
                foreach (TabPage tabPage in tabControlDrilldown.TabPages)
                {
                    if (tabPage.Text == "Data View")
                    {
                        tabPage.Text = "Software";
                    }
                     

                    if (drillDownTabPageHw == null)
                    {
                        drillDownTabPageHw = new TabPage("Hardware");                

                        drillDownTabPageHw.ContextMenuStrip = contextMenuStripExportData;
                        _currentGrid =CreateNewGrid(drilldownReportName);    
                        drillDownTabPageHw.Controls.Add(_currentGrid);
                        _currentGrid.DataSource =ReportDataDrilldown(strReportName+" Asset:");      
                        tabControlDrilldown.TabPages.Add(drillDownTabPageHw);
                    }
                    else
                    {
                        _currentGrid.DataSource = ReportDataDrilldown(strReportName + " Asset:");
                    }
                }
            }
        }

        private UltraGrid CreateNewGrid(string aRowLabel)
        {
            UltraGrid lResultsGrid = new UltraGrid();

            try
            {
                Infragistics.Win.Appearance lGridAppearance = new Infragistics.Win.Appearance();
                Infragistics.Win.Appearance lRowAppearance = new Infragistics.Win.Appearance();
                Infragistics.Win.Appearance lRowAlternateAppearance = new Infragistics.Win.Appearance();
                Infragistics.Win.Appearance lSelectedRowAppearance = new Infragistics.Win.Appearance();

                lGridAppearance.BackColor = System.Drawing.Color.White;
                lResultsGrid.DisplayLayout.Appearance = lGridAppearance;

                lSelectedRowAppearance.BackColor = System.Drawing.Color.FromArgb(238, 243, 223);
                lSelectedRowAppearance.ForeColor = System.Drawing.Color.Black;
                lResultsGrid.DisplayLayout.Override.SelectedRowAppearance = lSelectedRowAppearance;

                lRowAppearance.BackColor = System.Drawing.Color.White;

                lResultsGrid.DisplayLayout.Override.FilterCellAppearance.BackColor = System.Drawing.Color.FromArgb(246, 252, 255);

                lResultsGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
                lResultsGrid.Dock = DockStyle.Fill;

                lRowAppearance.BorderColor = System.Drawing.Color.LightGray;
                lRowAppearance.TextVAlignAsString = "Middle";
                lRowAlternateAppearance.BackColor = System.Drawing.Color.FromArgb(246, 252, 255);
                lResultsGrid.DisplayLayout.Override.RowAppearance = lRowAppearance;
                lResultsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
                lResultsGrid.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.None;
                lResultsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;

                lResultsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
                lResultsGrid.DisplayLayout.Override.CellPadding = 4;
                lResultsGrid.DisplayLayout.Override.RowAlternateAppearance = lRowAlternateAppearance;

                lResultsGrid.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
                lResultsGrid.DisplayLayout.Override.AllowRowSummaries = AllowRowSummaries.BasedOnDataType;
                lResultsGrid.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;
                lResultsGrid.DisplayLayout.Override.SelectTypeRow = SelectType.Single;

                lResultsGrid.DisplayLayout.Override.AllowColMoving = AllowColMoving.WithinGroup;
                lResultsGrid.DisplayLayout.Override.AllowColSwapping = AllowColSwapping.WithinGroup;

                lResultsGrid.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                //lResultsGrid.DisplayLayout.Override.HeaderAppearance.BackColor = Color.White;
                lResultsGrid.DisplayLayout.Override.HeaderAppearance.BackColor = Color.FromArgb(221, 236, 255);
                lResultsGrid.DisplayLayout.Override.HeaderAppearance.ForeColor = Color.DimGray;
                lResultsGrid.DisplayLayout.Override.HeaderAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                lResultsGrid.DisplayLayout.Override.RowSelectorAppearance.BackColor = Color.White;
                lResultsGrid.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;

                // Set the view style to OutlookGroupBy. Without it, group by box won't show up
                lResultsGrid.DisplayLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;

                Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
                appearance14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(228)))), ((int)(((byte)(248)))));
                appearance14.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(157)))), ((int)(((byte)(185)))), ((int)(((byte)(235)))));
                //appearance14.BackColor = Color.White;
                appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
                //appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.None;
                appearance14.FontData.BoldAsString = "True";
                appearance14.FontData.Name = "Verdana";
                appearance14.FontData.SizeInPoints = 10F;
                appearance14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(127)))), ((int)(((byte)(177)))));
                //appearance14.ForeColor = Color.DimGray;
                lResultsGrid.DisplayLayout.GroupByBox.Appearance = appearance14;
                lResultsGrid.DisplayLayout.GroupByBox.BandLabelBorderStyle = Infragistics.Win.UIElementBorderStyle.None;
                lResultsGrid.DisplayLayout.GroupByBox.ButtonBorderStyle = Infragistics.Win.UIElementBorderStyle.None;

                Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
                appearance15.BackColor = System.Drawing.Color.Transparent;
                appearance15.FontData.Name = "Verdana";
                lResultsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance15;

                lResultsGrid.DisplayLayout.ViewStyle = ViewStyle.MultiBand;
                lResultsGrid.DisplayLayout.ScrollBounds = ScrollBounds.ScrollToFill;
                lResultsGrid.DisplayLayout.Override.SelectTypeCol = SelectType.None;
                lResultsGrid.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;

                lResultsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
                lResultsGrid.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.None;
                lResultsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
                lResultsGrid.DisplayLayout.Override.CellAppearance.BorderColor = System.Drawing.Color.Transparent;

                lResultsGrid.DoubleClickRow += new DoubleClickRowEventHandler(lResultsGridAsset_DoubleClickRow);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return lResultsGrid;
        }

        private void lResultsGridAsset_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            string assetName = e.Row.Cells[0].Value.ToString();

            List<WorkItem> workItemList = (List<WorkItem>)workItem.RootWorkItem.WorkItems.FindByType(typeof(NetworkWorkItem));
            NetworkWorkItem netDiscWorkItem = workItemList[0] as NetworkWorkItem;            

            NetworkExplorerView explorerView = (NetworkExplorerView)netDiscWorkItem.ExplorerView;
            Infragistics.Win.UltraWinTree.UltraTree explorerTree = explorerView.GetDisplayedTree;

            Infragistics.Win.UltraWinTree.UltraTreeNode rootNode = explorerTree.Nodes[0];
            Infragistics.Win.UltraWinTree.UltraTreeNode selectedNode = AddMatches(rootNode, assetName);

            if (selectedNode != null)
            {
                NetworkWorkItemController controller = netDiscWorkItem.Controller as NetworkWorkItemController;
                controller.ActivateWorkItem();

                explorerTree.SelectedNodes.Clear();
                selectedNode.Selected = true;
                selectedNode.BringIntoView();                                
            }
        }

        private Infragistics.Win.UltraWinTree.UltraTreeNode AddMatches(Infragistics.Win.UltraWinTree.UltraTreeNode parentNode, string assetName)
        {
            foreach (Infragistics.Win.UltraWinTree.UltraTreeNode childNode in parentNode.Nodes)
            {
                // If this node represents an asset group then we should check it's children first
                if (childNode.Tag is AssetGroup)
                {
                    // If this branch is not currently expanded then we need to expand it now 
                    // in order to search it
                    bool currentState = childNode.Expanded;
                    //
                    if (!childNode.Expanded)
                        childNode.Expanded = true;
                    //	
                    //AddMatches(childNode, assetName);
                    Infragistics.Win.UltraWinTree.UltraTreeNode foundNode = AddMatches(childNode, assetName);
                    if (foundNode != null)
                        return foundNode;

                    // Contract the branch if it was NOT previously expanded
                    if (!currentState)
                        childNode.Expanded = false;
                }

                else if (childNode.Tag is Asset)
                {
                    Asset thisAsset = childNode.Tag as Asset;
                    string upperAssetname = thisAsset.Name.ToUpper();
                    //
                    if (upperAssetname.Equals(assetName))
                        return childNode;
                }
            }

            return null;
        }

        private DataTable ReportDataDrilldown(string aRowLabel)
        {
            DataTable reportDataSet = new DataTable();
            StatisticsDAO lStatisticsDAO = new StatisticsDAO();

            string reportName = "";

            if (aRowLabel.StartsWith("Compliance : "))
            {
                reportName = aRowLabel.Substring(13);
            }
            else if (aRowLabel.StartsWith("Installations for : "))
            {
                reportName = aRowLabel.Substring(20);
            }
            else
            {
                reportName = aRowLabel.Substring(0, aRowLabel.IndexOf(":"));
                reportName += ": ";
            }

            switch (reportName)
            {
                case StatisticTitles.ComputersDiscovered:
                    reportDataSet = lStatisticsDAO.ComputersDiscovered();
                    break;

                case StatisticTitles.ComputersAudited:
                    reportDataSet = lStatisticsDAO.ComputersAudited();
                    break;

                case StatisticTitles.ComputerLastAudit:
                    reportDataSet = lStatisticsDAO.ComputerLastAudit();
                    break;

                case StatisticTitles.AssetsInStock:
                    reportDataSet = lStatisticsDAO.AssetsInStock();
                    break;

                case StatisticTitles.AssetsInUse:
                    reportDataSet = lStatisticsDAO.AssetsInUse();
                    break;

                case StatisticTitles.AssetsPending:
                    reportDataSet = lStatisticsDAO.AssetsPending();
                    break;

                case StatisticTitles.AssetsDisposed:
                    reportDataSet = lStatisticsDAO.AssetsDisposed();
                    break;

                case StatisticTitles.AgentsDeployed:
                    reportDataSet = lStatisticsDAO.AgentsDeployed();
                    break;

                case StatisticTitles.UniqueApplications:
                    reportDataSet = lStatisticsDAO.UniqueApplications();
                    break;

                case StatisticTitles.TotalApplications:
                    reportDataSet = lStatisticsDAO.TotalApplications();
                    break;

                case StatisticTitles.AuditedToday:
                    reportDataSet = lStatisticsDAO.AuditedToday();
                    break;

                case StatisticTitles.NotAudited7:
                    reportDataSet = lStatisticsDAO.NotAudited7();
                    break;

                case StatisticTitles.NotAudited14:
                    reportDataSet = lStatisticsDAO.NotAudited14();
                    break;

                case StatisticTitles.NotAudited30:
                    reportDataSet = lStatisticsDAO.NotAudited30();
                    break;

                case StatisticTitles.NotAudited90:
                    reportDataSet = lStatisticsDAO.NotAudited90();
                    break;

                case StatisticTitles.NotAudited:
                    reportDataSet = lStatisticsDAO.NotAudited();
                    break;

                case StatisticTitles.LastAlert:
                    reportDataSet = lStatisticsDAO.LastAlert();
                    break;

                case StatisticTitles.OutstandingAlerts:
                    reportDataSet = lStatisticsDAO.StatisticsAssetsStatusDrilldown(aRowLabel);
                    break;

                case StatisticTitles.AlertsToday:
                    reportDataSet = lStatisticsDAO.AlertsToday();
                    break;

                case StatisticTitles.AlertsThisWeek:
                    reportDataSet = lStatisticsDAO.AlertsThisWeek();
                    break;

                case StatisticTitles.AlertsThisMonth:
                    reportDataSet = lStatisticsDAO.AlertsThisMonth();
                    break;

                case StatisticTitles.SupportExpired:
                    reportDataSet = lStatisticsDAO.SupportExpired();
                    break;

                case StatisticTitles.SupportExpireToday:
                    reportDataSet = lStatisticsDAO.SupportExpireToday();
                    break;

                case StatisticTitles.SupportExpireThisWeek:
                    reportDataSet = lStatisticsDAO.SupportExpireThisWeek();
                    break;

                case StatisticTitles.SupportExpireThisMonth:
                    reportDataSet = lStatisticsDAO.SupportExpireThisMonth();
                    break;

                case StatisticTitles.SupportExpiredAsset:
                    reportDataSet = lStatisticsDAO.SupportExpiredAsset();
                    break;

                case StatisticTitles.SupportExpireTodayAsset:
                    reportDataSet = lStatisticsDAO.SupportExpireTodayAsset();
                    break;

                case StatisticTitles.SupportExpireThisWeekAsset:
                    reportDataSet = lStatisticsDAO.SupportExpireThisWeekAsset();
                    break;

                case StatisticTitles.SupportExpireThisMonthAsset:
                    reportDataSet = lStatisticsDAO.SupportExpireThisMonthAsset();
                    break;

                case "Compliant":
                case "Non-Compliant":
                case "Ignored":
                case "Not Defined":
                    reportDataSet = lStatisticsDAO.StatisticsComplianceDrilldown(reportName);
                    break;

                default:
                    AssetDAO lAssetDAO = new AssetDAO();
                    string lVersion = (reportName.EndsWith(")")) ? reportName.Substring(reportName.LastIndexOf('(') + 2).TrimEnd(')') : "";
                    string lApplicationName = (lVersion == "") ? reportName : reportName.Substring(0, reportName.LastIndexOf(" ("));
                    //reportDataSet = lAssetDAO.FindAssetByApplicationNameAndVersion(lApplicationName, lVersion, lAssetDAO.GetAllAssetIdsAsString());

					// CMD 8.4.2.3 - Why pass in all asset IDS???  A null string is better from what I can see
                    reportDataSet = lAssetDAO.FindAssetByApplicationNameAndVersion(lApplicationName, lVersion, "");						
                    break;
            }

            return reportDataSet;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControlDrilldown.SelectedTab.Text != "Dashboard View")
                tabControlDrilldown.TabPages.Remove(tabControlDrilldown.SelectedTab);
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage selectedTab = tabControlDrilldown.SelectedTab;

            foreach (TabPage tabPage in tabControlDrilldown.TabPages)
            {
                if (tabPage != selectedTab)
                {
                    if (tabPage.Text != "Dashboard View")
                        tabControlDrilldown.TabPages.Remove(tabPage);
                }
            }
        }

        private void closeAllTabsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TabPage tabPage in tabControlDrilldown.TabPages)
            {
                if (tabPage.Text != "Dashboard View")
                    tabControlDrilldown.TabPages.Remove(tabPage);
            }
        }

        private void contextMenuStripCloseTab_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int index = 0; index <= this.tabControlDrilldown.TabCount - 1; index += 1)
                {
                    if (this.tabControlDrilldown.GetTabRect(index).Contains(e.Location))
                    {
                        this.tabControlDrilldown.SelectedIndex = index;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }
        }

        private void tabControlDrilldown_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //contextMenuStripCloseTab.Show(e.Location);
                //for (int index = 0; index <= this.tabControlDrilldown.TabCount - 1; index += 1)
                //{
                //    if (this.tabControlDrilldown.GetTabRect(index).Contains(e.Location))
                //    {
                //        this.tabControlDrilldown.SelectedIndex = index;
                //        break; // TODO: might not be correct. Was : Exit For
                //    }
                //}
            }
        }

        /// <summary>
        /// Called to export the contents of the displayed grid to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportXlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToXLS();
        }


        /// <summary>
        /// Called to export the data from the grid to a PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToPDF();
        }

        /// <summary>
        /// Handle Export to XPS selected from the context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportXPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToXPS();
        }

        /// <summary>
        /// Export the graph data to an XLS format file
        /// </summary>
        public void ExportToXLS()
        {
            // If there are no rows in the grid then we cannot export
            if (_currentGrid.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                UltraGridExporter.ExportUltraGridToExcel(_currentGrid, "AuditWizardData");
            }
        }


        /// <summary>
        /// Export to PDF
        /// </summary>
        public void ExportToPDF()
        {
            // If there are no rows in the grid then we cannot export
            if (this._currentGrid.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                // We need to temporarily set the grid view to 'Resize all columns' in order to get
                // the resultant PDF file formatted correctly.
                AutoFitStyle oldStyle = _currentGrid.DisplayLayout.AutoFitStyle;
                _currentGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

                // First browse for the folder / file that we will save
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
                saveFileDialog.FileName = "AuditWizardData.pdf";
                saveFileDialog.Filter = "Adobe Acrobat Document (*.pdf)|*.pdf";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    UltraGridExporter.Export(saveFileDialog.FileName
                                            , ""
                                            , ""
                                            , ""
                                            , _currentGrid
                                            , Infragistics.Documents.Reports.Report.FileFormat.PDF);

                    DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
                }

                // Populate the old autofit style
                this._currentGrid.DisplayLayout.AutoFitStyle = oldStyle;
            }
        }


        /// <summary>
        /// Export to XPS
        /// </summary>
        public void ExportToXPS()
        {
            // If there are no rows in the grid then we cannot export
            if (_currentGrid.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                // First browse for the folder / file that we will save
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
                saveFileDialog.FileName = "AuditWizardData.xps";
                saveFileDialog.Filter = "XML Paper Specification (*.xps)|*.xps";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    UltraGridExporter.Export(saveFileDialog.FileName
                                            , ""
                                            , ""
                                            , ""
                                            , _currentGrid
                                            , Infragistics.Documents.Reports.Report.FileFormat.XPS);

                    DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
                }
            }
        }

        private void widgetMatrixView_Leave(object sender, EventArgs e)
        {

        }

        private void contextMenuStripCloseTab_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tabControlDrilldown.SelectedTab.Text == "Dashboard View")
            {
                closeAllTabsToolStripMenuItem.Enabled = false;
                closeAllToolStripMenuItem.Enabled = true;
                closeToolStripMenuItem.Enabled = false;
            }
            else
            {
                closeAllTabsToolStripMenuItem.Enabled = true;
                closeAllToolStripMenuItem.Enabled = true;
                closeToolStripMenuItem.Enabled = true;
            }
        }
    }
}
