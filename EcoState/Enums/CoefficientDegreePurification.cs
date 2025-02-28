namespace EcoState.Enums;

/// <summary>
/// Коэффицент степени очистки
/// </summary>
public enum CoefficientDegreePurification
{
    /// <summary>
    /// При среднем эксплуатационном коэффициенте очистки выбросов свыше 90%
    /// </summary>
    High = 1,
    
    /// <summary>
    /// При среднем эксплуатационном коэффициенте очистки выбросов от 75% до 90% включительно
    /// </summary>
    Medium = 2,
    
    /// <summary>
    /// При среднем эксплуатационном коэффициенте очистки выбросов менее 75% или отсутствии очистки выбросов
    /// </summary>
    Low = 3
}