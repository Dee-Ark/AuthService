using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs
{
    public record AuthResponse(string Token, DateTime ExpiresAt);
    public record RegisterResponse(Guid Id, string Email, string FullName);

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string? message = null) =>
            new ApiResponse<T> { Success = true, Message = message, Data = data };

        public static ApiResponse<T> FailResponse(string message, T? data = default) =>
            new ApiResponse<T> { Success = false, Message = message, Data = data };
    }

}