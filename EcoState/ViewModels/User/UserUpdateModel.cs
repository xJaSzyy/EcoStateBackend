using EcoState.Domain;

namespace EcoState.ViewModels.User;

/// <summary>
/// Модель обновления пользователя
/// </summary>
public class UserUpdateModel
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Роль
    /// </summary>
    public Role? Role { get; set; }
    
    /// <summary>
    /// Логин/никнейм
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Пароль
    /// </summary>
    public string? Password { get; set; }
    
    /// <summary>
    /// Почтовый адрес
    /// </summary>
    public string? Email { get; set; }
}