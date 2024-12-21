using System.Reflection;
using System.Text;
using EcoState.Context;
using EcoState.Extensions;
using EcoState.Helpers;
using EcoState.Interfaces;
using EcoState.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(cfg=>cfg.AddProfile(new EntityMapper()));

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

builder.Services.AddSwagger(builder.Configuration);
builder.Services.AddAuth(builder.Configuration);

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();