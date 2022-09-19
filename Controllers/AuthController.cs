using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace MicrosoftIdentityWeb.Controllers
{
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("sign-in")]
        public IActionResult SignIn([FromQuery] string redirectUri = "/swagger/index.html")
        {
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet("sign-out")]
        public IActionResult SignOut([FromQuery] string redirectUri = "/swagger/index.html")
        {
            // SignOut application cookie only - keeps AAD session alive

            return SignOut(new AuthenticationProperties { RedirectUri = redirectUri },
                CookieAuthenticationDefaults.AuthenticationScheme);

            //SignOut application cookie and AAD - asks user to signout from AAD account

            //return SignOut(new AuthenticationProperties { RedirectUri = redirectUri },
            //    CookieAuthenticationDefaults.AuthenticationScheme,
            //    OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
