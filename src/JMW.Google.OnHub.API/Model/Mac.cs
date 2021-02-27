using System;
using System.ComponentModel.DataAnnotations;

namespace JMW.Google.OnHub.API.Model
{
    public class Mac : OnHub.Model.Mac
    {
        [Required]
        public DateTimeOffset SeenFrom { get; set; }

        [Required]
        public DateTimeOffset SeenTo { get; set; }

        public static Mac ToMac(OnHub.Model.Mac mac, DateTimeOffset from, DateTimeOffset to)
        {
            return new Mac
            {
                HwAddress = mac.HwAddress,
                Age = mac.Age,
                IfIndex = mac.IfIndex,
                IsLocal = mac.IsLocal,
                SeenFrom = from,
                SeenTo = to
            };
        }
    }
}