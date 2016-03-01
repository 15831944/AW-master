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
using Layton.AuditWizard.DataAccess;
//
using PickerSample;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class LicenseTypesTabView : UserControl, ILaytonView, IAdministrationView
    {
        private LaytonWorkItem workItem;
		LicenseTypesTabViewPresenter presenter;

        [InjectionConstructor]
		public LicenseTypesTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
        }

		[CreateNew]
		public LicenseTypesTabViewPresenter Presenter
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
			InitializeLicenseTypes();
		}


		/// <summary>
		/// Called as this tab is activated to ensure that we display the latest possible data
		/// This function comes from the IAdministrationView Interface
		/// </summary>
		public void Activate()
		{
			InitializeLicenseTypes();
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
		/// Initialize the license types form
		/// </summary>
		private void InitializeLicenseTypes()
		{
			lvLicenseTypes.BeginUpdate();
			lvLicenseTypes.Items.Clear();

			// Populate the list view with the existing license types
			LicenseTypesDAO lwDataAccess = new LicenseTypesDAO();
			DataTable licenseTypesTable = lwDataAccess.EnumerateLicenseTypes();

			// Iterate through the returned items
			foreach (DataRow row in licenseTypesTable.Rows)
			{
				LicenseType licenseType = new LicenseType(row);
				UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[1];
				subItemArray[0] = new UltraListViewSubItem();
				subItemArray[0].Value = (licenseType.PerComputer) ? "Yes" : "No";
				UltraListViewItem item = new UltraListViewItem(licenseType.Name, subItemArray);
				item.Tag = licenseType;
				lvLicenseTypes.Items.Add(item);
			}
			lvLicenseTypes.EndUpdate();
		}

		/// <summary>
		/// Called when we want to edit the current license type
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnEditLicenseType_Click(object sender, EventArgs e)
		{
			FormLicenseType form = new FormLicenseType(lvLicenseTypes.SelectedItems[0].Tag as LicenseType);
			if (form.ShowDialog() == DialogResult.OK)
			{
				UltraListViewItem editedItem = lvLicenseTypes.SelectedItems[0];
				LicenseType editedLicenseType = form.LicenseType;
				editedItem.Tag = editedLicenseType;

				// Re-load the data
				InitializeLicenseTypes();
			}
		}


		/// <summary>
		/// Called when we want to add a new license type
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnAddLicenseType_Click(object sender, EventArgs e)
		{
			FormLicenseType form = new FormLicenseType(null);
			if (form.ShowDialog() == DialogResult.OK)
			{
				LicenseType newLicenseType = form.LicenseType;
				UltraListViewSubItem[] subItemArray = new UltraListViewSubItem[1];
				subItemArray[0] = new UltraListViewSubItem();
				subItemArray[0].Value = (newLicenseType.PerComputer) ? "Yes" : "No";
				UltraListViewItem item = new UltraListViewItem(newLicenseType.Name, subItemArray);
				item.Tag = newLicenseType;
				lvLicenseTypes.Items.Add(item);
			}
		}


		/// <summary>
		/// Called to delete the selected license type after confirmation
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnDeleteLicenseType_Click(object sender, EventArgs e)
		{
			// Sanity check - ensure that we have an item selected
			if (lvLicenseTypes.SelectedItems.Count == 0)
				return;

			// Get the database object
            LicensesDAO lwDataAccess = new LicensesDAO();

			// First ensure that we do not have any references to this license type as we 
			// cannot delete it if we have
			LicenseType deleteLicenseType = lvLicenseTypes.SelectedItems[0].Tag as LicenseType;
			DataTable references = lwDataAccess.EnumerateLicenses(deleteLicenseType);
			if (references.Rows.Count != 0)
			{
				MessageBox.Show("Cannot delete this license type as Licenses exist which refer to it.", "Delete Failed");
				return;
			}

			// No references but we should still confirm the delete
			if (MessageBox.Show("Are you certain that you want to delete this license type?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
                LicenseTypesDAO licensesTypesDAO = new LicenseTypesDAO();
                licensesTypesDAO.LicenseTypeDelete(deleteLicenseType);
			}

            this.RefreshView();
		}



		/// <summary>
		/// Double clicking on an item in the list box should select that item and then edit it
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lvLicenseTypes_ItemDoubleClick(object sender, Infragistics.Win.UltraWinListView.ItemDoubleClickEventArgs e)
		{
			// Add this item to the selected items collection
			UltraListViewItem clickedItem = e.Item;
			lvLicenseTypes.SelectedItems.Add(clickedItem);

			// ...then use the edit function
			bnEditLicenseType_Click(sender, e);
		}


		/// <summary>
		/// Called after the selection state of the list view has changed so that we can
		/// update the other controls on this form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lvLicenseTypes_ItemSelectionChanged(object sender, Infragistics.Win.UltraWinListView.ItemSelectionChangedEventArgs e)
		{
			// Set the button states based on what we have selected
			SetLicenseButtonStates();
		}


		/// <summary>
		/// Called to set the display state of the buttons on this form
		/// </summary>
		private void SetLicenseButtonStates()
		{
			// Delete and edit are only applicable if we have an item selected
			bnDelete.Enabled = (lvLicenseTypes.SelectedItems.Count != 0);
			bnEdit.Enabled = (lvLicenseTypes.SelectedItems.Count != 0);
		}
    }
}
