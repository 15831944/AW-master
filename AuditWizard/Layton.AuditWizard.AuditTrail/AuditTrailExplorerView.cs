using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
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

namespace Layton.AuditWizard.AuditTrail
{
    [SmartPart]
    public partial class AuditTrailExplorerView : UserControl, ILaytonView
    {
        private LaytonWorkItem workItem;
		private AuditTrailFilterEventArgs _filterEventArgs = new AuditTrailFilterEventArgs();

		/// <summary>
		/// Event declaration for when the Audit Trail Filter is changed.  
		/// Anyone can subscribe to this event and will be notified when any of the filters that can be
		/// applied to the audit trail records are changed.
		/// </summary>
		[EventPublication(EventTopics.AuditTrailFilterChanged, PublicationScope.WorkItem)]
		public event EventHandler<AuditTrailFilterEventArgs> FilterChanged;

        [InjectionConstructor]
        public AuditTrailExplorerView([ServiceDependency] WorkItem workItem)
        {
            this.workItem = workItem as LaytonWorkItem;
            InitializeComponent();
			this.Paint += new PaintEventHandler(AuditTrailExplorerView_Paint);
		}

        public void RefreshView()
        {
            base.Refresh();
        }

        public void RefreshViewSinglePublisher()
        {
        }

        public LaytonWorkItem WorkItem
        {
            get { return workItem; }
        }

		void AuditTrailExplorerView_Paint(object sender, PaintEventArgs e)
		{
			Image bgImg = Properties.Resources.audittrail_view_ghosted_96;
			e.Graphics.DrawImage(bgImg, (this.Width - bgImg.Width - 30), (this.Height - bgImg.Height - 30));
		}

		#region Filter Changes Functions

		/// <summary>
		/// Called when we are informed that the filter start date has been changed
		/// </summary>
		/// <param name="startDate"></param>
		public void FilterStartDateChanged(DateTime startDate)
		{
			_filterEventArgs.StartDate = startDate;
			FireFilterChangedEvent();
		}


		/// <summary>
		/// Called when we are informed that the filter end date has been changed
		/// </summary>
		/// <param name="endDate"></param>
		public void FilterEndDateChanged(DateTime endDate)
		{
			_filterEventArgs.EndDate = endDate;
			FireFilterChangedEvent();
		}


		/// <summary>
		/// Fire a 'Filter Changed' event with the latest filter settings
		/// </summary>
		private void FireFilterChangedEvent()
		{
			if (FilterChanged != null)
				FilterChanged(this, _filterEventArgs);
		}
		#endregion Filter Changes Functions

        private void rdLicenseChanges_CheckedChanged(object sender, EventArgs e)
        {
            if (rdLicenseChanges.Checked)
            {
                _filterEventArgs.RequiredClass = AuditTrailEntry.CLASS.license;
                FireFilterChangedEvent();
            }
        }

        private void rdPropertyChanges_CheckedChanged(object sender, EventArgs e)
        {
            if (rdPropertyChanges.Checked)
            {
                _filterEventArgs.RequiredClass = AuditTrailEntry.CLASS.application_changes;
                FireFilterChangedEvent();
            }
        }

        private void rdAssets_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAssetChanges.Checked)
            {
                _filterEventArgs.RequiredClass = AuditTrailEntry.CLASS.asset;
                FireFilterChangedEvent();
            }
        }

		private void rbSuppliers_CheckedChanged(object sender, EventArgs e)
		{
			if (rbSuppliers.Checked)
			{
				_filterEventArgs.RequiredClass = AuditTrailEntry.CLASS.supplier;
				FireFilterChangedEvent();
			}
		}

		private void rbUsers_CheckedChanged(object sender, EventArgs e)
		{
			if (rbUsers.Checked)
			{
				_filterEventArgs.RequiredClass = AuditTrailEntry.CLASS.user;
				FireFilterChangedEvent();
			}
		}

		private void rbAll_CheckedChanged(object sender, EventArgs e)
		{
			if (rbAll.Checked)
			{
				_filterEventArgs.RequiredClass = AuditTrailEntry.CLASS.all;
				FireFilterChangedEvent();
			}
		}
	}
}
