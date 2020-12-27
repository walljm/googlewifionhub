using JMW.Google.OnHub.API.Data;
using JMW.Google.OnHub.API.Model;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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

        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(Arp))]
        [HttpGet("arp/history")]
        public IActionResult GetArpHistory()
        {
            return Ok(this.context.Arp.ToList());
        }
    }
}