using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
	public class DatabaseSettings
	{

		#region Data
		public enum PURGEUNITS { days, months ,years ,never };

		/// <summary>Flag to show if automatic database purging is enabled</summary>
		private bool _autoPurge;

		/// <summary>Count of (units) for purging of asset history records</summary>
		private int _historyPurge;

		/// <summary>Units specified for asset history purging</summary>
		private PURGEUNITS _historyPurgeUnits;

		/// <summary>Count of (units) for purging of Internet records</summary>
		private int _internetPurge;

		/// <summary>Units specified for Internet record purging</summary>
		private PURGEUNITS _internetPurgeUnits;

		/// <summary>Count of (units) for purging of unaudited assets</summary>
		private int _assetPurge;

		/// <summary>Units specified for unaudited asset purging</summary>
		private PURGEUNITS _assetPurgeUnits;

		/// <summary>Folder specified to hold database backups</summary>
		private string _databaseBackupFolder;

		#endregion Data

		#region Setting Keys

		public const string Setting_AutoPurge			= "Database AutoPurge Enabled";
		public const string Setting_DatabaseFolder		= "Database Backups Folder";
		public const string Setting_HistoryPurge		= "Database History Purge";
		public const string Setting_HistoryPurgeUnits	= "Database History Purge Units";
		public const string Setting_InternetPurge		= "Database Internet Purge";
		public const string Setting_InternetPurgeUnits	= "Database Internet Purge Units";
		public const string Setting_AssetPurge			= "Database Asset Purge";
		public const string Setting_AssetPurgeUnits		= "Database Asset Purge Units";
		//
		public const string Setting_AlertMonitorEnable = "AlertMonitorEnable";
		public const string Setting_AlertMonitorSettingInterval = "AlertMonitorSettingsInterval";
		public const string Setting_AlertMonitorRecheckInterval = "AlertMonitorRecheckInterval";
		public const string Setting_AlertMonitorSystemTray = "AlertMonitorSystemTray";
		public const string Setting_LastAlertEmailDate = "AlertMonitorLastEmail";
		//
		public const string Setting_AlertMonitorEmailFrequency = "AlertMonitorEMailFrequency";	// 8.3.3
		public const string Setting_AlertMonitorEmailHourly = "Hourly";							// 8.3.3
		public const string Setting_AlertMonitorEmailDaily = "Daily";							// 8.3.3
		public const string Setting_AlertMonitorEmailTime = "AlertMonitorEmailTime";			// 8.3.3

		#endregion Setting Keys

		#region Properties

		/// <summary>AutoPurge Accessor</summary>
		public bool AutoPurge
		{
			get { return _autoPurge; }
			set { _autoPurge = value; }
		}

		/// <summary>Count of (UNITS) for purging of asset history assets</summary>
		public int HistoryPurge
		{
			get { return _historyPurge; }
			set { _historyPurge = value; }
		}

		public PURGEUNITS HistoryPurgeUnits
		{
			get { return _historyPurgeUnits; }
			set { _historyPurgeUnits = value; }
		}

		public int InternetPurge
		{
			get { return _internetPurge; }
			set { _internetPurge = value; }
		}

		public PURGEUNITS InternetPurgeUnits
		{
			get { return _internetPurgeUnits; }
			set { _internetPurgeUnits = value; }
		}

		public int AssetPurge
		{
			get { return _assetPurge; }
			set { _assetPurge = value; }
		}

		public PURGEUNITS AssetPurgeUnits
		{
			get { return _assetPurgeUnits; }
			set { _assetPurgeUnits = value; }
		}

		public string DatabaseBackupFolder
		{
			get { return _databaseBackupFolder; }
			set { _databaseBackupFolder = value; }
		}

		#endregion Properties
		
		#region Constructor

		public DatabaseSettings()
		{
			_autoPurge = true;
			_databaseBackupFolder = Path.Combine(Application.StartupPath, "Backup");
			_historyPurge = 3;
			_historyPurgeUnits = PURGEUNITS.months;
			_assetPurge = 6;
			_assetPurgeUnits = PURGEUNITS.months;
			_internetPurge = 28;
			_internetPurgeUnits = PURGEUNITS.days;
		}

		#endregion Constructor

		#region Methods

		public void LoadSettings()
		{
            SettingsDAO lwDataAccess = new SettingsDAO();
			AutoPurge = lwDataAccess.GetSettingAsBoolean(Setting_AutoPurge, false);
			HistoryPurge = Convert.ToInt32(lwDataAccess.GetSetting(Setting_HistoryPurge ,false));
			HistoryPurgeUnits = (PURGEUNITS)Convert.ToInt32(lwDataAccess.GetSetting(Setting_HistoryPurgeUnits, false));
			//
			InternetPurge = Convert.ToInt32(lwDataAccess.GetSetting(Setting_InternetPurge, false));
			InternetPurgeUnits = (PURGEUNITS)Convert.ToInt32(lwDataAccess.GetSetting(Setting_InternetPurgeUnits, false));
			//
			AssetPurge = Convert.ToInt32(lwDataAccess.GetSetting(Setting_AssetPurge, false));
			AssetPurgeUnits = (PURGEUNITS)Convert.ToInt32(lwDataAccess.GetSetting(Setting_AssetPurgeUnits, false));
			//
			DatabaseBackupFolder = lwDataAccess.GetSetting(Setting_DatabaseFolder, false);
		}

		public void SaveSettings()
		{
            SettingsDAO lwDataAccess = new SettingsDAO();
			lwDataAccess.SetSetting(Setting_AutoPurge, AutoPurge.ToString(), false);
			//
			lwDataAccess.SetSetting(Setting_HistoryPurge, HistoryPurge.ToString(), false);
			lwDataAccess.SetSetting(Setting_HistoryPurgeUnits, ((int)HistoryPurgeUnits).ToString(), false);
			//
			lwDataAccess.SetSetting(Setting_InternetPurge, InternetPurge.ToString(), false);
			lwDataAccess.SetSetting(Setting_InternetPurgeUnits, ((int)InternetPurgeUnits).ToString(), false);
			//
			lwDataAccess.SetSetting(Setting_AssetPurge, AssetPurge.ToString(), false);
			lwDataAccess.SetSetting(Setting_AssetPurgeUnits, ((int)AssetPurgeUnits).ToString(), false);
			//
			lwDataAccess.SetSetting(Setting_DatabaseFolder, DatabaseBackupFolder, false);
		}

		#endregion Methods

	}
}
