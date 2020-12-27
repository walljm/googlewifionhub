namespace JMW.Google.OnHub.Models
{
    public class SpeedTestResults
    {
        public string DateTimeSecondsSinceEpoch { get; set; }
        public string DownloadSpeedBytesPerSecond { get; set; }
        public string UploadSpeedBytesPerSecond { get; set; }
        public string TotalBytesDownloaded { get; set; }
        public string TotalBytesUploaded { get; set; }
    }
}