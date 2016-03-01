using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Infragistics.Win;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Administration
{
    public partial class FormUserDataField : AWForm
    {
        private readonly UserDataCategory _category;
        private readonly UserDataField _field;
        private readonly bool _editing;
        private readonly UserDataField.FieldType _existingType;

        // Possible user data categories
        private UserDataCategoryList _listCategories;

        public FormUserDataField(UserDataCategory category, UserDataField field, bool editing)
        {
            InitializeComponent();
            _category = category;
            _field = field;
            _editing = editing;
            _existingType = field.Type;

            cbInteractiveInclude.Visible = cbMandatory.Visible = (_category.Scope != UserDataCategory.SCOPE.Application);

            InitializeForm();
        }

        private void InitializeForm()
        {
            // Create the list of user data categories given the appropriate type from that supplied
            _listCategories = new UserDataCategoryList(_category.Scope);

            // Populate the 'Categories' combo with possible values
            _listCategories.Populate();
            foreach (UserDataCategory availableCategory in _listCategories)
            {
                ValueListItem item = cbCategories.Items.Add(availableCategory, availableCategory.Name);
                if (availableCategory.CategoryID == _category.CategoryID)
                    cbCategories.SelectedItem = item;
            }

            // Set the form title
            Text = (_field.FieldID == 0) ? "New User Data Field" : "User Data Field Properties";
            Text += " [" + _category.ScopeAsString + "]";

            // Populate the 'Type' combo with possible types
            Dictionary<UserDataField.FieldType, string> fieldTypes = _field.FieldTypes;
            foreach (KeyValuePair<UserDataField.FieldType, string> kvp in fieldTypes)
            {
                cbFieldType.Items.Add(kvp.Key, kvp.Value);
            }

            // Populate the Case Combo with possible input cases
            Dictionary<UserDataField.FieldCase, string> fieldCases = _field.FieldInputCases;
            foreach (KeyValuePair<UserDataField.FieldCase, string> kvp in fieldCases)
            {
                InputCase.Items.Add(kvp.Key, kvp.Value);
            }
            InputCase.SelectedIndex = 0;

            // Populate the Picklists combo with the names of any picklists that have been defined
            PickListList picklists = new PickListList();
            picklists.Populate();
            foreach (PickList picklist in picklists)
            {
                Picklist.Items.Add(picklist, picklist.Name);
            }

            if (Picklist.Items.Count != 0)
                Picklist.SelectedIndex = 0;

            // Set the initial data
            tbFieldName.Text = _field.Name;

            //TabOrder.Value = field.TabOrder;
            cbMandatory.Checked = _field.IsMandatory;
            cbInteractiveInclude.Checked = (_field.ParentScope == UserDataCategory.SCOPE.Asset);

            // Set the field type in the combo box
            int selectedIndex = cbFieldType.FindStringExact(fieldTypes[_field.Type]);
            cbFieldType.SelectedIndex = selectedIndex == -1 ? 0 : selectedIndex;

            // The remainder of the fields depend on the field type
            switch ((int)_field.Type)
            {
                // For text fields the input length is stored in 'Value1' and Input Case encoded into Value2
                case (int)UserDataField.FieldType.Text:

                    selectedIndex = InputCase.FindStringExact(_field.InputCase == "title" ? "TitleCase" : _field.InputCase);
                    InputCase.SelectedIndex = (selectedIndex == -1) ? 0 : selectedIndex;
                    break;

                // Numeric fields - Minimum is 'Value1' Maximum is 'Value2'
                case (int)UserDataField.FieldType.Number:
                    break;

                // For a picklist, the name of the picklist (if any) is stored in 'Value1'
                case (int)UserDataField.FieldType.Picklist:
                    selectedIndex = (Picklist.FindStringExact(_field.Picklist));
                    Picklist.SelectedIndex = (selectedIndex == -1) ? 0 : selectedIndex;
                    break;

                // Environment Variable - the permitted length is stored as 'Value1' with the name of the variable 
                // in question stored as 'Value2'
                case (int)UserDataField.FieldType.Environment:
                    tbEnvironmentVariableName.Text = _field.EnvironmentVariable;
                    break;

                // Registry Keys : The Key name is stored as value1 with the value name as Value2
                case (int)UserDataField.FieldType.Registry:
                    tbRegistryKey.Text = _field.RegistryKey;
                    tbRegistryValue.Text = _field.RegistryValue;
                    break;

                //// Boolean Fields - no additional fields required
                //case (int)UserDataField.FieldType.boolean:
                //    break;

                // Date Fields - no additional fields required
                case (int)UserDataField.FieldType.Date:
                    break;

                // Currency Fields - no additional fields required
                case (int)UserDataField.FieldType.Currency:
                    break;

                default:
                    MessageBox.Show("An invalid type has been identified in the User Data Field Definition");
                    break;
            }
        }

        /// <summary>
        /// Enable/Disable fields depending on the current Field Type
        /// </summary>
        protected void SetDisplayType()
        {
            UserDataField.FieldType fieldType = (UserDataField.FieldType)cbFieldType.SelectedItem.DataValue;

            pnlText.Visible = false;
            pnlRegistry.Visible = false;
            pnlPicklist.Visible = false;
            pnlNumeric.Visible = false;
            pnlEnvVar.Visible = false;

            switch ((int)fieldType)
            {
                case (int)UserDataField.FieldType.Text:
                    pnlText.Visible = true;
                    break;

                case (int)UserDataField.FieldType.Number:
                    //pnlNumeric.Visible = true;
                    break;

                case (int)UserDataField.FieldType.Picklist:
                    pnlPicklist.Visible = true;
                    break;

                case (int)UserDataField.FieldType.Environment:
                    pnlEnvVar.Visible = true;
                    //showMandatory = false;
                    break;

                case (int)UserDataField.FieldType.Registry:
                    pnlRegistry.Visible = true;
                    //showMandatory = false;
                    break;

                case (int)UserDataField.FieldType.Date:
                    break;

                case (int)UserDataField.FieldType.Currency:
                    //pnlNumeric.Visible = true;
                    break;
            }
        }


        /// <summary>
        /// Called as we change the item selected in the field type combo box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FieldType_SelectionChanged(object sender, EventArgs e)
        {
            SetDisplayType();
        }



        /// <summary>
        /// Called as we try an exit from this form - we need to ensure that the category name is unique
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnOK_Click(object sender, EventArgs e)
        {
            if (!ValidateData())
            {
                DialogResult = DialogResult.None;
                return;
            }

            // OK depending on the field type we need to save data in different ways
            SaveData();
        }



        /// <summary>
        /// Validate the data entered
        /// </summary>
        /// <returns></returns>
        private bool ValidateData()
        {
            // First ensure that we have specified the field name
            if (tbFieldName.Text == "")
            {
                MessageBox.Show("You must specify the name of the User Defined Data Field", "Validation Failed");
                tbFieldName.Focus();
                return false;
            }

            // Ensure that this field does not duplicate an existing one within the same category
            foreach (UserDataField field in _category)
            {
                if ((field.Name == _field.Name) && (field.FieldID != _field.FieldID))
                {
                    MessageBox.Show("A User Defined Data Field of this name already exists within this category, please enter a unique name for this field", "Field Exists");
                    tbFieldName.Focus();
                    return false;
                }
            }

            // Validate the remainder of the fields based on the field type
            UserDataField.FieldType fieldType = (UserDataField.FieldType)cbFieldType.SelectedItem.DataValue;
            switch ((int)fieldType)
            {
                // For text fields we need to the length and the Case - both must be valid anyway
                case (int)UserDataField.FieldType.Text:
                    break;

                // Numeric fields save the minimum and maximum values as Value1 and Value2 respectively ensuring that 
                // minimum is less than or equal to maximum
                case (int)UserDataField.FieldType.Number:
                    break;

                // Picklist - save the name of the picklist as Value1 - there must be a picklist specified
                case (int)UserDataField.FieldType.Picklist:
                    if (Picklist.SelectedIndex == -1)
                    {
                        MessageBox.Show("A picklist must be associated with this field", "Validation Error");
                        Picklist.Focus();
                        return false;
                    }
                    break;

                // Environment Variable - We must have specified a name
                case (int)UserDataField.FieldType.Environment:
                    if (tbEnvironmentVariableName.Text == "")
                    {
                        MessageBox.Show("You must specify the name of an environment variable for this field", "Validation Error");
                        tbEnvironmentVariableName.Focus();
                        return false;
                    }
                    break;

                // Registry Key - Key at least must be specified - an empty value means recover the default value
                case (int)UserDataField.FieldType.Registry:
                    if (tbRegistryKey.Text == "")
                    {
                        MessageBox.Show("You must specify a registry key for the field", "Validation Error");
                        tbRegistryKey.Focus();
                        return false;
                    }
                    break;

                //// No additional values required for boolean
                //case (int)UserDataField.FieldType.boolean:
                //    break;

                case (int)UserDataField.FieldType.Date:
                    if (_editing && _existingType != UserDataField.FieldType.Date)
                    {
                        if (MessageBox.Show("Please ensure any existing dates are stored in the format 'YYYY-MM-DD'." +
                            Environment.NewLine + Environment.NewLine +
                            "Any data not in this format may be lost. Do you wish to proceed?",
                            "Change data type",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.No)
                            return false;
                    }
                    break;

                case (int)UserDataField.FieldType.Currency:
                    if (_editing && _existingType != UserDataField.FieldType.Currency)
                    {
                        if (MessageBox.Show("Please ensure any existing currency data is stored in numeric format only." +
                            Environment.NewLine + Environment.NewLine +
                            "Any data not in this format may be lost. Do you wish to proceed?",
                            "Change data type",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.No)
                            return false;
                    }
                    break;
            }

            return true;
        }


        /// <summary>
        /// Save the data entered into the underlying UserDataField object
        /// </summary>
        private void SaveData()
        {
            // Have we changed the category?
            UserDataCategory parentCategory = cbCategories.SelectedItem.DataValue as UserDataCategory;
            if (parentCategory == null) 
                return;

            _field.ParentID = parentCategory.CategoryID;

            // Update the asset type definition with any changes
            _field.Name = tbFieldName.Text;

            if (!_editing)
                _field.TabOrder = parentCategory.Count;

            // if we are dealing with an asset, set the specific type based on the interactive checkbox
            if (_field.ParentScope != UserDataCategory.SCOPE.Application)
            {
                _field.ParentScope = (!cbInteractiveInclude.Checked) ? UserDataCategory.SCOPE.NonInteractiveAsset : UserDataCategory.SCOPE.Asset;
            }

            _field.IsMandatory = cbMandatory.Checked;
            _field.Type = (UserDataField.FieldType)cbFieldType.SelectedItem.DataValue;

            // Save the data back to the underlying field definition
            switch ((int)_field.Type)
            {
                // For text fields we need to save the length (as Value1) and the Case (as value2)
                case (int)UserDataField.FieldType.Text:
                    _field.InputCase = InputCase.SelectedItem.DataValue.ToString();
                    break;

                // Numeric fields save the minimum and maximum values as Value1 and Value2 respectively
                case (int)UserDataField.FieldType.Number:
                    break;

                // Picklist - save the name of the picklist as Value1
                case (int)UserDataField.FieldType.Picklist:
                    if (Picklist.SelectedItem != null)
                        _field.Picklist = ((PickList) Picklist.SelectedItem.DataValue).Name;
                    break;

                // Environment Variable - save the length as Value1 and the name of the variable as Value2
                case (int)UserDataField.FieldType.Environment:
                    _field.EnvironmentVariable = tbEnvironmentVariableName.Text;
                    break;

                // Registry Key - Save key as Value1 and the value as Value2
                case (int)UserDataField.FieldType.Registry:
                    _field.RegistryKey = tbRegistryKey.Text;
                    _field.RegistryValue = tbRegistryValue.Text;
                    break;

                // Date - no additional fields required
                case (int)UserDataField.FieldType.Date:
                    break;

                // Currency - no additional fields required
                case (int)UserDataField.FieldType.Currency:
                    break;

                // No additional values required for boolean
                //case (int)UserDataField.FieldType.boolean:
                //    break;
            }

            // ...and save the field
            _field.Add();
        }

        private void cbInteractiveInclude_CheckedChanged(object sender, EventArgs e)
        {
            if (cbInteractiveInclude.Checked == false)
            {
                cbMandatory.Checked = false;
            }
            cbMandatory.Enabled = cbInteractiveInclude.Checked;
        }

        private void bnRegistryBrowse_Click(object sender, EventArgs e)
        {
            FormRegistryBrowser form = new FormRegistryBrowser(false);
            if (form.ShowDialog() == DialogResult.OK)
            {
                tbRegistryValue.Text = form.RegValue;
                tbRegistryKey.Text = form.RegKey;
            }
        }

        private void bnEnvVarBrowse_Click(object sender, EventArgs e)
        {
            FormEnvVarBrowser form = new FormEnvVarBrowser();
            if(form.ShowDialog() == DialogResult.OK)
            {
                tbEnvironmentVariableName.Text = form.EnvVariable;
            }
        }
    }
}