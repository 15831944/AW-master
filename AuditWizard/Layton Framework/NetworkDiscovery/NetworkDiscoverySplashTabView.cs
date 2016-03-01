using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;

namespace Layton.NetworkDiscovery
{
    [SmartPart]
    public partial class NetworkDiscoverySplashTabView : UserControl, ILaytonView
    {
        private NetworkDiscoveryWorkItem workItem;
        
        [Microsoft.Practices.ObjectBuilder.InjectionConstructor]
        public NetworkDiscoverySplashTabView([ServiceDependency] NetworkDiscoveryWorkItem workItem)
        {
            InitializeComponent();
            this.workItem = workItem;
        }

        public void RefreshView()
        {
            base.Refresh();
        }

        public LaytonWorkItem WorkItem
        {
            get { return (LaytonWorkItem)workItem; }
        }
    }
}
