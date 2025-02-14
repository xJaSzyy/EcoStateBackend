using System.ComponentModel.DataAnnotations;

namespace EcoState.Domain;

/// <summary>
/// Погода
/// </summary>
public class Weather
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Key] 
    public Guid Id { get; set; }
    
    /// <summary>
    /// Дата
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