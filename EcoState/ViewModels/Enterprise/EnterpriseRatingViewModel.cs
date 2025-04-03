namespace EcoState.ViewModels.Enterprise;

/// <summary>
/// Модель представления предприятия в рейтинге
/// </summary>
public class EnterpriseRatingViewModel
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Город
    /// </summary>
    public string City { get; set; }
    
    /// <summary>
    /// Место в рейтинге
    /// </summary>
    public int Place { get; set; }
    
    /// <summary>
    /// СрЗнач последних концентраций по всем источникам выброса предприятия
    /// </summary>
    public double AverageConcentration { get; set; }
}