using AutoMapper;
using EcoState.Domain;
using EcoState.ViewModels.Concentration;
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
        
        CreateMap<ConcentrationListSaveModel, ConcentrationList>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<ConcentrationList, ConcentrationListViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<Weather, WeatherViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<WeatherResponse, WeatherViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<WeatherSaveModel, Weather>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<User, UserViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
    }
}