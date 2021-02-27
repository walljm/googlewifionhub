using System;
using System.ComponentModel.DataAnnotations;

namespace JMW.Google.OnHub.API.Model
{
    public class Arp : OnHub.Model.Arp
    {
        [Required]
        public DateTimeOffset FirstSeen { get; set; }

        [Required]
        public DateTimeOffset LastSeen { get; set; }

        public static Arp ToArp(OnHub.Model.Arp arp, DateTimeOffset first, DateTimeOffset last)
        {
            return new Arp
            {
                IpAddress = arp.IpAddress,
                HwType = arp.HwType,
                Flags = arp.Flags,
                HwAddress = arp.HwAddress,
                Mask = arp.Mask,
                Interface = arp.Interface,
                FirstSeen = first,
                LastSeen = last
            };
        }
    }

    public class ArpHistory : OnHub.Model.Arp
    {
        [Required]
        public DateTimeOffset SeenFrom { get; set; }

        [Required]
        public DateTimeOffset SeenTo { get; set; }

        public static ArpHistory ToArpHistory(Arp arp)
        {
            return new ArpHistory
            {
                IpAddress = arp.IpAddress,
                HwType = arp.HwType,
                Flags = arp.Flags,
                HwAddress = arp.HwAddress,
                Mask = arp.Mask,
                Interface = arp.Interface,
                SeenFrom = arp.FirstSeen,
                SeenTo = arp.LastSeen
            };
        }
    }
}