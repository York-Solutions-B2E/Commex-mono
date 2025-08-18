using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Commex_Unit_Tests.IntegrationTests.Helpers
{
    public static class JwtTokenHelper
    {
        public static string GenerateTestToken(
            string userId = "test-user-id",
            string userName = "Test User",
            string role = "User",
            int expirationMinutes = 60)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsATestSecretKeyThatIsAtLeast256Bits!!!!"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, role),
                new Claim("sub", userId),
                new Claim("email", $"{userName.Replace(" ", ".").ToLower()}@example.com"),
                new Claim("jti", Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "test-issuer",
                audience: "test-audience",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateExpiredToken()
        {
            return GenerateTestToken(expirationMinutes: -60);
        }

        public static string GenerateAdminToken()
        {
            return GenerateTestToken(role: "Admin");
        }
    }
}