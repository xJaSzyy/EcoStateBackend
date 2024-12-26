using EcoState.Enums;
using EcoState.ViewModels.Concentration;

namespace EcoState.Interfaces;

public interface IEmissionService
{
    /// <summary>
    /// Установить стартовые значения для дальнейших расчетов
    /// </summary>
    /// <param name="model"></param>
    public void Setup(ConcentrationCalculateModel model);
    
    /// <summary>
    /// Рассчитать концентрации всех частиц
    /// </summary>
    /// <returns></returns>
    public ConcentrationListViewModel CalculateConcentrationList();

    /// <summary>
    /// Рассчитать концентрации выбранной частицы
    /// </summary>
    /// <param name="concentration"></param>
    /// <returns></returns>
    public ConcentrationViewModel CalculateConcentration(ConcentrationType concentration);
}