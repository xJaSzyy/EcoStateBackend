using AutoMapper;
using EcoState.Context;
using EcoState.Domain;
using EcoState.Extensions;
using EcoState.ViewModels.Weather;
using Microsoft.AspNetCore.Mvc;

namespace EcoState.Controllers;

/// <summary>
/// Контроллер погоды
/// </summary>
public class WeatherController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public WeatherController(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpGet("currentWeather-get")]
    public async Task<IActionResult> GetCurrentWeather()
    {
        //запрос к api погоды
        var result = new WeatherViewModel(); 
        
        return Ok(new Result<WeatherViewModel>(result));
    }

    [HttpGet("weather-get")]
    public async Task<IActionResult> GetWeather(WeatherGetModel model)
    {
        var weathers = _dbContext.Weathers.Where(x => x.Date == model.Date).ToList();

        var result = _mapper.Map<List<WeatherViewModel>>(weathers);

        return Ok(new Result<List<WeatherViewModel>>(result));
    }

    [HttpPost("weather-save")]
    public async Task<IActionResult> SaveWeather(WeatherSaveModel model)
    {
        var weather = _mapper.Map<Weather>(model);

        _dbContext.Weathers.Add(weather);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<WeatherViewModel>(weather);

        return Ok(new Result<WeatherViewModel>(result));
    }
}