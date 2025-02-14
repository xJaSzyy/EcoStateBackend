namespace EcoState.ViewModels.Concentration;

/// <summary>
/// Модель представления случайных концентраций
/// </summary>
public class ConcentrationRandomViewModel
{
    /// <summary>
    /// Ширина зоны выброса
    /// </summary>
    public double DangerZoneWidth { get; set; }
    
    /// <summary>
    /// Длина зоны выброса
    /// </summary>
    public double DangerZoneLength { get; set; }
    
    /// <summary>
    /// Значения концентрации
    /// </summary>
    public List<double> Concentrations { get; set; } = null!;
}