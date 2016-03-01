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
    public class ScannerConfigurationTabViewPresenter
    {
        private DatabaseTabView _tabView;

        [InjectionConstructor]
		public ScannerConfigurationTabViewPresenter()
        {
        }

        public ILaytonView View
        {
			set { _tabView = (DatabaseTabView)value; }
        }


		public void Initialize()
		{
			_tabView.RefreshView();
		}


        private void InitializeTabView()
        {
		}

		/// <summary>
		/// Close the view noting that we should ask the view to confirm whether or not it should be saved
		/// </summary>
		public void OnCloseView()
		{
			_tabView.Save();
		}

    }
}
