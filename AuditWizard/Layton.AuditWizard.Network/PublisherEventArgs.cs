using System;
using System.Collections.Generic;
using System.Text;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Applications
{
    public class PublishersEventArgs : EventArgs
    {
        private List<ApplicationPublisher> _listPublishers;

		public PublishersEventArgs(List<ApplicationPublisher> value)
        {
			_listPublishers = value;
        }

		public List<ApplicationPublisher> ApplicationPublishers
        {
			get { return _listPublishers; }
        }
    }
}
