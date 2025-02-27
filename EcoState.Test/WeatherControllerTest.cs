using AutoFixture;
using AutoMapper;
using EcoState.Context;
using EcoState.Controllers;
using EcoState.Domain;
using EcoState.Helpers;
using EcoState.ViewModels.Weather;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace EcoState.Test;

public class WeatherControllerTest
{
    private Mock<ApplicationDbContext> _dbContext = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Fixture _fixture = new();
    
    private List<Weather> _testData = new();
    
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

        _testData = new List<Weather>();
    }
    
    [Test]
    public async Task GetWeather_WithWeatherGetModel_ShouldReturnCorrectListWeatherViewModel()
    {
        // Arrange
        var dateTimeNow = DateTime.Now;
        var weatherGetModel = new WeatherGetModel()
        {
            Date = dateTimeNow
        };

        var testWeather = _fixture.Build<Weather>()
            .With(x => x.Date, dateTimeNow)
            .Create();

        _testData = new List<Weather>() { testWeather };
        SetDataToContext(_testData.AsQueryable());

        var weatherViewModelList = _testData.Select(weather => new WeatherViewModel()
            {
                Date = weather.Date,
                Temperature = weather.Temperature,
                WindDirection = weather.WindDirection,
                WindSpeed = weather.WindSpeed,
                IconUrl = weather.IconUrl
            })
            .Where(x => x.Date == dateTimeNow)
            .ToList();

        _mapper.Setup(x => x.Map<List<WeatherViewModel>>(It.IsAny<List<Weather>>()))
            .Returns(weatherViewModelList);

        var controller = GetWeatherController();
        
        // Act
        var result = await controller.GetWeather(weatherGetModel) as OkObjectResult;

        // Assert
        result!.Value.Should().Be(weatherViewModelList);
        foreach (var weather in weatherViewModelList)
        {
            weather.Date.Should().Be(dateTimeNow);
        }
        
        _dbContext.Verify(x => x.Weathers, Times.Once);
    }
    
    [Test]
    public async Task SaveWeather_WithWeatherSaveModel_ShouldCorrectSaveWeather()
    {
        // Arrange
        var weatherSaveModel = _fixture.Create<WeatherSaveModel>();

        var weather = new Weather()
        {
            Date = weatherSaveModel.Date,
            Temperature = weatherSaveModel.Temperature,
            WindDirection = weatherSaveModel.WindDirection,
            WindSpeed = weatherSaveModel.WindSpeed,
            IconUrl = weatherSaveModel.IconUrl
        };

        var weatherViewModel = new WeatherViewModel()
        {
            Date = weatherSaveModel.Date,
            Temperature = weatherSaveModel.Temperature,
            WindDirection = weatherSaveModel.WindDirection,
            WindSpeed = weatherSaveModel.WindSpeed,
            IconUrl = weatherSaveModel.IconUrl
        };
        
        _mapper.Setup(x => x.Map<Weather>(weatherSaveModel))
            .Returns(weather);
        _mapper.Setup(x => x.Map<WeatherViewModel>(weather))
            .Returns(weatherViewModel);

        var controller = GetWeatherController();
        
        // Act
        var result = await controller.SaveWeather(weatherSaveModel) as OkObjectResult;

        // Assert
        result!.Value.Should().Be(weatherViewModel);
        
        _dbContext.Verify(x => x.Weathers.Add(weather), Times.Once);
        _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    private void SetDataToContext(IQueryable<Weather> testData)
    {
        var mockSet = new Mock<DbSet<Weather>>();
        mockSet.As<IQueryable<Weather>>().Setup(m => m.Provider).Returns(testData.Provider);
        mockSet.As<IQueryable<Weather>>().Setup(m => m.Expression).Returns(testData.Expression);
        mockSet.As<IQueryable<Weather>>().Setup(m => m.ElementType).Returns(testData.ElementType);
        mockSet.As<IQueryable<Weather>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator);
        
        _dbContext = new Mock<ApplicationDbContext>();
        _dbContext.Setup(c => c.Weathers).Returns(mockSet.Object);
    }

    private WeatherController GetWeatherController()
    {
        var settings = new Mock<IOptions<WeatherSettings>>();
        var factory = new Mock<IHttpClientFactory>();
        
        return new WeatherController(_dbContext.Object, _mapper.Object, settings.Object, factory.Object);
    }
}