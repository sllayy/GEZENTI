using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using GeziRotasi.API.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GeziRotasi.API.Configurations;

namespace GeziRotasi.API.Services
{
    public interface ITokenService
    {
        (string token, DateTimeOffset expiresAtUtc) Create(AppUser user);
    }

    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwt;

        public TokenService(IOptions<JwtSettings> jwtOptions)
        {
            _jwt = jwtOptions.Value;
        }

        public (string token, DateTimeOffset expiresAtUtc) Create(AppUser user)
        {
            if (string.IsNullOrWhiteSpace(_jwt.Key) || _jwt.Key.Length < 16)
                throw new InvalidOperationException("Jwt:Key is missing or too short (>=16 chars).");
            if (string.IsNullOrWhiteSpace(_jwt.Issuer)) throw new InvalidOperationException("Jwt:Issuer missing.");
            if (string.IsNullOrWhiteSpace(_jwt.Audience)) throw new InvalidOperationException("Jwt:Audience missing.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var exp = DateTimeOffset.UtcNow.AddMinutes(_jwt.ExpiresMinutes > 0 ? _jwt.ExpiresMinutes : 1);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier,   user.Id.ToString()),
                new Claim(ClaimTypes.Email,            user.Email ?? string.Empty),
                new Claim("given_name",                user.FirstName ?? string.Empty),
                new Claim("family_name",               user.LastName  ?? string.Empty)
            };

            var jwt = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: exp.UtcDateTime,
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(jwt);
            return (tokenStr, exp);
        }
    }
}
