using System.ComponentModel.DataAnnotations;

namespace EcoState.ViewModels.User;

public class LoginModel
{
    [Required(ErrorMessage = "Не указано имя")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Не указан пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}