using System.ComponentModel.DataAnnotations;

namespace EcoState.Domain;

/// <summary>
/// Пользователь
/// </summary>
public class User
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Роль
    /// </summary>
    public Role Role { get; set; }
    
    /// <summary>
    /// Логин
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Хэш пароля
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Почта
    /// </summary>
    public string? Email { get; set; }
}