using System;
using System.Drawing;
using System.Windows.Forms;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;

namespace Layton.NetworkDiscovery
{
    [SmartPart]
    public partial class NetworkDiscoveryTabView : UserControl, ILaytonView
    {
        private NetworkDiscoveryWorkItem workItem;
        private string statusText = "Initializing Network Discovery";
        private int discoveryCount = 0;
        private int computerCount = 0;
        private int networkDeviceCount = 0;
        private int printerCount = 0;
        private int progressCount = 0;
        private NetworkDiscoveryTabViewPresenter presenter;
        private string deviceType = "Computer";

        private int finalCounter = 0;
        private string foundCounter = "0";

        [Microsoft.Practices.ObjectBuilder.InjectionConstructor]
        public NetworkDiscoveryTabView([ServiceDependency] NetworkDiscoveryWorkItem workItem)
        {
            InitializeComponent();
            this.workItem = workItem;
        }

        [CreateNew]
        public NetworkDiscoveryTabViewPresenter Presenter
        {
            set
            {
                presenter = value;
                presenter.View = this;
                presenter.Initialize();
            }
        }

        public LaytonWorkItem WorkItem
        {
            get { return (LaytonWorkItem)workItem; }
        }

        public void RefreshView()
        {
            // Refresh the view as needed...nothing to refresh at the moment...
            base.Refresh();
        }

        public int DiscoveryCount
        {
            get { return discoveryCount; }
            set { discoveryCount = value; }
        }

        public int ProgressCount
        {
            get { return progressCount; }
            set { progressCount = value; }
        }

        public string StatusText
        {
            get { return statusText; }
            set { statusText = value; }
        }

        public string DeviceType
        {
            get { return deviceType; }
            set { deviceType = value; }
        }

        public string FoundCounter
        {
            get { return foundCounter; }
            set { foundCounter = value; }
        }

        private delegate void ShowStartDelegate();
        public void ShowStart()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowStartDelegate(ShowStart));
                return;
            }

            lblNetworkDeviceCount.Text = "0";
            lblPrinterCount.Text = "0";
            lblComputerCount.Text = "0";
            lbDiscoverdAssetCount.Text = "0";

            this.discoveryCount = 0;
            this.computerCount = 0;
            this.networkDeviceCount = 0;
            this.printerCount = 0;
            this.progressCount = 0;

            this.finalCounter = 0;
            this.foundCounter = "0";

            lbStatusText.ForeColor = Color.Red;
            this.lbStatusText.Text = "Performing Network Discovery...";

            this.pbDiscoveryProgress.Value = 0;

            lbDiscProg.Text = "Discovery Progress";
            lbDiscProg.ForeColor = Color.Black;

            this.lbDiscoverdAssetCount.Text = "0";

            Refresh();
        }

        private delegate void ShowUpdateDelegate();
        public void ShowUpdate()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowUpdateDelegate(ShowUpdate));
                return;
            }

            this.pbDiscoveryProgress.Maximum = discoveryCount;

            switch (deviceType)
            {
                case "Computer":
                    System.Threading.Interlocked.Decrement(ref finalCounter);
                    System.Threading.Interlocked.Increment(ref computerCount);
                    //System.Threading.Interlocked.Increment(ref foundCounter);

                    //this.lblComputerCount.Text = Convert.ToString(computerCount);
                    this.lblComputerCount.Text = foundCounter;                    

                    this.deviceType = "";
                    break;

                case "Printer":
                    System.Threading.Interlocked.Decrement(ref finalCounter);
                    System.Threading.Interlocked.Increment(ref printerCount);
                    //System.Threading.Interlocked.Increment(ref foundCounter);

                    this.lblPrinterCount.Text = Convert.ToString(printerCount);
                    this.deviceType = "";
                    break;

                case "Network Device":
                    System.Threading.Interlocked.Decrement(ref finalCounter);
                    System.Threading.Interlocked.Increment(ref networkDeviceCount);
                    //System.Threading.Interlocked.Increment(ref foundCounter);

                    this.lblNetworkDeviceCount.Text = Convert.ToString(networkDeviceCount);
                    this.deviceType = "";
                    break;

                default:
                    //System.Threading.Interlocked.Increment(ref staticFoundCounter);
                    break;
            }

            System.Threading.Interlocked.Increment(ref finalCounter);

            if (pbDiscoveryProgress.Value == this.pbDiscoveryProgress.Maximum)
            {
                this.lbStatusText.Text = "Finalising Network Discovery...";
                this.lbStatusText.ForeColor = Color.Blue;
            }
            else
            {
                this.pbDiscoveryProgress.Value = finalCounter;
            }

            lbDiscoverdAssetCount.Text = foundCounter;
            Refresh();
        }

        private delegate void ShowCompleteDelegate();
        public void ShowComplete()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowCompleteDelegate(ShowComplete));
                return;
            }

            this.pbDiscoveryProgress.Value = this.pbDiscoveryProgress.Maximum;

            this.lbStatusText.Text = "Completed Network Discovery";
            this.lbDiscProg.Text = "Completed Network Discovery";

            lbStatusText.ForeColor = Color.Green;
            lbDiscProg.ForeColor = Color.Green;
            Refresh();
            BringToFront();
        }
    }
}