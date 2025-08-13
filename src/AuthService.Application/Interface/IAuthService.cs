using AuthService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest req, CancellationToken ct = default);
    }
}
