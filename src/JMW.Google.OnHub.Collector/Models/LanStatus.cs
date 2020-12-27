using System.Collections.Generic;

namespace JMW.Google.OnHub.Collector.Models
{
    public class LanStatus
    {
        public string OperIpAddress { get; set; }
        public string OperNetMask { get; set; }
        public string OperDhcpPoolBegin { get; set; }
        public string OperDhcpPoolEnd { get; set; }
        public string DnsType { get; set; }
        public HashSet<string> IPv4DnsServers { get; set; } = new HashSet<string>();
        public HashSet<string> IPv6DnsServers { get; set; } = new HashSet<string>();
    }
}