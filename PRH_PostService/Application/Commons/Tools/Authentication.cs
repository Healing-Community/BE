using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace Application.Commons.Tools;

public static class Authentication
{
    public static string? GetUserIdFromHttpContext(HttpContext httpContext)
    {
        // Check if the Authorization header exists
        if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            return null;

        string? authorizationHeader = httpContext.Request.Headers["Authorization"];

        // Validate the Authorization header format
        if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            return null;

        var jwtToken = authorizationHeader["Bearer ".Length..];
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            // Read the JWT token
            var token = tokenHandler.ReadJwtToken(jwtToken);
            var idClaim = token.Claims.FirstOrDefault(claim =>
                claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            // Return the user ID if found, otherwise return null
            return idClaim?.Value;
        }
        catch (Exception)
        {
            // Return null if there's any issue parsing the token or getting the claim
            return null;
        }
    }
}
