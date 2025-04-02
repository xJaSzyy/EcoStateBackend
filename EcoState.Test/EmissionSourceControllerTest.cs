using AutoFixture;
using AutoMapper;
using EcoState.Context;
using EcoState.Controllers;
using EcoState.Domain;
using EcoState.ViewModels.EmissionSource;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;

namespace EcoState.Test;

public class EmissionSourceControllerTest
{
    private Mock<ApplicationDbContext> _dbContext = new();
    private Mock<IMapper> _mapper = new();
    private readonly Fixture _fixture = new();

    private List<EmissionSource> _testData = new();

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

        _testData = new List<EmissionSource>();
    }

    [Test]
    public async Task AddEmissionSource_WithEmissionSourceAddModel_ShouldAddEmissionSource()
    {
        // Arrange
        var model = _fixture.Create<EmissionSourceAddModel>();

        var emissionSource = new EmissionSource()
        {
            EnterpriseId = model.EnterpriseId,
            Lon = model.Lon,
            Lat = model.Lat,
            EjectedTemp = model.EjectedTemp,
            AvgExitSpeed = model.AvgExitSpeed,
            HeightSource = model.HeightSource,
            DiameterSource = model.DiameterSource
        };

        var viewModel = new EmissionSourceViewModel()
        {
            EnterpriseId = model.EnterpriseId,
            Lon = model.Lon,
            Lat = model.Lat,
            EjectedTemp = model.EjectedTemp,
            AvgExitSpeed = model.AvgExitSpeed,
            HeightSource = model.HeightSource,
            DiameterSource = model.DiameterSource
        };
        
        _mapper.Setup(x => x.Map<EmissionSource>(model)).Returns(emissionSource);
        _dbContext.Setup(x => x.EmissionSources.Add(emissionSource)).Returns<EmissionSource>(AddEmissionSource);
        _mapper.Setup(x => x.Map<EmissionSourceViewModel>(emissionSource)).Returns(viewModel);

        var controller = new EmissionSourceController(_dbContext.Object, _mapper.Object);

        // Act
        var result = await controller.AddEmissionSource(model) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
        result!.Value.Should().Be(viewModel);
        _testData.Should().HaveCount(1);
        _testData.Contains(emissionSource).Should().BeTrue();
        
        _dbContext.Verify(x => x.EmissionSources.Add(emissionSource), Times.Once);
        _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(x => x.Map<EmissionSource>(model), Times.Once);
        _mapper.Verify(x => x.Map<EmissionSourceViewModel>(emissionSource), Times.Once);
    }
    
    [Test]
    public async Task GetAllEmissionSources_ShouldReturnAllEmissionSources()
    {
        // Arrange
        _testData = _fixture.Create<List<EmissionSource>>();

        SetDataToContext(_testData.AsQueryable());

        var viewModelList = _testData.Select(item => new EmissionSourceViewModel()
            {
                EnterpriseId = item.EnterpriseId,
                Lon = item.Lon,
                Lat = item.Lat,
                EjectedTemp = item.EjectedTemp,
                AvgExitSpeed = item.AvgExitSpeed,
                HeightSource = item.HeightSource,
                DiameterSource = item.DiameterSource
            })
            .ToList();

        _mapper.Setup(x => x.Map<List<EmissionSourceViewModel>>(_testData)).Returns(viewModelList);

        var controller = new EmissionSourceController(_dbContext.Object, _mapper.Object);

        // Act
        var result = await controller.GetAllEmissionSources() as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
        result!.Value.Should().Be(viewModelList);
        var resultList = result!.Value as List<EmissionSourceViewModel>;
        resultList!.Count.Should().Be(_testData.Count);
        
        _dbContext.Verify(x => x.EmissionSources, Times.Once);
        _mapper.Verify(x => x.Map<List<EmissionSourceViewModel>>(_testData), Times.Once);
    }
    
    [Test]
    public async Task UpdateEmissionSource_WithEmissionSourceUpdateModel_ShouldCorrectUpdateEmissionSource()
    {
        // Arrange
        var model = new EmissionSourceUpdateModel()
        {
            Id = _fixture.Create<int>(),
            EjectedTemp = -10
        };
        var testEmissionSource = _fixture.Build<EmissionSource>()
            .With(x => x.Id, model.Id)
            .Without(x => x.Enterprise)
            .Create();
        
        _testData.Add(testEmissionSource);
        
        SetDataToContext(_testData.AsQueryable());
        
        var emissionSource = new EmissionSource()
        {
            Id = model.Id,
            EjectedTemp = (double)model.EjectedTemp,
            EnterpriseId = testEmissionSource.EnterpriseId,
            Lon = testEmissionSource.Lon,
            Lat = testEmissionSource.Lat,
            AvgExitSpeed = testEmissionSource.AvgExitSpeed,
            HeightSource = testEmissionSource.HeightSource,
            DiameterSource = testEmissionSource.DiameterSource
        };
        
        var viewModel = new EmissionSourceViewModel()
        {
            EjectedTemp = (double)model.EjectedTemp,
            EnterpriseId = testEmissionSource.EnterpriseId,
            Lon = testEmissionSource.Lon,
            Lat = testEmissionSource.Lat,
            AvgExitSpeed = testEmissionSource.AvgExitSpeed,
            HeightSource = testEmissionSource.HeightSource,
            DiameterSource = testEmissionSource.DiameterSource
        };
        
        _dbContext.Setup(x => x.EmissionSources.Update(It.IsAny<EmissionSource>())).Returns<EmissionSource>(UpdateEmissionSource);
        _mapper.Setup(x => x.Map<EmissionSourceViewModel>(testEmissionSource)).Returns(viewModel);

        var controller = new EmissionSourceController(_dbContext.Object, _mapper.Object);

        // Act
        var result = await controller.UpdateEmissionSource(model) as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.Value.Should().NotBeNull();
        result!.Value.Should().Be(viewModel);
        _testData.Should().HaveCount(1);
        _testData[0].Should().BeEquivalentTo(emissionSource);
        
        _dbContext.Verify(x => x.EmissionSources.Update(It.IsAny<EmissionSource>()), Times.Once);
        _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(x => x.Map<EmissionSourceViewModel>(testEmissionSource), Times.Once);
    }
    
    [Test]
    public async Task DeleteEmissionSource_WithId_ShouldDeleteEmissionSource()
    {
        // Arrange
        _testData = _fixture.Create<List<EmissionSource>>();

        var id =  _testData[0].Id;

        var emissionSourceViewModel = new EmissionSourceViewModel()
        {
            Id = _testData[0].Id,
            EnterpriseId = _testData[0].EnterpriseId,
            Lon = _testData[0].Lon,
            Lat = _testData[0].Lat,
            EjectedTemp = _testData[0].EjectedTemp,
            AvgExitSpeed = _testData[0].AvgExitSpeed,
            HeightSource = _testData[0].HeightSource,
            DiameterSource = _testData[0].DiameterSource
        };

        _mapper.Setup(x => x.Map<EmissionSourceViewModel>(_testData[0]))
            .Returns(emissionSourceViewModel);

        SetDataToContext(_testData.AsQueryable());
        
        var controller = new EmissionSourceController(_dbContext.Object, _mapper.Object);

        // Act
        var result = await controller.DeleteEmissionSource(id) as OkObjectResult;

        // Assert
        _dbContext.Verify(x => x.EmissionSources, Times.Exactly(2));
        _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapper.Verify(x => x.Map<EmissionSourceViewModel>(_testData[0]), Times.Once);
        result!.Value.Should().Be(emissionSourceViewModel);
    }
    
    [Test]
    public async Task DeleteEmissionSource_WithId_ShouldNotFoundEmissionSource()
    {
        // Arrange
        SetDataToContext(_testData.AsQueryable());
        
        var id = _fixture.Create<int>();
        
        var controller = new EmissionSourceController(_dbContext.Object, _mapper.Object);

        // Act
        var result = await controller.DeleteEmissionSource(id) as OkObjectResult;

        // Assert
        _dbContext.Verify(x => x.EmissionSources, Times.Once);
        result!.Value.Should().Be("Источник выброса не найден");
    }

    private void SetDataToContext(IQueryable<EmissionSource> testData)
    {
        var mockSet = new Mock<DbSet<EmissionSource>>();
        mockSet.As<IQueryable<EmissionSource>>().Setup(m => m.Provider).Returns(testData.Provider);
        mockSet.As<IQueryable<EmissionSource>>().Setup(m => m.Expression).Returns(testData.Expression);
        mockSet.As<IQueryable<EmissionSource>>().Setup(m => m.ElementType).Returns(testData.ElementType);
        mockSet.As<IQueryable<EmissionSource>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator);
        
        _dbContext = new Mock<ApplicationDbContext>();
        _dbContext.Setup(c => c.EmissionSources).Returns(mockSet.Object);
    }
    
    private EntityEntry<EmissionSource> AddEmissionSource(EmissionSource enterprise)
    {
        _testData.Add(enterprise);
        
        return null!;
    }
    
    private EntityEntry<EmissionSource> UpdateEmissionSource(EmissionSource emissionSource)
    {
        var item = _testData.FirstOrDefault(x => x.Id == emissionSource.Id);

        if (item != null)
        {
            _testData.Remove(item);
            _testData.Add(emissionSource);
        }
        
        return null!;
    }
}