using AutoMapper;
using EcoState.Context;
using EcoState.Domain;
using EcoState.Enums;
using EcoState.Interfaces;
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
    public async Task<IActionResult> UpdateEnterprise(EnterpriseUpdateModel model)
    {
        var enterprise = _dbContext.Enterprises.FirstOrDefault(x => x.Id == model.Id);

        if (enterprise == null)
        {
            return Ok("Предприятие не найдено");
        }

        if (model.Name != null) enterprise.Name = model.Name;
        if (model.City != null) enterprise.City = model.City;
        if (model.Lon != null) enterprise.Lon = (double)model.Lon;
        if (model.Lat != null) enterprise.Lat = (double)model.Lat;
        if (model.EjectedTemp != null) enterprise.EjectedTemp = (double)model.EjectedTemp;
        if (model.AvgExitSpeed != null) enterprise.AvgExitSpeed = (double)model.AvgExitSpeed;
        if (model.HeightSource != null) enterprise.HeightSource = (double)model.HeightSource;
        if (model.DiameterSource != null) enterprise.DiameterSource = (double)model.DiameterSource;
        if (model.TempStratificationRatio != null) enterprise.TempStratificationRatio = (CoefficientRegion)model.TempStratificationRatio;
        if (model.SedimentationRateRatio != null) enterprise.SedimentationRateRatio = (CoefficientDegreePurification)model.SedimentationRateRatio;

        _dbContext.Enterprises.Update(enterprise);
        await _dbContext.SaveChangesAsync();
        
        var result = _mapper.Map<EnterpriseViewModel>(enterprise);

        return Ok(result);
    }
}