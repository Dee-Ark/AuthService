using AuthService.Application.JwtTokenSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthService.WebApi.AuthServiceExtension
{
    public static class AuthenticationInstaller
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration cfg)
        {
            services.Configure<JwtSettings>(cfg.GetSection("JwtSettings"));
            var jwt = cfg.GetSection("JwtSettings").Get<JwtSettings>()!;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }
    }
}
