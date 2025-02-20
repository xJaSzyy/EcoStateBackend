using AutoFixture;
using AutoMapper;
using EcoState.Context;
using EcoState.Controllers;
using EcoState.Domain;
using EcoState.Extensions;
using EcoState.Helpers;
using EcoState.Interfaces;
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

        _testData = new List<Weather>();
    }
    
    [Test]
    public async Task Test()
    {
        // Arrange
        var _mapper = new Mock<IMapper>();
        var _settings = new Mock<IOptions<WeatherSettings>>();
        var _factory = new Mock<IHttpClientFactory>();

        var dateTimeNow = DateTime.Now;
        
        var weatherGetModel = new WeatherGetModel()
        {
            Date = dateTimeNow
        };

        var weatherList = new List<Weather>() { new Weather() { Date = dateTimeNow, Temperature = 1 } };
        SetDataToContext(weatherList.AsQueryable());

        _mapper.Setup(x => x.Map<List<WeatherViewModel>>(It.IsAny<List<Weather>>()))
            .Returns(new List<WeatherViewModel>() { new WeatherViewModel() { Temperature = 1, Date = weatherGetModel.Date}});
        
        var controller = new WeatherController(_dbContext.Object, _mapper.Object, _settings.Object, _factory.Object);

        // Act
        var result = await controller.GetWeather(weatherGetModel) as OkObjectResult;

        // Assert
        //result.Result.Should().Be("");

        //result.Value.Should().Be("");
        result.Value.Should().Be("");
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
}