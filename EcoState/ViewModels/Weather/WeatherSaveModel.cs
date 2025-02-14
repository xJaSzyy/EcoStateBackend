namespace EcoState.ViewModels.Weather;

/// <summary>
/// Модель сохранения погоды
/// </summary>
public class WeatherSaveModel
{
    /// <summary>
    /// Дата и время, в которые была такая погода
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Температура
    /// </summary>
    public float Temperature { get; set; }
    
    /// <summary>
    /// Направление ветра
    /// </summary>
    public int WindDirection { get; set; }
    
    /// <summary>
    /// Скорость ветра
    /// </summary>
    public float WindSpeed { get; set; }
    
    /// <summary>
    /// Ссылка на иконку
    /// </summary>
    public string IconUrl { get; set; } = null!;
}