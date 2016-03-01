using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;

namespace Layton.NetworkDiscovery
{
    public class NetworkDiscoveryExplorerViewPresenter
    {
        private NetworkDiscoveryExplorerView explorerView;

        [InjectionConstructor]
        public NetworkDiscoveryExplorerViewPresenter()
        {
        }

        public NetworkDiscoveryExplorerView View
        {
            set { explorerView = (NetworkDiscoveryExplorerView)value; }
        }

        public void Initialize()
        {
            // Initialize anything here...nothing for now
        }
    }
}