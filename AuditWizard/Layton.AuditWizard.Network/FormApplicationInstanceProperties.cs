using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Network
{
	public partial class FormApplicationInstanceProperties : ShadedImageForm
	{
		private ApplicationInstance _applicationInstance;

		/// <summary>
		/// Return the application license object
		/// </summary>
		public ApplicationInstance GetApplicationInstance
		{
			get { return _applicationInstance; }
		}


		public FormApplicationInstanceProperties(ApplicationInstance applicationInstance)
		{
			InitializeComponent();
			_applicationInstance = applicationInstance;

			// Populate the tabs
			InitializeGeneralTab();
			//
			InitializeNotesTab();
			//
			InitializeDocumentsTab();
		}

		protected void InitializeGeneralTab()
		{
			tbPublisher.Text = _applicationInstance.Publisher;
			tbName.Text = _applicationInstance.Name;
			tbVersion.Text = _applicationInstance.Version;
			tbInstalledOn.Text = _applicationInstance.InstalledOnComputer;

			// Serial number information
			if (_applicationInstance.Serial != null)
			{
				tbSerialNumber.Text = _applicationInstance.Serial.ProductId;
				tbCdKey.Text = _applicationInstance.Serial.CdKey;
			}
		}

		protected void InitializeNotesTab()
		{
			notesControl.LoadNotes(SCOPE.Application_Instance, _applicationInstance.InstanceID);
		}


		protected void InitializeDocumentsTab()
		{
			documentsControl.LoadDocuments(SCOPE.Application_Instance, _applicationInstance.InstanceID);
		}

		#region General Tab Functions

		/// <summary>
		/// Called when we click on the button to select a different publisher
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnSetPublisher_Click(object sender, EventArgs e)
		{
			FormSelectPublisher selectPublisher = new FormSelectPublisher(true);
			if (selectPublisher.ShowDialog() == DialogResult.OK)
				tbPublisher.Text = selectPublisher.SelectedPublisher;
		}

		#endregion General Tab Functions
		

		/// <summary>
		/// Called as we click the OK button - save the definition back to the database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bnOK_Click(object sender, EventArgs e)
		{
            ApplicationsDAO lwDataAccess = new ApplicationsDAO();

			// We need to update the application instance with any changes that the user has made
			ApplicationInstance updatedInstance = new ApplicationInstance(_applicationInstance);

			// Update the publisher first if that has changed
			if (_applicationInstance.Publisher != tbPublisher.Text)
			{
				lwDataAccess.ApplicationUpdatePublisher(_applicationInstance.ApplicationID, tbPublisher.Text);
				_applicationInstance.Publisher = tbPublisher.Text;
			}

			// Update serial number /CD key if specified
			if (updatedInstance.Serial == null)
				updatedInstance.Serial = new ApplicationSerial();
			if (tbSerialNumber.Text != "" || tbCdKey.Text != null)
			{
				updatedInstance.Serial.ProductId = tbSerialNumber.Text;
				updatedInstance.Serial.CdKey = tbCdKey.Text;
			}

			// ...and update the database if the object has changed
			if (updatedInstance != _applicationInstance)
			{
				new ApplicationInstanceDAO().ApplicationInstanceUpdate(updatedInstance);

				// We need to log any changes made to this definition in the audit trail
				List<AuditTrailEntry> listChanges = updatedInstance.ListChanges(_applicationInstance);
				foreach (AuditTrailEntry thisChange in listChanges)
				{
					new AuditTrailDAO().AuditTrailAdd(thisChange);
				}		
			}
		}
	}
}