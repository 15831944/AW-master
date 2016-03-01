using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder;
using System.Data;
using Infragistics.Win.UltraWinTree;
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Applications
{
    public class ApplicationsTabViewPresenter
    {
        private ApplicationsTabView _tabView;
        private string _currentPublisher;
        private bool _isPublisherDisplayed = false;
        private List<InstalledApplication> _installedApplications;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [InjectionConstructor]
        public ApplicationsTabViewPresenter()
        {
        }

        public ILaytonView View
        {
            set { _tabView = (ApplicationsTabView)value; }
        }

        public bool IsPublisherDisplayed
        {
            get { return _isPublisherDisplayed; }
        }

        public void Initialize()
        {
            // If we have a Publisher Selected then we should display the applications for that publisher
            // JML_LINDE
            if (_isPublisherDisplayed)
                ShowPublisher(_currentPublisher);
            else
                ShowPublisher(null);
        }

        public void DisplayPublishers()
        {
            _tabView.Refresh();
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            if (_isPublisherDisplayed)
            {
                ShowPublisher(_currentPublisher);
            }
            else
            {
                ShowPublisher(null);
            }

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }


        /// <summary>
        /// Called to show the list of applications for a specific publisher
        /// </summary>
        /// <param name="forPublisher"></param>
        public void ShowPublisher(string forPublisher)
        {
            // JML_LINDE
            if (forPublisher == null)
                return;

            try
            {
                _tabView.SuspendLayout();

                DataRow[] dataRows;
                // Get the work item controller
                ApplicationsWorkItemController wiController = _tabView.WorkItem.Controller as ApplicationsWorkItemController;

                // ...and from there settings which alter what we display in this view
                bool showIncluded = wiController.ShowIncludedApplications;
                bool showIgnored = wiController.ShowIgnoredApplications;
                string publisherFilter = wiController.PublisherFilter;

                // If we have not been supplied a publisher to display then flag that we are not displaying
                // a publisher at this time, regardless save the supplied publisher
                if (forPublisher == null)
                {
                    _isPublisherDisplayed = false;

                    // OK there is no explicit publisher but is there a general publisher filter that we
                    // will need to supply to reduce the number of Publishers reported?
                    if (wiController.PublisherFilter != "")
                        publisherFilter = wiController.PublisherFilter;
                }

                else
                {
                    _isPublisherDisplayed = true;
                    _currentPublisher = forPublisher;
                    publisherFilter = forPublisher;
                }

                // Initialize the tab view now that we know what we are displaying
                InitializeTabView();

                // Call database function to return list of applications (for the specified publisher)
                ApplicationsDAO lwDataAccess = new ApplicationsDAO();
                DataTable applicationsTable = lwDataAccess.GetApplications(forPublisher, showIncluded, showIgnored);

                // Set the header text and image for the tab view based on whether we are displaying
                // all (possibly filtered) publishers or a sepcific publisher
                _tabView.HeaderText = (forPublisher == null) ? MiscStrings.AllPublishers : forPublisher;
                _tabView.HeaderImage = Properties.Resources.application_publisher_72;

                DataTable applicationInstancesTable = new ApplicationInstanceDAO().GetApplicationInstances(forPublisher);
                DataTable applicationLicensesTable = new LicensesDAO().GetApplicationLicenses();

                // get a list of aliased applications - will save processing time later
                List<int> aliasedToApplicationsList = new List<int>();

                foreach (DataRow dataRow in lwDataAccess.GetAliasedToApplications().Rows)
                {
                    aliasedToApplicationsList.Add(Convert.ToInt32(dataRow[0]));
                }

                // ...the create InstalledApplication objects for each returned and add to the view
                foreach (DataRow row in applicationsTable.Rows)
                {
                    InstalledApplication thisApplication = new InstalledApplication(row);

                    // Read instances/licenses of this application

                    dataRows = applicationInstancesTable.Select("_APPLICATIONID = " + thisApplication.ApplicationID);
                    thisApplication.LoadInstances1(dataRows);

                    dataRows = applicationLicensesTable.Select("_APPLICATIONID = " + thisApplication.ApplicationID);
                    thisApplication.LoadLicenses1(dataRows);

                    // find any applications which are aliased to this application as we also need their licenses
                    if (aliasedToApplicationsList.Contains(thisApplication.ApplicationID))
                    {
                        foreach (DataRow dataRow in lwDataAccess.GetAliasedApplicationsByApplicationId(thisApplication.ApplicationID).Rows)
                        {
                            dataRows = applicationLicensesTable.Select("_APPLICATIONID = " + dataRow[0]);
                            thisApplication.LoadLicenses1(dataRows);
                        }
                    }

                    // ...and add to the tab view
                    _tabView.AddApplication(thisApplication);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in ShowPublisher()", ex);
            }
            finally
            {
                _tabView.ResumeLayout();
            }
        }

        public void ShowPublisher(int aCompliantType)
        {
            try
            {
                // Get the work item controller
                ApplicationsWorkItemController wiController = _tabView.WorkItem.Controller as ApplicationsWorkItemController;

                // ...and from there settings which alter what we display in this view
                bool showIncluded = wiController.ShowIncludedApplications;
                bool showIgnored = wiController.ShowIgnoredApplications;
                string publisherFilter = wiController.PublisherFilter;

                _isPublisherDisplayed = false;
                _currentPublisher = null;

                // Initialize the tab view now that we know what we are displaying
                InitializeTabView();

                // Call database function to return list of applications (for the specified publisher)
                ApplicationsDAO lwDataAccess = new ApplicationsDAO();
                DataTable applicationsTable = lwDataAccess.GetApplications(publisherFilter, showIncluded, showIgnored);

                // Set the header text and image for the tab view based on whether we are displaying
                // all (possibly filtered) publishers or a sepcific publisher
                _tabView.HeaderText = "Generating report data, please wait...";
                _tabView.HeaderImage = Properties.Resources.application_publisher_72;

                _tabView.Refresh();

                DataTable applicationInstancesTable = new ApplicationInstanceDAO().GetApplicationInstances();
                DataTable applicationLicensesTable = new LicensesDAO().GetApplicationLicenses();

                // get a list of aliased applications - will save processing time later
                List<int> aliasedToApplicationsList = new List<int>();

                foreach (DataRow dataRow in lwDataAccess.GetAliasedToApplications().Rows)
                {
                    aliasedToApplicationsList.Add(Convert.ToInt32(dataRow[0]));
                }

                // ...the create InstalledApplication objects for each returned and add to the view
                foreach (DataRow row in applicationsTable.Rows)
                {
                    InstalledApplication thisApplication = new InstalledApplication(row);

                    // Read instances/licenses of this application
                    DataRow[] dataRows = applicationInstancesTable.Select("_APPLICATIONID = " + thisApplication.ApplicationID);
                    thisApplication.LoadInstances1(dataRows);

                    dataRows = applicationLicensesTable.Select("_APPLICATIONID = " + thisApplication.ApplicationID);
                    thisApplication.LoadLicenses1(dataRows);

                    // find any applications which are aliased to this application as we also need their licenses
                    if (aliasedToApplicationsList.Contains(thisApplication.ApplicationID))
                    {
                        foreach (DataRow dataRow in lwDataAccess.GetAliasedApplicationsByApplicationId(thisApplication.ApplicationID).Rows)
                        {
                            dataRows = applicationLicensesTable.Select("_APPLICATIONID = " + dataRow[0]);
                            thisApplication.LoadLicenses1(dataRows);
                        }
                    }

                    //thisApplication.LoadData();

                    if (CheckApplicationState(aCompliantType, thisApplication))
                    {
                        // ...and add to the tab view
                        _tabView.AddApplication(thisApplication);
                    }
                }

                switch (aCompliantType)
                {
                    case 1:
                        _tabView.HeaderText = "Compliant Applications";
                        break;
                    case 2:
                        _tabView.HeaderText = "Non-compliant Applications";
                        break;
                    default:
                        _tabView.HeaderText = "All Applications";
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error in ShowPublisher()", ex);
            }
        }

        private bool CheckApplicationState(int aCompliantType, InstalledApplication thisApplication)
        {
            switch (aCompliantType)
            {
                case 0:
                    return true;
                case 1:
                    return (thisApplication.IsCompliant() && !thisApplication.IsIgnored);
                default:
                    if (!thisApplication.IsCompliant())
                        return (!thisApplication.IsIgnored && thisApplication.Licenses.Count > 0);
                    break;
            }

            return false;
        }

        public void ShowMultiplePublishers(List<InstalledApplication> installedApplications)
        {
            // Get the work item controller
            ApplicationsWorkItemController wiController = _tabView.WorkItem.Controller as ApplicationsWorkItemController;

            // ...and from there settings which alter what we display in this view
            bool showIncluded = wiController.ShowIncludedApplications;
            bool showIgnored = wiController.ShowIgnoredApplications;

            // Initialize the tab view now that we know what we are displaying
            InitializeTabView();

            // set the header
            _tabView.HeaderText = "Multiple Selection";
            _tabView.HeaderImage = Properties.Resources.application_publisher_72;

            // JML TODO - when we ignore/include multiple apps we end up here
            // each application needs to be refreshed so that the correct ignore flag is displayed...
            foreach (InstalledApplication app in installedApplications)
            {
                // Read instances/licenses of this application
                app.LoadData();

                // ...and add to the tab view
                _tabView.AddApplication(app);
            }

            _installedApplications = installedApplications;
        }

        /// <summary>
        /// This is called when we have selected a single application and we simply want to display its
        /// attributes in the explorer view
        /// </summary>
        /// <param name="selectedApplication">The application to display</param>
        public void ShowApplication(InstalledApplication selectedApplication)
        {
            InitializeTabView();
            _tabView.AddApplication(selectedApplication);
            _tabView.HeaderText = selectedApplication.Name;
            _tabView.HeaderImage = Properties.Resources.application_72;

        }

        private void InitializeTabView()
        {
            // clear the existing view
            _tabView.Clear();
        }
    }
}
