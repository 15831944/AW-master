using System;

namespace Layton.AuditWizard.DataAccess
{
	public class DataStrings
	{
		// Fields
		public const string UNIDENIFIED_PUBLISHER = "<unidentified>";
		public const string MICROSOFT = "Microsoft Corporation, Inc.";
		public const string Disclaimer = "The information contained in this report has been automatically generated based on data collected from Computers on your network and manually entered.  Although Layton Technology, Inc. have taken all possible steps to ensure the accuracy of this data we cannot guarantee that (a) all computers owned by your organization have been audited or (b) the accuracy of any information manually entered.  As such, this report should be treated as a guide to assist in ensuring software licensing compliancy.  Layton Technology, Inc. cannot be held liable for any omissions or inaccuracies which may be contained within this report.";
	}

	public class TableNames
	{
		// Fields
		public const string APPLICATION_INSTANCES = "APPLICATION_INSTANCES";
		public const string APPLICATION_STATISTICS = "APPLICATION_STATISTICS";
		public const string SUPPORT_STATISTICS = "APPLICATION_SUPPORT_STATISTICS";
		public const string APPLICATIONS = "APPLICATIONS";
		public const string COMPUTER_STATISTICS = "COMPUTER_STATISTICS";
		public const string ASSETS = "ASSETS";
		public const string LICENSE_ALLOCATIONS = "LICENSE_ALLOCATIONS";
		public const string LICENSES = "LICENSES";
		public const string AUDITEDITEMS = "AUDITEDITEMS";
		public const string LICENSETYPES = "LICENSE_TYPES";
		public const string LOCATIONS = "LOCATIONS";
		public const string NOTES = "NOTES";
		public const string DOCUMENTS = "DOCUMENTS";
		public const string VERSION = "VERSION";
		public const string AUDITTRAIL = "AUDITTRAIL";
		public const string USERS = "USERS";
		public const string ALERTS = "ALERTS";
		public const string SUPPLIERS = "SUPPLIERS";
		public const string ACTIONS = "ACTIONS";
		public const string UDDD = "UDDD";
		public const string ASSET_TYPES = "ASSET_TYPES";
		public const string PICKLISTS = "PICKLISTS";
		public const string OPERATIONS = "OPERATIONS";
		public const string FS_FOLDERS = "FS_FOLDERS";
		public const string FS_FILES = "FS_FILES";
        public const string ASSET_SUPPORTCONTRACT = "ASSET_SUPPORTCONTRACT";
	}

	public class MailSettingsKeys
	{
		public const string MailSender = "MailSender";
		public const string MailAddress = "MailAddress";
		public const string MailPort = "MailPort";
		public const string MailServer = "MailServer";
		public const string MailRequiresAuthentication = "MailRequiresAuthentication";
		public const string MailUserName = "MailUserName";
		public const string MailPassword = "MailPassword";
		public const string MailFrequency = "MailFrequency";
        public const string MailSSLEnabled = "MailSSLEnabled"; // Added for ID 66125/66652
	}

	public class DatabaseSettingsKeys
	{
		public const string Setting_AutoUploadEnabled	= "AutoUpload Enabled";
		public const string Setting_OverwriteUserData	= "OverwriteUserData";
		public const string Setting_DeleteAfterUpload	= "DeleteAfterUpload";
		public const string Setting_BackupAfterUpload	= "BackupAfterUpload";
		public const string Setting_SampleDataLoaded	= "SampleDataLoaded";
		public const string Setting_ShowNewAlertsAtStartup	= "ShowAlertsAtStartup";
        public const string Setting_DisableAllUpolads = "DisableAllUploads";
        public const string Setting_FindAssetByName = "FindAssetByName";
        //public const string Setting_UpdateAgent = "UpdateAgent";
	}


	public class MailFrequencyValues
	{
		public const string Never = "Never";
		public const string Daily = "Daily";
		public const string Weekly = "Weekly";
		public const string Monthly = "Monthly";
	}


	public class AWMiscStrings
	{
		public const string AssetDetails = "Asset Details";
		public const string AllApplications = "<All Applications>";
		public const string AllComputers = "<All Computers>";
		public const string EntireNetwork = "Entire Network";
		public const string SummaryNode = "Summary";
		public const string ApplicationsNode = "Applications";
		public const string HardwareNode = "Hardware";
		public const string SystemNode = "System";
		public const string OSNode = "Operating Systems";
		public const string InternetNode = "Internet";
		public const string FileSystem = "FileSystem";
		public const string HistoryNode = "History";
		public const string UserDataNode = "UserData";
		public const string AllAssetsNode = "<All Assets>";
        public const string NoValueFound = "<no value>";
        public const string SystemPatchesNode = "System|Patches|";
	}

}



