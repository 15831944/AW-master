using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.Common
{
	public class StatisticTitles
	{
		public const string CompliantComputers = "Computers Fully Licensed: ";
		public const string NonCompliantComputers = "Computers NOT Fully Licensed: ";
		public const string OutOfDateComputers = "Computers not Recently Audited: ";
		public const string UnauditedComputers = "Computers not Audited: ";
		//
		public const string PublishersAudited = "Publishers Identified: ";
		public const string PublishersInFilter = "Number of Filter Publishers: ";
		public const string UniqueApplications = "Number of Unique Applications: ";
		public const string TotalApplications = "Total Applications Audited: ";
		public const string MostCommonApplication = "Most Common Application: ";
		//
		public const string LicensesDeclared = "Licenses Declared: ";
		public const string LicensedApplications = "Compliant Applications: ";
		public const string NonLicensedApplications = "Non-Compliant Applications: ";
        public const string IgnoredApplications = "Ignored Applications: ";
        public const string NonSpecifiedApplications = "None Specified Applications: ";
		//
		public const string SupportExpired = "Expired: ";
		public const string SupportExpiringToday = "Expiring Today: ";
		public const string SupportExpiringWeek = "Expiring within a Week: ";
		public const string SupportExpiringMonth = "Expiring within a Month: ";
	}
}
