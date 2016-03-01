using System;
using System.Collections.Generic;
using System.Text;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;

namespace Layton.AuditWizard.Network
{
	public class NetworkModuleInit : Layton.Cab.Interface.LaytonModuleInit
	{
		public NetworkModuleInit(WorkItem workItem) : base(workItem) { }
	}
}
