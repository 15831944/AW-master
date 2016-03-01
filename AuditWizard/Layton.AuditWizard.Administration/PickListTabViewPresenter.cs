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
    public class PickListTabViewPresenter
    {
        private PickListTabView _tabView;

        [InjectionConstructor]
		public PickListTabViewPresenter()
        {
        }

        public ILaytonView View
        {
			set { _tabView = (PickListTabView)value; }
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
