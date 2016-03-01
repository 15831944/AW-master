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

namespace Layton.AuditWizard.Network
{
	public partial class FormFileProperties : ShadedImageForm
	{
		private FileSystemFile _displayedFile;
		
		public FormFileProperties(FileSystemFile displayedFile)
		{
			InitializeComponent();
			_displayedFile = displayedFile;
				
			// Set the initial display
			UpdateFields();						
		}


		private void FormFileproperties_Load(object sender, EventArgs e)
		{
			tbFileName.Text = _displayedFile.Name;
			tbDescription.Text = _displayedFile.Description;
			tbCreated.Text = _displayedFile.CreatedDateTime.ToString();
			tbFVersion1.Text = _displayedFile.FileVersion1;
			tbFVersion2.Text = _displayedFile.FileVersion2;
			tbLastAccessed.Text = _displayedFile.LastAccessedDateTime.ToString();
			tbModified.Text = _displayedFile.ModifiedDateTime.ToString();
			tbOriginalFileName.Text = _displayedFile.OriginalFileName;
			tbProductName.Text = _displayedFile.ProductName;
			tbPublisher.Text = _displayedFile.Publisher;
			tbPVersion1.Text = _displayedFile.ProductVersion1;
			tbPVersion2.Text = _displayedFile.ProductVersion2;
			tbSize.Text = _displayedFile.Size.ToString();
		}


		/// <summary>
		/// Called as we click to assign this file - we cannot assign directly from here as the user may want
		/// to change one or more of the fields
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnAssignFile_Click(object sender, EventArgs e)
		{
			FormFileAssign form = new FormFileAssign(_displayedFile);
			if (form.ShowDialog() == DialogResult.OK)
				UpdateFields();
		}


		/// <summary>
		/// Called to unassign this file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnUnassign_Click(object sender, EventArgs e)
		{
			_displayedFile.Assign(0);
			UpdateFields();
		}
		
		
		protected void UpdateFields ()
		{
			// Get the Assigned to Application name if required
			if (_displayedFile.AssignApplicationID != 0)
			{
				ApplicationsDAO lwDataAccess = new ApplicationsDAO();
				InstalledApplication application = lwDataAccess.GetApplication(_displayedFile.AssignApplicationID);
				if (application != null)
					tbAssignedApplication.Text = application.Name;

				// Hide the assign stuff then
				panelAssign.Enabled = false;
				bnUnassign.Enabled = true;
			}
			else
			{
				panelAssign.Enabled = true;
				bnUnassign.Enabled = false;
                tbAssignedApplication.Text = String.Empty;
			}
		}
	}
}