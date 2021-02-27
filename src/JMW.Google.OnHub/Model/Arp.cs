using System.ComponentModel.DataAnnotations;

namespace JMW.Google.OnHub.Model
{
    public class Arp
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
    }
}