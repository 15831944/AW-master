using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
#region HiddenApplications class

	public class NotLicenseableApplications
	{
		private string _publisher;
		private List<String> _listApplications = new List<String>();

		/// <summary>
		/// Return the list of applications for this publisher
		/// </summary>
		public List<String> ListApplications
		{
			get { return _listApplications; }
		}

		/// <summary>
		/// Accessor for the Application publisher
		/// </summary>
		public string Publisher
		{
		  get { return _publisher; }
			set { _publisher = value; }
		}

		public NotLicenseableApplications (String publisher)
		{
			_publisher = publisher;
		}

		public NotLicenseableApplications ()
		{
			_publisher = "";
		}

		public void Add (String application)
		{
			_listApplications.Add(application);
		}

		public bool Contains(String application)
		{
			return _listApplications.Contains(application);
		}

		public int IndexOf(String application)
		{
			return _listApplications.IndexOf(application);
		}

		public bool Remove(String application)
		{
			return _listApplications.Remove(application);
		}

		public void RemoveAt(int index)
		{
			_listApplications.RemoveAt(index);
		}
	}
#endregion HiddenApplications class

#region ApplicationDefinitionsFile Class

	public class ApplicationDefinitionsFile : IniFile
	{
#region Static Data
		public static string FILENAME = "ApplicationDefinitions.ini";

		// Section names
		public static string PUBLISHERS_SECTION = "Publishers";
		public static string APPLICATION_MAPPINGS_SECTION = "ApplicationSerials";
		public static string PUBLISHER_ALIASES_SECTION = "PublisherAliases";
		public static string NOLICENSE_APPLICATIONS_SECTION = "UnlicensedApplications - ";
		public static string IGNORE_SECTION = "Ignored Applications - ";
#endregion Static Data

#region Data Declarations

		/// <summary>
		/// This list holds entries for hidden application onjects read from the ini file
		/// </summary>
		private List<NotLicenseableApplications> _listIgnoreApplications = new List<NotLicenseableApplications>();

#endregion Data Declarations

#region Properties (data accessors)

#endregion Properties (data accessors)

#region Constructor

		// Main Constructor
		public ApplicationDefinitionsFile() : base(Path.Combine(Application.StartupPath, FILENAME), 0)
		{
			ReadIgnoreApplications();
		}

#endregion Constructor

		//
		protected void ReadIgnoreApplications()
		{
			// Sections which begin with 'Ignored Applications - ' indicate a list of 
			// applications for a specific publisher which should be initially flagged as IGNORED
			// as they have been pre-defined as requiring no purchased/cost license
			foreach (IniSection thisSection in this)
			{
				if (!thisSection.Name.StartsWith(IGNORE_SECTION))
					continue;

				// Ok this is a hidden applications section so get the publisher and create the 
				// object to store the details in
				String publisher = thisSection.Name.Substring(IGNORE_SECTION.Length);
				NotLicenseableApplications IgnoreApplications = new NotLicenseableApplications(publisher);

				// Enumerate the keys within this section
				List<String> keys = new List<String>();
				this.EnumerateKeys(thisSection.Name, keys);

				// ...and add these keys as applications to the list of Ignored applications
				foreach (String thisApplication in keys)
				{
					IgnoreApplications.Add(thisApplication);
				}

				// ...and store this instance in our internal list
				_listIgnoreApplications.Add(IgnoreApplications);
			}
		}


		/// <summary>
		/// Return the list of not-NotIgnore applications for the specified publisher
		/// </summary>
		/// <param name="forPublisher">Name of the publisher to return the list for</param>
		/// <returns>List of application names if this publisher found, null otherwise</returns>
		public List<String> GetIgnoreApplicationList(String forPublisher)
		{
			// Ok is this publisher in our list?
			foreach (NotLicenseableApplications IgnoreApplications in this._listIgnoreApplications)
			{
				if (IgnoreApplications.Publisher == forPublisher)
					return IgnoreApplications.ListApplications;
			}
			return null;
		}


		/// <summary>
		/// Called to determine if the specified publisher/application pair equate to an application
		/// which has been flagged as beign ignored or not.  Note that the application name is only
		/// matched to the extent of the name specified in the initialization file.  Therefore if the
		/// initialization file contains the application name 'Microsoft .NET', ANY application whose
		/// name begins with this string will be flagged as hidden
		/// </summary>
		/// <param name="publisher"></param>
		/// <param name="application"></param>
		/// <returns></returns>
		public bool IsIgnored(String publisher, String application)
		{
			foreach (NotLicenseableApplications IgnoreApplications in this._listIgnoreApplications)
			{
				if (IgnoreApplications.Publisher == publisher)
				{
					foreach (String thisApplication in IgnoreApplications.ListApplications)
					{
						if (application.StartsWith(thisApplication))
							return true;
					}

					// correct publisher but no application match so we can break now
					break;
				}
			}

			// No matches so return that this application is NOT IGNORED
			return false;
		}


	}

#endregion ApplicationDefinitionsFile Class
}
