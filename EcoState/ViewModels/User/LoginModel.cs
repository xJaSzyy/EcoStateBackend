using System.ComponentModel.DataAnnotations;

namespace EcoState.ViewModels.User;

/// <summary>
/// Модель входа
/// </summary>
public class LoginModel
{
    /// <summary>
    /// Логин пользователя
    /// </summary>
    [Required(ErrorMessage = "Не указано имя")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    [Required(ErrorMessage = "Не указан пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}