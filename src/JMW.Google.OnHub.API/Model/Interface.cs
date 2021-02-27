using System;
using System.ComponentModel.DataAnnotations;

namespace JMW.Google.OnHub.API.Model
{
    public class Interface
    {
        [Required] public string IfIndex { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Info { get; set; }
        [Required] public string State { get; set; }
        [Required] public string MAC { get; set; }
        [Required] public string BRD { get; set; }

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

        [Required]
        public DateTimeOffset SeenFrom { get; set; }

        [Required]
        public DateTimeOffset SeenTo { get; set; }

        public static Interface ToInterface(OnHub.Model.Interface ifc, DateTimeOffset from, DateTimeOffset to)
        {
            return new Interface
            {
                IfIndex = ifc.IfIndex,
                Name = ifc.Name,
                Info = ifc.Info,
                State = ifc.State,
                MAC = ifc.MAC,
                BRD = ifc.BRD,
                MTU = ifc.MTU,
                Qdisc = ifc.Qdisc,
                Group = ifc.Group,
                Qlen = ifc.Qlen,
                Link = ifc.Link,
                Promiscuity = ifc.Promiscuity,
                NumRxQueues = ifc.NumRxQueues,
                RxBytes = ifc.RxBytes,
                RxPackets = ifc.RxPackets,
                RxErrors = ifc.RxErrors,
                RxDropped = ifc.RxDropped,
                RxOverrun = ifc.RxOverrun,
                RxMcast = ifc.RxMcast,
                NumTxQueues = ifc.NumTxQueues,
                TxBytes = ifc.TxBytes,
                TxPackets = ifc.TxPackets,
                TxErrors = ifc.TxErrors,
                TxDropped = ifc.TxDropped,
                TxOverrun = ifc.TxOverrun,
                TxMcast = ifc.TxMcast,
                StpPriority = ifc.StpPriority,
                StpCost = ifc.StpCost,
                StpHairpin = ifc.StpHairpin,
                StpGuard = ifc.StpGuard,
                StpRootBlock = ifc.StpRootBlock,
                StpFastLeave = ifc.StpFastLeave,
                StpLearning = ifc.StpLearning,
                StpFlood = ifc.StpFlood,
                StpMcastFastLeave = ifc.StpMcastFastLeave,
                StpForwardDelay = ifc.StpForwardDelay,
                StpHelloTime = ifc.StpHelloTime,
                StpMaxAge = ifc.StpMaxAge,

                SeenFrom = from,
                SeenTo = to
            };
        }
    }

    public class IpInfo
    {
        [Required] public string IfIndex { get; set; }
        [Required] public string InetType { get; set; }
        [Required] public string Inet { get; set; }
        public string InetScope { get; set; }
        public string InetValidLifetime { get; set; }
        public string InetPreferredLifetime { get; set; }

        [Required] public DateTimeOffset SeenFrom { get; set; }

        [Required] public DateTimeOffset SeenTo { get; set; }

        public static IpInfo ToInetInfo(OnHub.Model.InetInfo info, DateTimeOffset from, DateTimeOffset to)
        {
            return new IpInfo
            {
                IfIndex = info.IfIndex,
                InetType = info.InetType,
                Inet = info.Inet,
                InetScope = info.InetScope,
                InetValidLifetime = info.InetValidLifetime,
                InetPreferredLifetime = info.InetPreferredLifetime,
                SeenFrom = from,
                SeenTo = to
            };
        }
    }
}