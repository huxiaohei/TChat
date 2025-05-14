/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 3/19/2025, 10:09:53 AM
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Grains;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Network.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Hotfix : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> ReloadModuleAsync(long roleId, string hotfixAssemblyPath)
        {
            var factory = HttpContext.RequestServices.GetRequiredService<IGrainFactory>();
            bool suc = await factory.GetGrain<IPlayerGrain>(roleId).HotfixModuleAsync(hotfixAssemblyPath);
            return suc ? Ok("Success") : BadRequest("Failed");
        }
    }
}