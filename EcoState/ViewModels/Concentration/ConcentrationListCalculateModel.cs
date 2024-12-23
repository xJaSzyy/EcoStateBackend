using System.ComponentModel.DataAnnotations;
using EcoState.Enums;

namespace EcoState.ViewModels.Concentration;

public class ConcentrationListCalculateModel
{
    /// <summary>
    /// Температура выбрасываемой ГВС
    /// </summary>
    [Range(235, 265)]
    public double Tgam { get; set; }

    /// <summary>
    /// Температура атмосферного воздуха
    /// </summary>
    [Range(-40, 40)]
    public double Ta { get; set; }

    /// <summary>
    /// Средняя скорость выхода ГВС из устья источника выброса, м/с
    /// </summary>
    [Range(15, 25)]
    public double w0 { get; set; }

    /// <summary>
    /// Высота источника выброса, м.
    /// </summary>
    [Range(13, 65)]
    public double H { get; set; }

    /// <summary>
    /// Диаметр устья источника, м.
    /// </summary>
    [Range(1, 7)]
    public double D { get; set; }

    /// <summary>
    /// Коэффицент региона
    /// </summary>
    public CoefficientRegion A { get; set; }

    /// <summary>
    /// Коэффицент степени очистки
    /// </summary>
    public CoefficientDegreePurification F { get; set; }
}