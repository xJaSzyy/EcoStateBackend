using System.Reflection;
using System.Text;
using EcoState.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace EcoState.Extensions;

/// <summary>
/// DependencyInjection
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// DI для аутентификации
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddAuth(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AuthSettings>(options =>
        {
            options.SecretKey = Environment.GetEnvironmentVariable("AuthSettings__SecretKey")!;
            options.Expires = TimeSpan.Parse(Environment.GetEnvironmentVariable("AuthSettings__Expires")!);
        });
        
        var authSettings = configuration.GetSection(nameof(AuthSettings))
            .Get<AuthSettings>();
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings!.SecretKey))
            };
        });
    }

    /// <summary>
    /// DI для сваггера
    /// </summary>
    /// <param name="services"></param>
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //options.IncludeXmlComments(xmlPath);
            options.SchemaFilter<EnumTypesSchemaFilter>(xmlPath);
    
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2", Name = "Bearer", In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });
    }

    /// <summary>
    /// DI для погоды
    /// </summary>
    /// <param name="services"></param>
    public static void AddWeather(this IServiceCollection services)
    {
        services.Configure<WeatherSettings>(options =>
        {
            options.ApiKey = Environment.GetEnvironmentVariable("WeatherSettings__ApiKey")!;
            options.BaseUrl = Environment.GetEnvironmentVariable("WeatherSettings__BaseUrl")!;
            options.Units = Environment.GetEnvironmentVariable("WeatherSettings__Units")!;
        });
    }
}