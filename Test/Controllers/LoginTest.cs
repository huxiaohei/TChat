using Abstractions.Message;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Network.Message;
using Network.Protos;

namespace Test.Controllers
{
    [ApiController]
    [Route("api/test/[controller]")]
    public class LoginTest : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login(long roleId, long serverId, string token)
        {
            var handler = HttpContext.RequestServices.GetRequiredService<IMessageHandler>();
            var resp = await handler.HandleMessageAsync(roleId, new CSMessage(roleId, 0, 0, new CSLoginReq()
            {
                RoleId = roleId,
                ServerId = serverId,
                Token = token
            }));
            if (resp != null)
            {
                return Ok($"{resp.Message}");
            }
            return Ok("ok");
        }
    }
}