using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Cab.Interface;
using Layton.NetworkDiscovery;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;
using System.Collections;
using System.Net;

namespace Layton.AuditWizard.Network
{
    [SmartPart]
    public partial class GroupTabView : UserControl, ILaytonView
    {
        #region Data
        GroupTabViewPresenter presenter;
        LaytonWorkItem workItem;
        private static string _gridLayoutFile = "NetworkGroupTabLayout.xml";
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BackgroundWorker backgroundWorker1;

        #endregion Data

        #region Data Accessors
        [CreateNew]
        public GroupTabViewPresenter Presenter
        {
            set
            {
                presenter = value;
                presenter.View = this;
                presenter.Initialize();
            }
            get { return presenter; }
        }

        public string HeaderText
        {
            set
            {
                headerLabel.Text = value;
                groupGridView.Text = value;
                CenterHeaderLabel();
            }
        }

        public Image HeaderImage
        {
            set { headerLabel.Appearance.Image = value; }
        }

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }

        #endregion Data Accessors

        #region Constructor
        [InjectionConstructor]
        public GroupTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);

            // See if we have a layout file saved and if so read it to initialize the grid
            LoadLayout();
        }

        #endregion

        #region Form Population Functions


        /// <summary>
        /// DisplayGroup
        /// ============
        /// 
        /// Display the contents of the specified group within this tab view
        /// 
        /// </summary>
        /// <param name="displayGroup"></param>
        public void DisplayGroup(AssetGroup displayGroup)
        {
             // Depending on the type of group being displayed we need to set the header image, title and also set the 
            // text for the 'Domain' column to either 'Domain' or 'Location'            
            if (displayGroup.GroupType == AssetGroup.GROUPTYPE.domain)
            {
                int lCount = displayGroup.Groups.Count + displayGroup.Assets.Count;
                this.headerLabel.Text = (lCount == 1) ? displayGroup.Name + " (" + lCount + " item)" : displayGroup.Name + " (" + lCount + " items)";
                this.headerLabel.Appearance.Image = Properties.Resources.domain96;
                this.groupGridView.DisplayLayout.Bands[0].Columns["Domain"].Header.Caption = "Domain";                
            }
            else
            {
                int lCount = displayGroup.Groups.Count + displayGroup.Assets.Count;
                this.headerLabel.Text = (lCount == 1) ? displayGroup.Name + " (" + lCount + " item)" : displayGroup.Name + " (" + lCount + " items)";
                this.headerLabel.Appearance.Image = Properties.Resources.location_96;
                this.groupGridView.DisplayLayout.Bands[0].Columns["Domain"].Header.Caption = "Location";                
            }



            // Now add the data to the DataSet
            foreach (AssetGroup childGroup in displayGroup.Groups)
            {
                groupDataSet.Tables[0].Rows.Add(new object[] { childGroup, childGroup.Name, "", "", "", "", "" });
            }

            int lLocation = -1;

            // Add any child assets
            foreach (Asset childAsset in displayGroup.Assets)
            {
                //string agentStatus = childAsset.AgentStatusText;

                //if (agentStatus != "Not Deployed")
                //{
                //    lLocation = childAsset.AgentVersion.IndexOf("v");

                //    if (lLocation != -1)
                //        agentStatus += " (" + childAsset.AgentVersion.Substring(lLocation) + ")";
                //}

                string agentStatus = childAsset.AgentVersion;

                groupDataSet.Tables[0].Rows.Add(new object[] { childAsset, childAsset.Domain, childAsset.Name, childAsset.IPAddress, childAsset.LastAuditDateString, agentStatus, Asset.GetStockStatusText(childAsset.StockStatus) });
            }
        }

        #endregion Form Population Functions

        #region Form Control Functions
        /// <summary>
        /// Handle the re-sizing of the header box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void headerGroupBox_SizeChanged(object sender, EventArgs e)
        {
            CenterHeaderLabel();
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            //if (WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart == this)
            //{
            //    RefreshView();
            //}
        }

        /// <summary>
        /// Called as each row in the grid is initialized - we use this to set the appearance of the row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupGridView_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            // Get the application object being displayed
            UltraGridRow thisRow = e.Row;
            UltraGridCell objectCell = thisRow.Cells[0];

            // Set the icon to be displayed based on what type of object we are displaying
            // the object being displayed is held as the value for the first (hidden) column in the row
            if (objectCell.Value is AssetGroup)
            {
                AssetGroup displayGroup = objectCell.Value as AssetGroup;
                thisRow.Cells["Domain"].Appearance.Image = displayGroup.DisplayIcon();
            }

            else if (objectCell.Value is Asset)
            {
                Asset thisComputer = objectCell.Value as Asset;
                Bitmap icon = thisComputer.DisplayIcon();
                thisRow.Cells["Domain"].Appearance.Image = icon;
            }
        }




        #endregion Form Control Functions

        #region Context Menu Handlers

        /// <summary>
        /// Called to reaudit an asset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reAuditAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void ReauditAssets()
        {
            System.Collections.Specialized.NameValueCollection ipRanges = new System.Collections.Specialized.NameValueCollection();

            foreach (Asset asset in GetSelectedAssets())
            {
                if (asset.IPAddress != String.Empty)
                    ipRanges.Add(asset.IPAddress, asset.IPAddress);
            }

            SNMPDiscovery discoverer = new SNMPDiscovery(ipRanges);
			discoverer.Start();

			// Yes - we need to check whether or not the current scanner is configured to upload audits to an FTP
			// site as in this case we need to create audit files for any assets discovered
			AuditScannerDefinition auditScannerDefinition = null;
			try
			{
				string scannerPath = Path.Combine(Application.StartupPath, "scanners") + "\\default.xml";
				auditScannerDefinition = AuditWizardSerialization.DeserializeObject(scannerPath);
			}
			catch (Exception)
			{
			}


			// 8.3.4 - CMD
			//
			// If we found a scanner and it defines an FTP upload of audit files then process this
			// Note that we support either Upload to FTP Location or Audit to FTP which are flagged differently in the scanner
			if ((auditScannerDefinition != null)
			&& (auditScannerDefinition.FTPCopyToNetwork || auditScannerDefinition.UploadSetting == AuditScannerDefinition.eUploadSetting.ftp))
			{
				discoverer.UploadDiscoveredAssets(auditScannerDefinition);
			}
        }

        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {


            DesktopAlert.ShowDesktopAlert("The selected asset(s) have been re-audited.");
        }

        void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ReauditAssets();
        }

        private void findAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.FindAsset();
        }

        /// <summary>
        /// Called to delete the specfiied computer from the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteComputersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Ok delete the selected computers
            List<Asset> listComputers = GetSelectedAssets();

            // Get our controller
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;

            // ...and request it to delete the currently selected item
            if (listComputers.Count > 0)
                wiController.DeleteAsset(listComputers);
        }

        /// <summary>
        /// Flag the selected assets as 'Stock'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stockMenuItem_Click(object sender, EventArgs e)
        {
            List<Asset> listComputers = GetSelectedAssets();
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.SetAssetState(listComputers, Asset.STOCKSTATUS.stock);
        }


        /// <summary>
        /// Flag the selected assets as 'In Use'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inUseMenuItem_Click(object sender, EventArgs e)
        {
            List<Asset> listComputers = GetSelectedAssets();
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.SetAssetState(listComputers, Asset.STOCKSTATUS.inuse);
        }


        /// <summary>
        /// Flag the selected assets as 'Pending Disposal'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pendingDisposalMenuItem_Click(object sender, EventArgs e)
        {
            List<Asset> listComputers = GetSelectedAssets();
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.SetAssetState(listComputers, Asset.STOCKSTATUS.pendingdisposal);
        }


        /// <summary>
        /// Flag the selected assets as 'disposed'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void disposedMenuItem_Click(object sender, EventArgs e)
        {
            List<Asset> listComputers = GetSelectedAssets();
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.SetAssetState(listComputers, Asset.STOCKSTATUS.disposed);
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

        private void deployToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

            if (menuItem != null)
            {
                NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
                if (wiController != null && menuItem.Tag != null)
                    wiController.DeployAgents(menuItem.Tag.ToString());
            }
        }

        private void updateConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

            if (menuItem != null)
            {
                NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
                if (wiController != null && menuItem.Tag != null)
                    wiController.UpdateAgentConfiguration(menuItem.Tag.ToString());
            }
        }

        private void auditNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.RequestReaudit();
        }

        /// <summary>
        /// Start the AuditWizard Agent on the specified computers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.Start();
        }


        /// <summary>
        /// Stop the AuditWizard Agent Service on the specified computers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.Stop();
        }


        /// <summary>
        /// Remove the AuditWizard Agent from the specified computers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.Remove();

            //Layton.NetworkDiscovery.TcpipNetworkDiscovery tcp = new Layton.NetworkDiscovery.TcpipNetworkDiscovery(null);
            //tcp.Start();
        }


        /// <summary>
        /// Check the status of the AuditWizard Agent on the selected Computers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.CheckStatus();
        }


        /// <summary>
        /// View the AuditWizard Agent Log File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.ViewLogFile();
        }


        /// <summary>
        /// Clear the AuditWizard Agent Log File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.ClearLogFile();
        }

        /// <summary>
        /// Called to re-locate the selected assets based on their IP address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void relocateByIPMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.RelocateByIPAddress();
        }



        /// <summary>
        /// Called to remote desktop to the specific computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void remoteDesktopMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.RemoteDesktop();
        }

        #endregion Context Menu Handlers

        #region Export Functions

        /// <summary>
        /// Export the graph data to an XLS format file
        /// </summary>
        public void ExportToXLS()
        {
            // If there are no rows in the grid then we cannot export
            if (this.groupDataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                UltraGridExporter.ExportUltraGridToExcel(groupGridView, headerLabel.Text);
            }
        }


        /// <summary>
        /// Export to PDF
        /// </summary>
        public void ExportToPDF()
        {
            // If there are no rows in the grid then we cannot export
            if (this.groupDataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                // We need to temporarily set the grid view to 'Resize all columns' in order to get
                // the resultant PDF file formatted correctly.
                AutoFitStyle oldStyle = groupGridView.DisplayLayout.AutoFitStyle;
                groupGridView.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;

                // First browse for the folder / file that we will save
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
                saveFileDialog.FileName = headerLabel.Text + ".pdf";
                saveFileDialog.Filter = "Adobe Acrobat Document (*.pdf)|*.pdf";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    UltraGridExporter.Export(saveFileDialog.FileName
                                            , "AuditWizard Applications View : " + groupGridView.Text
                                            , "Generated by AuditWizard from Layton Technology, Inc."
                                            , DataStrings.Disclaimer
                                            , groupGridView
                                            , Infragistics.Documents.Reports.Report.FileFormat.PDF);
                    DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
                }

                // Populate the old autofit style
                this.groupGridView.DisplayLayout.AutoFitStyle = oldStyle;
            }
        }


        /// <summary>
        /// Export to XPS
        /// </summary>
        public void ExportToXPS()
        {
            // If there are no rows in the grid then we cannot export
            if (this.groupDataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("There is no data to Export", "Export Error");
            }

            else
            {
                // First browse for the folder / file that we will save
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.ValidateNames = false;
                saveFileDialog.FileName = headerLabel.Text + ".xps";
                saveFileDialog.Filter = "XML Paper Specification (*.xps)|*.xps";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    UltraGridExporter.Export(saveFileDialog.FileName
                                            , "AuditWizard Applications View : " + groupGridView.Text
                                            , "Generated by AuditWizard from Layton Technology, Inc."
                                            , DataStrings.Disclaimer
                                            , groupGridView
                                            , Infragistics.Documents.Reports.Report.FileFormat.XPS);
                    DesktopAlert.ShowDesktopAlert("Data successfully exported to '" + saveFileDialog.FileName + "'");
                }
            }
        }

        #endregion Export Functions

        #region Helper Functions

        private void CenterHeaderLabel()
        {
        }

        public void Clear()
        {
            groupDataSet.Tables[0].Rows.Clear();
        }

        public void RefreshView()
        {
            presenter.Initialize();
            base.Refresh();
        }


        /// <summary>
        /// GetSelectedAssets
        /// ====================
        /// 
        /// Returns a list of the computers selected in this view
        /// </summary>
        /// <returns></returns>
        public List<Asset> GetSelectedAssets()
        {
            List<Asset> listComputers = new List<Asset>();
            int selectedRowCount = groupGridView.Selected.Rows.Count;
            for (int isub = 0; isub < selectedRowCount; isub++)
            {
                UltraGridRow selectedRow = this.groupGridView.Selected.Rows[isub];
                if (selectedRow.Cells[0].Value is Asset)
                {
                    Asset thisComputer = selectedRow.Cells[0].Value as Asset;
                    listComputers.Add(thisComputer);
                }
            }
            return listComputers;
        }

        #endregion Helper Functions

        #region Drag and Drop Functions

        private void groupGridView_SelectionDrag(object sender, CancelEventArgs e)
        {
            List<Asset> listDraggedComputers = new List<Asset>();

            foreach (UltraGridRow selectedRow in ((UltraGrid)sender).Selected.Rows)
            {
                // Add any computers selected to our draggable list
                if (selectedRow.Cells["ComputerObject"].Value is Asset)
                    listDraggedComputers.Add(selectedRow.Cells["ComputerObject"].Value as Asset);
            }

            // If the tag is an Application object and our parent is a publisher then this node must be an 
            // application and as such can be dragged and dropped
            if (listDraggedComputers.Count != 0)
                this.groupGridView.DoDragDrop(listDraggedComputers, DragDropEffects.Move);
        }

        #endregion Drag and Drop Functions

        #region Load/Save Layout

        /// <summary>
        /// Called to load the layout for the rid from file
        /// </summary>
        private void LoadLayout()
        {
            try
            {
                string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
                this.groupGridView.DisplayLayout.LoadFromXml(layoutFile);
            }
            catch (Exception)
            {
                return;
            }
        }


        /// <summary>
        /// Called to request the grid to save it's layout to disk
        /// </summary>
        public void SaveLayout()
        {
            if (this.groupGridView != null)
            {
                string layoutFile = Path.Combine(Application.StartupPath, _gridLayoutFile);
                this.groupGridView.DisplayLayout.SaveAsXml(layoutFile);
            }
        }

        #endregion Load/Save Layout

        private void groupGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Is this the <delete> key in which case we handle it as if the user has selected delete
            // from the context menu
            if (e.KeyChar == (char)Keys.Delete)
                deleteComputersToolStripMenuItem_Click(sender, e);
        }

        private void groupGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control) && (e.KeyCode == Keys.F))
            {
                e.Handled = true;
                findAssetToolStripMenuItem_Click(sender, null);
            }
        }

        private void groupGridView_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            string assetName = e.Row.Cells["Asset"].Value.ToString();

            List<WorkItem> workItemList = (List<WorkItem>)workItem.RootWorkItem.WorkItems.FindByType(typeof(NetworkWorkItem));
            NetworkWorkItem netDiscWorkItem = workItemList[0] as NetworkWorkItem;
            NetworkWorkItemController controller = netDiscWorkItem.Controller as NetworkWorkItemController;

            NetworkExplorerView explorerView = (NetworkExplorerView)netDiscWorkItem.ExplorerView;
            Infragistics.Win.UltraWinTree.UltraTree explorerTree = explorerView.GetDisplayedTree;

            Infragistics.Win.UltraWinTree.UltraTreeNode rootNode = explorerTree.Nodes[0];
            Infragistics.Win.UltraWinTree.UltraTreeNode selectedNode = AddMatches(rootNode, assetName);

            if (selectedNode != null)
            {
                selectedNode.BringIntoView();
                //explorerTree.SelectedNodes.Clear();

                selectedNode.Expanded = true;
                selectedNode.Selected = true;

                //controller.ActivateWorkItem();
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

        private void groupMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            List<Asset> listAssets = GetSelectedAssets();
            deleteComputersToolStripMenuItem.Enabled = (listAssets.Count != 0);
            //assetPropertiesMenuItem.Enabled = (listAssets.Count == 1);

            // The deployment options are more complex as they are affected both by the count and also 
            // if only a single PC is selected, its current status
            if (listAssets.Count == 0)
            {
                deployToolStripMenuItem.Enabled = false;
                startToolStripMenuItem.Enabled = false;
                stopToolStripMenuItem.Enabled = false;
                removeToolStripMenuItem.Enabled = false;
                viewLogFileToolStripMenuItem.Enabled = false;
                clearLogFileToolStripMenuItem.Enabled = false;
                auditNowToolStripMenuItem.Enabled = false;
            }

            else if (listAssets.Count > 1)
            {
                deployToolStripMenuItem.Enabled = true;
                startToolStripMenuItem.Enabled = true;
                stopToolStripMenuItem.Enabled = true;
                removeToolStripMenuItem.Enabled = true;
                clearLogFileToolStripMenuItem.Enabled = true;
                auditNowToolStripMenuItem.Enabled = true;
                viewLogFileToolStripMenuItem.Enabled = false;
                auditAgentToolStripMenuItem.Visible = true;

                // if at least one of the assets in auditable, turn off the reaudit device by SNMP option
                foreach (Asset asset in listAssets)
                {
                    if (asset.Auditable)
                    {
                        reAuditDeviceToolStripMenuItem.Visible = false;
                        break;
                    }
                }

                // if at least one of the assets in not auditable, turn off the Audit Agent option
                foreach (Asset asset in listAssets)
                {
                    if (!asset.Auditable)
                    {
                        auditAgentToolStripMenuItem.Visible = false;
                        break;
                    }
                }
            }

            else
            {
                // Just a single asset so we can be a bit more strict with what is enabled - 
                // View Log and Check Status are always enabled
                viewLogFileToolStripMenuItem.Enabled = true;
                Asset asset = listAssets[0];

                if (asset.Auditable)
                {
                    auditAgentToolStripMenuItem.Visible = true;
                    reAuditDeviceToolStripMenuItem.Visible = false;

                    // If the Agent is missing we can clear the log and deploy
                    if (asset.AgentStatus == Asset.AGENTSTATUS.notdeployed)
                    {
                        clearLogFileToolStripMenuItem.Enabled = true;
                        deployToolStripMenuItem.Enabled = true;
                        //
                        startToolStripMenuItem.Enabled = false;
                        stopToolStripMenuItem.Enabled = false;
                        removeToolStripMenuItem.Enabled = false;
                        auditNowToolStripMenuItem.Enabled = false;
                    }

                    // If deployed but not running we can Start, Remove and Clear Log
                    else if (asset.AgentStatus == Asset.AGENTSTATUS.deployed)
                    {
                        clearLogFileToolStripMenuItem.Enabled = true;
                        startToolStripMenuItem.Enabled = true;
                        removeToolStripMenuItem.Enabled = true;
                        //
                        deployToolStripMenuItem.Enabled = false;
                        stopToolStripMenuItem.Enabled = false;
                        auditNowToolStripMenuItem.Enabled = false;
                    }

                    // The Agent is running so we can stop, reaudit or remove
                    else
                    {
                        stopToolStripMenuItem.Enabled = true;
                        removeToolStripMenuItem.Enabled = true;
                        auditNowToolStripMenuItem.Enabled = true;
                        //
                        deployToolStripMenuItem.Enabled = false;
                        startToolStripMenuItem.Enabled = false;
                        clearLogFileToolStripMenuItem.Enabled = false;
                    }
                }
                else
                {
                    auditAgentToolStripMenuItem.Visible = false;
                    reAuditDeviceToolStripMenuItem.Visible = true;
                }
            }

            // Relocate by IP address is valid if we have one or more assets selected and we are in Custom
            // locations display mode
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            relocateByIPMenuItem.Enabled = ((listAssets.Count != 0) && !wiController.DomainViewStyle);

            // Remote desktop only works if there is a single selected asset
            remoteDesktopMenuItem.Enabled = (listAssets.Count == 1);

            if (deployToolStripMenuItem.Enabled)
            {
                PopulateMenuItemWithAgents(deployToolStripMenuItem);

                foreach (ToolStripMenuItem toolStripMenuItem in deployToolStripMenuItem.DropDownItems)
                {
                    toolStripMenuItem.Click += deployToolStripMenuItem_Click;
                }
            }

            if (updateConfigurationToolStripMenuItem.Enabled)
            {
                PopulateMenuItemWithAgents(updateConfigurationToolStripMenuItem);

                foreach (ToolStripMenuItem toolStripMenuItem in updateConfigurationToolStripMenuItem.DropDownItems)
                {
                    toolStripMenuItem.Click += updateConfigurationToolStripMenuItem_Click;
                }
            }
        }

        private void PopulateMenuItemWithAgents(ToolStripMenuItem menuItem)
        {
            menuItem.DropDownItems.Clear();

            // Get the path to the scanner configurations
            string scannerPath = Path.Combine(Application.StartupPath, @"scanners\\auditagent");

            DirectoryInfo di = new DirectoryInfo(scannerPath);
            FileInfo[] rgFiles = di.GetFiles("*.xml");
            foreach (FileInfo fi in rgFiles)
            {
                try
                {
                    string scannerName = fi.Name.Replace(".xml", "");
                    string fileName = Path.Combine(Application.StartupPath, @"scanners\auditagent\" + scannerName + ".xml");

                    AuditScannerDefinition configuration = AuditWizardSerialization.DeserializeObject(fileName);

                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Tag = fileName;
                    item.Text = scannerName;
                    item.ToolTipText = configuration.Description;

                    menuItem.DropDownItems.Add(item);
                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message);
                }
            }
        }

        private void groupGridView_MouseClick(object sender, MouseEventArgs e)
        {
            UltraGrid grid = sender as UltraGrid;
            int iColCount = grid.DisplayLayout.Bands[0].Columns.Count;
            for (int i = 0; i < iColCount; i++)
            {
                string strColName = grid.DisplayLayout.Bands[0].Columns[i].ToString();
                string strKey = grid.DisplayLayout.Bands[0].Columns[i].Key.ToString();
                if (strColName == "IP Address" || strColName == "IPAddress")
                {
                    grid.DisplayLayout.Bands[0].Columns[i].SortComparer = new srtComparer(strKey);
                    break;
                }
            }
                         
        }
    }

    /// <summary>
    /// added to sort the IP address
    /// Sojan E John KtsInfotech
    /// </summary>
    public class srtComparer : IComparer
    {
        public string m_strKey;
        public srtComparer()
        {
            m_strKey = "IPAddress";
        }
        public srtComparer(string strKey)
        {
            m_strKey = strKey;
        }

        public int Compare(object x, object y)
        {
            UltraGridCell xCell = (UltraGridCell)x;
            UltraGridCell yCell = (UltraGridCell)y;            

            //return DateTime.Compare((DateTime)xCell.Row.Cells["Date"].Value, (DateTime)yCell.Row.Cells["Date"].Value);

            string strFirstIP = xCell.Row.Cells[m_strKey].Value.ToString();
            string strSecondIP = yCell.Row.Cells[m_strKey].Value.ToString();            
            
            if (strFirstIP == "")
            {
                return 1;
            }
            else if (strSecondIP == "")
            {
                return -1;
            }
            else
            {
                return IPAddressComparer.ComapreIP(strFirstIP, strSecondIP);
            }
            
        }
    }

}
