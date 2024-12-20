using EcoState.Enums;

namespace EcoState.ViewModels.Concentration;

public class ConcentrationSaveModel
{
    /// <summary>
    /// Вид частиц
    /// </summary>
    public ConcentrationType Concentration { get; set; }
    
    /// <summary>
    /// Концентрации на дистанции некоторой
    /// </summary>
    public List<double> Concentrations { get; set; }
}