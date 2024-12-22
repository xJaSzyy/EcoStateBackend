using System.Text.Json;
using AutoMapper;
using EcoState.Context;
using EcoState.Domain;
using EcoState.Extensions;
using EcoState.Helpers;
using EcoState.ViewModels.Weather;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EcoState.Controllers;

/// <summary>
/// Контроллер погоды
/// </summary>
public class WeatherController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IOptions<WeatherSettings> _options;
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherController(ApplicationDbContext dbContext, IMapper mapper, IOptions<WeatherSettings> options, IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _options = options;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("currentWeather-get")]
    public async Task<IActionResult> GetCurrentWeather([FromQuery] string city)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString["q"] = city;
        queryString["appid"] = _options.Value.ApiKey;
        queryString["units"] = _options.Value.Units;

        var requestUrl = $"{_options.Value.BaseUrl}?{queryString}";
        
        try
        {
            var response = await httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Ошибка при запросе к API погоды.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(responseContent);

            var result = new WeatherViewModel
            {
                Date = DateTime.UtcNow,
                Temperature = (float)weatherResponse.MainData.Temp,
                WindSpeed = (float)weatherResponse.WindData.Speed,
                WindDirection = weatherResponse.WindData.Deg,
                IconUrl = $"https://openweathermap.org/img/wn/{weatherResponse.WeatherData[0].Icon}@2x.png"
            };

            return Ok(new Result<WeatherViewModel>(result));
        }
        catch (Exception ex)
        {
            return Ok(new Result<string>(ex.ToString()));
        }
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
        weather.Id = Guid.NewGuid();
        
        _dbContext.Weathers.Add(weather);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<WeatherViewModel>(weather);

        return Ok(new Result<WeatherViewModel>(result));
    }
}