using EcoState.Domain;
using EcoState.ViewModels.User;

namespace EcoState.Interfaces;

/// <summary>
/// Интерфейс сервиса пользователей
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Попытка входа пользователя
    /// </summary>
    /// <param name="model">Модель входа пользователя</param>
    /// <returns></returns>
    public string Login(LoginModel model);
    
    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="model">Модель регистрации пользователя</param>
    /// <returns></returns>
    public User Register(RegisterModel model);
}