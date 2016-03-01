using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.SmartParts;
using Layton.Cab.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Utility;
using Microsoft.Practices.ObjectBuilder;
//
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

using PickerSample;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class ServiceTabView : UserControl, ILaytonView, IAdministrationView
    {
        private LaytonWorkItem workItem;
        ServiceTabViewPresenter presenter;

        // The Layton Service Controller object for the AuditWizardService
        private AuditWizardServiceController _AuditWizardServiceController = new AuditWizardServiceController();

        [InjectionConstructor]
        public ServiceTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
        }

        [CreateNew]
        public ServiceTabViewPresenter Presenter
        {
            set { presenter = value; presenter.View = this; presenter.Initialize(); }
            get { return presenter; }
        }

        public void RefreshViewSinglePublisher()
        {
        }

        /// <summary>
        /// Refresh the current view
        /// </summary>
        public void RefreshView()
        {
            base.Refresh();
            ShowAuditWizardServiceStatus();
        }

        public void Activate()
        {
            ShowAuditWizardServiceStatus();
        }


        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }

        private void bnAuditWizardServiceControl_Click(object sender, EventArgs e)
        {
            // Show the service control form
            FormAuditWizardServiceControl serviceControl = new FormAuditWizardServiceControl();
            serviceControl.ShowDialog();

            // Update the status indicator
            ShowAuditWizardServiceStatus();
        }

        public void Save()
        {

        }

        /// <summary>
        /// Display the appropriate service status indicator
        /// </summary>
        protected void ShowAuditWizardServiceStatus()
        {
            LaytonServiceController.ServiceStatus serviceStatus = _AuditWizardServiceController.CheckStatus();

            switch (serviceStatus)
            {
                case LaytonServiceController.ServiceStatus.Running:
                    pbAuditWizardServiceStatus.Image = Properties.Resources.active;
                    break;
                case LaytonServiceController.ServiceStatus.Stopped:
                    pbAuditWizardServiceStatus.Image = Properties.Resources.stopped;
                    break;
                case LaytonServiceController.ServiceStatus.NotInstalled:
                    pbAuditWizardServiceStatus.Image = Properties.Resources.notinstalled;
                    break;
                default:
                    pbAuditWizardServiceStatus.Image = Properties.Resources.unavailable;
                    break;
            }

            // Get the setting for the 'Ping' check box
            Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
            try
            {
                cbUsePing.Checked = Convert.ToBoolean(config.AppSettings.Settings["PingConnections"].Value);
            }
            catch (Exception)
            {
                cbUsePing.Checked = true;
            }
        }

        private void cbUsePing_CheckStateChanged(object sender, EventArgs e)
        {
            // Save the check state of the 'Ping'
            Configuration config = ConfigurationManager.OpenExeConfiguration(Path.Combine(Application.StartupPath, "AuditWizardv8.exe"));
            config.AppSettings.Settings["PingConnections"].Value = cbUsePing.Checked.ToString();
            config.Save();
        }
    }
}
