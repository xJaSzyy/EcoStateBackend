using AutoFixture;
using AutoMapper;
using EcoState.Context;
using EcoState.Controllers;
using EcoState.Domain;
using EcoState.Interfaces;
using EcoState.ViewModels.User;
using FluentAssertions;
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
    public async Task Test()
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
        _dbContext.Setup(x => x.Users.FindAsync(model.Id)).ReturnsAsync(user);
        
        var controller = new UserController(_dbContext.Object, _mapper.Object, _service.Object);

        // Act
        var result = await controller.GetUser(model) as OkObjectResult;

        // Assert
        _dbContext.Verify(x => x.Users.FindAsync(model.Id), Times.Once);
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