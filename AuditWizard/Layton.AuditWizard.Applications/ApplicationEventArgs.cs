using System;
using System.Collections.Generic;
using System.Text;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Applications
{
    public class ApplicationsEventArgs : EventArgs
    {
        private List<InstalledApplication> _listApplications;

		public ApplicationsEventArgs(List<InstalledApplication> value)
        {
			_listApplications = value;
        }

		public List<InstalledApplication> Applications
        {
			get { return _listApplications; }
        }
    }


	/// <summary>
	/// This event is used when we select the 'Installs or licenses' node for an application or OS
	/// </summary>
	public class ApplicationLicenseEventArgs : EventArgs
	{
		private Object _selectedNodeObject;

		public ApplicationLicenseEventArgs(Object value)
		{
			_selectedNodeObject = value;
		}

		public Object SelectedNodeObject
		{
			get { return _selectedNodeObject; }
		}
	}


	/// <summary>
	/// This event is used when we select the 'Installations' node for an application or OS
	/// </summary>
	public class ApplicationInstallsEventArgs : EventArgs
	{
		private Object _selectedNodeObject;

		public ApplicationInstallsEventArgs(Object value)
		{
			_selectedNodeObject = value;
		}

		public Object SelectedNodeObject
		{
			get { return _selectedNodeObject; }
		}
	}


	/// <summary>
	/// Event fired when we select the OS node
	/// </summary>
	public class OperatingSystemEventArgs : EventArgs
	{
		private List<InstalledOS> _listInstalledOSs;

		public OperatingSystemEventArgs(List<InstalledOS> value)
		{
            _listInstalledOSs = value;
		}

		public List<InstalledOS> SelectedOS
		{
            get { return _listInstalledOSs; }
		}
	}


	/// <summary>
	/// Event fired when we select a Publishers node
	/// </summary>
	public class PublishersEventArgs : EventArgs
	{
		private List<ApplicationPublisher> _listPublishers;

		public PublishersEventArgs(List<ApplicationPublisher> value)
		{
			_listPublishers = value;
		}

		public List<ApplicationPublisher> ApplicationPublishers
		{
			get { return _listPublishers; }
		}
	}


	public class ActionsEventArgs : EventArgs
	{
		public ActionsEventArgs()
		{
		}
	}

	public class AlertsEventArgs : EventArgs
	{
		public AlertsEventArgs()
		{
		}
	}

}
