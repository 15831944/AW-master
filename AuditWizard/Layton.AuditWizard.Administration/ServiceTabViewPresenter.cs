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
    public class ServiceTabViewPresenter
    {
        private ServiceTabView _tabView;

        [InjectionConstructor]
		public ServiceTabViewPresenter()
        {
        }

        public ILaytonView View
        {
			set { _tabView = (ServiceTabView)value; }
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
