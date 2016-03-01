using System;
using System.Data;
using Infragistics.Win.UltraWinTree;
using Layton.AuditWizard.DataAccess;
using Layton.Cab.Interface;
using Microsoft.Practices.ObjectBuilder;
using System.Windows.Forms;

namespace Layton.AuditWizard.Applications
{
    public class ApplicationsExplorerViewPresenter
    {
        private ApplicationsExplorerView explorerView;
        private ApplicationPublisherList _listPublishers = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ApplicationPublisherList PublisherList
        {
            get { return _listPublishers; }
        }

        [InjectionConstructor]
        public ApplicationsExplorerViewPresenter()
        {
        }

        public ILaytonView View
        {
            set { explorerView = (ApplicationsExplorerView)value; }
        }

        public void Initialize()
        {
            // JML_LINDE
            InitializeApplicationsView();
        }


        /// <summary>
        /// Initialize the Applications Tree View
        /// </summary>
        private void InitializeApplicationsView()
        {
            explorerView.Clear();

            // First fill-in in the 'All Publishers' branch
            InitializeAllPublishers();

            // fill-in in the 'All Operating Systems' branch
            InitializeAllOperatingSystems();

            // Fill in the 'Actions' branch
            InitializeActions();

            // Fill in the 'Alerts' branch
            InitializeAlerts();
        }


        /// <summary>
        /// Initialize the 'All Publishers' node of the applications tree
        /// </summary>
        private void InitializeAllPublishers()
        {
            Infragistics.Win.Appearance ignoredAppearance = new Infragistics.Win.Appearance();
            ignoredAppearance.ForeColor = System.Drawing.Color.Gray;

            // Add the publishers to the tree 
            try
            {
                ApplicationsWorkItemController wiController = explorerView.WorkItem.Controller as ApplicationsWorkItemController;
                bool showIncluded = wiController.ShowIncludedApplications;
                bool showIgnored = wiController.ShowIgnoredApplications;
  
                ApplicationsDAO lApplicationsDAO = new ApplicationsDAO();

                DataTable dt = lApplicationsDAO.GetAllPublisherNamesAsDataTable(wiController.PublisherFilter, showIncluded, showIgnored);
                UltraTreeNode[] publisherNodes = new UltraTreeNode[dt.Rows.Count];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string lPublisher = dt.Rows[i][0].ToString();

                    UltraTreeNode publisherNode = new UltraTreeNode(lPublisher, lPublisher);
                    publisherNode.Tag = "PUBLISHER";
                    publisherNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnExpand;
                    publisherNodes[i] = publisherNode;
                }

                explorerView.AllPublishersNode.Nodes.AddRange(publisherNodes);
            }

            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Initialize the 'All Operating Systems' node of the applications tree
        /// </summary>
        private void InitializeAllOperatingSystems()
        {
            // Add the entries to the tree
            try
            {
                DataTable dt = new ApplicationsDAO().GetOperatingSystems();
                UltraTreeNode osNode;
                UltraTreeNode[] nodes = new UltraTreeNode[dt.Rows.Count];
                InstalledOS theOS;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    theOS = new InstalledOS(dt.Rows[i]);

                    osNode = new UltraTreeNode(theOS.Name + "|" + theOS.Version, theOS.Name);
                    osNode.Tag = theOS;
                    osNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnExpand;
                    nodes[i] = osNode;
                }

                // Add the OS node (and it's sub-nodes to the tree
                explorerView.AllOperatingSystemsNode.Nodes.AddRange(nodes);
            }

            catch (Exception ex)
            {
                //MessageBox.Show("An exception occurred while populating the list of Operating Systems [InitializeAllOperatingSystems], the exception text was " + ex.Message);
                logger.Error(ex.Message);
            }
        }

        public void ExpandApplications(UltraTreeNode node)
        {
            Infragistics.Win.Appearance ignoredAppearance = new Infragistics.Win.Appearance();
            ignoredAppearance.ForeColor = System.Drawing.Color.Gray;

            ApplicationsWorkItemController wiController = explorerView.WorkItem.Controller as ApplicationsWorkItemController;
            bool showIncluded = wiController.ShowIncludedApplications;
            bool showIgnored = wiController.ShowIgnoredApplications; 

            InstalledApplication theApplication;
            string publisher;
            string key;


            publisher = node.FullPath.Substring(20);
            UltraTreeNode applicationNode;
            DataTable dt = new ApplicationsDAO().GetApplicationsByPublisher(publisher, showIncluded, showIgnored);
            UltraTreeNode[] applicationNodes = new UltraTreeNode[dt.Rows.Count];

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                theApplication = new InstalledApplication(dt.Rows[j]);
                key = publisher + "|" + theApplication.Name + "|" + theApplication.ApplicationID;

                if (theApplication.Version == String.Empty || theApplication.Name.EndsWith(theApplication.Version))
                    applicationNode = new UltraTreeNode(key, theApplication.Name);
                else
                    applicationNode = new UltraTreeNode(key, theApplication.Name + " (" + theApplication.Version + ")");

                applicationNode.Tag = theApplication;
                applicationNode.Override.ShowExpansionIndicator = ShowExpansionIndicator.CheckOnExpand;

                if (theApplication.IsIgnored) applicationNode.Override.NodeAppearance = ignoredAppearance;

                applicationNodes[j] = applicationNode;
            }

            node.Nodes.AddRange(applicationNodes);
        }

        /// <summary>
        /// Initialize the 'Actions' node of the applications tree
        /// </summary>
        private void InitializeActions()
        {
            UltraTreeNode actionsNode = new UltraTreeNode("Actions");
            //actionsNode.LeftImages.Add(Properties.Resources.action_16);
        }

        /// <summary>
        /// Initialize the 'Alerts' node of the applications tree
        /// </summary>
        private void InitializeAlerts()
        {
            UltraTreeNode node = new UltraTreeNode("Alerts");
            node.LeftImages.Add(Properties.Resources.Alert_16);
        }

        /// <summary>
        /// Find and return the publisher object
        /// </summary>
        /// <param name="publisherName"></param>
        /// <returns></returns>
        public ApplicationPublisher FindPublisher(string publisherName)
        {
            foreach (ApplicationPublisher thePublisher in _listPublishers)
            {
                if (publisherName == thePublisher.Name)
                    return thePublisher;
            }
            return null;
        }

    }
}
