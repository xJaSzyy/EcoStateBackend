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
        CreateMap<ConcentrationCalculateModel, ConcentrationListCalculateModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<Concentration, ConcentrationSaveModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<Concentration, ConcentrationViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<ConcentrationList, ConcentrationListSaveModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<ConcentrationList, ConcentrationListViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<Weather, WeatherViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<Weather, WeatherSaveModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<User, UserViewModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
        
        CreateMap<User, UserAddModel>()
            .ForAllMembers(o => o.ExplicitExpansion());
    }
}