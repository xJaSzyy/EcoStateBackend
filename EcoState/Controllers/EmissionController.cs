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

    [HttpGet("concentration-rnd")]
    public async Task<IActionResult> GetRandomConcentration()
    {
        var model = new EmissionCalculateModel()
        {
            Tgam = RandomDouble(235, 265),
            Ta = RandomDouble(-30, -20),
            w0 = RandomDouble(15, 25),
            D = RandomDouble(1, 7),
            H = RandomDouble(13, 65),
            A = CoefficientRegion.SouthernPart,
            F = CoefficientDegreePurification.High,
        };

        _service.Setup(model);

        var calculateConcentration = _service.CalculateConcentration(ConcentrationType.SP);

        int maxIndex = -1;
        double maxValue = -1;
        int maxDistance = -1;

        List<double> range = new List<double>();

        for (int i = 0; i < calculateConcentration.Concentrations.Count; i++)
        {
            range.Add(calculateConcentration.Concentrations[i]);

            if (calculateConcentration.Concentrations.Max() == calculateConcentration.Concentrations[i])
            {
                maxValue = calculateConcentration.Concentrations[i];
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
        for (int i = maxIndex; i < calculateConcentration.Concentrations.Count; i++)
        {
            if (calculateConcentration.Concentrations[i] < med)
            {
                minDistance = (i + 1) * 5;
                break;
            }
        }

        var result = new ConcentrationRandomViewModel()
        {
            DangerZoneLength = minDistance,
            DangerZoneHalfWidth = (minDistance - maxDistance),
            Concentrations = calculateConcentration.Concentrations
        };

        return Ok(new Result<ConcentrationRandomViewModel>(result));
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
    
    public static double RandomDouble(double min, double max)
    {
        Random random = new Random();
        return min + random.NextDouble() * (max - min);
    }
}

