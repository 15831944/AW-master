using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Infragistics.Win.UltraWinListView;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Reports
{
    public partial class FormComplianceReport : Form
    {
        private List<string> _filterConditions;
        private string _selectedField;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ReportsWorkItemController _wiController;
        private AuditedItemsDAO lAuditedItemsDAO = new AuditedItemsDAO();
        private string _originalFileName;
        private bool _reportSaved;
        private int _complianceIndex = -1;

        public FormComplianceReport(ReportsWorkItemController wiController)
        {
            InitializeComponent();
            lblUnits.Text = "";
            _wiController = wiController;
            _filterConditions = new List<string>();
            _reportSaved = false;
            cbBooleanValue.SelectedIndex = 0;
        }

        public FormComplianceReport(ReportsWorkItemController wiController, int aReportId)
        {
            InitializeComponent();
            lblUnits.Text = "";
            _wiController = wiController;
            _reportSaved = false;
            cbBooleanValue.SelectedIndex = 0;

            DataTable reportsDataTable = new ReportsDAO().GetReportById(aReportId);
            _originalFileName = reportsDataTable.Rows[0][1].ToString();
            tbReportName.Text = _originalFileName;
            string lReportData = reportsDataTable.Rows[0][2].ToString();

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream(Convert.FromBase64String(lReportData));

            _filterConditions = (List<string>)bf.Deserialize(mem);

            selectedFields.Focus();
        }

        private void bnAddTrigger_Click(object sender, EventArgs e)
        {
            if (tbSelectedComplianceField.Text == "")
            {
                MessageBox.Show(
                    "Please select a compliance field from the list",
                    "Compliance Report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            if (_complianceIndex == -1)
                _filterConditions.Add(CreateFilterExpression());
            else
                _filterConditions.Insert(_complianceIndex, CreateFilterExpression());

            ComplianceField newComplianceField = new ComplianceField();

            string complianceValue = cbValuePicker.Text;
            int intValue;

            if (!Int32.TryParse(complianceValue, out intValue) && complianceValue != "")
                complianceValue = String.Format("'{0}'", complianceValue);

            newComplianceField.TriggerField = _selectedField;
            newComplianceField.Condition = (ComplianceField.eCondition)cbConditions.SelectedItem.DataValue;
            newComplianceField.Value = complianceValue;
            newComplianceField.BooleanValue = (cbBooleanValue.Enabled) ? cbBooleanValue.SelectedItem.ToString() : "";

            AddComplianceFieldToList(newComplianceField);

            _originalFileName = String.Empty;

            if (lvTriggers.Items.Count > 0)
            {
                bnRunComplianceReport.Enabled = true;
                cbBooleanValue.Enabled = true;
            }
            else
                bnRunComplianceReport.Enabled = false;

            bnSaveComplianceReport.Enabled = true;
        }

        private string CreateFilterExpression()
        {
            string category = _selectedField.Substring(0, _selectedField.LastIndexOf('|'));
            string name = _selectedField.Substring(_selectedField.LastIndexOf('|') + 1);
            string value = cbValuePicker.Text;
            string comparison = "=";

            switch (cbConditions.Text)
            {
                case "Equals":
                    comparison = "=";
                    break;
                case "Less Than":
                    comparison = "<";
                    break;
                case "Greater Than":
                    comparison = ">";
                    break;
                case "Greater or Equals":
                    comparison = ">=";
                    break;
                case "Less or Equals":
                    comparison = "<=";
                    break;
                case "Installed":
                    comparison = "==";
                    break;
                case "Not Installed":
                    comparison = "<>";
                    break;
            }

            int intValue;
            string filterExpression = (lvTriggers.Items.Count == 0) ? "1ST" : cbBooleanValue.SelectedItem.ToString();

            if (_complianceIndex == 0)
                filterExpression = "";

            if (Int32.TryParse(value, out intValue))
                filterExpression += String.Format("_category = '{0}' and _name = '{1}' and _value {2} {3}", category, name, comparison, intValue);

            else if (value == "")
                filterExpression += String.Format("_category = '{0}' and _name = '{1}' and _value {2} {3}", category, name, comparison, value);

            else
                filterExpression += String.Format("_category = '{0}' and _name = '{1}' and _value {2} '{3}'", category, name, comparison, value);

            return filterExpression;
        }

        /// <summary>
        /// Adds a newly defined trigger to the list
        /// </summary>
        /// <param name="complianceField"></param>
        private void AddComplianceFieldToList(ComplianceField complianceField)
        {
            UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[3];
            subItemArray[0] = new UltraListViewSubItem();
            subItemArray[0].Value = complianceField.ConditionAsText;
            subItemArray[1] = new UltraListViewSubItem();
            subItemArray[1].Value = complianceField.Value;
            subItemArray[2] = new UltraListViewSubItem();
            subItemArray[2].Value = (_complianceIndex == 0) ? "" : complianceField.BooleanValue;
            //
            UltraListViewItem item = new UltraListViewItem(complianceField.TriggerField, subItemArray);
            item.Tag = complianceField;

            if (_complianceIndex == -1)
                lvTriggers.Items.Add(item);
            else
                lvTriggers.Items.Insert(_complianceIndex, item);
        }

        private void bnRunComplianceReport_Click(object sender, EventArgs e)
        {
            if (_filterConditions.Count == 0)
            {
                MessageBox.Show(
                    "To run a report, you must add at least one compliance field.",
                    "AuditWizard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                return;
            }

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

            RunComplianceReport();
            Close();
        }

        public void EditComplianceReport()
        {
            ComplianceField trigger = new ComplianceField();

            for (int i = 0; i < _filterConditions.Count; i++)
            {
                string cf = _filterConditions[i];
                trigger.BooleanValue = (cf.Substring(0, cf.IndexOf("_category")) == "1ST") ? "" : cf.Substring(0, cf.IndexOf("_category"));
                cf = cf.Substring(cf.IndexOf("_category"));

                trigger.TriggerField = (cf.Substring(13, cf.IndexOf("' and _name") - 13) + "|" + cf.Substring(cf.IndexOf("_name = ") + 9, cf.IndexOf("and _value") - cf.IndexOf("_name = ") - 11));

                if (cf.Contains("_value =="))
                {
                    trigger.Condition = ComplianceField.eCondition.installed;
                    trigger.Value = String.Empty;
                }
                else if (cf.Contains("_value <>"))
                {
                    trigger.Condition = ComplianceField.eCondition.notinstalled;
                    trigger.Value = String.Empty;
                }
                else if (cf.Contains("_value >="))
                {
                    trigger.Condition = ComplianceField.eCondition.greaterequals;
                    trigger.Value = cf.Substring(cf.IndexOf("_value >=") + 10);
                }
                else if (cf.Contains("_value <="))
                {
                    trigger.Condition = ComplianceField.eCondition.lessequals;
                    trigger.Value = cf.Substring(cf.IndexOf("_value <=") + 10);
                }
                else if (cf.Contains("_value >"))
                {
                    trigger.Condition = ComplianceField.eCondition.greaterthan;
                    trigger.Value = cf.Substring(cf.IndexOf("_value >") + 9);
                }
                else if (cf.Contains("_value <"))
                {
                    trigger.Condition = ComplianceField.eCondition.lessthan;
                    trigger.Value = cf.Substring(cf.IndexOf("_value <") + 9);
                }
                else if (cf.Contains("_value ="))
                {
                    trigger.Condition = ComplianceField.eCondition.equals;
                    trigger.Value = cf.Substring(cf.IndexOf("_value =") + 9);
                }

                AddComplianceFieldToList(trigger);
            }

            tbReportName.Focus();
        }

        public void RunComplianceReport()
        {
            _wiController.RunComplianceReport(tbReportName.Text, _filterConditions);
        }

        private void lvTriggers_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            btnDeleteTrigger.Enabled = (lvTriggers.SelectedItems.Count != 0);
            btnEditComplianceField.Enabled = (lvTriggers.SelectedItems.Count != 0);
        }

        /// <summary>
        /// Called as the selected item in the tree control is changed as this affects the possible 
        /// conditions and values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="node"></param>
        void selectedFields_AfterSelect(object sender, Infragistics.Win.UltraWinTree.UltraTreeNode node)
        {
            if (node == null)
            {
                _selectedField = "";
                return;
            }

            string lNodeKey = node.Key;

            if (lvTriggers.Items.Count > 0)
            {
                // always make sure the first item begins with "1ST" - this might e the case after an edit compliance field operation
                if (_filterConditions.Count > 0)
                    _filterConditions[0] = "1ST" + _filterConditions[0].Substring(_filterConditions[0].IndexOf("_category"));

                lvTriggers.Items[0].SubItems[2].Value = "";
                lvTriggers.Refresh();
            }
            else
                cbBooleanValue.Enabled = false;

            _complianceIndex = -1;
            cbConditions.Items.Clear();
            tbSelectedComplianceField.Text = "";
            cbValuePicker.Items.Clear();

            lblUnits.Text = String.Empty;

            //if (node.Text == "Applications")
            //{
            //    lbConditions.Enabled = false;
            //    lbValue.Enabled = false;
            //    cbConditions.Enabled = false;
            //    cbValuePicker.Enabled = false;
            //    bnAddTrigger.Enabled = false;
            //}
            if (node.HasNodes)
            {
                lbConditions.Enabled = false;
                lbValue.Enabled = false;
                cbConditions.Enabled = false;
                cbValuePicker.Enabled = false;
                bnAddTrigger.Enabled = false;
                lbReportName.Enabled = false;
                tbReportName.Enabled = false;
                lbAddField.Enabled = false;
                lbComplianceField.Enabled = false;
                lbBooleanOperator.Enabled = false;
            }
            else
            {
                lbValue.Enabled = true;
                cbValuePicker.Enabled = true;
                lbConditions.Enabled = true;
                cbConditions.Enabled = true;
                bnAddTrigger.Enabled = true;
                lbReportName.Enabled = true;
                tbReportName.Enabled = true;
                lbAddField.Enabled = true;
                lbComplianceField.Enabled = true;
                lbBooleanOperator.Enabled = true;
            }

            //if (lNodeKey.StartsWith("Applications|"))
            //{
            //    if (lNodeKey.Split('|').Length > 2)
            //        lNodeKey = lNodeKey.Substring(0, lNodeKey.LastIndexOf("|"));
            //}

            if (!node.HasNodes)
                PopulateComplianceFields(lNodeKey);
        }

        private void PopulateComplianceFields(string itemKey)
        {
            _selectedField = itemKey;
            tbSelectedComplianceField.Text = _selectedField;

            List<string> itemParts = Utility.ListFromString(itemKey, '|', true);

            // Clear existing conditions
            cbConditions.Items.Clear();

            // 'Changed' is valid for all but Internet fields
            if (itemParts[0] == AWMiscStrings.InternetNode)
            {
                cbConditions.Items.Add(ComplianceField.eCondition.contains, "Contains");
                lbValue.Enabled = true;
                cbValuePicker.Enabled = true;
            }

            else if ((itemParts.Count == 3 && itemParts[0] == AWMiscStrings.ApplicationsNode) || (_selectedField.StartsWith(AWMiscStrings.SystemPatchesNode)))
            {
                cbConditions.Items.Add(ComplianceField.eCondition.installed, "Installed");
                cbConditions.Items.Add(ComplianceField.eCondition.notinstalled, "Not Installed");
                lbValue.Enabled = false;
                cbValuePicker.Enabled = false;
            }

            else
            {
                cbConditions.Items.Add(ComplianceField.eCondition.equals, "Equals");

                // Some specific fields should also allow other numeric operations
                string fieldName = itemParts[itemParts.Count - 1];
                if (
                    (fieldName == "Count") || (fieldName == "Speed") ||
                    (fieldName.StartsWith("Available")) || (fieldName.StartsWith("Total")) ||
                    (fieldName.StartsWith("Free")) || (fieldName.StartsWith("Pages Printed Since Reboot")) ||
                    (fieldName.Contains("Date of last Audit"))
                    )
                {
                    cbConditions.Items.Add(ComplianceField.eCondition.greaterthan, "Greater Than");
                    cbConditions.Items.Add(ComplianceField.eCondition.greaterequals, "Greater or Equals");
                    cbConditions.Items.Add(ComplianceField.eCondition.lessthan, "Less Than");
                    cbConditions.Items.Add(ComplianceField.eCondition.lessequals, "Less or Equals");

                }

                //if (fieldName.StartsWith("Total") || fieldName.StartsWith("Available") || fieldName.StartsWith("Free"))
                //{
                //    if (!fieldName.StartsWith("Total Pages"))
                //        lblUnits.Text = "MB";
                //}

                //else if (fieldName == "Speed")
                //    lblUnits.Text = "MHz";

                //else
                //    lblUnits.Text = String.Empty;
            }

            cbConditions.SelectedIndex = 0;
            cbConditions.EndUpdate();

            if (itemParts.Count > 1)
                PopulatePickerList(itemParts);
        }

        private void PopulatePickerList(List<string> itemParts)
        {
            string category;
            string name;
            string value;

            if (itemParts.Count == 3)
            {
                category = itemParts[0] + "|" + itemParts[1];
                name = itemParts[2];
            }
            else if (itemParts.Count == 4)
            {
                category = itemParts[0] + "|" + itemParts[1] + "|" + itemParts[2];
                name = itemParts[3];
            }
            else if (itemParts.Count == 5)
            {
                category = itemParts[0] + "|" + itemParts[1] + "|" + itemParts[2] + "|" + itemParts[3];
                name = itemParts[4];
            }
            else
            {
                category = itemParts[0];
                name = itemParts[1];
            }

            cbValuePicker.Items.Clear();
            cbValuePicker.Text = String.Empty;
            cbValuePicker.Refresh();

            DataTable pickerValuesDataTable = new DataTable();

            if (category.StartsWith("UserData|"))
            {
                pickerValuesDataTable = new UserDataDefinitionsDAO().GetUserDataPickerValues(name, category.Split('|')[1]);
            }
            else if (category.StartsWith("Asset Details"))
            {
                string columnName = String.Empty;

                switch (name)
                {
                    case "Asset Name":
                        columnName = "_name";
                        break;
                    case "Location":
                        columnName = "_locationid";
                        break;
                    case "Date of last Audit":
                        columnName = "_lastaudit";
                        break;
                    case "IP Address":
                        columnName = "_ipaddress";
                        break;
                    case "MAC Address":
                        columnName = "_macaddress";
                        break;
                    case "Make":
                        columnName = "_make";
                        break;
                    case "Model":
                        columnName = "_model";
                        break;
                    case "Serial Number":
                        columnName = "_serial_number";
                        break;
                    case "Category":
                        columnName = "_category";
                        break;
                    case "Type":
                        columnName = "_type";
                        break;
                    case "Asset Tag":
                        columnName = "_assettag";
                        break;
                }

                if (columnName == "_category")
                {
                    pickerValuesDataTable = new AssetTypesDAO().GetAssetCategoriesPickerValues();
                }
                else if (columnName == "_lastaudit")
                {
                    pickerValuesDataTable = new AssetDAO().GetAssetPickerValuesForLastAudit();
                }
                else if (columnName == "_locationid")
                {
                    pickerValuesDataTable = new LocationsDAO().GetLocationPickerValues();
                }
                else if (columnName == "_type")
                {
                    pickerValuesDataTable = new AssetTypesDAO().GetAssetTypesPickerValues();
                }
                else
                {
                    pickerValuesDataTable = new AssetDAO().GetAssetPickerValues(columnName);
                }
            }
            else if (category.StartsWith("Operating Systems"))
            {
                switch (name)
                {
                    case "Family":
                        pickerValuesDataTable = new ApplicationsDAO().GetOSPickerValues("_name");
                        break;
                    case "Version":
                        pickerValuesDataTable = new ApplicationsDAO().GetOSPickerValues("_version");
                        break;
                    case "CD Key":
                        pickerValuesDataTable = new ApplicationInstanceDAO().GetOSPickerValues("_cdkey");
                        break;
                    case "Serial Number":
                        pickerValuesDataTable = new ApplicationInstanceDAO().GetOSPickerValues("_productid");
                        break;
                }
            }
            else
            {
                pickerValuesDataTable = lAuditedItemsDAO.GetPickerValue(category, name);
                lblUnits.Text = lAuditedItemsDAO.GetDisplayUnitValue(category, name);
            }

            foreach (DataRow pickerValueRow in pickerValuesDataTable.Rows)
            {
                value = pickerValueRow.ItemArray[0].ToString();

                if (value != String.Empty)
                    cbValuePicker.Items.Add(value);
            }

            if (cbValuePicker.Items.Count > 0)
                cbValuePicker.SelectedIndex = 0;
        }

        private void bnSaveComplianceReport_Click(object sender, EventArgs e)
        {
            if (tbReportName.Text == "")
            {
                MessageBox.Show(
                    "Please enter a name for the compliance report.",
                    "Save Compliance Report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (_filterConditions.Count == 0)
            {
                MessageBox.Show(
                    "Please select one or more conditions to include in the report.",
                    "Save Compliance Report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            bf.Serialize(mem, _filterConditions);
            string lReportData = Convert.ToBase64String(mem.ToArray());

            DataTable lExistingReports = new ReportsDAO().GetReportsByTypeAndName(ReportsDAO.ReportType.ComplianceReport, tbReportName.Text);

            if (lExistingReports.Rows.Count > 0)
            {
                if (MessageBox.Show(
                    "A custom report already exists with this name." + Environment.NewLine + Environment.NewLine +
                    "Do you wish to overwrite?",
                    "Save Custom Report",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    new ReportsDAO().Update(Convert.ToInt32(lExistingReports.Rows[0][0]), tbReportName.Text, lReportData);
                }
                else
                {
                    return;
                }
            }
            else
            {
                new ReportsDAO().Insert(tbReportName.Text, ReportsDAO.ReportType.ComplianceReport, lReportData);
            }

            bnSaveComplianceReport.Enabled = false;
            DesktopAlert.ShowDesktopAlert(String.Format("The compliance report '{0}' has been saved.", tbReportName.Text));
            _reportSaved = true;
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tbReportName_TextChanged(object sender, EventArgs e)
        {
            bnSaveComplianceReport.Enabled = tbReportName.Text.Length > 0;
        }

        private void FormComplianceReport_KeyUp(object sender, KeyEventArgs e)
        {
            if (lvTriggers.Focused && e.KeyCode == Keys.Delete)
            {
                DeleteComplianceField();
            }
        }

        private void FormComplianceReport_Load(object sender, EventArgs e)
        {
            cbBooleanValue.Enabled = lvTriggers.Items.Count > 0;
            lvTriggers.ViewSettingsDetails.ImageSize = System.Drawing.Size.Empty;
        }

        private void DeleteComplianceField()
        {
            if (lvTriggers.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    "Please select a compliance field to delete.",
                    "Compliance Report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            List<UltraListViewItem> elems = new List<UltraListViewItem>();

            foreach (UltraListViewItem lvi in lvTriggers.SelectedItems)
            {
                //UltraListViewItem lvi = lvTriggers.SelectedItems[i];
                int index = lvi.Index;

                string category = lvi.Text.Substring(0, lvi.Text.LastIndexOf('|'));
                string name = lvi.Text.Substring(lvi.Text.LastIndexOf('|') + 1);
                string value = lvi.SubItems[1].Value.ToString();
                string comparison = "=";

                switch (lvi.SubItems[0].Value.ToString())
                {
                    case "Equals":
                        comparison = "=";
                        break;
                    case "Less Than":
                        comparison = "<";
                        break;
                    case "Greater Than":
                        comparison = ">";
                        break;
                    case "Greater or Equals":
                        comparison = ">=";
                        break;
                    case "Less or Equals":
                        comparison = "<=";
                        break;
                    case "Installed":
                        comparison = "==";
                        break;
                    case "Not Installed":
                        comparison = "<>";
                        break;
                }

                //
                //int intValue;
                //string filterExpression = "";

                //if (Int32.TryParse(value, out intValue))
                //    filterExpression += String.Format("_category = '{0}' and _name = '{1}' and _value {2} {3}", category, name, comparison, intValue);
                //else
                //    filterExpression += String.Format("_category = '{0}' and _name = '{1}' and _value {2} '{3}'", category, name, comparison, value);

                //

                string filterExpression = String.Format("_category = '{0}' and _name = '{1}' and _value {2} {3}", category, name, comparison, value);

                //if (filterExpression.StartsWith("_category = 'Applications|"))
                //    filterExpression = filterExpression.Replace("_value == ", "_value == ''");

                // add the booleanValue
                filterExpression = (lvi.SubItems[2].Value.ToString() == "") ? "1ST" + filterExpression : lvi.SubItems[2].Value.ToString() + filterExpression;

                _filterConditions.Remove(filterExpression);

                // Remove the item from the list view
                elems.Add(lvi);
            }

            foreach (UltraListViewItem item in elems)
            {
                lvTriggers.Items.Remove(item);
            }

            // Select the next definition in the list if any
            //if (index >= lvTriggers.Items.Count)
            //    index--;
            //if (index >= 0)
            //    lvTriggers.SelectedItems.Add(lvTriggers.Items[index]);

            if (lvTriggers.Items.Count > 0)
            {
                if (_complianceIndex == -1)
                {
                    if (_filterConditions.Count > 0)
                        _filterConditions[0] = "1ST" + _filterConditions[0].Substring(_filterConditions[0].IndexOf("_category"));

                    lvTriggers.Items[0].SubItems[2].Value = "";
                    bnRunComplianceReport.Enabled = true;
                    cbBooleanValue.Enabled = true;
                }
            }
            else
            {
                bnRunComplianceReport.Enabled = false;
                cbBooleanValue.Enabled = false;
            }
        }

        private void btnDeleteTrigger_Click(object sender, EventArgs e)
        {
            DeleteComplianceField();
            bnSaveComplianceReport.Enabled = true;
        }

        private void btnEditComplianceField_Click(object sender, EventArgs e)
        {
            if (lvTriggers.SelectedItems.Count != 1)
            {
                MessageBox.Show(
                    "Please select one compliance field to edit.",
                    "Compliance Report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            UltraListViewItem lvi = lvTriggers.SelectedItems[0];
            _complianceIndex = lvi.Index;

            if (_complianceIndex == 0)
                cbBooleanValue.Enabled = false;

            PopulateComplianceFields(lvi.Text);

            cbConditions.Text = lvi.SubItems[0].Value.ToString();
            cbValuePicker.Text = lvi.SubItems[1].Value.ToString().Replace("'", "");
            cbBooleanValue.Text = lvi.SubItems[2].Value.ToString();

            DeleteComplianceField();
            bnSaveComplianceReport.Enabled = true;
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (lvTriggers.SelectedItems.Count != 1)
            {
                MessageBox.Show(
                    "Please select one compliance field to move.",
                    "Compliance Report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            UltraListViewItem lvi = lvTriggers.SelectedItems[0];
            int index = lvi.Index;

            if (index == 0)
                return;

            lvTriggers.Items.RemoveAt(index);
            lvTriggers.Items.Insert(index - 1, lvi);

            // special case if we've moved second record to top - update the boolean values
            if (index == 1)
            {
                UltraListViewItem secondLvi = lvTriggers.Items[0];
                secondLvi.SubItems[2].Value = "";

                secondLvi = lvTriggers.Items[1];
                secondLvi.SubItems[2].Value = "And"; 

                lvTriggers.Refresh();
            }

            // also change its position in the _filterConditions list
            string selectedFilterContition = _filterConditions[index];
            _filterConditions.Remove(selectedFilterContition);

            // if the second record is being moved to the top, ensure it is prefaced with '1ST' and current first is prefaced by 'And'
            if (index == 1)
            {
                selectedFilterContition = "1ST" + selectedFilterContition.Substring(selectedFilterContition.IndexOf("_category"));
                _filterConditions[0] = "And" + _filterConditions[0].Substring(_filterConditions[0].IndexOf("_category"));
            }

            _filterConditions.Insert(index - 1, selectedFilterContition);

            bnSaveComplianceReport.Enabled = true;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            bnSaveComplianceReport.Enabled = true;
        }
    }
}
