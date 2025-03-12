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
            Lon = model.Lon,
            Lat = model.Lat,
            EjectedTemp = model.EjectedTemp,
            AvgExitSpeed = model.AvgExitSpeed,
            HeightSource = model.HeightSource,
            DiameterSource = model.DiameterSource,
            TempStratificationRatio = model.TempStratificationRatio,
            SedimentationRateRatio = model.SedimentationRateRatio
        };

        var viewModel = new EnterpriseViewModel()
        {
            Name = model.Name,
            City = model.City,
            Lon = model.Lon,
            Lat = model.Lat,
            EjectedTemp = model.EjectedTemp,
            AvgExitSpeed = model.AvgExitSpeed,
            HeightSource = model.HeightSource,
            DiameterSource = model.DiameterSource,
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
                Lon = item.Lon,
                Lat = item.Lat,
                EjectedTemp = item.EjectedTemp,
                AvgExitSpeed = item.AvgExitSpeed,
                HeightSource = item.HeightSource,
                DiameterSource = item.DiameterSource,
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
            Id = Guid.NewGuid(),
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
            Lon = testEnterprise.Lon,
            Lat = testEnterprise.Lat,
            EjectedTemp = testEnterprise.EjectedTemp,
            AvgExitSpeed = testEnterprise.AvgExitSpeed,
            HeightSource = testEnterprise.HeightSource,
            DiameterSource = testEnterprise.DiameterSource,
            TempStratificationRatio = testEnterprise.TempStratificationRatio,
            SedimentationRateRatio = testEnterprise.SedimentationRateRatio,
        };
        
        var viewModel = new EnterpriseViewModel()
        {
            Name = model.Name,
            City = testEnterprise.City,
            Lon = testEnterprise.Lon,
            Lat = testEnterprise.Lat,
            EjectedTemp = testEnterprise.EjectedTemp,
            AvgExitSpeed = testEnterprise.AvgExitSpeed,
            HeightSource = testEnterprise.HeightSource,
            DiameterSource = testEnterprise.DiameterSource,
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