using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XuongMay.Repositories.Entity;

namespace XuongMayBE.API.Config
{
    /// <summary>
    /// Helper class to generate JWT tokens.
    /// </summary>
    public static class TokenHelper
    {
        /// <summary>
        /// Generates a JWT token for a given user.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <param name="role">The user's role.</param>
        /// <param name="key">The secret key used to sign the token.</param>
        /// <param name="issuer">The issuer of the token.</param>
        /// <param name="audience">The audience of the token.</param>
        /// <returns>The generated JWT token.</returns>
        public static string GenerateJwtToken(ApplicationUser user, string role, List<string> permissions, string key, string issuer, string audience)
        {
            // Define the claims for the JWT token
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, role)
        };

            // Add permissions as claims
            claims.AddRange(permissions.Select(permission => new Claim("Permission", permission)));

            // Generate the security key from the secret key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // Token expires in 30 minutes
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
