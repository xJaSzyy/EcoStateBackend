namespace EcoState.ViewModels.Concentration;

/// <summary>
/// Модель получения концентрации по дате 
/// </summary>
public class ConcentrationGetByDateModel
{
    /// <summary>
    /// Дата и время, когда были получены концентрации
    /// </summary>
    public DateTime Date { get; set; }
}