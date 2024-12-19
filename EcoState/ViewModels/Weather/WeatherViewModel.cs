namespace EcoState.ViewModels.Weather;

public class WeatherViewModel
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
}