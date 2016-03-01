using System;
using System.Collections.Generic;
using System.Text;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;

namespace Layton.AuditWizard.Applications
{
    public class ApplicationsInit : Layton.Cab.Interface.LaytonModuleInit
    {
        public ApplicationsInit(WorkItem workItem) : base(workItem) {}
    }
}
