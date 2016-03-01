//////////////////////////////////////////////////////////////////////////////////////////
//																						//
//    SNMPDiscovery																		//
//	  =============																		//
//																						//
//////////////////////////////////////////////////////////////////////////////////////////
//																						//
//  History																				//
//	-------																				//
//																						//
//	25-Oct-2011			Chris Drew			AW 8.3.3									//
//	Write an ADF file for all SNMP discovered assets if 'Upload Copy of Audit Files'    //
//	has been set in the audit scanner													//
//																						//
//////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using Layton.AuditWizard.DataAccess;
using System.Windows.Forms;
using SnmpSharpNet;
//
using Layton.AuditWizard.Common;

namespace Layton.NetworkDiscovery
{
    public class SNMPDiscovery : NetworkDiscovery
    {
        #region Data

        private bool _isComplete;
        private bool _isStarted;
        private List<string[]> _computers = new List<string[]>();
        private List<string> _domains = new List<string>();
        private string[] communityNames;
        private string publicCommunityName = String.Empty;

        private NameValueCollection ipRanges;
        private int remainingCount;
        private int maximumCount;
        private AssetTypeList assetTypes = new AssetTypeList();
        private static int foundCounter;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		// 8.3.3
		// This is a list of SNMP audited items (as List<AuditedItem>)
		// It is used to build a list of assets audited by SNMP discovery which can then be written
		// to an ADF file and from there uploaded to an FTP site
		private List<List<AuditedItem>> _snmpAudits = new List<List<AuditedItem>>();


        public SNMPDiscovery(NameValueCollection ipRanges)
        {
            this.ipRanges = ipRanges;
            communityNames = new SettingsDAO().GetSettingAsString("SNMPRead", "public").Split(',');

            // if we didn't get a community string, set 'public' as default
            if (communityNames.Length == 1 && communityNames[0] == String.Empty) communityNames[0] = "public";

            //assetTypes.Populate();
            PopulateAssetTypes();
        }

        private void PopulateAssetTypes()
        {
            try
            {
                lock (this)
                {
                    assetTypes.Populate();
                }
            }
            catch (Exception Ex)
            {
                logger.Error("Exception in AssetTypes Populate Lock:"+Ex.Message);
            }
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

        public override bool CanRunInOwnThread
        {
            get { return true; }
        }

		// 8.3.3
		// Return list of audited assets
		public List<List<AuditedItem>> SNMPAuditedAssets
		{
			get { return _snmpAudits; }
		}


        #endregion

        public override void Start()
        {
            try
            {
                _isStarted = true;
                List<string> ipAddresses = GetIpAddressList(ipRanges);

                Thread[] theThreads = new Thread[ipAddresses.Count];

                maximumCount = ipAddresses.Count;
                remainingCount = ipAddresses.Count;
                foundCounter = 0;

                for (int i = 0; i < ipAddresses.Count; i++)
                {
                    int threadCount = i - ipAddresses.Count + remainingCount;
                    if (threadCount > 256)
                    {
                        RunSnmpDiscovery(ipAddresses[i]);
                    }
                    else
                    {
                        theThreads[i] = new Thread(new ParameterizedThreadStart(RunSnmpDiscovery));
                        theThreads[i].Start(ipAddresses[i]);
                    }
                }

                foreach (Thread t in theThreads)
                {
                    if (t != null)
                        t.Join();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayApplicationErrorMessage(String.Format(
                    "AuditWizard encountered an error during Network Discovery. The error message is:{0}{1}{2}",
                    Environment.NewLine, Environment.NewLine, ex.Message));

                _isComplete = true;
            }
        }

        #region SNMP methods

        private void RunSnmpDiscovery(object aIpAddress)
        {
            FireNetworkDiscoveryUpdate(new DiscoveryUpdateEventArgs(foundCounter.ToString(), "", maximumCount, 0));

            string strIPAddress = (string)aIpAddress;

            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(strIPAddress, 120);

                // is there a response from this IP address
                if (reply != null)
                    if (reply.Status == IPStatus.Success)
                    {
                        if (GetSNMP(strIPAddress, "1.3.6.1.2.1.1.5.0") == null)
                            return;
                        string strPrinterStatus = GetSNMP(strIPAddress, "1.3.6.1.2.1.25.3.5.1.1.1");
                                         
                        int iErrorState = -1;
                        string strPrinterErrorState = GetNextSNMP(strIPAddress, "1.3.6.1.2.1.25.3.5.1.2");
                        if (strPrinterErrorState != "Null" || strPrinterErrorState != null)
                        {
                            try
                            {
                                iErrorState = Convert.ToInt32(strPrinterErrorState.Trim());
                            }
                            catch (Exception Ex)
                            {
                                //WriteDebugLog(strIPAddress + ": IntegerConvertion:" + Ex.Message);
                            }                           
                        }

                        if (((strPrinterStatus != "Null") && (strPrinterStatus != "1")) || (iErrorState != -1))//Changed TOM as all the generic devices returns 1
                                                                      
                            ProcessNetworkPrinter(strIPAddress);                  
                        else                                       
                            ProcessNetworkDevice(strIPAddress);                            
                       
                    }
            }
            catch (Exception ex)
            {
                // TODO - log this message
                return;
            }
            finally
            {
                remainingCount--;
            }
        }

        private void ProcessNetworkDevice(string aIPAddress)
        {
        
            AssetDAO lAssetDAO = new AssetDAO();
            AuditTrailEntry.CLASS ateClass;
            string ipAddress = aIPAddress;
            DateTime dateAudited = DateTime.Now;
            string strMacAddress = String.Empty;
            
            string modelName = GetSNMP(aIPAddress, "1.3.6.1.2.1.1.1.0");
            
            string adminName = GetSNMP(aIPAddress, "1.3.6.1.2.1.1.5.0");
            

        
            List<string> macAddresses = WalkSNMP(aIPAddress, "1.3.6.1.2.1.2.2.1.6");

            foreach (string macAddress in macAddresses)
            {
                 // if we have an empty address or local (00:00:00:00:00:00), ignore it
                //if ((macAddress != String.Empty) && (macAddress != "00 00 00 00 00 00"))
                if ((macAddress != String.Empty) && (macAddress.Contains("00 00 00 00 00 00") == false)) //TOM To fix the null address issue
                {
                    strMacAddress = macAddress;
                    break;
                }
            }

            strMacAddress = strMacAddress.Replace(' ', '-');
           
            string manufacturer = VendorFromMacAddress(strMacAddress);
            
            if (manufacturer == String.Empty)
                manufacturer = "Unknown";

            DateTime sysUpTime = DateTime.Now.Subtract(new TimeSpan((((SnmpSharpNet.TimeTicks)(GetSNMPObject(aIPAddress, "1.3.6.1.2.1.1.3.0"))).Miliseconds) * 10000));
            string strSysUpTime = sysUpTime.ToShortDateString() + " " + sysUpTime.ToShortTimeString();

            // add as an asset first
            Asset discoveredAsset = new Asset();
            discoveredAsset.Name = adminName;
            discoveredAsset.Make = manufacturer;
            discoveredAsset.Model = modelName;
            discoveredAsset.IPAddress = ipAddress;
            discoveredAsset.MACAddress = strMacAddress.Replace('-', ':');
            discoveredAsset.LastAudit = dateAudited;
            discoveredAsset.AgentVersion = "SNMP";
            discoveredAsset.AssetTypeID = GetNetworkDeviceTypeId();

            
            // check if asset exists first               
            int lAssetID = lAssetDAO.AssetFind(discoveredAsset);
         
            if (lAssetID == 0)
            {
                ateClass = AuditTrailEntry.CLASS.audited;
                
                //Check for duplication from tcp discovery if there is delete and then add
                Asset tmpasset = new Asset();
                tmpasset.Name = discoveredAsset.Name;
                tmpasset.AssetTypeID = discoveredAsset.AssetTypeID;
                tmpasset.IPAddress = discoveredAsset.IPAddress;
                tmpasset.MACAddress = discoveredAsset.MACAddress;
                tmpasset.AgentVersion = "DUP";
                int iTmpAssetID = GetAssetID(tmpasset);
                //SNMP asset of type PC identified as PC delete that         
                if (iTmpAssetID > 0)
                {
                    lAssetDAO.AssetDelete(iTmpAssetID);
                }
                //Comments from Sojan E John, KTS Infotech, This is done as a work around for hilton problem
                //if iTmpAssetID -1 a PC with SNMP enabled, leave as it is discard the discovered asset,
                //this can be confirmed only after an auditscan
                //Also check while uploading an ADF, a PC enabled with SNMP added to database as SNMP device
                //If there is one delete that first then upload the ADF 

                if (iTmpAssetID != -1)
                {
                    lAssetID = lAssetDAO.AssetAdd(discoveredAsset);
                }
                
            }
            else
            {
                // this SNMP asset already exists - delete existing audited items data as we only want most recent
                ateClass = AuditTrailEntry.CLASS.reaudited;
                new AuditedItemsDAO().AuditedItemsDelete(lAssetID);

                Asset existingAsset = lAssetDAO.AssetGetDetails(lAssetID);

                discoveredAsset.Location = existingAsset.Location;
                discoveredAsset.LocationID = existingAsset.LocationID;
                discoveredAsset.Domain = existingAsset.Domain;
                discoveredAsset.DomainID = existingAsset.DomainID;
                discoveredAsset.OverwriteData = existingAsset.OverwriteData;

                if (!existingAsset.OverwriteData)
                {
                    // don't overwrite the data defined by user in Basic Information tab
                    discoveredAsset.Name = existingAsset.Name;
                    discoveredAsset.StockStatus = existingAsset.StockStatus;
                    discoveredAsset.AssetTypeID = existingAsset.AssetTypeID;
                    discoveredAsset.TypeAsString = existingAsset.TypeAsString;
                    discoveredAsset.Make = existingAsset.Make;
                    discoveredAsset.Model = existingAsset.Model;
                    discoveredAsset.SerialNumber = existingAsset.SerialNumber;
                    discoveredAsset.AssetTag = existingAsset.AssetTag;

                    manufacturer = existingAsset.Make;
                    modelName = existingAsset.Model;
                }

                discoveredAsset.AssetID = lAssetID;
                lAssetDAO.AssetUpdate(discoveredAsset);
                lAssetDAO.AssetAudited(lAssetID, DateTime.Now);
            }

            List<AuditedItem> auditedItemList = new List<AuditedItem>();

            string lCategory = "System|Device Info";
            auditedItemList.Add(new AuditedItem(0, lAssetID, lCategory, "Manufacturer", manufacturer, String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, lAssetID, lCategory, "Model Name", modelName, String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, lAssetID, lCategory, "Admin Name", adminName, String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, lAssetID, lCategory, "IP Address", ipAddress, String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, lAssetID, lCategory, "MAC Address", strMacAddress, String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, lAssetID, lCategory, "Date Audited", dateAudited.ToShortDateString() + " " + dateAudited.ToShortTimeString(), String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, lAssetID, lCategory, "Last Network Initialisation", strSysUpTime, String.Empty, AuditedItem.eDATATYPE.text, true));

            foreach (AuditedItem auditedItem in auditedItemList)
            {
                auditedItem.Icon = "hardware.png";
                new AuditedItemsDAO().AuditedItemAdd(auditedItem);
            }

            // flag as audited
            new AssetDAO().AssetAudited(lAssetID, DateTime.Now);

			// 8.3.3
			// If the scanner configuration indicates that audited assets should be uploaded to an FTP site then we need to add this asset
			// to an internal list so that it can be processed at the end of the discovery process
			_snmpAudits.Add(auditedItemList);


            // Is this a first or a re-audit of this asset - create a history entry accordingly
            AuditTrailEntry ate = new AuditTrailEntry();
            ate.AssetID = lAssetID;
            ate.AssetName = discoveredAsset.Name;
            ate.Type = AuditTrailEntry.TYPE.added;
            ate.Date = DateTime.Now;
            ate.Key = ate.OldValue = ate.NewValue = "";
            ate.Class = ateClass;

            AuditTrailDAO lAuditTrailDAO = new AuditTrailDAO();
            lAuditTrailDAO.AuditTrailAdd(ate);

            Interlocked.Increment(ref foundCounter);
            FireNetworkDiscoveryUpdate(new DiscoveryUpdateEventArgs(foundCounter.ToString(), "Network Device", maximumCount, remainingCount));
                    
        }

        private int GetAssetID(Asset objAsset)
        {
            AssetDAO lAssetDAO = new AssetDAO();
            int iAssetID = lAssetDAO.AssetFind(objAsset);
            if (iAssetID != 0)
            {
                Asset existingAsset = lAssetDAO.AssetGetDetails(iAssetID);
                if (existingAsset.AgentVersion.Contains("AuditScanner"))
                {
                    return -1;
                }
                else
                {
                    if (existingAsset.IPAddress == objAsset.IPAddress || existingAsset.AgentVersion != "SNMP")
                    {
                        return iAssetID;
                    }
                }
            }
            return 0;
        }

        private int GetNetworkDeviceTypeId()
        {
            AssetType assetType = assetTypes.FindByName("Network Device");
            if (assetType == null)
            {
                // We need to create a new type of asset ensuring that we parent it correctly so first get our 
                // parent category.
                AssetType parentAssetType = assetTypes.FindByName("Peripherals");

                // Now create a child of this asset type						
                assetType = new AssetType();
                assetType.Name = "Network Device";
                assetType.Auditable = false;
                assetType.Icon = "router.png";
                assetType.ParentID = parentAssetType.AssetTypeID;
                assetType.Add();

                // Update the internal list
                assetTypes.Add(assetType);
            }

            //assetTypes.Populate();
            PopulateAssetTypes();
            assetType = assetTypes.FindByName("Network Device");
            return assetType.AssetTypeID;
        }

        private void ProcessNetworkPrinter(string aIPAddress)
        {
            
            AuditTrailEntry.CLASS ateClass;
            Printer printer = DiscoverPrinterBySNMP(aIPAddress);
            AssetDAO lAssetDAO = new AssetDAO();

            // add the printer as an asset first
            Asset printerAsset = new Asset();                
            printerAsset.Name = printer.AdminName;                    
            printerAsset.Make = printer.Manufacturer;                    
            printerAsset.Model = printer.ModelName;                    
            printerAsset.SerialNumber = printer.SerialNumber;                    
            printerAsset.IPAddress = printer.IpAddress;                    
            printerAsset.MACAddress = printer.MacAddress.Replace('-', ':');                    
            printerAsset.AgentVersion = "SNMP";                

            AssetType assetType = assetTypes.FindByName("Network Printer");                
            if (assetType == null)
            {
                // We need to create a new type of asset ensuring that we parent it correctly so first get our 
                // parent category.
                
                AssetType parentAssetType = assetTypes.FindByName("Peripherals");

                // Now create a child of this asset type						
                assetType = new AssetType();
                assetType.Name = "Network Printer";
                assetType.Auditable = false;
                assetType.Icon = "printer.png";
                assetType.ParentID = parentAssetType.AssetTypeID;
                assetType.Add();                        
                // Update the internal list
                assetTypes.Add(assetType);                    
               
            }
           
            //assetTypes.Populate();
            PopulateAssetTypes();                    
            assetType = assetTypes.FindByName("Network Printer");                    
            printerAsset.AssetTypeID = assetType.AssetTypeID;              
            
            // check if asset exists first                
            int lAssetID = lAssetDAO.AssetFind(printerAsset);             
           
            if (lAssetID == 0)
            {
                ateClass = AuditTrailEntry.CLASS.audited;
            
                //Check for any duplicates
                Asset tmpasset = new Asset();
                tmpasset.Name = printerAsset.Name;
                tmpasset.AgentVersion = "DUP";
                tmpasset.IPAddress = printerAsset.IPAddress;
                tmpasset.MACAddress = printerAsset.MACAddress;
                int iTmpAssetID = GetAssetID(tmpasset);
                if (iTmpAssetID > 0)
                {
                    lAssetDAO.AssetDelete(iTmpAssetID);
                }
                lAssetID = lAssetDAO.AssetAdd(printerAsset);
                
            }
            else
            {
                // this SNMP asset already exists - delete existing data as we only want most recent
                ateClass = AuditTrailEntry.CLASS.reaudited;                    
                new AuditedItemsDAO().AuditedItemsDelete(lAssetID);                   

                Asset existingAsset = lAssetDAO.AssetGetDetails(lAssetID);
                
                printerAsset.Location = existingAsset.Location;
                printerAsset.LocationID = existingAsset.LocationID;
                printerAsset.Domain = existingAsset.Domain;
                printerAsset.DomainID = existingAsset.DomainID;
                printerAsset.OverwriteData = existingAsset.OverwriteData;
                

                if (!existingAsset.OverwriteData)
                {
                    // don't overwrite the data defined by user in Basic Information tab
                    
                    printerAsset.Name = existingAsset.Name;
                    printerAsset.StockStatus = existingAsset.StockStatus;
                    printerAsset.AssetTypeID = existingAsset.AssetTypeID;
                    printerAsset.TypeAsString = existingAsset.TypeAsString;
                    printerAsset.Make = existingAsset.Make;
                    printerAsset.Model = existingAsset.Model;
                    printerAsset.SerialNumber = existingAsset.SerialNumber;
                    printerAsset.AssetTag = existingAsset.AssetTag;

                    printer.Manufacturer = existingAsset.Make;
                    printer.ModelName = existingAsset.Model;
                    
                }                    
                
                printerAsset.AssetID = lAssetID;
                lAssetDAO.AssetUpdate(printerAsset);                    
                lAssetDAO.AssetAudited(lAssetID, DateTime.Now);
            }
            

            List<AuditedItem> auditedItemList = GetAuditedItemsPrinterList(printer, lAssetID);

            // insert into AUDTIEDITEMS table
            foreach (AuditedItem auditedItem in auditedItemList)
            {
                auditedItem.Icon = "printer.png";
                new AuditedItemsDAO().AuditedItemAdd(auditedItem);
            }

            // flag as audited
            lAssetDAO.AssetAudited(lAssetID, DateTime.Now);

			// 8.3.3
			// If the scanner configuration indicates that audited assets should be uploaded to an FTP site then we need to add this asset
			// to an internal list so that it can be processed at the end of the discovery process
			_snmpAudits.Add(auditedItemList);

            // Is this a first or a re-audit of this asset - create a history entry accordingly
            AuditTrailEntry ate = new AuditTrailEntry();
            ate.AssetID = lAssetID;
            ate.AssetName = printerAsset.Name;
            ate.Type = AuditTrailEntry.TYPE.added;
            ate.Date = DateTime.Now;
            ate.Key = ate.OldValue = ate.NewValue = "";
            ate.Class = ateClass;                
            
            CheckNewPrinterLevels(lAssetID, printerAsset.Name);                   
            
            AuditTrailDAO lAuditTrailDAO = new AuditTrailDAO();
            lAuditTrailDAO.AuditTrailAdd(ate);

            Interlocked.Increment(ref foundCounter);
            FireNetworkDiscoveryUpdate(new DiscoveryUpdateEventArgs(foundCounter.ToString(), "Printer", maximumCount, remainingCount));              
        
        }

        private void CheckNewPrinterLevels(int assetID, string assetName)
        {
            string supplyName;
            int supplyLevel;
            System.Data.DataTable dataTable = new StatisticsDAO().CheckNewPrinterLevels(assetID, assetName);

            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                supplyName = row.ItemArray[1].ToString();
                supplyLevel = Convert.ToInt32(row.ItemArray[2]);

                NewsFeed.AddNewsItem(NewsFeed.Priority.Warning,
                    String.Format("{0} on {1} is at {2}%.", supplyName, assetName, supplyLevel));
            }
        }

        private Printer DiscoverPrinterBySNMP(string aIPAddress)
        {
            Printer printer = new Printer();
            printer.IpAddress = aIPAddress;
            printer.DateAudited = DateTime.Now;

            printer.ModelName = GetSNMP(aIPAddress, "1.3.6.1.2.1.25.3.2.1.3.1");                
            printer.SerialNumber = GetSNMP(aIPAddress, "1.3.6.1.2.1.43.5.1.1.17.1");               
            printer.AdminName = GetSNMP(aIPAddress, "1.3.6.1.2.1.1.5.0"); ;                
            printer.MacAddress = GetSNMP(aIPAddress, "1.3.6.1.2.1.2.2.1.6.1").Replace(' ', '-');                
            printer.Manufacturer = VendorFromMacAddress(printer.MacAddress);
        
            printer.SysUpTime = DateTime.Now.Subtract(new TimeSpan((((SnmpSharpNet.TimeTicks)(GetSNMPObject(aIPAddress, "1.3.6.1.2.1.1.3.0"))).Miliseconds) * 10000));
        
            string strMemorySize = GetSNMP(aIPAddress, "1.3.6.1.2.1.25.2.2.0");
            if (strMemorySize != "Null" && strMemorySize != null)
            {
                double memorySize = Convert.ToDouble(strMemorySize);
                printer.Memory = Convert.ToInt32(memorySize / 1024);
            }
            
            List<string> supplyDescriptions = WalkSNMP(aIPAddress, "1.3.6.1.2.1.43.11.1.1.6");
            List<string> maxSupplyCapacities = WalkSNMP(aIPAddress, "1.3.6.1.2.1.43.11.1.1.8");
            List<string> currentSupplyCapacities = WalkSNMP(aIPAddress, "1.3.6.1.2.1.43.11.1.1.9");
            List<string> currentSupplyTypes = WalkSNMP(aIPAddress, "1.3.6.1.2.1.43.11.1.1.5");

            for (int i = 0; i < supplyDescriptions.Count; i++)
            {
                PrinterLevel printerLevel = new PrinterLevel();
                printerLevel.MediaName = supplyDescriptions[i];
                printerLevel.MediaType = (PrinterLevel.SupplyType)(Convert.ToInt32(currentSupplyTypes[i]));

                // a value of -3 indicates unknown (not full or empty)
                // set level to 50% in this instance
                if (currentSupplyCapacities[i] == "-3")
                {
                    printerLevel.MediaLevel = 50;
                }
                else
                {
                    printerLevel.MediaLevel = Convert.ToInt32(((Convert.ToDouble(currentSupplyCapacities[i])) / (Convert.ToDouble(maxSupplyCapacities[i]))) * 100);
                }
                printer.PrinterLevels.Add(printerLevel);
            }

            string strPrintedTotal = GetSNMP(aIPAddress, "1.3.6.1.2.1.43.10.2.1.4.1.1");
            if (strPrintedTotal != "Null" && strPrintedTotal != null)
            {
                printer.PagesPrintedTotal = Convert.ToInt32(strPrintedTotal);
            }

            string strPrintedSinceReboot = GetSNMP(aIPAddress, "1.3.6.1.2.1.43.10.2.1.5.1.1");
            if (strPrintedSinceReboot != "Null" && strPrintedSinceReboot != null)
            {
                printer.PagesPrintedSinceReboot = Convert.ToInt32(strPrintedSinceReboot);
            }

            string strPrinterCurrentStatus = GetSNMP(aIPAddress, "1.3.6.1.2.1.25.3.5.1.1.1");
            if (strPrinterCurrentStatus != "Null" && strPrinterCurrentStatus != null)
            {
                printer.PrinterCurrentStatus = (Printer.PrinterStatus)(Convert.ToInt32(strPrinterCurrentStatus));
            }

            string strPrinterType = GetSNMP(aIPAddress, "1.3.6.1.2.1.43.10.2.1.2.1.1");
            if (strPrinterType != "Null" && strPrinterType != null)
            {
                printer.TypeOfPrinter = (Printer.PrinterType)(Convert.ToInt32(strPrinterType));
            }
            
                       
            List<string> printTrayNames = WalkSNMP(aIPAddress, "1.3.6.1.2.1.43.8.2.1.13");
            List<string> printTrayMaxCapacity = WalkSNMP(aIPAddress, "1.3.6.1.2.1.43.8.2.1.9");
            List<string> printTrayMediaType = WalkSNMP(aIPAddress, "1.3.6.1.2.1.43.8.2.1.12");
            List<string> printTrayStatuses = WalkSNMP(aIPAddress, "1.3.6.1.2.1.43.8.2.1.10");

            for (int i = 0; i < printTrayNames.Count; i++)
            {
                PrinterInputTray printerTray = new PrinterInputTray();
                printerTray.TrayName = printTrayNames[i];
                printerTray.TrayCapacity = Convert.ToInt32(printTrayMaxCapacity[i]);
                printerTray.PaperType = printTrayMediaType[i];
                printerTray.TrayStatus = (PrinterInputTray.PrinterTrayStatus)Convert.ToInt32(printTrayStatuses[i]);

                printer.PrinterInputTrays.Add(printerTray);
            }

            List<string> outputTrayNames = WalkSNMP(aIPAddress, "1.3.6.1.2.1.43.9.2.1.7");
            List<string> outputTrayMaxCapacities = WalkSNMP(aIPAddress, "1.3.6.1.2.1.43.9.2.1.4");
            List<string> outputTrayStackingOrder = WalkSNMP(aIPAddress, "1.3.6.1.2.1.43.9.2.1.19");
            List<string> outputTrayPageOrientation = WalkSNMP(aIPAddress, "1.3.6.1.2.1.43.9.2.1.20");

            for (int i = 0; i < outputTrayNames.Count; i++)
            {
                PrinterOutputTray printerOutputTray = new PrinterOutputTray();
                printerOutputTray.TrayName = outputTrayNames[i];
                printerOutputTray.TrayCapacity = Convert.ToInt32(outputTrayMaxCapacities[i]);
                printerOutputTray.StackingOrder = (PrinterOutputTray.OutputStackingOrder)Convert.ToInt32(outputTrayStackingOrder[i]);
                printerOutputTray.PageOrientation = (PrinterOutputTray.OutputPageDelivery)Convert.ToInt32(outputTrayPageOrientation[i]);

                printer.PrinterOutputTrays.Add(printerOutputTray);
            }

            return printer;
        }

        /// <summary>
        /// Performs an SNMP walk
        /// </summary>
        /// <param name="oID">OID to walk</param>
        /// <returns>List of values under the given OID</returns>
        private List<string> WalkSNMP(string aIPAddress, string oID)
        {
            List<string> walkResults = new List<string>();

            foreach (string communityName in communityNames)
            {
                SimpleSnmp snmp = new SimpleSnmp(aIPAddress, communityName);
                Dictionary<Oid, AsnType> result = snmp.Walk(SnmpVersion.Ver2, oID);

                if (result != null)
                {
                    foreach (KeyValuePair<Oid, AsnType> kvp in result)
                    {
                        walkResults.Add(kvp.Value.ToString());
                    }

                    return walkResults;
                }
            }

            return walkResults;
        }

        /// <summary>
        /// Performs an SNMP get
        /// </summary>
        /// <param name="oID">The OID to get</param>
        /// <returns>Value of the OID</returns>
        private string GetSNMP(string aIPAddress, string oID)
        {
            foreach (string communityName in communityNames)
            {
                SimpleSnmp snmp = new SimpleSnmp(aIPAddress, communityName);
                Dictionary<Oid, AsnType> result = snmp.Get(SnmpVersion.Ver2, new string[] { oID });

                if (result != null)
                {
                    foreach (KeyValuePair<Oid, AsnType> kvp in result)
                    {
                        return kvp.Value.ToString();
                    }
                }
            }

            return null;
        }

        private string GetNextSNMP(string aIPAddress, string oID)
        {
            foreach (string communityName in communityNames)
            {
                SimpleSnmp snmp = new SimpleSnmp(aIPAddress, communityName);
                Dictionary<Oid, AsnType> result = snmp.GetNext(SnmpVersion.Ver2, new string[] { oID });

                if (result != null)
                {
                    foreach (KeyValuePair<Oid, AsnType> kvp in result)
                    {
                       return kvp.Value.ToString(); 
                    }

                }
            }
            return null;
        }


        /// <summary>
        /// Performs an SNMP get
        /// </summary>
        /// <param name="oID">The OID to get</param>
        /// <returns>Value of the OID</returns>
        private object GetSNMPObject(string aIPAddress, string oID)
        {
            foreach (string communityName in communityNames)
            {
                SimpleSnmp snmp = new SimpleSnmp(aIPAddress, communityName);
                Dictionary<Oid, AsnType> result = snmp.Get(SnmpVersion.Ver2, new string[] { oID });

                if (result != null)
                {
                    foreach (KeyValuePair<Oid, AsnType> kvp in result)
                    {
                        return kvp.Value;
                    }
                }
            }

            return null;
        }

        #endregion

        #region Helper methods

        private void CheckAllIpAddresses()
        {
            List<string> ipAddresses = GetIpAddressList(ipRanges);

            foreach (string ipAddress in ipAddresses)
            {
                RunSnmpDiscovery(ipAddress);
            }
        }

        private List<string> GetIpAddressList(NameValueCollection ipRanges)
        {
            List<string> ipAddresses = new List<string>();
            foreach (string startAddress in ipRanges.AllKeys)
            {
                IPAddress startIpAddress = IPAddress.Parse(startAddress);
                IPAddress endIpAddress = IPAddress.Parse(ipRanges[startAddress]);
                IPAddress currentIpAddress = startIpAddress;
                while (!currentIpAddress.Equals(endIpAddress))
                {
                    ipAddresses.Add(currentIpAddress.ToString());
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
            return ipAddresses;
        }

        private string VendorFromMacAddress(string aMacAddress)
        {
            if (aMacAddress == "")
            {
                return String.Empty;
            }
            aMacAddress = aMacAddress.Substring(0, 8);
            using (System.IO.StreamReader sr = new System.IO.StreamReader(System.IO.Path.Combine(Application.StartupPath, "oui.txt")))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith(aMacAddress))
                        return line.Substring(18);
                }
            }

            return String.Empty;
        }

        #endregion

        #region Worker Methods

        private List<AuditedItem> GetAuditedItemsPrinterList(Printer aPrinter, int aAssetID)
        {
            List<AuditedItem> auditedItemList = new List<AuditedItem>();

            string lCategory = "System|Device Info";
            auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory, "Manufacturer", aPrinter.Manufacturer, String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory, "Model Name", aPrinter.ModelName, String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory, "Admin Name", aPrinter.AdminName, String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory, "Serial Number", aPrinter.SerialNumber, String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory, "IP Address", aPrinter.IpAddress, String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory, "MAC Address", aPrinter.MacAddress, String.Empty, AuditedItem.eDATATYPE.text, true));

            lCategory = "System|Configuration";
            auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory, "Date Audited", aPrinter.DateAudited.ToShortDateString() + " " + aPrinter.DateAudited.ToShortTimeString(), String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory, "Printer Type", aPrinter.TypeOfPrinter.ToString(), String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory, "Total RAM", aPrinter.Memory.ToString(), "MB", AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory, "Printer Status", aPrinter.PrinterCurrentStatus.ToString(), String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory, "Last Network Initialisation", aPrinter.SysUpTime.ToShortDateString() + " " + aPrinter.SysUpTime.ToShortTimeString(), String.Empty, AuditedItem.eDATATYPE.text, true));

            lCategory = "System|Counters";
            auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory, "Total Pages Printed", aPrinter.PagesPrintedTotal.ToString(), String.Empty, AuditedItem.eDATATYPE.text, true));
            auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory, "Pages Printed Since Reboot", aPrinter.PagesPrintedSinceReboot.ToString(), String.Empty, AuditedItem.eDATATYPE.text, true));

            foreach (PrinterLevel printerLevel in aPrinter.PrinterLevels)
            {
                lCategory = "System|Supply Levels";
                auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory + "|" + printerLevel.MediaName, "Supply Name", printerLevel.MediaName, String.Empty, AuditedItem.eDATATYPE.text, true));
                auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory + "|" + printerLevel.MediaName, "Supply Type", printerLevel.MediaType.ToString(), String.Empty, AuditedItem.eDATATYPE.text, true));
                auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory + "|" + printerLevel.MediaName, "Supply Level", printerLevel.MediaLevel.ToString(), "%", AuditedItem.eDATATYPE.text, true));
            }

            foreach (PrinterInputTray inputTray in aPrinter.PrinterInputTrays)
            {
                lCategory = "System|Printer Trays|Input";
                auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory + "|" + inputTray.TrayName, "Tray Name", inputTray.TrayName, String.Empty, AuditedItem.eDATATYPE.text, true));
                auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory + "|" + inputTray.TrayName, "Tray Capacity", inputTray.TrayCapacity.ToString(), " pages", AuditedItem.eDATATYPE.text, true));
                auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory + "|" + inputTray.TrayName, "Paper Type", inputTray.PaperType, String.Empty, AuditedItem.eDATATYPE.text, true));
                auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory + "|" + inputTray.TrayName, "Tray Status", inputTray.TrayStatus.ToString(), String.Empty, AuditedItem.eDATATYPE.text, true));
            }

            foreach (PrinterOutputTray outputTray in aPrinter.PrinterOutputTrays)
            {
                lCategory = "System|Printer Trays|Output";
                auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory + "|" + outputTray.TrayName, "Tray Name", outputTray.TrayName, String.Empty, AuditedItem.eDATATYPE.text, true));
                auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory + "|" + outputTray.TrayName, "Tray Capacity", outputTray.TrayCapacity.ToString(), " pages", AuditedItem.eDATATYPE.text, true));
                auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory + "|" + outputTray.TrayName, "Page Orientation", outputTray.PageOrientation.ToString(), String.Empty, AuditedItem.eDATATYPE.text, true));
                auditedItemList.Add(new AuditedItem(0, aAssetID, lCategory + "|" + outputTray.TrayName, "Stacking Order", outputTray.StackingOrder.ToString(), String.Empty, AuditedItem.eDATATYPE.text, true));
            }

            return auditedItemList;
        }

        #endregion


		/// <summary>
		/// Process a list of SNMP audited items given that we need to create ADF file versions of these for upload to a network (FTP/SFTP) location
		/// </summary>
		/// <param name="auditScannerDefinition"></param>
		public void UploadDiscoveredAssets(AuditScannerDefinition auditScannerDefinition)
		{
			string tempPath = Path.GetTempPath();

			AssetDAO lAssetDAO = new AssetDAO();
			AssetTypeList assetTypes = new AssetTypeList();
			assetTypes.Populate();

			// Loop through the list of audited items
			foreach (List<AuditedItem> listAuditedItems in _snmpAudits)
			{
				if (listAuditedItems.Count == 0)
					continue;

				// Get the asset name and from there the asset itself
				int assetID = listAuditedItems[0].AssetID;
				Asset existingAsset = lAssetDAO.AssetGetDetails(assetID);
				if (existingAsset == null)
					return;

				// Create an AuditDataFile for this asset
				AuditDataFile auditDataFile = new AuditDataFile();
				auditDataFile.AgentVersion = "SNMP";
				auditDataFile.AssetName = existingAsset.Name;
				auditDataFile.AssetTag = existingAsset.AssetTag;
				auditDataFile.AuditDate = DateTime.Now;
				auditDataFile.Ipaddress = existingAsset.IPAddress;
				auditDataFile.Macaddress = existingAsset.MACAddress;
				auditDataFile.Make = existingAsset.Make;
				auditDataFile.Model = existingAsset.Model;
				auditDataFile.AgentVersion = existingAsset.AgentVersion;
				auditDataFile.Serial_number = existingAsset.SerialNumber;
				auditDataFile.Domain = existingAsset.Domain;

				// Set asset category
				foreach (AssetType assetType in assetTypes)
				{
					if (assetType.AssetTypeID == existingAsset.AssetTypeID)
					{
						auditDataFile.Category = assetType.Name;
						break;
					}
				}

				// Now we add the audited items to the data file
				foreach (AuditedItem auditedItem in listAuditedItems)
				{
					auditDataFile.AuditedItems.Add(auditedItem);
				}

				// Now we need to save the audit data file to the Windows Temp Folder so that we have a physical file 
				// to move to the FTP site
				string tempFile = Path.Combine(tempPath, existingAsset.Name + ".ADF");
				auditDataFile.Write(tempFile);

				// Now upload to the FTP / SFTP site
				if ((auditScannerDefinition.UploadSetting == AuditScannerDefinition.eUploadSetting.ftp && auditScannerDefinition.FTPType == "FTP")
				||  (auditScannerDefinition.FTPCopyToNetwork && auditScannerDefinition.FTPTypeBackup == "FTP"))
				{
					UploadViaFTP(auditScannerDefinition, tempFile);
				}
				else
				{
					UploadViaSFTP(auditScannerDefinition, tempFile);
				}

			}
		}


		/// <summary>
		/// Called to upload the specified file to an FTP Site
		/// </summary>
		/// <param name="fileName"></param>
		protected void UploadViaFTP(AuditScannerDefinition auditScannerDefinition, string fileName)
		{
			// target file
			string targetFile = "";
			string uri = "";
			bool anonymous = false;
			string username = "";
			string password = "";

			// Get file information for the temporary file created
			FileInfo fileInfo = new FileInfo(fileName);

			// There are 2 methods by which we can specify an FTP upload 
			//	- Upload Setting could be set to FTP
			//  - Upload copy of audit data files could be checked
			//
			// These each have their own FTP settings and we must read the correct ones.
			if (auditScannerDefinition.UploadSetting == AuditScannerDefinition.eUploadSetting.ftp)
			{
				// Get target file taking note of any default folder specified for the upload
				if (!string.IsNullOrEmpty(auditScannerDefinition.FTPDefDir))
					targetFile = auditScannerDefinition.FTPDefDir + "/" + fileInfo.Name;
				else
					targetFile = fileInfo.Name;

				uri = "ftp://" + auditScannerDefinition.FTPSite + ":" + auditScannerDefinition.FTPPort.ToString() + "/" + targetFile;

				// Get login credentials
				anonymous = auditScannerDefinition.FTPAnonymous;
				username = auditScannerDefinition.FTPUser;
				password = auditScannerDefinition.FTPPassword;
			}

			else 
			{
				// Get target file taking note of any default folder specified for the upload
				if (!string.IsNullOrEmpty(auditScannerDefinition.FTPDefDirBackup))
					targetFile = auditScannerDefinition.FTPDefDirBackup + "/" + fileInfo.Name;
				else
					targetFile = fileInfo.Name;

				uri = "ftp://" + auditScannerDefinition.FTPSiteBackup + ":" + auditScannerDefinition.FTPPortBackup.ToString() + "/" + targetFile;

				// Get login credentials
				anonymous = auditScannerDefinition.FTPAnonymousBackup;
				username = auditScannerDefinition.FTPUserBackup;
				password = auditScannerDefinition.FTPPasswordBackup;
			}

			// Create the web request
			FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(uri);

			// Credentials
			if (anonymous)
			{
			}
			else
			{
				ftpWebRequest.Credentials = new System.Net.NetworkCredential(username, AES.DecryptFTPPassword(password));
			}

			// By default KeepAlive is true, where the control connection is not closed
			// after a command is executed.
			ftpWebRequest.KeepAlive = false;

			// Specify the command to be executed.
			ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;

			// Specify the data transfer type.
			ftpWebRequest.UseBinary = true;

			// Notify the server about the size of the uploaded file
			ftpWebRequest.ContentLength = fileInfo.Length;

			// The buffer size is set to 2kb
			int buffLength = 2048;
			byte[] buff = new byte[buffLength];
			int contentLen;

			// Opens a file stream (System.IO.FileStream) to read the file to be uploaded
			FileStream fs = fileInfo.OpenRead();
			try
			{
				// Stream to which the file to be upload is written
				Stream strm = ftpWebRequest.GetRequestStream();

				// Read from the file stream 2kb at a time
				contentLen = fs.Read(buff, 0, buffLength);

				// Till Stream content ends
				while (contentLen != 0)
				{
					// Write Content from the file stream to the FTP Upload Stream
					strm.Write(buff, 0, contentLen);
					contentLen = fs.Read(buff, 0, buffLength);
				}

				// Close the file stream and the Request Stream
				strm.Close();
				fs.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to upload SNMP Discovered Device audit file to the FTP Site " + auditScannerDefinition.FTPSiteBackup + ".  The error was " + ex.Message, "FTP Upload Error");
			}
		}




		/// <summary>
		/// 8.3.4 - CMD
		/// 
		/// Called to upload the specified file to an SFTP (SSH File Transfer Protocol) Site
		/// </summary>
		/// <param name="fileName"></param>
		protected void UploadViaSFTP(AuditScannerDefinition auditScannerDefinition, string tempFileName)
		{
			// target file
			string targetFile = "";
			string hostname = "";
			int port = 22;
			string username = "";
			string password = "";

			// Get file information for the temporary file created
			FileInfo fileInfo = new FileInfo(tempFileName);

			// Create the chilkat component and unlock it
			bool success;
			Chilkat.SFtp sftp = new Chilkat.SFtp();
			success = sftp.UnlockComponent("LAYTONSSH_pErKX4Vp3InH");
			if (success != true)
			{
				MessageBox.Show("Failed to upload SNMP Discovered Device audit file to the SFTP Site.\n\nThe error was " + sftp.LastErrorText, "SFTP Upload Error");
				return;
			}

			//  Set some timeouts, in milliseconds:
			sftp.ConnectTimeoutMs = 5000;
			sftp.IdleTimeoutMs = 15000;

			// There are 2 methods by which we can specify an FTP upload 
			//	- Upload Setting could be set to FTP
			//  - Upload copy of audit data files could be checked
			//
			// These each have their own FTP settings and we must read the correct ones.
			if (auditScannerDefinition.UploadSetting == AuditScannerDefinition.eUploadSetting.ftp)
			{
				// Get target file taking note of any default folder specified for the upload
				if (!string.IsNullOrEmpty(auditScannerDefinition.FTPDefDir))
					targetFile = auditScannerDefinition.FTPDefDir + "/" + fileInfo.Name;
				else
					targetFile = fileInfo.Name;

				hostname = auditScannerDefinition.FTPSite;
				port = auditScannerDefinition.FTPPort;

				// Get login credentials
				username = auditScannerDefinition.FTPUser;
				password = auditScannerDefinition.FTPPassword;
			}

			else 
			{
				// Get target file taking note of any default folder specified for the upload
				if (!string.IsNullOrEmpty(auditScannerDefinition.FTPDefDirBackup))
					targetFile = auditScannerDefinition.FTPDefDirBackup + "/" + fileInfo.Name;
				else
					targetFile = fileInfo.Name;

				hostname = auditScannerDefinition.FTPSiteBackup;
				port = auditScannerDefinition.FTPPortBackup;

				// Get login credentials
				username = auditScannerDefinition.FTPUserBackup;
				password = auditScannerDefinition.FTPPasswordBackup;
			}

			// Create the web request
			success = sftp.Connect(hostname,port);
			if (success != true)
			{
				MessageBox.Show("Failed to upload SNMP Discovered Device audit file to the SFTP Site.\n\nThe error was " + sftp.LastErrorText, "SFTP Upload Error");
				return;
			}

			// Authenticate with the SSH Server
			password = AES.DecryptFTPPassword(password);
			success = sftp.AuthenticatePw(username, password);
			if (success != true)
			{
				MessageBox.Show("Failed to upload SNMP Discovered Device audit file to the SFTP Site.\n\nThe error was " + sftp.LastErrorText, "SFTP Upload Error");
				return;
			}

			//  After authenticating, the SFTP subsystem must be initialized:
			success = sftp.InitializeSftp();
			if (success != true) 
			{
				MessageBox.Show("Failed to upload SNMP Discovered Device audit file to the SFTP Site.\n\nThe error was " + sftp.LastErrorText, "SFTP Upload Error");
				return;
			}

			//  Open a file on the server for writing.
			//  "createTruncate" means that a new file is created; if the file already exists, it is opened and truncated.
			string handle;
			handle = sftp.OpenFile(targetFile, "writeOnly", "createTruncate");
			if (handle == null ) 
			{
				MessageBox.Show("Failed to upload SNMP Discovered Device audit file to the SFTP Site.\n\nThe error was " + sftp.LastErrorText, "SFTP Upload Error");
				return;
			}

			
			// Upload the local file by name
			success = sftp.UploadFile(handle, tempFileName);
			if (success != true)
			{
				MessageBox.Show("Failed to upload SNMP Discovered Device audit file to the SFTP Site.\n\nThe error was " + sftp.LastErrorText, "SFTP Upload Error");
				return;
			}

			//  Close the file.
			sftp.CloseHandle(handle);
		}
    }
}
