using System.ComponentModel.DataAnnotations;
using EcoState.Enums;

namespace EcoState.Domain;

public class Concentration
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid? EmissionId { get; set; }
    
    public DateTime? Date { get; set; }
    
    public ConcentrationType Type { get; set; }
    
    public List<double> Concentrations { get; set; }
}