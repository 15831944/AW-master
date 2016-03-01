using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win.UltraWinGrid;
//
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Utility;
using Microsoft.Practices.ObjectBuilder;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Reports
{
    public class ReportsTabViewPresenter
    {
        private ReportsTabView tabView;

        [InjectionConstructor]
        public ReportsTabViewPresenter()
        {
        }

        public ILaytonView View
        {
            set { tabView = (ReportsTabView) value; }
        }


		/// <summary>
		/// Initialization function for this tab view
		/// </summary>
        public void Initialize()
        {
			ReportsExplorerView explorerView = tabView.WorkItem.ExplorerView as ReportsExplorerView;
			explorerView.RefreshView();
        }


		/// <summary>
		/// Initialize the Computers Tab View - this simply clears the view
		/// </summary>
		private void InitializeTabView()
		{
		}
    }
}