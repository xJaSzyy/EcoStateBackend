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
    public string Role { get; set; }
    
    /// <summary>
    /// Логин/никнейм
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Пароль
    /// </summary>
    public string PasswordHash { get; set; }
    
    /// <summary>
    /// Почтовый адрес
    /// </summary>
    public string Email { get; set; }
}