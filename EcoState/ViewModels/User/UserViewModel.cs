using EcoState.Domain;

namespace EcoState.ViewModels.User;

public class UserViewModel
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Роль
    /// </summary>
    public Role Role { get; set; }
    
    /// <summary>
    /// Логин/никнейм
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Пароль
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Почтовый адрес
    /// </summary>
    public string Email { get; set; } = null!;
}