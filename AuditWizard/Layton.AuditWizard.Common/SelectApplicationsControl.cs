using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win.UltraWinTree;
//
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
    public partial class SelectApplicationsControl : UserControl
    {
        #region Data
        public enum eSelectionType { all, singleapplication, multipleapplications };

        public const string AllPublishers = "Software Publishers";
        protected UltraTreeNode _rootPublisherNode;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _showIncluded;
        private bool _showIgnored;

        /// <summary>This flag controls what can be selected within the control</summary>
        private eSelectionType _selectionType = eSelectionType.all;

        #endregion Data

        #region Properties
        public eSelectionType SelectionType
        {
            get { return _selectionType; }
            set { _selectionType = value; }
        }

        #endregion Properties

        #region Constructor

        public SelectApplicationsControl()
        {
            InitializeComponent();
            _showIncluded = true;
            _showIgnored = false;            
        }

        #endregion Constructor

        #region Methods

        public void PopulatePublishers(string publisherFilter, bool showIncluded, bool showIgnored, bool aShowOS)
        {
            try
            {
                _showIncluded = showIncluded;
                _showIgnored = showIgnored; 

                // Handle the selection type as this determine if and where we can have check boxes
                if (_selectionType == eSelectionType.singleapplication)
                    applicationsTree.Override.NodeStyle = NodeStyle.Standard;
                else
                    applicationsTree.Override.NodeStyle = NodeStyle.CheckBox;

                // begin updating the tree
                applicationsTree.BeginUpdate();
                applicationsTree.Nodes.Clear();

                // add the root node which is ALL Publishers
                UltraTreeNode rootNode = applicationsTree.Nodes.Add(AllPublishers, AllPublishers);
                rootNode.LeftImages.Add(Properties.Resources.application_view_16);

                // Only allow the root node to be selected if selection type is all
                if (_selectionType == eSelectionType.all)
                    rootNode.CheckedState = CheckState.Unchecked;
                else
                    rootNode.Override.NodeStyle = NodeStyle.Standard;

                //
                UltraTreeNode publisherNode;

                DataTable dt = new ApplicationsDAO().GetAllPublisherNamesAsDataTable(publisherFilter, showIncluded, showIgnored);
                UltraTreeNode[] publisherNodes = new UltraTreeNode[dt.Rows.Count];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string lPublisher = dt.Rows[i][0].ToString();

                    publisherNode = new UltraTreeNode(rootNode.Key + @"|" + lPublisher, lPublisher);
                    publisherNode.Tag = rootNode.Tag;
                    publisherNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnExpand;

                    publisherNodes[i] = publisherNode;
                }

                rootNode.Nodes.AddRange(publisherNodes);

                rootNode.Expanded = true;
            }
            finally
            {
                applicationsTree.EndUpdate();
            }
        }

        protected void PopulateApplications(UltraTreeNode publisherNode)
        {
            publisherNode.Nodes.Clear();

            string publisher = publisherNode.FullPath.Substring(20);
            string key;
            UltraTreeNode applicationNode;
            InstalledApplication theApplication;

            DataTable dt = new ApplicationsDAO().GetApplicationsByPublisher(publisher, _showIncluded, _showIgnored);
            UltraTreeNode[] applicationNodes = new UltraTreeNode[dt.Rows.Count];

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                theApplication = new InstalledApplication(dt.Rows[j]);

                if (theApplication.Version != String.Empty)
                {
                    key = publisherNode.Key + @"|" + theApplication.Name + " (v" + theApplication.Version + ")" + "|" + theApplication.ApplicationID;
                    applicationNode = new UltraTreeNode(key, theApplication.Name + " (v" + theApplication.Version + ")");
                }
                else
                {
                    key = publisherNode.Key + @"|" + theApplication.Name + "|" + theApplication.ApplicationID;
                    applicationNode = new UltraTreeNode(key, theApplication.Name);
                }

                applicationNode.Tag = theApplication;

                if (_selectionType == eSelectionType.singleapplication)
                    applicationNode.Override.NodeStyle = NodeStyle.Standard;
                else
                    applicationNode.Override.NodeStyle = NodeStyle.CheckBox;

                applicationNodes[j] = applicationNode;
            }

            publisherNode.Nodes.AddRange(applicationNodes);
        }

        /// <summary>
        /// This function is called to restore a previously saved set of checked Publishers and Applications
        /// </summary>
        /// <param name="selectedPublishers"></param>
        /// <param name="selectedApplications"></param>
        public void RestoreSelections(string selectedPublishers, string selectedApplications)
        {
            if (_selectionType != eSelectionType.all)
                return;

            // begin updating the tree
            applicationsTree.BeginUpdate();

            // Is the entire tree checked?
            bool selectAll = ((selectedPublishers == "") && (selectedApplications == ""));
            applicationsTree.Nodes[0].CheckedState = (selectAll) ? CheckState.Checked : CheckState.Unchecked;

            // We iterate through each of the Publishers first
            List<string> listPublishers = Utility.ListFromString(selectedPublishers, ';', true);
            //
            foreach (string checkedPublisher in listPublishers)
            {
                // Find the node containing this Publisher
                UltraTreeNode node = FindPublisher(checkedPublisher);
                if (node != null)
                    node.CheckedState = CheckState.Checked;
            }

            // Now iterate through each of the child applications
            List<string> listApplications = Utility.ListFromString(selectedApplications, ';', true);
            //
            foreach (string checkedApplication in listApplications)
            {
                // Find the node containing this application
                UltraTreeNode node = FindApplication(checkedApplication);
                if (node != null)
                    node.CheckedState = CheckState.Checked;
            }

            // end updating the tree
            applicationsTree.EndUpdate();
        }


        /// <summary>
        /// Search the entire tree looking for the node containing the specified Publisher
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected UltraTreeNode FindPublisher(string publisherToFind)
        {
            UltraTreeNode rootNode = applicationsTree.Nodes[0];
            foreach (UltraTreeNode childNode in rootNode.Nodes)
            {
                if ((childNode.Tag as ApplicationPublisher).Name == publisherToFind)
                    return childNode;
            }
            return null;
        }



        /// <summary>
        /// Search the entire tree looking for the node containing the specified application
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected UltraTreeNode FindApplication(string applicationToFind)
        {
            UltraTreeNode rootNode = applicationsTree.Nodes[0];
            foreach (UltraTreeNode publisherNode in rootNode.Nodes)
            {
                foreach (UltraTreeNode applicationNode in publisherNode.Nodes)
                {
                    if ((applicationNode.Tag as InstalledApplication).Name == applicationToFind)
                        return applicationNode;
                }
            }

            return null;
        }


        /// <summary>
        /// Return lists of all publishers and applications selected in the tree
        /// </summary>
        /// <param name="listSelectedPublishers">return list of publishers selected</param>
        /// <param name="listSelectedApplications">return list of applications selected</param>
        /// <returns>true if all nodes selected, false if specific nodes selected and returned in lists</returns>
        /// 
        public bool GetSelectedItems(out ApplicationPublisherList listSelectedPublishers, out InstalledApplicationList listSelectedApplications)
        {
            // Allocate the lists that we will return
            listSelectedPublishers = new ApplicationPublisherList();
            listSelectedApplications = new InstalledApplicationList();

            // Are all nodes selected as this is SO much easier to handle as we just return TRUE to indicate that
            // all nodes have been selected - the return lists are empty
            UltraTreeNode rootNode = applicationsTree.Nodes[0];
            if (rootNode.CheckedState == CheckState.Checked)
                return true;

            // OK unfortunately not all nodes are selected so we now need to populate the lists based on what has 
            // been selected.  Note that if a node is checked then by definition ALL applications beneath that
            // node are deemed to be checked but we do not bother iterating them
            GetSelectedItems(listSelectedPublishers, listSelectedApplications);

            // return false to show that there is a selection
            return false;
        }


        /// <summary>
        /// Return the application selected when we are in single selection mode
        /// </summary>
        /// <returns></returns>
        public InstalledApplication GetSelectedApplication()
        {
            // Only applicable for single selection so ensure we only have one node selected
            if (applicationsTree.SelectedNodes.Count != 1)
                return null;

            // ...and that it is an application
            UltraTreeNode selectedNode = applicationsTree.SelectedNodes[0];
            if (selectedNode.Tag is InstalledApplication)
                return selectedNode.Tag as InstalledApplication;
            return null;
        }

        /// <summary>
        /// Recursive routine to traverse the locations tree and build up a list of the Asset Groups and Assets
        /// which are checked
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="listSelectedGroups"></param>
        /// <param name="listSelectedAssets"></param>
        protected void GetSelectedItems(ApplicationPublisherList listSelectedPublishers, InstalledApplicationList listSelectedApplications)
        {
            UltraTreeNode rootNode = applicationsTree.Nodes[0];

            // Publishers first
            foreach (UltraTreeNode publisherNode in rootNode.Nodes)
            {
                foreach (UltraTreeNode applicationNode in publisherNode.Nodes)
                {
                    if (applicationNode.CheckedState == CheckState.Checked)
                        listSelectedApplications.Add(applicationNode.Tag as InstalledApplication);
                }
            }
        }

        #endregion Methods

        #region Tree Control Handlers


        /// <summary>
        /// We do not want to allow the user to explicitely set the check state to indeterminate
        /// as this is a pre-set state based on the state of child items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationsTree_BeforeCheck(object sender, BeforeCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Indeterminate)
                e.Cancel = false;
        }



        /// <summary>
        /// Handles the UltraTree's AfterCheck event.
        /// We need to propogate the change to our children and parents as necessary
        /// </summary>
        private void applicationsTree_AfterCheck(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {
            // Disable checking node states while we play around with them
            applicationsTree.AfterCheck -= applicationsTree_AfterCheck;

            // Update check state for parents / children of this node
            UpdateStates(e.TreeNode);

            // Update our parent(s) state
            VerifyParentNodeCheckState(e.TreeNode);

            // Re-establish the AfterCheck event
            applicationsTree.AfterCheck += new Infragistics.Win.UltraWinTree.AfterNodeChangedEventHandler(this.applicationsTree_AfterCheck);
        }


        /// <summary>
        /// Called as we change the state of a node - we must propogate this state down to all
        /// of our children
        /// </summary>
        /// <param name="forNode"></param>
        private void UpdateStates(UltraTreeNode forNode)
        {
            // Set out children to the same state as we are propogating the change down the line
            foreach (UltraTreeNode childNode in forNode.Nodes)
            {
                childNode.CheckedState = forNode.CheckedState;
                UpdateStates(childNode);
            }
        }



        /// <summary>
        /// Verify the check state of the parent9s) of the specified child node
        /// </summary>
        /// <param name="childNode"></param>
        private void VerifyParentNodeCheckState(UltraTreeNode childNode)
        {
            // get the parent node and return if it is null (top of tree)
            UltraTreeNode parentNode = childNode.Parent;
            if (parentNode == null)
                return;

            // Save the parent's current state
            CheckState currentState = parentNode.CheckedState;
            CheckState childNewState = childNode.CheckedState;
            bool notifyParent = false;

            // Get the parent state based on the current state of its children
            CheckState newState = GetOverallChildState(parentNode);

            // We must notify our parent if our status is going to change
            notifyParent = (newState != currentState);

            // should we notify the parent? ( has our state changed? )
            if (notifyParent)
            {
                // change state
                parentNode.CheckedState = newState;
                if (parentNode.Parent != null)
                    VerifyParentNodeCheckState(parentNode);
            }
        }



        /// <summary>
        /// Traverse the children beneath the specified node and check their individual state
        /// If ALL are checked then the return state is checked
        /// If ALL are unchecked then the return state is unchecked
        /// Otherwise return state is indeterminate
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        private CheckState GetOverallChildState(UltraTreeNode parentNode)
        {
            CheckState returnState;

            // Get the child count
            int childCount = parentNode.Nodes.Count;

            // Sanity check - we should never get here with no children but who knows
            if (childCount == 0)
                returnState = CheckState.Indeterminate;

            else if (childCount == 1)
                returnState = parentNode.Nodes[0].CheckedState;

            else
            {
                // Start off with the check state of the first child	
                returnState = parentNode.Nodes[0].CheckedState;

                // ...and loop through the rest
                for (int index = 1; index < parentNode.Nodes.Count; index++)
                {
                    if (returnState != parentNode.Nodes[index].CheckedState)
                    {
                        returnState = CheckState.Indeterminate;
                        break;
                    }
                }
            }

            return returnState;
        }


        #endregion Tree Control Handlers

        private void applicationsTree_BeforeExpand(object sender, CancelableNodeEventArgs e)
        {
            UltraTreeNode node = e.TreeNode;

            if (node.FullPath.StartsWith("Software Publishers\\"))
            {
                PopulateApplications(node);
            }
        }
    }
}
