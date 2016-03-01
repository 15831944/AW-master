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
    public class AssetTypesTabViewPresenter
    {
        private SerialNumbersTabView _tabView;

        [InjectionConstructor]
		public AssetTypesTabViewPresenter()
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
