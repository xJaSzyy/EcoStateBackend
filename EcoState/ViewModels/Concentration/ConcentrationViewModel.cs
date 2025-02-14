using EcoState.Enums;

namespace EcoState.ViewModels.Concentration;

/// <summary>
/// Модель представления концентрации
/// </summary>
public class ConcentrationViewModel
{
    /// <summary>
    /// Идентификатор выброса
    /// </summary>
    public Guid? EmissionId { get; set; } 
    
    /// <summary>
    /// Дата и время, когда были получены концентрации
    /// </summary>
    public DateTime? Date { get; set; }
    
    /// <summary>
    /// Вид частиц
    /// </summary>
    public ConcentrationType Type { get; set; }
    
    /// <summary>
    /// Концентрации на дистанции некоторой
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