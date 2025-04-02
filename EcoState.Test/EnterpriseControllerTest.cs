using AutoFixture;
using AutoMapper;
using EcoState.Context;
using EcoState.Controllers;
using EcoState.Domain;
using EcoState.ViewModels.Enterprise;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;

namespace EcoState.Test;

public class EnterpriseControllerTest
{
    private Mock<ApplicationDbContext> _dbContext = new();
    private Mock<IMapper> _mapper = new();
    private readonly Fixture _fixture = new();

    private List<Enterprise> _testData = new();

    [SetUp]
    public void Setup()
    {
        _dbContext = new Mock<ApplicationDbContext>();
        _mapper = new Mock<IMapper>();
        
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Reset();
        _mapper.Reset();

        _testData = new List<Enterprise>();
    }

    [Test]
    public async Task AddEnterprise_WithEnterpriseAddModel_ShouldAddEnterprise()
    {
        // Arrange
        var model = _fixture.Create<EnterpriseAddModel>();

        var enterprise = new Enterprise()
        {
            Name = model.Name,
            City = model.City,
            TempStratificationRatio = model.TempStratificationRatio,
            SedimentationRateRatio = model.SedimentationRateRatio
        };

        var viewModel = new EnterpriseViewModel()
        {
            Name = model.Name,
            City = model.City,
            TempStratificationRatio = model.TempStratificationRatio,
            SedimentationRateRatio = model.SedimentationRateRatio
        };
        
        _mapper.Setup(x => x.Map<Enterprise>(model)).Returns(enterprise);
        _dbContext.Setup(x => x.Enterprises.Add(enterprise)).Returns<Enterprise>(AddEnterprise);
        _mapper.Setup(x => x.Map<EnterpriseViewModel>(enterprise)).Returns(viewModel);

        var controller = new EnterpriseController(_dbContext.Object, _mapper.Object);

        // Act
        var result = await controller.AddEnterprise(model) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
        result!.Value.Should().Be(viewModel);
        _testData.Should().HaveCount(1);
        _testData.Contains(enterprise).Should().BeTrue();
        
        _dbContext.Verify(x => x.Enterprises.Add(enterprise), Times.Once);
        _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(x => x.Map<Enterprise>(model), Times.Once);
        _mapper.Verify(x => x.Map<EnterpriseViewModel>(enterprise), Times.Once);
    }
    
    [Test]
    public async Task GetAllEnterprises_ShouldReturnAllEnterprises()
    {
        // Arrange
        _testData = _fixture.Create<List<Enterprise>>();

        SetDataToContext(_testData.AsQueryable());

        var viewModelList = _testData.Select(item => new EnterpriseViewModel()
            {
                Name = item.Name,
                City = item.City,
                TempStratificationRatio = item.TempStratificationRatio,
                SedimentationRateRatio = item.SedimentationRateRatio
            })
            .ToList();

        _mapper.Setup(x => x.Map<List<EnterpriseViewModel>>(_testData)).Returns(viewModelList);

        var controller = new EnterpriseController(_dbContext.Object, _mapper.Object);

        // Act
        var result = await controller.GetAllEnterprises() as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
        result!.Value.Should().Be(viewModelList);
        var resultList = result!.Value as List<EnterpriseViewModel>;
        resultList!.Count.Should().Be(_testData.Count);
        
        _dbContext.Verify(x => x.Enterprises, Times.Once);
        _mapper.Verify(x => x.Map<List<EnterpriseViewModel>>(_testData), Times.Once);
    }
    
    [Test]
    public async Task UpdateEnterprise_WithEnterpriseUpdateModel_ShouldCorrectUpdateEnterprise()
    {
        // Arrange
        var model = new EnterpriseUpdateModel()
        {
            Id = _fixture.Create<int>(),
            Name = "NewName"
        };
        var testEnterprise = _fixture.Build<Enterprise>()
            .With(x => x.Id, model.Id)
            .Create();
        
        _testData.Add(testEnterprise);
        
        SetDataToContext(_testData.AsQueryable());
        
        var enterprise = new Enterprise()
        {
            Id = model.Id,
            Name = model.Name,
            City = testEnterprise.City,
            TempStratificationRatio = testEnterprise.TempStratificationRatio,
            SedimentationRateRatio = testEnterprise.SedimentationRateRatio,
            EmissionSources = testEnterprise.EmissionSources
        };
        
        var viewModel = new EnterpriseViewModel()
        {
            Name = model.Name,
            City = testEnterprise.City,
            TempStratificationRatio = testEnterprise.TempStratificationRatio,
            SedimentationRateRatio = testEnterprise.SedimentationRateRatio,
        };
        
        _dbContext.Setup(x => x.Enterprises.Update(It.IsAny<Enterprise>())).Returns<Enterprise>(UpdateEnterprise);
        _mapper.Setup(x => x.Map<EnterpriseViewModel>(testEnterprise)).Returns(viewModel);

        var controller = new EnterpriseController(_dbContext.Object, _mapper.Object);

        // Act
        var result = await controller.UpdateEnterprise(model) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
        result!.Value.Should().Be(viewModel);
        _testData.Should().HaveCount(1);
        _testData[0].Should().BeEquivalentTo(enterprise);
        
        _dbContext.Verify(x => x.Enterprises.Update(It.IsAny<Enterprise>()), Times.Once);
        _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(x => x.Map<EnterpriseViewModel>(testEnterprise), Times.Once);
    }
    
    [Test]
    public async Task DeleteEnterprise_WithId_ShouldDeleteEnterprise()
    {
        // Arrange
        _testData = _fixture.Create<List<Enterprise>>();

        var id =  _testData[0].Id;

        var enterpriseViewModel = new EnterpriseViewModel()
        {
            Id = _testData[0].Id,
            Name = _testData[0].Name,
            City = _testData[0].City,
            TempStratificationRatio = _testData[0].TempStratificationRatio,
            SedimentationRateRatio = _testData[0].SedimentationRateRatio,
        };

        _mapper.Setup(x => x.Map<EnterpriseViewModel>(_testData[0]))
            .Returns(enterpriseViewModel);

        SetDataToContext(_testData.AsQueryable());
        
        var controller = new EnterpriseController(_dbContext.Object, _mapper.Object);

        // Act
        var result = await controller.DeleteEnterprise(id) as OkObjectResult;

        // Assert
        _dbContext.Verify(x => x.Enterprises, Times.Exactly(2));
        _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(x => x.Map<EnterpriseViewModel>(_testData[0]), Times.Once);
        result!.Value.Should().Be(enterpriseViewModel);
    }
    
    [Test]
    public async Task DeleteEnterprise_WithId_ShouldNotFoundEnterprise()
    {
        // Arrange
        SetDataToContext(_testData.AsQueryable());
        
        var id = _fixture.Create<int>();
        
        var controller = new EnterpriseController(_dbContext.Object, _mapper.Object);

        // Act
        var result = await controller.DeleteEnterprise(id) as OkObjectResult;

        // Assert
        _dbContext.Verify(x => x.Enterprises, Times.Once);
        result!.Value.Should().Be("Предприятие не найдено");
    }

    private void SetDataToContext(IQueryable<Enterprise> testData)
    {
        var mockSet = new Mock<DbSet<Enterprise>>();
        mockSet.As<IQueryable<Enterprise>>().Setup(m => m.Provider).Returns(testData.Provider);
        mockSet.As<IQueryable<Enterprise>>().Setup(m => m.Expression).Returns(testData.Expression);
        mockSet.As<IQueryable<Enterprise>>().Setup(m => m.ElementType).Returns(testData.ElementType);
        mockSet.As<IQueryable<Enterprise>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator);
        
        _dbContext = new Mock<ApplicationDbContext>();
        _dbContext.Setup(c => c.Enterprises).Returns(mockSet.Object);
    }
    
    private EntityEntry<Enterprise> AddEnterprise(Enterprise enterprise)
    {
        _testData.Add(enterprise);
        
        return null!;
    }
    
    private EntityEntry<Enterprise> UpdateEnterprise(Enterprise enterprise)
    {
        var item = _testData.FirstOrDefault(x => x.Id == enterprise.Id);

        if (item != null)
        {
            _testData.Remove(item);
            _testData.Add(enterprise);
        }
        
        return null!;
    }
}