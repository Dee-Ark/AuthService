using AuthService.Application.Abstractions;
using AuthService.Application.Services;
using AuthService.Application.Validator;
using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.Security;
using AuthService.WebApi.Filters;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.WebApi.ApplicationServiceExtensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDbContext<AuthDbContext>(o =>
                o.UseNpgsql(cfg.GetConnectionString("DefaultConnection")));

            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();


            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<IAuthService, AuthServices>();

            return services;
        }
    }
}
