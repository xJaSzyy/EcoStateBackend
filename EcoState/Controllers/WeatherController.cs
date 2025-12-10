using System.Net;
using System.Text.Json;
using AutoMapper;
using EcoState.Context;
using EcoState.Domain;
using EcoState.Extensions;
using EcoState.Helpers;
using EcoState.ViewModels.User;
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

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="dbContext">ApplicationDbContext</param>
    /// <param name="mapper">IMapper</param>
    /// <param name="options">IOptions</param>
    /// <param name="httpClientFactory">IHttpClientFactory</param>
    public WeatherController(ApplicationDbContext dbContext, IMapper mapper, IOptions<WeatherSettings> options, IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _options = options;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Метод получения текущей погоды
    /// </summary>
    /// <param name="city">Название города</param>
    /// <returns></returns>
    [HttpGet("weather/current")]
    [ProducesResponseType(typeof(WeatherViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetCurrentWeather([FromQuery] string city)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var weatherUrl =
            "https://api.open-meteo.com/v1/forecast?latitude=55.355198&longitude=86.086847&current_weather=true";
        
        try
        {
            var response = await httpClient.GetAsync(weatherUrl);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Ошибка при запросе к API погоды.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var weatherResponse = JsonSerializer.Deserialize<OpenMeteoResponse>(responseContent);

            var result = new WeatherViewModel
            {
                Date = DateTime.UtcNow.Date,
                Temperature = (float)weatherResponse!.CurrentWeather.Temperature,
                WindSpeed = (float)weatherResponse.CurrentWeather.WindSpeed,
                WindDirection = (int)weatherResponse.CurrentWeather.WindDirection,
                IconUrl = GetWeatherIconUrl(weatherResponse.CurrentWeather.WeatherCode, weatherResponse.CurrentWeather.IsDay == 1)
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return Ok(ex.ToString());
        }
    }

    /// <summary>
    /// Метод получения погоды из БД по дате
    /// </summary>
    /// <param name="model">Модель получения погоды</param>
    /// <returns></returns>
    [HttpGet("weather")]
    [ProducesResponseType(typeof(List<WeatherViewModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetWeather(WeatherGetModel model)
    {
        var targetDate = new DateTime(model.Year, model.Month, model.Day).Date;
        var weathers = _dbContext.Weathers.Where(x => x.Date == targetDate.Date).ToList();

        var result = _mapper.Map<List<WeatherViewModel>>(weathers);

        return Ok(result);
    }

    /// <summary>
    /// Метод сохранения погоды в БД
    /// </summary>
    /// <param name="model">Модель сохранения погоды</param>
    /// <returns></returns>
    [EnumAuthorize(Role.Admin)]
    [HttpPost("weather")]
    [ProducesResponseType(typeof(WeatherViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> SaveWeather(WeatherSaveModel model)
    {
        var weather = _mapper.Map<Weather>(model);
        weather.Date = DateTime.UtcNow.Date;
        
        _dbContext.Weathers.Add(weather);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<WeatherViewModel>(weather);

        return Ok(result);
    }

    #region Temp

    private (string Description, string IconName) GetWeatherInfo(int weatherCode, bool isDay)
    {
        return weatherCode switch
        {
            0 => isDay ? 
                ("Ясное небо", "01d") : 
                ("Ясная ночь", "01n"),
                
            1 => isDay ? 
                ("Преимущественно ясно", "02d") : 
                ("Преимущественно ясно", "02n"),
                
            2 => isDay ? 
                ("Переменная облачность", "03d") : 
                ("Переменная облачность", "03n"),
                
            3 => ("Пасмурно", "04"),
                
            45 or 48 => ("Туман", "50"),
                
            51 or 53 or 55 => ("Морось", "09"),
                
            56 or 57 => ("Ледяная морось", "09"),
                
            61 or 63 or 65 => ("Дождь", "10"),
                
            66 or 67 => ("Ледяной дождь", "10"),
                
            71 or 73 or 75 => ("Снег", "13"),
                
            77 => ("Снежные зёрна", "13"),
                
            80 or 81 or 82 => ("Ливень", "09"),
                
            85 or 86 => ("Снегопад", "13"),
                
            95 => ("Гроза", "11"),
                
            96 or 99 => ("Гроза с градом", "11"),
                
            _ => ("Неизвестно", "01d")
        };
    }

    private string GetWeatherIconUrl(int weatherCode, bool isDay)
    {
        var (description, iconName) = GetWeatherInfo(weatherCode, isDay);
        return $"https://openweathermap.org/img/wn/{iconName}@2x.png";
    }

    #endregion
}