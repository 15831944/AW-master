using System;
using System.Collections.Generic;
using System.Text;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.AuditTrail
{
	public class AuditTrailFilterEventArgs : EventArgs
	{
		private AuditTrailEntry.CLASS _requiredClass;
		private DateTime _startDate = DateTime.Now.Date;
		private DateTime _endDate = DateTime.Now.Date;

		public AuditTrailFilterEventArgs()
		{
			_requiredClass = AuditTrailEntry.CLASS.all;
			_startDate = DateTime.Now.Date;
			_endDate = DateTime.Now.Date;
		}

		public AuditTrailFilterEventArgs(AuditTrailEntry.CLASS requiredClass ,DateTime startDate ,DateTime endDate)
		{
			_requiredClass = requiredClass;
			_startDate = startDate;
			_endDate = endDate;
		}

		#region Properties

		public AuditTrailEntry.CLASS RequiredClass
		{
			get { return _requiredClass; }
			set { _requiredClass = value; }
		}

		public DateTime StartDate
		{
			get { return _startDate; }
			set { _startDate = value; }
		}

		public DateTime EndDate
		{
			get { return _endDate; }
			set { _endDate = value; }
		}
		#endregion Properties
	}


	/// <summary>
	/// Event arguments for the 'Audit Trail Purge Date Changed' Event - simply supplies the new value
	/// </summary>
	public class AuditTrailPurgeEventArgs : EventArgs
	{
		private DateTime _purgeDate;

		public AuditTrailPurgeEventArgs(DateTime value)
		{
			_purgeDate = value;
		}

		public DateTime PurgeDate
		{
			get { return _purgeDate; }
		}
	}

}
