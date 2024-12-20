namespace EcoState.ViewModels.User;

public class UserAddModel
{
    /// <summary>
    /// Логин/никнейм
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Пароль
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    /// Почтовый адрес
    /// </summary>
    public string Email { get; set; }
}