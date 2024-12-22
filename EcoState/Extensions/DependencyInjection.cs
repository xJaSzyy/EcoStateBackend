using System.Reflection;
using System.Text;
using EcoState.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace EcoState.Extensions;

public static class DependencyInjection
{
    public static void AddAuth(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.Configure<AuthSettings>(configuration.GetSection("AuthSettings"));
        
        var authSettings = configuration.GetSection(nameof(AuthSettings))
            .Get<AuthSettings>();
        
        serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.SecretKey))
            };
        });
    }

    public static void AddSwagger(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(options =>
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
}