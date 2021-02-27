using System.Collections.Generic;

namespace JMW.Google.OnHub.Model
{
    public class NetworkServiceState
    {
        public IPv6 IPv6 { get; set; }
        public RouterConfig RouterConfig { get; set; } = new RouterConfig();
        public WanStatus WAN { get; set; } = new WanStatus();
        public LanStatus LAN { get; set; } = new LanStatus();
        public List<StationState> Stations { get; set; } = new List<StationState>();
    }
}