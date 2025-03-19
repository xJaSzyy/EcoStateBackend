using System.ComponentModel.DataAnnotations;
using EcoState.Enums;

namespace EcoState.Domain;

/// <summary>
/// Концентрация
/// </summary>
public class Concentration
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Идентификатор выброса
    /// </summary>
    public int? EmissionId { get; set; }
    
    /// <summary>
    /// Дата
    /// </summary>
    public DateTimeOffset Date { get; set; }
    
    /// <summary>
    /// Тип концентрации
    /// </summary>
    public ConcentrationType Type { get; set; }
    
    /// <summary>
    /// Значения концентрации
    /// </summary>
    public List<double> Concentrations { get; set; } = null!;
    
    /// <summary>
    /// Предельно допустимая концентрация
    /// </summary>
    public double PDK { get; set; }
    
    /// <summary>
    /// Среднее значение из n макисмальных концентраций
    /// </summary>
    public double AverageConcentration { get; set; }

    /// <summary>
    /// Длина зоны выброса
    /// </summary>
    public double DangerZoneLength { get; set; }
    
    /// <summary>
    /// Ширина зоны выброса
    /// </summary>
    public double DangerZoneWidth { get; set; }
    
    /// <summary>
    /// Цвет зоны выброса
    /// </summary>
    public string DangerZoneColorHex { get; set; } = null!;
    
    /// <summary>
    /// Угол смещения зоны выброса
    /// </summary>
    public double DangerZoneAngle { get; set; }
}