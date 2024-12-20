using AutoMapper;
using EcoState.Context;
using EcoState.Domain;
using EcoState.Extensions;
using EcoState.Interfaces;
using EcoState.ViewModels.Concentration;
using Microsoft.AspNetCore.Mvc;

namespace EcoState.Controllers;

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

    [HttpGet("concentraionList-calc")]
    public async Task<IActionResult> CalculateConcentrationList(ConcentrationListCalculateModel model)
    {
        _service.Setup(model);
        
        var result = _service.CalculateConcentrationList();
        
        return Ok(new Result<ConcentrationListViewModel>(result));
    }

    [HttpGet("concentraion-calc")]
    public async Task<IActionResult> CalculateConcentration(ConcentrationCalculateModel model)
    {
        _service.Setup(_mapper.Map<ConcentrationListCalculateModel>(model));
        
        var result = _service.CalculateConcentration(model.Concentration);
        
        return Ok(new Result<ConcentrationViewModel>(result));
    }

    [HttpPost("concentraionList-save")]
    public async Task<IActionResult> SaveConcentrationList(ConcentrationListSaveModel model)
    {
        var concentrationList = _mapper.Map<ConcentrationList>(model);
        concentrationList.Date = DateTime.Now;

        _dbContext.ConcentrationLists.Add(concentrationList);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<ConcentrationListViewModel>(concentrationList);
        
        return Ok(new Result<ConcentrationListViewModel>(result));
    }

    [HttpPost("concentraion-save")]
    public async Task<IActionResult> SaveConcentration(ConcentrationSaveModel model)
    {
        var concentration = _mapper.Map<Concentration>(model);
        concentration.Date = DateTime.Now;

        _dbContext.Concentrations.Add(concentration);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<ConcentrationViewModel>(concentration);
        
        return Ok(new Result<ConcentrationViewModel>(result));
    }

    [HttpGet("concentraionList-get")]
    public async Task<IActionResult> GetConcentrationList(ConcentrationListGetModel model)
    {
        var concentrationLists = _dbContext.ConcentrationLists.Where(x => x.Date == model.Date).ToList();
        
        var result = _mapper.Map<List<ConcentrationListViewModel>>(concentrationLists);
        
        return Ok(new Result<List<ConcentrationListViewModel>>(result));
    }

    [HttpGet("concentraion-get")]
    public async Task<IActionResult> GetConcentration(ConcentrationGetModel model)
    {
        var concentrations = _dbContext.Concentrations.Where(x => x.Date == model.Date).ToList();
        
        var result = _mapper.Map<List<ConcentrationViewModel>>(concentrations);
        
        return Ok(new Result<List<ConcentrationViewModel>>(result));
    }
}