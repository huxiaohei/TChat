using Microsoft.AspNetCore.Mvc;


namespace Network.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KeepAlive : ControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok($"Pong");
        }
    }
}