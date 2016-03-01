using System;
using System.Windows.Forms;
using Infragistics.Win.UltraWinListView;
using Layton.AuditWizard.Common;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Administration
{
	public partial class FormRegistryDetails : AWForm
	{
		AuditScannerDefinition _scannerConfiguration;

		public FormRegistryDetails(AuditScannerDefinition scannerConfiguration)
		{
			InitializeComponent();
			_scannerConfiguration = scannerConfiguration;
		}

		/// <summary>
		/// Called as the form is loading - populate from the configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormRegistryDetails_Load(object sender, EventArgs e)
		{
			InitializeRegistryMappings();
		}


		/// <summary>
		/// Load data into the controls
		/// </summary>
		private void InitializeRegistryMappings()
		{
			foreach (string registryKey in _scannerConfiguration.ListRegistryKeys)
			{
				string key = "";
				string value = "";
				int delimiter = registryKey.IndexOf(';');
				if (delimiter == -1)
				{
					key = registryKey;
				}
				else
				{
					key = registryKey.Substring(0, delimiter);
					value = registryKey.Substring(delimiter + 1);
				}

				// Add to the list
				UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[1];
				subItemArray[0] = new UltraListViewSubItem();
				subItemArray[0].Value = value;
				UltraListViewItem item = new UltraListViewItem(key, subItemArray);
				lvRegistryKeys.Items.Add(item);
			}
		}



		/// <summary>
		/// Called when we want to add a new registry mapping
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnAddKey_Click(object sender, EventArgs e)
		{
			FormShadedAskInput2 form = new FormShadedAskInput2("Please enter the registry key name and value name to be audited.  You should include the registry hive (e.g. HKEY_LOCAL_MACHINE)"
												 , "Enter Registry Key"
												 , "Registry Key Name:"
												 , "Value Name"
												 , ""
												 , ""
												 , Properties.Resources.application_serial_number_corner,
                                                 false);
			if (form.ShowDialog() == DialogResult.OK)
			{
				// OK add the new mapping to our list
				UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[1];
				subItemArray[0] = new UltraListViewSubItem();
				subItemArray[0].Value = form.Value2Entered();
				UltraListViewItem item = new UltraListViewItem(form.Value1Entered(), subItemArray);
				lvRegistryKeys.Items.Add(item);
			}
		}



		/// <summary>
		/// Edit an existing registry key mapping
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnEditKey_Click(object sender, EventArgs e)
		{
			// Get the mapping that is to be edited
			UltraListViewItem editItem = lvRegistryKeys.SelectedItems[0];
			string key = editItem.Text;
			string value = editItem.SubItems[0].Text;

			// ...and invoke a form to edit it
			FormShadedAskInput2 form = new FormShadedAskInput2("Please enter the registry key name and value name to be audited.  You should include the registry hive (e.g. HKEY_LOCAL_MACHINE)"
												 , "Enter Registry Key"
												 , "Registry Key Name:"
												 , "Value Name"
												 , key
												 , value
												 , Properties.Resources.application_serial_number_corner,
                                                 false);
			if (form.ShowDialog() == DialogResult.OK)
			{
				// Remove the existing item from the list
				int index = editItem.Index;
				lvRegistryKeys.Items.Remove(editItem);

				// ...and add in the new item
				UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[1];
				subItemArray[0] = new UltraListViewSubItem();
				subItemArray[0].Value = form.Value2Entered();
				UltraListViewItem item = new UltraListViewItem(form.Value1Entered(), subItemArray);
				lvRegistryKeys.Items.Add(item);
			}
		}


		/// <summary>
		/// Called to delete the selected registry mapping after confirmation
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnDeleteKey_Click(object sender, EventArgs e)
		{
			// Sanity check - ensure that we have an item selected
			if (lvRegistryKeys.SelectedItems.Count == 0)
				return;

			if (MessageBox.Show("Are you certain that you want to delete this registry key?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				lvRegistryKeys.Items.Remove(lvRegistryKeys.SelectedItems[0]);
			}
		}


		/// <summary>
		/// Called to set the display state of the buttons on this form
		/// </summary>
		private void SetMappingsButtonStates()
		{
			// Delete and edit are only applicable if we have an item selected
			bnDeleteKey.Enabled = (lvRegistryKeys.SelectedItems.Count != 0);
			bnEditKey.Enabled = (lvRegistryKeys.SelectedItems.Count != 0);
		}

		private void lvRegistryKeys_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
		{
			SetMappingsButtonStates();
		}


		/// <summary>
		/// Called as we select to exit the form.  We need to save the potntially updated list of registry keys
		/// back to the configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			_scannerConfiguration.ListRegistryKeys.Clear();
			foreach (UltraListViewItem item in this.lvRegistryKeys.Items)
			{
				string entry = item.Text + ";" + item.SubItems[0].Text;
				_scannerConfiguration.ListRegistryKeys.Add(entry);
			}
		}

	}
}

