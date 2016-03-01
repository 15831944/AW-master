using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;
//
using Infragistics.Win;

namespace Layton.AuditWizard.Common
{
	public partial class SupplierControl : UserControl
	{
		public Supplier Supplier
		{
			get { Supplier theSupplier = SaveSupplier(); return theSupplier; }
		}
			
		public SupplierControl()
		{
			InitializeComponent();
		}
		
		
		#region Supplier Functions

		/// <summary>
		/// Initialize the supplier tab
		/// </summary>
		public void LoadSuppliers(int supplierID ,string supplierName)
		{
			SuppliersDAO lwDataAccess = new SuppliersDAO();
			DataTable suppliersTable = lwDataAccess.EnumerateSuppliers();

            cbSuppliers.Items.Clear();

			// Add these to our combo box
			foreach (DataRow row in suppliersTable.Rows)
			{
				this.cbSuppliers.Items.Add(new Supplier(row));
			}

			// Select the 'current' supplier or the first if none
			int index = 0;
			if (supplierID > 1)
				index = cbSuppliers.FindStringExact(supplierName);
			if (index == -1)
				index = 0;
			cbSuppliers.SelectedIndex = index;
		}



		/// <summary>
		/// Called as we click New to create a new supplier
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnNewSupplier_Click(object sender, EventArgs e)
		{
			FormAskInput1 askSupplier = new FormAskInput1("Please enter the name of a new Supplier", "New Supplier", "Supplier:");
			if (askSupplier.ShowDialog() == DialogResult.OK)
			{
				// Ensure that this name is not already in our list and if not add and select it
				string newSupplierName = askSupplier.ValueEntered();
				int supplierIndex = cbSuppliers.FindStringExact(newSupplierName);
				if (supplierIndex != -1)
				{
					MessageBox.Show("This Supplier already exists and will be selected", "Supplier Already Exists");
					cbSuppliers.SelectedIndex = supplierIndex;
				}
				else
				{
					// This is a new supplier - Add to the list and select it
					Supplier newSupplier = new Supplier();
					newSupplier.Name = newSupplierName;
					ValueListItem newItem = cbSuppliers.Items.Add(newSupplier);
					cbSuppliers.SelectedItem = newItem;
				}
			}
		}


		/// <summary>
		/// Called as we change the selection in the Supplier combo.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbSuppliers_SelectionChanged(object sender, EventArgs e)
		{
			// Supplier Information - get the selected supplier
			Supplier theSupplier = cbSuppliers.SelectedItem.DataValue as Supplier;

			// If this is a new supplier then we need to allow the user to enter its details
			bool newSupplier = (theSupplier.SupplierID == 0);
			tbSupplierAddress1.ReadOnly = !newSupplier;
			tbSupplierAddress2.ReadOnly = !newSupplier;
			tbSupplierCity.ReadOnly = !newSupplier;
			tbSupplierState.ReadOnly = !newSupplier;
			tbSupplierZip.ReadOnly = !newSupplier;
			tbSupplierTelephone.ReadOnly = !newSupplier;
			tbSupplierContactName.ReadOnly = !newSupplier;
			tbSupplierContactEmail.ReadOnly = !newSupplier;
			tbSupplierFax.ReadOnly = !newSupplier;
			tbSupplierWWW.ReadOnly = !newSupplier;

			// Populate the supplier details
			tbSupplierAddress1.Text = theSupplier.AddressLine1;
			tbSupplierAddress2.Text = theSupplier.AddressLine2;
			tbSupplierCity.Text = theSupplier.City;
			tbSupplierState.Text = theSupplier.State;
			tbSupplierZip.Text = theSupplier.Zip;
			tbSupplierTelephone.Text = theSupplier.Telephone;
			tbSupplierContactName.Text = theSupplier.Contact;
			tbSupplierContactEmail.Text = theSupplier.Email;
			tbSupplierFax.Text = theSupplier.Fax;
			tbSupplierWWW.Text = theSupplier.WWW;
		}


		/// <summary>
		/// Save any changes made on the Support Contract tab
		/// </summary>
		/// <param name="updatedLicense"></param>
		private Supplier SaveSupplier()
		{
			Supplier theSupplier = cbSuppliers.SelectedItem.DataValue as Supplier;

			// If there is no supplier index this means that we are creating the supplier
			if (theSupplier.SupplierID == 0)
			{
				theSupplier.Name = cbSuppliers.Text;
				theSupplier.AddressLine1 = tbSupplierAddress1.Text;
				theSupplier.AddressLine2 = tbSupplierAddress2.Text;
				theSupplier.City = tbSupplierCity.Text;
				theSupplier.State = tbSupplierState.Text;
				theSupplier.Zip = tbSupplierZip.Text;
				theSupplier.Telephone = tbSupplierTelephone.Text;
				theSupplier.Contact = tbSupplierContactName.Text;
				theSupplier.Email = tbSupplierContactEmail.Text;
				theSupplier.Fax = tbSupplierFax.Text;
				theSupplier.WWW = tbSupplierWWW.Text;
				theSupplier.Add();
			}
			
			return theSupplier;
		}

		#endregion Supplier Functions

	}
}
