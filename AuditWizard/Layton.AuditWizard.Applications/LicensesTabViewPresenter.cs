using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using System.Data;
using Infragistics.Win.UltraWinTree;
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Applications
{
    public class LicensesTabViewPresenter
    {
        private LicensesTabView tabView;
		private Object _licenseObject = null;

        [InjectionConstructor]
		public LicensesTabViewPresenter()
        {
        }

        public ILaytonView View
        {
			set { tabView = (LicensesTabView)value; }
        }

        public void Initialize()
        {
            InitializeTabView();
			ShowApplicationLicenses(_licenseObject);
        }


		/// <summary>
		/// Called to show the list of LICENSES for a specific APPLICATION
		/// </summary>
		public void ShowApplicationLicenses(Object licenseObject)
        {
			// Initialize the tab view
            InitializeTabView();

			// Reject NULL passed in values
			if (licenseObject == null)
				return;

			// Save the license object passed in to us
			_licenseObject = licenseObject;

			// Set the header and image for the tab view
			if (licenseObject is InstalledApplication)
			{
				// Update the internal data for this InstalledApplication as we are refreshing it
				InstalledApplication theApplication = (InstalledApplication)licenseObject;
				theApplication.LoadData();

				// ...then display it's attributes in the view
				tabView.HeaderText = theApplication.Name;
				tabView.HeaderImage = Properties.Resources.application_license_72;

				// Add the licenses to our tab view
				foreach (ApplicationLicense thisLicense in theApplication.Licenses)
				{
					tabView.AddApplicationLicense(thisLicense);
				}
			}

			else
			{
				// Update the internal data for this InstalledOS as we are refreshing it
				InstalledOS theOS = (InstalledOS)licenseObject;
				theOS.LoadData();

				// ...then display it's attributes in the view
				tabView.HeaderText = theOS.Name;
				tabView.HeaderImage = Properties.Resources.application_license_72;

				// Add the licenses to our tab view
				foreach (ApplicationLicense thisLicense in theOS.Licenses)
				{
					tabView.AddApplicationLicense(thisLicense);
				}
			}
		}

        private void InitializeTabView()
        {
            // clear the existing view
            tabView.Clear();
        }
    }
}
