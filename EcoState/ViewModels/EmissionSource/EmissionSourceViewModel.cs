namespace EcoState.ViewModels.EmissionSource;

/// <summary>
/// Модель представления источника выбросов
/// </summary>
public class EmissionSourceViewModel
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Идентификатор предприятия
    /// </summary>
    public int EnterpriseId { get; set; }
    
    /// <summary>
    /// Долгота
    /// </summary>
    public double Lon { get; set; }
    
    /// <summary>
    /// Широта
    /// </summary>
    public double Lat { get; set; }
    
    /// <summary>
    /// Температура выбрасываемой ГВС
    /// </summary>
    public double EjectedTemp { get; set; }
    
    /// <summary>
    /// Средняя скорость выхода ГВС из устья источника выброса, м/с
    /// </summary>
    public double AvgExitSpeed { get; set; }

    /// <summary>
    /// Высота источника выброса, м.
    /// </summary>
    public double HeightSource { get; set; }

    /// <summary>
    /// Диаметр устья источника, м.
    /// </summary>
    public double DiameterSource { get; set; }
    
    /// <summary>
    /// Последняя концентрация от выброса
    /// </summary>
    public double LastConcentration { get; set; }
}