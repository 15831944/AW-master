using System;
using System.Collections.Generic;
using System.Text;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;

namespace Layton.AuditWizard.Overview
{
    public class OverviewModuleInit : Layton.Cab.Interface.LaytonModuleInit
    {
        public OverviewModuleInit(WorkItem workItem) : base(workItem) { }
    }
}
