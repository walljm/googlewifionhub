using System.ComponentModel.DataAnnotations;

namespace JMW.Google.OnHub.Model
{
    public class Mac
    {
        [Required] public string IfIndex { get; set; }
        [Required] public string HwAddress { get; set; }
        public string IsLocal { get; set; }
        public string Age { get; set; }
    }
}