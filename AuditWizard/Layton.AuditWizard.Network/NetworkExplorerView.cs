using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using Layton.AuditWizard.Applications;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Cab.Interface;
using Layton.NetworkDiscovery;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;

namespace Layton.AuditWizard.Network
{
    [SmartPart]
    public partial class NetworkExplorerView : UserControl, ILaytonView
    {
        #region Data
        private LaytonWorkItem _workItem;
        private NetworkExplorerViewPresenter _presenter;
        private UltraTreeNode _rootNode;

        //Create a new instance of the DrawFilter class to handle drawing the DropHighlight/DropLines
        private UltraTree_DropHightLight_DrawFilter_Class UltraTree_DropHightLight_DrawFilter = new UltraTree_DropHightLight_DrawFilter_Class();
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Data fields which determine how applications will be displayed in this tree</summary>
        protected string _publisherFilter = "";
        protected bool _showIncluded = true;
        protected bool _showIgnored = false;

        private BackgroundWorker backgroundWorker1;

        #endregion Data

        #region Properties

        [CreateNew]
        public NetworkExplorerViewPresenter Presenter
        {
            set
            {
                _presenter = value;
                _presenter.View = this;
                //_presenter.Initialize();
                //_rootNode.Selected = true;
            }
        }


        public LaytonWorkItem WorkItem
        {
            get { return _workItem as LaytonWorkItem; }
        }


        /// <summary>
        /// Add the 'Root Node' to the root of the tree
        /// </summary>
        public UltraTreeNode RootNode
        {
            set { _rootNode = value; networkTree.Nodes.Add(_rootNode); _rootNode.Expanded = true; _rootNode.Selected = true; }
            get { return _rootNode; }
        }

        public UltraTree GetDisplayedTree
        {
            get { return networkTree; }
        }

        #endregion Data Accessors

        #region Constructor

        [InjectionConstructor]
        public NetworkExplorerView([ServiceDependency] WorkItem workItem)
        {
            this._workItem = workItem as LaytonWorkItem;
            InitializeComponent();
            this.networkTree.Override.SelectionType = SelectType.ExtendedAutoDrag;
            this.Paint += new PaintEventHandler(NetworkExplorerView_Paint);

            //Attach the Drawfiler to the tree
            networkTree.DrawFilter = UltraTree_DropHightLight_DrawFilter;
            UltraTree_DropHightLight_DrawFilter.Invalidate += new EventHandler(this.UltraTree_DropHightLight_DrawFilter_Invalidate);
            UltraTree_DropHightLight_DrawFilter.QueryStateAllowedForNode += UltraTree_DropHightLight_DrawFilter_QueryStateAllowedForNode;

            // Set the tree view to be dynamically expanded
            networkTree.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnExpand;

            // Set the display style foe the tree so that we can swap between vista and 'standard' trees
            AuditWizardConfiguration configuration = new AuditWizardConfiguration();
            networkTree.DisplayStyle = (configuration.VistaTrees) ? UltraTreeDisplayStyle.WindowsVista : UltraTreeDisplayStyle.Standard;

            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
        }

        #endregion Constructor

        #region View Refresh Functions

        /// <summary>
        /// RefreshView
        /// ===========
        /// 
        /// Refresh this view 
        /// </summary>
        public void RefreshView()
        {
            // First save any existing selections
            List<string> selectedNodeKeys = new List<string>();
            foreach (UltraTreeNode node in networkTree.SelectedNodes)
            {
                selectedNodeKeys.Add(node.Key);
            }

            // ...then save any 'expanded' nodes
            List<UltraTreeNode> expandedNodes = new List<UltraTreeNode>();
            GetExpandedNodes(expandedNodes, networkTree.Nodes);

            // Initialize the view
            _presenter.Initialize();

            // Call our base class refresh code next
            base.Refresh();

            // ...then reinstate any selections
            RestoreSelectedNodes(selectedNodeKeys);

            // ...then re-instate any expansions
            RestoreExpandedNodes(expandedNodes);
        }

        /// <summary>
        /// Clear all contents from this tree view
        /// </summary>
        public void Clear()
        {
            networkTree.Nodes.Clear();
        }


        /// <summary>
        /// Iterative function to check for all nodes in the tree which have been expanded
        /// </summary>
        /// <param name="expandedNodes"></param>
        /// <param name="childNodes"></param>
        /// <returns></returns>
        protected void GetExpandedNodes(List<UltraTreeNode> expandedNodes, TreeNodesCollection childNodes)
        {
            // Starting from the root item traverse down looking for nodes which are expanded
            foreach (UltraTreeNode thisNode in childNodes)
            {
                if (thisNode.Expanded)
                {
                    expandedNodes.Add(thisNode);
                    GetExpandedNodes(expandedNodes, thisNode.Nodes);
                }
            }
        }


        /// <summary>
        /// Restore the selection state of any nodes contained within the supplied list
        /// </summary>
        /// <param name="selectedNodes"></param>
        protected void RestoreSelectedNodes(List<string> selectedNodeKeys)
        {
            // Clear any current selections
            networkTree.SelectedNodes.Clear();

            // ...then select what-ever nodes we had selected previously
            foreach (string nodeKey in selectedNodeKeys)
            {
                UltraTreeNode selectedNode = networkTree.GetNodeByKey(nodeKey);
                if (selectedNode != null)
                {
                    selectedNode.BringIntoView();
                    if (selectedNode.Selected == false)
                        selectedNode.Selected = true;
                }
            }
        }


        /// <summary>
        /// Restore the expanded state of the supplied nodes
        /// </summary>
        /// <param name="expandedNodes"></param>
        protected void RestoreExpandedNodes(List<UltraTreeNode> expandedNodes)
        {
            foreach (UltraTreeNode node in expandedNodes)
            {
                UltraTreeNode expandedNode = networkTree.GetNodeByKey(node.Key);
                if (expandedNode != null)
                {
                    if (expandedNode.Expanded == false)
                        expandedNode.Expanded = true;
                }
            }
        }
        #endregion

        #region Form Handling Functions

        void NetworkExplorerView_Paint(object sender, PaintEventArgs e)
        {
            Image bgImg = Properties.Resources.ghosted_network_icon;
            e.Graphics.DrawImage(bgImg, (this.Width - bgImg.Width - 30), (this.Height - bgImg.Height - 30));
        }

        #endregion Form Handling Functions

        #region Tree Control Functions


        /// <summary>
        /// Called as we select a new node within the tree - we may need to populate the node if we have 
        /// not previously done so
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void networkTree_BeforeExpand(object sender, CancelableNodeEventArgs e)
        {
            UltraTreeNode node = e.TreeNode;

            if ((node.FullPath.Contains("\\<All Assets>\\System\\")) ||
                (node.FullPath.Contains("\\<All Assets>\\Hardware\\")))
            {
                Cursor.Current = Cursors.WaitCursor;
                PopulateAllAssetsHardware(node);
                Cursor.Current = Cursors.Default;
            }

            ExpandNode(node);
        }



        /// <summary>
        /// This function is called when we change the selection made within the main Explorer Tree
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void networkTree_AfterSelect(object sender, SelectEventArgs e)
        {
            // recover the work item controller
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;

            if (e.NewSelections.Count > 0)
            {
                UltraTreeNode node = e.NewSelections[0];

                // Have we selected an asset group?
                if (node.Tag is AssetGroup)
                {
                    // Group selected
                    AssetGroup selectedGroup = node.Tag as AssetGroup;
                    wiController.SetTabView(node, TreeSelectionEventArgs.ITEMTYPE.assetgroup, null);
                }

                // Have we selected an asset (or a node beneath one)
                else if ((node.Tag is Asset) || (node.Tag is FileSystemFolder))
                {
                    
                    // Asset selected
                    TreeSelectionEventArgs.ITEMTYPE itemType = GetAssetNodeTypeFromKey(node);
                    wiController.Tree = networkTree;
                    wiController.SetTabView(node, itemType, null);
                    
                }

                else if (node.Tag is UserDataCategory)
                {
                    // User Defined Data Category selected
                    wiController.SetTabView(node, TreeSelectionEventArgs.ITEMTYPE.asset_userdata, node.Tag);
                }

                else if (node.Tag is AllAssets)
                {
                    // An All Assets branch has been selected - get the type of item being displayed
                    TreeSelectionEventArgs.ITEMTYPE itemType = GetAllAssetNodeTypeFromKey(node);
                    wiController.SetTabView(node, itemType, node.Tag);
                    
                }
            }
        }


        private void networkTree_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Delete)
            {
                NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
                wiController.DeleteAsset();
            }
        }

        #endregion

        #region Event declarations and handlers

        public void SelectViewToDisplay(string key, Asset forAsset)
        {
            if (networkTree.SelectedNodes.Count > 0)
            {
                UltraTreeNode node = networkTree.SelectedNodes[0];
                networkTree.SelectedNodes.Clear();
                if (!node.Expanded)
                    node.Expanded = true;

                // Format the key for the required sub-node and select it, ensuring that it is visible
                string subKey = node.Key + "|" + key;
                UltraTreeNode subNode = node.Nodes[subKey];
                subNode.Selected = true;
                subNode.Visible = true;
            }
        }

        /// <summary>
        /// This is the handler for the GLOBAL PublisherFilterChangeEvent which is fired when 
        /// the Publisher Filter has been updated elsewhere in the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EventSubscription(CommonEventTopics.PublisherFilterChanged)]
        public void PublisherFilterChangedHandler(object sender, PublisherFilterEventArgs e)
        {
            // Simply update our internal publisher filter with that specified
            _publisherFilter = e.PublisherFilter;
            _showIncluded = e.ViewIncludedApplications;
            _showIgnored = e.ViewIgnoredApplications;

            // ...and force a refresh if we are the active work item
            ILaytonView activeExplorerView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].ActiveSmartPart;
            if (activeExplorerView == WorkItem.ExplorerView)
            {
                WorkItem.TabView.RefreshView();
                WorkItem.ExplorerView.RefreshView();
            }
        }

        #endregion

        #region Drag and Drop Support

        private void networkTree_SelectionDragStart(object sender, EventArgs e)
        {
            // Construct a list of the assets being dragged
            List<Asset> listDraggedAssets = new List<Asset>();
            foreach (UltraTreeNode selectedNode in networkTree.SelectedNodes)
            {
                // Add any assets selected to our draggable list
                if (selectedNode.Tag is Asset)
                {
                    TreeSelectionEventArgs.ITEMTYPE itemType = GetAssetNodeTypeFromKey(selectedNode);
                    if (itemType == TreeSelectionEventArgs.ITEMTYPE.asset)
                        listDraggedAssets.Add(selectedNode.Tag as Asset);
                }
            }

            // Begin drag operation if we have a computer to drag
            if (listDraggedAssets.Count != 0)
                networkTree.DoDragDrop(listDraggedAssets, DragDropEffects.Move);
        }


        /// <summary>
        /// Complete a drag/drop operation by dropping on a Group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void networkTree_DragDrop(object sender, DragEventArgs e)
        {
            //Get the position of the mouse in the tree, as opposed to form coords
            Point currentPoint = networkTree.PointToClient(new Point(e.X, e.Y));

            //Get the node the mouse is over.
            UltraTreeNode node = networkTree.GetNodeFromPoint(currentPoint);

            // Get the data being dropped - this should be a list of ApplicationInstance objects
            List<Asset> listDroppedComputers = (List<Asset>)e.Data.GetData(typeof(List<Asset>));

            // ...and do the drop
            if (IsGroupNode(node))
                ((NetworkWorkItemController)WorkItem.Controller).ChangeGroup(listDroppedComputers, node.Tag as AssetGroup);
            else
                MessageBox.Show("Assets can only be dragged to a Domain or Location", "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);


            //After the drop is complete, erase the current drop highlight. 
            UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
        }


        /// <summary>
        /// Called as we drag over an item in the tree - we can only drop on a domain/Location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void networkTree_DragOver(object sender, DragEventArgs e)
        {
            //Get the position of the mouse in the tree, as opposed to form coords
            Point currentPoint = networkTree.PointToClient(new Point(e.X, e.Y));

            //Get the node the mouse is over.
            UltraTreeNode node = networkTree.GetNodeFromPoint(currentPoint);

            //Make sure the mouse is over a node
            if (IsGroupNode(node))
            {
                if (!node.HasNodes)
                    ExpandAssetGroup(node);

                // allow dropping on this node
                e.Effect = DragDropEffects.Move;

                // Tell the DrawFilter where we are by calling SetDropHighlightNode
                UltraTree_DropHightLight_DrawFilter.SetDropHighlightNode(node, currentPoint);
            }
            else
            {
                e.Effect = DragDropEffects.None;

                //Erase any DropHighlight
                UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
            }
        }



        /// <summary>
        /// Test to see if we want to continue dragging
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsTree_QueryContinueDrag(object sender, System.Windows.Forms.QueryContinueDragEventArgs e)
        {
            //Did the user press escape? 
            if (e.EscapePressed)
            {
                //User pressed escape - cancel the Drag
                e.Action = DragAction.Cancel;

                //Clear the Drop highlight, since we are no longer dragging
                UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
            }
        }


        /// <summary>
        /// Occassionally, the DrawFilter will let us know that the control needs to be invalidated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UltraTree_DropHightLight_DrawFilter_Invalidate(object sender, System.EventArgs e)
        {
            networkTree.Invalidate();
        }



        /// <summary>
        /// This event is fired by the DrawFilter to let us determine what kinds of drops we want to allow 
        /// on any particular node - we only allow dropping on Publishers at this time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UltraTree_DropHightLight_DrawFilter_QueryStateAllowedForNode(Object sender, UltraTree_DropHightLight_DrawFilter_Class.QueryStateAllowedForNodeEventArgs e)
        {
            if (this.IsGroupNode(e.Node))
                e.StatesAllowed = DropLinePositionEnum.OnNode;
            else
                e.StatesAllowed = DropLinePositionEnum.None;
        }


        /// <summary>
        /// Fires when the user drags outside the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void networkTree_DragLeave(object sender, System.EventArgs e)
        {
            // When the mouse goes outside the control, clear the drophighlight. 
            // Since the DropHighlight is cleared when the mouse is not over a node, anyway,  this is probably 
            // not needed but, just in case the user goes from a node directly off the control...
            UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
        }


        private void networkTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //Get the position of the mouse
                Point currentPoint = new Point(e.X, e.Y);

                //Get the node the mouse is over.
                UltraTreeNode node = networkTree.GetNodeFromPoint(currentPoint);

                if (node != null)
                {
                    if (!networkTree.SelectedNodes.Contains(node))
                    {
                        networkTree.SelectedNodes.Clear();
                        node.Selected = true;
                    }
                }
            }
        }


        private bool IsGroupNode(UltraTreeNode node)
        {
            return (node != null && node.Tag is AssetGroup);
        }

        #endregion

        #region Context Menu Handlers


        /// <summary>
        /// Called to search for an asset in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.FindAsset();
        }

        /// <summary>
        /// Called to reaudit an asset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reAuditAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DesktopAlert.ShowDesktopAlert("The selected asset(s) have been re-audited.");
        }

        void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ReauditAssets();
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

			// 8.3.3
			// On completion we should process the re-audited assets creating ADF files as necessary
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
			&&  (auditScannerDefinition.FTPCopyToNetwork || auditScannerDefinition.UploadSetting == AuditScannerDefinition.eUploadSetting.ftp))
			{
				discoverer.UploadDiscoveredAssets(auditScannerDefinition);
			}
        }


        /// <summary>
        /// Called as the network context menu is opening - determine whether to enable or disable options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void networkMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            List<Asset> listAssets = GetSelectedAssets();
            deleteComputersToolStripMenuItem.Enabled = (listAssets.Count != 0);
            //assetPropertiesMenuItem.Enabled = (listAssets.Count == 1);

            newAssetToolStripMenuItem.Enabled = false;

            // New Asset is possible if the currently selected item is a GROUP
            if (networkTree.SelectedNodes.Count == 1)
            {
                UltraTreeNode selectedNode = networkTree.SelectedNodes[0];
                newAssetToolStripMenuItem.Enabled = (selectedNode.Tag is AssetGroup);
            }

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
            else
            {
                deployToolStripMenuItem.DropDownItems.Clear();
            }

            if (updateConfigurationToolStripMenuItem.Enabled)
            {
                PopulateMenuItemWithAgents(updateConfigurationToolStripMenuItem);

                foreach (ToolStripMenuItem toolStripMenuItem in updateConfigurationToolStripMenuItem.DropDownItems)
                {
                    toolStripMenuItem.Click += updateConfigurationToolStripMenuItem_Click;
                }
            }
            else
            {
                updateConfigurationToolStripMenuItem.DropDownItems.Clear();
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

        /// <summary>
        /// Add a new (user defined) asset to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UltraTreeNode selectedNode = networkTree.SelectedNodes[0];
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.AddAsset(selectedNode.Tag as AssetGroup);
        }


        private void deleteComputersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.DeleteAsset();
        }

        /// <summary>
        /// Deply the AuditWizard Agent to the specified computers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// Called as we request a re-audit of the selected computer(s)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void auditNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.RequestReaudit();
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



        private void stockMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.SetAssetStock();
        }

        private void inUseMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.SetAssetInUse();
        }

        private void pendingDisposalMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.SetAssetPending();
        }

        private void disposedMenuItem_Click(object sender, EventArgs e)
        {
            NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
            wiController.SetAssetDisposed();
        }

        #endregion Context Menu Handlers

        #region Tree Expansion / Population Functions

        /// <summary>
        /// Expand a node within the tree
        /// As this tree expands on demand we are called as a result of an expand request from the tree to 
        /// populate the child nodes of the specified node.
        /// </summary>
        /// <param name="node"></param>
        public void ExpandNode(UltraTreeNode node)
        {
            // Nothing to do if already expanded
            if (node.HasNodes == true)
                return;

            //	Call BeginUpdate to prevent drawing while we are populating the control
            this.networkTree.BeginUpdate();
            this.Cursor = Cursors.WaitCursor;

            // Recover the item being expanded
            if (node.Tag is AssetGroup)
                ExpandAssetGroup(node);

            else if (node.Tag is Asset)
                ExpandAsset(node);

            else if (node.Tag is AllAssets)
                ExpandAllAssets(node);


            //	Restore the cursor and finish updating the tree
            this.Cursor = Cursors.Default;
            this.networkTree.EndUpdate(true);
        }



        /// <summary>
        /// Expand a Tree Node which contains an AssetGroup
        /// </summary>
        /// <param name="node"></param>
        private void ExpandAssetGroup(UltraTreeNode expandingNode)
        {
            // We are expanding a traditional group within AuditWizard
            // get our controller and from there the current display moderators
            NetworkWorkItemController wiController = this.WorkItem.Controller as NetworkWorkItemController;

            // get the asset group and populate it
            AssetGroup expandingGroup = expandingNode.Tag as AssetGroup;
            expandingGroup.Populate(false, false, true);

            // Start to update the tree
            networkTree.BeginUpdate();

            try
            {
                // Add the 'All Assets' Node
                UltraTreeNode allAssetsNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.AllAssetsNode);
                allAssetsNode.LeftImages.Add(Properties.Resources.allassets_16);
                allAssetsNode.Tag = new AllAssets(expandingGroup, AllAssets.eNodeType.root);
                expandingNode.Nodes.Add(allAssetsNode);

                // Add the top level groups (and assets if available) to the tree
                foreach (AssetGroup group in expandingGroup.Groups)
                {
                    UltraTreeNode childNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, group.Name);

                    // Set the correct image for this group type
                    Bitmap image = (group.GroupType == AssetGroup.GROUPTYPE.domain) ? Properties.Resources.domain16 : Properties.Resources.location_16;
                    childNode.LeftImages.Add(image);

                    // Set the tag for the node to be the AssetGroup object
                    childNode.Tag = group;

                    // ...and add the node to the tree
                    expandingNode.Nodes.Add(childNode);
                }

                // Add any assets which have been defined outside of groups to the tree also - note that we can have
                // multiple assets with the same name as we may be auditing multiple domains and so we must be careful
                // with the keys for the tree as these must be unique
                foreach (Asset asset in expandingGroup.Assets)
                {
                    // Create the new tree node for this asset
                    UltraTreeNode childNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, asset);

                    // Recover the icon which we need to display for this asset
                    Bitmap icon = asset.DisplayIcon();
                    childNode.LeftImages.Add(icon);

                    // Set the tag for the node to be the 'asset' object
                    childNode.Tag = asset;

                    // ...and add the node to the tree
                    expandingNode.Nodes.Add(childNode);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayErrorMessage("Exception adding tree nodes in [NetworkExplorerView::ExpandAssetGroup], the exception text was " + ex.Message);
            }

            // End updating the tree
            networkTree.EndUpdate();
        }



        #region 'All Asset' Handling Functions

        /// <summary>
        /// Called as we expand the 'All Assets' node of the tree or a sub-branch beneath that
        /// </summary>
        /// <param name="expandingNode"></param>
        public void ExpandAllAssets(UltraTreeNode expandingNode)
        {
            // Get the node tag as this will help us to know what on earth we are expanding
            AllAssets allAssets = expandingNode.Tag as AllAssets;

            // Are we expanding the root 'All Assets' node?
            if (expandingNode.Key.EndsWith(AWMiscStrings.AllAssetsNode))
            {
                ExpandAllAssetsRoot(expandingNode);
            }

            else if (allAssets.AllAssetType == AllAssets.eNodeType.item_value)
            {
                ExpandAllAssetsItemValues(expandingNode);
            }
        }



        /// <summary>
        /// Expand the root 'All Assets' node
        /// </summary>
        /// <param name="expandingNode"></param>
        protected void ExpandAllAssetsRoot(UltraTreeNode expandingNode)
        {
            // Begin Updating the tree
            networkTree.BeginUpdate();
            try
            {
                // Add in all of the possible top-level categories
                // We support:
                //		Operating System
                //		Applications
                //		Hardware
                //		System
                //		All Asset User Defined Data categories

                AssetGroup parentGroup = (expandingNode.Tag as AllAssets).ParentGroup;

                UltraTreeNode osNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.OSNode);
                osNode.LeftImages.Add(Properties.Resources.os_16);
                osNode.Tag = new AllAssets(parentGroup, AWMiscStrings.OSNode);
                expandingNode.Nodes.Add(osNode);

                UltraTreeNode applicationsNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.ApplicationsNode);
                applicationsNode.LeftImages.Add(Properties.Resources.application_16);
                applicationsNode.Tag = new AllAssets(parentGroup, AWMiscStrings.ApplicationsNode);
                expandingNode.Nodes.Add(applicationsNode);

                UltraTreeNode hardwareNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.HardwareNode);
                hardwareNode.LeftImages.Add(Properties.Resources.hardware);
                hardwareNode.Tag = new AllAssets(parentGroup, AWMiscStrings.HardwareNode);
                expandingNode.Nodes.Add(hardwareNode);

                UltraTreeNode systemNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.SystemNode);
                systemNode.LeftImages.Add(Properties.Resources.system_16);
                systemNode.Tag = new AllAssets(parentGroup, AWMiscStrings.SystemNode);
                expandingNode.Nodes.Add(systemNode);

                UltraTreeNode internetNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.InternetNode);
                internetNode.LeftImages.Add(Properties.Resources.aw_internet);
                internetNode.Tag = new AllAssets(parentGroup, AWMiscStrings.InternetNode);
                expandingNode.Nodes.Add(internetNode);

                UltraTreeNode userDataNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.UserDataNode);
                userDataNode.LeftImages.Add(Properties.Resources.userdata_16);
                userDataNode.Tag = new AllAssets(parentGroup, AWMiscStrings.UserDataNode);
                expandingNode.Nodes.Add(userDataNode);

                // Add on the User Defined Data Categories (if any)
                NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
                foreach (UserDataCategory category in wiController.UserDataCategories)
                {
                    if (category.Scope == UserDataCategory.SCOPE.Asset)
                    {
                        UltraTreeNode categoryNode = new UltraTreeNode(expandingNode.Key + "|" + AWMiscStrings.UserDataNode + "|" + category.Name, category.Name);
                        categoryNode.LeftImages.Add(IconMapping.LoadIcon(category.Icon, IconMapping.Iconsize.Small));

                        // In all cases beneath ALL ASSETS the tag must be an AllAssets object so set this now
                        AllAssets allAssets = new AllAssets(parentGroup, AWMiscStrings.UserDataNode);
                        categoryNode.Tag = allAssets;

                        // ...but so that we know where we are set the tag for all assets to hold the user data category
                        allAssets.Tag = category;

                        // ...and add the user data category to its parent
                        //expandingNode.Nodes.Add(categoryNode);
                        userDataNode.Nodes.Add(categoryNode);

                        // Populate this user defined data category
                        PopulateAllAssetsUserData(categoryNode);
                    }
                }

                // Populate the Operating Systems Branch
                PopulateAllAssetsOperatingSystems(osNode);

                // Populate the Applications Branch
                PopulateAllAssetsApplications(applicationsNode);

                // Populate the Hardware Branch
                PopulateAllAssetsHardware(hardwareNode);

                // Populate the System Branch
                PopulateAllAssetsHardware(systemNode);

                // Populate teh Internet branch
                PopluateAllAssetsInternet(internetNode);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayErrorMessage("Exception adding tree nodes in [NetworkExplorerView::ExpandAllAssetsRoot], the exception text was " + ex.Message);
            }

            // End update
            networkTree.EndUpdate();
        }


        /// <summary>
        /// Populate the Asset Details node of the tree
        /// </summary>
        /// <param name="expandingNode"></param>
        protected void PopulateAllAssetsOperatingSystems(UltraTreeNode expandingNode)
        {
            // The OS category only has item values so create the appropriate object to use as a tag
            AllAssets allAssets = new AllAssets(expandingNode.Tag as AllAssets);
            allAssets.AllAssetType = AllAssets.eNodeType.item_value;

            // Add in the OS attributes
            UltraTreeNode node = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, "Extended Name");
            node.LeftImages.Add(Properties.Resources.os_16);
            node.Tag = allAssets;
            expandingNode.Nodes.Add(node);
        }

        protected void PopluateAllAssetsInternet(UltraTreeNode expandingNode)
        {
            // The Internet category only has item values so create the appropriate object to use as a tag
            AllAssets allAssets = new AllAssets(expandingNode.Tag as AllAssets);
            allAssets.AllAssetType = AllAssets.eNodeType.item_value;

            UltraTreeNode rootInternetNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, "Default Browser");
            rootInternetNode.Tag = allAssets;
            rootInternetNode.LeftImages.Add(Properties.Resources.aw_internet);
            expandingNode.Nodes.Add(rootInternetNode);

            rootInternetNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, "Browser Versions");
            rootInternetNode.Tag = allAssets;
            rootInternetNode.LeftImages.Add(Properties.Resources.aw_internet);
            expandingNode.Nodes.Add(rootInternetNode);

            foreach (DataRow row in new AssetDAO().GetDistinctBrowsersWithVersions().Rows)
            {
                string browser = row[0].ToString();
                UltraTreeNode valueNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, browser);
                valueNode.Tag = allAssets;

                if (browser.Contains("Internet Explorer"))
                {
                    valueNode.LeftImages.Add(Properties.Resources.ie);
                }
                else if (browser.Contains("Chrome"))
                {
                    valueNode.LeftImages.Add(Properties.Resources.chrome);
                }
                else if (browser.Contains("Firefox"))
                {
                    valueNode.LeftImages.Add(Properties.Resources.firefox);
                }
                else if (browser.Contains("Safari"))
                {
                    valueNode.LeftImages.Add(Properties.Resources.safari);
                }
                else if (browser.Contains("Opera"))
                {
                    valueNode.LeftImages.Add(Properties.Resources.opera);
                }

                rootInternetNode.Nodes.Add(valueNode);
            }
        }


        /// <summary>
        /// Populate the Applications node of the tree with publishers and then applications
        /// </summary>
        /// <param name="expandingNode"></param>
        protected void PopulateAllAssetsApplications(UltraTreeNode expandingNode)
        {
            ApplicationsWorkItemController wiController = WorkItem.Controller as ApplicationsWorkItemController;

            // Populate the list of Publishers and applications first
            ApplicationPublisherList listPublishers = new ApplicationPublisherList(_publisherFilter, _showIncluded, _showIgnored);

            // Add the publishers to the tree 
            foreach (ApplicationPublisher thePublisher in listPublishers)
            {
                UltraTreeNode publisherNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, thePublisher.Name);
                publisherNode.Tag = expandingNode.Tag;

                // Set the primary icon to be a publisher
                //publisherNode.LeftImages.Add(Properties.Resources.application_publisher_16);

                // Add the Applications beneath this publisher
                foreach (InstalledApplication theApplication in thePublisher)
                {
                    AddApplicationNode(publisherNode, theApplication);
                }

                // Add the PUBLISHER node (and it's sub-nodes to the tree
                try
                {
                    expandingNode.Nodes.Add(publisherNode);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }
        }


        /// <summary>
        /// Populate the All Assets > User Defined Data > [Category]
        /// </summary>
        /// <param name="parentNode"></param>
        protected void PopulateAllAssetsUserData(UltraTreeNode expandingNode)
        {
            AllAssets nodeAllAssets = expandingNode.Tag as AllAssets;
            UserDataCategory category = nodeAllAssets.Tag as UserDataCategory;

            // Recover the AssetGroup which is the tag of the All Assets and get a list of the assets
            // at or below this group in the heirarchy
            AssetGroup parentGroup = nodeAllAssets.ParentGroup;
            AssetList listChildAssets = parentGroup.GetAllAssets();

            // Add the user defined data fields within this category to the tree 
            foreach (UserDataField field in category)
            {
                UltraTreeNode fieldNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, field.Name);
                AllAssets fieldAllAssets = new AllAssets(nodeAllAssets);
                fieldAllAssets.Tag = field;
                fieldNode.Tag = fieldAllAssets;
                fieldNode.LeftImages.Add(expandingNode.LeftImages[0]);

                // Add the VALUES for this field beneath this field - first we need to get the values
                field.PopulateValues();
                Dictionary<int, string> listCurrentValues = field.ListCurrentValues;
                foreach (KeyValuePair<int, string> kvp in listCurrentValues)
                {
                    string valueKey = fieldNode.Key + @"|" + kvp.Value;

                    // Get the ID of the asset in question - we can then check to see if this asset is one
                    // included in the current AllAssets branch
                    int assetID = kvp.Key;

                    // OK check the ASSETS in this AllAssets branch and if this is not one of them ignore the value.
                    Asset asset = listChildAssets.GetAssetById(assetID);
                    if (asset == null)
                        continue;

                    // OK This value relates to an asset which is within this branch so we will need to check to see if
                    // this value has already been added to the tree.  If so we add the asset to the existing node, otherwise
                    // we will have to create a new node first
                    if (fieldNode.Nodes.Exists(valueKey))
                    {
                        UltraTreeNode valueNode = fieldNode.Nodes[valueKey];
                        AllAssets newAllAssets = valueNode.Tag as AllAssets;
                        newAllAssets.ListAssets.Add(asset);
                    }
                    else
                    {
                        UltraTreeNode valueNode = new UltraTreeNode(valueKey, kvp.Value);
                        valueNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;
                        valueNode.LeftImages.Add(fieldNode.LeftImages[0]);

                        AllAssets valueAllAssets = new AllAssets(parentGroup, 0, kvp.Value);
                        valueAllAssets.ListAssets.Add(asset);

                        // Set the tag for the value node to be a new instance of the AllAssets object - this way
                        // we can add the assets into the list beneath the AllAssets for this value
                        valueNode.Tag = valueAllAssets;
                        fieldNode.Nodes.Add(valueNode);
                    }
                }

                // Add the field node (and it's sub-nodes to the tree
                try
                {
                    expandingNode.Nodes.Add(fieldNode);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }
        }


        /// <summary>
        /// Add an application to the publisher node
        /// </summary>
        /// <param name="publisherNode"></param>
        /// <param name="theApplication"></param>
        protected void AddApplicationNode(UltraTreeNode publisherNode, InstalledApplication theApplication)
        {
            Infragistics.Win.Appearance ignoredAppearance = new Infragistics.Win.Appearance();
            ignoredAppearance.ForeColor = Color.Gray;

            string key = theApplication.Publisher + "|" + theApplication.Name + "|" + theApplication.ApplicationID;

            UltraTreeNode applicationNode;

            if (theApplication.Version == String.Empty || theApplication.Name.EndsWith(theApplication.Version))
                applicationNode = new UltraTreeNode(key, theApplication.Name);
            else
                applicationNode = new UltraTreeNode(key, theApplication.Name + " (" + theApplication.Version + ")");

            applicationNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;

            if (theApplication.IsIgnored) applicationNode.Override.NodeAppearance = ignoredAppearance;

            // The tag is the application object itself
            AllAssets allAssets = new AllAssets(publisherNode.Tag as AllAssets);
            allAssets.Tag = theApplication;
            allAssets.ItemValue = theApplication.Name;
            allAssets.AllAssetType = AllAssets.eNodeType.item_value;
            applicationNode.Tag = allAssets;
            publisherNode.Nodes.Add(applicationNode);
        }

        /// <summary>
        /// Populate the Hardware node of the tree
        /// </summary>
        /// <param name="parentNode"></param>
        protected void PopulateSystemPatches(UltraTreeNode parentNode)
        {
            AuditedItemsDAO lwDataAccess = new AuditedItemsDAO();
            DataTable categoryTable = lwDataAccess.GetAuditedItemCategories(parentNode.Key);

            // Iterate through the returned rows and add the text and icon to the tree
            // All the returned items are categories
            foreach (DataRow row in categoryTable.Rows)
            {
                string category = (string)row["_CATEGORY"];
                string icon = (string)row["_ICON"];
                UltraTreeNode categoryNode;
                int index;
                string nodeText = String.Empty;

                // We display just the last portion of the category name as the node text
                index = category.LastIndexOf("|");
                nodeText = category.Substring(index + 1);

                // Add the category itself
                categoryNode = new UltraTreeNode(category, nodeText);
                categoryNode.Tag = parentNode.Tag;
                categoryNode.LeftImages.Add(IconMapping.LoadIcon(icon, IconMapping.Iconsize.Small));

                try
                {
                    parentNode.Nodes.Add(categoryNode);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Populate the Hardware node of the tree
        /// </summary>
        /// <param name="parentNode"></param>
        protected void PopulateAllAssetsHardware(UltraTreeNode parentNode)
        {
            AllAssets allAssets = new AllAssets(parentNode.Tag as AllAssets);
            allAssets.AllAssetType = AllAssets.eNodeType.item_value;

            // The key of the node passed into us is prefixed with the 'All Assets' path which we
            // do not want when we are trying to expand a hardware category.  Strip the key back so
            // that it starts with the 'Hardware' node identifier text
            int delimiter = parentNode.Key.LastIndexOf(allAssets.BranchName);

            if (delimiter == -1) return;

            // Get just the hardware part of the key
            string hardwareKey = parentNode.Key.Substring(delimiter);

            // some keys may just be '%', which is the SQL wildcard
            // in this case, just return
            if (hardwareKey == "%") return;

            // ...and use this to determine child  hardware categories
            AuditedItemsDAO lwDataAccess = new AuditedItemsDAO();
            DataTable categoryTable = lwDataAccess.GetAuditedItemCategories(hardwareKey);

            // Iterate through the returned rows and add the text and icon to the tree
            // All the returned items are categories
            foreach (DataRow row in categoryTable.Rows)
            {
                string category = (string)row["_CATEGORY"];
                //string icon = (string)row["_ICON"];
                UltraTreeNode categoryNode;
                int index;
                string nodeText;

                if (category.StartsWith("System|Patches|"))
                {
                    string reducedCategory = category.Substring(0, category.LastIndexOf('|'));

                    index = reducedCategory.LastIndexOf("|");
                    nodeText = reducedCategory.Substring(index + 1);

                    categoryNode = new UltraTreeNode(reducedCategory, nodeText) { Tag = parentNode.Tag };
                    //categoryNode.LeftImages.Add(IconMapping.LoadIcon(icon, IconMapping.ICONSIZE.small));

                    try
                    {
                        parentNode.Nodes.Add(categoryNode);
                        // Recurse to check for child categories of the category we have just added
                        PopulateSystemPatches(categoryNode);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    // We display just the last portion of the category name as the node text
                    index = category.LastIndexOf("|");
                    nodeText = category.Substring(index + 1);

                    // Add the category itself
                    categoryNode = AuditWizardUtility.CreateKeyedTreeNode(parentNode, nodeText);
                    categoryNode.Tag = allAssets;
                    //categoryNode.LeftImages.Add(IconMapping.LoadIcon(icon, IconMapping.ICONSIZE.small));

                    try
                    {
                        parentNode.Nodes.Add(categoryNode);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }

                    // Recurse to check for child categories of the category we have just added
                    //PopulateAllAssetsHardware(categoryNode);
                }
            }

            // Now check for any NAMES within this category
            DataTable namesTable = lwDataAccess.GetAuditedItemCategoryNames(hardwareKey);

            // Add any NAMES after the Categories
            foreach (DataRow row in namesTable.Rows)
            {
                string name = (string)row["_NAME"];
                //string icon = (string)row["_ICON"];

                // Add the NAME node
                UltraTreeNode nameNode = AuditWizardUtility.CreateKeyedTreeNode(parentNode, name);
                nameNode.Tag = allAssets;
                //nameNode.LeftImages.Add(IconMapping.LoadIcon(icon, IconMapping.ICONSIZE.small));

                try
                {
                    parentNode.Nodes.Add(nameNode);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }

            // If no sub-categories or Names then set the expansion indicator to none
            if ((categoryTable.Rows.Count == 0) && (namesTable.Rows.Count == 0))
                parentNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnExpand;

        }



        /// <summary>
        /// Expand an 'All Assets' node where are displaying the terminus 'value'
        /// We need to display all of the possible values for this field (note that we have to then filter
        /// these values to ensure that we only include values for assets within the current group
        /// </summary>
        /// <param name="expandingNode"></param>
        protected void ExpandAllAssetsItemValues(UltraTreeNode expandingNode)
        {
            // Get the 'AllAssets' object tag
            AllAssets allAssets = expandingNode.Tag as AllAssets;

            // ...and from that recover the branch type
            switch (allAssets.BranchName)
            {
                case AWMiscStrings.HardwareNode:
                    ExpandAllAssetsHardwareValues(expandingNode);
                    break;

                case AWMiscStrings.SystemNode:
                    ExpandAllAssetsHardwareValues(expandingNode);
                    break;

                case AWMiscStrings.OSNode:
                    ExpandAllAssetsOSValues(expandingNode);
                    break;

                case AWMiscStrings.ApplicationsNode:
                    ExpandAllAssetsApplication(expandingNode);
                    break;

                case AWMiscStrings.InternetNode:
                    ExpandAllAssetsInternetValues(expandingNode);
                    break;
            }
        }

        /// <summary>
        /// Get possible values for the Operating System Data Field defined by this node
        /// </summary>
        /// <param name="expandingNode"></param>
        protected void ExpandAllAssetsOSValues(UltraTreeNode expandingNode)
        {
            // Get the 'AllAssets' object tag
            AllAssets allAssets = expandingNode.Tag as AllAssets;

            // Recover the AssetGroup which is the tag of the All Assets and get a list of the assets
            // at or below this group in the heirarchy
            AssetGroup parentGroup = allAssets.ParentGroup;
            AssetList listChildAssets = parentGroup.GetAllAssets();

            // Split off the OS Category from the node key
            int delimiter = expandingNode.Key.IndexOf(allAssets.BranchName);
            if (delimiter == -1)
                return;

            // Get ALL possible values for the OS field
            // Recover a list of Operating Systems
            InstalledOSList listOperatingSystems = new InstalledOSList();

            // Add the entries to the tree 
            foreach (InstalledOS installedOS in listOperatingSystems)
            {
                // For each OS record we need to check the instances to see if any of them appear
                // in the list of assets at this location and if not we discard the record
                installedOS.LoadData();

                // Create a new All Assets object which will be added to the node as its tag - this will 
                // maintain the list of assets with this OS
                AllAssets newAllAssets = new AllAssets(parentGroup, 0, installedOS.Name);

                // Loop through the OS Instances and add any assets which are in the selected group to the list
                foreach (OSInstance osInstance in installedOS.Instances)
                {
                    Asset asset = listChildAssets.GetAssetById(osInstance.InstalledOnComputerID);
                    if (asset != null)
                        newAllAssets.ListAssets.Add(asset);
                }

                // Did we find any instances of this OS in our list of assets?
                if (newAllAssets.ListAssets.Count != 0)
                {
                    string valueKey = expandingNode.Key + "|" + installedOS.Name + "|" + installedOS.Version;
                    newAllAssets.Tag = installedOS;
                    UltraTreeNode valueNode = new UltraTreeNode(valueKey, installedOS.Name);
                    valueNode.Tag = newAllAssets;
                    //valueNode.LeftImages.Add(expandingNode.LeftImages[0]);
                    valueNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;
                    expandingNode.Nodes.Add(valueNode);
                }
            }
        }

        protected void ExpandAllAssetsInternetValues(UltraTreeNode expandingNode)
        {
            AssetDAO lAssetDAO = new AssetDAO();

            // Get the 'AllAssets' object tag
            AllAssets allAssets = expandingNode.Tag as AllAssets;

            // Recover the AssetGroup which is the tag of the All Assets and get a list of the assets
            // at or below this group in the heirarchy
            AssetGroup parentGroup = allAssets.ParentGroup;
            AssetList listChildAssets = parentGroup.GetAllAssets();

            // Split off the OS Category from the node key
            int delimiter = expandingNode.Key.IndexOf(allAssets.BranchName);
            if (delimiter == -1)
                return;

            if (expandingNode.Text == "Default Browser")
            {
                DataTable installedBrowsersDataTable = new AuditedItemsDAO().GetDefaultBrowsers();

                // Add the entries to the tree 
                foreach (DataRow row in installedBrowsersDataTable.Rows)
                {
                    string browser = row[0].ToString();
                    AllAssets newAllAssets = new AllAssets(parentGroup, 0, browser);

                    foreach (DataRow browserRow in lAssetDAO.GetInstalledBrowserAssetId(browser).Rows)
                    {
                        Asset asset = listChildAssets.GetAssetById((int) browserRow[0]);

                        if (asset != null)
                            newAllAssets.ListAssets.Add(asset);
                    }

                    if (newAllAssets.ListAssets.Count != 0)
                    {
                        string valueKey = expandingNode.Key + "|" + browser;
                        newAllAssets.Tag = browser;
                        UltraTreeNode valueNode = new UltraTreeNode(valueKey, browser);
                        valueNode.Tag = newAllAssets;
                        valueNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;

                        if (browser.Contains("Internet Explorer"))
                        {
                            valueNode.LeftImages.Add(Properties.Resources.ie);
                        }
                        else if (browser.Contains("Chrome"))
                        {
                            valueNode.LeftImages.Add(Properties.Resources.chrome);
                        }
                        else if (browser.Contains("Firefox"))
                        {
                            valueNode.LeftImages.Add(Properties.Resources.firefox);
                        }
                        else if (browser.Contains("Safari"))
                        {
                            valueNode.LeftImages.Add(Properties.Resources.safari);
                        }
                        else if (browser.Contains("Opera"))
                        {
                            valueNode.LeftImages.Add(Properties.Resources.opera);
                        }

                        expandingNode.Nodes.Add(valueNode);
                    }
                }
            }
            else
            {
                DataTable installedBrowsersDataTable = new AuditedItemsDAO().GetVersionBrowsers(expandingNode.Text);

                // Add the entries to the tree 
                foreach (DataRow row in installedBrowsersDataTable.Rows)
                {
                    string browser = row[0].ToString();
                    string version = row[1].ToString();

                    AllAssets newAllAssets = new AllAssets(parentGroup, 0, browser);

                    foreach (DataRow browserRow in lAssetDAO.GetBrowserVersionAssetId(browser, version).Rows)
                    {
                        Asset asset = listChildAssets.GetAssetById((int)browserRow[0]);

                        if (asset != null)
                            newAllAssets.ListAssets.Add(asset);
                    }

                    if (newAllAssets.ListAssets.Count != 0)
                    {
                        string valueKey = expandingNode.Key + "|" + browser + "|" + version;
                        newAllAssets.Tag = browser;
                        UltraTreeNode valueNode = new UltraTreeNode(valueKey, version);
                        valueNode.Tag = newAllAssets;
                        valueNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;

                        if (browser.Contains("Internet Explorer"))
                        {
                            valueNode.LeftImages.Add(Properties.Resources.ie);
                        }
                        else if (browser.Contains("Chrome"))
                        {
                            valueNode.LeftImages.Add(Properties.Resources.chrome);
                        }
                        else if (browser.Contains("Firefox"))
                        {
                            valueNode.LeftImages.Add(Properties.Resources.firefox);
                        }
                        else if (browser.Contains("Safari"))
                        {
                            valueNode.LeftImages.Add(Properties.Resources.safari);
                        }
                        else if (browser.Contains("Opera"))
                        {
                            valueNode.LeftImages.Add(Properties.Resources.opera);
                        }

                        expandingNode.Nodes.Add(valueNode);
                    }
                }
            }
        }


        /// <summary>
        /// Get possible values for the Hardware Data Field defined by this node
        /// </summary>
        /// <param name="expandingNode"></param>
        protected void ExpandAllAssetsHardwareValues(UltraTreeNode expandingNode)
        {
            AuditedItemsDAO lwDataAccess = new AuditedItemsDAO();

            // Get the 'AllAssets' object tag
            AllAssets allAssets = expandingNode.Tag as AllAssets;

            // Recover the AssetGroup which is the tag of the All Assets and get a list of the assets
            // at or below this group in the heirarchy
            AssetGroup parentGroup = allAssets.ParentGroup;
            AssetList listChildAssets = parentGroup.GetAllAssets();

            // Split off the Hardware/System Category from the node key
            int delimiter = expandingNode.Key.LastIndexOf(allAssets.BranchName);
            if (delimiter == -1)
                return;

            // Get just the hardware/system part of the key
            string parentKey = expandingNode.Key.Substring(delimiter);

            // The last entry is the name of the value which we need to remove
            delimiter = parentKey.LastIndexOf('|');
            string hardwareCategory = parentKey.Substring(0, delimiter);
            string hardwareValue = parentKey.Substring(delimiter + 1);

            // Get ALL possible values for the specified hardware/system field
            DataTable table = lwDataAccess.GetAuditedItemValues(null, hardwareCategory, hardwareValue);

            // Move these value records into the tree noting that we may get the same values multiple time for
            // different assets and we need to be able to handle this situation.
            //
            // We also filter out values for assets which are not within this branch as we go
            foreach (DataRow row in table.Rows)
            {
                int assetId = (int)row["_ASSETID"];
                string itemValue = (string)row["_VALUE"];
                string valueKey = expandingNode.Key + "|" + itemValue;

                // Does this asset exist within the branch being displayed?
                Asset asset = listChildAssets.GetAssetById(assetId);
                if (asset == null)
                    continue;

                // Do we already have a node with this key in this branch?
                if (expandingNode.Nodes.Exists(valueKey))
                {
                    // Yes - add the asset to the existing node in the tree
                    UltraTreeNode valueNode = expandingNode.Nodes[valueKey];
                    AllAssets newAllAssets = valueNode.Tag as AllAssets;
                    if (newAllAssets != null) newAllAssets.ListAssets.Add(asset);
                }

                else
                {
                    // No - add a new node to the tree for this value and add the asset to it
                    AllAssets newAllAssets = new AllAssets(parentGroup, 0, itemValue);
                    newAllAssets.ListAssets.Add(asset);
                    UltraTreeNode valueNode = new UltraTreeNode(valueKey, itemValue);
                    valueNode.Tag = newAllAssets;
                    valueNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;
                    //valueNode.LeftImages.Add(expandingNode.LeftImages[0]);
                    expandingNode.Nodes.Add(valueNode);
                }
            }
        }

        /// <summary>
        /// This function is called when we are 'expanding' an application located beneath the 'All Assets'
        /// node.  Normally when we expand an item such as this we would be displaying the possible values 
        /// for the item however with applications there are no values as such - we simply need to know what
        /// assets (in the group) have the application installed.
        /// </summary>
        /// <param name="expandingNode"></param>
        protected void ExpandAllAssetsApplication(UltraTreeNode expandingNode)
        {
            // Get the 'AllAssets' object tag
            AllAssets allAssets = expandingNode.Tag as AllAssets;
            InstalledApplication application = allAssets.Tag as InstalledApplication;

            // Recover the AssetGroup which is the tag of the All Assets and get a list of the assets
            // at or below this group in the heirarchy
            AssetGroup parentGroup = allAssets.ParentGroup;
            AssetList listChildAssets = parentGroup.GetAllAssets();

            // We need to recover ALL instances of this application and then filter out those not in the 
            // selected group as we can't get just those for the group 
            application.LoadData();

            allAssets.ListAssets.Clear();

            // Add the entries to the tree 
            foreach (ApplicationInstance instance in application.Instances)
            {
                int assetID = instance.InstalledOnComputerID;
                Asset asset = listChildAssets.GetAssetById(assetID);
                if (asset != null)
                    allAssets.ListAssets.Add(asset);
            }
        }

        #endregion 'All Assets' Node Handling



        /// <summary>
        /// Expand a Tree Node which contains an Asset
        /// </summary>
        /// <param name="node"></param>
        public void ExpandAsset(UltraTreeNode expandingNode)
        {
            if (expandingNode.Nodes.Count != 0)
                return;

            // get the asset group and populate it
            Asset expandingAsset = expandingNode.Tag as Asset;

            // There are multiple branches here however only some of which we can expand
            TreeSelectionEventArgs.ITEMTYPE returnType = GetAssetNodeTypeFromKey(expandingNode);

            // Now act as required for the item being expanded...
            switch (returnType)
            {
                case TreeSelectionEventArgs.ITEMTYPE.asset:
                    ExpandAssetRoot(expandingNode);
                    break;

                case TreeSelectionEventArgs.ITEMTYPE.asset_auditdata_category:
                    ExpandAssetAuditData(expandingNode);
                    break;

                case TreeSelectionEventArgs.ITEMTYPE.asset_filesystem:
                    ExpandAssetFileSystem(expandingNode);
                    break;

                case TreeSelectionEventArgs.ITEMTYPE.asset_summary:
                case TreeSelectionEventArgs.ITEMTYPE.asset_applications:
                    break;

            }
        }



        /// <summary>
        /// Add the child items immediately beneath an Asset
        /// </summary>
        /// <param name="expandingNode"></param>
        private void ExpandAssetRoot(UltraTreeNode expandingNode)
        {
            // Begin update of the tree
            networkTree.BeginUpdate();

            try
            {
                // get the asset group and populate it
                Asset expandingAsset = expandingNode.Tag as Asset;

                // If this is an 'auditable' asset then we must add one set of items otherwise the
                // items are somewhat different
                //
                //	'Summary'
                //  'Operating System'
                //	'Applications'			
                //	'Hardware'				(Only if audited)
                //	'System'				(Only if audited)
                //  'Interet Explorer'		(Only if audited)
                //	'History'
                //  'User Defined Data category #1'
                //  ...
                //  'User Defined Data category #n'
                // All Assets have Summary
                //UltraTreeNode summaryNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.SummaryNode);
                //summaryNode.LeftImages.Add(Properties.Resources.computer16);
                //summaryNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;
                //summaryNode.Tag = expandingAsset;
                //expandingNode.Nodes.Add(summaryNode);

                expandingNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;

                if ((expandingAsset.Auditable) && (expandingAsset.LastAudit != DateTime.MinValue))
                {
                    //bool lAssetAudited = expandingAsset.LastAudit != DateTime.MinValue;

                    UltraTreeNode applicationsNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.ApplicationsNode);
                    applicationsNode.LeftImages.Add(Properties.Resources.application_16);
                    applicationsNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;
                    applicationsNode.Tag = expandingAsset;
                    expandingNode.Nodes.Add(applicationsNode);
                    //
                    UltraTreeNode hardwareNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.HardwareNode);
                    hardwareNode.LeftImages.Add(Properties.Resources.hardware);
                    //hardwareNode.Override.ShowExpansionIndicator = lAssetAudited ? ShowExpansionIndicator.Always : ShowExpansionIndicator.Never;
                    hardwareNode.Tag = expandingAsset;
                    expandingNode.Nodes.Add(hardwareNode);
                }

                if (expandingAsset.LastAudit != DateTime.MinValue)
                {
                    UltraTreeNode systemNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.SystemNode);
                    systemNode.LeftImages.Add(Properties.Resources.system_16);
                    //systemNode.Override.ShowExpansionIndicator = lAssetAudited ? ShowExpansionIndicator.Always : ShowExpansionIndicator.Never;
                    expandingNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Always;
                    systemNode.Tag = expandingAsset;
                    expandingNode.Nodes.Add(systemNode);
                }

                // Auditable Assets now get OS / Hardware / System / Internet Explorer / File System
                if ((expandingAsset.Auditable) && (expandingAsset.LastAudit != DateTime.MinValue))
                {
                    UltraTreeNode internetNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.InternetNode);
                    internetNode.LeftImages.Add(Properties.Resources.aw_internet);
                    //internetNode.Override.ShowExpansionIndicator = lAssetAudited ? ShowExpansionIndicator.Always : ShowExpansionIndicator.Never;
                    internetNode.Tag = expandingAsset;
                    expandingNode.Nodes.Add(internetNode);
                    //
                    UltraTreeNode filesystemNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.FileSystem);
                    filesystemNode.LeftImages.Add(Properties.Resources.filesystem_16);
                    //filesystemNode.Override.ShowExpansionIndicator = lAssetAudited ? ShowExpansionIndicator.Always : ShowExpansionIndicator.Never;
                    filesystemNode.Tag = expandingAsset;
                    expandingNode.Nodes.Add(filesystemNode);
                }

                // Although not auditable as such, USB and Mobile Devices may have file system and hardware branches so 
                // add these if that is the type of this asset - usb/mobile devices are always children
                else if (expandingAsset.ParentAssetID != 0)
                {
                    //
                    UltraTreeNode hardwareNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.HardwareNode);
                    hardwareNode.LeftImages.Add(Properties.Resources.hardware);
                    hardwareNode.Tag = expandingAsset;
                    expandingNode.Nodes.Add(hardwareNode);
                    //
                    UltraTreeNode filesystemNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.FileSystem);
                    filesystemNode.LeftImages.Add(Properties.Resources.filesystem_16);
                    filesystemNode.Tag = expandingAsset;
                    expandingNode.Nodes.Add(filesystemNode);
                }

                // All Assets get History and User Defined Data however we need to be careful with the user defined data 
                // as the available categories will depend on the asset type
                if (expandingAsset.LastAudit != DateTime.MinValue)
                {
                    UltraTreeNode historyNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, AWMiscStrings.HistoryNode);
                    historyNode.LeftImages.Add(Properties.Resources.history_16);
                    historyNode.Tag = expandingAsset;
                    historyNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;
                    expandingNode.Nodes.Add(historyNode);
                }

                // Add on the User Defined Data Categories (if any)
                //NetworkWorkItemController wiController = WorkItem.Controller as NetworkWorkItemController;
                //foreach (UserDataCategory category in wiController.UserDataCategories)
                //{
                //    // Is this category restricted to a specific asset category or type?
                //    if (wiController.CategoryAppliesTo(category, expandingAsset))
                //    {
                //        UltraTreeNode categoryNode = new UltraTreeNode(expandingNode.Key + "|" + AWMiscStrings.UserDataNode + "|" + category.Name, category.Name);
                //        categoryNode.LeftImages.Add(IconMapping.LoadIcon(category.Icon, IconMapping.ICONSIZE.small));
                //        categoryNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;
                //        category.Tag = expandingAsset;
                //        categoryNode.Tag = new UserDataCategory(category);
                //        expandingNode.Nodes.Add(categoryNode);
                //        expandingNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnExpand;
                //    }
                //}

                // Add any child assets
                AssetList childAssets = expandingAsset.ChildAssets;
                foreach (Asset childAsset in childAssets)
                {
                    // Create the new tree node for this asset noting again that we must be very careful about the
                    // key created for this child asset to ensure that it is unique
                    UltraTreeNode childNode = AuditWizardUtility.CreateKeyedTreeNode(expandingNode, childAsset);

                    // Recover the icon which we need to display for this asset
                    Bitmap icon = childAsset.DisplayIcon();
                    childNode.LeftImages.Add(icon);

                    // Set the tag for the node to be the 'asset' object
                    childNode.Tag = childAsset;

                    // ...and add the node to the tree
                    expandingNode.Nodes.Add(childNode);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayErrorMessage("Exception adding tree nodes in [NetworkExplorerView::ExpandAssetRoot], the exception text was " + ex.Message);
            }

            // End tree update
            networkTree.EndUpdate();
        }



        /// <summary>
        /// Add the child items beneath the 'Audited Data' node for an Asset
        /// </summary>
        /// <param name="expandingNode"></param>
        private void ExpandAssetAuditData(UltraTreeNode expandingNode)
        {
            networkTree.BeginUpdate();

            try
            {
                // get the parent asset 
                Asset expandingAsset = expandingNode.Tag as Asset;

                // We now need to add the categories to the tree
                foreach (AuditedItem item in expandingAsset.AuditedItems)
                {
                    // We pass the PARENT of the expanding node - which will be the node for the asset to ensure
                    // that we handle all data categories audited.
                    AddCategory(expandingNode.Parent, expandingAsset, item);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayErrorMessage("Exception adding tree nodes in [NetworkExplorerView::ExpandAssetRoot], the exception text was " + ex.Message);
            }

            networkTree.EndUpdate();
        }


        /// <summary>
        /// Add an Audited Data Category to the tree
        /// </summary>
        /// <param name="assetNode"></param>
        /// <param name="expandingAsset"></param>
        /// <param name="item"></param>
        protected void AddCategory(UltraTreeNode assetNode, Asset expandingAsset, AuditedItem item)
        {
            // Get the node being populated - this SHOULD be the asset node
            UltraTreeNode childNode = assetNode;

            // Split the name of the category passed to us into its components i.e. Hardware|Network is split into 
            // Hardware and Network
            List<string> listCategories = Utility.ListFromString(item.Category, '|', true);
            string childKey = assetNode.Key;

            // If this item is flagged as being grouped then we do not include the last segment of the item name
            // as this is the item on which we shall group 
            if (item.Grouped)
                listCategories.RemoveAt(listCategories.Count - 1);

            for (int index = 0; index < listCategories.Count; index++)
            {
                string name = listCategories[index];

                // Determine the key that should be set for this child category
                childKey = childKey + "|" + name;

                // If a node with this key already exists then we select the node
                if (childNode.Nodes.Exists(childKey))
                {
                    childNode = childNode.Nodes[childKey];
                }

                // If no child node with the specified key exists already then create one and add it below the
                // current child node
                else
                {
                    // Child node does not exist so create it
                    UltraTreeNode newNode = new UltraTreeNode(childKey, name);
                    //
                    newNode.LeftImages.Add(IconMapping.LoadIcon(item.Icon, IconMapping.Iconsize.Small));
                    newNode.Tag = expandingAsset;

                    // If we are adding the final category then we need to check whether or not this item will terminate
                    // the branch - that is if it is flagged as grouped or has a value.
                    if (index == (listCategories.Count - 1))
                    {
                        if (item.Grouped || item.Name != "")
                            newNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;
                    }

                    // Add the node
                    childNode.Nodes.Add(newNode);
                    childNode = newNode;
                }
            }
        }


        /// <summary>
        /// Add the child items beneath the 'FileSystem' node for an Asset
        /// </summary>
        /// <param name="expandingNode"></param>
        private void ExpandAssetFileSystem(UltraTreeNode expandingNode)
        {
            networkTree.BeginUpdate();

            try
            {
                // get the parent asset 
                Asset expandingAsset = expandingNode.Tag as Asset;

                // Does the asset have its FileSystem expanded?  If not do so
                FileSystemFolderList fileSystemFolders = expandingAsset.FileSystemFolders;

                // Are there any items in the FileSystem/  If not then we set the branch to be unexpandable
                if (fileSystemFolders.Count == 0)
                    expandingNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;

                // OK - we may as well add the entire FileSystem tree to the explorer tree to save time later on
                AddFileSystem(expandingNode, fileSystemFolders);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayErrorMessage("Exception adding tree nodes in [NetworkExplorerView::ExpandAssetFileSystem], the exception text was " + ex.Message);
            }

            networkTree.EndUpdate();
        }


        protected void AddFileSystem(UltraTreeNode expandingNode, List<FileSystemFolder> listFolders)
        {
            networkTree.BeginUpdate();

            try
            {
                Asset expandingAsset = expandingNode.Tag as Asset;
                //
                foreach (FileSystemFolder folder in listFolders)
                {
                    string childKey = expandingNode.Key + "Folder|" + folder.DisplayName;
                    UltraTreeNode folderNode = new UltraTreeNode(childKey, folder.DisplayName);
                    folderNode.LeftImages.Add(Properties.Resources.folder_16);
                    folderNode.Tag = folder;
                    expandingNode.Nodes.Add(folderNode);

                    if (folder.FoldersList.Count == 0)
                        folderNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.Never;

                    // Recurse to add any child folders/files
                    AddFileSystem(folderNode, folder.FoldersList);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayErrorMessage("Exception adding tree nodes in [NetworkExplorerView::AddFileSystem], the exception text was " + ex.Message);
            }

            networkTree.EndUpdate();

        }




        /// <summary>
        /// This function looks at the node key specified for an asset level node and returns its type
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TreeSelectionEventArgs.ITEMTYPE GetAssetNodeTypeFromKey(UltraTreeNode treeNode)
        {
            // We have not been able to identify the item from the TAG so look at the key now
            TreeSelectionEventArgs.ITEMTYPE returnType = TreeSelectionEventArgs.ITEMTYPE.asset_auditdata;

            // If no delimiter then this is the top level asset place-holder
            string key = treeNode.Key;
            Asset theAsset = treeNode.Tag as Asset;
            if ((treeNode.Tag is Asset) && (key.EndsWith(String.Format("{0}|{1}", theAsset.Name, theAsset.UniqueID))))
                returnType = TreeSelectionEventArgs.ITEMTYPE.asset;

            else if (key.Contains("|" + AWMiscStrings.SummaryNode))
                returnType = TreeSelectionEventArgs.ITEMTYPE.asset_summary;

            else if (key.Contains("|" + AWMiscStrings.ApplicationsNode))
                returnType = TreeSelectionEventArgs.ITEMTYPE.asset_applications;

            else if (key.Contains("|" + AWMiscStrings.InternetNode + "|"))
                returnType = TreeSelectionEventArgs.ITEMTYPE.asset_auditdata;

            else if (key.Contains("|" + AWMiscStrings.FileSystem))
                returnType = TreeSelectionEventArgs.ITEMTYPE.asset_filesystem;

            else if ((key.EndsWith("|" + AWMiscStrings.HardwareNode))
            || (key.EndsWith("|" + AWMiscStrings.SystemNode))
            || (key.EndsWith("|" + AWMiscStrings.OSNode))
            || (key.EndsWith("|" + AWMiscStrings.InternetNode)))
                returnType = TreeSelectionEventArgs.ITEMTYPE.asset_auditdata_category;

            else if (key.Contains("|" + AWMiscStrings.HistoryNode))
                returnType = TreeSelectionEventArgs.ITEMTYPE.asset_history;

            return returnType;
        }


        /// <summary>
        /// This function looks at the node key specified for an ALL ASSETS level node and returns its type
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TreeSelectionEventArgs.ITEMTYPE GetAllAssetNodeTypeFromKey(UltraTreeNode treeNode)
        {
            // Assume that we are at the top level of the 'All Assets' branch for now
            TreeSelectionEventArgs.ITEMTYPE returnType = TreeSelectionEventArgs.ITEMTYPE.asset_auditdata;

            // Split the key into its parts
            // Note these are formatted as location | All Assets | Item
            string key = treeNode.Key;
            List<string> keyParts = Utility.ListFromString(key, '|', true);
            int count = keyParts.Count;

            // If the last segment is 'All Assets' then we have selected the 'All Assets' placeholder
            if (keyParts[count - 1] == AWMiscStrings.AllAssetsNode)
                returnType = TreeSelectionEventArgs.ITEMTYPE.allassets;

            // If the key ENDS WITH 'Applications' then we should display a list of publishers	
            else if (keyParts[count - 1] == AWMiscStrings.ApplicationsNode)
                returnType = TreeSelectionEventArgs.ITEMTYPE.asset_applications;

            // If the next to last entry is Applications then we are displaying a Publisher
            // location>All Assets>Applications
            else if ((count == 4) && (keyParts[2] == AWMiscStrings.ApplicationsNode))
                returnType = TreeSelectionEventArgs.ITEMTYPE.asset_publisher;

            // If the next to last entry is Operating System then we are displaying an OS field
            // Location > All Assets > Operating System > Extended Name > Windows XP
            else if ((count == 5) && (keyParts[2] == AWMiscStrings.OSNode))
                returnType = TreeSelectionEventArgs.ITEMTYPE.asset_os;

            // If we have 5 entries and the 2nd is Applications then we have selected a specific application				
            // location>All Assets>Applications>Application
            else if ((count == 5) && (keyParts[2] == AWMiscStrings.ApplicationsNode))
                returnType = TreeSelectionEventArgs.ITEMTYPE.asset_application;

            else if (key.Contains("|" + AWMiscStrings.InternetNode + "|"))
                returnType = TreeSelectionEventArgs.ITEMTYPE.asset_auditdata;

            else if ((key.EndsWith("|" + AWMiscStrings.HardwareNode))
            || (key.EndsWith("|" + AWMiscStrings.SystemNode))
            || (key.EndsWith("|" + AWMiscStrings.InternetNode)))
                returnType = TreeSelectionEventArgs.ITEMTYPE.asset_auditdata_category;

            return returnType;
        }


        #endregion Tree Expansion / Population Functions


        /// <summary>
        /// This routine is responsible for returning a list of all of the assets which are currently
        /// selected within this tree noting that if a domain/location is selected that we have to return 
        /// all of the assets within that domain or child domains thereof.
        /// </summary>
        /// <returns></returns>
        public List<Asset> GetSelectedAssets()
        {
            List<Asset> selectedAssets = new List<Asset>();

            // Loop through the selected node(s)
            foreach (UltraTreeNode selectedNode in this.networkTree.SelectedNodes)
            {
                // Ok what type of item is it - if it is a location / domain then we need to identify ALL
                // child assets beneath this node
                if (selectedNode.Tag is AssetGroup)
                {
                    // Ensure that this node is fully populated
                    AssetGroup selectedGroup = selectedNode.Tag as AssetGroup;
                    selectedGroup.Populate(true, false, true);
                    EnumerateGroupAssets(selectedGroup, selectedAssets);
                }

                else if (selectedNode.Tag is Asset)
                {
                    selectedAssets.Add(selectedNode.Tag as Asset);
                }
            }

            return selectedAssets;
        }


        /// <summary>
        /// Add all of the child assets in the specified group - and all sub-groups to the supplied list
        /// </summary>
        /// <param name="listAssets"></param>
        protected void EnumerateGroupAssets(AssetGroup selectedGroup, List<Asset> listAssets)
        {
            foreach (AssetGroup childGroup in selectedGroup.Groups)
            {
                EnumerateGroupAssets(childGroup, listAssets);
            }

            // Add in direct child assets in this group
            foreach (Asset childAsset in selectedGroup.Assets)
            {
                listAssets.Add(childAsset);
            }
        }


        /// <summary>
        /// KeyDown handler to trap CTRL+F (Find)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void networkTree_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control) && (e.KeyCode == Keys.F))
            {
                e.Handled = true;
                findAssetToolStripMenuItem_Click(sender, null);
            }
        }
        // End of Class
    }
}
