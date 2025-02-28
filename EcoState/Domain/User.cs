using System.ComponentModel.DataAnnotations;

namespace EcoState.Domain;

public class User
{
    [Key]
    public Guid Id { get; set; }

    public Role Role { get; set; }
    
    public string Name { get; set; }
    
    public string PasswordHash { get; set; }
    
    public string? Email { get; set; }
}