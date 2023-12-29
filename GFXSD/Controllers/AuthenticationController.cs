using GFXSD.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GFXSD.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpPost]
        [Authorize]
        public IActionResult Authenticate()
            => Ok(Request.Headers[BasicAuthentication.Authorization].ToString());
    }
}
