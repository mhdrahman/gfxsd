using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GFXSD.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpPost]
        [Authorize]
        public IActionResult Authenticate()
        {
            var token = Request.Headers["Authorization"].ToString();
            HttpContext.Response.Cookies.Append("token", token, new CookieOptions { Expires = DateTime.Now.AddSeconds(10) });
            return Ok();
        }
    }
}
