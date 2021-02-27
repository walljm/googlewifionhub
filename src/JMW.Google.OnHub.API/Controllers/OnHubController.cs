using JMW.Google.OnHub.API.Data;
using JMW.Google.OnHub.API.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JMW.Google.OnHub.API.Controllers
{
    [ApiController]
    [Route("v1/")]
    public class OnHubController : ControllerBase
    {
        private readonly ApplicationContext context;

        public OnHubController(
            ApplicationContext context
            )
        {
            this.context = context;
        }
        
        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(Interface))]
        [HttpGet("interface")]
        public IActionResult GetInterface()
        {
            return Ok(this.context.Interface);
        }

        
        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(IpInfo))]
        [HttpGet("ip")]
        public IActionResult GetInterfaceIps()
        {
            return Ok(this.context.InterfaceInets);
        }


        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(Arp))]
        [HttpGet("arp")]
        public IActionResult GetArp()
        {
            return Ok(this.context.Arp);
        }

        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(ArpHistory))]
        [HttpGet("arp/history")]
        public IActionResult GetArpHistory()
        {
            return Ok(this.context.ArpHistory);
        }
    }
}