using System.ComponentModel.DataAnnotations;

namespace JMW.Google.OnHub.API.Options
{
    public class CacheOptions
    {
        [Range(1, int.MaxValue)]
        public int CurrentTimeoutInDays { get; set; } = 30;

        [Range(1, int.MaxValue)]
        public int HistoryTimeoutInDays { get; set; } = 360;
    }
}