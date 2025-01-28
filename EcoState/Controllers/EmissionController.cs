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

    public EmissionController(ApplicationDbContext dbContext, IMapper mapper, IEmissionService service)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _service = service;
    }

    [HttpGet("emission-calc")]
    public async Task<IActionResult> CalculateEmission(EmissionCalculateModel model)
    {
        _service.Setup(model);
        
        var result = _service.CalculateEmission();
        
        return Ok(new Result<EmissionViewModel>(result));
    }

    [HttpGet("concentraion-calc")]
    public async Task<IActionResult> CalculateConcentration(EmissionCalculateModel model, ConcentrationType concentration)
    {
        _service.Setup(model);
        
        var result = _service.CalculateConcentration(concentration);
        
        return Ok(new Result<ConcentrationViewModel>(result));
    }

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
        
        return Ok(new Result<EmissionViewModel>(result));
    }

    [EnumAuthorize(Role.Admin)]
    [HttpPost("concentraion-save")]
    public async Task<IActionResult> SaveConcentration(ConcentrationSaveModel model)
    {
        var concentration = _mapper.Map<Concentration>(model);
        concentration.Id = Guid.NewGuid();

        _dbContext.Concentrations.Add(concentration);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<ConcentrationViewModel>(concentration);
        
        return Ok(new Result<ConcentrationViewModel>(result));
    }

    [HttpGet("emission-getByDate")]
    public async Task<IActionResult> GetEmissionByDate(EmissionGetByDateModel model)
    {
        var emissions = _dbContext.Emissions
            .Include(c => c.Concentrations)
            .Where(x => x.Date == model.Date).ToList();
        
        var result = _mapper.Map<List<EmissionViewModel>>(emissions);
        
        return Ok(new Result<List<EmissionViewModel>>(result));
    }
    
    [HttpGet("concentraion-getByDate")]
    public async Task<IActionResult> GetConcentrationByDate(ConcentrationGetByDateModel model)
    {
        var concentrations = _dbContext.Concentrations
            .Where(x => x.Date == model.Date).ToList();
        
        var result = _mapper.Map<List<ConcentrationViewModel>>(concentrations);
        
        return Ok(new Result<List<ConcentrationViewModel>>(result));
    }

    [HttpGet("concentraion-getByType")]
    public async Task<IActionResult> GetConcentrationByType(ConcentrationGetByTypeModel model)
    {
        var concentrations = _dbContext.Concentrations
            .Where(x => x.Type == model.Type).ToList();
        
        var result = _mapper.Map<List<ConcentrationViewModel>>(concentrations);
        
        return Ok(new Result<List<ConcentrationViewModel>>(result));
    }
}