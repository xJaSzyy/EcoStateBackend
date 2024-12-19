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
    
    /// <summary>
    /// Значение скорости ветра в диапазоне от 0.5 м/с до u_mr
    /// </summary>
    public double u { get; set; }
    
    /// <summary>
    /// Расстояние от источника выброса
    /// </summary>
    public double x { get; set; }
    
    /// <summary>
    /// Расстояние по нормали к оси факела
    /// </summary>
    public double y { get; set; }

    /// <summary>
    /// Гряда, гребень, холм 
    /// </summary>
    public double ridge { get; set; }
    
    /// <summary>
    /// Ложбина, долина, котловина, впадина
    /// </summary>
    public double hollow { get; set; }
    
    /// <summary>
    /// Уступ
    /// </summary>
    public double ledge { get; set; }
    
    /// <summary>
    /// Верхнее плато
    /// </summary>
    public bool upperPlateau { get; set; }
    
    /// <summary>
    /// Рельеф местности
    /// </summary>
    public Landform landform { get; set; }

    /// <summary>
    /// Высота (глубина) формы рельефа, м.
    /// </summary>
    public double h0 { get; set; }
    
    /// <summary>
    /// Полуширина гряды, холма, ложбины или протяженность бокового склона уступа, м.
    /// </summary>
    public double a0 { get; set; }
}