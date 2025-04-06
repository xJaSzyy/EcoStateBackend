using AutoMapper;
using EcoState.Domain;
using EcoState.ViewModels.Concentration;
using EcoState.ViewModels.EmissionSource;
using EcoState.ViewModels.Enterprise;
using EcoState.ViewModels.User;
using EcoState.ViewModels.Weather;

namespace EcoState.Helpers;

public class EntityMapper : Profile
{
    public EntityMapper()
    {
        CreateMap<ConcentrationSaveModel, Concentration>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<Concentration, ConcentrationViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<Emission, EmissionViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<Weather, WeatherViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<WeatherResponse, WeatherViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<WeatherSaveModel, Weather>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<User, UserViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<EnterpriseAddModel, Enterprise>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<Enterprise, EnterpriseViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<EmissionSourceAddModel, EmissionSource>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<EmissionSource, EmissionSourceViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
    }
}