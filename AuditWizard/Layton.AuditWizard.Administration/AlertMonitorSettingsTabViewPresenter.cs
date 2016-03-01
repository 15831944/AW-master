using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using Layton.AuditWizard.Common;
using System.Data;
using Infragistics.Win.UltraWinTree;
using Layton.Cab.Interface;

namespace Layton.AuditWizard.Administration
{
    public class AlertMonitorSettingsTabViewPresenter
    {
        private AlertMonitorSettingsTabView _tabView;

        [InjectionConstructor]
		public AlertMonitorSettingsTabViewPresenter()
        {
        }

        public ILaytonView View
        {
			set { _tabView = (AlertMonitorSettingsTabView)value; }
        }


		public void Initialize()
		{
			_tabView.RefreshView();
		}


        private void InitializeTabView()
        {
			_tabView.RefreshView();
		}
    }
}
