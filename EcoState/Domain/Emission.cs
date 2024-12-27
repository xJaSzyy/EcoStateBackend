using System.ComponentModel.DataAnnotations;

namespace EcoState.Domain;

public class Emission
{
    [Key]
    public Guid Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public List<Concentration> Concentrations { get; set; }
}