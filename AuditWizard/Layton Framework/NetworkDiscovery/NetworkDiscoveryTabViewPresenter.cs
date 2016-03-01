using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;

namespace Layton.NetworkDiscovery
{
    public class NetworkDiscoveryTabViewPresenter
    {
        private NetworkDiscoveryTabView tabView;

        [InjectionConstructor]
        public NetworkDiscoveryTabViewPresenter()
        {
        }

        public NetworkDiscoveryTabView View
        {
            set { tabView = (NetworkDiscoveryTabView)value; }
        }

        public void Initialize()
        {
            // TO DO:
        }
    }
}
