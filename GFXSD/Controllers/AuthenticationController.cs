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
            // TODO still neeed to check that the password and username are correct here before storing it. Otherwise if there was already a cookie and that's how he got into this
            // method but we provided bad credentials - the bad creds will be stored and then we get an issue of the bad cookie always being picked up
            var token = Request.Headers["Authorization"].ToString();
            HttpContext.Response.Cookies.Append("token", token, new CookieOptions { Expires = DateTime.Now.AddMinutes(10) });
            return Ok();
        }
    }
}
