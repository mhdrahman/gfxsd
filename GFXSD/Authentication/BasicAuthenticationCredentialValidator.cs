using System;
using System.Text;

namespace GFXSD.Authentication
{
    public class BasicAuthenticationCredentialValidator
    {
        public bool Validate(string authHeader)
        {
            var token = authHeader.Substring(6).Trim();
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(':');
            var username = credentials[0];
            var password = credentials[1];

            if (username != Configuration.Username || password != Configuration.Password)
            {
                return false;
            }

            return true;
        }
    }
}
