using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.Common
{
	public class CommonToolNames
	{
		// These tools appear in the 'Filters' group
		public const string FilterPublishers = "Publisher Filter";
		public const string FilterPublishersTooltip = "Invoke the 'Filter Publishers' Form to select the publishers for which applications are to be displayed";
		public const string ViewIncludedTooltip = "View applications that have NOT been flagged as ignored and will therefore be counted within AuditWizard";
		public const string ViewIncluded = "View Included Applications";
		public const string ViewIgnoredTooltip = "View applications that have been flagged as ignored and will therefore NOT be counted within AuditWizard";
		public const string ViewIgnored = "View Ignored Applications";
		
		// These tools appear in the 'Export' Group
		public const string ExportXLS = "Excel";
		public const string ExportPDF = "PDF";
		public const string ExportXPS = "XPS";
		public const string Print = "Print...";
        public const string Schedule = "Schedules";
        public const string LocationFilter = "Location Filter";
		public const string PrintPreview = "Print Preview";
	}
	
}
