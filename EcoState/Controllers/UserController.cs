using AutoMapper;
using EcoState.Context;
using EcoState.Domain;
using EcoState.Extensions;
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

    public UserController(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpGet("user-get")]
    public async Task<IActionResult> GetUser(UserGetModel model)
    {
        var user = await _dbContext.Users.FindAsync(model.Id);
        
        if (user == null)
        {
            return Ok(new Result()
            {
                ErrorMessage = "Пользователь не найден",
                ReturnCode = 13
            });
        }

        var result = _mapper.Map<UserViewModel>(user);

        return Ok(new Result<UserViewModel>(result));
    }

    [HttpGet("user-getAll")]
    public async Task<IActionResult> GetAllUsers()
    {
        var userList = _dbContext.Users.ToList();

        var result = _mapper.Map<List<UserViewModel>>(userList);

        return Ok(new Result<List<UserViewModel>>(result));
    }

    [HttpPost("user-add")]
    public async Task<IActionResult> AddUser(UserAddModel model)
    {
        var user = _mapper.Map<User>(model);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<UserViewModel>(user);

        return Ok(new Result<UserViewModel>(result));
    }

    [HttpPost("user-delete")]
    public async Task<IActionResult> DeleteUser(UserDeleteModel model)
    {
        var user = await _dbContext.Users.FindAsync(model.Id);

        if (user == null)
        {
            return Ok(new Result()
            {
                ErrorMessage = "Пользователь не найден",
                ReturnCode = 13
            });
        }
        
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<UserViewModel>(user);

        return Ok(new Result<UserViewModel>(result));
    }

    [HttpPost("user-update")]
    public async Task<IActionResult> UpdateUser(UserUpdateModel model)
    {
        var user = await _dbContext.Users.FindAsync(model.Id);

        if (user == null)
        {
            return Ok(new Result()
            {
                ErrorMessage = "Пользователь не найден",
                ReturnCode = 13
            });
        }

        if (model.Role != null) user.Role = model.Role;
        if (model.Name != null) user.Name = model.Name;
        if (model.Password != null) user.Password = model.Password;
        if (model.Email != null) user.Email = model.Email;

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<UserViewModel>(user);

        return Ok(new Result<UserViewModel>(result));
    }

    /*[HttpPost("user-login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        
    }

    [HttpPost("user-register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        
    }*/
}