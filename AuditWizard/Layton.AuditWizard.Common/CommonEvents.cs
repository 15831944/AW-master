using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.Common
{
	public class CommonEventTopics
	{
		public const string PublisherFilterChanged = "event://AuditWizardCommonEvents/PublisherFilterChanged";
		public const string EmailSettingsChanged = "event://AuditWizardCommonEvents/EmailSettingsChanged";
		public const string ShowLicensedApplicationsChanged = "event://AuditWizardCommonEvents/ShowLicensedApplicationsChanged";
		public const string ViewStyleChanged = "event://AuditWizardCommonEvents/ViewStyleChanged";
	}

	/// <summary>
	/// Event arguments for the 'Publisher Filter Changed' Event - simply supplies the new filter string
	/// </summary>
	public class PublisherFilterEventArgs : EventArgs
	{
		private String	_publisherFilter;
		private bool	_viewIncludedApplications;
		private bool	_viewIgnoredApplications;

		public PublisherFilterEventArgs(string publisherFilter, bool viewIncludedApplications, bool viewIgnoredApplications)
		{
			_publisherFilter = publisherFilter;
			_viewIncludedApplications = viewIncludedApplications;
			_viewIgnoredApplications = viewIgnoredApplications;
		}

		public String PublisherFilter
		{
			get { return _publisherFilter; }
		}

		public bool ViewIncludedApplications
		{
			get { return _viewIncludedApplications; }
			set { _viewIncludedApplications = value; }
		}

		public bool ViewIgnoredApplications
		{
			get { return _viewIgnoredApplications; }
			set { _viewIgnoredApplications = value; }
		}
	}


	/// <summary>
	/// Event arguments for the 'Email Settings Changed' Event - no arguments
	/// </summary>
	public class EmailSettingsChangedEventArgs : EventArgs
	{
	}


	/// <summary>
	/// Event arguments for the 'View Style Changed' Event 
	/// </summary>
	public class ViewStyleChangedEventArgs : EventArgs
	{
		private bool _domainViewStyle;

		public bool DomainViewStyle
		{
			get { return _domainViewStyle; }
			set { _domainViewStyle = value; }
		}

		public ViewStyleChangedEventArgs(bool domainViewStyle)
		{ _domainViewStyle = domainViewStyle; }
	}
}
