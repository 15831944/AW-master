using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.NetworkDiscovery
{
    public class EventTopics
    {
        public const string NetworkDiscoveryStarted = "event://NetworkDiscovery/NetworkDiscoveryStarted";
        public const string NetworkDiscoveryComplete = "event://NetworkDiscovery/NetworkDiscoveryComplete";
        public const string NetworkDiscoveryUpdate = "event://NetworkDiscovery/NetworkDiscoveryUpdate";
    }

    public class RibbonNames
    {
        public const string tabName = "Network Discovery";
        public const string adGroupName = "Active Directory";
        public const string netbiosGroupName = "NetBIOS";
        public const string tcpipGroupName = "TCP/IP";
    }

    public class ToolNames
    {
        public const string adImport = "Import Computers";
        public const string netbiosImport = "Discover Computers";
        public const string tcpipImport = "Find Computers";
        public const string tcpipSettings = "Advanced Settings";
    }

    public class XmlSettings
    {
        public const string tcpipSettings = "TcpipSettings";
        public const string ipAddressRanges = "IpAddressRanges";
		//
		public const string activeDirectorySettings = "ActiveDirectorySettings";
		public const string adDomainList = "ADDomainList";
		//
		public const string NetBiosSettings = "NetBiosSettings";
		public const string NetBiosDomainList = "NetBiosDomainList";
	}
}
