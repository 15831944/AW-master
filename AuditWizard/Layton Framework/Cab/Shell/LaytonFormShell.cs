using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Infragistics.Practices.CompositeUI.WinForms;
using Infragistics.Win.UltraWinExplorerBar;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI.SmartParts;
using Timer = System.Windows.Forms.Timer;

namespace AuditWizardv8
{
    [SmartPart]
    public partial class LaytonFormShell : Form
    {
        private Timer timer;

        // PAYG or standard version
        private bool _standardVersion;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LaytonFormShell()
        {
            _standardVersion = true;
            PerformStartupTasks();
            InitializeComponent();

            try
            {
                // set the 32x32 icon on the ribbon menu
                toolbarsWorkspace.Ribbon.ApplicationMenuButtonImage = new Bitmap(Properties.Settings.Default.appLogo32);

                // set the form's icon
                this.Icon = new Icon(Properties.Settings.Default.appIcon);
            }
            catch
            {
                // continue using the default logos
            }

            Name = Properties.Settings.Default.appName;
            Text = Properties.Settings.Default.appName;
            explorerWorkspace.CreationFilter = new NavigationOverflowButtonFilter();
            tabWorkspace.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
            explorerWorkspace.SelectedGroupChanging += new SelectedGroupChangingEventHandler(explorerWorkspace_SelectedGroupChanging);

            timer = new Timer();

            timer.Interval = 84600000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void PerformStartupTasks()
        {
            if (!DatabaseConnection.TestConnection())
            {
                MessageBox.Show(
                    "AuditWizard was unable to connect to the database. Please check in the log file for further details." +
                    Environment.NewLine + Environment.NewLine + "AuditWizard will now close.",
                    "Database connection error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                Environment.Exit(0);
            }

            // check for PAYG users
            CheckCustomerPaid();

            // need to check if the HB stored procs have been added
            // this has to be done here because a user may have patched and not gone through a create SQL DB method since 8.1.4
            CheckHBStoredProcs();

            // also delete the old log files Bug #553
            //if (File.Exists(Path.Combine(Application.StartupPath, "AuditWizard_logfile.log")))
            //    File.Delete(Path.Combine(Application.StartupPath, "AuditWizard_logfile.log"));

            //if (File.Exists(Path.Combine(Application.StartupPath, "AuditWizardService.txt")))
            //    File.Delete(Path.Combine(Application.StartupPath, "AuditWizardService.txt"));

            // check latest version of tables are in place (for upgraders)
            new ReportsDAO().CheckTablePresent();
            new TaskSchedulesDAO().CheckTablePresent();
            new ReportSchedulesDAO().CheckTablePresent();
            new PublisherAliasDAO().CheckTablePresent();
            new PublisherAliasAppDAO().CheckTablePresent();
            new NewsFeedDAO().CheckTablePresent();
            new LocationSettingsDAO().CheckTablePresent();
            new AuditedItemsDAO().CheckNewFieldsAvailable();
            new SettingsDAO().AlterValueColumnType();
            new LayIpAddressDAO().CheckTablePresent();
            new AuditedItemsDAO().UpdateHardwareDisplayCategory();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Enabled = false;

            CheckSupportExpiry();
            CheckNewVersionAvailable();
            CheckCustomerPaid();

            timer.Enabled = true;
            timer.Start();
        }

        public void CheckCustomerPaid()
        {
            if (_standardVersion)
                return;

            if (IsTrialVersion())
                return;

            bool lCustomerPaid = false;

            try
            {
                AuditWizardWebService.CustomerWebService lWebService = new AuditWizardWebService.CustomerWebService();
                lCustomerPaid = lWebService.CheckCustomerPaid(ConfigurationManager.AppSettings["CompanyID"].ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                lCustomerPaid = false;
            }
            finally
            {
                if (!lCustomerPaid)
                {
                    FormAWMessageBox form = new FormAWMessageBox(
                        "It appears that your account has expired or you are not currently connected to the Internet." + Environment.NewLine + Environment.NewLine +
                        "Please contact your local Layton Technology support office.");
                    form.ShowDialog();

                    System.Diagnostics.Process.Start("http://www.laytontechnology.com/contact-layton-technology.html");
                    Environment.Exit(0);
                }
            }
        }

        private string CheckSupportExpiry()
        {
            string supportMsg = "";

            // only check support expiry if we are in standard application (not PAYG)
            if (!_standardVersion)
                return supportMsg;

            try
            {
                AuditWizardWebService.CustomerWebService lWebService = new AuditWizardWebService.CustomerWebService();
                DateTime lSupportExpiryDate = lWebService.GetCustomerExpiryDate(ConfigurationManager.AppSettings["CompanyID"].ToString());
                bool lNotifySupport = new SettingsDAO().GetSettingAsBoolean("NotifySupport", true);

                if (lNotifySupport)
                {
                    if (lSupportExpiryDate != DateTime.MinValue)
                    {
                        if ((lSupportExpiryDate < DateTime.Now.AddMonths(1)) && (lSupportExpiryDate > DateTime.Now))
                        {
                            return "Your AuditWizard support will expire on " + lSupportExpiryDate.ToLongDateString();
                        }

                        else if (lSupportExpiryDate < DateTime.Now)
                        {
                            return "Your AuditWizard support expired on " + lSupportExpiryDate.ToLongDateString();
                        }
                    }
                }
                else
                {
                    // if support is more than a month away, check if NotifySupport is set to False - 
                    // if so we can assume that they have renewed support (they have been prompted before) 
                    // we should set the flag to true so they are notified the next time support is about to expire
                    if (lSupportExpiryDate > DateTime.Now.AddMonths(1))
                        new SettingsDAO().SetSetting("NotifySupport", true);
                }
            }
            catch (Exception)
            {
            }

            return supportMsg;
        }

        private string CheckNewVersionAvailable()
        {
            string alertMsg = "";

            try
            {
                string lLastAlertedVersion = new SettingsDAO().GetSetting("LatestVersionAlert", false);

                AuditWizardWebService.CustomerWebService lWebService = new AuditWizardWebService.CustomerWebService();
                string lNewVersion = lWebService.GetLatestVersionNumber();

                if (lNewVersion.Equals(lLastAlertedVersion))
                    return alertMsg;

                string[] lLatestAppVersion = lNewVersion.Split('.');
                string[] lCurrentAppVersion = Application.ProductVersion.Split('.');

                bool lNewVersionAvailable = Convert.ToInt32(lLatestAppVersion[0]) > Convert.ToInt32(lCurrentAppVersion[0]);

                if (!lNewVersionAvailable)
                {
                    lNewVersionAvailable =
                        (Convert.ToInt32(lLatestAppVersion[0]) == Convert.ToInt32(lCurrentAppVersion[0])) &&
                        (Convert.ToInt32(lLatestAppVersion[1]) > Convert.ToInt32(lCurrentAppVersion[1]));
                }

                if (!lNewVersionAvailable)
                {
                    lNewVersionAvailable =
                        (Convert.ToInt32(lLatestAppVersion[0]) == Convert.ToInt32(lCurrentAppVersion[0])) &&
                        (Convert.ToInt32(lLatestAppVersion[1]) == Convert.ToInt32(lCurrentAppVersion[1]) &&
                        (Convert.ToInt32(lLatestAppVersion[2]) > Convert.ToInt32(lCurrentAppVersion[2])));
                }

                if (lNewVersionAvailable)
                {
                    new SettingsDAO().SetSetting("LatestVersionAlert", lNewVersion, false);
                    NewsFeed.AddNewsItem(NewsFeed.Priority.Information, String.Format("AuditWizard version {0} is now available.", lNewVersion));

                    return String.Format("AuditWizard version {0} is now available. Click here to download the latest version.", lNewVersion);
                }
            }
            catch (Exception)
            {
            }

            return alertMsg;
        }


        /// <summary>
        /// Gets/Sets the whether the ExplorerWorkspace is collapsed (hidden).
        /// </summary>
        public bool ExplorerWorkspaceCollapsed
        {
            get
            {
                return splitContainer1.Panel1Collapsed;
            }
            set
            {
                splitContainer1.Panel1Collapsed = value;
            }
        }

        void explorerWorkspace_SelectedGroupChanging(object sender, CancelableGroupEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            // hide all the explorer groups except for the one just selected
            for (int i = 0; i <= explorerWorkspace.Groups.Count - 1; i++)
            {
                UltraExplorerBarGroup group = explorerWorkspace.Groups[i];
                if (group != e.Group)
                {
                    group.Visible = false;
                }
            }

            this.Cursor = Cursors.Default;
        }

        internal UltraToolbarsManagerWorkspace ToolbarsWorkspace
        {
            get { return toolbarsWorkspace; }
        }

        private void toolbarsWorkspace_BeforeToolbarListDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolbarListDropdownEventArgs e)
        {
            e.Cancel = true;
        }

        private void explorerWorkspace_NavigationOptionsDialogDisplaying(object sender, CancelableNavigationOptionsDialogDisplayingEventArgs e)
        {
            e.Cancel = true;
        }

        private void explorerWorkspace_ContextMenuInitializing(object sender, CancelableContextMenuInitializingEventArgs e)
        {
            e.Cancel = true;
        }

        private void explorerWorkspace_NavigationContextMenuInitializing(object sender, CancelableNavigationContextMenuInitializingEventArgs e)
        {
            e.Cancel = true;
        }

        private void LaytonFormShell_KeyUp(object sender, KeyEventArgs e)
        {
            // check if the F5 key was pressed
            if (e.KeyCode == Keys.F5)
            {
                // Refresh the TabView and ExplorerView
                ILaytonView explorerView = (ILaytonView)explorerWorkspace.ActiveSmartPart;
                explorerView.RefreshView();
                ILaytonView tabView = (ILaytonView)tabWorkspace.ActiveSmartPart;
                tabView.RefreshView();
            }
        }

        private void LaytonFormShell_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            backgroundWorker1.RunWorkerAsync();
        }

        private bool IsTrialVersion()
        {
            LaytonCabShellWorkItem workItem = new LaytonCabShellWorkItem();
            LaytonProductKey productKey = workItem.LoadProductKey();

            return productKey.IsTrial;
        }

        private void CheckHBStoredProcs()
        {
            new VersionDAO().CheckHBStoredProcs();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Dictionary<string, string> startupStrings = new Dictionary<string, string>();
            startupStrings.Add("NewVersion", CheckNewVersionAvailable());
            startupStrings.Add("SupportExpired", CheckSupportExpiry());

            e.Result = startupStrings;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dictionary<string, string> results = (Dictionary<string, string>)e.Result;

            if (results != null)
            {
                string newVersionString = results["NewVersion"];
                string supportExpiredString = results["SupportExpired"];

                if (newVersionString != "")
                    DesktopAlert.ShowDesktopAlertForNewVersion(newVersionString);

                if (supportExpiredString != "")
                {
                    FormSupportExpiry form = new FormSupportExpiry(supportExpiredString);
                    form.ShowDialog();
                }
            }
        }
    }
}