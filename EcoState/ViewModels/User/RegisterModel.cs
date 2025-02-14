using System.ComponentModel.DataAnnotations;

namespace EcoState.ViewModels.User;

public class RegisterModel
{
    [Required(ErrorMessage = "Не указано имя")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Не указан пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
}