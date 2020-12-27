using System.ComponentModel.DataAnnotations;

namespace JMW.Google.OnHub.API.Options
{
    public class CollectionOptions
    {
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$", 
            ErrorMessage ="The target must be a valid IPv4 Address")]
        public string Target { get; set; }

        public string Schedule { get; set; }
    }
}