/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 3/17/2025, 10:35:10 AM
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Grains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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

        [HttpPost]
        public async Task<IActionResult> ActivePlayerGrain(long roleId)
        {
            var factory = HttpContext.RequestServices.GetRequiredService<IGrainFactory>();
            var pingSuc = await factory.GetGrain<IPlayerGrain>(roleId).PingAsync();
            return Ok($"Active PlayerGrain {roleId} Ping:{pingSuc}");
        }
    }
}