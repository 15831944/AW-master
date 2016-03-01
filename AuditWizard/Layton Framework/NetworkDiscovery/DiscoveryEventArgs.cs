using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.NetworkDiscovery
{
    public class DiscoveryUpdateEventArgs : EventArgs
    {
        private string computer;
        private string domain;
        private int discoveryCount;
        private int progressCount;
        private bool isDomainOnly;

        public DiscoveryUpdateEventArgs(string computer, string domain, int discoveredCount, int progressCount)
        {
            this.computer = computer;
            if (string.IsNullOrEmpty(computer))
            {
                isDomainOnly = true;
            }
            this.domain = domain;
            this.discoveryCount = discoveredCount;
            this.progressCount = progressCount;
        }

        public bool IsDomainOnly
        {
            get { return isDomainOnly; }
        }

        public int ProgressCount
        {
            get { return progressCount; }
        }

        public string Computer
        {
            get { return computer; }
        }

        public string Domain
        {
            get { return domain; }
        }

        public int DiscoveredComputerCount
        {
            get { return discoveryCount; }
        }
    }
}
