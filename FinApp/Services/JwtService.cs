using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinApp.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FinApp.Services
{
    public interface IJwtService
    {
        string GeneratePageAccessToken(AppUserModel user);
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

        public string GeneratePageAccessToken(AppUserModel user)
        {
            var expiration = DateTime.UtcNow.AddHours(72);

            // Create claims as a dictionary
            var claims = new Dictionary<string, object>
            {
                { "Id", user.Id },
                { "Role", user.Role },
                { "Email", user.Email },
                { "FullName", user.FullName },
                { "LicenseType", user.License.Type },
                { "LicenseExpiry", user.License.Expiry },
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

        public string GetTokenPayload(HttpRequest request, string claimName)
        {
            try
            {
                // Extract the token from the 'jwttoken' cookie
                if (!request.Cookies.TryGetValue("jwttoken", out var token) || string.IsNullOrEmpty(token))
                {
                    throw new ArgumentException("JWT token is missing in the cookie.");
                }

                // Decode the token
                var tokenHandler = new JwtSecurityTokenHandler();
                if (tokenHandler.CanReadToken(token))
                {
                    var jwtToken = tokenHandler.ReadJwtToken(token);

                    // Find the claim with the specified name
                    var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimName);
                    if (claim != null)
                    {
                        return claim.Value;
                    }

                    throw new ArgumentException($"Claim '{claimName}' not found in the token.");
                }

                throw new ArgumentException("Invalid token format.");
            }
            catch (Exception ex)
            {
                // Log exception if needed
                Console.WriteLine($"Error extracting token claim: {ex.Message}");
                return null; // Return null or throw based on your application's needs
            }
        }
    }
}
