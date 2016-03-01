using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
//
using Layton.Cab.Interface;
//
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;

namespace Layton.AuditWizard.AuditTrail
{
    public partial class RibbonDateRangeControl : UserControl
    {

		/// <summary>
		/// Event declaration for when the Audit Trail Start Date is changed.
		/// </summary>
		[EventPublication(EventTopics.FilterStartDateChanged, PublicationScope.Global)]
		public event EventHandler<AuditTrailFilterEventArgs> FilterStartDateChanged;

		/// <summary>
		/// Event declaration for when the Audit Trail End Date is changed.
		/// </summary>
		[EventPublication(EventTopics.FilterEndDateChanged, PublicationScope.Global)]
		public event EventHandler<AuditTrailFilterEventArgs> FilterEndDateChanged;

		public RibbonDateRangeControl()
        {
            InitializeComponent();
			DateTime today = DateTime.Today;
			DateTime lastWeek = today.AddDays(-7);

			// Set the start and end date noting that we must fire the changed events to ensure that the
			// values are picked up externally.
			startDateCombo.Value = lastWeek;
			endDateCombo.Value = today;
		}

        public DateTime StartDate
        {
            get { return ((DateTime)startDateCombo.Value).Date; }
        }

        public DateTime EndDate
        {
            get { return ((DateTime)endDateCombo.Value).Date.AddSeconds(24*60*60 - 1); }
        }

		/// <summary>
		/// Called as we change the start date
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void startDateCombo_ValueChanged(object sender, EventArgs e)
		{
			AnnounceFilterStartDateChanged();
		}

		private void AnnounceFilterStartDateChanged()
		{
			if (FilterStartDateChanged != null)
			{
				AuditTrailFilterEventArgs eventargs = new AuditTrailFilterEventArgs();
				eventargs.StartDate = StartDate;
				FilterStartDateChanged(this, eventargs);
			}
		}


		/// <summary>
		/// Called as we change the end date
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void endDateCombo_ValueChanged(object sender, EventArgs e)
		{
			AnnounceFilterEndDateChanged();
		}

		private void AnnounceFilterEndDateChanged()
		{
			if (FilterEndDateChanged != null)
			{
				AuditTrailFilterEventArgs eventargs = new AuditTrailFilterEventArgs();
				eventargs.EndDate = EndDate;
				FilterEndDateChanged(this, eventargs);
			}
		}

		private void RibbonDateRangeControl_Load(object sender, EventArgs e)
		{
			AnnounceFilterStartDateChanged();
			AnnounceFilterEndDateChanged();
		}
    }
}
