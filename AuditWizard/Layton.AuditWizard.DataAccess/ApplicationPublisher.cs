using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{

#region ApplicationPublisher class

	/// <summary>
	/// This class defines an instance of a Publisher of an ApplicationID
	/// It encapsulates a list of the InstalledApplications for this publisher
	/// </summary>
	public class ApplicationPublisher : List<InstalledApplication>
	{
		#region Local Data

		// Fields
		private int _publisherID;
		private string _name;

		#endregion Local Data

		#region Properties
		
		public int PublisherID
		{
			get { return _publisherID; }
		}

		public int ApplicationID
		{
			get { return this._publisherID; }
		}
		
		public string Name
		{
			get { return _name; }
			set
			{
				_name = (value == "") ? DataStrings.UNIDENIFIED_PUBLISHER : value;
			}
		}

		public List<InstalledApplication> Applications
		{
			get { return (List<InstalledApplication>)this; }
		}
		
		#endregion Properties

		#region Constructor
		// Methods
		public ApplicationPublisher(string name, int id)
		{
			Name = name;
			_publisherID = id;
		}
#endregion Constructor
	
		#region Methods

		/// <summary>
		/// Gets the <see cref="InstalledApplication"/> based on the supplied name
		/// </summary>
		/// <param name="applicationName">Name of the <see cref="InstalledApplication"/> to find</param>
		/// <returns>The <see cref="InstalledApplication"/> object, otherwise null if not found.</returns>
		public InstalledApplication FindApplication(string applicationName)
		{
			foreach (InstalledApplication theApplication in this)
			{
                //if (String.Compare(applicationName, theApplication.Name, true) == 0)
                //    return theApplication;

                if (applicationName == (theApplication.Name))
                    return theApplication;

                else if (applicationName == (theApplication.Name + " (v" + theApplication.Version + ")"))
                    return theApplication;
			}
			return null;
		}
		
		
		/// <summary>
		/// Iterate through the applications list and recover their compliancy status - if any
		/// are not compliant then the publisher as a whole is non-compliant
		/// </summary>
		public bool IsCompliant()
		{
			foreach (InstalledApplication thisApplication in this)
			{
				if (!thisApplication.IsCompliant())
					return false;
			}
			return true;
		}


		/// <summary>
		/// Called to rebuild the application list for this publisher
		/// </summary>
		public void Populate(bool includeIncluded ,bool includeIgnore)
		{
            ApplicationsDAO lwDataAccess = new ApplicationsDAO();

			// First clear any existing entries
			this.Clear();

			// ...then get all applications for this publisher
			DataTable applicationsTable = lwDataAccess.GetApplications(this.Name, includeIncluded, includeIgnore);
			foreach (DataRow row in applicationsTable.Rows)
			{
				InstalledApplication application = new InstalledApplication(row);

				// Load child data for this application
				application.LoadData();

				// ...add to our list
				this.Add(application);
			}
		}


		public override string ToString()
		{
			return this.Name;
		}
				
		
#endregion Methods

	}

#endregion Application Publishers Class

#region ApplicationPublisherList class

	public class ApplicationPublisherList : List<ApplicationPublisher>
	{
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = logger.IsDebugEnabled;

        private static int _installedApplicationCount = 0;

		#region Local Data Declarations

		/// <summary>
		/// Filter to apply to publishers to limit the number of applications returned
		/// </summary>
		private String _forPublisherFilter;
		#endregion Local Data Declarartions

		#region Constructor
	
		/// <summary>
		/// Base constructor does nothing
		/// </summary>
		public ApplicationPublisherList()
		{
			_forPublisherFilter = "";
		}

        public ApplicationPublisherList(string forPublisherFilter, bool includeNotIgnore, bool includeIgnore, bool aShowOS)
        {
            _forPublisherFilter = forPublisherFilter;

            // Now build the list of applications (for the specified publisher)
            // First get a list of the applications which match the publisher filter
            // We will build the list of Publishers as we go along
            ApplicationsDAO lwDataAccess = new ApplicationsDAO();
            DataTable applicationsTable = lwDataAccess.GetApplications(_forPublisherFilter, includeNotIgnore, includeIgnore, aShowOS);
            LoadFromTable(applicationsTable);
        }

		/// <summary>
		/// Constructor = this takes a publisher filter and will populate the ApplicationPublisherList
		/// </summary>
		/// <param name="forPublisherFilter"></param>
		public ApplicationPublisherList(string forPublisherFilter, bool includeNotIgnore ,bool includeIgnore)
		{
			_forPublisherFilter = forPublisherFilter;

			// Now build the list of applications (for the specified publisher)
			// First get a list of the applications which match the publisher filter
			// We will build the list of Publishers as we go along
			ApplicationsDAO lwDataAccess = new ApplicationsDAO();
			DataTable applicationsTable = lwDataAccess.GetApplications(_forPublisherFilter, includeNotIgnore, includeIgnore);
			LoadFromTable(applicationsTable);
		}

		#endregion Constructor

		#region Methods

		public void PopulateAliases()
		{
			ApplicationsDAO lwDataAccess = new ApplicationsDAO();
			DataTable applicationsTable = lwDataAccess.GetAliasedApplications();
			LoadFromTable(applicationsTable);
		}

		/// <summary>
		/// Return a compliancy status either for all data read or for a specific publisher
		/// </summary>
		/// <param name="publisher">If null, check all publishers otherwise check the specified publisher</param>
		/// <returns></returns>
		public bool IsCompliant(String publisher)
		{
			// Check for a publisher having been specified and if so just check that one
			if (publisher != null && publisher != "")
			{
				ApplicationPublisher thePublisher = FindPublisher(publisher);
				return (thePublisher != null) ? thePublisher.IsCompliant() : false;
			}

			// We have not specified a publisher so are looking at an overall status
			// If any publishers are not-compliant then we as a whole are not compliant
			foreach (ApplicationPublisher thePublisher in this)
			{
				if (!thePublisher.IsCompliant())
					return false;
			}

			return true;
		}

		public void CompliantApplicationCount(out int compliant ,out int noncompliant)
		{
			compliant = 0;
			noncompliant = 0;

			foreach (ApplicationPublisher thePublisher in this)
			{
				foreach (InstalledApplication theApplication in thePublisher.Applications)
				{
					if (theApplication.IsCompliant())
						compliant++;
					else
						noncompliant++;
				}
			}
		}

		#endregion Methods

		#region Internal Functions

		protected void LoadFromTable(DataTable applicationsTable)
		{
			// Iterate through the returned data creating the InstalledApplication instances for each row			
			foreach (DataRow row in applicationsTable.Rows)
			{
				try
				{
					// Create the installed application object
					InstalledApplication theApplication = new InstalledApplication(row);

                    // Ensure that we add the application to it's Publishers internal list
                    ApplicationPublisher thePublisher = FindPublisher(theApplication.Publisher);
                    if (thePublisher == null)
                    {
                        thePublisher = new ApplicationPublisher(theApplication.Publisher, 0);
                        this.Add(thePublisher);
                    }
                    
                    thePublisher.Add(theApplication);

					// Read instances of this application
					//theApplication.LoadData();
				}
               

				catch (Exception)
				{
				}
			}
		}
		/// <summary>
		/// Gets the <see cref="ApplicationPublisher"/> based on the supplied name
		/// </summary>
		/// <param name="publisherName">Name of the <see cref="ApplicationPublisher"/> to find</param>
		/// <returns>The <see cref="ApplicationPublisher"/> object, otherwise null if not found.</returns>
		public ApplicationPublisher FindPublisher(string publisherName)
		{
			foreach (ApplicationPublisher thePublisher in this)
			{
				if (String.Compare(publisherName ,thePublisher.Name ,true) == 0)
					return thePublisher;
			}
			return null;
		}
		#endregion Internal Functions



		/// <summary>
		/// Override of ToString returns the list of publisher names as a semi-colon delimited string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string returnString = "";
			foreach (ApplicationPublisher publisher in this)
			{
				if (returnString != "")
					returnString += ";";
				returnString += publisher.Name;
			}
			return returnString;
		}

	}
#endregion InstalledApplicationList class


}

