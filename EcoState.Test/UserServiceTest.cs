using AutoFixture;
using EcoState.Context;
using EcoState.Domain;
using EcoState.Helpers;
using EcoState.Interfaces;
using EcoState.Services;
using EcoState.ViewModels.User;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace EcoState.Test;

public class UserServiceTest
{
    private Mock<ApplicationDbContext> _dbContext = new();
    private Fixture _fixture = new();
    
    private List<User> _testData = new();
    
    private readonly AuthSettings _testAuthSettings = new AuthSettings()
    {
        Expires = new TimeSpan(0, 24, 0),
        SecretKey = "mega_super_duper_secret_key_for_unit_test"
    };
    
    [SetUp]
    public void Setup()
    {
        SetDataToContext(_testData.AsQueryable());
    }
    
    [TearDown]
    public void Cleanup()
    {
        _dbContext.Reset();
        
        _testData = new List<User>();
    }
    
    [Test]
    public void Login_WithLoginModel_ShouldReturnNotEmptyToken()
    {
        //Arrange
        var user = new User
        {
            Name = "Name",
            Email = "Email",
            Role = Role.User
        };
        
        var passwordHash = new PasswordHasher<User>().HashPassword(user, "Password");
        user.PasswordHash = passwordHash;
        
        _testData = new List<User>() { user };
        SetDataToContext(_testData.AsQueryable());

        var loginModel = new LoginModel()
        {
            Name = user.Name,
            Password = "Password"
        };
        
        var service = new UserService(_dbContext.Object, Options.Create(_testAuthSettings));
        
        //Act
        var token = service.Login(loginModel);
         
        //Assert
        _dbContext.Verify(x => x.Users, Times.Once);
        token.Should().NotBeEmpty();
    }
    
    [Test]
    public void Login_WithLoginModel_ShouldNotFoundUser()
    {
        //Arrange
           var loginModel = new LoginModel()
           {
               Name = "Name",
               Password = "Password"
           };
           
           var service = new UserService(_dbContext.Object, Options.Create(_testAuthSettings));
           
           //Act
           var token = service.Login(loginModel);
            
           //Assert
           _dbContext.Verify(x => x.Users, Times.Once);
           token.Should().Be("Пользователь не найден");
    }
    
    [Test]
    public void Login_WithLoginModel_ShouldPasswordVerificationResultNotEqualSuccess()
    {
        //Arrange
        var user = new User
        {
            Name = "Name",
            Email = "Email",
            Role = Role.User
        };
        
        var passwordHash = new PasswordHasher<User>().HashPassword(user, "Password");
        user.PasswordHash = passwordHash;
        
        _testData = new List<User>() { user };
        SetDataToContext(_testData.AsQueryable());

        var loginModel = new LoginModel()
        {
            Name = user.Name,
            Password = "Password123"
        };
        
        var service = new UserService(_dbContext.Object, Options.Create(_testAuthSettings));
        
        //Act
        var token = service.Login(loginModel);
         
        //Assert
        _dbContext.Verify(x => x.Users, Times.Once);
        token.Should().Be("Неверный пароль");
    }

    [Test]
    public void Register_WithRegisterModel_ShouldReturnUser()
    {
        //Arrange
        var registerModel = _fixture.Create<RegisterModel>();
        
        var service = new UserService(_dbContext.Object, Options.Create(_testAuthSettings));
        
        //Act
        var user = service.Register(registerModel);
        
        //Assert
        user.PasswordHash.Should().NotBeEmpty();
    }
    
    private void SetDataToContext(IQueryable<User> testData)
    {
        var mockSet = new Mock<DbSet<User>>();
        mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(testData.Provider);
        mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(testData.Expression);
        mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(testData.ElementType);
        mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator);
        
        _dbContext = new Mock<ApplicationDbContext>();
        _dbContext.Setup(c => c.Users).Returns(mockSet.Object);
    }
}