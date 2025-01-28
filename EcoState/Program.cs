using EcoState.Context;
using EcoState.Extensions;
using EcoState.Helpers;
using EcoState.Interfaces;
using EcoState.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(cfg=>cfg.AddProfile(new EntityMapper()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() 
            .AllowAnyHeader()
            .AllowAnyMethod(); 
    });
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

/*builder.Services.AddDbContext<DbContext>(opt =>
    opt.UseNpgsql(connectionString));*/

builder.Services.AddTransient<IEmissionService, EmissionService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IDbContext, ApplicationDbContext>();

builder.Services.Configure<WeatherSettings>(builder.Configuration.GetSection("WeatherSettings"));
builder.Services.AddSwagger();
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddHttpClient();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ТОЛЬКО ДЛЯ РАЗРАБОТКИ
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();