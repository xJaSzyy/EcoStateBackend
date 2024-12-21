using System.ComponentModel.DataAnnotations;

namespace EcoState.Domain;

public class User
{
    [Key]
    public Guid Id { get; set; }

    public string Role { get; set; } = "User";
    
    public string Name { get; set; }
    
    public string PasswordHash { get; set; }
    
    public string? Email { get; set; }
}