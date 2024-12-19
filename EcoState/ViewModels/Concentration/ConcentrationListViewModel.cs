namespace EcoState.ViewModels.Concentration;

public class ConcentrationListViewModel
{
    /// <summary>
    /// Концентрации SO2 на дистанции некоторой
    /// </summary>
    public List<double> ConcentrationsSO2 { get; set; }
    
    /// <summary>
    /// Концентрации NO на дистанции некоторой
    /// </summary>
    public List<double> ConcentrationsNO { get; set; }
    
    /// <summary>
    /// Концентрации NO2 на дистанции некоторой
    /// </summary>
    public List<double> ConcentrationsNO2 { get; set; }
    
    /// <summary>
    /// Концентрации CO2 на дистанции некоторой
    /// </summary>
    public List<double> ConcentrationsCO2 { get; set; }
    
    /// <summary>
    /// Концентрации SP на дистанции некоторой
    /// </summary>
    public List<double> ConcentrationsSP { get; set; }
}