using AutoMapper;
using EcoState.Context;
using EcoState.Domain;
using EcoState.Enums;
using EcoState.Extensions;
using EcoState.Interfaces;
using EcoState.ViewModels.Concentration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoState.Controllers;

/// <summary>
/// Контроллер выбросов
/// </summary>
public class EmissionController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IEmissionService _service;
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="dbContext">ApplicationDbContext</param>
    /// <param name="mapper">IMapper</param>
    /// <param name="service">Интерфейс сервиса выбросов</param>
    public EmissionController(ApplicationDbContext dbContext, IMapper mapper, IEmissionService service)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _service = service;
    }

    /// <summary>
    /// Метод расчета нескольких концентраций выброса
    /// </summary>
    /// <param name="model">Модель расчета концентраций выброса</param>
    /// <returns></returns>
    [HttpGet("emission-calc")]
    public async Task<IActionResult> CalculateEmission(EmissionCalculateModel model)
    {
        _service.Setup(model);
        
        var result = _service.CalculateEmission();
        
        return Ok(result);
    }

    /// <summary>
    /// Метод расчета конкретной концентрации выброса
    /// </summary>
    /// <param name="model">Модель расчета концентраций выброса</param>
    /// <param name="concentration">Вид частиц</param>
    /// <returns></returns>
    [HttpGet("concentraion-calc")]
    public async Task<IActionResult> CalculateConcentration(EmissionCalculateModel model, ConcentrationType concentration)
    {
        _service.Setup(model);
        
        var result = _service.CalculateConcentration(concentration);
        
        return Ok(result);
    }

    /// <summary>
    /// Метод сохранения нескольких концентраций выброса в базу данных
    /// </summary>
    /// <param name="concentrations">Список концентраций</param>
    /// <returns></returns>
    [EnumAuthorize(Role.Admin)]
    [HttpPost("emission-save")]
    public async Task<IActionResult> SaveEmission([FromBody] List<Concentration> concentrations)
    {
        var emission = new Emission()
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Concentrations = concentrations
        };
        
        _dbContext.Emissions.Add(emission);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<EmissionViewModel>(emission);
        
        return Ok(result);
    }

    /// <summary>
    /// Метод сохранения конкретной концентрации выброса в базу данных
    /// </summary>
    /// <param name="model">Модель сохранения концентрации</param>
    /// <returns></returns>
    [EnumAuthorize(Role.Admin)]
    [HttpPost("concentration-save")]
    public async Task<IActionResult> SaveConcentration(ConcentrationSaveModel model)
    {
        var concentration = _mapper.Map<Concentration>(model);
        concentration.Id = Guid.NewGuid();

        _dbContext.Concentrations.Add(concentration);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<ConcentrationViewModel>(concentration);
        
        return Ok(result);
    }

    /// <summary>
    /// Метод получения данных о выбросе в конкретную дату
    /// </summary>
    /// <param name="model">Модель получения выброса по дате</param>
    /// <returns></returns>
    [HttpGet("emission-getByDate")]
    public async Task<IActionResult> GetEmissionByDate(EmissionGetByDateModel model)
    {
        var emissions = _dbContext.Emissions
            .Include(c => c.Concentrations)
            .Where(x => x.Date == model.Date).ToList();
        
        var result = _mapper.Map<List<EmissionViewModel>>(emissions);
        
        return Ok(result);
    }

    /// <summary>
    /// Метод получения данных о концентрации в конкретную дату
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet("concentraion-getByDate")]
    public async Task<IActionResult> GetConcentrationByDate(ConcentrationGetByDateModel model)
    {
        var concentrations = _dbContext.Concentrations
            .Where(x => x.Date == model.Date).ToList();
        
        var result = _mapper.Map<List<ConcentrationViewModel>>(concentrations);
        
        return Ok(result);
    }

    /// <summary>
    /// Метод получения данных о концентрациях по типу концентрации
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet("concentraion-getByType")]
    public async Task<IActionResult> GetConcentrationByType(ConcentrationGetByTypeModel model)
    {
        var concentrations = _dbContext.Concentrations
            .Where(x => x.Type == model.Type).ToList();
        
        var result = _mapper.Map<List<ConcentrationViewModel>>(concentrations);
        
        return Ok(result);
    }
}

