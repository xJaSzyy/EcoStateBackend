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

    public UserController(ApplicationDbContext dbContext, IMapper mapper, IUserService service)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _service = service;
    }

    [HttpGet("user-get")]
    public async Task<IActionResult> GetUser([FromBody] UserGetModel model)
    {
        var user = await _dbContext.Users.FindAsync(model.Id);
        
        if (user == null)
        {
            return Ok(Results.NotFound(new { message= "Пользователь не найден"}));
        }

        var result = _mapper.Map<UserViewModel>(user);

        return Ok(result);
    }
    
    [HttpGet("user-getAll")]
    public async Task<IActionResult> GetAllUsers()
    {
        var userList = _dbContext.Users.ToList();

        var result = _mapper.Map<List<UserViewModel>>(userList);

        return Ok(result);
    }
    
    [EnumAuthorize(Role.Admin)]
    [HttpPost("user-delete")]
    public async Task<IActionResult> DeleteUser([FromBody] UserDeleteModel model)
    {
        var user = await _dbContext.Users.FindAsync(model.Id);

        if (user == null)
        {
            return Ok(Results.NotFound(new { message= "Пользователь не найден"}));
        }
        
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<UserViewModel>(user);

        return Ok(result);
    }

    [EnumAuthorize(Role.Admin)]
    [HttpPost("user-update")]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateModel model)
    {
        var user = await _dbContext.Users.FindAsync(model.Id);

        if (user == null)
        {
            return Ok(Results.NotFound(new { message= "Пользователь не найден"}));
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

    [HttpPost("user-login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var token = _service.Login(model);

        if (token == string.Empty)
        {
            return Ok(Results.NotFound(new { message= "Пользователь не найден"}));
        }
        
        return Ok(token);
    }

    [HttpPost("user-register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = _service.Register(model);
        
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<UserViewModel>(user);

        return Ok(result);
    }
}