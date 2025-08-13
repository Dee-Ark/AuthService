using AuthService.Application.Abstractions;
using AuthService.Application.DTOs;
using AuthService.Application.JwtTokenSettings;
using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Application.Services
{
    public class AuthServices(IUserRepository users, IPasswordHasher hasher, IOptions<JwtSettings> jwtOpt, ILogger<AuthServices> logger) : IAuthService
    {
        private readonly IUserRepository _users = users;
        private readonly IPasswordHasher _hasher = hasher;
        private readonly JwtSettings _jwt = jwtOpt.Value;
        private readonly ILogger<AuthServices> _logger = logger;

        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

            try
            {
                var existing = await _users.GetByEmailAsync(request.Email, ct);
                if (existing is not null)
                {
                    _logger.LogWarning("User with Email {Email} already registered", request.Email);
                    return ApiResponse<RegisterResponse>.FailResponse($"User with Email {request.Email} already exists.");
                }

                var user = new User
                {
                    Email = request.Email,
                    FullName = request.FullName,
                    PasswordHash = _hasher.Hash(request.Password)
                };

                await _users.AddAsync(user, ct);
                await _users.SaveChangesAsync(ct);

                _logger.LogInformation("User registered successfully: {UserId}", user.Id);

                var response = new RegisterResponse(user.Id, user.Email, user.FullName);

                return ApiResponse<RegisterResponse>.SuccessResponse(response, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
                return ApiResponse<RegisterResponse>.FailResponse("An unexpected error occurred.");
            }
        }


        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest req, CancellationToken ct = default)
        {
            _logger.LogInformation("Login attempt for email: {Email}", req.Email);

            try
            {
                var user = await _users.GetByEmailAsync(req.Email, ct);

                if (user is null)
                {
                    _logger.LogWarning("Login failed: User not found for email {Email}", req.Email);
                    return ApiResponse<AuthResponse>.FailResponse("Invalid credentials.");
                }

                if (!_hasher.VerifyPassword(req.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed: Incorrect password for email {Email}", req.Email);
                    return ApiResponse<AuthResponse>.FailResponse("Invalid credentials.");
                }

                var token = GenerateToken(user);

                _logger.LogInformation("User logged in successfully: {UserId}", user.Id);

                return ApiResponse<AuthResponse>.SuccessResponse(token, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", req.Email);
                return ApiResponse<AuthResponse>.FailResponse("An unexpected error occurred.");
            }
        }

        private AuthResponse GenerateToken(User user)
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
