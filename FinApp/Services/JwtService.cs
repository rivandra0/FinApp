using System.Security.Claims;
using System.Text;
using FinApp.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FinApp.Services
{
    public interface IJwtService
    {
        string GeneratePageAccessToken(AppUser user);
    }

    public class JwtSetting
    {
        //maybe we can add more here later
        public string TokenSecret { get; set; } = string.Empty;
    }

    public class JwtService : IJwtService
    {
        JwtSetting _jwtSetting { get; set; }

        public JwtService(JwtSetting jwtSetting)
        {
            _jwtSetting = jwtSetting;
        }

        public string GeneratePageAccessToken(AppUser user)
        {
            var expiration = DateTime.UtcNow.AddHours(72);

            // Create claims as a dictionary
            var claims = new Dictionary<string, object>
            {
                { "Id", user.Id },
                { "Role", user.Role },
                { "Email", user.Email },
                //{ "LicenseType", user.License.Type },
                //{ "LicenseExpiry", user.License.Expiry },
                { "exp", new DateTimeOffset(expiration).ToUnixTimeSeconds() }, // Expiration in Unix time
            };

            // Create a security key and signing credentials
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.TokenSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the token descriptor
            var tokenDescriptor = new JsonWebTokenHandler().CreateToken(
                new SecurityTokenDescriptor
                {
                    Claims = claims,
                    Expires = expiration,
                    SigningCredentials = credentials,
                }
            );

            return tokenDescriptor;
        }
    }
}
