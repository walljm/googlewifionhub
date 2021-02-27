using System;
using System.ComponentModel.DataAnnotations;

namespace JMW.Google.OnHub.API.Model
{
    public class Arp : OnHub.Model.Arp
    {
        [Required]
        public DateTimeOffset SeenFrom { get; set; }

        [Required]
        public DateTimeOffset SeenTo { get; set; }

        public static Arp ToArp(OnHub.Model.Arp arp, DateTimeOffset from, DateTimeOffset to)
        {
            return new Arp
            {
                IpAddress = arp.IpAddress,
                HwType = arp.HwType,
                Flags = arp.Flags,
                HwAddress = arp.HwAddress,
                Mask = arp.Mask,
                Interface = arp.Interface,
                SeenFrom = from,
                SeenTo = to
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
                SeenFrom = arp.SeenFrom,
                SeenTo = arp.SeenTo
            };
        }
    }
}