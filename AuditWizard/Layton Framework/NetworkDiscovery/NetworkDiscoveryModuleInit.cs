using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;

namespace Layton.NetworkDiscovery
{
    /// <summary>
    /// This is the initializationm class for the NetworkDiscovery module.  <see cref="LaytonModuleInit"/>
    /// for implementation.
    /// </summary>
    public class NetworkDiscoveryModuleInit : Layton.Cab.Interface.LaytonModuleInit
    {
        [InjectionConstructor]
        public NetworkDiscoveryModuleInit([ServiceDependency] WorkItem workItem) : base(workItem)
        {

        }
    }
}