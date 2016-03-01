using System;
using System.Collections.Generic;
using System.Text;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;

namespace Layton.AuditWizard.Reports
{
    public class ReportsModuleInit : Layton.Cab.Interface.LaytonModuleInit
    {
        public ReportsModuleInit(WorkItem workItem) : base(workItem) {}
    }
}
