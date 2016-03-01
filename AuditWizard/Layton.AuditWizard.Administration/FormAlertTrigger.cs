using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win.UltraWinTree;
//
using Layton.Common.Controls;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
	public partial class FormAlertTrigger : ShadedImageForm
	{
		private AlertDefinition _alertDefinition;
		private AlertTrigger	_alertTrigger;
		private string			_selectedField;

		public AlertTrigger AlertTrigger
		{
			get { return _alertTrigger; }
			set { _alertTrigger = value; }
		}

		public FormAlertTrigger(AlertDefinition alertDefinition ,AlertTrigger alertTrigger)
		{
			InitializeComponent();
			_alertDefinition = alertDefinition;
			_alertTrigger = alertTrigger;

			// Seems as if some attributes simply do not get set for the embedded SelectAuditDataFieldsControl so set them here
			selectedFields.AllowExpandApplications = false;
			selectedFields.AllowInternetSelection = true;
			selectedFields.ReportSpecificItemsShow = false;
			selectedFields.NodeStyle = NodeStyle.Standard;
		}


		/// <summary>
		/// Called as we load the form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormAlertTrigger_Load(object sender, EventArgs e)
		{
			tbName.Text = _alertDefinition.Name;

			// Setup a handler so that we know when a field is selected in the tree
			selectedFields.AfterSelect += new AfterSelectEventHandler(selectedFields_AfterSelect);
			
			// Do we already have a field selected>?  If so select it again in the tree
			if (_alertTrigger.TriggerField != "")
				selectedFields.SelectedItems = new List<string>(Utility.ListFromString(_alertTrigger.TriggerField, '|' ,true));
						
		}


		/// <summary>
		/// Called as the selected item in the tree control is changed as this affects the possible 
		/// coniditons and values
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
			
			// OK split the item into its component parts
			string itemKey = node.Key;
			_selectedField = itemKey;
			
			List<string> itemParts = Utility.ListFromString(itemKey ,'|' ,true);
			
			// Clear existing conditions
			cbConditions.Items.Clear();
			
			// 'Changed' is valid for all but Internet fields
			if (itemParts[0] == AWMiscStrings.InternetNode)
			{
				cbConditions.Items.Add(AlertTrigger.eCondition.contains ,"Contains");
				tbValue.Enabled = true;
			}
			
			else
			{
				cbConditions.Items.Add(AlertTrigger.eCondition.changed, "Changed");
				
				// Some specific fields should also allow other numeric operations
				string fieldName = itemParts[itemParts.Count - 1];
				if ((fieldName == "Count") 
				||  (fieldName == "Speed")
				||  (fieldName.StartsWith("Available"))
				||  (fieldName.StartsWith("Total"))
				||  (fieldName.StartsWith("Free")))
				{
					cbConditions.Items.Add(AlertTrigger.eCondition.equals ,"Equals");
					cbConditions.Items.Add(AlertTrigger.eCondition.greaterthan ,"Greater Than");
					cbConditions.Items.Add(AlertTrigger.eCondition.greaterequals, "Greater or Equals");
					cbConditions.Items.Add(AlertTrigger.eCondition.lessthan, "Less Then");
					cbConditions.Items.Add(AlertTrigger.eCondition.lessequals ,"Less or Equals");
					tbValue.Enabled = true;
				}
				
				else
				{
					tbValue.Enabled = false;
				}
			}
						
			cbConditions.SelectedIndex = 0;
			
		}



		/// <summary>
		/// Called as we try to exit from this form to create the alert trigger - get the selected fields
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			// We must have a field selected
			if (_selectedField == "")
			{
				MessageBox.Show("You must select a field to alert on" ,"Invalid Trigger");
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
			
			_alertTrigger.TriggerField = _selectedField;
			_alertTrigger.Condition = (AlertTrigger.eCondition)cbConditions.SelectedItem.DataValue;
			_alertTrigger.Value = tbValue.Text;
		}

		private void cbConditions_ValueChanged(object sender, EventArgs e)
		{
			tbValue.Enabled = ((AlertTrigger.eCondition)cbConditions.SelectedItem.DataValue != AlertTrigger.eCondition.changed);
		}
	}
}