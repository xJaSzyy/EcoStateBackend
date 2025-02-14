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
    public Guid Id { get; set; }
    
    /// <summary>
    /// Идентификатор выброса
    /// </summary>
    public Guid? EmissionId { get; set; }
    
    /// <summary>
    /// Дата
    /// </summary>
    public DateTime? Date { get; set; }
    
    /// <summary>
    /// Тип концентрации
    /// </summary>
    public ConcentrationType Type { get; set; }
    
    /// <summary>
    /// Значения концентрации
    /// </summary>
    public List<double> Concentrations { get; set; } = null!;

    /// <summary>
    /// Длина зоны выброса
    /// </summary>
    public double DangerZoneLength { get; set; }
    
    /// <summary>
    /// Ширина зоны выброса
    /// </summary>
    public double DangerZoneWidth { get; set; }
}