using Microsoft.OpenApi.Models;

namespace AuthService.WebApi.ApplicationServiceExtensions
{
    public static class SwaggerInstaller
    {
        public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthService API", Version = "v1" });
                // JWT auth in Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter: Bearer {your JWT}"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                { new OpenApiSecurityScheme{ Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "Bearer"}}, new List<string>() }
            });
            });
            return services;
        }
    }
}
