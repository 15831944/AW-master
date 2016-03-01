using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;

namespace Layton.NetworkDiscovery
{
    public class TcpipNetworkDiscovery : NetworkDiscovery
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);

        private List<string> _ipAddresses;
        private bool _isComplete;
        private bool _isStarted;
        private List<string[]> _computers = new List<string[]>();
        private List<string> _domains = new List<string>();
        private NameValueCollection _ipRanges;
        private int _maximumCount;
        private static int _foundCounter;
        private Countdown _countdown;
        private bool _workToDo;
        private const int MaxRange = 256;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private AssetTypeList assetTypes = new AssetTypeList();

        public TcpipNetworkDiscovery(NameValueCollection ipRanges)
        {
            _ipRanges = ipRanges;
            assetTypes.Populate();
        }

        public override bool HasDiscoverStarted
        {
            get { return _isStarted; }
        }

        public override bool IsDiscoverComplete
        {
            get { return _isComplete; }
        }

        public override List<string> DomainList
        {
            get { return _domains; }
        }

        public override List<string[]> ComputerList
        {
            get { return _computers; }
        }

        public override void Start()
        {
            try
            {
                _isStarted = true;
                _workToDo = true;
                _foundCounter = 0;

                int minw;
                int minc;
                ThreadPool.GetMinThreads(out minw, out minc);
                ThreadPool.SetMinThreads(20, minc);

                _maximumCount = GetIpAddressCount(_ipRanges);
                while (_workToDo)
                {
                    _ipAddresses = GetIpAddressList(_ipRanges);
                    _countdown = new Countdown(_ipAddresses.Count);

                    for (int i = 0; i < _ipAddresses.Count; i++)
                    {
                        ThreadPool.QueueUserWorkItem(ExecuteTcpipDiscover, i);
                    }

                    _countdown.Wait();
                }

                _isComplete = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayApplicationErrorMessage(String.Format(
                    "AuditWizard encountered an error during Network Discovery: {0}{1}{2}",
                    Environment.NewLine, Environment.NewLine, ex.Message));

                _isComplete = true;
            }
        }

        private int GetIpAddressCount(NameValueCollection ipRanges)
        {
            int count = 0;

            foreach (string startAddress in ipRanges.AllKeys)
            {
                IPAddress startIpAddress = IPAddress.Parse(startAddress);
                IPAddress endIpAddress = IPAddress.Parse(ipRanges[startAddress]);
                IPAddress currentIpAddress = startIpAddress;

                while (!currentIpAddress.Equals(endIpAddress))
                {
                    count++;

                    byte[] address = currentIpAddress.GetAddressBytes();

                    if (address[3] == 255)
                    {
                        if (address[2] == 255)
                        {
                            if (address[1] == 255)
                            {
                                address[0] += 1;
                                address[1] = 0;
                                address[2] = 0;
                                address[3] = 0;
                            }
                            else
                            {
                                address[1] += 1;
                                address[2] = 0;
                                address[3] = 0;
                            }
                        }
                        else
                        {
                            address[2] += 1;
                            address[3] = 0;
                        }
                    }
                    else
                    {
                        address[3] += 1;
                    }
                    currentIpAddress = new IPAddress(address);
                }

                count++;
            }

            return count;
        }

        private void ExecuteTcpipDiscover(object o)
        {
            FireNetworkDiscoveryUpdate(new DiscoveryUpdateEventArgs(_foundCounter.ToString(), "Computer", _maximumCount, 0));

            int index = (int)o;
            string ipAddress = _ipAddresses[index];
            string[] hostDetails;

            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(ipAddress, 5000);

                // is there a response from this IP address
                if (reply.Status == IPStatus.Success)
                {
                    IPAddress hostIPAddress = IPAddress.Parse(ipAddress);

                    // try and run SNMP on device first
                    // if it fails, just add the computer name and domain
                    IPHostEntry IpEntry = Dns.GetHostByAddress(hostIPAddress);
                    hostDetails = IpEntry.HostName.Split('.');

                    // certain machines need this step to get the hostname
                    if (hostDetails.Length == 1)
                    {
                        IpEntry = Dns.GetHostByName(hostDetails[0]);
                        hostDetails = IpEntry.HostName.Split('.');
                    }

                    // domain may be empty
                    string domain = (hostDetails.Length != 1) ? hostDetails[1].ToUpper() : "UNKNOWN";
					string macAddress = GetMacAddress(ipAddress);
					InsertComputer(hostDetails[0].ToUpper(), domain, ipAddress, macAddress);
                }
            }
            catch (IndexOutOfRangeException)
            {
                // means we have not found a hostname
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                // The usual example for this is a host name-to-address translation attempt 
                // (using gethostbyname or WSAAsyncGetHostByName) which uses the DNS (Domain Name Server). 
                // An MX record is returned but no A record — indicating the host itself exists, but is not directly reachable.

                // we can arrive here for an iPhone - final test to see if this is the case
                string iPhoneMacAddress = GetMacAddress(ipAddress);
                string iPhoneVendor = String.Empty;

                if (iPhoneMacAddress != String.Empty)
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(System.IO.Path.Combine(Application.StartupPath, "oui.txt")))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null) 
                        {
                            if (line.StartsWith(iPhoneMacAddress.Substring(0, 8)))
                            {
                                if (line.Substring(18).ToUpper().StartsWith("APPLE"))
                                    iPhoneVendor = line.Substring(18);

                                break;
                            }
                        }
                    }
                }

                iPhoneVendor = iPhoneVendor.ToUpper();

                if (iPhoneVendor.StartsWith("APPLE"))
                {
                    InsertComputer("Apple Device (" + iPhoneMacAddress + ")", String.Empty, ipAddress, iPhoneMacAddress);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                _countdown.Signal();
            }
        }

        public void InsertComputer(string assetName, string groupName, string ipAddress, string macAddress)
        {
            LocationsDAO lwDataAccess = new LocationsDAO();
            SettingsDAO lSettingsDao = new SettingsDAO();

            // We need to get the root item as all of the domains need to be parented to this
            System.Data.DataTable table = lwDataAccess.GetGroups(new AssetGroup(AssetGroup.GROUPTYPE.domain));
            AssetGroup rootGroup = new AssetGroup(table.Rows[0], AssetGroup.GROUPTYPE.domain);

            // Get the child domains - as domains are single level we do not need to recurse
            rootGroup.Populate(false, false, true);

            // We'll loop through the domains first and add then to the database recovering their ids so that 
            // we only have to do this once.  
            // Does this domain already exist?

            AssetGroup childGroup;

            lock (this)
            {
                childGroup = rootGroup.IsChildGroup(groupName);

                // No - add it as a new group both to the database and to the parent
                if (childGroup == null)
                {
                    childGroup = new AssetGroup(AssetGroup.GROUPTYPE.domain);
                    childGroup.Name = groupName;
                    childGroup.ParentID = rootGroup.GroupID;
                    childGroup.GroupID = lwDataAccess.GroupAdd(childGroup);
                    rootGroup.Groups.Add(childGroup);
                }
            }
            string vendor = String.Empty;

            try
            {
                if (macAddress != String.Empty)
                {

//
// CMD IMPORTANT UNCOMMENT THESE LINES
//                    using (System.IO.StreamReader sr = new System.IO.StreamReader(System.IO.Path.Combine(Application.StartupPath, "oui.txt")))
//                    {
//                        string line;
//                        while ((line = sr.ReadLine()) != null)
//                        {
//                            if (line.StartsWith(macAddress.Substring(0, 8)))
//                            {
//                                if (line.Substring(18).ToUpper().StartsWith("APPLE"))
//                                {
//                                   vendor = line.Substring(18);
//                                    break;
//                                }
//                           }
//                        }
//                    }
                }
            }
            catch (FormatException)
            {
            }

            // Now that we have the ID of the group (even if we just added the group) we can now
            // add the asset to the database also.
            Asset newAsset = new Asset();
            newAsset.Name = assetName;
            newAsset.MACAddress = macAddress.Replace('-', ':');
            newAsset.Make = vendor;

            if (vendor.ToUpper().StartsWith("APPLE"))
            {
                // add as an Apple Device
                assetTypes.Populate();
                AssetType parentAssetType = assetTypes.FindByName("Apple Devices");
                if (parentAssetType == null)
                {
                    // Now create a child of this asset type						
                    parentAssetType = new AssetType();
                    parentAssetType.Name = "Apple Devices";
                    parentAssetType.Auditable = false;
                    parentAssetType.Icon = "apple.png";
                    parentAssetType.ParentID = 0;
                    parentAssetType.Add();

                    // Update the internal list
                    assetTypes.Add(parentAssetType);
                }

                assetTypes.Populate();
                parentAssetType = assetTypes.FindByName("Apple Devices");

                AssetType childAssetType = assetTypes.FindByName("Apple Device");
                if (childAssetType == null)
                {
                    // Now create a child of this asset type						
                    childAssetType = new AssetType();
                    childAssetType.Name = "Apple Device";
                    childAssetType.Auditable = false;
                    childAssetType.Icon = parentAssetType.Icon;
                    childAssetType.ParentID = parentAssetType.AssetTypeID;
                    childAssetType.Add();

                    // Update the internal list
                    assetTypes.Add(childAssetType);
                }

                assetTypes.Populate();
                childAssetType = assetTypes.FindByName("Apple Device");
                newAsset.AssetTypeID = childAssetType.AssetTypeID;
            }

            AssetList assetList = new AssetList(new AssetDAO().GetAssets(0, AssetGroup.GROUPTYPE.userlocation, false), true);
            bool bUpdateAsset = true;
            bool bSNMPAsset = false;
            bool bExistingAuditedAsset = false;

            foreach (Asset existingAsset in assetList)
            {
                if ((existingAsset.AgentVersion == "SNMP") && (existingAsset.IPAddress == ipAddress))
                {
                    bSNMPAsset = true;
                    break;
                }
                
                if ((assetName == existingAsset.Name) && (groupName == existingAsset.Domain))
                {
                    
                    // this asset already exists - only need to check if domain or IP have changed
                    // if they have, send it away to be updated
                    if (existingAsset.IPAddress != ipAddress || existingAsset.DomainID != childGroup.GroupID)
                    {
                        newAsset = existingAsset;
                        newAsset.IPAddress = newAsset.IPAddress != ipAddress ? ipAddress : newAsset.IPAddress;
                        newAsset.DomainID = newAsset.DomainID != childGroup.GroupID ? childGroup.GroupID : newAsset.DomainID;
                    }
                    else
                    {
                        // asset exists, nothing has changed so don't process
                        bUpdateAsset = false;                        
                    }
                    break;
                }
                if (!bSNMPAsset && existingAsset.IPAddress == ipAddress && existingAsset.Domain != newAsset.Domain)
                {
                    bExistingAuditedAsset = true;
                    //check for any asset name change if so update asset with audittrail entry
                    if (existingAsset.Name != assetName)
                    {
                        string strOldValue = existingAsset.Name;
                        newAsset = existingAsset;
                        newAsset.Name = assetName;
                        newAsset.Update();
                        AuditTrailDAO objAuditTrailDAO = new AuditTrailDAO();
                        // Build a blank AuditTrailEntry
                        AuditTrailEntry ate = CreateAteForAssetNameChange(newAsset);
                        ate.Key = ate.Key + "|" + "Computer Name";
                        ate.OldValue = strOldValue;                        
                        ate.NewValue = assetName;                        
                        objAuditTrailDAO.AuditTrailAdd(ate);
                    }

                }
            }

            if (bUpdateAsset && !bSNMPAsset && !bExistingAuditedAsset)
            {
                newAsset.Domain = childGroup.Name;
                newAsset.DomainID = childGroup.GroupID;
                newAsset.IPAddress = ipAddress;

                // Add the asset
                newAsset.Add();

                if (lSettingsDao.GetSettingAsBoolean("AutoScanNetwork", false) && lSettingsDao.GetSettingAsBoolean("AutoScanDeployAgent", false))
                {
                    string scannerPath = System.IO.Path.Combine(Application.StartupPath, "scanners") + "\\auditagent\\default.xml";
                    System.IO.File.Copy(scannerPath, "AuditAgent\\AuditAgent.xml", true);
                    Operation newOperation = new Operation(newAsset.AssetID, Operation.OPERATION.deployagent);
                    newOperation.Add();
                }
            }

            if (!bSNMPAsset)
            {
                Interlocked.Increment(ref _foundCounter);
                FireNetworkDiscoveryUpdate(new DiscoveryUpdateEventArgs(_foundCounter.ToString(), "Computer", _maximumCount, 0));
            }
        }

        private AuditTrailEntry CreateAteForAssetNameChange(Asset objAsset)
        {
            AuditTrailEntry ate = new AuditTrailEntry();
            ate.Date = DateTime.Now;
            ate.Class = AuditTrailEntry.CLASS.asset;
            ate.Type = AuditTrailEntry.TYPE.changed;
            ate.Key = objAsset.Name;
            ate.AssetID = objAsset.AssetID;
            ate.AssetName = objAsset.Name; ;
            ate.Username = System.Environment.UserName;

            return ate;
        }

        private List<string> GetIpAddressList(NameValueCollection ipRanges)
        {
            int iIPRangesCount = ipRanges.Count;
            string strAddressToRemove=string.Empty; 
            if (iIPRangesCount > 1)
            {
                strAddressToRemove = ipRanges.GetKey(0);
            }
            List<string> ipAddresses = new List<string>();
            foreach (string startAddress in ipRanges.AllKeys)
            {
                IPAddress startIpAddress = IPAddress.Parse(startAddress);
                IPAddress endIpAddress = IPAddress.Parse(ipRanges[startAddress]);
                IPAddress currentIpAddress = startIpAddress;

                while (!currentIpAddress.Equals(endIpAddress))
                {
                    ipAddresses.Add(currentIpAddress.ToString());

                    if (ipAddresses.Count >= MaxRange)
                    {
                        if (iIPRangesCount > 1)
                        {
                            _ipRanges.Remove(strAddressToRemove);
                        }
                        else
                        {
                            _ipRanges.Remove(startAddress);
                            _ipRanges.Add(currentIpAddress.ToString(), endIpAddress.ToString());
                        }                
                                              
                        return ipAddresses;
                    }

                    byte[] address = currentIpAddress.GetAddressBytes();

                    if (address[3] == 255)
                    {
                        if (address[2] == 255)
                        {
                            if (address[1] == 255)
                            {
                                address[0] += 1;
                                address[1] = 0;
                                address[2] = 0;
                                address[3] = 0;
                            }
                            else
                            {
                                address[1] += 1;
                                address[2] = 0;
                                address[3] = 0;
                            }
                        }
                        else
                        {
                            address[2] += 1;
                            address[3] = 0;
                        }
                    }
                    else
                    {
                        address[3] += 1;
                    }
                    currentIpAddress = new IPAddress(address);
                }
                
                ipAddresses.Add(endIpAddress.ToString());
            }

            _workToDo = false;
            return ipAddresses;
        }

		// +8.4.1 CMD - Routine updated as it was not reliably getting the MAC address
        private string GetMacAddress(string aIpAddress)
        {
            try
            {
                //IPAddress hostIPAddress = IPAddress.Parse(aIpAddress);
				IPHostEntry hostEntry = Dns.GetHostEntry(aIpAddress);
				if (hostEntry.AddressList.Length == 0)
					return String.Empty;
				byte[] macAddr = new byte[6];
				int macAddrLen = (int) macAddr.Length;
				if (SendARP((int) hostEntry.AddressList[0].Address, 0, macAddr, ref macAddrLen) != 0)
					return String.Empty;

				StringBuilder macAddressString = new StringBuilder();
				for (int i = 0; i < macAddr.Length; i++)
				{
					if (macAddressString.Length > 0)
						macAddressString.Append("-");
					macAddressString.AppendFormat("{0:x2}", macAddr[i]);
				}
				return macAddressString.ToString().ToUpper();



			//    byte[] ab = new byte[6];
			//    int len = ab.Length;
			//    int r = SendARP((int)hostIPAddress.Address, 0, ab, ref len);

			//    return BitConverter.ToString(ab, 0, 6);
			}
			catch (Exception ex)
			{
			    return String.Empty;
			}
        }
    }

    public class Countdown : IDisposable
    {
        private readonly ManualResetEvent _done;
        private long _current;

        public Countdown(int total)
        {
            _current = total;
            _done = new ManualResetEvent(false);
        }

        public void Signal()
        {
            if (Interlocked.Decrement(ref _current) == 0)
            {
                _done.Set();
            }
        }

        public void Wait()
        {
            _done.WaitOne();
        }

        public void Dispose()
        {
            ((IDisposable)_done).Dispose();
        }
    }
}