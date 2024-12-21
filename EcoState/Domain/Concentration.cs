using System.ComponentModel.DataAnnotations;

namespace EcoState.Domain;

public class Concentration
{
    [Key]
    public Guid Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public List<double> Concentrations { get; set; }
}