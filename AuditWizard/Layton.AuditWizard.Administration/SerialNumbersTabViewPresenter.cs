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
    public class SerialNumbersTabViewPresenter
    {
        private SerialNumbersTabView _tabView;

        [InjectionConstructor]
		public SerialNumbersTabViewPresenter()
        {
        }

        public ILaytonView View
        {
			set { _tabView = (SerialNumbersTabView)value; }
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
