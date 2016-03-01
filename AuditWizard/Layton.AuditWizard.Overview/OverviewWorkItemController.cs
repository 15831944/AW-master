using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Windows.Forms;
using System.Runtime.InteropServices;
//
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.Utility;
//
using Layton.Cab.Interface;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.Administration;
using Layton.AuditWizard.Network;
using Layton.NetworkDiscovery;
using Layton.AuditWizard.DataAccess;

using log4net;

namespace Layton.AuditWizard.Overview
{
    public class OverviewWorkItemController : Layton.Cab.Interface.LaytonWorkItemController
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);

        OverviewWorkItem workItem;
        private static readonly ILog log = LogManager.GetLogger(typeof(OverviewWorkItemController));

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// This is the current publisher filter which will determine which publishers we display.  It can
        /// be updated by the PublisherFilterChanged event being fired.
        /// </summary>
        private String _publisherFilter = "";
        public string PublisherFilter
        {
            get { return _publisherFilter; }
            set { _publisherFilter = value; }
        }

        /// <summary>
        /// These flags guard against performin once-only activation/initialization too
        /// early and/or multiple times
        /// </summary>
        private bool _initializationRequired = false;
        private bool _initializationDone = false;

        public OverviewWorkItemController(WorkItem workItem)
            : base(workItem)
        {
            this.workItem = workItem as OverviewWorkItem;

            // We need to pull the publisher filter list from the database
            SettingsDAO lwDataAccess = new SettingsDAO();
            _publisherFilter = lwDataAccess.GetPublisherFilter();
        }

        public override void ActivateWorkItem()
        {
            base.ActivateWorkItem();

            // If the form is in a ready state - that is we are displaying the main window
            // then we can perform addional startup processing
            if (_initializationRequired && !_initializationDone)
                PerformInitializationActions();

            // After first time in flag initialization as required
            _initializationRequired = true;
            
            WorkItem.ExplorerView.RefreshView();
        }

        /// <summary>
        /// This function is called once and once only during a run of the program.  We call this
        /// from the main 'ActivateWorkItem' function which may be called multiple times.  As such
        /// we guard against performing these actions either too early or multiple times
        /// </summary>
        protected void PerformInitializationActions()
        {
            // Show that we have performed the initialization actions now
            _initializationDone = true;

            // First of all we need to allow the user to login to the system if security has been enabled
            UsersDAO awDataAccess = new UsersDAO();
            bool securityEnabled = awDataAccess.SecurityStatus();
            if (securityEnabled)
            {
                FormLogin loginform = new FormLogin();
                if (loginform.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                    return;
                }

                // If we are logged in as a USER then we need to disable the Administration Tab
                User loggedInUser = loginform.LoggedInUser;
                if (loggedInUser.AccessLevel != 0)
                {
                    List<WorkItem> workItemList = (List<WorkItem>)workItem.RootWorkItem.WorkItems.FindByType(typeof(AdministrationWorkItem));
                    AdministrationWorkItem administrationWorkItem = workItemList[0] as AdministrationWorkItem;
                    administrationWorkItem.Terminate();



                    AdministrationWorkItemController controller = administrationWorkItem.Controller as AdministrationWorkItemController;
                    controller.HideWorkspace();
                }
            }

            // Display the startup wizard now if required
            if (!Properties.Settings.Default.DoNotShowWizard)
                RunStartupWizard();

            // Ok Logged in - do we need to display any (New) Alerts?
            SettingsDAO lwDataAccess = new SettingsDAO();
            if (lwDataAccess.GetSettingAsBoolean(DatabaseSettingsKeys.Setting_ShowNewAlertsAtStartup, false))
                ShowNewAlerts();
        }

        /// <summary>
        /// Run the startup wizard
        /// </summary>
        public void RunStartupWizard()
        {
            log.Debug("Entering RunStartupWizard");

            // Get the Layton.LicenseWizard.NetworkWorkItemController object to pass to the wizard
            List<WorkItem> workItemList = (List<WorkItem>)workItem.RootWorkItem.WorkItems.FindByType(typeof(NetworkWorkItem));
            NetworkWorkItem netDiscWorkItem = workItemList[0] as NetworkWorkItem;
            NetworkWorkItemController controller = netDiscWorkItem.Controller as NetworkWorkItemController;

            // Get the Layton.NetworkDiscovery.NetworkDiscoveryWorkItemController object also
            workItemList = (List<WorkItem>)workItem.RootWorkItem.WorkItems.FindByType(typeof(NetworkDiscoveryWorkItem));
            NetworkDiscoveryWorkItem networkDiscoveryWorkItem = workItemList[0] as NetworkDiscoveryWorkItem;
            NetworkDiscoveryWorkItemController ndController = networkDiscoveryWorkItem.Controller as NetworkDiscoveryWorkItemController;

            // Save the active tab view as the discovery process may change it
            ILaytonView activeTabView = (ILaytonView)workItem.RootWorkItem.Workspaces[WorkspaceNames.TabWorkspace].ActiveSmartPart;
            FormStartupWizard wizard = new FormStartupWizard(this, controller, ndController);
            wizard.ShowDialog();

            OverviewTabView tabView = WorkItem.TabView as OverviewTabView;
            tabView.RefreshView();
        }


        /// <summary>
        /// Invoke the Alerts form if we have had some alerts generated since the last time this form was shown
        /// </summary>
        protected void ShowNewAlerts()
        {
            DateTime dateLastAlertsShown;
            string lastAlertsShown = Properties.Settings.Default.LastShownAlertDate;
            if (lastAlertsShown == "")
                dateLastAlertsShown = DateTime.Now.AddDays(-1);
            else
                dateLastAlertsShown = Convert.ToDateTime(lastAlertsShown);

            FormAlertLog form = new FormAlertLog(dateLastAlertsShown);

            // Now display the form if there were any alerts recovered
            if (form.AlertCount != 0)
            {
                form.ShowDialog();
                Properties.Settings.Default.LastShownAlertDate = DateTime.Now.ToString();
                Properties.Settings.Default.Save();
            }
        }


        /// <summary>
        /// This event is fired at the completion of a Network Discovery and is responsible for adding the 
        /// computers and groups discovered to the database.
        /// 
        /// Note that this event may be fired both as a result of running the Startup Wizard and as a result of
        /// running the discovery from the Network Discovery Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EventSubscription(Layton.NetworkDiscovery.EventTopics.NetworkDiscoveryComplete, ThreadOption.UserInterface)]
        public void NetworkDiscoveryCompleteHandler(object sender, DataEventArgs<List<string[]>> e)
        {
            try
            {
                LocationsDAO lwDataAccess = new LocationsDAO();
                string macAddress = String.Empty;
                string vendor = String.Empty;
                string ipAddress = String.Empty;

                // We need to get the root item as all of the domains need to be parented to this
                DataTable table = lwDataAccess.GetGroups(new AssetGroup(AssetGroup.GROUPTYPE.domain));
                AssetGroup rootGroup = new AssetGroup(table.Rows[0], AssetGroup.GROUPTYPE.domain);

                // Get the child domains - as domains are single level we do not need to recurse
                rootGroup.Populate(false, false, true);

                // We'll loop through the domains first and add then to the database recovering their ids so that 
                // we only have to do this once.
                for (int i = 0; i < e.Data.Count; i++)
                {
                    string assetName = e.Data[i][0];
                    string groupName = e.Data[i][1];

                    // Does this domain already exist?
                    AssetGroup childGroup = rootGroup.IsChildGroup(groupName);

                    // No - add it as a new group both to the database and to the parent
                    if (childGroup == null)
                    {
                        childGroup = new AssetGroup(AssetGroup.GROUPTYPE.domain);
                        childGroup.Name = groupName;
                        childGroup.ParentID = rootGroup.GroupID;
                        childGroup.GroupID = lwDataAccess.GroupAdd(childGroup);
                        rootGroup.Groups.Add(childGroup);
                    }

                    try
                    {
                        // Recover the IP address for the asset
                        ipAddress = Utility.GetIpAddress(assetName);

                        // try to get the MAC address and vendor from IP
                        IPAddress hostIPAddress = IPAddress.Parse(ipAddress);
                        byte[] ab = new byte[6];
                        int len = ab.Length;

                        int r = SendARP((int)hostIPAddress.Address, 0, ab, ref len);

                        macAddress = BitConverter.ToString(ab, 0, 6);
                    }
                    catch (Exception ex)
                    {
                        // if we hit an invalid IP error here we can just log and carry on
                        log.Error(String.Format("Error {0} with IP address [{1}]", ex.Message, ipAddress));
                    }

                    if (macAddress != String.Empty)
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(System.IO.Path.Combine(Application.StartupPath, "oui.txt")))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.StartsWith(macAddress.Substring(0, 8)))
                                {
                                    if (line.Substring(18).ToUpper().StartsWith("APPLE"))
                                        vendor = line.Substring(18);

                                    break;
                                }
                            }
                        }
                    }

                    // Now that we have the ID of the group (even if we just added the group) we can now
                    // add the asset to the database also.
                    Asset newAsset = new Asset();
                    newAsset.Name = assetName;
                    newAsset.MACAddress = macAddress.Replace('-', ':');
                    newAsset.Make = vendor;

                    AssetList assetList = new AssetList(new AssetDAO().GetAssets(0, AssetGroup.GROUPTYPE.userlocation, false), true);
                    bool bUpdateAsset = true;

                    foreach (Asset existingAsset in assetList)
                    {
                        if (assetName == existingAsset.Name)
                        {
                            // this asset already exists - only need to check if domain or IP have changed
                            // if they have, send it away to be updated
                            if (existingAsset.IPAddress != ipAddress || existingAsset.DomainID != childGroup.GroupID)
                            {
                                newAsset = existingAsset;
                                newAsset.IPAddress = newAsset.IPAddress != ipAddress ? ipAddress : newAsset.IPAddress;
                                newAsset.DomainID = newAsset.DomainID != childGroup.GroupID ? childGroup.GroupID : newAsset.DomainID;
                            }
                            else
                            {
                                // asset exists, nothing has changed so don't process
                                bUpdateAsset = false;
                            }
                            break;
                        }
                    }

                    if (bUpdateAsset)
                    {
                        newAsset.DomainID = childGroup.GroupID;
                        newAsset.IPAddress = ipAddress;

                        // Add the asset
                        newAsset.Add();
                    }

                    // Does this asset already exist?
                    //if (rootGroup.FindAsset(assetName) == null)
                    //{
                    //    newAsset.DomainID = childGroup.GroupID;
                    //    newAsset.IPAddress = ipAddress;

                    //    // Add the asset
                    //    newAsset.Add();
                    //}
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            //WorkItem.ExplorerView.RefreshView();
            //WorkItem.TabView.RefreshView();
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
            // Simply update our internal publisher filter with that specified
            _publisherFilter = e.PublisherFilter;

            // ...and force a refresh if we are the active explorer view
            ILaytonView activeExplorerView = (ILaytonView)WorkItem.RootWorkItem.Workspaces[WorkspaceNames.ExplorerWorkspace].ActiveSmartPart;
            if (activeExplorerView == WorkItem.ExplorerView)
            {
                WorkItem.TabView.RefreshView();
                WorkItem.ExplorerView.RefreshView();
            }
        }

        public void ConfigureDashboard()
        {
            //OverviewTabView tabView = WorkItem.TabView as OverviewTabView;
            //tabView.ConfigureWidgets();
        }

        public void DisplayDrilldownData(string drillDownReportName)
        {
            OverviewTabView tabView = WorkItem.TabView as OverviewTabView;
            tabView.DisplayDrilldownTabView(drillDownReportName);
        }

        /// <summary>
        /// Display the Alert Log Form
        /// </summary>
        public void AlertLog()
        {
            FormAlertLog form = new FormAlertLog(DateTime.Now.AddDays(-1));
            form.ShowDialog();
        }

        /// <summary>
        /// Display the Alert Log Form
        /// </summary>
        public void AddTasks()
        {
            FormTasks form = new FormTasks();
            form.ShowDialog();
        }
    }
}
