using System.Collections.Generic;

namespace JMW.Google.OnHub.Model
{
    public class StationState
    {
        public string StationId { get; set; }
        public string MdnsName { get; set; }
        public string Connected { get; set; }
        public string IpAddresses { get; set; }
        public string Wireless { get; set; }
        public string DhcpHostname { get; set; }
        public string WirelessInterface { get; set; }
        public List<string> TaxonomyIds { get; set; } = new List<string>();
        public DnsSdFeature DnsSdFeatures { get; set; } = new DnsSdFeature();
        public string LastSeenSecondsSinceEpoch { get; set; }
        public string Oui { get; set; }
        public string Guest { get; set; }
        public string OsVersion { get; set; }
        public string OsBuild { get; set; }
        public string DeviceModel { get; set; }
    }
}