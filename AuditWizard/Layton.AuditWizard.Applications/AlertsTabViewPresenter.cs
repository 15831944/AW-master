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
    public class AlertsTabViewPresenter
    {
        private AlertsTabView _tabView;

        [InjectionConstructor]
		public AlertsTabViewPresenter()
        {
        }

        public ILaytonView View
        {
			set { _tabView = (AlertsTabView)value; }
        }

        public void Initialize()
        {
			ShowAlerts();
		}


		/// <summary>
		/// Called to show the list of Actions
		/// </summary>
		public void ShowAlerts()
        {
			// Get the work item controller
			ApplicationsWorkItemController wiController = _tabView.WorkItem.Controller as ApplicationsWorkItemController;

			// clear the existing view
			_tabView.Clear();

			// Call database function to return list of Alerts
			DateTime dtYearAgo = DateTime.Now.AddYears(-1);
			AlertsDAO lwDataAccess = new AlertsDAO();
			DataTable alertsTable = lwDataAccess.EnumerateAlerts(dtYearAgo);

			// ...and add these to the tab view
			foreach (DataRow row in alertsTable.Rows)
			{
				Alert alert = new Alert(row);

				// ...and add to the tab view
				_tabView.AddAlert(alert);
			}
        }
    }
}
