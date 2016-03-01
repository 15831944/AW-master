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
    public class OSTabViewPresenter
    {
        private OSTabView _tabView;
        private InstalledOS _currentOS;
        private bool _isAllOSDisplayed = false;

        [InjectionConstructor]
		public OSTabViewPresenter()
        {
        }

        public ILaytonView View
        {
			set { _tabView = (OSTabView)value; }
        }

		public bool IsAllOSDisplayed
		{
			get { return _isAllOSDisplayed; }
		}

        public void Initialize()
        {
            if (_isAllOSDisplayed)
				ShowOS(null);
            else
				ShowOS(_currentOS);
		}


		/// <summary>
		/// Called to show the list of Operating Systems (or details of a specific one)
		/// </summary>
		/// <param name="forPublisher"></param>
		public void ShowOS(InstalledOS forOS)
        {
			// Get the work item controller
			ApplicationsWorkItemController wiController = _tabView.WorkItem.Controller as ApplicationsWorkItemController;

			// ...and from there settings which alter what we display in this view
			bool showHidden = wiController.ShowIgnoredApplications;

			// Are we displaying all OS's or a specific one as we need to save this state
			_isAllOSDisplayed = (forOS == null);

			// clear the existing view
			_tabView.Clear();

			// Set the header text and image for the tab view based on whether we are displaying
			// all (possibly filtered) publishers or a sepcific publisher
			_tabView.HeaderText = (forOS == null) ? MiscStrings.OperatingSystems : forOS.Name;
			_tabView.HeaderImage = Properties.Resources.os_96;

			// If we have not been supplied a specific OS to display then flag that we are not displaying
			// an OS at this time but save the supplied OS regardless
			if (_isAllOSDisplayed)
			{
				// Displaying all Operating Systems

				// Call database function to return list of Operating Systems
				ApplicationsDAO lwDataAccess = new ApplicationsDAO();
				DataTable OSTable = lwDataAccess.GetOperatingSystems();

				// ...and add these to the tab view
				foreach (DataRow row in OSTable.Rows)
				{
					InstalledOS thisOS = new InstalledOS(row);

					// Read instances/licenses of this OS
					thisOS.LoadData();

                    if (thisOS.Instances.Count == 0)
                        continue;

					// ...and add to the tab view
					_tabView.AddOS(thisOS);
				}			
			}

			else
			{
				// Displaying a specific OS
				_currentOS = forOS;
				_tabView.AddOS(_currentOS);
			}
        }
    }
}
