using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.SmartParts;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Utility;
using Microsoft.Practices.ObjectBuilder;
//
using Infragistics.Win.UltraWinListView;
using Infragistics.Win;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

//
using PickerSample;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class SerialNumbersTabView : UserControl, ILaytonView, IAdministrationView
    {
        private LaytonWorkItem workItem;
		SerialNumbersTabViewPresenter presenter;
		private ApplicationDefinitionsFile _applicationsFile = new ApplicationDefinitionsFile();

        [InjectionConstructor]
		public SerialNumbersTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
        }

		[CreateNew]
		public SerialNumbersTabViewPresenter Presenter
		{
			set { presenter = value; presenter.View = this; presenter.Initialize(); }
			get { return presenter; }
		}

        public void RefreshViewSinglePublisher()
        {
        }

		/// <summary>
		/// Refresh the current view
		/// </summary>
		public void RefreshView()
		{
			base.Refresh();
			InitializeRegistryMappings();
		}


		/// <summary>
		/// Called as this tab is activated to ensure that we display the latest possible data
		/// This function comes from the IAdministrationView Interface
		/// </summary>
		public void Activate()
		{
			InitializeRegistryMappings();
		}


		/// <summary>
		/// save function for the IAdministrationView Interface
		/// </summary>
		public void Save()
		{
		}

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }


		/// <summary>
		/// Load data into the controls
		/// </summary>
		private void InitializeRegistryMappings()
		{
			// Populate the combo box with each of the applications for which we have already got
			// a registry mapping
			_applicationsFile = new ApplicationDefinitionsFile();
			List<string> listRegistryMappings = new List<string>();
			_applicationsFile.SetSection(ApplicationDefinitionsFile.APPLICATION_MAPPINGS_SECTION);
			_applicationsFile.EnumerateKeys(ApplicationDefinitionsFile.APPLICATION_MAPPINGS_SECTION, listRegistryMappings);

			// Iterate through the keys and create an application mapping object
			cbApplications.BeginUpdate();
			cbApplications.Items.Clear();
			//
			foreach (string thisKey in listRegistryMappings)
			{
				// The key is the application name, the value the registry mapping
				ApplicationRegistryMapping applicationMapping = new ApplicationRegistryMapping(thisKey);
				String mappings = _applicationsFile.GetString(thisKey, "");
				if (thisKey == "Travel")
				{
					int a = 0;
				}
				applicationMapping.AddMapping(mappings);

				// Add this entry to the combo box noting that we store the object with it to make
				// it easier to display the registry keys when selected
				ValueListItem newItem = cbApplications.Items.Add(applicationMapping.ApplicationName);
				newItem.Tag = applicationMapping;
			}
			cbApplications.EndUpdate();
			
			// Select the first entry
			cbApplications.SelectedIndex = 0;
		}



		/// <summary>
		/// Add a new application to the list of registry mappings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnNewApplication_Click(object sender, EventArgs e)
		{
			FormSelectApplication form = new FormSelectApplication();
			if (form.ShowDialog() == DialogResult.OK)
			{
				String newApplication = form.SelectedApplication;
				ApplicationRegistryMapping applicationMapping = new ApplicationRegistryMapping(newApplication);

				// Add this entry to the combo box noting that we store the object with it to make
				// it easier to display the registry keys when selected
				ValueListItem newItem = cbApplications.Items.Add(applicationMapping.ApplicationName);
				newItem.Tag = applicationMapping;
			}
		}


		/// <summary>
		/// Called when we want to add a new registry mapping
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnAddKey_Click(object sender, EventArgs e)
		{
			FormShadedAskInput2 form = new FormShadedAskInput2("Please enter the registry key name and value name to map to the application.  Do not specify the registry hive as HKEY_LOCAL_MACHINE is assumed."
												 , "Enter Registry Key (beneath HKEY_LOCAL_MACHINE)"
												 , "Registry Key Name:", "Value Name", "", "" ,Properties.Resources.application_serial_number_corner
                                                 , true);
			if (form.ShowDialog() == DialogResult.OK)
			{
				// OK add the new mapping to our list
				RegistryMapping mapping = new RegistryMapping(form.Value1Entered(), form.Value2Entered());
				UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[1];
				subItemArray[0] = new UltraListViewSubItem();
				subItemArray[0].Value = mapping.ValueName;
				UltraListViewItem item = new UltraListViewItem(mapping.RegistryKey, subItemArray);
				item.Tag = mapping;
				lvRegistryKeys.Items.Add(item);

				// Build the mappings string
				String mappingString = BuildMappingString();

				// ..and update the configuration file
				_applicationsFile.SetString(cbApplications.SelectedItem.ToString(), mappingString, true);
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
			RegistryMapping editMapping = editItem.Tag as RegistryMapping;

			// ...and invoke a form to edit it
            //FormAskInput2 form = new FormAskInput2("Please enter the registry key name and value name to map to the application.  Do not specify the registry hive as HKEY_LOCAL_MACHINE is assumed"
            //                                     , "Enter Registry Key (beneath HKEY_LOCAL_MACHINE)"
            //                                     , "Registry Key Name:"
            //                                     , "Value Name"
            //                                     , editMapping.RegistryKey
            //                                     , editMapping.ValueName);
            FormShadedAskInput2 form = new FormShadedAskInput2("Please enter the registry key name and value name to map to the application.  Do not specify the registry hive as HKEY_LOCAL_MACHINE is assumed"
                                                 , "Enter Registry Key (beneath HKEY_LOCAL_MACHINE)"
                                                 , "Registry Key Name:"
                                                 , "Value Name"
                                                 , editMapping.RegistryKey
                                                 , editMapping.ValueName
                                                 , Properties.Resources.application_serial_number_corner
                                                 , true);
			if (form.ShowDialog() == DialogResult.OK)
			{
				// Remove the existing item from the list
				int index = editItem.Index;
				lvRegistryKeys.Items.Remove(editItem);

				// ...and add in the new item
				RegistryMapping mapping = new RegistryMapping(form.Value1Entered(), form.Value2Entered());
				UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[1];
				subItemArray[0] = new UltraListViewSubItem();
				subItemArray[0].Value = mapping.ValueName;
				UltraListViewItem item = new UltraListViewItem(mapping.RegistryKey, subItemArray);
				item.Tag = mapping;
				lvRegistryKeys.Items.Insert(index, item);

				// Build the mappings string
				String mappingString = BuildMappingString();

				// ..and update the configuration file
				_applicationsFile.SetString(cbApplications.SelectedItem.ToString(), mappingString, true);
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

			if (MessageBox.Show("Are you certain that you want to delete this registry key mapping?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				// Remove the mapping from the list
				lvRegistryKeys.Items.Remove(lvRegistryKeys.SelectedItems[0]);

				// Build the mappings string
				String mappingsString = BuildMappingString();

				// ..and update the configuration file
				_applicationsFile.SetString(cbApplications.SelectedItem.ToString(), mappingsString, true);
			}
		}



		/// <summary>
		/// Called to set the display state of the buttons on this form
		/// </summary>
		private void SetMappingsButtonStates()
		{
			// Delete and edit are only applicable if we have an item selected
			bnDeleteKey.Enabled = (lvRegistryKeys.SelectedItems.Count != 0);
			bnNewapplication.Enabled = (lvRegistryKeys.SelectedItems.Count != 0);
		}



		/// <summary>
		/// Double clicking on an item in the list box should select that item and then edit it
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lvRegistryKeys_ItemDoubleClick(object sender, Infragistics.Win.UltraWinListView.ItemDoubleClickEventArgs e)
		{
			// Add this item to the selected items collection
			UltraListViewItem clickedItem = e.Item;
			lvRegistryKeys.SelectedItems.Add(clickedItem);

			// ...then use the edit function
			bnEditKey_Click(sender, e);
		}



		/// <summary>
		/// Called as we change the selection in the combo box of applications
		/// we need to populate the list of registry mappings for the newly selected
		/// application
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbApplications_SelectionChanged(object sender, EventArgs e)
		{
			// First clear the list view of any existing items
			lvRegistryKeys.Items.Clear();

			// Get the selected application object
			ApplicationRegistryMapping applicationMapping = cbApplications.SelectedItem.Tag as ApplicationRegistryMapping;

			// ...and add to the list view
			foreach (RegistryMapping mapping in applicationMapping)
			{
				UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[1];
				subItemArray[0] = new UltraListViewSubItem();
				subItemArray[0].Value = mapping.ValueName;
				UltraListViewItem item = new UltraListViewItem(mapping.RegistryKey, subItemArray);
				item.Tag = mapping;
				lvRegistryKeys.Items.Add(item);
			}
		}


		/// <summary>
		/// Called as we change the registry key selected in the list view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lvRegistryKeys_ItemSelectionChanged(object sender, Infragistics.Win.UltraWinListView.ItemSelectionChangedEventArgs e)
		{
			bnEditKey.Enabled = (lvRegistryKeys.SelectedItems.Count != 0);
			bnDeleteKey.Enabled = (lvRegistryKeys.SelectedItems.Count != 0);
		}



		/// <summary>
		/// Build a string which combines the individual registry mappings into a single string
		/// </summary>
		/// <returns></returns>
		private String BuildMappingString()
		{
			String mappingString = "";
			foreach (UltraListViewItem item in lvRegistryKeys.Items)
			{
				if (mappingString != "")
					mappingString += ";";

				// +8.3.6
				//
				// Get the registry key and value and ensure that they are quoted if they contain a comma as we use the comma as
				// the delimiter here
				string registryKey = item.Text;
				string registryValue = item.SubItems[0].Text;
				if (registryKey.Contains(","))
					registryKey = "\"" + registryKey + "\"";
				if (registryValue.Contains(","))
					registryValue = "\"" + registryValue + "\"";
				mappingString = mappingString + registryKey + "," + registryValue;
			}
			return mappingString;
		}

    }
}
