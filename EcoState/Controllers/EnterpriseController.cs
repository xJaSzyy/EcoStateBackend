using System.Net;
using AutoMapper;
using EcoState.Context;
using EcoState.Domain;
using EcoState.Enums;
using EcoState.ViewModels.Enterprise;
using Microsoft.AspNetCore.Mvc;

namespace EcoState.Controllers;

/// <summary>
/// Контроллер предприятий
/// </summary>
public class EnterpriseController: ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="dbContext">ApplicationDbContext</param>
    /// <param name="mapper">IMapper</param>
    public EnterpriseController(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Метод добавления предприятия
    /// </summary>
    /// <param name="model">Модель добавления предприятия</param>
    /// <returns></returns>
    [HttpPost("enterprise")]
    [ProducesResponseType(typeof(EnterpriseViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> AddEnterprise(EnterpriseAddModel model)
    {
        var enterprise = _mapper.Map<Enterprise>(model);
        
        _dbContext.Enterprises.Add(enterprise);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<EnterpriseViewModel>(enterprise);
        
        return Ok(result);
    }
    
    /// <summary>
    /// Метод получения всех предприятий
    /// </summary>
    /// <returns></returns>
    [HttpGet("enterprise")]
    [ProducesResponseType(typeof(List<EnterpriseViewModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetAllEnterprises()
    {
        var enterprises = _dbContext.Enterprises.ToList();

        var result = _mapper.Map<List<EnterpriseViewModel>>(enterprises);

        return Ok(result);
    }
    
    /// <summary>
    /// Метод изменения предприятия
    /// </summary>
    /// <param name="model">Модель изменения предприятия</param>
    /// <returns></returns>
    [HttpPut("enterprise")]
    [ProducesResponseType(typeof(EnterpriseViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateEnterprise(EnterpriseUpdateModel model)
    {
        var enterprise = _dbContext.Enterprises.FirstOrDefault(x => x.Id == model.Id);

        if (enterprise == null)
        {
            return Ok("Предприятие не найдено");
        }

        if (model.Name != null) enterprise.Name = model.Name;
        if (model.City != null) enterprise.City = model.City;
        if (model.TempStratificationRatio != null) enterprise.TempStratificationRatio = (CoefficientRegion)model.TempStratificationRatio;
        if (model.SedimentationRateRatio != null) enterprise.SedimentationRateRatio = (CoefficientDegreePurification)model.SedimentationRateRatio;

        _dbContext.Enterprises.Update(enterprise);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<EnterpriseViewModel>(enterprise);

        return Ok(result);
    }
    
    /// <summary>
    /// Метод удаления предприятия
    /// </summary>
    /// <returns></returns>
    [HttpDelete("enterprise/{id}")]
    [ProducesResponseType(typeof(EnterpriseViewModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteEnterprise(int id)
    {
        var enterprise = _dbContext.Enterprises.FirstOrDefault(x => x.Id == id);

        if (enterprise == null)
        {
            return Ok("Предприятие не найдено");
        }

        _dbContext.Enterprises.Remove(enterprise);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<EnterpriseViewModel>(enterprise);

        return Ok(result);
    }
}