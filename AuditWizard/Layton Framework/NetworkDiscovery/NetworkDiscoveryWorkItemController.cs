using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.Utility;
using Layton.AuditWizard.DataAccess;
using Layton.AuditWizard.Common;

namespace Layton.NetworkDiscovery
{
    public class NetworkDiscoveryWorkItemController : LaytonWorkItemController
    {
        NetworkDiscoveryWorkItem workItem;
        Thread discoveryThread = null;
        NetworkDiscovery discoverer = null;

        private bool runBothDiscovery = false;
        private bool changeView = true;

        public bool RunBothDiscovery
        {
            get { return runBothDiscovery; }
            set { runBothDiscovery = value; }
        }

        public bool ChangeView
        {
            get { return changeView; }
            set { changeView = value; }
        }

        [Microsoft.Practices.ObjectBuilder.InjectionConstructor]
        public NetworkDiscoveryWorkItemController([ServiceDependency] WorkItem workItem)
            : base(workItem)
        {
            this.workItem = workItem as NetworkDiscoveryWorkItem;
            ThreadStart threadDelegate = new ThreadStart(RunThreadedNetworkDiscovery);
            discoveryThread = new Thread(threadDelegate);
        }

        /// <summary>
        /// Event declaration for when a new Network Discovery has started.
        /// </summary>
        [EventPublication(EventTopics.NetworkDiscoveryStarted, PublicationScope.WorkItem)]
        public event EventHandler NetworkDiscoveryStarted;

        /// <summary>
        /// Event declaration for when the Network discovery operation has completed.
        /// </summary>
        [EventPublication(EventTopics.NetworkDiscoveryComplete, PublicationScope.Global)]
        public event EventHandler<DataEventArgs<List<string[]>>> NetworkDiscoveryComplete;

        protected virtual void FireNetworkDiscoveryComplete(List<string[]> e)
        {
            EventHandler<DataEventArgs<List<string[]>>> temp = NetworkDiscoveryComplete;
            if (temp != null)
                temp(this, new DataEventArgs<List<string[]>>(e));
        }

        protected virtual void FireNetworkDiscoveryStarted()
        {
            EventHandler temp = NetworkDiscoveryStarted;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        public void RunAdNetworkDiscovery()
        {
            if (discoverer == null || discoverer.IsDiscoverComplete)
            {
                // prepare to start the LDAP Discovery
                LdapNetworkDiscovery ldapDiscovery = new LdapNetworkDiscovery();
                //ActiveDirectoryNetworkDiscovery ldapDiscovery = new ActiveDirectoryNetworkDiscovery();
                discoverer = ldapDiscovery;
                discoverer.NetworkDiscoveryUpdate += new EventHandler<DiscoveryUpdateEventArgs>(discoverer_NetworkDiscoveryUpdate);
                NetworkDiscoveryTabView tabView = workItem.Items[Properties.Settings.Default.NetworkDiscoveryTabView] as NetworkDiscoveryTabView;
                base.SetTabView(tabView);

                // Start the discovery
                tabView.ShowStart();
                FireNetworkDiscoveryStarted();
                discoverer.Start();

                // LDAP discovery has completed
                tabView.ShowComplete();
                FireNetworkDiscoveryComplete(discoverer.ComputerList);
            }
            else
            {
                MessageBox.Show("There is another Network Discovery in progress. Please wait until Network Discovery is complete before starting another.", "Network Discovery", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void RunNetbiosNetworkDiscovery()
        {
            if (discoverer == null || discoverer.IsDiscoverComplete)
            {
                NetbiosNetworkDiscovery netbiosDiscovery = new NetbiosNetworkDiscovery();
                FireNetworkDiscoveryStarted();
                BeginThreadedNetworkDiscovery(netbiosDiscovery);
            }
            else
            {
                MessageBox.Show("There is another Network Discovery in progress. Please wait until Network Discovery is complete before starting another.", "Network Discovery", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void RunTcpipNetworkDiscovery()
        {
            if (discoverer == null || discoverer.IsDiscoverComplete)
            {
                TcpipNetworkDiscovery tcpipDiscovery = new TcpipNetworkDiscovery(Utility.GetComputerIpRanges());
                FireNetworkDiscoveryStarted();                
                BeginThreadedNetworkDiscovery(tcpipDiscovery);
            }
            else
            {
                MessageBox.Show("There is another Network Discovery in progress. Please wait until Network Discovery is complete before starting another.", "Network Discovery", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void RunSNMPNetworkDiscovery()
        {
            if (discoverer == null || discoverer.IsDiscoverComplete)
            {
                SNMPDiscovery snmpDiscovery = new SNMPDiscovery(Utility.GetSNMPIpRanges());
                FireNetworkDiscoveryStarted();
                BeginThreadedNetworkDiscovery(snmpDiscovery);
            }
            else
            {
                MessageBox.Show("There is another Network Discovery in progress. Please wait until Network Discovery is complete before starting another.", "Network Discovery", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BeginThreadedNetworkDiscovery(NetworkDiscovery instance)
        {
            if (discoveryThread.ThreadState == ThreadState.Running) return;

            discoverer = null;
            discoverer = instance;
            discoverer.NetworkDiscoveryUpdate += new EventHandler<DiscoveryUpdateEventArgs>(discoverer_NetworkDiscoveryUpdate);
            if (instance.CanRunInOwnThread)
            {
                if (changeView)
                {
                    NetworkDiscoveryTabView tabView = workItem.Items[Properties.Settings.Default.NetworkDiscoveryTabView] as NetworkDiscoveryTabView;
                    if (tabView != null) base.SetTabView(tabView);
                }

                discoveryThread.Start();
            }
        }

        private void RunThreadedNetworkDiscovery()
        {
            if (discoverer.CanRunInOwnThread)
            {
                // start the discovery
                NetworkDiscoveryTabView tabView = workItem.Items[Properties.Settings.Default.NetworkDiscoveryTabView] as NetworkDiscoveryTabView;
                if (tabView != null) tabView.ShowStart();
                discoverer.Start();

                if (runBothDiscovery)
                {
                    SNMPDiscovery snmpDiscovery = new SNMPDiscovery(Utility.GetSNMPIpRanges());
                    discoverer = snmpDiscovery;

                    //tabView.ShowStart();
                    discoverer.Start();
                }

                // discover has completed...update subscribers and TabView
                if (tabView != null) 
					tabView.ShowComplete();

				// +8.3.3
				// Did we complete an SNMP Discovery?
				if (discoverer is SNMPDiscovery)
				{
					// Yes - we need to check whether or not the current scanner is configured to upload audits to an FTP
					// site as in this case we need to create audit files for any assets discovered
					AuditScannerDefinition auditScannerDefinition = null;
					try
					{
						string scannerPath = Path.Combine(Application.StartupPath, "scanners") + "\\default.xml";
						auditScannerDefinition = AuditWizardSerialization.DeserializeObject(scannerPath);
					}
					catch (Exception)
					{
					}

					// 8.3.4 - CMD
					//
					// If we found a scanner and it defines an FTP upload of audit files then process this
					// Note that we support either Upload to FTP Location or Audit to FTP which are flagged differently in the scanner
					if ((auditScannerDefinition != null)
					&&  (auditScannerDefinition.FTPCopyToNetwork || auditScannerDefinition.UploadSetting == AuditScannerDefinition.eUploadSetting.ftp))
					{
						((SNMPDiscovery)discoverer).UploadDiscoveredAssets(auditScannerDefinition);
					}
				}

				// -8.3.3
                FireNetworkDiscoveryComplete(discoverer.ComputerList);
                discoveryThread = new Thread(new ThreadStart(RunThreadedNetworkDiscovery));
                discoverer.NetworkDiscoveryUpdate -= discoverer_NetworkDiscoveryUpdate;
                discoverer = null;
            }
        }

        void discoverer_NetworkDiscoveryUpdate(object sender, DiscoveryUpdateEventArgs e)
        {
            lock (this)
            {
                NetworkDiscoveryTabView tabView = workItem.Items[Properties.Settings.Default.NetworkDiscoveryTabView] as NetworkDiscoveryTabView;
                string statusText = "Discovering domain, " + e.Domain + "...";
                if (!e.IsDomainOnly)
                {
                    statusText = "Discovered Asset, " + e.Domain + "\\" + e.Computer;
                }

                tabView.DeviceType = e.Domain;
                tabView.StatusText = statusText;
                tabView.DiscoveryCount = e.DiscoveredComputerCount;
                tabView.ProgressCount = e.ProgressCount;
                tabView.FoundCounter = e.Computer;

                tabView.ShowUpdate();
            }

        }
	}
}
