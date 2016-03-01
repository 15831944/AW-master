using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Infragistics.Win.UltraWinTree;

namespace Layton.AuditWizard.Applications
{
    [SmartPart]
    public partial class ApplicationsExplorerView : UserControl, ILaytonView
    {
        private LaytonWorkItem _workItem;
        private ApplicationsExplorerViewPresenter _presenter;
        private UltraTreeNode _rootPublisherNode;
        private UltraTreeNode _rootOsNode;

        //Create a new instance of the DrawFilter class to handle drawing the DropHighlight/DropLines
        private UltraTree_DropHightLight_DrawFilter_Class UltraTree_DropHightLight_DrawFilter = new UltraTree_DropHightLight_DrawFilter_Class();

        [InjectionConstructor]
        public ApplicationsExplorerView([ServiceDependency] WorkItem workItem)
        {
            this._workItem = workItem as LaytonWorkItem;
            InitializeComponent();
            this.Paint += new PaintEventHandler(ApplicationsExplorerView_Paint);

            //Attach the Drawfilter to the tree
            applicationsTree.DrawFilter = UltraTree_DropHightLight_DrawFilter;
            UltraTree_DropHightLight_DrawFilter.Invalidate += new EventHandler(this.UltraTree_DropHightLight_DrawFilter_Invalidate);
            UltraTree_DropHightLight_DrawFilter.QueryStateAllowedForNode += new UltraTree_DropHightLight_DrawFilter_Class.QueryStateAllowedForNodeEventHandler(this.UltraTree_DropHightLight_DrawFilter_QueryStateAllowedForNode);

            // Set the display style foe the tree so that we can swap between vista and 'standard' trees
            AuditWizardConfiguration configuration = new AuditWizardConfiguration();
            this.applicationsTree.DisplayStyle = (configuration.VistaTrees) ? UltraTreeDisplayStyle.WindowsVista : UltraTreeDisplayStyle.Standard;

            // Add the root nodes to the tree for OS and Publishers - the tree is not sorted at the root so we add OS first
            applicationsTree.Override.Sort = SortType.None;
            _rootOsNode = applicationsTree.Nodes.Add(MiscStrings.OperatingSystems, MiscStrings.OperatingSystems);
            _rootOsNode.LeftImages.Add(Properties.Resources.os_16);
            _rootOsNode.Override.Sort = SortType.Descending;
            //
            _rootPublisherNode = applicationsTree.Nodes.Add(MiscStrings.AllPublishers, MiscStrings.AllPublishers);
            _rootPublisherNode.LeftImages.Add(Properties.Resources.application_view_16);
            _rootPublisherNode.Override.Sort = SortType.Descending;

            _rootPublisherNode.Expanded = true;
        }

        [CreateNew]
        public ApplicationsExplorerViewPresenter Presenter
        {
            set
            {
                _presenter = value;
                _presenter.View = this;
                _presenter.Initialize();

                // JML_LINDE
                //_rootPublisherNode.Selected = true;
            }
            get { return _presenter; }
        }

        public LaytonWorkItem WorkItem
        {
            get { return _workItem; }
        }

        public UltraTree ApplicationsTree
        {
            get { return applicationsTree; }
        }

        /// <summary>
        /// RefreshView
        /// ===========
        /// 
        /// Refresh this view 
        /// </summary>
        public void RefreshView()
        {
            // First save any existing selections
            SelectedNodesCollection selectedNodes = applicationsTree.SelectedNodes;

            // ...then save any 'expanded' nodes
            List<UltraTreeNode> expandedNodes = new List<UltraTreeNode>();
            GetExpandedNodes(expandedNodes, applicationsTree.Nodes);

            // ...then initialize the view
            _presenter.Initialize();

            // Call our base class refresh code next
            base.Refresh();

            // ...then reinstate any selections
            RestoreSelectedNodes(selectedNodes);

            // ...then re-instate any expansions
            RestoreExpandedNodes(expandedNodes);
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
        protected void RestoreSelectedNodes(SelectedNodesCollection selectedNodes)
        {
            foreach (UltraTreeNode node in selectedNodes)
            {
                UltraTreeNode selectedNode = applicationsTree.GetNodeByKey(node.Key);
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
                UltraTreeNode expandedNode = applicationsTree.GetNodeByKey(node.Key);
                if (expandedNode != null)
                {
                    if (expandedNode.Expanded == false)
                        expandedNode.Expanded = true;
                }
            }
        }


        /// <summary>
        /// Clear all contents from this tree view
        /// </summary>
        public void Clear()
        {
            this.applicationsTree.Nodes[AllPublishersNode.Key].Nodes.Clear();
            this.applicationsTree.Nodes[AllOperatingSystemsNode.Key].Nodes.Clear();
            //this.applicationsTree.Nodes[ActionsNode.Key].Nodes.Clear();
        }

        /// <summary>
        /// Add the 'All Publishers Node' to the root of the tree
        /// </summary>
        public UltraTreeNode AllPublishersNode
        {
            get { return _rootPublisherNode; }
        }

        /// <summary>
        /// Add the 'All Operating Systems Node' to the root of the tree
        /// </summary>
        public UltraTreeNode AllOperatingSystemsNode
        {
            get { return _rootOsNode; }
        }


        void ApplicationsExplorerView_Paint(object sender, PaintEventArgs e)
        {
            Image bgImg = Properties.Resources.application_view_ghosted_96;
            e.Graphics.DrawImage(bgImg, (this.Width - bgImg.Width - 30), (this.Height - bgImg.Height - 30));
        }

        private void applicationsTree_BeforeExpand(object sender, CancelableNodeEventArgs e)
        {
            UltraTreeNode node = e.TreeNode;

            if (node.Parent == null)
                return;

            node.Nodes.Clear();

            InstalledApplication theApplication;
            string key;

            if (node.Parent == AllPublishersNode)
            {
                _presenter.ExpandApplications(node);
            }
            else if (node.Parent.Parent == AllPublishersNode)
            {
                // here after clicking the application node - show licenses and installations

                string publisher = node.FullPath.Substring(20);
                theApplication = node.Tag as InstalledApplication;
                key = publisher + "|" + theApplication.Name + "|" + theApplication.ApplicationID;

                UltraTreeNode licensesNode = new UltraTreeNode(key + "|" + MiscStrings.ApplicationLicenseNode, "Licenses");
                licensesNode.Tag = theApplication;
                node.Nodes.Add(licensesNode);

                UltraTreeNode instancesNode = new UltraTreeNode(key + "|" + MiscStrings.ApplicationInstanceNode, "Installations");
                instancesNode.Tag = theApplication;
                node.Nodes.Add(instancesNode);

                theApplication.LoadData();
            }
            else if (node.Parent == AllOperatingSystemsNode)
            {
                // Beneath each Operating System we have placeholders for licenses and instances which 
                // we can add now as two sub-nodes
                key = node.Key;

                InstalledOS theOS = node.Tag as InstalledOS;

                UltraTreeNode licensesNode = new UltraTreeNode(key + "|" + MiscStrings.ApplicationLicenseNode, "Licenses");
                licensesNode.LeftImages.Add(Properties.Resources.application_license_16);
                licensesNode.Tag = theOS;
                node.Nodes.Add(licensesNode);
                //
                UltraTreeNode instancesNode = new UltraTreeNode(key + "|" + MiscStrings.ApplicationInstanceNode, "Installations");
                instancesNode.LeftImages.Add(Properties.Resources.computer16);
                instancesNode.Tag = theOS;
                node.Nodes.Add(instancesNode);
            }
        }


        #region Event declarations and handlers

        /// <summary>
        /// Event declaration for when a publisher has been selected in the tree view.
        /// </summary>
        [EventPublication(EventTopics.PublisherSelectionChanged, PublicationScope.WorkItem)]
        public event EventHandler<PublishersEventArgs> PublisherSelectionChanged;

        /// <summary>
        /// Event declaration for when an ApplicationID has been selected in the tree view.
        /// </summary>
        [EventPublication(EventTopics.ApplicationSelectionChanged, PublicationScope.WorkItem)]
        public event EventHandler<ApplicationsEventArgs> ApplicationSelectionChanged;

        /// <summary>
        /// Event declaration for when an ApplicationID>License node has been selected in the tree view.
        /// </summary>
        [EventPublication(EventTopics.ApplicationLicenseSelectionChanged, PublicationScope.WorkItem)]
        public event EventHandler<ApplicationLicenseEventArgs> ApplicationLicenseSelectionChanged;

        /// <summary>
        /// Event declaration for when an ApplicationID>Installations node has been selected in the tree view.
        /// </summary>
        [EventPublication(EventTopics.ApplicationInstallsSelectionChanged, PublicationScope.WorkItem)]
        public event EventHandler<ApplicationInstallsEventArgs> ApplicationInstallsSelectionChanged;

        /// <summary>
        /// Event declaration for when an Operating System has been selected in the tree view.
        /// </summary>
        [EventPublication(EventTopics.OperatingSystemSelectionChanged, PublicationScope.WorkItem)]
        public event EventHandler<OperatingSystemEventArgs> OperatingSystemSelectionChanged;

        /// <summary>
        /// Event declaration for when the Actions node has been selected in the tree view.
        /// </summary>
        [EventPublication(EventTopics.ActionsSelectionChanged, PublicationScope.WorkItem)]
        public event EventHandler<ActionsEventArgs> ActionsSelectionChanged;

        /// <summary>
        /// Called after we select a different node in the Explorer Tree
        /// This function deals with the firing of the appropriate event to inform interested parties
        /// of the change in selection so that they may update themselves accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationTree_AfterSelect(object sender, SelectEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (e.NewSelections.Count > 0)
            {
                UltraTreeNode node = e.NewSelections[0];

                // check if the 'All Publishers' node is selected and fire the PublisherSelectedChanged event
                // to inform other interested parties
                if (node.Key == MiscStrings.AllPublishers)
                {
                    List<ApplicationPublisher> allPublishers = new List<ApplicationPublisher>();
                    //allPublishers.Add(_presenter.FindPublisher(node.Key));

                    ApplicationPublisher thePublisher = new ApplicationPublisher(node.Key, 0);
                    //thePublisher.Populate(true, false);
                    allPublishers.Add(thePublisher);

                    if (PublisherSelectionChanged != null)
                        PublisherSelectionChanged(this, new PublishersEventArgs(allPublishers));
                }

                // If we have clicked on the Operating systems node and fire the OperatingSystemSelecyionChanged 
                // event to inform other interested parties
                else if (node.Key == MiscStrings.OperatingSystems)
                {
                    List<InstalledOS> allPublishers = new List<InstalledOS>();
                    allPublishers.Add(null);

                    if (OperatingSystemSelectionChanged != null)
                        OperatingSystemSelectionChanged(this, new OperatingSystemEventArgs(allPublishers));
                }

                // If we have clicked on the Actions node and fire the ActionsSelectionChanged 
                // event to inform other interestted parties
                else if (node.Key == MiscStrings.Actions)
                {
                    if (ActionsSelectionChanged != null)
                        ActionsSelectionChanged(this, new ActionsEventArgs());
                }

                // If the parent is the 'All Publishers' node, then we have selected a publisher in the tree
                // - populate the list of publishers in the tree and fire the PublisherSelectionChanged 
                // event.
                else if (node.Parent == AllPublishersNode)
                {
                    // fire PublisherSelectionChanged event
                    List<ApplicationPublisher> allPublishers = new List<ApplicationPublisher>();
                    foreach (UltraTreeNode publisherNode in e.NewSelections)
                    {
                        ApplicationPublisher thePublisher = new ApplicationPublisher(publisherNode.Key, 0);
                        //thePublisher.Populate(true, false);
                        allPublishers.Add(thePublisher);
                    }

                    if (PublisherSelectionChanged != null)
                        PublisherSelectionChanged(this, new PublishersEventArgs(allPublishers));
                }

                // If the parent is the 'All Operating Systems' node, then we have selected an OS in the tree
                // Fire the OperatingSystemSelectionChanged event passing the selected OS.
                else if (node.Parent == AllOperatingSystemsNode)
                {
                    List<InstalledOS> listOSs = new List<InstalledOS>();
                    foreach (UltraTreeNode osNode in e.NewSelections)
                    {
                        InstalledOS thisOS = osNode.Tag as InstalledOS;
                        thisOS.LoadData();
                        listOSs.Add(thisOS);
                    }

                    if (OperatingSystemSelectionChanged != null)
                        OperatingSystemSelectionChanged(this, new OperatingSystemEventArgs(listOSs));

                    //UltraTreeNode OSNode = e.NewSelections[0] as UltraTreeNode;
                    //InstalledOS thisOS = OSNode.Tag as InstalledOS;
                    //if (OperatingSystemSelectionChanged != null)
                    //    OperatingSystemSelectionChanged(this, new OperatingSystemEventArgs(thisOS));
                }


                // Applications have a parent of publisher and grand-parent of the root node
                // So if our grand parent is the root node we know that we are an application
                else if (node.Parent.Parent == AllPublishersNode)
                {
                    List<InstalledApplication> listApplications = new List<InstalledApplication>();
                    foreach (UltraTreeNode applicationNode in e.NewSelections)
                    {
                        InstalledApplication thisApplication = applicationNode.Tag as InstalledApplication;
                        thisApplication.LoadData();
                        listApplications.Add(thisApplication);
                    }

                    // fire ApplicationSelectionChanged event
                    if (ApplicationSelectionChanged != null)
                        ApplicationSelectionChanged(this, new ApplicationsEventArgs(listApplications));
                }

                // OK we must be right at the bottom of the heirarchy displaying application/OS instances
                // and licenses nodes - determine which by checking the last entry in the key
                // If we have selected the 'licenses' node beneath an application/OS then we fire the 
                // 'ApplicationLicenseSelectionChanged' event
                else if (node.Key.EndsWith(MiscStrings.ApplicationLicenseNode))
                {
                    Object nodeTag = node.Tag;

                    // fire ApplicationLicenseSelectionChanged event
                    if (ApplicationLicenseSelectionChanged != null)
                        ApplicationLicenseSelectionChanged(this, new ApplicationLicenseEventArgs(nodeTag));
                }

                else if (node.Key.EndsWith(MiscStrings.ApplicationInstanceNode))
                {
                    Object nodeTag = node.Tag;

                    // fire ApplicationLicenseSelectionChanged event
                    if (ApplicationInstallsSelectionChanged != null)
                        ApplicationInstallsSelectionChanged(this, new ApplicationInstallsEventArgs(nodeTag));
                }
            }

            Cursor.Current = Cursors.Default;
        }

        #endregion

        #region Context Menu Handlers

        /// <summary>
        /// Determine which context menu items are valid for the currently selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsTreeMenu_Opening(object sender, CancelEventArgs e)
        {
            // To save coding we assume all items are disabled initially
            ignoreAppToolStripMenuItem.Enabled = false;
            includeAppToolStripMenuItem.Enabled = false;
            newlicenseToolStripMenuItem.Enabled = false;
            editLicenseToolStripMenuItem.Enabled = false;
            propertiesToolStripMenuItem.Enabled = false;
            newApplicationToolStripMenuItem.Enabled = false;
            deleteApplicationToolStripMenuItem.Enabled = false;
            deleteApplicationToolStripMenuItem.Text = "Delete Application(s)";
            aliasToolStripMenuItem.Enabled = false;
            editPublisherToolStripMenuItem.Visible = false;

            // If we have nothing selected then no menu items are valid
            if (applicationsTree.SelectedNodes.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            // OK - get the selected node and flag that we want the menu
            UltraTreeNode node = applicationsTree.SelectedNodes[0];
            e.Cancel = false;

            // Root Item?  We can only add publishers
            if (node.Parent == null && node.Text != "Operating Systems")
            {
                newApplicationToolStripMenuItem.Enabled = true;
            }

            // Have we selected a publisher?  If so we can hide/show applications
            else if ((node.Parent != null) && (node.Parent == AllPublishersNode))
            {
                ignoreAppToolStripMenuItem.Enabled = true;
                ignoreAppToolStripMenuItem.Text = ToolNames.SetIgnored;
                ignoreAppToolStripMenuItem.ToolTipText = ToolNames.SetIgnoredTooltip;
                includeAppToolStripMenuItem.Enabled = true;
                includeAppToolStripMenuItem.Text = ToolNames.SetIncluded;
                includeAppToolStripMenuItem.ToolTipText = ToolNames.SetIncludedTooltip;
                newApplicationToolStripMenuItem.Enabled = true;
                aliasToolStripMenuItem.Enabled = true;
                newlicenseToolStripMenuItem.Enabled = false;
                deleteApplicationToolStripMenuItem.Enabled = true;
                deleteApplicationToolStripMenuItem.Text = "Delete Publisher(s)";
                editPublisherToolStripMenuItem.Visible = true;
            }

            // Have we selected an application?  If so then we can show/hide it and create a new license
            // Applications have a parent of publisher and grand-parent of the root node
            else if ((node.Parent != null) && (node.Parent.Parent == AllPublishersNode))
            {
                // Set as Ignored/Not Ignored options are mutually exclusive so we need to determine which one 
                // is valid and which is not
                if (node.Tag is InstalledApplication)
                {
                    InstalledApplication installedApplication = node.Tag as InstalledApplication;
                    ignoreAppToolStripMenuItem.Enabled = !(installedApplication.IsIgnored);
                    includeAppToolStripMenuItem.Enabled = !ignoreAppToolStripMenuItem.Enabled;
                }
                newlicenseToolStripMenuItem.Enabled = true;
                deleteApplicationToolStripMenuItem.Enabled = true;
                aliasToolStripMenuItem.Enabled = true;

                // We can select properties if we only have a single item selected
                if (applicationsTree.SelectedNodes.Count == 1)
                    propertiesToolStripMenuItem.Enabled = true;
            }

            // Have we selected an operating system?  If so we can hide/show and create new
            // licenses
            else if ((node.Parent != null) && (node.Parent == AllOperatingSystemsNode))
            {
                // Operating systems must always be licensed
                ignoreAppToolStripMenuItem.Enabled = false;
                includeAppToolStripMenuItem.Enabled = false;
                newlicenseToolStripMenuItem.Enabled = true;
                aliasToolStripMenuItem.Enabled = true;
            }

            // OK we must be right at the bottom of the heirarchy displaying application instances
            // and licenses nodes - determine which by checking the last entry in the key
            // If we have selected the 'licenses' node then we can add a license
            else if (node.Key.EndsWith(MiscStrings.ApplicationLicenseNode))
            {
                newlicenseToolStripMenuItem.Enabled = true;
            }
        }


        /// <summary>
        /// Called to hide the selected Applications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IgnoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ApplicationsWorkItemController)WorkItem.Controller).SetIgnored();
        }


        /// <summary>
        /// Called to show the selected applications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IncludeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ApplicationsWorkItemController)WorkItem.Controller).SetIncluded();
        }


        /// <summary>
        /// Called to create a new license for the selected application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newlicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ApplicationsWorkItemController)WorkItem.Controller).NewLicense();
        }


        /// <summary>
        /// Called to edit the selected license
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }


        /// <summary>
        /// Called to display the properties of an application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ApplicationsWorkItemController)WorkItem.Controller).ApplicationProperties();
        }


        /// <summary>
        /// Called to allow the user to define their own applications - this must not conflict with any existing names
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the currently selected node as we may be able to initialize the publisher for this application
            UltraTreeNode node = applicationsTree.SelectedNodes[0];

            // Can we identify a specific publisher?
            string publisher = "";

            if (node == AllPublishersNode)
                publisher = "";

            else if ((node.Parent != null) && (node.Parent == AllPublishersNode))
                publisher = node.Text;

            else if ((node.Parent != null) && (node.Parent.Parent != null) && (node.Parent.Parent == AllPublishersNode))
                publisher = node.Parent.Text;

            // Now display the 'Add Application' form			
            ((ApplicationsWorkItemController)WorkItem.Controller).AddApplication(publisher);
        }

        private void deleteApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (applicationsTree.SelectedNodes[0].Tag.ToString() == "PUBLISHER")
            {
                if (MessageBox.Show(
                    "Are you sure you want to delete this publisher(s)?" + Environment.NewLine + Environment.NewLine +
                    "All applications and associated items will also be deleted.",
                    "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }
            else
            {
                if (MessageBox.Show(
                    "Are you sure you want to delete this application?" + Environment.NewLine + Environment.NewLine +
                    "All associated items will also be deleted.",
                    "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            // can only be here from Publishers or Applications
            List<int> appIdsToDelete = GetSelectedApplications();

            ((ApplicationsWorkItemController)WorkItem.Controller).DeleteApplication(appIdsToDelete);
        }

        private void editPublisherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UltraTreeNode node = applicationsTree.SelectedNodes[0];
            ((ApplicationsWorkItemController)WorkItem.Controller).EditPublisherName(node.Text);
        }

        private List<int> GetSelectedApplications()
        {
            List<int> appIdsToDelete = new List<int>();

            if (applicationsTree.SelectedNodes[0].Tag.ToString() == "PUBLISHER")
            {
                ApplicationsDAO lApplicationsDAO = new ApplicationsDAO();
                foreach (UltraTreeNode node in applicationsTree.SelectedNodes)
                {
                    DataTable dt = lApplicationsDAO.GetApplicationIdsByPublisherName(node.Text);

                    foreach (DataRow row in dt.Rows)
                    {
                        appIdsToDelete.Add((int)row[0]);
                    }
                }
            }
            else
            {
                foreach (UltraTreeNode node in applicationsTree.SelectedNodes)
                {
                    InstalledApplication installedApplication = node.Tag as InstalledApplication;
                    if (installedApplication != null)
                        appIdsToDelete.Add(installedApplication.ApplicationID);
                }
            }
            return appIdsToDelete;
        }

        private void aliasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int> appsToAlias = GetSelectedApplications();
            ((ApplicationsWorkItemController)WorkItem.Controller).AliasApplication(appsToAlias);
        }

        #endregion Context Menu Handlers

        #region Drag and Drop Support

        /// <summary>
        /// Start of the drag and drop process - note that we can only drag applications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsTree_SelectionDragStart(object sender, EventArgs e)
        {
            // Construct a list of the applications being dragged
            List<InstalledApplication> listDraggedApplications = new List<InstalledApplication>();
            foreach (UltraTreeNode selectedNode in applicationsTree.SelectedNodes)
            {
                // Add any applications selected to our draggable list
                //if ((selectedNode.Tag is InstalledApplication) && (selectedNode.Parent.Tag is ApplicationPublisher))
                if ((selectedNode.Tag is InstalledApplication) && (selectedNode.Parent.Tag == null))
                    listDraggedApplications.Add(selectedNode.Tag as InstalledApplication);
            }

            // If the tag is an Application object and our parent is a publisher then this node must be an 
            // application and as such can be dragged and dropped
            if (listDraggedApplications.Count != 0)
                applicationsTree.DoDragDrop(listDraggedApplications, DragDropEffects.Move);
        }

        /// <summary>
        /// Called when we drop the item being dragged onto a node - we can only drop onto a publisher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsTree_DragDrop(object sender, DragEventArgs e)
        {
            //Get the position of the mouse in the tree, as opposed to form coords
            Point currentPoint = applicationsTree.PointToClient(new Point(e.X, e.Y));

            //Get the node the mouse is over.
            UltraTreeNode node = applicationsTree.GetNodeFromPoint(currentPoint);

            // Get the data being dropped - this should be a list of ApplicationInstance objects
            List<InstalledApplication> listDroppedApplications = (List<InstalledApplication>)e.Data.GetData(typeof(List<InstalledApplication>));

            // ...and do the drop
            if (IsPublisherNode(node))
            {
                ((ApplicationsWorkItemController)WorkItem.Controller).ChangeApplicationPublisher(listDroppedApplications, node.Text);
            }
            else
            {
                MessageBox.Show("Applications can only be dragged to a Publisher", "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //After the drop is complete, erase the current drop highlight. 
            UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
        }


        /// <summary>
        /// Called as we drag over a node - we must indicate whether or not the node is a valid drop target
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsTree_DragOver(object sender, DragEventArgs e)
        {
            //Get the position of the mouse in the tree, as opposed to form coords
            Point currentPoint = applicationsTree.PointToClient(new Point(e.X, e.Y));

            //Get the node the mouse is over.
            UltraTreeNode node = applicationsTree.GetNodeFromPoint(currentPoint);

            //Make sure the mouse is over a node
            if (IsPublisherNode(node))
            {
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
            applicationsTree.Invalidate();
        }



        /// <summary>
        /// This event is fired by the DrawFilter to let us determine what kinds of drops we want to allow 
        /// on any particular node - we only allow dropping on Publishers at this time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UltraTree_DropHightLight_DrawFilter_QueryStateAllowedForNode(Object sender, UltraTree_DropHightLight_DrawFilter_Class.QueryStateAllowedForNodeEventArgs e)
        {
            e.StatesAllowed = IsPublisherNode(e.Node) ? DropLinePositionEnum.OnNode : DropLinePositionEnum.None;
        }


        /// <summary>
        /// Fires when the user drags outside the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsTree_DragLeave(object sender, System.EventArgs e)
        {
            // When the mouse goes outside the control, clear the drophighlight. 
            // Since the DropHighlight is cleared when the mouse is not over a node, anyway,  this is probably 
            // not needed but, just in case the user goes from a node directly off the control...
            UltraTree_DropHightLight_DrawFilter.ClearDropHighlight();
        }


        /// <summary>
        /// Trap mouse down when on the tree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //Get the position of the mouse
                Point currentPoint = new Point(e.X, e.Y);

                //Get the node the mouse is over.
                UltraTreeNode node = applicationsTree.GetNodeFromPoint(currentPoint);

                if (node != null)
                {
                    if (!applicationsTree.SelectedNodes.Contains(node))
                    {
                        applicationsTree.SelectedNodes.Clear();
                        node.Selected = true;
                    }
                }
            }
        }

        private static bool IsPublisherNode(UltraTreeNode node)
        {
            return (node != null && node.Parent != null && node.Parent.Text == MiscStrings.AllPublishers);
        }

        #endregion
    }
}
