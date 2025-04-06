using System.Net;
using AutoMapper;
using EcoState.Context;
using EcoState.Domain;
using EcoState.Extensions;
using EcoState.Interfaces;
using EcoState.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace EcoState.Controllers;

/// <summary>
/// Контроллер пользователей
/// </summary>
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserService _service;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="dbContext">ApplicationDbContext</param>
    /// <param name="mapper">IMapper</param>
    /// <param name="service">Интерфейс сервиса пользователей</param>
    public UserController(ApplicationDbContext dbContext, IMapper mapper, IUserService service)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _service = service;
    }

    /// <summary>
    /// Метод получения пользователя из БД по ID
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns></returns>
    [HttpGet("user/{id}")]
    [ProducesResponseType(typeof(UserViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
        
        if (user == null)
        {
            return Ok("Пользователь не найден");
        }

        var result = _mapper.Map<UserViewModel>(user);

        return Ok(result);
    }
    
    /// <summary>
    /// Метод получения всех пользователей из БД
    /// </summary>
    /// <returns></returns>
    [HttpGet("user")]
    [ProducesResponseType(typeof(List<UserViewModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetAllUsers()
    {
        var userList = _dbContext.Users.ToList();

        var result = _mapper.Map<List<UserViewModel>>(userList);

        return Ok(result);
    }
    
    /// <summary>
    /// Метод удаления пользователя из БД по ID
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns></returns>
    [EnumAuthorize(Role.Admin)]
    [HttpDelete("user/{id}")]
    [ProducesResponseType(typeof(UserViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            return Ok("Пользователь не найден");
        }
        
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<UserViewModel>(user);

        return Ok(result);
    }

    /// <summary>
    /// Метод изменения пользователя из БД по ID
    /// </summary>
    /// <param name="model">Модель обновления пользователя</param>
    /// <returns></returns>
    [EnumAuthorize(Role.Admin)]
    [HttpPut("user")]
    [ProducesResponseType(typeof(UserViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateUser(UserUpdateModel model)
    {
        var user = _dbContext.Users.FirstOrDefault(x => x.Id == model.Id);

        if (user == null)
        {
            return Ok("Пользователь не найден");
        }

        if (model.Role != null) user.Role = (Role)model.Role;
        if (model.Name != null) user.Name = model.Name;
        if (model.Password != null) user.PasswordHash = model.Password;
        if (model.Email != null) user.Email = model.Email;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<UserViewModel>(user);

        return Ok(result);
    }

    /// <summary>
    /// Метод входа пользователя в систему
    /// </summary>
    /// <param name="model">Модель входа</param>
    /// <returns></returns>
    [HttpPost("user/login")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var token = _service.Login(model);
        
        return Ok(token);
    }

    /// <summary>
    /// Модель регистрации пользователя в системе
    /// </summary>
    /// <param name="model">Модель регистрации</param>
    /// <returns></returns>
    [HttpPost("user/register")]
    [ProducesResponseType(typeof(UserViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var user = _service.Register(model);
        
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<UserViewModel>(user);

        return Ok(result);
    }
}