using System.ComponentModel.DataAnnotations;

namespace EcoState.Domain;

public class Role
{
    public int Id { get; set; }
    
    public string Name { get; set; }
}