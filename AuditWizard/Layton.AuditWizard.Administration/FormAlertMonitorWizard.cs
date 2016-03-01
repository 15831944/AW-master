using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using Infragistics.Win.UltraWinListView;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
    public partial class FormAlertMonitorWizard : Form
    {
        #region Data

        private AlertDefinition _alertDefinition;
        private string _selectedField;
        private bool _editing;
        private string _originalName;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Contructor

        public FormAlertMonitorWizard()
        {
            InitializeComponent();
            _alertDefinition = new AlertDefinition();
            wizardControl1.NextButtonEnabled = false;
        }

        public FormAlertMonitorWizard(AlertDefinition alertDefinition)
        {
            InitializeComponent();
            _alertDefinition = alertDefinition;
            _editing = true;
            _originalName = alertDefinition.Name;

            wizardControl1.NextButtonEnabled = true;
        }

        #endregion

        #region Get Basic Details Screen

        private bool processStepGetBasicDetails()
        {
            string selectedFileName = Path.Combine(Application.StartupPath, "scanners\\alertmonitors\\") + tbAlertMonitorName.Text + ".xml";

            if (tbAlertMonitorName.Text.IndexOfAny(Path.GetInvalidFileNameChars()) != -1 || tbAlertMonitorName.Text.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                Utility.DisplayApplicationErrorMessage("The alert name cannot contain any illegal characters. Please edit the name.");
                return false;
            }

            if (!_editing)
            {
                if (File.Exists(selectedFileName))
                {
                    if (MessageBox.Show("The config file '" + tbAlertMonitorName.Text + "' already exists." + Environment.NewLine + Environment.NewLine +
                        "Do you wish to proceed and overwrite the file?", "AuditWizard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return false;
                    }
                }
            }

            _alertDefinition.Name = tbAlertMonitorName.Text;
            _alertDefinition.Description = tbAlertDescription.Text;
            _alertDefinition.EmailAlert = cbEmailAlert.Checked;

            stepGetTriggers1.Subtitle = _alertDefinition.Name;
            stepGetComputers1.Subtitle = _alertDefinition.Name;

            return true;
        }

        #endregion

        #region Get Triggers Screen

        private void processStepGetTriggers()
        {
            _alertDefinition.Triggers.Clear();

            foreach (UltraListViewItem item in lvTriggers.Items)
            {
                AlertTrigger newTrigger = new AlertTrigger();
                newTrigger.TriggerField = item.Value.ToString();

                switch (item.SubItems[0].Value.ToString().ToLower())
                {
                    case "changed":
                        newTrigger.Condition = AlertTrigger.eCondition.changed;
                        break;
                    case "contains":
                        newTrigger.Condition = AlertTrigger.eCondition.contains;
                        break;
                    case "less than":
                        newTrigger.Condition = AlertTrigger.eCondition.lessthan;
                        break;
                    case "greater than":
                        newTrigger.Condition = AlertTrigger.eCondition.greaterthan;
                        break;
                    case "equals":
                        newTrigger.Condition = AlertTrigger.eCondition.equals;
                        break;
                    case "less or equals":
                        newTrigger.Condition = AlertTrigger.eCondition.lessequals;
                        break;
                    case "greater or equals":
                        newTrigger.Condition = AlertTrigger.eCondition.greaterequals;
                        break;
                    default:
                        break;
                }

                newTrigger.Value = item.SubItems[1].Value.ToString();
                _alertDefinition.Triggers.Add(newTrigger);
            }

        }

        /// <summary>
        /// Called as the selected item in the tree control is changed as this affects the possible 
        /// coniditons and values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="node"></param>
        void selectedFields_AfterSelect(object sender, Infragistics.Win.UltraWinTree.UltraTreeNode node)
        {
            lblUnits.Text = String.Empty;

            if (node == null)
            {
                _selectedField = "";
                return;
            }

			// If we have selected Applications or Intrernet then enable immediate checking as we can drill down no further
            if (node.Text == "Applications")
            {
                lbConditions.Enabled = true;
                lbValue.Enabled = false;
                cbConditions.Enabled = true;
                tbValue.Enabled = false;
                lbAddTrigger.Enabled = true;
                bnAddTrigger.Enabled = true;
            }

			else if  (node.Text == "Internet")
			{
				lbConditions.Enabled = true;
				lbValue.Enabled = true;
				cbConditions.Enabled = true;
				tbValue.Enabled = true;
				lbAddTrigger.Enabled = true;
				bnAddTrigger.Enabled = true;
			}
	
			else if (node.HasNodes)
            {
                lbConditions.Enabled = false;
                lbValue.Enabled = false;
                cbConditions.Enabled = false;
                tbValue.Enabled = false;
                lbAddTrigger.Enabled = false;
                bnAddTrigger.Enabled = false;
            }

            else
            {
                lbConditions.Enabled = true;
                lbValue.Enabled = true;
                cbConditions.Enabled = true;
                tbValue.Enabled = true;
                lbAddTrigger.Enabled = true;
                bnAddTrigger.Enabled = true;
            }

            // OK split the item into its component parts
            string itemKey = node.Key;
            _selectedField = itemKey;

            List<string> itemParts = Utility.ListFromString(itemKey, '|', true);

            // Clear existing conditions
            cbConditions.Items.Clear();

            // Internet Node - just add 'Contains'
            if (itemParts[0] == AWMiscStrings.InternetNode)
            {
                cbConditions.Items.Add(AlertTrigger.eCondition.contains, "Contains");
            }

			else
			{
				// All other types add 'Changed' as always valid
                cbConditions.Items.Add(AlertTrigger.eCondition.changed, "Changed");

                // Some specific fields should also allow other numeric operations
				if (itemParts.Count >= 3)
				{
					string fieldName = itemParts[itemParts.Count - 1];
					string strDrivePart=itemParts[itemParts.Count-2];
					if ((fieldName == "Count")
					|| (fieldName == "Speed")
					|| (fieldName.StartsWith("Available"))
					|| (fieldName.StartsWith("Total"))
					|| (fieldName.StartsWith("Processor Count"))
					|| (fieldName.StartsWith("Core Count"))
					|| (fieldName.StartsWith("Free")))
					{
						cbConditions.Items.Add(AlertTrigger.eCondition.equals, "Equals");
						cbConditions.Items.Add(AlertTrigger.eCondition.greaterthan, "Greater Than");
						cbConditions.Items.Add(AlertTrigger.eCondition.greaterequals, "Greater or Equals");
						cbConditions.Items.Add(AlertTrigger.eCondition.lessthan, "Less Than");
						cbConditions.Items.Add(AlertTrigger.eCondition.lessequals, "Less or Equals");
						tbValue.Enabled = true;
					}

					else
					{
						tbValue.Enabled = false;
					}

					if ((fieldName.StartsWith("Total") || fieldName.StartsWith("Available") || fieldName.StartsWith("Free"))&&(strDrivePart.StartsWith("Drive")))
						lblUnits.Text = "GB";

					else if(fieldName.StartsWith("Total") || fieldName.StartsWith("Available") || fieldName.StartsWith("Free"))
						lblUnits.Text = "MB";

					else if (fieldName == "Speed")
						lblUnits.Text = "MHz";

					else
						lblUnits.Text = String.Empty;
				}
            }

            cbConditions.SelectedIndex = 0;

        }

        /// <summary>
        /// Called when the Add button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnAddTrigger_Click(object sender, EventArgs e)
        {
            // We must have a field selected
            if (_selectedField == "")
            {
                MessageBox.Show("You must select a field to alert on", "Invalid Trigger");
                DialogResult = DialogResult.None;
                return;
            }

            if (tbValue.Enabled && tbValue.Text == "")
            {
                MessageBox.Show("You must specify a value to compare against", "Invalid Trigger");
                DialogResult = DialogResult.None;
                return;
            }

            if (cbConditions.SelectedIndex == -1)
            {
                MessageBox.Show("You must select a condition for the test", "Invalid Trigger");
                DialogResult = DialogResult.None;
                return;
            }

            AlertTrigger newAlertTrigger = new AlertTrigger();

            newAlertTrigger.TriggerField = _selectedField;
            newAlertTrigger.Condition = (AlertTrigger.eCondition)cbConditions.SelectedItem.DataValue;
            newAlertTrigger.Value = tbValue.Text;

            AddTriggerToList(newAlertTrigger);
            _alertDefinition.Triggers.Add(newAlertTrigger);

            wizardControl1.NextButtonEnabled = true;
        }

        /// <summary>
        /// Adds a newly defined trigger to the list
        /// </summary>
        /// <param name="alertTrigger"></param>
        private void AddTriggerToList(AlertTrigger alertTrigger)
        {
            UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[2];
            subItemArray[0] = new UltraListViewSubItem();
            subItemArray[0].Value = alertTrigger.ConditionAsText;
            subItemArray[1] = new UltraListViewSubItem();
            subItemArray[1].Value = alertTrigger.Value;
            //
            UltraListViewItem item = new UltraListViewItem(alertTrigger.TriggerField, subItemArray);
            item.Tag = alertTrigger;
            lvTriggers.Items.Add(item);
        }

        #endregion

        #region Get Computers Screen

        private void processStepGetComputers()
        {
            AssetGroupList listSelectedGroups;
            AssetList listSelectedAssets;
            _alertDefinition.MonitoredComputers.Clear();

            selectLocationsControl.GetSelectedItemsAlertMonitor(out listSelectedGroups, out listSelectedAssets);

            foreach (Asset asset in listSelectedAssets)
            {
                _alertDefinition.MonitoredComputers.Add(asset.Name);
            }

            foreach (Asset groupAsset in listSelectedGroups.GetAllAssets())
            {
                _alertDefinition.MonitoredComputers.Add(groupAsset.Name);
            }
        }


        #endregion

        #region Event Handlers

        private void Form_Load(object sender, EventArgs e)
        {
			// Don't populate the control in design mode
			if (DesignMode)
				return;

			// basic settings
            tbAlertMonitorName.Text = _alertDefinition.Name;
            tbAlertDescription.Text = _alertDefinition.Description;
            cbEmailAlert.Checked = _alertDefinition.EmailAlert;

            // trigger settings
            selectedFields.AfterSelect += new AfterSelectEventHandler(selectedFields_AfterSelect);

            foreach (AlertTrigger trigger in _alertDefinition.Triggers)
            {
                AddTriggerToList(trigger);
            }

			// CMD 8.4.1
            // select computers settings - note that we opt to display root level assets but not lower level as these
			// shgould only be populated as and when required
            selectLocationsControl.Populate(true, false, true);

            // get the ID of the asset first - selectLocationsControl.RestoreSelections requires an ID
            List<string> monitoredComputers = new List<string>();
            AssetDAO lAssetDao = new AssetDAO();

            foreach (string monitoredComputer in _alertDefinition.MonitoredComputers)
            {
                monitoredComputers.Add(lAssetDao.ConvertNameListToIds(monitoredComputer));
            }

            if (_alertDefinition.MonitoredComputers.Count > 0)
                selectLocationsControl.RestoreSelections("", Utility.ListToString(monitoredComputers, ','));
        }

        /// <summary>
        /// Called as we skip back through the wizard pages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void wizardControl_NextButtonClick(WizardBase.WizardControl sender, WizardBase.WizardNextButtonClickEventArgs args)
        {
            switch (wizardControl1.CurrentStepIndex)
            {
                case 0:
                    if (!processStepGetBasicDetails())
                        args.Cancel = true;
                    break;
                case 1:
                    processStepGetTriggers();
                    wizardControl1.NextButtonEnabled = true;
                    break;
                case 2:
                    processStepGetComputers();
                    wizardControl1.NextButtonEnabled = true;
                    break;
                default:
                    break;
            }
        }

        private void wizardControl_FinishButtonClick(object sender, EventArgs e)
        {
            // We now need to serialize the AlertDefinitions
            TextWriter textWriter = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AlertDefinition));

                string alertMonitorDirectory = Path.Combine(Application.StartupPath, "scanners\\alertmonitors\\");
                string fileName = alertMonitorDirectory + _alertDefinition.Name + ".xml";

                if (!Directory.Exists(alertMonitorDirectory))
                    Directory.CreateDirectory(alertMonitorDirectory);

                textWriter = new StreamWriter(fileName);
                serializer.Serialize(textWriter, _alertDefinition);

                // if we were editing and changed name, delete the original
                if (_editing && (_alertDefinition.Name != _originalName))
                    File.Delete(alertMonitorDirectory + _originalName + ".xml");

                DesktopAlert.ShowDesktopAlert("The alert monitor '" + _alertDefinition.Name + "' has been saved.");
            }
            catch (Exception ex)
            {
                logger.Error("Error in wizardControl_FinishButtonClick", ex);
                Utility.DisplayApplicationErrorMessage("An error occurred whilst trying to save the alert definition.");
            }
            finally
            {
                // close text writer
                if (textWriter != null)
                    textWriter.Close();

                // close the form                
                Close();
            }
        }

        /// <summary>
        /// Handle canceling from the wizard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wizardControl_CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void cbConditions_ValueChanged(object sender, EventArgs e)
        {
            tbValue.Enabled = ((AlertTrigger.eCondition)cbConditions.SelectedItem.DataValue != AlertTrigger.eCondition.changed);
        }

        /// <summary>
        /// Called as move off the single folder  so that we can validate the path specified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAlertMonitorName_Leave(object sender, EventArgs e)
        {
        }

        private void btnDeleteTrigger_Click(object sender, EventArgs e)
        {
            UltraListViewItem lvi = lvTriggers.SelectedItems[0];
            int index = lvi.Index;

            // Remove the item from the list view
            lvTriggers.Items.Remove(lvi);

            // Select the next definition in the list if any
            if (index >= lvTriggers.Items.Count)
                index--;
            if (index >= 0)
                lvTriggers.SelectedItems.Add(lvTriggers.Items[index]);

            wizardControl1.NextButtonEnabled = (lvTriggers.Items.Count > 0);

        }

        private void lvTriggers_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            btnDeleteTrigger.Enabled = (lvTriggers.SelectedItems.Count != 0);
        }

        private void tbAlertMonitorName_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox != null)
            {
                if (textBox.Text.Length > 0)
                {
                    string letter = textBox.Text.Substring(textBox.Text.Length - 1);

                    if (letter.IndexOfAny(Path.GetInvalidFileNameChars()) != -1 ||
                        letter.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                    {
                        textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
                        textBox.Select(textBox.Text.Length, 0);

                        Utility.DisplayApplicationErrorMessage(
                            "An alert name cannot contain any of the following characters:" +
                            Environment.NewLine + Environment.NewLine +
                            @"\ / : * ? "" < > |");

                        return;
                    }
                }
            }

            wizardControl1.NextButtonEnabled = tbAlertMonitorName.Text.Length > 0;
        }

        #endregion
    }
}
