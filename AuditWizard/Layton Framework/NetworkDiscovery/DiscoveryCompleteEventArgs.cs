using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.NetworkDiscovery
{
    public class DiscoveryCompleteEventArgs : EventArgs
    {
        private List<string[]> computers;
        private List<string> domains;

        public DiscoveryCompleteEventArgs(List<string> domains, List<string[]> computers)
        {
            this.computers = computers;
            this.domains = domains;
        }

        public List<string> DomainList
        {
            get { return domains; }
        }

        public List<string[]> ComputerList
        {
            get { return computers; }
        }
    }
}