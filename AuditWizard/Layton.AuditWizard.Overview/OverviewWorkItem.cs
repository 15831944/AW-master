using System;
using System.Collections.Generic;
using System.Text;
using Layton.Cab.Interface;

namespace Layton.AuditWizard.Overview
{
    public class OverviewWorkItem : Layton.Cab.Interface.LaytonWorkItem 
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            //TabView.RefreshView();
            //ExplorerView.RefreshView();
        }
    }
}
