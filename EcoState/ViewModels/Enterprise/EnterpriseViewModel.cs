using EcoState.Enums;

namespace EcoState.ViewModels.Enterprise;

/// <summary>
/// Модель представления предприятия
/// </summary>
public class EnterpriseViewModel
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Список источников выбросов
    /// </summary>
    public List<Domain.EmissionSource> EmissionSources { get; set; }
    
    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Город
    /// </summary>
    public string City { get; set; }
    
    /// <summary>
    /// Коэффицент региона
    /// </summary>
    public CoefficientRegion TempStratificationRatio { get; set; }

    /// <summary>
    /// Коэффицент степени очистки
    /// </summary>
    public CoefficientDegreePurification SedimentationRateRatio { get; set; }
}