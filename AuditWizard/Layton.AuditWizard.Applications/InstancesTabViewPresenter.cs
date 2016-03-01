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
    public class InstancesTabViewPresenter
    {
        private InstancesTabView tabView;

        [InjectionConstructor]
		public InstancesTabViewPresenter()
        {
        }

        public ILaytonView View
        {
			set { tabView = (InstancesTabView)value; }
        }

        public void Initialize()
        {
            InitializeTabView();
			ShowApplicationInstances(null);
        }


		/// <summary>
		/// Called to show the list of installed instances of the specified application / OS
		/// </summary>
		/// <param name="publisherName"></param>
		public void ShowApplicationInstances(Object instanceObject)
        {
			// First initialize the tab view
            InitializeTabView();

			// Reject NULL passed in values
			if (instanceObject == null)
				return;

			// Set the header and image for the tab view
			if (instanceObject is InstalledApplication)
			{
				InstalledApplication theApplication = (InstalledApplication)instanceObject;
                theApplication.LoadData();

				tabView.HeaderText = theApplication.Name + " - Instances";
				tabView.HeaderImage = Properties.Resources.application_instance_72;

				// Add the instances to our dataset
				foreach (ApplicationInstance thisInstance in theApplication.Instances)
				{
					tabView.AddInstance(thisInstance);
				}
			}
			else
			{
				InstalledOS theOS = (InstalledOS)instanceObject;
                theOS.LoadData();

				tabView.HeaderText = theOS.Name;
				tabView.HeaderImage = Properties.Resources.application_license_72;

				// Add the licenses to our tab view
				foreach (ApplicationInstance thisInstance in theOS.Instances)
				{
					tabView.AddInstance(thisInstance);
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
