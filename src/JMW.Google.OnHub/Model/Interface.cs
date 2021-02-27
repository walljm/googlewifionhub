using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JMW.Google.OnHub.Model
{
    public class Interface
    {
        [Required] public string IfIndex { get; set; }
        [Required] public string Name { get; set; }
        public string Info { get; set; }
        public string State { get; set; }
        public string MAC { get; set; }
        public string BRD { get; set; }

        public string MTU { get; set; }
        public string Qdisc { get; set; }
        public string Group { get; set; }
        public string Qlen { get; set; }
        public string Link { get; set; }
        public string Promiscuity { get; set; }

        public string NumRxQueues { get; set; }
        public string RxBytes { get; set; }
        public string RxPackets { get; set; }
        public string RxErrors { get; set; }
        public string RxDropped { get; set; }
        public string RxOverrun { get; set; }
        public string RxMcast { get; set; }

        public string NumTxQueues { get; set; }
        public string TxBytes { get; set; }
        public string TxPackets { get; set; }
        public string TxErrors { get; set; }
        public string TxDropped { get; set; }
        public string TxOverrun { get; set; }
        public string TxMcast { get; set; }

        public string StpPriority { get; set; }
        public string StpCost { get; set; }
        public string StpHairpin { get; set; }
        public string StpGuard { get; set; }
        public string StpRootBlock { get; set; }
        public string StpFastLeave { get; set; }
        public string StpLearning { get; set; }
        public string StpFlood { get; set; }
        public string StpMcastFastLeave { get; set; }
        public string StpForwardDelay { get; set; }
        public string StpHelloTime { get; set; }
        public string StpMaxAge { get; set; }

        public List<InetInfo> Inet4 { get; set; } = new List<InetInfo>();
        public List<InetInfo> Inet6 { get; set; } = new List<InetInfo>();
    }

    public class InetInfo
    {
        public string Inet { get; set; }
        public string InetScope { get; set; }
        public string InetValidLifetime { get; set; }
        public string InetPreferredLifetime { get; set; }
    }
}