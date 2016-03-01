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
    public class UploadSettingsTabViewPresenter
    {
        private UploadSettingsTabView _tabView;

        [InjectionConstructor]
		public UploadSettingsTabViewPresenter()
        {
        }

        public ILaytonView View
        {
			set { _tabView = (UploadSettingsTabView)value; }
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
