using System;
using System.ComponentModel.DataAnnotations;

namespace JMW.Google.OnHub.API.Model
{
    public class Arp
    {
        [Required]
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "The target must be a valid IPv4 Address")]
        public string IpAddress { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "HwAddress is MAC and must be 12 characters")]
        public string HwAddress { get; set; }

        public string HwType { get; set; }

        [Required]
        public string Interface { get; set; }

        public string Flags { get; set; }
        public string Mask { get; set; }

        [Required]
        public DateTimeOffset SeenFrom { get; set; }

        [Required]
        public DateTimeOffset SeenTo { get; set; }

        public static Arp Convert(OnHub.Model.Arp arp, DateTimeOffset from, DateTimeOffset to)
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

    public class ArpHistory
    {
        [Required]
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "The target must be a valid IPv4 Address")]
        public string IpAddress { get; set; }

        [Required]
        public string HwAddress { get; set; }

        public string HwType { get; set; }

        [Required]
        public string Interface { get; set; }

        public string Flags { get; set; }
        public string Mask { get; set; }

        [Required]
        public DateTimeOffset SeenFrom { get; set; }

        [Required]
        public DateTimeOffset SeenTo { get; set; }

        public static ArpHistory Convert(Arp arp)
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