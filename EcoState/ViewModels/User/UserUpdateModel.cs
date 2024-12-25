namespace EcoState.ViewModels.User;

public class UserUpdateModel
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Идентификатор роли
    /// </summary>
    public int? RoleId { get; set; }
    
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