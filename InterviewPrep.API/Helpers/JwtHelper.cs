using InterviewPrep.API.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InterviewPrep.API.Helpers
{
    public class JwtHelper
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryHours;

        public JwtHelper(IConfiguration config)
        {
            _secretKey = "YourSuperSecretKeyThatIsAtLeast32CharactersLongForSecurity2024!";
            _issuer = "InterviewPrepAPI";
            _audience = "InterviewPrepApp";
            _expiryHours = 24;
        }

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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_expiryHours),
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
