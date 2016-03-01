using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
    public partial class FormAliasApplications : Form
    {
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

        public FormAliasApplications()
        {
            InitializeComponent();
            RefreshGrid();
        }

        /// <summary>
        /// Called as we load the form - we need to ensure that both selection controls are enabled.
        /// We can multiply select from the left hand list but not from the right hand one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormAliasApplications_Load(object sender, EventArgs e)
        {
            // Allow multiple selection in the left hand list as this will be the list from which the user
            // will choose those applications which are to be aliased.
            selectToAlias.SelectionType = SelectApplicationsControl.eSelectionType.multipleapplications;

            // Set the selection type to be 'Single Application'
            selectTarget.SelectionType = SelectApplicationsControl.eSelectionType.singleapplication;

            // Populate the selection control
            //PopulatePublisherLists();
        }

        private void PopulatePublisherLists()
        {
            //selectToAlias.Populate(_publisherFilter, _showIncluded, _showIgnored, true);
            //selectTarget.Populate(_publisherFilter, _showIncluded, _showIgnored, true);

            selectToAlias.PopulatePublishers(_publisherFilter, _showIncluded, _showIgnored, true);
            selectTarget.PopulatePublishers(_publisherFilter, _showIncluded, _showIgnored, true);
        }


        /// <summary>
        /// The user has clicked on the 'Alias' button - now alias the selected applications in the left hand
        /// view to that selecte in the right - first ensure that we have at least one selection in both lists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnAlias_Click(object sender, EventArgs e)
        {
            AliasApplications();
        }

        private void AliasApplications()
        {
            ApplicationsDAO lwDataAccess = new ApplicationsDAO();
            ApplicationPublisherList listSelectedPublishers;
            InstalledApplicationList listSelectedApplications;
            selectToAlias.GetSelectedItems(out listSelectedPublishers, out listSelectedApplications);

            // Anything selected?
            // Note that the Publisher list will always be empty as we have requested multiple applications only
            if (listSelectedApplications.Count == 0)
            {
                MessageBox.Show("Please select one or more applications which are to be aliased", "Select Alias Application");
                DialogResult = DialogResult.None;
                return;
            }

            // OK - we must also have the 'target' application selected
            InstalledApplication targetApplication = selectTarget.GetSelectedApplication();
            if (targetApplication == null)
            {
                MessageBox.Show("Please select the target application to which these applications should be aliased", "Select Target Application");
                DialogResult = DialogResult.None;
                return;
            }

            // One last check to make sure that we don't have a single application selected in both lists and it is
            // the same as someone is bound to try it to catch us out
            if ((listSelectedApplications.Count == 1)
            && (targetApplication.ApplicationID == listSelectedApplications[0].ApplicationID))
            {
                MessageBox.Show("You cannot alias an application to itself, please choose different applications", "Circular Alias");
                DialogResult = DialogResult.None;
                return;
            }

            // ...and we cannot alias an application which is itself already aliased by other applications
            if ((listSelectedApplications.Count == 1)
            && (lwDataAccess.GetAliasCount(listSelectedApplications[0].ApplicationID) != 0))
            {
                MessageBox.Show("You cannot alias an application which is itself already the target of an alias, please choose different applications", "Circular Alias");
                DialogResult = DialogResult.None;
                return;
            }

            List<string> lAliasedApplications = new List<string>();

            // OK All validation complete - now we have to actually act on the selected applications
            foreach (InstalledApplication application in listSelectedApplications)
            {
                if (application.Version == "" || application.Name.EndsWith(application.Version))
                    lAliasedApplications.Add(application.Name);
                else
                    lAliasedApplications.Add(application.Name + " (" + application.Version + ")");

                if (application.ApplicationID != targetApplication.ApplicationID)
                    lwDataAccess.ApplicationSetAlias(application.ApplicationID, targetApplication.ApplicationID);
            }

            string targetApplicationDisplay = "";

            if (targetApplication.Version == "" || targetApplication.Name.EndsWith(targetApplication.Version))
                targetApplicationDisplay = targetApplication.Name;
            else
                targetApplicationDisplay = targetApplication.Name + " (" + targetApplication.Version + ")";

            MessageBox.Show(
                        String.Format("The following application(s) have been successfully aliased to '{0}' "
                        + Environment.NewLine + Environment.NewLine + string.Join("\n", lAliasedApplications.ToArray()), targetApplicationDisplay),
                        "Alias Applications",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

            PopulatePublisherLists();
            RefreshGrid();
            tabControl1.SelectedTab = tabPage2;
        }

        private void RefreshGrid()
        {
            DataTable aliasTable = new ApplicationsDAO().GetAliasedApplications();

            ultraGrid1.DataSource = aliasTable;
            ultraGrid1.DataBind();
            ultraGrid1.DisplayLayout.Bands[0].SortedColumns.Add("Aliased To Application", false);
        }

        /// <summary>
        /// Delete the selected Application Aliases
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnDelete_Click(object sender, EventArgs e)
        {
            UnaliasApplications();
        }

        private void UnaliasApplications()
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

            ApplicationsDAO lApplicationsDAO = new ApplicationsDAO();
            string[] lApplicationDetails;
            string lSourcePublisher;
            string lSourceName;
            string lSourceVersion;
            int lApplicationId;

            foreach (UltraGridRow row in ultraGrid1.Selected.Rows)
            {
                lApplicationDetails = row.Cells[0].Text.Split('|');

                lSourcePublisher = lApplicationDetails[0].Trim();
                lSourceName = lApplicationDetails[1].Trim();
                lSourceVersion = lApplicationDetails[2].Trim();

                lApplicationId = lApplicationsDAO.SelectIdByPublisherNameVersion(lSourcePublisher, lSourceName, lSourceVersion);
                lApplicationsDAO.ApplicationSetAlias(lApplicationId, 0);
            }

            RefreshGrid();
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
            UnaliasApplications();
        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            PopulatePublisherLists();
        }
    }
}
