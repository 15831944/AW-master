using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
    public partial class FormAliasApplication : Form
    {
        protected string _publisherFilter = "";
        protected bool _showIncluded = true;
        protected bool _showIgnored = false;
        private List<int> _selectedApplicationIds;
        private ApplicationsDAO lApplicationsDAO = new ApplicationsDAO();

        public FormAliasApplication(List<int> aSelectedApplicationIds)
        {
            InitializeComponent();
            _selectedApplicationIds = aSelectedApplicationIds;
        }

        private void FormAliasApplication_Load(object sender, EventArgs e)
        {
            selectTarget.SelectionType = SelectApplicationsControl.eSelectionType.singleapplication;
            selectTarget.PopulatePublishers(_publisherFilter, _showIncluded, _showIgnored, true);
            PopulateListBox();
        }

        private void PopulateListBox()
        {
            listBoxApps.Items.Clear();
            string lApplicationName = "";
            string lApplicationVersion = "";
            DataTable lAppDetailsDataTable = new DataTable();

            foreach (int lApplicationId in _selectedApplicationIds)
            {
                lAppDetailsDataTable = lApplicationsDAO.GetApplicationNameAndVersionById(lApplicationId);
                lApplicationName = lAppDetailsDataTable.Rows[0][0].ToString();
                lApplicationVersion = lAppDetailsDataTable.Rows[0][1].ToString();

                if (lApplicationVersion == "" || lApplicationName.EndsWith(lApplicationVersion))
                    listBoxApps.Items.Add(lApplicationName);
                else
                    listBoxApps.Items.Add(lApplicationName + " (" + lApplicationVersion + ")");
            }
        }

        private void AliasApplications(int aApplicationId, InstalledApplication _targetApplication)
        {
            if (aApplicationId != _targetApplication.ApplicationID)
                lApplicationsDAO.ApplicationSetAlias(aApplicationId, _targetApplication.ApplicationID);
        }

        private void bnAlias_Click(object sender, EventArgs e)
        {
            InstalledApplication _targetApplication = selectTarget.GetSelectedApplication();

            foreach (int lApplicationId in _selectedApplicationIds)
            {
                if (_targetApplication == null)
                {
                    MessageBox.Show("Please select the target application to which these applications should be aliased", "Select Target Application");
                    DialogResult = DialogResult.None;
                    return;
                }

                // One last check to make sure that we don't have a single application selected in both lists and it is
                // the same as someone is bound to try it to catch us out
                if (_targetApplication.ApplicationID == lApplicationId)
                {
                    MessageBox.Show("You cannot alias an application to itself, please choose different applications", "Circular Alias");
                    DialogResult = DialogResult.None;
                    return;
                }

                // ...and we cannot alias an application which is itself already aliased by other applications
                if (lApplicationsDAO.GetAliasCount(lApplicationId) != 0)
                {
                    MessageBox.Show(
                        String.Format("You cannot alias an application which is itself already the target of an alias, please choose different applications"),
                        "Circular Alias");
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            foreach (int lApplicationId in _selectedApplicationIds)
            {
                AliasApplications(lApplicationId, _targetApplication);
            }
        }

        private void bnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
