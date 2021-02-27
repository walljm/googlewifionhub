using JMW.Google.OnHub.API.Data;
using JMW.Google.OnHub.API.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

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

        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(IEnumerable<Interface>))]
        [HttpGet("interface")]
        public IActionResult GetInterface()
        {
            return Ok(this.context.Interface);
        }

        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(Interface))]
        [HttpGet("interface/{ifIndex}")]
        public IActionResult GetInterfaceByIndex(string ifIndex)
        {
            return Ok(this.context.Interface.Find(ifIndex));
        }

        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(IEnumerable<IpInfo>))]
        [HttpGet("ip")]
        public IActionResult GetInterfaceIps()
        {
            return Ok(this.context.InterfaceInets);
        }

        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(IEnumerable<IpInfo>))]
        [HttpGet("ip/{ifIndex}")]
        public IActionResult GetInterfaceIpsByIfIndex(string ifIndex)
        {
            return Ok(this.context.InterfaceInets.Where(o => o.IfIndex == ifIndex));
        }

        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(IEnumerable<Arp>))]
        [HttpGet("arp")]
        public IActionResult GetArp()
        {
            return Ok(this.context.Arp);
        }

        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(IEnumerable<Arp>))]
        [HttpGet("arp/{subnet}")]
        public IActionResult GetArp([FromRoute] string subnet)
        {
            if (!IPNetwork.TryParse(HttpUtility.UrlDecode(subnet), out var net))
            {
                return BadRequest("Subnet must be a valid Subnet in CIDR notation.");
            }

            return Ok(this.context.Arp.ToList().Where(o => net.Contains(IPAddress.Parse(o.IpAddress))));
        }

        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(IEnumerable<ArpHistory>))]
        [HttpGet("arp/history")]
        public IActionResult GetArpHistory()
        {
            return Ok(this.context.ArpHistory);
        }

        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(IEnumerable<ArpHistory>))]
        [HttpGet("arp/history/{subnet}")]
        public IActionResult GetArpHistoryBySubnet(string subnet)
        {
            if (!IPNetwork.TryParse(HttpUtility.UrlDecode(subnet), out var net))
            {
                return BadRequest("Subnet must be a valid Subnet in CIDR notation.");
            }

            return Ok(this.context.ArpHistory.ToList().Where(o => net.Contains(IPAddress.Parse(o.IpAddress))));
        }

        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(IEnumerable<Mac>))]
        [HttpGet("mac")]
        public IActionResult GetAllMacs()
        {
            return Ok(this.context.Mac);
        }

        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(Mac))]
        [HttpGet("mac/{hwAddress}")]
        public IActionResult GetMacByMac(string hwAddress)
        {
            return Ok(this.context.Mac.Find(hwAddress));
        }

        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(IEnumerable<Mac>))]
        [HttpGet("mac/interface/{ifIndex}")]
        public IActionResult GetMacByIfIndex(string ifIndex)
        {
            return Ok(this.context.Mac.Where(o => o.IfIndex == ifIndex));
        }
    }
}