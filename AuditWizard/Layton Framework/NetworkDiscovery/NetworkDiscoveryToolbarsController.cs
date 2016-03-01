using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using System.Drawing;
using System.Threading;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.SmartParts;
using Infragistics.Practices.CompositeUI.WinForms;
using Layton.Cab.Interface;

namespace Layton.NetworkDiscovery
{
    public class NetworkDiscoveryToolbarsController : LaytonToolbarsController
    {
        private NetworkDiscoveryWorkItem workItem;
        private RibbonTab ribbonTab;
        private RibbonGroup adRibbonGroup;
        private RibbonGroup tcpipRibbonGroup;
        private RibbonGroup netbiosRibbonGroup;
        private RibbonGroup snmpRibbonGroup;
        private RibbonGroup autoScanRibbonGroup;

        [Microsoft.Practices.ObjectBuilder.InjectionConstructor]
        public NetworkDiscoveryToolbarsController([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as NetworkDiscoveryWorkItem;
        }

        public override RibbonTab RibbonTab
        {
            get { return ribbonTab; }
        }

        public override void UpdateTools()
        {
            // TO DO...
        }

        public override void Initialize()
        {
            // Now create a Ribbon Tab and add it to the ribbonTabs collection
            ribbonTab = new RibbonTab(RibbonNames.tabName, RibbonNames.tabName);
            workItem.RootWorkItem.UIExtensionSites["ribbonTabs"].Add<RibbonTab>(ribbonTab);

            // Set the Tag property to the WorkItem
            // this will allow the Shell to activate the WorkItem given the RibbonTab
            ribbonTab.Tag = workItem;

            // Now register the ribbon tab's Groups collection
            workItem.RootWorkItem.UIExtensionSites.RegisterSite("discoverRibbonGroups", ribbonTab.Groups);

            adRibbonGroup = new RibbonGroup(RibbonNames.adGroupName);
            adRibbonGroup.Caption = RibbonNames.adGroupName;
            adRibbonGroup.PreferredToolSize = RibbonToolSize.Large;

            workItem.UIExtensionSites["discoverRibbonGroups"].Add<RibbonGroup>(adRibbonGroup);

            // Now register the Active Directory group Tools collection
            workItem.RootWorkItem.UIExtensionSites.RegisterSite("adRibbonTools", adRibbonGroup.Tools);
            InitializeADTools();

            netbiosRibbonGroup = new RibbonGroup(RibbonNames.netbiosGroupName);
            netbiosRibbonGroup.Caption = RibbonNames.netbiosGroupName;
            netbiosRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
            workItem.UIExtensionSites["discoverRibbonGroups"].Add<RibbonGroup>(netbiosRibbonGroup);

            // Now register the NetBIOS group Tools collection
            workItem.RootWorkItem.UIExtensionSites.RegisterSite("netbiosRibbonTools", netbiosRibbonGroup.Tools);
            InitializeNetbiosTools();

            tcpipRibbonGroup = new RibbonGroup(RibbonNames.tcpipGroupName);
            tcpipRibbonGroup.Caption = RibbonNames.tcpipGroupName;
            tcpipRibbonGroup.PreferredToolSize = RibbonToolSize.Large;
            workItem.UIExtensionSites["discoverRibbonGroups"].Add<RibbonGroup>(tcpipRibbonGroup);

            // Now register the TCP/IP group Tools collection
            workItem.RootWorkItem.UIExtensionSites.RegisterSite("tcpipRibbonTools", tcpipRibbonGroup.Tools);
            InitializeTcpipTools();

            snmpRibbonGroup = new RibbonGroup("SNMP");
            snmpRibbonGroup.Caption = "SNMP";
            snmpRibbonGroup.PreferredToolSize = RibbonToolSize.Large;

            workItem.UIExtensionSites["discoverRibbonGroups"].Add<RibbonGroup>(snmpRibbonGroup);

            // Now register the Active Directory group Tools collection
            workItem.RootWorkItem.UIExtensionSites.RegisterSite("snmpRibbonTools", snmpRibbonGroup.Tools);
            InitializeSNMPTools();

            autoScanRibbonGroup = new RibbonGroup("Auto-Discovery");
            autoScanRibbonGroup.Caption = "Auto-Discovery";
            autoScanRibbonGroup.PreferredToolSize = RibbonToolSize.Large;

            workItem.UIExtensionSites["discoverRibbonGroups"].Add<RibbonGroup>(autoScanRibbonGroup);

            // Now register the Active Directory group Tools collection
            workItem.RootWorkItem.UIExtensionSites.RegisterSite("autoScanRibbonTools", autoScanRibbonGroup.Tools);
            InitializeAutoScanTools();
        }

        private void InitializeADTools()
        {
            ButtonTool tool = new ButtonTool(ToolNames.adImport);
            tool.SharedProps.Caption = ToolNames.adImport;
            tool.SharedProps.ToolTipText = "Add computers by selecting them from your directory";

            Image bgImage = Properties.Resources.activedirectory_netdisc32;
            tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            workItem.UIExtensionSites["adRibbonTools"].Add<ButtonTool>(tool);
            tool.ToolClick += new ToolClickEventHandler(discovery_ToolClick);

            // Add the "Settings" button
            ButtonTool settingsTool = new ButtonTool("Active Directory Advanced Settings");
            settingsTool.SharedProps.Caption = "Active Directory Advanced Settings";
            settingsTool.SharedProps.ToolTipText = "Modify the Active Directory Discovery settings";

            settingsTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.settings32;
            workItem.UIExtensionSites["adRibbonTools"].Add<ButtonTool>(settingsTool);
            settingsTool.ToolClick += new ToolClickEventHandler(discovery_ToolClick);
        }
        
        private void InitializeNetbiosTools()
        {
            ButtonTool tool = new ButtonTool(ToolNames.netbiosImport);
            tool.SharedProps.Caption = ToolNames.netbiosImport;
            tool.SharedProps.ToolTipText = "Discovers computers from NetBIOS";

            Image bgImage = Properties.Resources.netbios_netdisc32;
            tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            workItem.UIExtensionSites["netbiosRibbonTools"].Add<ButtonTool>(tool);
            tool.ToolClick += new ToolClickEventHandler(discovery_ToolClick);
        }

        private void InitializeTcpipTools()
        {
            // Add the "Discover" buttton
            ButtonTool tool = new ButtonTool(ToolNames.tcpipImport);
            tool.SharedProps.Caption = ToolNames.tcpipImport;
            tool.SharedProps.ToolTipText = "Finds computers by pinging a list of configured IP address ranges";

            Image bgImage = Properties.Resources.tcp_ip_netdisc32;
            tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            workItem.UIExtensionSites["tcpipRibbonTools"].Add<ButtonTool>(tool);
            tool.ToolClick += new ToolClickEventHandler(discovery_ToolClick);

            // Add the "Settings" button
            ButtonTool settingsTool = new ButtonTool(ToolNames.tcpipSettings);
            settingsTool.SharedProps.Caption = ToolNames.tcpipSettings;
            settingsTool.SharedProps.ToolTipText = "Modify the Network Discovery settings";

            settingsTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.settings32;
            workItem.UIExtensionSites["tcpipRibbonTools"].Add<ButtonTool>(settingsTool);
            settingsTool.ToolClick += new ToolClickEventHandler(discovery_ToolClick);
        }

        private void InitializeSNMPTools()
        {
            ButtonTool tool = new ButtonTool("Find devices");
            tool.SharedProps.Caption = "Find SNMP devices";
            tool.SharedProps.ToolTipText = "Find SNMP-enabled devices";

            Image bgImage = Properties.Resources.tcp_ip_netdisc32;
            tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            workItem.UIExtensionSites["snmpRibbonTools"].Add<ButtonTool>(tool);
            tool.ToolClick += new ToolClickEventHandler(discovery_ToolClick);

            ButtonTool settingsTool = new ButtonTool("SNMP Advanced Settings");
            settingsTool.SharedProps.Caption = "SNMP Advanced Settings";
            settingsTool.SharedProps.ToolTipText = "Modify the SNMP Discovery settings";

            settingsTool.SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.settings32;
            workItem.UIExtensionSites["snmpRibbonTools"].Add<ButtonTool>(settingsTool);
            settingsTool.ToolClick += new ToolClickEventHandler(discovery_ToolClick);
        }

        private void InitializeAutoScanTools()
        {
            ButtonTool tool = new ButtonTool("Auto-Discovery Settings");
            tool.SharedProps.Caption = "Auto-Discovery Settings";
            tool.SharedProps.ToolTipText = "Auto-Discovery Settings";

            Image bgImage = Properties.Resources.settings32;
            tool.SharedProps.AppearancesLarge.Appearance.Image = bgImage;
            workItem.UIExtensionSites["autoScanRibbonTools"].Add<ButtonTool>(tool);
            tool.ToolClick += new ToolClickEventHandler(discovery_ToolClick);
        }

        private void discovery_ToolClick(object sender, ToolClickEventArgs e)
        {
            NetworkDiscoveryWorkItemController controller = workItem.Controller as NetworkDiscoveryWorkItemController;
            switch (e.Tool.SharedProps.Caption)
            {
                case ToolNames.adImport:
                    controller.RunAdNetworkDiscovery();
                    break;
                case ToolNames.netbiosImport:
                    controller.RunNetbiosNetworkDiscovery();                    
                    break;
                case ToolNames.tcpipImport:
                    controller.RunBothDiscovery = false;
                    controller.RunTcpipNetworkDiscovery();
                    break;
                case ToolNames.tcpipSettings:
                    //controller.ShowSettings();
                    FormTcpipSettings formTCPIP = new FormTcpipSettings();
                    formTCPIP.ShowDialog();
                    break;
                case "Find SNMP devices":
                    controller.RunSNMPNetworkDiscovery();
                    break;
                case "SNMP Advanced Settings":
                    FormSNMPSettings formSNMP = new FormSNMPSettings();
                    formSNMP.ShowDialog();
                    break;
                case "Auto-Discovery Settings":
                    FormAutoScan formAutoScan = new FormAutoScan();
                    formAutoScan.ShowDialog();
                    break;
                case "Active Directory Advanced Settings":
                    FormADSettings formADSettings = new FormADSettings();
                    formADSettings.ShowDialog();
                    break;
                default:
                    break;
            }
        }
    }
}
