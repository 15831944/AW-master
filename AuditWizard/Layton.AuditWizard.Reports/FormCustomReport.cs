using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Infragistics.Win.UltraWinListView;
using Infragistics.Win.UltraWinTree;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Reports
{
    public partial class FormCustomReport : Form
    {
        private List<string> _selectedFields;
        private int _customReportId;
        private ReportsWorkItemController _wiController;
        private string _displayResultsAsAssetRegister;
        private bool _editingReport;
        private bool _reportSaved;

        public FormCustomReport(ReportsWorkItemController wiController)
        {
            InitializeComponent();
            _selectedFields = new List<string>();
            _wiController = wiController;
            _editingReport = false;
            _reportSaved = false;

            lvSelectedItems.ViewSettingsDetails.ImageSize = System.Drawing.Size.Empty;
        }

        public FormCustomReport(ReportsWorkItemController wiController, int aCustomReportId)
        {
            InitializeComponent();
            _selectedFields = new List<string>();
            _customReportId = aCustomReportId;
            _wiController = wiController;
            _editingReport = true;
            _reportSaved = true;

            this.Text = "Edit existing custom report";
            selectAuditedDataFieldsControl.Focus();

            lvSelectedItems.ViewSettingsDetails.ImageSize = System.Drawing.Size.Empty;
        }

        private void InitializeFormEdit()
        {
            lvSelectedItems.Items.Clear();
            _displayResultsAsAssetRegister = "";

            DataTable reportsDataTable = new ReportsDAO().GetReportById(_customReportId);
            tbCustomReportName.Text = reportsDataTable.Rows[0][1].ToString(); ;
            string lReportData = reportsDataTable.Rows[0][2].ToString();

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream(Convert.FromBase64String(lReportData));

            List<string> lReportDataList = (List<string>)bf.Deserialize(mem);

            // remove the locations item, just need the asset list
            foreach (string reportItem in lReportDataList)
            {
                if (reportItem.StartsWith("ASSET_REGISTER:"))
                {
                    _displayResultsAsAssetRegister = reportItem;
                }
            }

            lReportDataList.Remove(_displayResultsAsAssetRegister);

            if (_displayResultsAsAssetRegister != "")
                cbAssetRegister.Checked = Convert.ToBoolean(_displayResultsAsAssetRegister.Substring(15));

            selectAuditedDataFieldsControl.SelectedItems = lReportDataList;
            selectAuditedDataFieldsControl.RestoreSelections();
        }

        /// <summary>
        /// Called when items are selected or de-selected in the tree view to ensure that the list matches
        /// the actual selection state at this point.  We rebuild the list from scratch owing to the complexities
        /// of trying to keep tab of partially selected items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="node"></param>
        void selectAuditedDataFieldsControl_CheckChanged(object sender, UltraTreeNode node)
        {
            _reportSaved = false;

            // Begin update of the list view of selected items
            lvSelectedItems.BeginUpdate();

            // Get the root for this item - note that we do not step back to the root of the entire tree
            UltraTreeNode ourRootNode = node;
            while ((ourRootNode.Parent != null) && (ourRootNode.Parent.Parent != null))
            {
                ourRootNode = ourRootNode.Parent;
            }

            // Get the type of the node which we have played around with
            int type = (int)ourRootNode.Tag;

            // Remove any items of the same type as this from the listbox as they may change
            for (int index = 0; index < lvSelectedItems.Items.Count; )
            {
                UltraListViewItem lvi = lvSelectedItems.Items[index];
                if ((int)lvi.Tag == type)
                    lvSelectedItems.Items.Remove(lvi);
                else
                    index++;
            }

            // OK now we need to rebuild the list by traversing down from the root node
            AddSelectedItems(ourRootNode);

            // Hopefully done updating the list view
            lvSelectedItems.EndUpdate();

            bnSaveComplianceReport.Enabled = true;
        }

        protected void AddSelectedItems(UltraTreeNode parentNode)
        {
            switch (parentNode.CheckedState)
            {
                case CheckState.Checked:
                    {
                        if (parentNode.Key.StartsWith("Applications|"))
                        {
                            if (parentNode.Key.Split('|').Length == 4)
                                parentNode.Key = parentNode.Key.Substring(0, parentNode.Key.LastIndexOf("|"));
                        }

                        UltraListViewItem lvi = new UltraListViewItem(parentNode.Key, null);
                        //lvi.Appearance.Image = parentNode.LeftImages[0];
                        lvi.Appearance.Image = GetIcon(parentNode);
                        lvi.Tag = parentNode.Tag;
                        lvSelectedItems.Items.Add(lvi);
                    }
                    break;

                case CheckState.Indeterminate:
                    foreach (UltraTreeNode childNode in parentNode.Nodes)
                    {
                        AddSelectedItems(childNode);
                    }
                    break;
            }

            return;
        }

        private Bitmap GetIcon(UltraTreeNode node)
        {
            Bitmap bitmap = null;

            if (node.LeftImages.Count > 0)
                return (Bitmap)node.LeftImages[0];
            
            return node.Parent != null ? GetIcon(node.Parent) : bitmap;
        }

        #region Event Handlers

        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            if (tbCustomReportName.Text.Length == 0)
            {
                MessageBox.Show(
                        "Please enter a name for the custom report.",
                        "Save Custom Report",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                return;
            }

            SaveFields();

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            bf.Serialize(mem, _selectedFields);

            string lReportData = Convert.ToBase64String(mem.ToArray());

            if (_editingReport)
            {
                new ReportsDAO().Update(_customReportId, tbCustomReportName.Text, lReportData);
            }
            else
            {
                DataTable lExistingReports = new ReportsDAO().GetReportsByTypeAndName(ReportsDAO.ReportType.CustomReport, tbCustomReportName.Text);

                if (lExistingReports.Rows.Count > 0)
                {
                    if (MessageBox.Show(
                        "A custom report already exists with this name." + Environment.NewLine + Environment.NewLine +
                        "Do you wish to overwrite?",
                        "Save Custom Report",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        new ReportsDAO().Update(Convert.ToInt32(lExistingReports.Rows[0][0]), tbCustomReportName.Text, lReportData);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    new ReportsDAO().Insert(tbCustomReportName.Text, ReportsDAO.ReportType.CustomReport, lReportData);
                }
            }

            bnSaveComplianceReport.Enabled = false;
            _reportSaved = true;
            DesktopAlert.ShowDesktopAlert(String.Format("The custom report '{0}' has been saved.", tbCustomReportName.Text));
        }

        private void SaveFields()
        {
            _selectedFields.Clear();

            foreach (UltraListViewItem item in lvSelectedItems.Items)
            {
                _selectedFields.Add(item.Value.ToString());
            }

            _selectedFields.Add("ASSET_REGISTER:" + cbAssetRegister.Checked.ToString());
        }

        private void bnRunCustomReport_Click(object sender, EventArgs e)
        {
            if (!_reportSaved)
            {
                if (MessageBox.Show(
                    "The report has not yet been saved." + Environment.NewLine + Environment.NewLine +
                    "Do you wish to only run this report once without saving?",
                    "Run Report",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No)
                {
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            SaveFields();
            _wiController.RunCustomReport(tbCustomReportName.Text, _selectedFields);
            Close();
        }

        private void FormCustomReport_Load(object sender, EventArgs e)
        {
            if (_customReportId != 0)
            {
                InitializeFormEdit();
            }
            else
            {
                List<string> lReportDataList = new List<string>();
                lReportDataList.Add("Asset Details|Asset Name");
                selectAuditedDataFieldsControl.SelectedItems = lReportDataList;
                selectAuditedDataFieldsControl.RestoreSelections();
            }

            _reportSaved = _editingReport;
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        private void tbCustomReportName_TextChanged(object sender, EventArgs e)
        {
            _reportSaved = false;
            bnSaveComplianceReport.Enabled = tbCustomReportName.Text.Length > 0;
        }
    }
}
