using System;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI.EventBroker;

namespace Layton.AuditWizard.AuditTrail
{
    public partial class AuditTrailPurgeControl : UserControl
    {

		/// <summary>This holds the current filter settings - we are only interested however in the start
		/// and end date</summary>
		private AuditTrailPurgeEventArgs _auditTrailPurgeEventArgs = new AuditTrailPurgeEventArgs(DateTime.Now);

		/// <summary>
		/// Event declaration for when the Audit Trail Purge Date is changed.
		/// </summary>
		[EventPublication(EventTopics.PurgeRequested, PublicationScope.WorkItem)]
		public event EventHandler<AuditTrailPurgeEventArgs> PurgeRequested;


		public AuditTrailPurgeControl()
        {
            InitializeComponent();
            purgeDateCombo.Value = DateTime.Today;
        }

        public DateTime PurgeDate
        {
            get { return ((DateTime)purgeDateCombo.Value).Date; }
        }


		private void bnPurge_Click(object sender, EventArgs e)
		{
			if (PurgeRequested != null)
			{
				AuditTrailPurgeEventArgs eventargs = new AuditTrailPurgeEventArgs(PurgeDate);
				PurgeRequested(this, eventargs);
			}
		}
	}
}
