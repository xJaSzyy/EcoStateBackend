namespace EcoState.ViewModels.Concentration;

/// <summary>
/// Модель представления выброса
/// </summary>
public class EmissionViewModel
{
    /// <summary>
    /// Идентификатор выброса
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Дата и время, когда были получены концентрации
    /// </summary>
    public DateTimeOffset Date { get; set; }
    
    /// <summary>
    /// Концентрации на дистанции некоторой
    /// </summary>
    public List<Domain.Concentration> Concentrations { get; set; } = null!;
}