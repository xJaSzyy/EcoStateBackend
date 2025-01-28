using EcoState.Domain;
using EcoState.ViewModels.User;

namespace EcoState.Interfaces;

public interface IUserService
{
    public string Login(LoginModel model);
    
    public User Register(RegisterModel model);
}