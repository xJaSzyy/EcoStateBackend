using EcoState.Enums;

namespace EcoState.ViewModels.Concentration;

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
    public List<double> Concentrations { get; set; }
}