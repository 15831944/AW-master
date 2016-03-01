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
    public class EmailSettingsTabViewPresenter
    {
        private EmailSettingsTabView _tabView;

        [InjectionConstructor]
		public EmailSettingsTabViewPresenter()
        {
        }

        public ILaytonView View
        {
			set { _tabView = (EmailSettingsTabView)value; }
        }


		public void Initialize()
		{
			_tabView.RefreshView();
		}


        private void InitializeTabView()
        {
		}
    }
}
