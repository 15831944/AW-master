using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
//
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
//
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
//
using Infragistics.Win.UltraWinToolbars;

namespace Layton.AuditWizard.Reports
{
    public class ReportsWorkItemController : Layton.Cab.Interface.LaytonWorkItemController
    {
        #region Data Definitions

        // Initially we assume that this will be a licensing report
        //protected ReportDefinition _currentReport = new LicensingReportDefinition();
        protected ReportDefinition _currentReport = new AuditedDataReportDefinition();


        /// <summary>
        /// This is the current publisher filter which will determine which publishers we display.  It can
        /// be updated by the PublisherFilterChanged event being fired.
        /// Note that publisher filter and the two 'show' flags below use the same event
        /// </summary>
        private String _publisherFilter = "";
        private string _selectedReport;

        /// <summary>
        /// Flag to indicate whether or not we should be showing applications and operating systems
        /// which have been flagged as 'NotIgnore' or 'non-NotIgnore'. They can be updated by the 
        /// PublisherFilterChanged event being fired.
        /// </summary>
        private bool _ShowIncludedApplications = true;
        private bool _showIgnoredApplications = true;

        #endregion

        #region Data Accessors

        public string SelectedReport
        {
            get { return _selectedReport; }
            set { _selectedReport = value; }
        }

        public ReportDefinition CurrentReport
        {
            get { return _currentReport; }
            set
            {
                _currentReport = value;

                // Now fire the ReportChanged event so that everyone knows that we have a new report
                //FireReportChangedEvent();
            }
        }

        /// <summary>
        /// Recover the filter that has been set for Publishers
        /// </summary>
        public String PublisherFilter
        {
            get { return _publisherFilter; }
            set { _publisherFilter = value; }
        }


        /// <summary>
        /// Show or hide applications that have been flagged as 'hidden' in the database
        /// </summary>
        public bool ShowIncludedApplications
        {
            get { return _ShowIncludedApplications; }
            set
            {
                _ShowIncludedApplications = value;

                // Fire an event to let everyone know about the change
                FirePublisherFilterChangedEvent();
            }
        }

        /// <summary>
        /// Show or hide applications that have been flagged as 'hidden' in the database
        /// </summary>
        public bool ShowIgnoredApplications
        {
            get { return _showIgnoredApplications; }
            set
            {
                _showIgnoredApplications = value;

                // Fire an event to let everyone know about the change
                FirePublisherFilterChangedEvent();
            }
        }

        /// <summary>
        /// Event declaration for when the Publisher Filter is changed.
        /// </summary>
        [EventPublication(CommonEventTopics.PublisherFilterChanged, PublicationScope.Global)]
        public event EventHandler<PublisherFilterEventArgs> PublisherFilterChanged;

        /// <summary>
        /// Invoke the 'Filter Publishers' form to change which publishers are filtered
        /// </summary>
        public void FilterPublishers()
        {
            FormFilterPublishers form = new FormFilterPublishers();
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Update our copy of the filter
                _publisherFilter = form.PublisherFilter;

                // OK - Fire an event to indicate to any interested parties that the list of 
                // publisher filters has changed
                FirePublisherFilterChangedEvent();
            }
        }

        #endregion


        public ReportsWorkItemController(WorkItem workItem)
            : base(workItem)
        {
            // We need to pull the publisher filter list from the database
            SettingsDAO lwDataAccess = new SettingsDAO();
            _publisherFilter = lwDataAccess.GetPublisherFilter();
        }

        /// <summary>
        /// Called as we activate the work item to force the report view to initialize
        /// </summary>
        public override void ActivateWorkItem()
        {
            base.ActivateWorkItem();
            WorkItem.ExplorerView.RefreshView();
            //WorkItem.TabView.RefreshView();
        }


        /// <summary>
        /// This is the primary function aimed at running a report - we simply pass the report
        /// through to the tabview who does the actual work
        /// </summary>
        /// <param name="report"></param>
        public void RunReport(ReportDefinition report)
        {
            // Simply call the relevent function in the tab view to do the work
            ReportsTabView tabView = WorkItem.TabView as ReportsTabView;
            tabView.RunCustomReport((AuditedDataReportDefinition)report);
        }

        /// <summary>
        /// This is the primary function aimed at running a report - we simply pass the report
        /// through to the tabview who does the actual work
        /// </summary>
        /// <param name="report"></param>
        public void RunReport(string reportName)
        {
            // Simply call the relevent function in the tab view to do the work
            ReportsTabView tabView = WorkItem.TabView as ReportsTabView;
            tabView.RunReport(reportName);
        }

        /// <summary>
        /// This is the primary function aimed at running a report - we simply pass the report
        /// through to the tabview who does the actual work
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="filterConditions"></param>
        public void RunCustomReport(string reportName, List<string> filterConditions)
        {
            // Simply call the relevent function in the tab view to do the work
            ReportsTabView tabView = WorkItem.TabView as ReportsTabView;
            tabView.RunCustomReport(reportName, filterConditions);
        }

        /// <summary>
        /// This is the primary function aimed at running a report - we simply pass the report
        /// through to the tabview who does the actual work
        /// </summary>
        public void RunSQLReport(string reportName, string aSQLString)
        {
            // Simply call the relevent function in the tab view to do the work
            ReportsTabView tabView = WorkItem.TabView as ReportsTabView;
            tabView.RunSQLReport(reportName, aSQLString);
        }

        /// <summary>
        /// This is the primary function aimed at running a report - we simply pass the report
        /// through to the tabview who does the actual work
        /// </summary>
        public void RunComplianceReport(string reportName, List<string> filterConditions)
        {
            // Simply call the relevent function in the tab view to do the work
            ReportsTabView tabView = WorkItem.TabView as ReportsTabView;
            tabView.RunComplianceReport(reportName, filterConditions);
        }

        #region Export Menu Functions



        /// <summary>
        /// Export to PDF format 
        /// We call the appropriate grid view to handle this request themselves
        /// </summary>
        public void ExportToPDF()
        {
            ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            if (tabView is ReportsTabView)
                ((ReportsTabView)tabView).ExportToPDF();
        }


        /// <summary>
        /// Export to XLS format 
        /// We call the appropriate grid view to handle this request themselves
        /// </summary>
        public void ExportToXLS()
        {
            ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            if (tabView is ReportsTabView)
                ((ReportsTabView)tabView).ExportToXLS();
        }


        /// <summary>
        /// Export to XPS format 
        /// We call the appropriate grid view to handle this request themselves
        /// </summary>
        public void ExportToXPS()
        {
            ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            if (tabView is ReportsTabView)
                ((ReportsTabView)tabView).ExportToXPS();
        }

        #endregion Export Menu Functions

        #region Schedule Menu Functions

        /// <summary>
        /// Schedule Form
        /// </summary>
        public void ScheduleReports()
        {
            FormViewSchedules form = new FormViewSchedules();
            form.ShowDialog();
        }

        /// <summary>
        /// Filter Locations Form
        /// </summary>
        public void FilterLocations()
        {
            FormSelectLocations form = new FormSelectLocations(new LocationsDAO().GetSelectedLocations(), new LocationSettingsDAO().GetSetting("LocationFilter"));
            
            if (form.ShowDialog() == DialogResult.OK)
            {
                new LocationSettingsDAO().SetSetting("LocationFilter", form.SelectedAssetIds);
                new LocationSettingsDAO().SetSetting("LocationFilterGroups", form.SelectedGroupNames);
            }
        }

        #endregion

        /// <summary>
        /// Print Grid
        /// </summary>
        public void PrintGrid()
        {
            ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            if (tabView is ReportsTabView)
                ((ReportsTabView)tabView).PrintGrid();
        }

        public void PrintPreviewGrid()
        {
            ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            if (tabView is ReportsTabView)
                ((ReportsTabView)tabView).PrintPreviewGrid();
        }


        #region New, Load and Save Reports

        /// <summary>
        /// Create a new compliancereport
        /// </summary>
        public void RunExistingComplianceReport(int aReportId)
        {
            // Run the Report Wizard
            FormComplianceReport form = new FormComplianceReport(this, aReportId);
            form.RunComplianceReport();
        }

        /// <summary>
        /// Create a new compliancereport
        /// </summary>
        public void EditExistingComplianceReport(int aReportId)
        {
            // Run the Report Wizard
            FormComplianceReport form = new FormComplianceReport(this, aReportId);
            form.EditComplianceReport();
            form.ShowDialog();
        }

        /// <summary>
        /// Create a new report
        /// </summary>
        public void NewReport()
        {
            FormCustomReport form = new FormCustomReport(this);
            form.ShowDialog();
        }


        /// <summary>
        /// Load a new report
        /// </summary>
        public void LoadReport(int aSelectedReportId)
        {
            //FormLoadReport form = new FormLoadReport();
            //if (form.ShowDialog() == DialogResult.OK)
            //{
            //    // Do we want to save the current report before switching?
            //    if (_currentReport.HasChanged && (MessageBox.Show("Do you want to save any changes that you have made to the current report before switching?", "Save Report" ,MessageBoxButtons.YesNo) == DialogResult.Yes))
            //        SaveReport();

            //    // Fire an event to let others know that the report has been changed
            //    _currentReport = form.SelectedReport;
            //    //FireReportChangedEvent();

            //    // We now need to cause the report to be executed if so desired
            //    if (Properties.Settings.Default.RunReportAfterLoad)
            //        ((ReportsExplorerView)WorkItem.ExplorerView).GenerateReport();			
            //}


        }

        /// <summary>
        /// Save any changes made to the current report
        /// </summary>
        public void SaveReport()
        {
            // Ensure that we save any options not saved in to the report defintion now
            //((ReportsExplorerView)WorkItem.ExplorerView).UpdateReportOptions();

            //// Simply call the relevent function in the tab view to do the work
            //// Display the 'Save Report' form 
            //FormSaveReport form = new FormSaveReport(_currentReport);
            //if (form.ShowDialog() == DialogResult.OK)
            //    FireReportSavedEvent();

            ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            if (tabView is ReportsTabView)
                ((ReportsTabView)tabView).SaveCustomReport();
        }

        /// <summary>
        /// Run the current report
        /// </summary>
        public void RunReport()
        {
            RunReport(_currentReport);
        }

        #endregion Load and Save Reports



        #region Event Handlers

        protected void FirePublisherFilterChangedEvent()
        {
            // If the settings were changed from this tab then we should fire an event to let all other
            // tabs know of the change - if however we are not the active tab then we have been informed
            // of the change ourselves and do not need to pass it on.
            if (PublisherFilterChanged != null)
            {
                // relay the filter change if we are the active explorer view and hence we have changed
                // the filter ourselves
                ILaytonView activeExplorerView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].ActiveSmartPart;
                if (activeExplorerView == WorkItem.ExplorerView)
                    PublisherFilterChanged(this, new PublisherFilterEventArgs(_publisherFilter, _ShowIncludedApplications, _showIgnoredApplications));
            }
        }


        /// <summary>
        /// This is the handler for the GLOBAL PublisherFilterChangeEvent which is fired when 
        /// the Publisher Filter has been updated elsewhere in the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EventSubscription(CommonEventTopics.PublisherFilterChanged)]
        public void PublisherFilterChangedHandler(object sender, PublisherFilterEventArgs e)
        {
            // Simply update our internal filters with those specified
            _publisherFilter = e.PublisherFilter;
            _ShowIncludedApplications = e.ViewIncludedApplications;
            _showIgnoredApplications = e.ViewIgnoredApplications;

            // ...and force a refresh if we are the active explorer view
            ILaytonView activeExplorerView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].ActiveSmartPart;
            if (activeExplorerView == WorkItem.ExplorerView)
            {
                WorkItem.TabView.RefreshView();
                WorkItem.ExplorerView.RefreshView();
            }
        }



        /// <summary>
        /// Called when we want to inform both the tab and explorer view that the report has been changed
        /// This may be as a result of a new report being created or loading a new report
        /// </summary>
        protected void FireReportChangedEvent()
        {
            // Tell the Tab and Explorer View about the change in the report
            if (WorkItem.TabView is ReportsTabView)
                ((ReportsTabView)WorkItem.TabView).ReportChanged();
            if (WorkItem.ExplorerView is ReportsExplorerView)
                ((ReportsExplorerView)WorkItem.ExplorerView).ReportChanged();
        }



        /// <summary>
        /// Fire the report saved event so that ther tab and explorer view know that this has happened
        /// </summary>
        protected void FireReportSavedEvent()
        {
            // Tell the Tab and Explorer View about the change in the report
            if (WorkItem.TabView is ReportsTabView)
                ((ReportsTabView)WorkItem.TabView).ReportSaved();
            if (WorkItem.ExplorerView is ReportsExplorerView)
                ((ReportsExplorerView)WorkItem.ExplorerView).ReportSaved();
        }

        #endregion Event Handlers

    }


}
