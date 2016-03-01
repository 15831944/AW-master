using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;
using Layton.Common.Controls;

namespace Layton.AuditWizard.Common
{
    #region AuditFileInfo Class

    /// <summary>
    /// This class is a helper class to the main AuditUploader class and holds information about
    /// a specific audit data file which will assist in its uploading
    /// </summary>
    public class AuditFileInfo : IComparable<AuditFileInfo>
    {
        public enum eUploadStatus { newAsset, existingAsset, invalidFile };

        #region Data
        private string _filename;
        private string _assetname;
        private string _uniqueID;
        private eUploadStatus _status;
        private DateTime _auditDate;
        private AuditDataFile _auditDataFile;
        #endregion

        #region Propertites

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        public string UniqueID
        {
            get { return _uniqueID; }
            set { _uniqueID = value; }
        }

        public string Assetname
        {
            get { return _assetname; }
            set { _assetname = value; }
        }

        public eUploadStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public DateTime AuditDate
        {
            get { return _auditDate; }
            set { _auditDate = value; }
        }

        public AuditDataFile AuditFile
        {
            get { return _auditDataFile; }
            set { _auditDataFile = value; }
        }

        public string StatusAsText
        {
            get
            {
                switch ((int)_status)
                {
                    case (int)eUploadStatus.newAsset:
                        return "New";
                    case (int)eUploadStatus.existingAsset:
                        return "Existing";
                    case (int)eUploadStatus.invalidFile:
                        return "Invalid File";
                    default:
                        return "Invalid File";
                }
            }
        }

        #endregion

        #region Constructors

        public AuditFileInfo()
        {
            _status = eUploadStatus.invalidFile;
        }

        // explicit constructor
        AuditFileInfo(string filename, string uniqueID, string assetname, eUploadStatus status, DateTime auditDate)
        {
            _filename = filename;
            _uniqueID = uniqueID;
            _assetname = assetname;
            _status = status;
            _auditDate = auditDate;
            _auditDataFile = null;
        }
        #endregion

        /// <summary>
        /// The comparison function works on the Audit Date to ensure that the objects are in date order
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(AuditFileInfo obj)
        {
            return _auditDate.CompareTo(obj.AuditDate);
        }

    }
    #endregion AuditFileInfo Class

    #region AuditUploader Class

    /// <summary>
    /// This class handles the uploading of Audit Data Files into AuditWizard
    /// </summary>
    public class AuditUploader
    {
        /// <summary>The base folder for the uploader</summary>
        private string _folder;

        /// <summary>This is the product key in use</summary>
        int _licenseCount = 0;

        /// <summary>This object allows us to map the audited items to their corresponding display icons</summary>
        IconMappings _iconMappings;

        /// <summary>date/time of last upload</summary>
        private DateTime _uploadTime;

        /// <summary>List of current groups in the dartabase (used for the re-locate by IP address code)</summary>
        AssetGroupList _listGroups;

        // The asset type list
        AssetTypeList _assetTypes = new AssetTypeList();

        // ALL User data categories
        UserDataCategoryList _userDataCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Asset);

        // Over-write any user data if new values recovered
        bool _overWriteUserData;

        private enum BrowserType { Firefox, IE, Safari, Opera, Chrome } ;

        private BrowserType _defaultBrowser;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Constructor</summary>
        /// <param name="folder"></param>
        public AuditUploader(string folder, int licenseCount)
        {
            _folder = folder;
            _licenseCount = licenseCount;

            // We'll need the icon mappings so that we set the correct icon in the tables		
            AuditedItemsDAO lAuditedItemsDAO = new AuditedItemsDAO();
            _iconMappings = new IconMappings(lAuditedItemsDAO);

            // Populate the asset types list also so that we can determine the type of the asset audited
            _assetTypes.Populate();

            // Populate the user data categories and fields
            _userDataCategories.Populate();

            // recover upload settings
            SettingsDAO lwDataAccess = new SettingsDAO();
            _overWriteUserData = lwDataAccess.GetSettingAsBoolean(DatabaseSettingsKeys.Setting_OverwriteUserData, false);

        }

        /// <summary>
        /// This function will enumerate the possible audit data files located in the specified folder
        /// and will validate them.  Any valid files are added to the return list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int EnumerateFiles(List<AuditFileInfo> list)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(_folder);
                FileInfo[] rgFiles = di.GetFiles("*" + AuditDataFile._fileExtension);
                foreach (FileInfo fi in rgFiles)
                {
                    // Try and read the file as an audit data file, if we fail skip this file
                    AuditDataFile thisFile = new AuditDataFile();
                    string fullPath = fi.FullName;

                    // clean the file of any invalid characters
                    string xmlString;
                    using (TextReader reader = new StreamReader(fullPath))
                    {
                        xmlString = reader.ReadToEnd();
                    }

                    if (!XmlSanitizedString.CheckXmlString(xmlString))
                    {
                        XmlSanitizedString safeXml = new XmlSanitizedString(xmlString);
                        using (TextWriter writer = new StreamWriter(fullPath))
                        {
                            writer.WriteLine(safeXml.ToString());
                        }
                    }

                    if (!thisFile.Read(fullPath))
					{
						// CMD 8.3.5 - Invalid file so rename it!
						fi.MoveTo(fullPath + "_" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "" + DateTime.Now.Hour + "" + DateTime.Now.Minute + ".BAD");
                        continue;
					}
					
                    AuditFileInfo auditFileInfo = new AuditFileInfo();
                    auditFileInfo.Filename = fi.Name;
                    auditFileInfo.UniqueID = thisFile.Uniqueid;
                    auditFileInfo.Assetname = thisFile.AssetName;
                    auditFileInfo.AuditDate = thisFile.AuditDate;
                    auditFileInfo.AuditFile = thisFile;

                    // See if this asset already exists in the database
                    //int computerID = new AssetDAO().AssetFind(auditFileInfo.Assetname, auditFileInfo.UniqueID);
                    //auditFileInfo.Status = (computerID == 0) ? AuditFileInfo.eUploadStatus.newAsset : AuditFileInfo.eUploadStatus.existingAsset;
                    if (list != null) list.Add(auditFileInfo);
                }
            }

            catch (Exception ex)
            {
                logger.Error("Error in EnumerateFiles()", ex);
            }

            return list.Count;
        }


        #region Audit Upload Functions

        private static bool IsCurrentVersionOlder(string scannerVersionToCheck, string latestVersion)
        {
            if (scannerVersionToCheck.Equals(latestVersion))
                return false;

            // e.g. currentVersion = 8.0.15.0, latestVersion = 8.2
            string[] lLatestAppVersion = latestVersion.Split('.');
            string[] lCurrentAppVersion = scannerVersionToCheck.Split('.');

            bool lCurrentVersionOlder = Convert.ToInt32(lLatestAppVersion[0]) > Convert.ToInt32(lCurrentAppVersion[0]);

            if (!lCurrentVersionOlder)
            {
                lCurrentVersionOlder =
                    (Convert.ToInt32(lLatestAppVersion[0]) == Convert.ToInt32(lCurrentAppVersion[0])) &&
                    (Convert.ToInt32(lLatestAppVersion[1]) > Convert.ToInt32(lCurrentAppVersion[1]));
            }

            return lCurrentVersionOlder;
        }

        private static void CheckAuditDataFileVersion(AuditDataFile aAuditDataFile)
        {
            if (aAuditDataFile.CreatedBy != AuditDataFile.CREATEDBY.awscanner)
                return;

            // if the version is older than current then check if the user has selected auto-update (update if so)
            // v8.1 is the minimum requirement - we need users to be on at least this version so won't upload anything before 8.1
            // as long as they are on >= 8.1 we will upload but check if auto-update is enabled as well
            const string latestScannerVersion = "8.1";
            string scannerVersionToCheck = aAuditDataFile.AgentVersion;
            scannerVersionToCheck = scannerVersionToCheck.Substring(scannerVersionToCheck.StartsWith("AuditAgent") ? 12 : 14);

            // if this is the latest version, return and continue
            if (IsCurrentVersionOlder(scannerVersionToCheck, latestScannerVersion) && (aAuditDataFile.CreatedBy == AuditDataFile.CREATEDBY.awscanner))
            {
                // rename this file so we don't keep trying to upload
                FileInfo fi = new FileInfo(aAuditDataFile.FileName);
                fi.MoveTo(aAuditDataFile.FileName + "_" +
                    DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "" + DateTime.Now.Hour + "" + DateTime.Now.Minute + ".oldversion");

                // the ADF has come from an older version so update if requested (and if AuditAgent)
                throw new Exception(
                        String.Format("{0} has been created using an older version of the scanner. Please deploy the " +
                        "new version to this machine.", aAuditDataFile.FileName));
            }
        }

        //private static void CheckAuditDataFileVersion(AuditDataFile aAuditDataFile)
        //{
        //    // if the version is older than current then check if the user has selected auto-update (update if so)
        //    // v8.1 is the minimum requirement - we need users to be on at least this version so won't upload anything before 8.1
        //    // as long as they are on >= 8.1 we will upload but check if auto-update is enabled as well
        //    const string latestScannerVersion = "8.2";
        //    string scannerVersionToCheck = aAuditDataFile.AgentVersion;
        //    scannerVersionToCheck = scannerVersionToCheck.Substring(scannerVersionToCheck.StartsWith("AuditAgent") ? 12 : 14);

        //    // if this is the latest version, return and continue
        //    if (IsCurrentVersionOlder(scannerVersionToCheck, latestScannerVersion) && (aAuditDataFile.CreatedBy == AuditDataFile.CREATEDBY.awscanner))
        //    {
        //        // ok we are here with an older version of the scanner/agent
        //        // firstly check if it's an agent or scanner
        //        bool isAgent = aAuditDataFile.AgentVersion.StartsWith("AuditAgent");

        //        // if this version is pre-8.1 then we need to rename the file as OLDVERSION
        //        if (IsCurrentVersionOlder(scannerVersionToCheck, "8.1"))
        //        {
        //            // rename this file so we don't keep trying to upload
        //            FileInfo fi = new FileInfo(aAuditDataFile.FileName);
        //            fi.MoveTo(aAuditDataFile.FileName + "_" +
        //                DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "" + DateTime.Now.Hour + "" + DateTime.Now.Minute + ".oldversion");

        //            // the ADF has come from an older version so update if requested (and if AuditAgent)
        //            if (new SettingsDAO().GetSettingAsBoolean("UpdateAgent", false) && isAgent)
        //            {
        //                // update the agent
        //                int lAssetId = new AssetDAO().AssetFind(new Asset(aAuditDataFile));

        //                // remove and deploy
        //                Operation newOperation = new Operation(lAssetId, Operation.OPERATION.removeagent);
        //                newOperation.Add();
        //                System.Threading.Thread.Sleep(5000);
        //                newOperation = new Operation(lAssetId, Operation.OPERATION.deployagent);
        //                newOperation.Add();

        //                throw new Exception(
        //                    String.Format("{0} has been created using an older version of the scanner. The latest version of " +
        //                    "the scanner has been automatically deployed and will now reaudit the machine.", aAuditDataFile.FileName));
        //            }
        //            else
        //            {
        //                throw new Exception(
        //                    String.Format("{0} has been created using an older version of the scanner. Please deploy the " +
        //                    "new version to this machine.", aAuditDataFile.FileName));
        //            }
        //        }
        //        else
        //        {
        //            // must be dealing with an 8.1 version so carry on but update agent if requested
        //            // the ADF has come from an older version so update if requested (and if AuditAgent)
        //            if (new SettingsDAO().GetSettingAsBoolean("UpdateAgent", false) && isAgent)
        //            {
        //                // update the agent
        //                int lAssetId = new AssetDAO().AssetFind(new Asset(aAuditDataFile));

        //                // remove and deploy
        //                Operation newOperation = new Operation(lAssetId, Operation.OPERATION.removeagent);
        //                newOperation.Add();
        //                System.Threading.Thread.Sleep(5000);
        //                newOperation = new Operation(lAssetId, Operation.OPERATION.deployagent);
        //                newOperation.Add();
        //            }
        //        }

        //    }
        //}
        /// <summary>
        /// Checks and delete if an asset is falsely identified in SNMP scan 
        /// </summary>
        /// <param name="objAsset"></param>
        public void CheckSNMPDuplicatedAssets(Asset objAsset)
        {
            AssetDAO lAssetDAO = new AssetDAO();
            int iAssetID = lAssetDAO.AssetFind(objAsset);
            if (iAssetID != 0)
            {
                Asset existingAsset = lAssetDAO.AssetGetDetails(iAssetID);             
                if (existingAsset.IPAddress == objAsset.IPAddress && existingAsset.AgentVersion == "SNMP")
                {
                    lAssetDAO.AssetDelete(iAssetID);
                }
                else if ((existingAsset.IPAddress == objAsset.IPAddress) && ((existingAsset.Domain == "UNKNOWN") && (existingAsset.Domain != objAsset.Domain)))
                {                   
                    lAssetDAO.AssetDelete(iAssetID);
                }
            }          

        }

        /// <summary>
        /// Uploads an AuditDataFile
        /// </summary>
        /// <param name="aAuditDataFile"></param>
        public void UploadAuditDataFile(AuditDataFile aAuditDataFile)
        {
            CheckAuditDataFileVersion(aAuditDataFile);

            // Database connection
            AssetDAO lAssetDao = new AssetDAO();

            // Recover the count of (licensable) assets which have been uploaded so far
            int licensesInUse = lAssetDao.LicensedAssetCount();

            // Create a 'Asset' object for the audited computer - note that this will include such
            // things as the make, model, serial etc
            Asset auditedAsset = new Asset(aAuditDataFile);

            // JML TODO - this is a temp fix to get the USB parent id
            // this whole process needs to be reworked
            if ((aAuditDataFile.CreatedBy == AuditDataFile.CREATEDBY.usbscanner) || (aAuditDataFile.CreatedBy == AuditDataFile.CREATEDBY.mdascanner))
            {
                auditedAsset.ParentAssetID = new AssetDAO().AssetIDByAssetName(aAuditDataFile.ParentAssetName);
            }

            // Take a look at the domain name specified and ensure that a domain of this name exists and if not 
            // create one.
            AssetGroup rootGroup = new AssetGroup(AssetGroup.GROUPTYPE.domain);
            rootGroup.GroupID = 1;
            AssetGroupList domains = new AssetGroupList(new LocationsDAO().GetGroups(rootGroup), AssetGroup.GROUPTYPE.domain);
            string auditedDomain = aAuditDataFile.Domain;

            // Does the domain have a period delimiter? (MYDOMAIN.LOCAL or similar)  If so we will strip
            // off after the delimiter 
            int delimiter = auditedDomain.IndexOf('.');
            if (delimiter != -1)
                auditedDomain = auditedDomain.Substring(0, delimiter);

            // Now see if this domain already exists
            AssetGroup auditedDomainGroup = domains.FindGroup(auditedDomain);
            if (auditedDomainGroup == null)
            {
                auditedDomainGroup = new AssetGroup(AssetGroup.GROUPTYPE.domain);
                auditedDomainGroup.Name = auditedDomain;
                auditedDomainGroup.ParentID = rootGroup.GroupID;
                auditedDomainGroup.Add();
            }

            //Comments from Sojan E John KTS Infotech
            //It may be possible that PC's enabled with SNMP added to database as SNMP asset
            //A work around for this is to delete them at the time of upload and add as a new asset to the database
            Asset objTmpAsset=new Asset();
            objTmpAsset.Name = auditedAsset.Name;
            objTmpAsset.IPAddress = auditedAsset.IPAddress;
            objTmpAsset.AgentVersion = "DUP";
            objTmpAsset.Domain = auditedAsset.Domain;
            CheckSNMPDuplicatedAssets(objTmpAsset);


            // Initially we just check to see if the asset already exists in the database
            auditedAsset.AssetID = lAssetDao.AssetFind(auditedAsset);

            // If the asset DOES exist then we need to recover its previously audited details as we need to be 
            // careful that we do not over-write any changes made to the basic attributes - such as type, make, 
            // model etc unless of course the asset has not been audited yet.  
            //
            // Type is the hardest but we basically say that if the audit file states anything other than PC 
            // then it can over-write an existing value of PC
            Asset existingAsset = null;
            if (auditedAsset.AssetID != 0)
                existingAsset = lAssetDao.AssetGetDetails(auditedAsset.AssetID);

            // OK so now we know if this asset already existed and its details if it did...
            // License checking - we cannot upload an audit of a new asset or one which has not previously
            // been audited without checking the license counts first
            if ((auditedAsset.AssetID == 0) || (existingAsset.LastAudit.Ticks == 0))
            {
                if (licensesInUse >= _licenseCount)
                {
                    // We can't leave the file behind as this could cause a loop as we keep trying to upload it 
                    // so instead we will rename the upload file
                    string badFile = aAuditDataFile.FileName.Replace(Path.GetExtension(aAuditDataFile.FileName), ".NOLICENSES");
                    File.Move(aAuditDataFile.FileName, badFile);

                    NewsFeed.AddNewsItem(NewsFeed.Priority.Fatal, "AuditWizard License count has been exceeded.");

                    // ...and throw an exception to inform the caller that we have run out of licenses
                    throw new Exception("Failed to upload the audit file [" + aAuditDataFile.FileName + "].  License Count Exceeded. Please purchase additional licenses for AuditWizard. The file has been renamed to prevent any attempt to upload the file again");
                }
            }            

            // If the asset being uploaded does not exist yet then create it
            if (auditedAsset.AssetID == 0)
            {
                // We have assumed that this asset is a PC but we had best check that with the category 
                // held in the audit data file
                AssetType assetType = _assetTypes.FindByName(aAuditDataFile.Category);
                if (assetType == null)
                {
                    // We need to create a new type of asset ensuring that we parent it correctly so first get our 
                    // parent category.
                    AssetType parentAssetType;
                    switch (aAuditDataFile.CreatedBy)
                    {
                        case AuditDataFile.CREATEDBY.awscanner:
                            parentAssetType = _assetTypes.FindByName("Computers");
                            break;
                        case AuditDataFile.CREATEDBY.mdascanner:
                            parentAssetType = _assetTypes.FindByName("Mobile Devices");
                            break;
                        default:
                            parentAssetType = _assetTypes.FindByName("USB Devices");
                            break;
                    }

                    // Now create a child of this asset type						
                    assetType = new AssetType();
                    assetType.Name = aAuditDataFile.Category;
                    assetType.Auditable = false;
                    assetType.Icon = parentAssetType.Icon;
                    assetType.ParentID = parentAssetType.AssetTypeID;
                    assetType.Add();

                    // Update the internal list
                    _assetTypes.Add(assetType);
                }

                // Set the domain for this asset
                auditedAsset.DomainID = auditedDomainGroup.GroupID;

                // Set the asset type ID
                auditedAsset.AssetTypeID = assetType.AssetTypeID;

                int lLocationId = FindLocation(auditedAsset.Location);

                if (lLocationId != 0) auditedAsset.LocationID = lLocationId;

                // This is a new asset however we need to first ensure that we do not exceed the permitted asset count
                auditedAsset.Add();
            }

            // Uploading an existing asset
            else
            {
                // Asset type - over-write any existing value of 'PC' with the audited type
                if (existingAsset.TypeAsString == "PC")
                {
                    AssetType assetType = _assetTypes.FindByName(aAuditDataFile.Category);
                    existingAsset.AssetTypeID = assetType.AssetTypeID;
                }

                // Update the audited asset with any values actually held in the existing asset such as the asset type,
                // make, model etc
                //keep the values of existing asset and audited asset names for creating an ate
                string strOldAssetName = existingAsset.Name;
                string strNewAssetName = auditedAsset.Name;                
                if (existingAsset.OverwriteData)
                {
                    existingAsset.Name = auditedAsset.Name;
                    existingAsset.MACAddress = auditedAsset.MACAddress.Replace('-', ':');
                    existingAsset.Make = auditedAsset.Make;
                    existingAsset.Model = auditedAsset.Model;
                    existingAsset.IPAddress = auditedAsset.IPAddress;
                    existingAsset.DomainID = auditedDomainGroup.GroupID;
                    existingAsset.SerialNumber = auditedAsset.SerialNumber;
                    existingAsset.AssetTypeID = auditedAsset.AssetTypeID;
                    existingAsset.TypeAsString = auditedAsset.TypeAsString;
                    existingAsset.AssetTag = auditedAsset.AssetTag;
                }

                existingAsset.AgentVersion = auditedAsset.AgentVersion;
                existingAsset.UniqueID = auditedAsset.UniqueID;
                // existingAsset.AssetTag = auditedAsset.AssetTag;

                int lLocationId = FindLocation(auditedAsset.Location);

                if (lLocationId != 0)
                    existingAsset.LocationID = lLocationId;

                //if (auditedAsset.Location != String.Empty)
                //{
                //    int lLocationID = new LocationsDAO().LocationFindByName(auditedAsset.Location);

                //    if (lLocationID != 0)
                //        existingAsset.LocationID = lLocationID;
                //}

                // Copy the existing asset definition to the audited asset
                auditedAsset = new Asset(existingAsset);

                // Update any of the basic attributes which may have changed
                auditedAsset.Update();

                //Check whether there is a name change if there is one add an ate entry
                if (existingAsset.OverwriteData)
                {
                    if (strOldAssetName != strNewAssetName)
                    {
                        AuditTrailDAO objAuditTrailDAO = new AuditTrailDAO();
                        AuditTrailEntry objAuditTrailEntry = new AuditTrailEntry();
                        objAuditTrailEntry.Date = DateTime.Now;
                        objAuditTrailEntry.Class = AuditTrailEntry.CLASS.asset;
                        objAuditTrailEntry.Type = AuditTrailEntry.TYPE.changed;
                        objAuditTrailEntry.AssetID = auditedAsset.AssetID;
                        objAuditTrailEntry.AssetName = auditedAsset.Name; ;
                        objAuditTrailEntry.Username = System.Environment.UserName;
                        objAuditTrailEntry.Key = auditedAsset.Name + "|" + "Computer Name";
                        objAuditTrailEntry.OldValue = strOldAssetName;
                        objAuditTrailEntry.NewValue = strNewAssetName;
                        objAuditTrailDAO.AuditTrailAdd(objAuditTrailEntry);
                    }
                }

                if (new SettingsDAO().GetSettingAsBoolean("NewsFeedUpdateAsset", false))
                    NewsFeed.AddNewsItem(NewsFeed.Priority.Information, "Updated asset (" + auditedAsset.Name + ").");
            }

            // Set the upload time - this will be used for all subsequent history entries
            _uploadTime = aAuditDataFile.AuditDate;

            // Is this a first or a re-audit of this asset - create a history entry accordingly
            AuditTrailEntry ate = new AuditTrailEntry();
            ate.AssetID = auditedAsset.AssetID;
            ate.AssetName = auditedAsset.Name;
            ate.Type = AuditTrailEntry.TYPE.added;
            ate.Date = _uploadTime;
            ate.Key = ate.OldValue = ate.NewValue = "";
            ate.Class = (auditedAsset.LastAudit.Ticks == 0L) ? AuditTrailEntry.CLASS.audited : AuditTrailEntry.CLASS.reaudited;

            AuditTrailDAO lAuditTrailDAO = new AuditTrailDAO();
            lAuditTrailDAO.AuditTrailAdd(ate);

            // Create instances for the baseline data
            AuditedItemList baselineAuditedItems = new AuditedItemList();
            ApplicationInstanceList baselineAuditedApplications = new ApplicationInstanceList();
            OSInstance baselineOS = null;

            // Before we upload, generate the baseline for history
            SetHistoryBaseline(auditedAsset, baselineAuditedItems, baselineAuditedApplications, baselineOS);

            // Perform the upload of the audit data file
            try
            {
                // Upload User Defined Data Fields
                UploadUserDefinedData(aAuditDataFile, auditedAsset);

                // Upload Audited Data (Hardware / System / Internet)
                UploadAuditedDataItems(aAuditDataFile, auditedAsset);

                // Upload Internet Explorer Information
                UploadInternetItems(aAuditDataFile, auditedAsset);

                // Upload Applications
                UploadAuditedApplications(aAuditDataFile, auditedAsset);

                // Upload any File System Information
                UploadFileSystem(aAuditDataFile, auditedAsset);

                // Flag in the database that the computer was audited
                lAssetDao.AssetAudited(auditedAsset.AssetID, _uploadTime);                

            }
            catch (Exception ex)
            {
                LogFile ourLog = LogFile.Instance;
                ourLog.Write("An exception occurred during the upload of [" + aAuditDataFile.FileName + "], the exception was [" + ex.Message + "]. This file will be ignored", true);
            }

            // Based on the IP address of the asset, see if we need to set its location
            RelocateByIPAddress(auditedAsset);

            // Cleanup operation after the upload to tidy up the database
            PostAuditCleanup(auditedAsset);

            // Generate the audit trail from the old and current information
            GenerateAuditTrail(aAuditDataFile, auditedAsset, baselineAuditedItems, baselineAuditedApplications, baselineOS);

            // Deal with the audit date file after upload
            HandleUploadedAuditDataFile(aAuditDataFile);

            // Check to see if anything needs to be written to new feed
            ProcessNewsFeed(auditedAsset);
        }

        private void ProcessNewsFeed(Asset auditedAsset)
        {
            CheckNewApplicationLicenses(auditedAsset.AssetID);
            CheckNewDiskUsage(auditedAsset.AssetID, auditedAsset.Name);
            CheckNewPrinterLevels(auditedAsset.AssetID, auditedAsset.Name);
        }

        private void CheckNewApplicationLicenses(int assetID)
        {
            DataTable dataTable = new StatisticsDAO().CheckForNewLicenseViolations(assetID);

            foreach (DataRow row in dataTable.Rows)
            {
                string applicationName = row.ItemArray[0].ToString();
                int licenseUsagePercentage = Convert.ToInt32(row.ItemArray[2]);

                NewsFeed.AddNewsItem(NewsFeed.Priority.Warning,
                    String.Format("{0} is using {1}% of designated licenses.", applicationName, licenseUsagePercentage));
            }
        }

        private void CheckNewPrinterLevels(int assetID, string aAssetName)
        {
            DataTable dataTable = new StatisticsDAO().CheckNewPrinterLevels(assetID, aAssetName);

            foreach (DataRow row in dataTable.Rows)
            {
                string printerName = row.ItemArray[1].ToString();
                int supplyLevelRemaining = Convert.ToInt32(row.ItemArray[2]);

                NewsFeed.AddNewsItem(NewsFeed.Priority.Warning,
                    String.Format("{0} supply level is {1}", printerName, supplyLevelRemaining));
            }
        }

        private void CheckNewDiskUsage(int assetID, string assetName)
        {
            DataTable resultsDataTable = new DataTable();
            StatisticsDAO _statisticsDAO = new StatisticsDAO();
            string lDiskSpace = new SettingsDAO().GetSetting("NewsFeedDiskSpace", false);

            lDiskSpace = (lDiskSpace == "") ? "25" : lDiskSpace;
            double diskSpaceThreshold = Convert.ToDouble(lDiskSpace);

            DataTable uniqueDrivesDataTable = _statisticsDAO.UniqueDriveLettersForAsset(assetID);
            DataTable serverDriveDataTable = _statisticsDAO.StatisticsServerData(assetID);
            DataColumn dataColumn;
            DataRow[] serverDriveRows;
            double percentageFree;

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Drive Letter";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.Int32");
            dataColumn.ColumnName = "Used Space (GB)";
            resultsDataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.DataType = Type.GetType("System.Int32");
            dataColumn.ColumnName = "Remaining Space (GB)";
            resultsDataTable.Columns.Add(dataColumn);

            try
            {
                foreach (DataRow driveLetterRow in uniqueDrivesDataTable.Rows)
                {
                    string driveLetter = (string)driveLetterRow.ItemArray[1];

                    serverDriveRows = serverDriveDataTable.Select("_CATEGORY = '" + driveLetter + "'");

                    if (serverDriveRows[6].ItemArray[3].ToString() != "Fixed Drive")
                        continue;

                    int totalSpace = Convert.ToInt32(serverDriveRows[4].ItemArray[3]);
                    int freeSpace = Convert.ToInt32(serverDriveRows[5].ItemArray[3]);
                    string assetDriveLetter = serverDriveRows[3].ItemArray[3].ToString();

                    percentageFree = (double)freeSpace / totalSpace;
                    percentageFree = double.Parse(percentageFree.ToString("####0.00")) * 100;

                    if (percentageFree < diskSpaceThreshold)
                    {
                        NewsFeed.AddNewsItem(NewsFeed.Priority.Warning,
                            String.Format("Drive {0} on {1} has {2}% space remaining.", assetDriveLetter, assetName, percentageFree));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private int FindLocation(string aAssetLocation)
        {
            int lLocationID = 0;

            try
            {
                AssetGroup assetGroup = new AssetGroup();

                DataTable table = new LocationsDAO().GetGroups(new AssetGroup(AssetGroup.GROUPTYPE.userlocation));
                //AssetGroup parentGroup = new AssetGroup(row, AssetGroup.GROUPTYPE.userlocation);

                AssetGroup rootLocation = new AssetGroup(table.Rows[0], AssetGroup.GROUPTYPE.userlocation);

                string fullLocationName = rootLocation.FullName + @"\";

                // split the audited location into it's components
                string[] locations = aAssetLocation.Split(';');

                foreach (string location in locations)
                {
                    fullLocationName += location + @"\";
                }

                fullLocationName = fullLocationName.Substring(0, fullLocationName.Length - 1);

                // there are two options
                if (aAssetLocation != String.Empty)
                {
                    lLocationID = new LocationsDAO().LocationFind(fullLocationName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return lLocationID;
        }

        /// <summary>
        /// Set the baseline for the audit of a specific computer
        /// </summary>
        /// <param name="baselineAuditedItems"></param>
        /// <param name="baselineAuditedApplications"></param>
        protected void SetHistoryBaseline(Asset auditedComputer, List<AuditedItem> baselineAuditedItems, ApplicationInstanceList baselineAuditedApplications, OSInstance baselineOS)
        {
            AuditedItemsDAO lwDataAccess = new AuditedItemsDAO();

            // Recover a list of the audited data items currently present for this computer 
            DataTable auditedItemsTable = lwDataAccess.GetAuditedItems(auditedComputer.AssetID, "", true);
            foreach (DataRow row in auditedItemsTable.Rows)
            {
                baselineAuditedItems.Add(new AuditedItem(row));
            }

            // Recover a list of the applications which are currently installed on this computer
            // so that we can generate an audit trail of any changes which have taken place
            //
            // NOTE: this list includes ALL applications even those which are hidden
            DataTable applicationsTable = new ApplicationsDAO().GetInstalledApplications(auditedComputer, "", true, true);
            foreach (DataRow row in applicationsTable.Rows)
            {
                baselineAuditedApplications.Add(new ApplicationInstance(row));
            }

            // ...and also add the OS for this computer to the list
            DataTable osTable = new ApplicationsDAO().GetInstalledOS(auditedComputer);
            if (osTable.Rows.Count != 0)
                baselineOS = new OSInstance(osTable.Rows[0]);

            // OK now we can perform a tidyup of the existing database entries for this asset as we have established
            // the baseline which we will use to generate history from.
            PreAuditCleanup(auditedComputer);
        }


        /// <summary>
        /// UploadUserDefinedData 
        /// </summary>
        /// <param name="auditDataFile"></param>
        /// <param name="auditedComputer"></param>
        protected void UploadUserDefinedData(AuditDataFile auditDataFile, Asset auditedComputer)
        {
            // Get the list of audited items which we will have to upload for this asset
            List<AuditedItem> auditedItems = auditDataFile.AuditedUserDataItems;

            // Determine which categories and fields apply to this asset
            foreach (UserDataCategory category in _userDataCategories)
            {
                // Is this category applicable to this asset?  
                // If so add a new tab to the control
                if (category.CategoryAppliesTo(auditedComputer))
                {
                    category.GetValuesFor(auditedComputer.AssetID);
                }
            }

            // Iterate through these items and add each to the database
            foreach (AuditedItem thisItem in auditedItems)
            {
                // Set the asset id in the audited item object
                thisItem.AssetID = auditedComputer.AssetID;

                // We need to find the specific category/field so that we may update the value
                UserDataCategory category = _userDataCategories.FindCategory(thisItem.Category);
                if (category == null)
                    continue;

                // Now the sepcific field in this category
                UserDataField field = category.FindField(thisItem.Name);
                if (field == null)
                    continue;

                // Update the value of this field for the specified asset depending on
                //
                //	1> If the field already has a value
                //  2> If a new value has been specified
                //  3> Whether we are to over-write user data in Upload Settings
                if ((thisItem.Value != "") && (_overWriteUserData))
                    field.SetValueFor(auditedComputer.AssetID, thisItem.Value, true);
            }
        }


        /// <summary>
        /// UploadAuditedDataItems
        /// ======================
        /// 
        /// This is called as part of the upload process to upload the audited data items for this
        /// computer into the database and generate history as required
        /// </summary>
        /// <param name="auditDataFile"></param>
        /// <param name="auditedComputer"></param>
        protected void UploadAuditedDataItems(AuditDataFile auditDataFile, Asset auditedComputer)
        {
            try
            {
                UserDataDefinitionsDAO lUserDataDefinitionsDAO = new UserDataDefinitionsDAO();
                bool assignedUserSet;

                int lAssetID = auditedComputer.AssetID;
                int lAssignedUserId = lUserDataDefinitionsDAO.GetAssignedUserId();

                if (lAssignedUserId != -1)
                    assignedUserSet = lUserDataDefinitionsDAO.AssignedUserPopulated(lAssetID, lAssignedUserId);
                else
                    assignedUserSet = true;

                foreach (AuditedItem item in auditDataFile.AuditedItems)
                {
                    item.Icon = _iconMappings.GetIconMapping(item.Category).Icon;
                    item.AssetID = lAssetID;

                    // Bug #444
                    if (item.Category.StartsWith("Hardware|Disk Drives|"))
                    {
                        if (!item.Category.EndsWith("\\"))
                            item.Category += "\\";

                        if (item.Name == "Drive Letter")
                        {
                            if (!item.Value.EndsWith("\\"))
                                item.Value += "\\";
                        }
                    }

                    if (assignedUserSet)
                        continue;

                    if (item.Category != "Hardware|Network" || item.Name != "Username")
                        continue;

                    string username = item.Value;

                    if (item.Value.Contains("\\"))
                    {
                        username = username.Substring(username.IndexOf("\\") + 1);
                        lUserDataDefinitionsDAO.UserDataUpdateValue(UserDataCategory.SCOPE.NonInteractiveAsset, lAssetID, lAssignedUserId, username);
                    }
                    else if (item.Value.Contains("@"))
                    {
                        username = username.Substring(0, username.IndexOf("@"));
                        lUserDataDefinitionsDAO.UserDataUpdateValue(UserDataCategory.SCOPE.NonInteractiveAsset, lAssetID, lAssignedUserId, username);
                    }

                    assignedUserSet = true;
                }

                new AuditedItemsDAO().AuditedItemAdd(auditDataFile.AuditedItems);

            }
            catch (Exception ex)
            {
                logger.Error("Error in UploadAuditedDataItems()", ex);
            }
        }


        /// <summary>
        /// UploadAuditedApplications
        /// =========================
        /// 
        /// This is called as part of the upload process to upload the audited applications for this
        /// computer into the database and generate history as required
        /// </summary>
        /// <param name="auditDataFile"></param>
        /// <param name="auditedComputer"></param>
        protected void UploadAuditedApplications(AuditDataFile auditDataFile, Asset auditedComputer)
        {
            try
            {
                ApplicationsDAO lApplicationsDao = new ApplicationsDAO();
                int applicationId, instanceId;
                OSInstance auditedOs = auditDataFile.AuditedOS;

                foreach (ApplicationInstance thisApplication in auditDataFile.AuditedApplications)
                {
                    lApplicationsDao.ApplicationAddInstance(auditedComputer.AssetID, thisApplication, out applicationId, out instanceId);

                    // special check for Internet Browsers which are added to AUDITEDITEMS
                    if (IsInternetBrowser(thisApplication.Name))
                    {
                        string category = "Internet|" + thisApplication.Name;
                        string name = "Default Browser";
                        string value = (GetBrowserType(thisApplication.Name) == _defaultBrowser) ? "Yes" : "No";
                        UploadBrowserAuditedItem(auditedComputer, thisApplication, category, name, value);

                        category = "Internet|" + thisApplication.Name;
                        name = "Version";
                        value = thisApplication.Version;
                        UploadBrowserAuditedItem(auditedComputer, thisApplication, category, name, value);
                    }
                }

                // Add in the OS if we have recovered one
                if (auditedOs.Name != "")
                    lApplicationsDao.OSAddInstance(auditedComputer.AssetID, auditedOs, out applicationId, out instanceId);

            }
            catch (Exception ex)
            {
                logger.Error("Error in UploadAuditedApplications()", ex);
            }
        }

        private static void UploadBrowserAuditedItem(Asset auditedComputer, ApplicationInstance thisApplication, string category, string name, string value)
        {
            BrowserType browserType = GetBrowserType(thisApplication.Name);

            AuditedItem auditedItem = new AuditedItem();
            auditedItem.AssetID = auditedComputer.AssetID;
            auditedItem.Category = category;
            auditedItem.Name = name;
            auditedItem.Value = value;
            auditedItem.Grouped = false;
            auditedItem.Historied = false;
            auditedItem.Datatype = AuditedItem.eDATATYPE.text;
            auditedItem.DisplayUnits = "";

            switch (browserType)
            {
                case BrowserType.IE:
                    auditedItem.Icon = "ie.png";
                    break;
                case BrowserType.Opera:
                    auditedItem.Icon = "opera.png";
                    break;
                case BrowserType.Safari:
                    auditedItem.Icon = "safari.png";
                    break;
                case BrowserType.Chrome:
                    auditedItem.Icon = "chrome.png";
                    break;
                case BrowserType.Firefox:
                    auditedItem.Icon = "firefox.png";
                    break;
                default:
                    auditedItem.Icon = "ie.png";
                    break;
            }

            new AuditedItemsDAO().AuditedItemAdd(auditedItem);
        }

        private static bool IsInternetBrowser(string appName)
        {
            return
                appName.Contains("Mozilla Firefox") ||
                appName.Contains("Google Chrome") ||
                appName.Equals("Internet Explorer") ||
                appName.Contains("Safari") ||
                appName.Contains("Opera");
        }


        /// <summary>
        /// UploadInternetItems
        /// ===================
        /// 
        /// This is called as part of the upload process to upload the Internet items for this
        /// computer into the database.
        /// 
        /// Internet items are arranged with the URL as part of the category string and the date (and other attributes)
        /// as items within the class - for example
        /// 
        /// <auditeditem class="Internet|Cookie|scooterforum.net/" historied="false" grouped="true">
        ///		<item name="Last Accessed On" value="01/06/2009 16:21" /> 
        /// </auditeditem>
        /// 
        /// and...
        /// 
        ///	<auditeditem class="Internet|History|01/06/2009|www.howtomendit.com" historied="false" grouped="true">
        ///		<item name="Number of Pages" value="1" /> 
        ///	</auditeditem>
        ///
        /// We need to massage this data as we store it within the database as items by date.  For example if we 
        /// have two web pages accessed on one day, this would be stored in the database as 
        ///
        ///		_CATEGORY									_NAME				_VALUE
        ///		Internet|History|07/01/2009|www.xe.com		Number of Pages		5
        ///		Internet|History|07/01/2009|www.ebay.com	Number of Pages		5
        /// 
        /// </summary>
        /// <param name="auditDataFile"></param>
        /// <param name="auditedComputer"></param>
        protected void UploadInternetItems(AuditDataFile auditDataFile, Asset auditedComputer)
        {
            // Database connection
            AuditedItemsDAO lAuditedItemsDao = new AuditedItemsDAO();

            // Get the list of audited items which we will have to upload for this asset
            List<AuditedItem> auditedItems = auditDataFile.AuditedInternetItems;

            // Iterate through these items and add each to the database
            foreach (AuditedItem auditedItem in auditedItems)
            {
                if (auditedItem.Category == "Internet|Browsers|Default Browser")
                {
                    _defaultBrowser = GetBrowserType(auditedItem.Value);
                    continue;
                }

                // Set the asset id in the audited item object
                auditedItem.AssetID = auditedComputer.AssetID;
                auditedItem.Icon = "ie.png";

                auditedItem.ItemID = lAuditedItemsDao.AuditedItemAdd(auditedItem);
            }
        }

        private static BrowserType GetBrowserType(string browserPath)
        {
            if (browserPath.Contains("Mozilla Firefox"))
                return BrowserType.Firefox;
            else if (browserPath.Contains("Safari"))
                return BrowserType.Safari;
            else if (browserPath.Contains("Opera"))
                return BrowserType.Opera;
            else if (browserPath.Contains("Chrome"))
                return BrowserType.Chrome;
            else
                return BrowserType.IE;
        }

        /// <summary>
        /// UploadFileSystem
        /// ================
        /// 
        /// This is called as part of the upload process to upload any File System information for this
        /// computer into the database.
        /// 
        /// 
        /// </summary>
        /// <param name="auditDataFile"></param>
        /// <param name="auditedAsset"></param>
        protected void UploadFileSystem(AuditDataFile auditDataFile, Asset auditedComputer)
        {
            // Get the list of folders audited 
            FileSystemFolderList auditedFolders = auditDataFile.AuditedFolders;

            // Iterate through each of these top level folders, set the parent asset id and call their Save method
            // which will write them and all their children to the database
            foreach (FileSystemFolder folder in auditedFolders)
            {
                // Set the asset id in the audited item object
                folder.AssetID = auditedComputer.AssetID;
                folder.Save();
            }
        }


        /// <summary>
        /// Called to clean up audit records for a computer prior to it being (re)audited
        /// </summary>
        protected void PreAuditCleanup(Asset auditedComputer)
        {
            AuditedItemsDAO lwDataAccess = new AuditedItemsDAO();

            // Delete all currently audited items for this computer
            lwDataAccess.AuditedItemsDelete(auditedComputer.AssetID);

            // Delete any application instances for this computer - any orphaned applications created by this action
            // will be tidied up if still orphaned by the post-audit cleanup function
            new ApplicationInstanceDAO().ApplicationInstanceDelete(auditedComputer.AssetID);

            // Clean up any audited files/folders
            FileSystemDAO fileSystemDAO = new FileSystemDAO();
            fileSystemDAO.FileSystemFolder_Clean(auditedComputer.AssetID);
        }


        /// <summary>
        /// Called to clean up the database after a (re)audit of a computer
        /// </summary>
        protected void PostAuditCleanup(Asset auditedComputer)
        {
            new FileSystemDAO().FileSystemDeleteOrphans(auditedComputer.AssetID);

            // Delete any orphans which may be left in the applications table as a result of the re-audit
            new ApplicationsDAO().ApplicationDeleteOrphans();

            // ...and ensure that any 'request audit' flag is cleared
            new AssetDAO().AssetRequestAudit(auditedComputer.AssetID, false);
        }



        /// <summary>
        /// Generate audit trail entries for any changes which have occurred in this audit
        /// </summary>
        /// <param name="oldOS"></param>
        /// <param name="auditedOS"></param>
        /// <param name="oldApplications"></param>
        /// <param name="auditedApplications"></param>
        protected void GenerateAuditTrail(AuditDataFile auditDataFile
                                        , Asset auditedComputer
                                        , AuditedItemList oldAuditedItems
                                        , ApplicationInstanceList oldApplications
                                        , OSInstance oldOS)
        {
            // We can assume a new audit if no applications
            if (oldApplications.Count == 0)
                return;

            // Create the temporary list
            List<AuditTrailEntry> listChanges = new List<AuditTrailEntry>();

            //  OS History Checking
            GenerateOSAuditTrail(auditDataFile, auditedComputer, listChanges, oldOS);

            // Installed Applications History Checking
            GenerateApplicationsAuditTrail(auditDataFile, auditedComputer, listChanges, oldApplications);

            // AuditedItems History Checking
            GenerateAuditedItemsAuditTrail(auditDataFile, auditedComputer, listChanges, oldAuditedItems);

            // User Defined Data Field History Checking

            // Now add these changes to the Audit Trail
            AuditTrailDAO lwDataAccess = new AuditTrailDAO();
            foreach (AuditTrailEntry ate in listChanges)
            {
                lwDataAccess.AuditTrailAdd(ate);
            }
        }


        /// <summary>
        /// Generate Audit Trail Entries for changes to the Operating System
        /// </summary>
        /// <param name="listChanges"></param>
        /// <param name="oldOS"></param>
        /// <param name="newOS"></param>
        protected void GenerateOSAuditTrail(AuditDataFile auditDataFile, Asset auditedComputer, List<AuditTrailEntry> listChanges, OSInstance oldOS)
        {
            OSInstance newOS = auditDataFile.AuditedOS;
            if ((oldOS != null) && (oldOS.Name != newOS.Name))
            {
                AuditTrailEntry ate = new AuditTrailEntry();
                ate.Class = AuditTrailEntry.CLASS.application_changes;
                ate.AssetID = auditedComputer.AssetID;
                ate.AssetName = auditedComputer.Name;
                ate.Type = AuditTrailEntry.TYPE.changed;
                ate.Date = _uploadTime;
                ate.Key = "Operating System";
                ate.OldValue = oldOS.Name;
                ate.NewValue = newOS.Name;
                listChanges.Add(ate);
            }
        }


        /// <summary>
        /// Generate Audit Trail Entries for any changes to installed applications
        /// </summary>
        /// <param name="auditDataFile"></param>
        /// <param name="listChanges"></param>
        /// <param name="oldApplications"></param>
        protected void GenerateApplicationsAuditTrail(AuditDataFile auditDataFile, Asset auditedComputer, List<AuditTrailEntry> listChanges, ApplicationInstanceList oldApplications)
        {
            // Recover the fresh audit data from the audit data file again
            ApplicationInstanceList auditedApplications = auditDataFile.AuditedApplications;

            // Installed Application History Checking
            // ======================================
            //
            // Loop through the application list - first check for applications having been uninstalled
            // that is they are in the old list but not the new list
            //
            // Note we can skip all of this if this is the first time this PC has been audited
            if (auditedComputer.LastAudit != null && auditedComputer.LastAudit.Ticks != 0)
            {
                // Loop through each of the application in our original (old) list noting that this
                // will include applications that were flagged as hidden
                foreach (ApplicationInstance oldInstance in oldApplications)
                {
                    // Ignore applications that were previously Ignored as they still existed
                    if (oldInstance.IsIgnored)
                        continue;

                    // Is this application still in our list
                    if (auditedApplications.ContainsApplication(oldInstance.Name) == null)
                    {
                        AuditTrailEntry ate = new AuditTrailEntry();
                        ate.Class = AuditTrailEntry.CLASS.application_installs;
                        ate.AssetID = auditedComputer.AssetID;
                        ate.AssetName = auditedComputer.Name;
                        ate.Date = _uploadTime;
                        ate.Type = AuditTrailEntry.TYPE.deleted;
                        ate.Key = oldInstance.Name;
                        listChanges.Add(ate);
                    }
                }

                // Look for any new applications - that is are in the new list but not the old
                foreach (ApplicationInstance newInstance in auditedApplications)
                {
                    // Ignore applications that were previously Ignored
                    if (newInstance.IsIgnored)
                        continue;

                    if (oldApplications.ContainsApplication(newInstance.Name) == null)
                    {
                        AuditTrailEntry ate = new AuditTrailEntry();
                        ate.Class = AuditTrailEntry.CLASS.application_installs;
                        ate.AssetID = auditedComputer.AssetID;
                        ate.AssetName = auditedComputer.Name;
                        ate.Type = AuditTrailEntry.TYPE.added;
                        ate.Key = newInstance.Name;
                        ate.Date = _uploadTime;
                        listChanges.Add(ate);
                    }
                }
            }
        }



        /// <summary>
        /// Generate Audit Trail entries for any changes in the audited items
        /// 
        /// We do not generate audit trail entries for items which appear or dis-appear
        /// as this can be down to changes in the audit settings rather we simply check for
        /// items which existed previously and have been audited this time also and then compare
        /// the audited values.  Note that we ignore any item which is flagged as not being 
        /// historied as this allows us to ignore items which we know are likely to change 
        /// such as active processes
        /// </summary>
        /// <param name="auditDataFile"></param>
        /// <param name="listChanges"></param>
        /// <param name="oldAuditedItems"></param>
        protected void GenerateAuditedItemsAuditTrail(AuditDataFile auditDataFile, Asset auditedComputer, List<AuditTrailEntry> listChanges, AuditedItemList oldAuditedItems)
        {
            // Get the list of audited items from the audit just uploaded
            List<AuditedItem> auditedItems = auditDataFile.AuditedItems;

            // Iterate through these items and check for a matching category / name in the old list
            foreach (AuditedItem newItem in auditedItems)
            {
                if (newItem.Historied)
                {
                    AuditedItem oldItem = oldAuditedItems.FindItemByName(newItem.Category, newItem.Name);
                    if ((oldItem == null) || (oldItem.Value == newItem.Value))
                        continue;

                    // Existing item has changed value so add an audit trail entry for it
                    AuditTrailEntry ate = new AuditTrailEntry();
                    ate.Class = AuditTrailEntry.CLASS.auditdata;
                    ate.AssetID = auditedComputer.AssetID;
                    ate.AssetName = auditedComputer.Name;
                    ate.Type = AuditTrailEntry.TYPE.changed;
                    ate.Key = newItem.Category + ":" + newItem.Name;
                    ate.Date = _uploadTime;
                    ate.OldValue = oldItem.Value;
                    ate.NewValue = newItem.Value;
                    listChanges.Add(ate);
                }
            }
        }



        /// <summary>
        /// Handle and audit data file which has been uploaded into the AuditWizard database
        /// </summary>
        /// <param name="auditDataFile"></param>
        public void HandleUploadedAuditDataFile(AuditDataFile auditDataFile)
        {
            // open the AuditWizardv8.exe.config file to get the Upload settings
            bool deleteAfterUpload = true;
            try
            {
                //Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
                //deleteAfterUpload = Convert.ToBoolean(config.AppSettings.Settings["delete_after_upload"].Value);

                SettingsDAO lwDataAccess = new SettingsDAO();
                deleteAfterUpload = lwDataAccess.GetSettingAsBoolean(DatabaseSettingsKeys.Setting_DeleteAfterUpload, false);
            }
            catch (Exception)
            {
            }

            // Was the file uploaded or did we have an error
            if (auditDataFile.FileIsValid)
            {
                if (deleteAfterUpload)
                {
                    File.Delete(auditDataFile.FileName);

                    // If the audit was for a USB/PDA then we may also have an .SWA file which we should also delete
                    if ((auditDataFile.CreatedBy == AuditDataFile.CREATEDBY.usbscanner)
                    || (auditDataFile.CreatedBy == AuditDataFile.CREATEDBY.mdascanner))
                    {
                        string swaFileName = Path.Combine(Path.GetDirectoryName(auditDataFile.FileName), Path.GetFileNameWithoutExtension(auditDataFile.FileName));
                        swaFileName += ".swa";
                        if (File.Exists(swaFileName))
                            File.Delete(swaFileName);
                    }
                }
                else
                {
                    //string backupFile = auditDataFile.FileName.Replace(Path.GetExtension(auditDataFile.FileName) ,"OLD");
                    System.IO.FileInfo fi = new FileInfo(auditDataFile.FileName);

                    Directory.CreateDirectory(fi.DirectoryName + @"\backup\");

                    int backupFileAppender = 0;
                    string backupFilename = fi.DirectoryName + @"\backup\" + fi.Name;

                    while (File.Exists(backupFilename))
                    {
                        backupFilename = fi.DirectoryName + @"\backup\" + fi.Name;
                        backupFileAppender++;
                        string newFileName = Path.GetFileNameWithoutExtension(backupFilename) + backupFileAppender + ".adf";
                        backupFilename = Path.GetDirectoryName(backupFilename) + @"\" + newFileName;
                    }

                    File.Move(auditDataFile.FileName, backupFilename);

                    // If the audit was for a USB/PDA then we may also have an .SWA file which we should also move
                    if ((auditDataFile.CreatedBy == AuditDataFile.CREATEDBY.usbscanner)
                    || (auditDataFile.CreatedBy == AuditDataFile.CREATEDBY.mdascanner))
                    {
                        string swaFileName = Path.Combine(Path.GetDirectoryName(auditDataFile.FileName), Path.GetFileNameWithoutExtension(auditDataFile.FileName));
                        swaFileName += ".swa";

                        fi = new FileInfo(swaFileName);

                        Directory.CreateDirectory(fi.DirectoryName + @"\backup\");

                        backupFileAppender = 0;
                        backupFilename = fi.DirectoryName + @"\backup\" + fi.Name;

                        while (File.Exists(backupFilename))
                        {
                            backupFileAppender++;
                            string newFileName = Path.GetFileNameWithoutExtension(backupFilename) + backupFileAppender;
                            backupFilename = Path.GetDirectoryName(backupFilename) + @"\" + newFileName;
                        }

                        if (File.Exists(swaFileName))
                            File.Move(swaFileName, backupFilename);
                    }
                }
            }
            else
            {
                // Rename the audit data file to .bad 
                string badFile = auditDataFile.FileName.Replace(Path.GetExtension(auditDataFile.FileName), "BAD");
                File.Move(auditDataFile.FileName, badFile);
            }
        }

        /// <summary>
        /// Called to re-locate the selected asset based on it's IP addresses
        /// </summary>
        public void RelocateByIPAddress(Asset asset)
        {
            // Does the asset have an existing location?  If so we do not move it here
            // We also do not re-locate if there is no IP address specified
            if ((asset.LocationID == 1) && (asset.IPAddress != ""))
            {
                // Populate the list of locations defined if not already done
                if (_listGroups == null)
                {
                    _listGroups = new AssetGroupList();
                    LocationsDAO lwDataAccess = new LocationsDAO();
                    DataTable table = lwDataAccess.GetAllLocations();

                    foreach (DataRow row in table.Rows)
                    {
                        AssetGroup group = new AssetGroup(row, AssetGroup.GROUPTYPE.userlocation);
                        _listGroups.Add(group);
                    }
                }

                // Loop through the groups and see if we can find a good match for the IP address of the asset (if any)
                AssetGroup matchedGroup = _listGroups.FindByIP(asset.IPAddress);
                if ((matchedGroup != null) && (matchedGroup.GroupID != asset.LocationID))
                {
                    // A match was found that is different to the current location so update this asset
                    asset.LocationID = matchedGroup.GroupID;
                    asset.Update();
                }
            }
        }


        #endregion Audit Upload Functions
    }

    #endregion AuditUploader Class
}
