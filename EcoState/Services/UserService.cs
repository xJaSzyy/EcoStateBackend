using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcoState.Context;
using EcoState.Domain;
using EcoState.Helpers;
using EcoState.Interfaces;
using EcoState.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EcoState.Services;

public class UserService : IUserService
{
    private readonly IDbContext _dbContext;
    private readonly IOptions<AuthSettings> _options;

    public UserService(IDbContext dbContext, IOptions<AuthSettings> options)
    {
        _dbContext = dbContext;
        _options = options;
    }

    public string Login(LoginModel model)
    {
        var user = _dbContext.Users.FirstOrDefault(x => x.Name == model.Name);

        var result = new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, model.Password);

        if (result == PasswordVerificationResult.Success)
        {
            var claims = new List<Claim>()
            {
                new Claim("Role", user.Role),
                new Claim("Name", user.Name),
                new Claim("Id", user.Id.ToString())
            };

            var jwtToken = new JwtSecurityToken(
                expires: DateTime.UtcNow.Add(_options.Value.Expires),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey)),
                    SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        return string.Empty;
    }

    public User Register(RegisterModel model)
    {
        var user = new User()
        {
            Role = "User",
            Name = model.Name,
            Email = model.Email 
        };

        var passwordHash = new PasswordHasher<User>().HashPassword(user, model.Password);
        user.PasswordHash = passwordHash;

        return user;
    }
}