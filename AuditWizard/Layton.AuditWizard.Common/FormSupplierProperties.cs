using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
	public partial class FormSupplierProperties : ShadedImageForm
	{
		private Supplier _supplier;

		public FormSupplierProperties(Supplier theSupplier)
		{
			InitializeComponent();

			// Save the supplied object for later
			_supplier = theSupplier;

			// Determine if we are editing or creating a new supplier
			if (theSupplier.SupplierID == 0)
			{
				this.Text = "New Supplier";
				this.footerPictureBox.Image = Properties.Resources.supplier_add_corner;
			}
			else
			{
				this.Text = "Edit Supplier";
				this.footerPictureBox.Image = Properties.Resources.supplier_edit_corner;
			}

			// Populate the supplier details
			tbSupplierName.Text = theSupplier.Name;
			tbSupplierAddress1.Text = theSupplier.AddressLine1;
			tbSupplierAddress2.Text = theSupplier.AddressLine2;
			tbSupplierCity.Text = theSupplier.City;
			tbSupplierState.Text = theSupplier.State;
			tbSupplierZip.Text = theSupplier.Zip;
			tbSupplierTelephone.Text = theSupplier.Telephone;
			tbSupplierContactName.Text = theSupplier.Contact;
			tbSupplierContactEmail.Text = theSupplier.Email;
			tbSupplierWWW.Text = theSupplier.WWW;
			tbSupplierFax.Text = theSupplier.Fax;
			tbSupplierNotes.Text = theSupplier.Notes;

			// Set the link types for the Email and Web Site fields
			tbSupplierContactEmail.LinkType = Layton.Common.Controls.LinkTypes.Email;
			tbSupplierWWW.LinkType = Layton.Common.Controls.LinkTypes.Http;
		}

		/// <summary>
		/// Called when we click OK to save the definition back to the database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			// Update our supplier definition
			_supplier.Name = tbSupplierName.Text;
			_supplier.AddressLine1 = tbSupplierAddress1.Text;
			_supplier.AddressLine2 = tbSupplierAddress2.Text;
			_supplier.City = tbSupplierCity.Text;
			_supplier.State = tbSupplierState.Text;
			_supplier.Zip = tbSupplierZip.Text;
			_supplier.Telephone = tbSupplierTelephone.Text;
			_supplier.Contact = tbSupplierContactName.Text;
			_supplier.Email = tbSupplierContactEmail.Text;
			_supplier.WWW = tbSupplierWWW.Text;
			_supplier.Fax = tbSupplierFax.Text;
			_supplier.Notes = tbSupplierNotes.Text;

			// Look for an existing supplier with the same name as that specified
            SuppliersDAO lwDataAccess = new SuppliersDAO();
			int existingID = lwDataAccess.SupplierFind(_supplier.Name);

			// If we have found a match, ensure that the ID does not match that stored as if it does then
			// this indicates that the Supplier already exists with a different ID which we cannot allow
			if ((existingID != 0) && (existingID != _supplier.SupplierID))
			{
				MessageBox.Show("There is already a Supplier with this name.  Please enter a different name for the new supplier" ,"Duplicate Name");
				DialogResult = DialogResult.None;
				return;
			}

			// Ok this is a valid entry so we need to either add it or update an existing entry
			_supplier.Update(_supplier);
		}
	}
}