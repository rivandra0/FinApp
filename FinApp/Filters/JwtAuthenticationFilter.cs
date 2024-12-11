using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

public class JwtAuthenticationFilter : IActionFilter
{
    private readonly string _cookieName;
    private readonly string _tokenSecret;

    public JwtAuthenticationFilter(string cookieName, string tokenSecret)
    {
        _cookieName = cookieName;
        _tokenSecret = tokenSecret;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint != null && endpoint.Metadata.Any(meta => meta is AllowAnonymousAttribute))
        {
            return; // Skip authorization if AllowAnonymous is present.
        }

        var request = context.HttpContext.Request;
        var cookies = request.Cookies;

        if (!cookies.TryGetValue(_cookieName, out var token))
        {
            context.Result = new RedirectToActionResult("Index", "Error", new { statusCode = 401, message = "Token not found" });
            return;
        }

        try
        {
            bool isValid = IsTokenValid(token, _tokenSecret);

            if (!isValid)
            {
                context.Result = new RedirectToActionResult("Index", "Error", new { statusCode = 401, message = "Invalid token" });
                return;
            }

            //context.HttpContext.User = principal;
        }
        catch (Exception ex)
        {
            context.Result = new RedirectToActionResult("Index", "Error", new { statusCode = 401, message = "unknown error" });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No implementation needed for post-action.
    }

    public bool IsTokenValid(string token, string tokensecret)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(tokensecret);

        try
        {
            // Set up token validation parameters
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidIssuer = "FinApp", // Ensure this matches the issuer in the token
                ValidateAudience = false, // Set to true if you include an audience
                ClockSkew =
                    TimeSpan.Zero // Immediate expiration check
                ,
            };

            // Validate the token
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            // Additional validation (optional): Ensure the token is a JWT
            if (validatedToken is JwtSecurityToken jwtToken)
            {
                // Check algorithm (optional, adds security)
                var alg = jwtToken.Header.Alg;
                if (alg != SecurityAlgorithms.HmacSha256)
                    throw new SecurityTokenInvalidSignatureException("Invalid token algorithm.");
            }

            // If no exception was thrown, the token is valid
            return true;
        }
        catch (Exception ex)
        {
            // Log or handle the exception for debugging purposes
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return false;
        }
    }
}
