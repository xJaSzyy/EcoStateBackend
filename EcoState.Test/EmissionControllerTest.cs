using AutoFixture;
using AutoMapper;
using EcoState.Context;
using EcoState.Controllers;
using EcoState.Domain;
using EcoState.Enums;
using EcoState.Interfaces;
using EcoState.Services;
using EcoState.ViewModels.Concentration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;

namespace EcoState.Test;

public class EmissionControllerTest
{
    private Mock<ApplicationDbContext> _dbContext = new();
    private readonly Mock<IMapper> _mapper = new();
    private IEmissionService _service = new EmissionService();
    private readonly Fixture _fixture = new();
    
    private List<Emission> _emissions = new();
    private List<Concentration> _concentrations = new();
    
    [SetUp]
    public void Setup()
    {
        SetDataToContext(_emissions.AsQueryable(), _concentrations.AsQueryable());
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Reset();
        _mapper.Reset();

        _service = new EmissionService();
        _emissions = new List<Emission>();
        _concentrations = new List<Concentration>();
    }
    
    [Test]
    public async Task CalculateEmission_WithEmissionCalculateModel_ShouldReturnCorrectEmissionViewModel()
    {
        // Arrange
        var model = _fixture.Create<EmissionCalculateModel>();
        
        _service.Setup(model);
        var viewModel = _service.CalculateEmission();
        _service = new EmissionService();
        
        var controller = new EmissionController(_dbContext.Object, _mapper.Object, _service);
        
        // Act
        var result = await controller.CalculateEmission(model) as OkObjectResult;
        
        // Assert
        result!.Value.Should().BeEquivalentTo(viewModel);
    }
    
    [Test]
    public async Task CalculateConcentration_WithEmissionCalculateModelAndConcentrationType_ShouldReturnCorrectEmissionViewModel()
    {
        // Arrange
        var model = _fixture.Create<EmissionCalculateModel>();
        var concentrationType = _fixture.Create<ConcentrationType>();
        
        _service.Setup(model);
        var viewModel = _service.CalculateConcentration(concentrationType);
        _service = new EmissionService();
        
        var controller = new EmissionController(_dbContext.Object, _mapper.Object, _service);
        
        // Act
        var result = await controller.CalculateConcentration(model, concentrationType) as OkObjectResult;
        
        // Assert
        result!.Value.Should().BeEquivalentTo(viewModel);
    }
    
    [Test]
    public async Task SaveEmission_WithConcentrationList_ShouldCorrectSaveEmission()
    {
        // Arrange
        var concentrations = _fixture.Create<List<Concentration>>();
        
        var viewModel = new EmissionViewModel()
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Concentrations = concentrations
        };

        _dbContext.Setup(x => x.Emissions.Add(It.IsAny<Emission>())).Returns<Emission>(AddEmission);
        
        _mapper.Setup(x => x.Map<EmissionViewModel>(It.IsAny<Emission>()))
            .Returns(viewModel);
        
        
        var controller = new EmissionController(_dbContext.Object, _mapper.Object, _service);
        
        // Act
        var result = await controller.SaveEmission(concentrations) as OkObjectResult;
        
        // Assert
        result!.Value.Should().Be(viewModel);
        
        _mapper.Verify(x => x.Map<EmissionViewModel>(It.IsAny<Emission>()), Times.Once);
        _dbContext.Verify(x => x.Emissions.Add(It.IsAny<Emission>()), Times.Once);
        _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task SaveConcentration_WithConcentrationSaveModel_ShouldCorrectSaveConcentration()
    {
        // Arrange
        var model = _fixture.Create<ConcentrationSaveModel>();

        var concentration = new Concentration()
        {
            Type = model.Type,
            Concentrations = model.Concentrations,
            DangerZoneLength = model.DangerZoneLength,
            DangerZoneWidth = model.DangerZoneWidth,
        };
        
        var viewModel = new ConcentrationViewModel()
        {
            Type = model.Type,
            Concentrations = model.Concentrations,
            DangerZoneLength = model.DangerZoneLength,
            DangerZoneWidth = model.DangerZoneWidth,
        };

        _mapper.Setup(x => x.Map<Concentration>(model)).Returns(concentration);

        _dbContext.Setup(x => x.Concentrations.Add(It.IsAny<Concentration>())).Returns<Concentration>(AddConcentration);
        
        _mapper.Setup(x => x.Map<ConcentrationViewModel>(It.IsAny<Concentration>()))
            .Returns(viewModel);
        
        var controller = new EmissionController(_dbContext.Object, _mapper.Object, _service);
        
        // Act
        var result = await controller.SaveConcentration(model) as OkObjectResult;
        
        // Assert
        result!.Value.Should().Be(viewModel);
        
        _mapper.Verify(x => x.Map<Concentration>(model), Times.Once);
        _mapper.Verify(x => x.Map<ConcentrationViewModel>(It.IsAny<Concentration>()), Times.Once);
        _dbContext.Verify(x => x.Concentrations.Add(It.IsAny<Concentration>()), Times.Once);
        _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task GetEmissionByDate_WithEmissionGetByDateModel_ShouldReturnCorrectEmissions()
    {
        // Arrange
        var model = _fixture.Create<EmissionGetByDateModel>();

        var emission = _fixture.Build<Emission>()
            .With(x => x.Date, model.Date)
            .Create();
        
        _emissions.Add(emission);
        SetDataToContext(_emissions.AsQueryable(), _concentrations.AsQueryable());

        var viewModelList = new List<EmissionViewModel>()
        {
            new EmissionViewModel()
            {
                Id = emission.Id,
                Date = emission.Date,
                Concentrations = emission.Concentrations
            }
        };
        
        _mapper.Setup(x => x.Map<List<EmissionViewModel>>(It.IsAny<List<Emission>>())).Returns(viewModelList);
        
        var controller = new EmissionController(_dbContext.Object, _mapper.Object, _service);
        
        // Act
        var result = await controller.GetEmissionByDate(model) as OkObjectResult;
        
        // Assert
        result!.Value.Should().Be(viewModelList);
        
        _mapper.Verify(x => x.Map<List<EmissionViewModel>>(It.IsAny<List<Emission>>()), Times.Once);
        _dbContext.Verify(x => x.Emissions, Times.Once);
    }
    
    [Test]
    public async Task GetConcentrationByDate_WithConcentrationGetByDateModel_ShouldReturnCorrectConcentrations()
    {
        // Arrange
        var model = _fixture.Create<ConcentrationGetByDateModel>();

        var concentration = _fixture.Build<Concentration>()
            .With(x => x.Date, model.Date)
            .Create();
        
        _concentrations.Add(concentration);
        SetDataToContext(_emissions.AsQueryable(), _concentrations.AsQueryable());

        var viewModelList = new List<ConcentrationViewModel>()
        {
            new ConcentrationViewModel()
            {
                EmissionId = concentration.EmissionId,
                Date = concentration.Date,
                Type = concentration.Type,
                Concentrations = concentration.Concentrations,
                DangerZoneLength = concentration.DangerZoneLength,
                DangerZoneWidth = concentration.DangerZoneWidth
            }
        };
        
        _mapper.Setup(x => x.Map<List<ConcentrationViewModel>>(It.IsAny<List<Concentration>>())).Returns(viewModelList);
        
        var controller = new EmissionController(_dbContext.Object, _mapper.Object, _service);
        
        // Act
        var result = await controller.GetConcentrationByDate(model) as OkObjectResult;
        
        // Assert
        result!.Value.Should().Be(viewModelList);
        
        _mapper.Verify(x => x.Map<List<ConcentrationViewModel>>(It.IsAny<List<Concentration>>()), Times.Once);
        _dbContext.Verify(x => x.Concentrations, Times.Once);
    }
    
    [Test]
    public async Task GetConcentrationByType_WithConcentrationGetByTypeModel_ShouldReturnCorrectConcentrations()
    {
        // Arrange
        var model = _fixture.Create<ConcentrationGetByTypeModel>();

        var concentration = _fixture.Build<Concentration>()
            .With(x => x.Type, model.Type)
            .Create();
        
        _concentrations.Add(concentration);
        SetDataToContext(_emissions.AsQueryable(), _concentrations.AsQueryable());

        var viewModelList = new List<ConcentrationViewModel>()
        {
            new ConcentrationViewModel()
            {
                EmissionId = concentration.EmissionId,
                Date = concentration.Date,
                Type = concentration.Type,
                Concentrations = concentration.Concentrations,
                DangerZoneLength = concentration.DangerZoneLength,
                DangerZoneWidth = concentration.DangerZoneWidth
            }
        };
        
        _mapper.Setup(x => x.Map<List<ConcentrationViewModel>>(It.IsAny<List<Concentration>>())).Returns(viewModelList);
        
        var controller = new EmissionController(_dbContext.Object, _mapper.Object, _service);
        
        // Act
        var result = await controller.GetConcentrationByType(model) as OkObjectResult;
        
        // Assert
        result!.Value.Should().Be(viewModelList);
        
        _mapper.Verify(x => x.Map<List<ConcentrationViewModel>>(It.IsAny<List<Concentration>>()), Times.Once);
        _dbContext.Verify(x => x.Concentrations, Times.Once);
    }

    private void SetDataToContext(IQueryable<Emission> emissions, IQueryable<Concentration> concentrations)
    {
        var emissionsMockSet = new Mock<DbSet<Emission>>();
        emissionsMockSet.As<IQueryable<Emission>>().Setup(m => m.Provider).Returns(emissions.Provider);
        emissionsMockSet.As<IQueryable<Emission>>().Setup(m => m.Expression).Returns(emissions.Expression);
        emissionsMockSet.As<IQueryable<Emission>>().Setup(m => m.ElementType).Returns(emissions.ElementType);
        emissionsMockSet.As<IQueryable<Emission>>().Setup(m => m.GetEnumerator()).Returns(emissions.GetEnumerator);
        
        var concentrationsMockSet = new Mock<DbSet<Concentration>>();
        concentrationsMockSet.As<IQueryable<Concentration>>().Setup(m => m.Provider).Returns(concentrations.Provider);
        concentrationsMockSet.As<IQueryable<Concentration>>().Setup(m => m.Expression).Returns(concentrations.Expression);
        concentrationsMockSet.As<IQueryable<Concentration>>().Setup(m => m.ElementType).Returns(concentrations.ElementType);
        concentrationsMockSet.As<IQueryable<Concentration>>().Setup(m => m.GetEnumerator()).Returns(concentrations.GetEnumerator);
        
        _dbContext = new Mock<ApplicationDbContext>();
        _dbContext.Setup(c => c.Emissions).Returns(emissionsMockSet.Object);
        _dbContext.Setup(c => c.Concentrations).Returns(concentrationsMockSet.Object);
    }

    private EntityEntry<Emission> AddEmission(Emission emission)
    {
        _emissions.Add(emission);
        
        return null!;
    }
    
    private EntityEntry<Concentration> AddConcentration(Concentration concentration)
    {
        _concentrations.Add(concentration);
        
        return null!;
    }
}