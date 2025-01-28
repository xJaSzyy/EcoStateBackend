using AutoMapper;
using EcoState.Context;
using EcoState.Domain;
using EcoState.Enums;
using EcoState.Extensions;
using EcoState.Interfaces;
using EcoState.ViewModels.Concentration;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("concentrationList-calc")]
    public async Task<IActionResult> CalculateConcentrationList(ConcentrationListCalculateModel model)
    {
        _service.Setup(model);
        
        var result = _service.CalculateConcentrationList();
        
        return Ok(new Result<ConcentrationListViewModel>(result));
    }

    [HttpGet("concentration-calc")]
    public async Task<IActionResult> CalculateConcentration(ConcentrationCalculateModel model)
    {
        _service.Setup(model);
        
        var result = _service.CalculateConcentration(model.Concentration);
        
        return Ok(new Result<ConcentrationViewModel>(result));
    }
    
    [HttpGet("concentration-rnd")]
    public async Task<IActionResult> RandomConcentration()
    {
        var model = new ConcentrationCalculateModel()
        {
            Concentration = ConcentrationType.SP,
            Tgam = 235,
            Ta = 10,
            w0 = 15,
            H = 13,
            D = 2,
            A = CoefficientRegion.SouthernPart,
            F = CoefficientDegreePurification.High
        };
        
        _service.Setup(_mapper.Map<ConcentrationListCalculateModel>(model));
        
        var result = _service.CalculateConcentration(model.Concentration);
        
        int maxIndex = -1;
        double maxValue = -1;
        int maxDistance = -1;

        List<double> range = new List<double>();

        for (int i = 0; i < result.Concentrations.Count; i++)
        {
            range.Add(result.Concentrations[i]);

            if (result.Concentrations.Max() == result.Concentrations[i])
            {
                maxValue = result.Concentrations[i];
                maxIndex = i;
                maxDistance = (i + 1) * 5;
                break;
            }
        }

        double med = 0;

        range.Sort();
        if (range.Count % 2 == 0)
        {
            med = (range[(range.Count / 2)] + range[(range.Count / 2) - 1]) / 2;
        }
        else
        {
            med = range[(range.Count / 2)];
        }


        int minDistance = -1;
        for (int i = maxIndex; i < result.Concentrations.Count; i++)
        {
            if (result.Concentrations[i] < med)
            {
                minDistance = (i + 1) * 5;
                break;
            }
        }

        var randomViewModel = new ConcentrationRandomViewModel()
        {
            DangerZoneLength = minDistance,
            DangerZoneHalfWidth = minDistance - maxDistance,
            Concentrations = result.Concentrations
        };
        
        return Ok(new Result<ConcentrationRandomViewModel>(randomViewModel));
    }

    [HttpPost("concentrationList-save")]
    public async Task<IActionResult> SaveConcentrationList(ConcentrationListSaveModel model)
    {
        var concentrationList = _mapper.Map<ConcentrationList>(model);
        concentrationList.Date = DateTime.Now;
        concentrationList.Id = Guid.NewGuid();

        _dbContext.ConcentrationLists.Add(concentrationList);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<ConcentrationListViewModel>(concentrationList);
        
        return Ok(new Result<ConcentrationListViewModel>(result));
    }

    [HttpPost("concentration-save")]
    public async Task<IActionResult> SaveConcentration(ConcentrationSaveModel model)
    {
        var concentration = _mapper.Map<Concentration>(model);
        concentration.Date = DateTime.Now;
        concentration.Id = Guid.NewGuid();

        _dbContext.Concentrations.Add(concentration);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<ConcentrationViewModel>(concentration);
        
        return Ok(new Result<ConcentrationViewModel>(result));
    }

    [HttpGet("concentrationList-get")]
    public async Task<IActionResult> GetConcentrationList(ConcentrationListGetModel model)
    {
        var concentrationLists = _dbContext.ConcentrationLists.Where(x => x.Date == model.Date).ToList();
        
        var result = _mapper.Map<List<ConcentrationListViewModel>>(concentrationLists);
        
        return Ok(new Result<List<ConcentrationListViewModel>>(result));
    }

    [HttpGet("concentration-get")]
    public async Task<IActionResult> GetConcentration(ConcentrationGetModel model)
    {
        var concentrations = _dbContext.Concentrations.Where(x => x.Date == model.Date).ToList();
        
        var result = _mapper.Map<List<ConcentrationViewModel>>(concentrations);
        
        return Ok(new Result<List<ConcentrationViewModel>>(result));
    }
}