using EcoState.Enums;

namespace EcoState.ViewModels.Concentration;

public class ConcentrationListCalculateModel
{
    /// <summary>
    /// Температура выбрасываемой ГВС
    /// </summary>
    public double Tgam { get; set; }
    
    /// <summary>
    /// Температура атмосферного воздуха
    /// </summary>
    public double Ta { get; set; }
    
    /// <summary>
    /// Средняя скорость выхода ГВС из устья источника выброса, м/с
    /// </summary>
    public double w0 { get; set; }
    
    /// <summary>
    /// Высота источника выброса, м.
    /// </summary>
    public double H { get; set; }
    
    /// <summary>
    /// Диаметр устья источника, м.
    /// </summary>
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