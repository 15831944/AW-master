using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using Layton.Cab.Interface;

namespace Layton.NetworkDiscovery
{
    [SmartPart]
    public partial class NetworkDiscoveryExplorerView : UserControl, ILaytonView
    {
        private WorkItem workItem;
        private NetworkDiscoveryExplorerViewPresenter presenter;

        [InjectionConstructor]
        public NetworkDiscoveryExplorerView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem;
            InitializeComponent();
            this.Paint += new PaintEventHandler(NetworkDiscoveryExplorerView_Paint);
            this.SizeChanged += new EventHandler(NetworkDiscoveryExplorerView_SizeChanged);
        }

        [CreateNew]
        public NetworkDiscoveryExplorerViewPresenter Presenter
        {
            set
            {
                presenter = value;
                presenter.View = this;
                presenter.Initialize();
            }
        }

        public void RefreshView()
        {
            base.Refresh();
        }

        public LaytonWorkItem WorkItem
        {
            get { return (LaytonWorkItem)workItem; }
        }

        private void NetworkDiscoveryExplorerView_Paint(object sender, PaintEventArgs e)
        {
            Image bgImg = Properties.Resources.ghosted_network_discovery;
            e.Graphics.DrawImage(bgImg, (this.Width - bgImg.Width), (this.Height - bgImg.Height));
        }

        void NetworkDiscoveryExplorerView_SizeChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void discoverButton_Click(object sender, EventArgs e)
        {
            NetworkDiscoveryWorkItemController controller = WorkItem.Controller as NetworkDiscoveryWorkItemController;
            if (adRadioButton.Checked)
            {
                controller.RunBothDiscovery = false;
                controller.RunAdNetworkDiscovery();
            }
            else if (netbiosRadioButton.Checked)
            {
                controller.RunBothDiscovery = false;
                controller.RunNetbiosNetworkDiscovery();
            }
            else if (tcpipRadioButton.Checked)
            {
                controller.RunBothDiscovery = false;
                controller.RunTcpipNetworkDiscovery();
            }
            else if (snmpRadioButton.Checked)
            {
                controller.RunBothDiscovery = false;
                controller.RunSNMPNetworkDiscovery();
            }
        }
    }
}