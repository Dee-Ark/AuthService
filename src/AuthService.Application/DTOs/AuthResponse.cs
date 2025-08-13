using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs
{
    public record AuthResponse(string Token, DateTime ExpiresAt);

    //public class ApiResponse<T>
    //{
    //    public bool Success { get; set; }
    //    public string Message { get; set; }
    //    public T? Data { get; set; }
    //    public List<string>? Errors { get; set; }
    //    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    //    public ApiResponse() { }

    //    public ApiResponse(bool success, string message, T? data = default, List<string>? errors = null)
    //    {
    //        Success = success;
    //        Message = message;
    //        Data = data;
    //        Errors = errors;
    //    }

    //    public static ApiResponse<T> SuccessResponse(T data, string message = "Request successful")
    //    {
    //        return new ApiResponse<T>(true, message, data);
    //    }

    //    public static ApiResponse<T> FailResponse(string message, List<string>? errors = null)
    //    {
    //        return new ApiResponse<T>(false, message, default, errors);
    //    }
    //}


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