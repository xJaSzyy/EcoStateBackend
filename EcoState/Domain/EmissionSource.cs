using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcoState.Domain;

/// <summary>
/// Источник выброса
/// </summary>
public class EmissionSource
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Идентификатор предприятия
    /// </summary>
    public int EnterpriseId { get; set; }
    
    /// <summary>
    /// Предприятие
    /// </summary>
    [JsonIgnore]
    public Enterprise Enterprise { get; set; } = null!;

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