using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GFXSD.Authentication
{
    public class BasicAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "BasicAuthentication";
        public const string AuthenticationType = "Basic";

        public BasicAuthentication(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader == null || !authHeader.StartsWith(AuthenticationType, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var token = authHeader.Substring(6).Trim();
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(':');
            var username = credentials[0];
            var password = credentials[1];

            if (username != "username" || password != "password")
            {
                return Task.FromResult(AuthenticateResult.Fail("Incorrect username or password."));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, "User") };
            var claimsIdentity = new ClaimsIdentity(claims, AuthenticationType);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
        }
    }
}
