using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace Application.Commons.Tools;

public static class Authentication
{
    public static string GenerateOtp(int length = 6)
    {
        // Tạo một instance của Random để tạo số ngẫu nhiên
        Random random = new Random();

        // Tạo chuỗi OTP với các ký tự số (0-9)
        string otp = string.Empty;
        for (int i = 0; i < length; i++)
        {
            // Tạo số ngẫu nhiên từ 0 đến 9 và thêm vào chuỗi OTP
            otp += random.Next(0, 10).ToString();
        }

        return otp;
    }
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
