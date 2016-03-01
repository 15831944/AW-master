using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.SmartParts;
using Layton.Cab.Interface;
//
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Utility;
using Microsoft.Practices.ObjectBuilder;
//
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
//
using Layton.AuditWizard.Common;
using Layton.AuditWizard.AuditWizardService;

namespace Layton.AuditWizard.Administration
{
    [SmartPart]
    public partial class EmailSettingsTabView : UserControl, ILaytonView, IAdministrationView
    {
        private LaytonWorkItem workItem;
		EmailSettingsTabViewPresenter presenter;

        [InjectionConstructor]
		public EmailSettingsTabView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
		}

		[CreateNew]
		public EmailSettingsTabViewPresenter Presenter
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

			// ...and refresh any settings
			emailConfigurationControl.InitializeEmailSettings();
		}

		/// <summary>
		/// Called as this tab is activated to ensure that we display the latest possible data
		/// This function comes from the IAdministrationView Interface
		/// </summary>
		public void Activate()
		{
			emailConfigurationControl.InitializeEmailSettings();
		}


		/// <summary>
		/// save function for the IAdministrationView Interface
		/// </summary>
		public void Save()
		{
			emailConfigurationControl.SaveEmailSettings();
		}

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }


		/// <summary>
		/// This is the handler for the GLOBAL EmailSettingsChanged Event which is fired when 
		/// the Email Settings have been updated elsewhere in the application.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		[EventSubscription(CommonEventTopics.EmailSettingsChanged)]
		public void EmailSettingsChangedHandler(object sender, EmailSettingsChangedEventArgs e)
		{
			emailConfigurationControl.InitializeEmailSettings();
		}

	}
}
