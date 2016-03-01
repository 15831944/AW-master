using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
	public partial class FormLicenseType : ShadedImageForm
	{
		private LicenseType _licenseType;
		public LicenseType LicenseType
		{
			get { return _licenseType; }
		}

		public FormLicenseType(LicenseType theLicenseType)
		{
			InitializeComponent();
			_licenseType = theLicenseType;

			// If this entry is null or the InstanceID is 0 then we are creating a new license type
			if (_licenseType == null)
				_licenseType = new LicenseType();

			this.Text = (_licenseType.LicenseTypeID == 0) ? "New License Type" : "Edit License Type";
			tbName.Enabled = (_licenseType.LicenseTypeID == 0);
			tbName.Text = (_licenseType.LicenseTypeID == 0) ? "New License Type" : _licenseType.Name;

			// Set the check state for counted license
			cbPerPC.Checked = _licenseType.PerComputer;
		}


		/// <summary>
		/// Called as want to exit from this form potentially saving the definition or updating
		/// an existing entry
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
			if (tbName.Text == "")
			{
				MessageBox.Show("You must enter a name for this new license type");
				DialogResult = DialogResult.None;
				return;
			}

			// If we are creating a new license type then we must ensure that we have specified a 
			// name which does not already exist
			LicenseTypesDAO lwDataAccess = new LicenseTypesDAO();
			if (_licenseType.LicenseTypeID == 0)
			{
				int existingID = lwDataAccess.LicenseTypeFind(tbName.Text);
				if (existingID != 0)
				{
					MessageBox.Show("The License Type " + tbName.Text + " already exists.  Please enter a different name for this new license type" ,"Duplicate Name");
					DialogResult = DialogResult.None;
					return;
				}

				// OK all valid so add this new entry
				_licenseType.Name = tbName.Text;
				_licenseType.PerComputer = cbPerPC.Checked;
				//
				_licenseType.LicenseTypeID = lwDataAccess.LicenseTypeAdd(_licenseType);
			}

			else
			{
				// Update the existing definition
				_licenseType.PerComputer = cbPerPC.Checked;
				lwDataAccess.LicenseTypeUpdate(_licenseType);
			}
		}

	
	
	}
}