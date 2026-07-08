using InterviewPrep.API.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InterviewPrep.API.Helpers
{
    public class JwtHelper
    {
        public const string SecretKey = "YourSuperSecretKeyThatIsAtLeast32CharactersLongForSecurity2024!";
        public const string Issuer = "InterviewPrepAPI";
        public const string Audience = "InterviewPrepApp";
        public const int ExpiryHours = 24;

        public JwtHelper(IConfiguration config) { }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.UserId.ToString()),
                new Claim("name", user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(ExpiryHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Guid GetUserIdFromToken(ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst("userId")
                ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)
                ?? throw new UnauthorizedAccessException("Invalid token");

            return Guid.Parse(userIdClaim.Value);
        }
    }
}
