using AuthService.Application.Abstractions;
using AuthService.Application.DTOs;
using AuthService.Application.JwtTokenSettings;
using AuthService.Domain.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Application.Services
{
    public class AuthServices(IUserRepository users, IPasswordHasher hasher, IOptions<JwtSettings> jwtOpt) : IAuthService
    {
        private readonly IUserRepository _users = users;
        private readonly IPasswordHasher _hasher = hasher;
        private readonly JwtSettings _jwt = jwtOpt.Value;
        //private readonly ILogger _logger = logger;

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            try
            {
                //_logger.Information("Fetching user by email {Email}", request.FullName);
                var existing = await _users.GetByEmailAsync(request.Email, ct);
                if (existing is not null) throw new InvalidOperationException("Email already registered.");

                var user = new Domain.Entities.User
                {
                    Email = request.Email,
                    FullName = request.FullName,
                    PasswordHash = _hasher.Hash(request.Password)
                };

                await _users.AddAsync(user, ct);
                await _users.SaveChangesAsync(ct);

                return GenerateToken(user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct = default)
        {
            try
            {
                var user = await _users.GetByEmailAsync(req.Email, ct)
                               ?? throw new UnauthorizedAccessException("Invalid credentials.");

                if (!_hasher.VerifyPassword(req.Password, user.PasswordHash))
                    throw new UnauthorizedAccessException("Invalid credentials.");

                return GenerateToken(user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private AuthResponse GenerateToken(Domain.Entities.User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

            var token = new JwtSecurityToken(
                _jwt.Issuer, 
                _jwt.Audience, 
                claims, expires: expires, 
                signingCredentials: creds
             );
            return new AuthResponse(new JwtSecurityTokenHandler().WriteToken(token), expires);
        }
    }
}
