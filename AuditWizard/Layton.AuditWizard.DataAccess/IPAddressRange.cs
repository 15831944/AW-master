using System;
using System.Collections.Generic;
using System.Text;

namespace Layton.AuditWizard.DataAccess
{
    public class IPAddressRange
    {
        public enum IPType { Tcpip = 1, Snmp = 2 } ;

        public string IpStart { get; set; }
        public string IpEnd { get; set; }
        public bool Active { get; set; }
        public IPType TypeOfIp { get; set; }

        public IPAddressRange(string ipStart, string ipEnd, bool active, IPType ipType)
        {
            IpStart = ipStart;
            IpEnd = ipEnd;
            Active = active;
            TypeOfIp = ipType;
        }

        public IPAddressRange()
        {

        }
    }
}
