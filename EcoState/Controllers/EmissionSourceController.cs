using System.Net;
using AutoMapper;
using EcoState.Context;
using EcoState.Domain;
using EcoState.Enums;
using EcoState.ViewModels.EmissionSource;
using EcoState.ViewModels.Enterprise;
using Microsoft.AspNetCore.Mvc;

namespace EcoState.Controllers;

/// <summary>
/// Контроллер источника выбросов
/// </summary>
public class EmissionSourceController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="dbContext">ApplicationDbContext</param>
    /// <param name="mapper">IMapper</param>
    public EmissionSourceController(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Метод добавления источника выбросов
    /// </summary>
    /// <param name="model">Модель добавления источника выбросов</param>
    /// <returns></returns>
    [HttpPost("emission/source")]
    [ProducesResponseType(typeof(EmissionSourceViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> AddEmissionSource(EmissionSourceAddModel model)
    {
        var emissionSource = _mapper.Map<EmissionSource>(model);
        
        _dbContext.EmissionSources.Add(emissionSource);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<EmissionSourceViewModel>(emissionSource);
        
        return Ok(result);
    }
    
    /// <summary>
    /// Метод получения всех источников выбросов
    /// </summary>
    /// <returns></returns>
    [HttpGet("emission/source")]
    [ProducesResponseType(typeof(List<EmissionSourceViewModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetAllEmissionSources()
    {
        var emissionSources = _dbContext.EmissionSources.ToList();

        var result = _mapper.Map<List<EmissionSourceViewModel>>(emissionSources);

        return Ok(result);
    }
    
    /// <summary>
    /// Метод изменения источника выбросов
    /// </summary>
    /// <param name="model">Модель изменения источника выбросов</param>
    /// <returns></returns>
    [HttpPut("emission/source")]
    [ProducesResponseType(typeof(EmissionSourceViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateEmissionSource(EmissionSourceUpdateModel model)
    {
        var emissionSource = _dbContext.EmissionSources.FirstOrDefault(x => x.Id == model.Id);

        if (emissionSource == null)
        {
            return Ok("Источник выброса не найден");
        }

        if (model.EnterpriseId != null) emissionSource.EnterpriseId = (int)model.EnterpriseId;
        if (model.Lon != null) emissionSource.Lon = (double)model.Lon;
        if (model.Lat != null) emissionSource.Lat = (double)model.Lat;
        if (model.EjectedTemp != null) emissionSource.EjectedTemp = (double)model.EjectedTemp;
        if (model.AvgExitSpeed != null) emissionSource.AvgExitSpeed = (double)model.AvgExitSpeed;
        if (model.HeightSource != null) emissionSource.HeightSource = (double)model.HeightSource;
        if (model.DiameterSource != null) emissionSource.DiameterSource = (double)model.DiameterSource;

        _dbContext.EmissionSources.Update(emissionSource);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<EmissionSourceViewModel>(emissionSource);

        return Ok(result);
    }
    
    /// <summary>
    /// Метод удаления источника выбросов
    /// </summary>
    /// <returns></returns>
    [HttpDelete("emission/source/{id}")]
    [ProducesResponseType(typeof(EmissionSourceViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteEmissionSource(int id)
    {
        var emissionSource = _dbContext.EmissionSources.FirstOrDefault(x => x.Id == id);

        if (emissionSource == null)
        {
            return Ok("Источник выброса не найден");
        }

        _dbContext.EmissionSources.Remove(emissionSource);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<EmissionSourceViewModel>(emissionSource);

        return Ok(result);
    }
}