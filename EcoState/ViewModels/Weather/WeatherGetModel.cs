using System.ComponentModel.DataAnnotations;

namespace EcoState.ViewModels.Weather;

/// <summary>
/// Модель получения погоды
/// </summary>
public class WeatherGetModel
{
    /// <summary>
    /// День
    /// </summary>
    [Range(1, 31)]
    public int Day { get; set; }
    
    /// <summary>
    /// Месяц
    /// </summary>
    [Range(1, 12)]
    public int Month { get; set; }
    
    /// <summary>
    /// Год
    /// </summary>
    public int Year { get; set; }
}