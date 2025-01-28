using EcoState.Domain;
using EcoState.Helpers;
using EcoState.Interfaces;
using EcoState.Services;
using EcoState.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;

namespace EcoState.Test;

public class UserServiceTest
{
    [Fact]
    public void Login_WithLoginModel_ShouldReturnNotEmptyToken()
    {
        //Arrange
        var dbContextMock = new Mock<IDbContext>();
        dbContextMock.Setup<DbSet<User>>(x => x.Users).ReturnsDbSet(_testUsers);

        var authOptions = Options.Create(_testAuthSettings);
        
        var service = new UserService(dbContextMock.Object, authOptions);

        //Act
        var token = service.Login(_loginModel);
         
        //Assert
        Assert.NotEmpty(token);
    }

    [Fact]
    public void Register_WithRegisterModel_ShouldReturnUser()
    {
        //Arrange
        var dbContextMock = new Mock<IDbContext>();

        var authOptions = Options.Create(_testAuthSettings);
        
        var service = new UserService(dbContextMock.Object, authOptions);
        
        //Act
        var user = service.Register(_registerModel);
        
        //Assert
        Assert.NotEmpty(user.PasswordHash);
        Assert.NotEmpty(user.Email);
    }

    #region TestData

    private readonly List<User> _testUsers = new List<User>()
    {
        new User()
        {
            Role = Role.User,
            Name = "Stepan",
            PasswordHash = "AQAAAAIAAYagAAAAEB6tRGXiPSzX9P/ufgjBMNqZp5OiniQssvADa5xeTOxv3rXuDBDA8RVkySJOU4uHPw=="
        },
    };
    
    private readonly AuthSettings _testAuthSettings = new AuthSettings()
    {
        Expires = new TimeSpan(0, 24, 0),
        SecretKey = "mega_super_duper_secret_key_for_unit_test"
    };

    private readonly LoginModel _loginModel = new LoginModel()
    {
        Name = "Stepan",
        Password = "StepanPassword"
    };
    
    private readonly RegisterModel _registerModel = new RegisterModel()
    {
        Name = "Stepan",
        Password = "StepanPassword",
        Email = "stepan@gmail.com"
    };

    #endregion
}