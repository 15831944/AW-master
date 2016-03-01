using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
//
using Infragistics.Win.UltraWinTree;
using Layton.Cab.Interface;
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;
//
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
//
using Infragistics.Win.UltraWinToolbars;

namespace Layton.AuditWizard.Applications
{
    public class ApplicationsWorkItemController : Layton.Cab.Interface.LaytonWorkItemController
    {
        #region Data
        ApplicationsWorkItem workItem;

        /// <summary>
        /// This is the current publisher filter which will determine which publishers we display.  It can
        /// be updated by the PublisherFilterChanged event being fired.
        /// </summary>
        private string _publisherFilter = "";

        /// <summary>
        /// Flag to indicate whether or not we should be showing applications and operating systems
        /// which have been flagged as 'NotIgnore' or 'non-NotIgnore'. They can be updated by the 
        /// PublisherFilterChanged event being fired.
        /// </summary>
        private bool _showIncludedApplications = true;
        private bool _showIgnoredApplications = true;

        /// <summary>
        /// This is an internal list which keeps track of which applications are selected in the explorer
        /// view.  This list is populated when the 'PublisherSelectionChanged' event fires to indicate 
        /// that a different publisher has been selected and at this time all applications for the specific publisher are loaded.
        /// 
        /// If we select a specific application, this fires the ApplicationSelectionChanged event and the
        /// selected application is loaded into the list
        /// </summary>
        List<InstalledApplication> _listApplications = new List<InstalledApplication>();
        List<InstalledApplication> _tmpListApplications = new List<InstalledApplication>();

        /// <summary>
        /// This is an internal item which keeps track of which Operating System (if any) are selected in 
        /// the explorer view.  This is set when we select a specific Operating System causing the 
        /// 'OperatingSystemSelectionChanged' event to fire.
        /// </summary>
        //InstalledOS _selectedOS = null;
        List<InstalledOS> _selectedOSs = new List<InstalledOS>();

        /// <summary>
        /// If we need to select a specific tab on entry to settings then we set this field to the
        /// key of the tab that should be selected.
        /// </summary>
        private String _selectedTabKey = "";
        #endregion Data

        #region Properties
        public String SelectedTabKey
        {
            get { return _selectedTabKey; }
            set { _selectedTabKey = value; }
        }

        /// <summary>
        /// Show or hide applications that have been flagged as 'hidden' in the database
        /// </summary>
        public bool ShowIncludedApplications
        {
            get { return _showIncludedApplications; }
            set
            {
                _showIncludedApplications = value;

                // OK - Fire an event to indicate to any interested parties that one of the filters has changed
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

                // If the settings were changed from this tab then we should fire an event to let all other
                // tabs know of the change - if however we are not the active tab then we have been informed
                // of the change ourselves and do not need to pass it on.
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
                if (PublisherFilterChanged != null)
                    PublisherFilterChanged(this, new PublisherFilterEventArgs(form.PublisherFilter, _showIncludedApplications, _showIgnoredApplications));

                //ILaytonView applicationsTabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
                //((ApplicationsTabView)applicationsTabView).Presenter.ShowPublisher(null);
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

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationsWorkItemController(WorkItem workItem)
            : base(workItem)
        {
            this.workItem = workItem as ApplicationsWorkItem;

            // We need to pull the publisher filter list from the database
            SettingsDAO lwDataAccess = new SettingsDAO();
            _publisherFilter = lwDataAccess.GetPublisherFilter();
        }

        #endregion Constructor

        /// <summary>
        /// Each time we activate the applications tab we automatically update it
        /// </summary>
        public override void ActivateWorkItem()
        {
            base.ActivateWorkItem();
            WorkItem.ExplorerView.RefreshView();
            //workItem.GetActiveTabView().RefreshView();
        }

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
                    PublisherFilterChanged(this, new PublisherFilterEventArgs(_publisherFilter, _showIncludedApplications, _showIgnoredApplications));
            }
        }



        /// <summary>
        /// This is the handler for the PublisherChangeEvent which is fired when we select
        /// a different publisher as we need to ensure that we display only those applications 
        /// for the selected publisher.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EventSubscription(EventTopics.PublisherSelectionChanged)]
        public void PublisherSelectionChangedHandler(object sender, PublishersEventArgs e)
        {
            ILaytonView applicationsTabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
            SwitchActiveTabView(applicationsTabView);

            // Clear our internal applications list first
            _listApplications.Clear();
            _selectedOSs.Clear();

            if (e.ApplicationPublishers[0] == null)
            {
                // We have selected the 'All Publishers' branch - so display All publishers
                ((ApplicationsTabView)applicationsTabView).Presenter.ShowPublisher(null);
            }
            else if (e.ApplicationPublishers.Count == 1)
            {
                // We have selected a specific publisher - add that publishers applications to our internal
                // list so that any operations on this publisher operate on its applications
                _listApplications = e.ApplicationPublishers[0].Applications;

                //// ...and initiate the tab view refresh
                ((ApplicationsTabView)applicationsTabView).Presenter.ShowPublisher(e.ApplicationPublishers[0].Name);
            }
            else if (e.ApplicationPublishers.Count > 1)
            {
                // use a temp list to stop the collection becoming corrupted
                _tmpListApplications.Clear();

                //e.ApplicationPublishers[0].Populate(true, true);

                foreach (ApplicationPublisher appPub in e.ApplicationPublishers)
                {
                    if (appPub.Applications.Count == 0)
                    {
                        appPub.Populate(true, true);
                    }

                    foreach (InstalledApplication app in appPub.Applications)
                    {
                        _tmpListApplications.Add(app);
                    }
                }

                _listApplications = _tmpListApplications;

                // display the publisher information for each selection
                ((ApplicationsTabView)applicationsTabView).Presenter.ShowMultiplePublishers(_tmpListApplications);
            }
        }


        /// <summary>
        /// This is the handler for the OperatingSystemChangeEvent which is fired when we select
        /// a different OS or the root OS place-holder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EventSubscription(EventTopics.OperatingSystemSelectionChanged)]
        public void OperatingSystemSelectionChangedHandler(object sender, OperatingSystemEventArgs e)
        {
            // Save (any) Operating System which is currently selected.
            _selectedOSs.Clear();
            _selectedOSs = e.SelectedOS;
            _listApplications.Clear();

            // Set the Operating System tab view to be the active view
            ILaytonView newView = WorkItem.Items[Properties.Settings.Default.OSTabView] as ILaytonView;
            SwitchActiveTabView(newView);

            if (e.SelectedOS == null)
            {
                // We have selected the 'All Publishers' branch - so display All publishers
                ((OSTabView)newView).Presenter.ShowOS(null);
            }
            else
            {
                ((OSTabView)newView).Presenter.ShowOS(_selectedOSs[0]);
            }
        }

        /// <summary>
        /// This function is called when we receive an event to indicate that the selected application
        /// in the explorer view (left hand pane) has changed.
        /// 
        /// The purpose of this function is to ensure that the appropriate Tab View is selected in the 
        /// right hand pane and to force it to refresh itself.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EventSubscription(EventTopics.ApplicationSelectionChanged)]
        public void ApplicationSelectionChangedHandler(object sender, ApplicationsEventArgs e)
        {
            _listApplications = e.Applications;
            ILaytonView applicationsTabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
            SwitchActiveTabView(applicationsTabView);
            ((ApplicationsTabView)applicationsTabView).Presenter.ShowApplication(_listApplications[0]);
        }


        /// <summary>
        /// This event handler deals with the 'ApplicationInstallsSelectionChanged' event fired when 
        /// the user clicks on the 'Installs' node beneath an application or OS.  The purpose of this function
        /// is to ensure that the Installs tab is selected in the right hand pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EventSubscription(EventTopics.ApplicationInstallsSelectionChanged)]
        public void ApplicationInstallsSelectionChangedHandler(object sender, ApplicationInstallsEventArgs e)
        {
            // Add the selected application as the only application in our list
            _listApplications.Clear();
            _selectedOSs.Clear();

            // OK have we selected licenses for an OS or Application - the passed object should know
            Object nodeObject = e.SelectedNodeObject;
            if (nodeObject is InstalledApplication)
                _listApplications.Add(e.SelectedNodeObject as InstalledApplication);
            else
                _selectedOSs.Add(e.SelectedNodeObject as InstalledOS);

            // Set the INSTANCES tab view to be the active view
            ILaytonView instancesView = WorkItem.Items[Properties.Settings.Default.InstancesTabView] as ILaytonView;
            SwitchActiveTabView(instancesView);
            ((InstancesTabView)instancesView).Presenter.ShowApplicationInstances(nodeObject);
        }


        /// <summary>
        /// This event handler deals with the 'ApplicationLicensesSelectionChanged' event fired when 
        /// the user clicks on the 'Licenses' node beneath an application or an OS.  
        /// The purpose of this function is to ensure that the Licenses tab is selected in the right 
        /// hand pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EventSubscription(EventTopics.ApplicationLicenseSelectionChanged)]
        public void ApplicationLicensesSelectionChanged(object sender, ApplicationLicenseEventArgs e)
        {
            _listApplications.Clear();
            _selectedOSs.Clear();

            // OK have we selected licenses for an OS or Application - the passed object should know
            Object nodeObject = e.SelectedNodeObject;
            if (nodeObject is InstalledApplication)
                _listApplications.Add(e.SelectedNodeObject as InstalledApplication);
            else
                _selectedOSs.Add(e.SelectedNodeObject as InstalledOS);

            //
            ILaytonView licensesView = WorkItem.Items[Properties.Settings.Default.LicensesTabView] as ILaytonView;
            SwitchActiveTabView(licensesView);
            ((LicensesTabView)licensesView).Presenter.ShowApplicationLicenses(nodeObject);
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
            _showIncludedApplications = e.ViewIncludedApplications;
            _showIgnoredApplications = e.ViewIgnoredApplications;

            // ...and force a refresh if we are the active explorer view
            ILaytonView activeExplorerView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].ActiveSmartPart;
            if (activeExplorerView == WorkItem.ExplorerView)
            {
                Trace.Write("Refreshing Applications Views\n");
                workItem.GetActiveTabView().RefreshView();
                WorkItem.ExplorerView.RefreshView();
            }
        }


        /// <summary>
        /// This is the handler for the AlertsSelectionChanged which is fired when we select the 'Alerts' node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EventSubscription(EventTopics.AlertsSelectionChanged)]
        public void AlertsSelectionChangedHandler(object sender, AlertsEventArgs e)
        {
            // Set the Actions tab view to be the active view
            ILaytonView newView = WorkItem.Items[Properties.Settings.Default.AlertsTabView] as ILaytonView;
            SwitchActiveTabView(newView);
        }

        #endregion

        #region Toolbar Handlers

        #region Application Menu Functions

        public void SetIgnored()
        {
            FindSelectedApplications();
            SetIgnore(_listApplications);
            WorkItem.ExplorerView.RefreshView();
        }

        /// <summary>
        /// Hide the selected application(s) within the database
        /// </summary>
        /// <param name="listApplications">list of applications to act on</param>
        public void SetIgnore(List<InstalledApplication> listApplications)
        {
            ILaytonView applicationsTabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
            ApplicationsDAO lwDataAccess = new ApplicationsDAO();
            foreach (InstalledApplication application in listApplications)
            {
                lwDataAccess.ApplicationSetIgnored(application.ApplicationID, true);
            }

            if (applicationsTabView != null)
                ((ApplicationsTabView)applicationsTabView).Presenter.DisplayPublishers();
        }


        /// <summary>
        /// Clear the 'hidden' flag for the selected computer(s) within the database
        /// </summary>
        public void SetIncluded()
        {
            FindSelectedApplications();
            SetIncluded(_listApplications);
            WorkItem.ExplorerView.RefreshView();
        }

        /// <summary>
        /// Clear the 'hidden' flag for the selected computer(s) within the database
        /// </summary>
        /// <param name="listApplications">list of applications to act on</param>
        public void SetIncluded(List<InstalledApplication> listApplications)
        {
            ILaytonView applicationsTabView = WorkItem.Items[ViewNames.MainTabView] as ILaytonView;
            ApplicationsDAO lwDataAccess = new ApplicationsDAO();
            foreach (InstalledApplication application in listApplications)
            {
                lwDataAccess.ApplicationSetIgnored(application.ApplicationID, false);
            }

            if (applicationsTabView != null)
                ((ApplicationsTabView)applicationsTabView).Presenter.DisplayPublishers();
        }

        public void DeleteApplication(List<int> applicationIds)
        {
            ApplicationsDAO lApplicationsDAO = new ApplicationsDAO();

            // check if any of the applications are used as an alias - stop if true
            List<string> aliasedApps = lApplicationsDAO.CheckApplicationIsAnAlias(applicationIds);
            if (aliasedApps.Count > 0)
            {
                MessageBox.Show(
                    "The following applications have other applications aliased to them. Please remove the aliases " +
                    "before deleting." + Environment.NewLine + Environment.NewLine + string.Join("\n", aliasedApps.ToArray()),
                    "Delete Application", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            lApplicationsDAO.DeleteApplications(applicationIds);
            WorkItem.ExplorerView.RefreshView();
            WorkItem.TabView.RefreshView();
        }

        public void EditPublisherName(string publisherName)
        {
            FormEditPublisherName form = new FormEditPublisherName(publisherName);

            if (form.ShowDialog() == DialogResult.OK)
            {
                if (form.PublisherName != publisherName)
                {
                    new ApplicationsDAO().UpdatePublisherName(publisherName, form.PublisherName);
                    WorkItem.ExplorerView.RefreshView();
                    WorkItem.TabView.RefreshView();
                }
            }
        }

        private void FindSelectedApplications()
        {
            _listApplications.Clear();

            ILaytonView applicationsTabView = WorkItem.Items[ViewNames.MainTabView] as ILaytonView;
            if (applicationsTabView != null)
            {
                List<InstalledApplication> listApplications = ((ApplicationsTabView)applicationsTabView).CheckSelectedApplications();

                if (listApplications.Count > 0)
                {
                    _listApplications = listApplications;
                }
                else
                {
                    // check if we have selected a publisher (bug #623)
                    ApplicationsExplorerView explorerView = WorkItem.ExplorerView as ApplicationsExplorerView;
                    if (explorerView != null)
                    {
                        foreach (UltraTreeNode selectedNode in explorerView.ApplicationsTree.SelectedNodes)
                        {
                            if (selectedNode.Tag.ToString() != "PUBLISHER")
                            {
                                // applications selected
                                _listApplications.Add(selectedNode.Tag as InstalledApplication);
                            }
                            else
                            {
                                // publishers selected
                                if (selectedNode.Nodes.Count == 0)
                                {
                                    explorerView.Presenter.ExpandApplications(selectedNode);
                                }

                                foreach (UltraTreeNode node in selectedNode.Nodes)
                                {
                                    _listApplications.Add(node.Tag as InstalledApplication);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Show the application properties for the currently selected application
        /// </summary>
        public void ApplicationProperties()
        {
            ApplicationProperties(_listApplications[0]);
        }

        /// <summary>
        /// Alias Applications
        /// </summary>
        public void AliasApplications()
        {
            ApplicationsWorkItemController wiController = workItem.Controller as ApplicationsWorkItemController;
            FormAliasApplications form = new FormAliasApplications();
            form.ShowIgnored = wiController.ShowIgnoredApplications;
            form.ShowIncluded = wiController.ShowIncludedApplications;
            form.PublisherFilter = wiController.PublisherFilter;
            form.ShowDialog();

            workItem.GetActiveTabView().RefreshView();
            WorkItem.ExplorerView.RefreshView();
        }

        /// <summary>
        /// Alias Publishers
        /// </summary>
        public void AliasPublishers()
        {
            ApplicationsWorkItemController wiController = workItem.Controller as ApplicationsWorkItemController;
            FormAliasPublishers form = new FormAliasPublishers();
            form.ShowIgnored = wiController.ShowIgnoredApplications;
            form.ShowIncluded = wiController.ShowIncludedApplications;
            form.PublisherFilter = wiController.PublisherFilter;
            form.ShowDialog();

            workItem.GetActiveTabView().RefreshView();
            WorkItem.ExplorerView.RefreshView();
        }


        /// <summary>
        /// Add a new application (for the selected publisher?)
        /// </summary>
        public void AddApplication(string publisher)
        {
            FormAddApplication form = new FormAddApplication(publisher, _publisherFilter, _showIncludedApplications, _showIgnoredApplications);
            if (form.ShowDialog() == DialogResult.OK)
            {
                workItem.GetActiveTabView().RefreshView();
                WorkItem.ExplorerView.RefreshView();
            }
        }


        /// <summary>
        /// Show the Application Properties window for the specified application
        /// </summary>
        /// <param name="theApplication"></param>
        public void ApplicationProperties(InstalledApplication theApplication)
        {
            FormApplicationProperties form = new FormApplicationProperties(theApplication);
            if (form.ShowDialog() == DialogResult.OK)
            {
                workItem.GetActiveTabView().RefreshView();
                WorkItem.ExplorerView.RefreshView();
            }
        }


        #endregion Application Menu Functions

        #region Licensing Menu Functions

        /// <summary>
        /// Called to allow a new application license to be defined
        /// Note that this is only applicable if we are displaying the LicenseTabView or have selected an 
        /// application in the explorer view
        /// </summary>
        public void NewLicense()
        {
            // Get details of the currently selected application or OS entry
            List<InstalledApplication> selectedApplications = GetSelectedApplications();
            List<InstalledOS> selectedOSs = GetSelectedOSs();
            if (selectedApplications != null)
            {
                if (selectedApplications.Count == 1)
                    NewLicense(selectedApplications[0]);
                else
                    NewLicenses(selectedApplications);
            }
            else
            {
                if (selectedOSs != null)
                {
                    if (selectedOSs.Count == 1)
                        NewLicense(selectedOSs[0]);
                    else
                        NewLicenses(selectedOSs);
                }
            }
        }

        public void NewLicenses(List<InstalledOS> aForOSs)
        {
            // Create a new license and then call the license form to define it
            ApplicationLicense newLicense = new ApplicationLicense();

            FormLicenseProperties form = new FormLicenseProperties("Multiple Applications", newLicense);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ApplicationLicense lNewApplicationLicense = form.GetLicense;
                foreach (InstalledOS lForApplication in aForOSs)
                {
                    lNewApplicationLicense.LicenseID = 0;
                    lNewApplicationLicense.ApplicationID = lForApplication.OSid;
                    lNewApplicationLicense.ApplicationName = lForApplication.Name;
                    lNewApplicationLicense.Add();

                    // Update this installed application with the new license
                    lForApplication.LoadData();
                }

                // The license will have been added to the application already so we simply refresh the views
                workItem.ExplorerView.RefreshView();
                workItem.GetActiveTabView().RefreshView();

                ILaytonView applicationsTabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
                ((ApplicationsTabView)applicationsTabView).Presenter.DisplayPublishers();
            }
        }

        public void NewLicenses(List<InstalledApplication> aForApplications)
        {
            // Create a new license and then call the license form to define it
            ApplicationLicense newLicense = new ApplicationLicense();

            FormLicenseProperties form = new FormLicenseProperties("Multiple Applications", newLicense);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ApplicationLicense lNewApplicationLicense = form.GetLicense;
                foreach (InstalledApplication lForApplication in aForApplications)
                {
                    lNewApplicationLicense.LicenseID = 0;
                    lNewApplicationLicense.ApplicationID = lForApplication.ApplicationID;
                    lNewApplicationLicense.ApplicationName = lForApplication.Name;
                    lNewApplicationLicense.Add();

                    // Update this installed application with the new license
                    lForApplication.LoadData();
                }

                // The license will have been added to the application already so we simply refresh the views
                workItem.ExplorerView.RefreshView();
                workItem.GetActiveTabView().RefreshView();

                ILaytonView applicationsTabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
                ((ApplicationsTabView)applicationsTabView).Presenter.DisplayPublishers();
            }
        }

        public void AliasApplication(List<int> selectedApplicationIds)
        {
            //List<int> selectedApplicationIds = new List<int>();

            // Get details of the currently selected application or OS entry
            List<InstalledOS> selectedOSs = GetSelectedOSs();

            if (selectedOSs != null)
            {
                selectedApplicationIds.Clear();

                foreach (InstalledOS lOSId in selectedOSs)
                {
                    selectedApplicationIds.Add(lOSId.OSid);
                }

            }

            if (selectedApplicationIds.Count > 0)
            {
                FormAliasApplication form = new FormAliasApplication(selectedApplicationIds);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    workItem.ExplorerView.RefreshView();
                    workItem.GetActiveTabView().RefreshView();
                }
            }
        }

        /// <summary>
        /// Called to allow a new application license to be defined
        /// Note that this is only applicable if we are displaying the LicenseTabView or have selected an 
        /// application in the explorer view
        /// </summary>
        /// <param name="theApplication"></param>
        public void NewLicense(InstalledApplication forApplication)
        {
            // Create a new license and then call the license form to define it
            ApplicationLicense newLicense = new ApplicationLicense();
            newLicense.ApplicationID = forApplication.ApplicationID;
            newLicense.ApplicationName = forApplication.Name;
            //
            FormLicenseProperties form = new FormLicenseProperties(forApplication.Name, newLicense);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Update this installed application with the new license
                forApplication.LoadData();

                // The license will have been added to the application already so we simply refresh the views
                workItem.ExplorerView.RefreshView();
                workItem.GetActiveTabView().RefreshView();

                ILaytonView applicationsTabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
                ((ApplicationsTabView)applicationsTabView).Presenter.DisplayPublishers();
            }
        }

        /// <summary>
        /// Called to allow a new application license to be defined
        /// Note that this is only applicable if we are displaying the LicenseTabView or have selected an 
        /// application in the explorer view
        /// </summary>
        /// <param name="theApplication"></param>
        public void NewLicense(InstalledOS aOS)
        {
            // Create a new license and then call the license form to define it
            ApplicationLicense newLicense = new ApplicationLicense();
            newLicense.ApplicationID = aOS.OSid;
            newLicense.ApplicationName = aOS.Name;
            //
            FormLicenseProperties form = new FormLicenseProperties(aOS.Name, newLicense);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Update this installed application with the new license
                aOS.LoadData();

                // The license will have been added to the application already so we simply refresh the views
                //workItem.ExplorerView.RefreshView();
                workItem.GetActiveTabView().RefreshView();

                //ILaytonView applicationsTabView = WorkItem.Items[Layton.Cab.Interface.ViewNames.MainTabView] as ILaytonView;
                //((ApplicationsTabView)applicationsTabView).Presenter.DisplayPublishers();
            }
        }



        /// <summary>
        /// Wrapper around the main EditLicense function which does the work.  Here we simply recover
        /// the licencse currently selected in the explorer view.  The tab view call the extended version 
        /// of this function which takes the license itself as a parameter
        /// </summary>
        public void EditLicense()
        {
            // Recover the license that is to be edited from the LicenseTabView
            ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            if (tabView is LicensesTabView)
            {
                LicensesTabView licensesTabView = (LicensesTabView)tabView;
                ApplicationLicense theLicense = licensesTabView.GetSelectedLicense();
                if (theLicense == null)
                    return;
                EditLicense(theLicense);
            }
        }

        /// <summary>
        /// Called to initiate the editing of an existing license
        /// Note that this is only applicable if we are displaying the LicenseTabView
        /// </summary>
        public void EditLicense(ApplicationLicense theLicense)
        {
            // Get details of the currently selected application or OS entry
            List<InstalledApplication> selectedApplications = GetSelectedApplications();
            List<InstalledOS> selectedOSs = GetSelectedOSs();

            if (selectedApplications != null)
            {
                EditApplicationLicense(theLicense, selectedApplications[0]);
            }
            else
            {
                if (selectedOSs != null)
                {
                    EditApplicationLicense(theLicense, selectedOSs[0]);
                }
            }
        }

        private void EditApplicationLicense(ApplicationLicense theLicense, InstalledApplication aSelectedApplication)
        {
            FormLicenseProperties form = new FormLicenseProperties(aSelectedApplication.Name, theLicense);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                aSelectedApplication.LoadData();

                // The license will have been added to the application already so we simply refresh the active tab view
                workItem.ExplorerView.RefreshView();
                workItem.GetActiveTabView().RefreshView();
            }
        }

        private void EditApplicationLicense(ApplicationLicense theLicense, InstalledOS aSelectedOS)
        {
            FormLicenseProperties form = new FormLicenseProperties(aSelectedOS.Name, theLicense);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                aSelectedOS.LoadData();

                // The license will have been added to the application already so we simply refresh the active tab view
                workItem.ExplorerView.RefreshView();
                workItem.GetActiveTabView().RefreshView();
            }
        }



        /// <summary>
        /// Wrapper around the main EditLicense function which does the work.  Here we simply recover
        /// the licencse currently selected in the explorer view.  The tab view call the extended version 
        /// of this function which takes the license itself as a parameter
        /// </summary>
        public void DeleteLicense()
        {
            ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            if (tabView is LicensesTabView)
            {
                // Recover the license that is to be edited from the LicenseTabView
                LicensesTabView licensesTabView = (LicensesTabView)tabView;
                ApplicationLicense theLicense = licensesTabView.GetSelectedLicense();
                if (theLicense == null)
                    return;
                DeleteLicense(theLicense);
            }

            // refresh the tab view
            workItem.ExplorerView.RefreshView();
            workItem.GetActiveTabView().RefreshView();
        }


        /// <summary>
        /// Called to initiate the deletion of an existing license
        /// </summary>
        public void DeleteLicense(ApplicationLicense theLicense)
        {
            // Delete the license from the database
            theLicense.Delete();
        }



        /// <summary>
        /// Return the Application ID and Name associated with the currently selected item.  Note that we may
        /// have selected a child of an application and therefore will need to look at our parent
        /// </summary>
        /// <returns></returns>
        protected List<InstalledApplication> GetSelectedApplications()
        {
            return (_listApplications.Count == 0) ? null : _listApplications;
        }

        protected List<InstalledOS> GetSelectedOSs()
        {
            return (_selectedOSs.Count == 0) ? null : _selectedOSs;
        }

        #endregion Licensing Menu Functions

        #region Configuration Menu Functions

        /// <summary>
        /// Invoke the 'Manage Serial Numbers' form
        /// </summary>
        public void ManageSerialNumbers()
        {
            if (WorkItem.SettingsView != null)
            {
                IWorkspace settingsWorkspace = WorkItem.RootWorkItem.Workspaces[WorkspaceNames.SettingsTabWorkspace];
                _selectedTabKey = "serialnumbermappings";
                settingsWorkspace.Show(WorkItem.SettingsView);
                workItem.RootWorkItem.WorkItems[WorkItemNames.SettingsWorkItem].Activate();

            }
        }

        #endregion Configuration Menu Functions

        #region Export Menu Functions



        /// <summary>
        /// Export to PDF format 
        /// We call the appropriate grid view to handle this request themselves
        /// </summary>
        public void ExportToPDF()
        {
            ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            if (tabView is ApplicationsTabView)
                ((ApplicationsTabView)tabView).ExportToPDF();

            else if (tabView is InstancesTabView)
                ((InstancesTabView)tabView).ExportToPDF();

            else if (tabView is LicensesTabView)
                ((LicensesTabView)tabView).ExportToPDF();
        }


        /// <summary>
        /// Export to XLS format 
        /// We call the appropriate grid view to handle this request themselves
        /// </summary>
        public void ExportToXLS()
        {
            ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            if (tabView is ApplicationsTabView)
                ((ApplicationsTabView)tabView).ExportToXLS();

            else if (tabView is InstancesTabView)
                ((InstancesTabView)tabView).ExportToXLS();

            else if (tabView is LicensesTabView)
                ((LicensesTabView)tabView).ExportToXLS();
        }


        /// <summary>
        /// Export to XPS format 
        /// We call the appropriate grid view to handle this request themselves
        /// </summary>
        public void ExportToXPS()
        {
            ILaytonView tabView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            if (tabView is ApplicationsTabView)
                ((ApplicationsTabView)tabView).ExportToXPS();

            else if (tabView is InstancesTabView)
                ((InstancesTabView)tabView).ExportToXPS();

            else if (tabView is LicensesTabView)
                ((LicensesTabView)tabView).ExportToXPS();
        }

        #endregion Export Menu Functions


        public void SwitchActiveTabView(ILaytonView newTabView)
        {
            LaytonToolbarsController tbController = WorkItem.ToolbarsController;

            // Are we going to be displaying the Applications Tab View?
            RibbonGroup ribbonGroup = tbController.RibbonTab.Groups[RibbonNames.licensingGroupName];
            if (newTabView is ApplicationsTabView)
            {
                // We allow 'New License' but disable 'Edit License' and 'Delete License'
                ButtonTool buttonTool = (ButtonTool)ribbonGroup.Tools[ToolNames.NewLicense];
                buttonTool.SharedProps.Enabled = true;
                buttonTool = (ButtonTool)tbController.RibbonTab.Groups[RibbonNames.licensingGroupName].Tools[ToolNames.EditLicense];
                buttonTool.SharedProps.Enabled = false;
                buttonTool = (ButtonTool)tbController.RibbonTab.Groups[RibbonNames.licensingGroupName].Tools[ToolNames.DeleteLicense];
                buttonTool.SharedProps.Enabled = false;
            }

            // Are we going to display the Licenses Tab View?
            else if (newTabView is LicensesTabView)
            {
                // Allow all licensing buttons to operate
                ButtonTool buttonTool = (ButtonTool)tbController.RibbonTab.Groups[RibbonNames.licensingGroupName].Tools[ToolNames.NewLicense];
                buttonTool.SharedProps.Enabled = true;
                buttonTool = (ButtonTool)tbController.RibbonTab.Groups[RibbonNames.licensingGroupName].Tools[ToolNames.EditLicense];
                buttonTool.SharedProps.Enabled = true;
                buttonTool = (ButtonTool)tbController.RibbonTab.Groups[RibbonNames.licensingGroupName].Tools[ToolNames.DeleteLicense];
                buttonTool.SharedProps.Enabled = true;
            }

            base.SetTabView(newTabView);
        }


        /// <summary>
        /// Called to change the publisher for the specified application(s)
        /// </summary>
        public void ChangeApplicationPublisher(List<InstalledApplication> listDroppedApplications, string publisherName)
        {
            ApplicationsDAO lwDataAccess = new ApplicationsDAO();
            foreach (InstalledApplication droppedApplication in listDroppedApplications)
            {
                droppedApplication.Publisher = publisherName;
                lwDataAccess.ApplicationUpdatePublisher(droppedApplication.ApplicationID, publisherName);
            }
            workItem.ExplorerView.RefreshView();
            workItem.GetActiveTabView().RefreshView();
        }

        #endregion
    }
}
