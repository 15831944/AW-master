using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTree;
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
    public partial class FormAliasPublishers : Form
    {
        public enum eSelectionType { all, singleapplication, multipleapplications };
        private eSelectionType _selectionType;
        public const string AllPublishers = "Software Publishers";

        #region Data

        protected string _publisherFilter = "";
        protected bool _showIncluded = true;
        protected bool _showIgnored = false;
        protected bool _showContextMenu = false;

        #endregion Data

        #region Properties

        public string PublisherFilter
        {
            set { _publisherFilter = value; }
        }

        public bool ShowIncluded
        {
            set { _showIncluded = value; }
        }

        public bool ShowIgnored
        {
            set { _showIgnored = value; }
        }

        #endregion Data

        public FormAliasPublishers()
        {
            InitializeComponent();
            RefreshGrid();
            //PopulatePublisherLists();
        }

        private void PopulatePublisherLists()
        {
            PopulateSourcePublishersTree("", _showIncluded, _showIgnored);
            PopulateTargetPublishersTree("", _showIncluded, _showIgnored);
        }

        private void FormAliasPublishers_Load(object sender, EventArgs e)
        {
            PopulatePublisherLists();
        }

        private void AliasButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (CheckValidAlias())
                {
                    List<string> lSourcePublisher = new List<string>();

                    UltraTreeNode rootNode = sourcePublishersTree.Nodes[0];
                    foreach (UltraTreeNode publisherNode in rootNode.Nodes)
                    {
                        if (publisherNode.CheckedState == CheckState.Checked)
                            lSourcePublisher.Add(publisherNode.Text);
                    }

                    string lTargetPublisher = targetPublishersTree.SelectedNodes[0].Text;

                    AliasPublisher(lSourcePublisher, lTargetPublisher);
                    RefreshGrid();

                    // clear all selections
                    foreach (UltraTreeNode publisherNode in rootNode.Nodes)
                    {
                        publisherNode.CheckedState = CheckState.Unchecked;
                    }

                    targetPublishersTree.SelectedNodes.Clear();

                    MessageBox.Show(
                        String.Format("The following publisher(s) have been successfully aliased to '{0}' "
                        + Environment.NewLine + Environment.NewLine + string.Join("\n", lSourcePublisher.ToArray()), lTargetPublisher),
                        "Alias Publishers",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    PopulatePublisherLists();

                    tabControl1.SelectedTab = tabPage2;
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void UnaliasButton_Click(object sender, EventArgs e)
        {
            int selectedRows = ultraGrid1.Selected.Rows.Count;
            string messageText;

            if (selectedRows == 0)
            {
                MessageBox.Show(
                        "Please select a row from the aliased applications table.",
                        "Alias Applications",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                return;
            }

            messageText = (selectedRows == 1) ? "Are you sure you want to delete this alias?" : "Are you sure you want to delete these aliases?";

            if (MessageBox.Show(messageText, "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            string lSourcePublisher;

            foreach (UltraGridRow row in ultraGrid1.Selected.Rows)
            {
                lSourcePublisher = row.Cells["Original Publisher"].Text;
                UnAliasPublisher(lSourcePublisher);
            }

            PopulatePublisherLists();
            RefreshGrid();
        }

        private void AliasPublisher(List<string> aSourcePublisher, string aTargetPublisher)
        {
            ApplicationsDAO lApplicationsDAO = new ApplicationsDAO();
            ApplicationInstanceDAO lApplicationInstanceDAO = new ApplicationInstanceDAO();

            // insert a record into the PUBLISHER_ALIAS table
            foreach (string lSourcePublisher in aSourcePublisher)
            {
                int lPublisherAliasID = new PublisherAliasDAO().Insert(lSourcePublisher, aTargetPublisher);

                // confirm record entered ok?
                if (lPublisherAliasID != -1)
                {
                    DataTable sourcePublisherApplicationsDataTable = lApplicationsDAO.SelectApplicationByPublisherName(lSourcePublisher);

                    foreach (DataRow applicationRow in sourcePublisherApplicationsDataTable.Rows)
                    {
                        // check if the proposed new application already exists
                        // it is possible (but unlikely) that an application name and version combo already exist
                        int lSourceApplicationId = (int)applicationRow.ItemArray[0];
                        string lApplicationName = applicationRow.ItemArray[2].ToString();
                        string lApplicationVersion = applicationRow.ItemArray[3].ToString();

                        int lExisitingApplicationId = lApplicationsDAO.SelectIdByPublisherNameVersion(aTargetPublisher, lApplicationName, lApplicationVersion);

                        if (lExisitingApplicationId > 0)
                        {
                            // found a match
                            // need to get the original applicationid and update all of the application_instances for this application                            

                            // one final issue is whether the application is aliased
                            // if it is, we need to update the application_instances so that the base applicationid is the original app id
                            // and the _applicationid is the aliased_toid
                            int lBaseApplicationId = 0;
                            int lAliasedToId = lApplicationsDAO.SelectAliasedToIdByApplicationId(lExisitingApplicationId);

                            if (lAliasedToId != 0)
                            {
                                lBaseApplicationId = lExisitingApplicationId;
                                lExisitingApplicationId = lAliasedToId;
                            }

                            lApplicationInstanceDAO.UpdateApplicationInstanceByApplicationId(lExisitingApplicationId, lBaseApplicationId, lSourceApplicationId);

                            // final step will be to delete the application we are aliasing. This will mean that if the user tries to revert
                            // this alias they won't be able to do so
                            lApplicationsDAO.DeleteByApplicationId(lSourceApplicationId);
                        }
                        else
                        {
                            // publiser, name, version combo doesn;t already exist so we can just update the publisher name
                            // enter a record into the PUBLISHER_ALIAS_APP table, this will allow us to revert the alias later if needed
                            new PublisherAliasAppDAO().Insert(lSourceApplicationId, lPublisherAliasID);

                            // finally updated the application to reflect the new publisher
                            lApplicationsDAO.UpdateAliasedPublishers(aTargetPublisher, lSourceApplicationId);
                        }
                    }
                }
            }
        }

        private void UnAliasPublisher(string aOriginalPublisher)
        {
            // foreach application that we aliased the publisher, revert them
            new ApplicationsDAO().RevertPublisherOfAliasedApplications(aOriginalPublisher);

            // remove all of the records from PUBLISHER_ALIAS_APP for this original publisher
            new PublisherAliasAppDAO().DeleteByOriginalPublisherName(aOriginalPublisher);

            // finally delete the record in PUBLISHER_ALIAS to remove the alias completely
            new PublisherAliasDAO().DeleteByOriginalPublisherName(aOriginalPublisher);
        }

        private void RefreshGrid()
        {
            DataTable aliasTable = new PublisherAliasDAO().GetAllPublisherAliases();

            ultraGrid1.DataSource = aliasTable;
            ultraGrid1.DataBind();
            ultraGrid1.DisplayLayout.Bands[0].SortedColumns.Add("Aliased To Publisher", false);
        }

        public void PopulateTargetPublishersTree(string publisherFilter, bool showIncluded, bool showIgnored)
        {
            // Handle the selection type as this determine if and where we can have check boxes
            _selectionType = eSelectionType.singleapplication;

            // Populate the tree
            using (new WaitCursor())
            {
                // begin updating the tree
                targetPublishersTree.BeginUpdate();
                targetPublishersTree.Nodes.Clear();

                // add the root node which is ALL Publishers
                UltraTreeNode rootNode = targetPublishersTree.Nodes.Add(AllPublishers, AllPublishers);
                rootNode.LeftImages.Add(Properties.Resources.application_view_16);

                // Populate the list of Publishers and applications first
                PopulatePublisherNodes(publisherFilter, showIncluded, showIgnored, rootNode, true);

                // Finished updating the tree			
                targetPublishersTree.EndUpdate();
            }
        }

        public void PopulateSourcePublishersTree(string publisherFilter, bool showIncluded, bool showIgnored)
        {
            // Handle the selection type as this determine if and where we can have check boxes
            _selectionType = eSelectionType.all;

            // Populate the tree
            using (new WaitCursor())
            {
                // begin updating the tree
                sourcePublishersTree.BeginUpdate();
                sourcePublishersTree.Nodes.Clear();

                // add the root node which is ALL Publishers
                UltraTreeNode rootNode = sourcePublishersTree.Nodes.Add(AllPublishers, AllPublishers);
                rootNode.LeftImages.Add(Properties.Resources.application_view_16);

                // Populate the list of Publishers and applications first
                PopulatePublisherNodes(publisherFilter, showIncluded, showIgnored, rootNode, false);

                // Finished updating the tree			
                sourcePublishersTree.EndUpdate();
            }
        }

        private void PopulatePublisherNodes(string publisherFilter, bool showIncluded, bool showIgnored, UltraTreeNode rootNode, bool showAliasedPublishers)
        {
            rootNode.Override.NodeStyle = NodeStyle.Standard;

            PublisherAliasDAO lPublisherAliasDao = new PublisherAliasDAO();
            List<string> existingTargetPublishers = lPublisherAliasDao.SelectAliasedToPublishers();

            // Add the publishers to the tree 
            try
            {
                UltraTreeNode publisherNode;

                //DataTable dt = new ApplicationsDAO().GetAllPublisherNamesAsDataTable(publisherFilter);
                DataTable dt = new ApplicationsDAO().GetAllPublisherNamesAsDataTable(publisherFilter, _showIncluded, _showIgnored);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string lPublisher = dt.Rows[i][0].ToString();

                    publisherNode = new UltraTreeNode(rootNode.Key + @"|" + lPublisher, lPublisher);
                    publisherNode.Override.NodeStyle = _selectionType == eSelectionType.all ? NodeStyle.CheckBox : NodeStyle.Standard;
                    publisherNode.Tag = rootNode.Tag;

                    if (!showAliasedPublishers)
                    {
                        if (!existingTargetPublishers.Contains(publisherNode.Text))
                            rootNode.Nodes.Add(publisherNode);
                    }
                    else
                        rootNode.Nodes.Add(publisherNode);
                }

                rootNode.Expanded = true;
            }

            catch (Exception)
            {
                // JML TODO log this error
            }

            // Expand the root node to show the publishers
            rootNode.Expanded = true;
        }

        private bool CheckValidAlias()
        {
            if (!CheckSourcePublisherSelected())
            {
                MessageBox.Show("Please select at least one publisher to be aliased.", "Alias Publisher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (!CheckTargetPublishersSelected())
            {
                MessageBox.Show("Please select a target publisher.", "Alias Publisher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (!CheckSourceTargetPublisherDifferent())
            {
                MessageBox.Show("You have attempted to alias a publisher to itself which is not allowed.", "Alias Publisher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            return true;
        }

        private List<string> CheckPublisherNotAlreadyAliased()
        {
            List<string> lAliasedPublishers = new List<string>();

            UltraTreeNode rootNode = sourcePublishersTree.Nodes[0];
            PublisherAliasDAO lPublisherAliasDAO = new PublisherAliasDAO();

            foreach (UltraTreeNode applicationNode in rootNode.Nodes)
            {
                if (applicationNode.CheckedState == CheckState.Checked)
                {
                    if (lPublisherAliasDAO.CheckPublisherExists(applicationNode.Text) > 0)
                        lAliasedPublishers.Add(applicationNode.Text);
                }
            }

            return lAliasedPublishers;
        }

        private bool CheckSourceTargetPublisherDifferent()
        {
            UltraTreeNode rootNode = sourcePublishersTree.Nodes[0];
            PublisherAliasDAO lPublisherAliasDAO = new PublisherAliasDAO();

            foreach (UltraTreeNode applicationNode in rootNode.Nodes)
            {
                if (applicationNode.CheckedState == CheckState.Checked)
                {
                    if (applicationNode.Text == targetPublishersTree.SelectedNodes[0].Text)
                        return false;
                }
            }

            return true;
        }

        private bool CheckTargetPublishersSelected()
        {
            if (targetPublishersTree.SelectedNodes.Count > 0)
                return true;
            else
                return false;
        }

        private bool CheckSourcePublisherSelected()
        {
            UltraTreeNode rootNode = sourcePublishersTree.Nodes[0];
            foreach (UltraTreeNode applicationNode in rootNode.Nodes)
            {
                if (applicationNode.CheckedState == CheckState.Checked)
                    return true;
            }

            return false;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (!_showContextMenu)
            {
                e.Cancel = true;
                return;
            }
        }

        private void ultraGrid1_MouseDown(object sender, MouseEventArgs e)
        {
            _showContextMenu = false;

            if (e.Button == MouseButtons.Right)
            {
                UltraGrid grid = (UltraGrid)sender;
                UIElement element = grid.DisplayLayout.UIElement.LastElementEntered;
                if (element == null)
                    return;

                UltraGridRow row = element.GetContext(typeof(UltraGridRow)) as UltraGridRow;
                if (row != null)
                {
                    ultraGrid1.Selected.Rows.Clear();
                    row.Activated = true;
                    row.Selected = true;
                    _showContextMenu = true;
                }
            }
        }

        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {
            UnAliasPublisher(ultraGrid1.Selected.Rows[0].Cells["Original Publisher"].Text);
            RefreshGrid();
        }
    }
}
