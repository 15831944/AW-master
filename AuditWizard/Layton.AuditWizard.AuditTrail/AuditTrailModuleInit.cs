using System;
using System.Collections.Generic;
using System.Text;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;

namespace Layton.AuditWizard.AuditTrail
{
    public class AuditTrailModuleInit : Layton.Cab.Interface.LaytonModuleInit
    {
        public AuditTrailModuleInit(WorkItem workItem) : base(workItem) {}
    }
}
