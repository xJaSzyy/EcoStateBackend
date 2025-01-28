namespace EcoState.Domain;

/// <summary>
/// Роль пользователя
/// </summary>
[Flags]
public enum Role
{
    /// <summary>
    /// Обычный пользователь
    /// </summary>
    User = 0,
    
    /// <summary>
    /// Пользователь с привилегиями администратора
    /// </summary>
    Admin = 1
}