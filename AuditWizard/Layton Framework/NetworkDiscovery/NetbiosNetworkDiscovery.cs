using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using Layton.AuditWizard.DataAccess;

namespace Layton.NetworkDiscovery
{
    public class NetbiosNetworkDiscovery : NetworkDiscovery
    {
        private bool isComplete = false;
        private bool isStarted = false;
        private int startCount = 1;
        private List<string[]> computers = new List<string[]>();
        private List<string> domains = new List<string>();
        private NameValueCollection _nbSettings = null;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The default <see cref="NETRESOURCE"/> used in the network discovery process.
        /// </summary>
        private NETRESOURCE netResource = new NETRESOURCE();

        public NetbiosNetworkDiscovery()
        {
        }

        public override bool HasDiscoverStarted
        {
            get { return isStarted; }
        }

        public override bool IsDiscoverComplete
        {
            get { return isComplete; }
        }

        public override List<string> DomainList
        {
            get { return domains; }
        }

        public override List<string[]> ComputerList
        {
            get { return computers; }
        }

        public override void Start()
        {
            try
            {
                uint bufferSize = 16384;
                IntPtr buffer = Marshal.AllocHGlobal((int)bufferSize);
                IntPtr handle = IntPtr.Zero;
                ErrorCodes result;
                uint entries = 1;

                result = WNetOpenEnum(ResourceScope.RESOURCE_GLOBALNET,
                                      ResourceType.RESOURCETYPE_DISK,
                                      ResourceUsage.RESOURCEUSAGE_ALL,
                                      netResource, out handle);

                if (result == ErrorCodes.NO_ERROR)
                {
                    do
                    {
                        result = WNetEnumResource(handle, ref entries, buffer, ref	bufferSize);

                        if (result == ErrorCodes.NO_ERROR)
                        {
                            Marshal.PtrToStructure(buffer, netResource);

                            if (netResource.dwDisplayType == ResourceDisplayType.RESOURCEDISPLAYTYPE_DOMAIN)
                            {
                                domains.Add(netResource.lpRemoteName);
                            }

                            if ((netResource.dwDisplayType == ResourceDisplayType.RESOURCEDISPLAYTYPE_SERVER) &&
                                netResource.lpRemoteName.Replace("\\", "") != "")
                            {
                                if (domains.Count > 0)
                                {
                                    string computer = netResource.lpRemoteName.Replace("\\", "");
                                    string domain = domains[domains.Count - 1];
                                    computers.Add(new string[] { computer, domain });
                                    FireNetworkDiscoveryUpdate(new DiscoveryUpdateEventArgs(computers.Count.ToString(), "Computer", computers.Count, 0));
                                }
                            }

                            else if ((netResource.dwUsage & ResourceUsage.RESOURCEUSAGE_CONTAINER) ==
                                ResourceUsage.RESOURCEUSAGE_CONTAINER)
                            {
                                startCount++;
                                Start();
                            }
                        }
                        else if (result != ErrorCodes.ERROR_NO_MORE_ITEMS)
                        {
                            break;
                        }
                    }
                    while (result != ErrorCodes.ERROR_NO_MORE_ITEMS);

                    WNetCloseEnum(handle);
                }
                Marshal.FreeHGlobal((IntPtr)buffer);

                startCount--;
                if (startCount == 0)
                {
                    isComplete = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayApplicationErrorMessage(String.Format(
                    "AuditWizard encountered an error during Network Discovery. The error message is:{0}{1}{2}",
                    Environment.NewLine, Environment.NewLine, ex.Message));

                isComplete = true;
            }

        }



        /// <summary>
        /// Return a list of just the domains/workgroups
        /// </summary>
        /// <returns></returns>
        public List<string> EnumerateDomains()
        {
            uint bufferSize = 16384;
            IntPtr buffer = Marshal.AllocHGlobal((int)bufferSize);
            IntPtr handle = IntPtr.Zero;
            ErrorCodes result;
            uint entries = 1;

            try
            {
                result = WNetOpenEnum(ResourceScope.RESOURCE_GLOBALNET,
                                      ResourceType.RESOURCETYPE_DISK,
                                      ResourceUsage.RESOURCEUSAGE_ALL,
                                      netResource, out handle);

                if (result == ErrorCodes.NO_ERROR)
                {
                    do
                    {
                        result = WNetEnumResource(handle, ref entries, buffer, ref	bufferSize);

                        if (result == ErrorCodes.NO_ERROR)
                        {
                            Marshal.PtrToStructure(buffer, netResource);

                            // If this is a domain then add to our list
                            if (netResource.dwDisplayType == ResourceDisplayType.RESOURCEDISPLAYTYPE_DOMAIN)
                            {
                                domains.Add(netResource.lpRemoteName);
                            }

                            else if ((netResource.dwUsage & ResourceUsage.RESOURCEUSAGE_CONTAINER) == ResourceUsage.RESOURCEUSAGE_CONTAINER)
                            {
                                EnumerateDomains();
                            }
                        }

                        else if (result != ErrorCodes.ERROR_NO_MORE_ITEMS)
                        {
                            break;
                        }
                    }
                    while (result != ErrorCodes.ERROR_NO_MORE_ITEMS);

                    WNetCloseEnum(handle);
                }
                Marshal.FreeHGlobal((IntPtr)buffer);
            }
            catch
            {
            }

            return domains;
        }


        /// <summary>
        /// The error codes used for the Win32 network api functions.
        /// </summary>
        private enum ErrorCodes
        {
            NO_ERROR = 0,
            ERROR_NO_MORE_ITEMS = 259
        };

        /// <summary>
        /// The structure found in the WNetEnumResource call when discovering the 
        /// network resources.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class NETRESOURCE
        {
            public ResourceScope dwScope = 0;
            public ResourceType dwType = 0;
            public ResourceDisplayType dwDisplayType = 0;
            public ResourceUsage dwUsage = 0;
            public string lpLocalName = null;
            public string lpRemoteName = null;
            public string lpComment = null;
            public string lpProvider = null;
        };

        [DllImport("Mpr.dll", EntryPoint = "WNetOpenEnumA", CallingConvention = CallingConvention.Winapi)]
        private static extern ErrorCodes WNetOpenEnum(ResourceScope dwScope, ResourceType dwType, ResourceUsage dwUsage, NETRESOURCE p, out IntPtr lphEnum);

        [DllImport("Mpr.dll", EntryPoint = "WNetCloseEnum", CallingConvention = CallingConvention.Winapi)]
        private static extern ErrorCodes WNetCloseEnum(IntPtr hEnum);

        [DllImport("Mpr.dll", EntryPoint = "WNetEnumResourceA", CallingConvention = CallingConvention.Winapi)]
        private static extern ErrorCodes WNetEnumResource(IntPtr hEnum, ref uint lpcCount, IntPtr buffer, ref uint lpBufferSize);

        [DllImport("mpr.dll", EntryPoint = "WNetGetUserA", CallingConvention = CallingConvention.Winapi)]
        private static extern int WNetGetUser(string lpName, string lpUserName, ref int lpnLength);

        [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2", CallingConvention = CallingConvention.Winapi)]
        private static extern int WNetAddConnection2([In] NETRESOURCE lpNetResource, string lpPassword, string lpUsername, Int32 dwFlags);
    }

    public enum ResourceScope
    {
        RESOURCE_CONNECTED = 1,
        RESOURCE_GLOBALNET,
        RESOURCE_REMEMBERED,
        RESOURCE_RECENT,
        RESOURCE_CONTEXT
    };

    public enum ResourceType
    {
        RESOURCETYPE_ANY,
        RESOURCETYPE_DISK,
        RESOURCETYPE_PRINT,
        RESOURCETYPE_RESERVED
    };

    public enum ResourceUsage
    {
        RESOURCEUSAGE_CONNECTABLE = 0x00000001,
        RESOURCEUSAGE_CONTAINER = 0x00000002,
        RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
        RESOURCEUSAGE_SIBLING = 0x00000008,
        RESOURCEUSAGE_ATTACHED = 0x00000010,
        RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
    };

    public enum ResourceDisplayType
    {
        RESOURCEDISPLAYTYPE_GENERIC,
        RESOURCEDISPLAYTYPE_DOMAIN,
        RESOURCEDISPLAYTYPE_SERVER,
        RESOURCEDISPLAYTYPE_SHARE,
        RESOURCEDISPLAYTYPE_FILE,
        RESOURCEDISPLAYTYPE_GROUP,
        RESOURCEDISPLAYTYPE_NETWORK,
        RESOURCEDISPLAYTYPE_ROOT,
        RESOURCEDISPLAYTYPE_SHAREADMIN,
        RESOURCEDISPLAYTYPE_DIRECTORY,
        RESOURCEDISPLAYTYPE_TREE,
        RESOURCEDISPLAYTYPE_NDSCONTAINER
    };
}
