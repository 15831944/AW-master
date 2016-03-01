using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using Infragistics.Win.UltraWinExplorerBar;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;

namespace Layton.AuditWizard.Overview
{
    [SmartPart]
    public partial class OverviewExplorerView : UserControl, ILaytonView
    {
        private LaytonWorkItem workItem;
        private Timer timer;
        private bool serviceRunning;

        [InjectionConstructor]
        public OverviewExplorerView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
            serviceRunning = false;

            CheckServiceRunning();

            timer = new Timer();
            timer.Interval = 10000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            RefreshView();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Enabled = false;

            CheckServiceRunning();

            timer.Enabled = true;
            timer.Start();
        }

        private void CheckServiceRunning()
        {
            UltraExplorerBarItem item = this.overviewExplorerBar.Groups["auditwizardservice"].Items[0];

            if (item != null)
            {
                foreach (Process process in Process.GetProcesses())
                {
                    if (process.ProcessName == "AuditWizardService")
                    {
                        serviceRunning = true;
                        item.Settings.AppearancesSmall.Appearance.Image = global::Layton.AuditWizard.Overview.Properties.Resources.green_button;
                        item.Text = "Running";
                        return;
                    }
                }

                item.Text = "Not Running";
                item.Settings.AppearancesSmall.Appearance.Image = global::Layton.AuditWizard.Overview.Properties.Resources.red_button;
                serviceRunning = false;
            }
        }

        public void RefreshView()
        {
            // Update statistics
            UpdateAssetStatistics();
            UpdateAuditHistoryStatistics();
            UpdateAlertStatistics();
            UpdateSupportStatistics();
            UpdateLicenseStatistics();
        }

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }


        /// <summary>
        /// Update the values specified for the Asset Statistics
        /// </summary>
        private void UpdateAssetStatistics()
        {
            // Get the current statistics
            StatisticsDAO awDataAccess = new StatisticsDAO();
            DataTable statisticsTable = awDataAccess.AssetStatistics();

            // Into an object for easier viewing
            AssetStatistics statistics = new AssetStatistics(statisticsTable.Rows[0]);

            // Update the Computer Statistics Explorer Bar
            //  Total, audited, not audited, stoc, in use ,pending, disposed
            //
            UltraExplorerBarGroup assetStatistics = overviewExplorerBar.Groups["assetstatistics"];
            if (assetStatistics == null)
                return;

            assetStatistics.Items["numberindatabase"].Text = StatisticTitles.ComputersDiscovered + statistics.Discovered.ToString();

            assetStatistics.Items["computersaudited"].Text = StatisticTitles.ComputersAudited + statistics.Audited.ToString();
            if (statistics.MostRecentAudit.Ticks == 0)
                assetStatistics.Items["computersnotaudited"].Text = StatisticTitles.ComputerLastAudit + "No Audits Run";
            else
                assetStatistics.Items["computersnotaudited"].Text = StatisticTitles.ComputerLastAudit + statistics.MostRecentAudit.ToString("dd MMM HH:mm");
            //
            assetStatistics.Items["assetsinstock"].Text = StatisticTitles.AssetsInStock + statistics.Stock.ToString();
            assetStatistics.Items["assetsinuse"].Text = StatisticTitles.AssetsInUse + statistics.InUse.ToString();
            assetStatistics.Items["assetspendingdisposal"].Text = StatisticTitles.AssetsPending + statistics.PendingDisposal.ToString();
            assetStatistics.Items["assetsdisposed"].Text = StatisticTitles.AssetsDisposed + statistics.Disposed.ToString();

            statisticsTable = awDataAccess.AuditStatistics();
            AuditStatistics auditStatistics = new AuditStatistics(statisticsTable.Rows[0]);

            assetStatistics.Items["auditagentsdeployed"].Text = StatisticTitles.AgentsDeployed + auditStatistics.DeployedAgents.ToString();
        }

        /// <summary>
        /// Update the Audit History Statistics
        /// </summary>
        private void UpdateAuditHistoryStatistics()
        {
            StatisticsDAO awDataAccess = new StatisticsDAO();
            DataTable statisticsTable = awDataAccess.AuditHistoryStatisticsForDashboard();
            AuditHistoryStatistics statistics = new AuditHistoryStatistics(statisticsTable.Rows[0]);

            // Update the ApplicationID Statistics Explorer Bar
            UltraExplorerBarGroup auditHistory = overviewExplorerBar.Groups["audithistory"];
            if (auditHistory == null)
                return;

            auditHistory.Items["auditedtoday"].Text = StatisticTitles.AuditedToday + statistics.AuditedToday.ToString();
            auditHistory.Items["notaudited7"].Text = StatisticTitles.NotAudited7 + statistics.NotAudited7.ToString();
            auditHistory.Items["notaudited14"].Text = StatisticTitles.NotAudited14 + statistics.NotAudited14.ToString();
            auditHistory.Items["notaudited30"].Text = StatisticTitles.NotAudited30 + statistics.NotAudited30.ToString();
            auditHistory.Items["notaudited90"].Text = StatisticTitles.NotAudited90 + statistics.NotAudited90.ToString();
            auditHistory.Items["neveraudited"].Text = StatisticTitles.NotAudited + statistics.NotAudited.ToString();
        }

        /// <summary>
        /// Update the values specified for the Alert Statistics
        /// </summary>
        private void UpdateAlertStatistics()
        {
            StatisticsDAO awDataAccess = new StatisticsDAO();
            DataTable statisticsTable = awDataAccess.AlertStatistics();
            AlertStatistics statistics = new AlertStatistics(statisticsTable.Rows[0]);

            // Update the ApplicationID Statistics Explorer Bar
            UltraExplorerBarGroup alerts = overviewExplorerBar.Groups["alerts"];
            if (alerts == null)
                return;

            string lastAlertDate = statistics.LastAlert.Ticks == 0 ? "<none>" : statistics.LastAlert.ToString("dd MMM HH:mm");

            alerts.Items["lastalerton"].Text = StatisticTitles.LastAlert + lastAlertDate;
            alerts.Items["alertstoday"].Text = StatisticTitles.AlertsToday + statistics.AlertsToday.ToString();
            alerts.Items["alertsthisweek"].Text = StatisticTitles.AlertsThisWeek + statistics.AlertsThisWeek.ToString();
            alerts.Items["alertsthismonth"].Text = StatisticTitles.AlertsThisMonth + statistics.AlertsThisMonth.ToString();
        }


        /// <summary>
        /// Update the values specified for the Support Statistics
        /// </summary>
        private void UpdateSupportStatistics()
        {
            StatisticsDAO awDataAccess = new StatisticsDAO();
            DataTable statisticsTable, statisticsTableAsset;
            SupportContractStatistics statistics, statisticsAsset;
            statisticsTable = awDataAccess.SupportStatistics();
            statisticsTableAsset = awDataAccess.SupportStatisticsAsset();
            statistics = new SupportContractStatistics(statisticsTable.Rows[0]);
            statisticsAsset = new SupportContractStatistics(statisticsTableAsset.Rows[0]);


            int iCountExpired = statistics.Expired + statisticsAsset.Expired;
            int iCountExpireToday = statistics.ExpireToday + statisticsAsset.ExpireToday;
            int iCountExpireWeek = statistics.ExpireWeek + statisticsAsset.ExpireWeek;
            int iCountExpireMonth = statistics.ExpireMonth + statisticsAsset.ExpireMonth;


            //
            UltraExplorerBarGroup supportContracts = overviewExplorerBar.Groups["supportcontracts"];
            if (supportContracts == null)
                return;
            supportContracts.Items["expired"].Text = StatisticTitles.SupportExpired + iCountExpired.ToString();
            supportContracts.Items["expirestoday"].Text = StatisticTitles.SupportExpireToday + iCountExpireToday.ToString();
            supportContracts.Items["expiresthisweek"].Text = StatisticTitles.SupportExpireThisWeek + iCountExpireWeek.ToString();
            supportContracts.Items["expiresthismonth"].Text = StatisticTitles.SupportExpireThisMonth + iCountExpireMonth.ToString();
        }


        /// <summary>
        /// Update the License Statistics
        /// </summary>
        private void UpdateLicenseStatistics()
        {
            // Recover the total license count from the product key
            Layton.Cab.Interface.LaytonProductKey key = WorkItem.RootWorkItem.Items[Layton.Cab.Interface.MiscStrings.ProductKey] as Layton.Cab.Interface.LaytonProductKey;
            int licenseCount = (key.IsTrial) ? 10 : key.AssetCount;

            UltraExplorerBarGroup licenses = overviewExplorerBar.Groups["licensing"];
            if (licenses == null)
                return;

            // Set this in the explorer bar
            licenses.Items["licensecount"].Text = StatisticTitles.LicensedFor + licenseCount.ToString();

            // Now we need a count of the 'licensable' assets within the database - this is the number of assets
            // which have been audited excluding child assets and any which have been flagged as 'disposed of'
            AssetDAO awDataAccess = new AssetDAO();
            int licensesUsed = awDataAccess.LicensedAssetCount();
            licenses.Items["licensesused"].Text = StatisticTitles.LicensesUsed + licensesUsed.ToString();
        }

        private void overviewExplorerBar_ContextMenuInitializing(object sender, CancelableContextMenuInitializingEventArgs e)
        {
            e.Cancel = true;
        }

        private void overviewExplorerBar_ItemClick(object sender, ItemEventArgs e)
        {
            if (e.Item.Text.StartsWith("License Count") || e.Item.Text.StartsWith("Licenses In Use"))
                return;

            else if (e.Item.Text.StartsWith("Running") || e.Item.Text.StartsWith("Not Running"))
            {
                FormAuditWizardServiceControl serviceForm = new FormAuditWizardServiceControl();
                serviceForm.ShowDialog();
                CheckServiceRunning();
                return;
            }

            LaytonWorkItem workItem = WorkItem as LaytonWorkItem;
            ((OverviewWorkItemController)workItem.Controller).DisplayDrilldownData(e.Item.Text);
        }
    }
}