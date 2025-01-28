using System.ComponentModel.DataAnnotations;

namespace EcoState.ViewModels.User;

public class RegisterModel
{
    [Required(ErrorMessage = "Не указано имя")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Не указан пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
}