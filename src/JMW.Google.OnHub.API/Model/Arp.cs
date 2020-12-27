using System;

namespace JMW.Google.OnHub.API.Model
{
    public class Arp : OnHub.Model.Arp
    {
        public DateTimeOffset SeenFrom { get; set; }
        public DateTimeOffset SeenTo { get; set; }
    }
}