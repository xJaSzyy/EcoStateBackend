using AutoFixture;
using AutoMapper;
using EcoState.Context;
using EcoState.Controllers;
using EcoState.Domain;
using EcoState.Interfaces;
using EcoState.ViewModels.User;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EcoState.Test;

public class UserControllerTest
{
    private Mock<ApplicationDbContext> _dbContext = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IUserService> _service = new();
    private readonly Fixture _fixture = new();
    
    private List<User> _testData = new();
    
    [SetUp]
    public void Setup()
    {
        SetDataToContext(_testData.AsQueryable());
    }
    
    [TearDown]
    public void Cleanup()
    {
        _dbContext.Reset();
        _mapper.Reset();
        _service.Reset();

        _testData = new List<User>();
    }

    [Test]
    public async Task GetUser_WithUserGetModel_ShouldGetCorrectUser()
    {
        // Arrange
        var model = _fixture.Create<UserGetModel>();

        var user = _fixture.Build<User>()
            .With(x => x.Id, model.Id)
            .Create();

        var userViewModel = new UserViewModel()
        {
            Id = user.Id,
            Role = user.Role,
            Name = user.Name,
            PasswordHash = user.PasswordHash,
            Email = user.Email!,
        };
        
        _mapper.Setup(x => x.Map<UserViewModel>(user))
            .Returns(userViewModel);

        _testData = new List<User>() { user };
        SetDataToContext(_testData.AsQueryable());
        
        var controller = new UserController(_dbContext.Object, _mapper.Object, _service.Object);

        // Act
        var result = await controller.GetUser(model) as OkObjectResult;

        // Assert
        _dbContext.Verify(x => x.Users, Times.Once);
        _mapper.Verify(x => x.Map<UserViewModel>(user), Times.Once);
        result!.Value.Should().Be(userViewModel);
    }
    
    [Test]
    public async Task GetUser_WithUserGetModel_ShouldNotFindUser()
    {
        // Arrange
        var model = _fixture.Create<UserGetModel>();
        
        var controller = new UserController(_dbContext.Object, _mapper.Object, _service.Object);

        // Act
        var result = await controller.GetUser(model) as OkObjectResult;

        // Assert
        _dbContext.Verify(x => x.Users, Times.Once);
        result!.Value.Should().Be("Пользователь не найден");
    }
    
    [Test]
    public async Task GetAllUsers_ShouldGetCorrectUsers()
    {
        // Arrange
        _testData = _fixture.Create<List<User>>();

        var usersViewModel = _testData.Select(user => new UserViewModel()
            {
                Id = user.Id,
                Role = user.Role,
                Name = user.Name,
                PasswordHash = user.PasswordHash,
                Email = user.Email!,
            })
            .ToList();

        _mapper.Setup(x => x.Map<List<UserViewModel>>(_testData))
            .Returns(usersViewModel);

        SetDataToContext(_testData.AsQueryable());
        
        var controller = new UserController(_dbContext.Object, _mapper.Object, _service.Object);

        // Act
        var result = await controller.GetAllUsers() as OkObjectResult;

        // Assert
        _dbContext.Verify(x => x.Users, Times.Once);
        _mapper.Verify(x => x.Map<List<UserViewModel>>(_testData), Times.Once);
        result!.Value.Should().Be(usersViewModel);
    }
    
    [Test]
    public async Task DeleteUser_WithUserDeleteModel_ShouldDeleteUser()
    {
        // Arrange
        _testData = _fixture.Create<List<User>>();

        var model = new UserDeleteModel() { Id = _testData[0].Id };

        var userViewModel = new UserViewModel()
        {
            Id = _testData[0].Id,
            Role = _testData[0].Role,
            Name = _testData[0].Name,
            PasswordHash = _testData[0].PasswordHash,
            Email = _testData[0].Email!,
        };

        _mapper.Setup(x => x.Map<UserViewModel>(_testData[0]))
            .Returns(userViewModel);

        SetDataToContext(_testData.AsQueryable());
        
        var controller = new UserController(_dbContext.Object, _mapper.Object, _service.Object);

        // Act
        var result = await controller.DeleteUser(model) as OkObjectResult;

        // Assert
        _dbContext.Verify(x => x.Users, Times.Exactly(2));
        _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(x => x.Map<UserViewModel>(_testData[0]), Times.Once);
        result!.Value.Should().Be(userViewModel);
    }
    
    [Test]
    public async Task DeleteUser_WithUserDeleteModel_ShouldNotFindUser()
    {
        // Arrange
        var model = _fixture.Create<UserDeleteModel>();
        
        var controller = new UserController(_dbContext.Object, _mapper.Object, _service.Object);

        // Act
        var result = await controller.DeleteUser(model) as OkObjectResult;

        // Assert
        _dbContext.Verify(x => x.Users, Times.Once);
        result!.Value.Should().Be("Пользователь не найден");
    }
    
    [Test]
    public async Task UpdateUser_WithUserUpdateModel_ShouldUpdateUserCorrect()
    {
        // Arrange
        _testData = _fixture.Create<List<User>>();

        var model = _fixture.Build<UserUpdateModel>()
            .With(x => x.Id, _testData[0].Id)
            .Create();

        var userViewModel = new UserViewModel()
        {
            Id = model.Id,
            Role = (Role)model.Role!,
            Name = model.Name!,
            PasswordHash = model.Password!,
            Email = model.Email!,
        };

        _mapper.Setup(x => x.Map<UserViewModel>(It.IsAny<User>()))
            .Returns(userViewModel);

        SetDataToContext(_testData.AsQueryable());
        
        var controller = new UserController(_dbContext.Object, _mapper.Object, _service.Object);

        // Act
        var result = await controller.UpdateUser(model) as OkObjectResult;

        // Assert
        _dbContext.Verify(x => x.Users, Times.Exactly(2));
        _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(x => x.Map<UserViewModel>(It.IsAny<User>()), Times.Once);
        result!.Value.Should().Be(userViewModel);
    }
    
    [Test]
    public async Task UpdateUser_WithUserUpdateModel_ShouldNotFindUser()
    {
        // Arrange
        var model = _fixture.Create<UserUpdateModel>();
        
        var controller = new UserController(_dbContext.Object, _mapper.Object, _service.Object);

        // Act
        var result = await controller.UpdateUser(model) as OkObjectResult;

        // Assert
        _dbContext.Verify(x => x.Users, Times.Once);
        result!.Value.Should().Be("Пользователь не найден");
    }
    
    [Test]
    public async Task Login_WithLoginModel_ShouldReturnToken()
    {
        // Arrange
        var token = _fixture.Create<string>();
        
        var model = _fixture.Create<LoginModel>();

        _service.Setup(x => x.Login(model)).Returns(token);
        
        var controller = new UserController(_dbContext.Object, _mapper.Object, _service.Object);

        // Act
        var result = await controller.Login(model) as OkObjectResult;

        // Assert
        result!.Value.Should().Be(token);
    }
    
    [Test]
    public async Task Login_WithLoginModel_ShouldReturnEmptyToken()
    {
        // Arrange
        var model = _fixture.Create<LoginModel>();

        _service.Setup(x => x.Login(model)).Returns(string.Empty);
        
        var controller = new UserController(_dbContext.Object, _mapper.Object, _service.Object);

        // Act
        var result = await controller.Login(model) as OkObjectResult;

        // Assert
        result!.Value.Should().Be("Пользователь не найден");
    }
    
    [Test]
    public async Task Register_WithRegisterModel_ShouldCreateNewUser()
    {
        // Arrange
        var model = _fixture.Create<RegisterModel>();

        var user = new User()
        {
            Name = model.Name,
            PasswordHash = model.Password,
            Email = model.Email
        };
        
        var userViewModel = new UserViewModel()
        {
            Name = user.Name,
            PasswordHash = user.PasswordHash,
            Email = user.Email
        };

        _service.Setup(x => x.Register(model)).Returns(user);
        
        _mapper.Setup(x => x.Map<UserViewModel>(user))
            .Returns(userViewModel);
        
        var controller = new UserController(_dbContext.Object, _mapper.Object, _service.Object);

        // Act
        var result = await controller.Register(model) as OkObjectResult;

        // Assert
        _dbContext.Verify(x => x.Users, Times.Once);
        _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(x => x.Map<UserViewModel>(user), Times.Once);
        result!.Value.Should().Be(userViewModel);
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