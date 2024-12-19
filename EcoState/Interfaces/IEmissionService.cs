using EcoState.Enums;
using EcoState.ViewModels.Concentration;

namespace EcoState.Interfaces;

public interface IEmissionService
{
    public void Setup(ConcentrationListCalculateModel model);
    
    public ConcentrationListViewModel CalculateConcentrationList();

    public ConcentrationViewModel CalculateConcentration(string concentration);
}