using System;
using System.Collections.Generic;
using System.Text;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Network
{
    public class ComputerEventArgs : EventArgs
    {
        private List<Asset> computers;

        public ComputerEventArgs(List<Asset> value) 
        {
            computers = value; 
        }

        public List<Asset> Computers
        {
            get { return computers; }
        }
    }
}
