using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.Utility;

namespace Layton.NetworkDiscovery
{
    abstract public class NetworkDiscovery
    {
        public abstract bool HasDiscoverStarted { get; }
        public abstract bool IsDiscoverComplete { get; }
        public abstract List<string> DomainList { get; }
        public abstract List<string[]> ComputerList { get; }
        public abstract void Start();
        public virtual bool CanRunInOwnThread { get { return true; } }

        /// <summary>
        /// Event declaration for when the Network discovery update is available.
        /// </summary>
        [EventPublication(EventTopics.NetworkDiscoveryUpdate, PublicationScope.Global)]
        public event EventHandler<DiscoveryUpdateEventArgs> NetworkDiscoveryUpdate;

        protected virtual void FireNetworkDiscoveryUpdate(DiscoveryUpdateEventArgs e)
        {
            EventHandler<DiscoveryUpdateEventArgs> temp = NetworkDiscoveryUpdate;
            if (temp != null)
                temp(this, e);
        }
    }
}
