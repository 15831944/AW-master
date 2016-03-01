using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.Overview
{
    public class RibbonNames
    {
        public const string tabName = "Dashboard1";
        public const string wizardGroupName = "Setup Wizard";
        public const string wizardRibbonUISite = "wizardRibbonTools";
        public const string setupGroupName = "Configure";
        public const string setupRibbonUISite = "configureRibbonTools";
        public const string alertsGroupName = "Alerts";
        public const string tasksGroupName = "Manage Tasks";
		public const string alertsRibbonUISite	= "alertsRibbonTools";
        public const string tasksRibbonUISite = "tasksRibbonTools";
	}

    public class ToolNames
    {
        public const string SetupWizard			= "Run Setup Wizard";
		public const string ConfigureDashboard	= "Configure Dashboard";
        public const string AlertLog = "Alerts Log";
        public const string Tasks = "Tasks";
	}

	public class StatisticTitles
	{
		public const string ComputersDiscovered = "Assets in Database: ";
		public const string ComputersHidden = "Assets Hidden: ";
		public const string ComputersAudited = "Assets Audited: ";
		public const string ComputerLastAudit = "Last Audit: ";
		public const string CompliantComputers = "Computers Fully Licensed: ";
		public const string NonCompliantComputers = "Computers NOT Fully Licensed: ";
		public const string OutOfDateComputers = "Computers not Recently Audited: ";
		public const string UnauditedComputers = "Computers not Audited: ";
		public const string AssetsInStock		= "Assets In Stock: ";
		public const string AssetsInUse			= "Assets In Use: ";
		public const string AssetsPending		= "Assets Pending Disposal: ";
		public const string AssetsDisposed		= "Assets Disposed: ";
		//
		public const string AgentsDeployed = "Audit Agents Deployed: ";
		public const string UniqueApplications = "Unique Applications Audited: ";
		public const string TotalApplications = "Applications Audited: ";
		public const string MostCommonApplication = "Most Common Application: ";
		//
		public const string LastAlert = "Last Alert: ";
		public const string OutstandingAlerts = "Outstanding Alerts: ";
		public const string AlertsToday = "Alerts Today: ";
		public const string AlertsThisWeek = "Alerts this Week: ";
		public const string AlertsThisMonth = "Alerts this Month: ";
		//
		public const string LicensesDeclared = "Licenses Declared: ";
		public const string LicensedApplications = "Compliant Applications: ";
		public const string NonLicensedApplications = "Non-Compliant Applications: ";
		//
		public const string SupportExpired			= "Expired: ";
		public const string SupportExpireToday		= "Expire Today: ";
		public const string SupportExpireThisWeek	= "Expire this Week: ";
		public const string SupportExpireThisMonth	= "Expire this Month: ";
        //
        public const string SupportExpiredAsset = "Expired Asset: ";
        public const string SupportExpireTodayAsset = "Expire Today Asset: ";
        public const string SupportExpireThisWeekAsset = "Expire this Week Asset: ";
        public const string SupportExpireThisMonthAsset = "Expire this Month  Asset: ";
		//
		public const string AuditedToday	= "Assets Audited Today: ";
		public const string NotAudited7		= "Assets NOT Audited in 7 Days: ";
		public const string NotAudited14	= "Assets NOT Audited in 14 Days: ";
		public const string NotAudited30	= "Assets NOT Audited in 30 Days: ";
		public const string NotAudited90	= "Assets NOT Audited in 90 Days: ";
		public const string NotAudited		= "Assets NOT Audited: ";
		//
		public const string LicensedFor		= "License Count: ";
		public const string LicensesUsed	= "Licenses In Use: ";

	}
}
