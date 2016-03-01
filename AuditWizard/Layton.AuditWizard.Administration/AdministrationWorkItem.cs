using System;
using System.Collections.Generic;
using System.Text;
using Layton.Cab.Interface;

namespace Layton.AuditWizard.Administration
{
    public class AdministrationWorkItem : Layton.Cab.Interface.LaytonWorkItem
    {
		private IAdministrationView _activeTabView = null;
		public IAdministrationView ActiveTabView
		{
			set { _activeTabView = value; }
		}
		
		protected override void OnActivated()
		{
			base.OnActivated();
			TabView.RefreshView();
			ExplorerView.RefreshView();
		}
    }
}
