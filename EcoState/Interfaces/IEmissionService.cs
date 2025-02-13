using EcoState.Enums;
using EcoState.ViewModels.Concentration;

namespace EcoState.Interfaces;

/// <summary>
/// Интерфейс сервиса выбросов
/// </summary>
public interface IEmissionService
{
    /// <summary>
    /// Установить стартовые значения для дальнейших расчетов
    /// </summary>
    /// <param name="model">Модель расчета концентраций выброса</param>
    public void Setup(EmissionCalculateModel model);
    
    /// <summary>
    /// Рассчитать концентрации всех частиц
    /// </summary>
    /// <returns></returns>
    public EmissionViewModel CalculateEmission();

    /// <summary>
    /// Рассчитать концентрации выбранной частицы
    /// </summary>
    /// <param name="type">Тип концентрации</param>
    /// <returns></returns>
    public ConcentrationViewModel CalculateConcentration(ConcentrationType type);
}