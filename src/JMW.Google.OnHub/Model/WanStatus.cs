using System.Collections.Generic;

namespace JMW.Google.OnHub.Model
{
    public class WanStatus
    {
        public string Type { get; set; }
        public string StaticIpAddress { get; set; }
        public string StaticNetMask { get; set; }
        public string StaticGateway { get; set; }
        public string PppoeUsername { get; set; }

        public string OperIpAddress { get; set; }
        public string OperGateway { get; set; }
        public string OperLinkSpeedMbps { get; set; }
        public string PrimaryWanInterface { get; set; }
        public HashSet<string> IPv4DnsServers { get; set; } = new HashSet<string>();
    }
}