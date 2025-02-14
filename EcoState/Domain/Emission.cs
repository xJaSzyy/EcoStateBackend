using System.ComponentModel.DataAnnotations;

namespace EcoState.Domain;

/// <summary>
/// Выброс
/// </summary>
public class Emission
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Дата
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Концентрации
    /// </summary>
    public List<Concentration> Concentrations { get; set; } = null!;
}