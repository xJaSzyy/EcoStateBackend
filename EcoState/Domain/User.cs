using System.ComponentModel.DataAnnotations;

namespace EcoState.Domain;

public class User
{
    [Key]
    public int Id { get; set; }

    public string Role { get; set; } = "User";
    
    public string Name { get; set; }
    
    public string Password { get; set; }

    public string Email { get; set; } 
}