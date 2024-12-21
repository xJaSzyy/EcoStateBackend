using System.ComponentModel.DataAnnotations;

namespace EcoState.Domain;

public class ConcentrationList
{
    [Key]
    public Guid Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public List<Concentration> Concentrations { get; set; }
}