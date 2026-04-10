using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartNetworkAnalyzer.API.Models;
using SmartNetworkAnalyzer.API.Services;

namespace SmartNetworkAnalyzer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly IPingService _pingService;
        public PingController(IPingService pingService)
        {
            _pingService = pingService;
        }

        [HttpGet]
        public async Task<ActionResult<PingResult>> GetPing()
        {
            var result = await _pingService.PingHostAsync("google.com");
            return Ok(result);
        }
    }
}
