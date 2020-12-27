using System.Collections.Generic;

namespace JMW.Google.OnHub.Models
{
    public class InfraState
    {
        public string BackgroundDataCollection { get; set; }
        public string AnonymousMetricsCollection { get; set; }
        public string LightingIntensity { get; set; }
        public string LightingAutoIntensity { get; set; }
        public string DeveloperMode { get; set; }
        public SpeedTestResults WanSpeedTestResults { get; set; }
        public string AutoUpdateChannel { get; set; }
        public string AutoUpdateChannelNewVersion { get; set; }
        public string AutoUpdateChannelStatus { get; set; }
        public string DeviceMode { get; set; }
        public string IspConfigurationType { get; set; }
        public string FirmwareVersion { get; set; }
        public string SetupState { get; set; }
        public List<string> ExperimentStateIds { get; set; }
    }
}