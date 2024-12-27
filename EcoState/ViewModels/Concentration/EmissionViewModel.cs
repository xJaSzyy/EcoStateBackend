namespace EcoState.ViewModels.Concentration;

public class EmissionViewModel
{
    /// <summary>
    /// Идентификатор выброса
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Дата и время, когда были получены концентрации
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Концентрации на дистанции некоторой
    /// </summary>
    public List<Domain.Concentration> Concentrations { get; set; }
}