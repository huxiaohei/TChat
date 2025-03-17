/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 3/17/2025, 10:35:10 AM
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

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