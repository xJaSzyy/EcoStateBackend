using System.ComponentModel.DataAnnotations;
using EcoState.Enums;

namespace EcoState.ViewModels.Concentration;

/// <summary>
/// Модель расчета концентраций выброса
/// </summary>
public class EmissionCalculateModel
{
    /// <summary>
    /// Температура выбрасываемой ГВС
    /// </summary>
    [Range(235, 265)]
    public double EjectedTemp { get; set; }

    /// <summary>
    /// Температура атмосферного воздуха
    /// </summary>
    [Range(-40, 40)]
    public double AirTemp { get; set; }

    /// <summary>
    /// Средняя скорость выхода ГВС из устья источника выброса, м/с
    /// </summary>
    [Range(15, 25)]
    public double AvgExitSpeed { get; set; }

    /// <summary>
    /// Высота источника выброса, м.
    /// </summary>
    [Range(13, 65)]
    public double HeightSource { get; set; }

    /// <summary>
    /// Диаметр устья источника, м.
    /// </summary>
    [Range(1, 7)]
    public double DiameterSource { get; set; }

    /// <summary>
    /// Коэффицент региона
    /// </summary>
    public CoefficientRegion TempStratificationRatio { get; set; }

    /// <summary>
    /// Коэффицент степени очистки
    /// </summary>
    public CoefficientDegreePurification SedimentationRateRatio { get; set; }
    
    /// <summary>
    /// Скорость ветра
    /// </summary>
    public double WindSpeed { get; set; }
}