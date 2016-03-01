using System;
using System.Collections.Generic;
using System.Text;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;

namespace Layton.AuditWizard.Administration
{
    public class AdministrationModuleInit : Layton.Cab.Interface.LaytonModuleInit
    {
        public AdministrationModuleInit(WorkItem workItem) : base(workItem) {}
    }
}
