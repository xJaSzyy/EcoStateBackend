using AutoMapper;
using EcoState.Enums;
using EcoState.Interfaces;
using EcoState.Services;
using EcoState.ViewModels.Concentration;

namespace EcoState.Test;

public class EmissionServiceTest
{
    private readonly IEmissionService _service = new EmissionService();
    
    [Fact]
    public void CalculateConcentrationList_ShouldCalculateCorrectly()
    {
        //Arrange
        var expected = _concentrationListViewModel;

        //Act
        _service.Setup(_calculateModel);
        var actual = _service.CalculateConcentrationList();

        //Assert
        Assert.Equivalent(expected, actual);
    }
    
    [Fact]
    public void CalculateConcentration_WithConcentrationType_ShouldCalculateCorrectly()
    {
        //Arrange
        var expected = _concentrationViewModel;

        //Act
        _service.Setup(_calculateModel);
        var actual = _service.CalculateConcentration(_calculateModel.Concentration);

        //Assert
        Assert.Equivalent(expected, actual);
    }

    #region TestData

    private readonly ConcentrationCalculateModel _calculateModel = new ConcentrationCalculateModel()
    {
        Concentration = ConcentrationType.SP,
        Tgam = 235,
        Ta = 10,
        w0 = 15,
        H = 13,
        D = 2,
        A = CoefficientRegion.NorthernPart,
        F = CoefficientDegreePurification.High
    };
    
    private readonly ConcentrationListViewModel _concentrationListViewModel = new ConcentrationListViewModel()
    {
        ConcentrationsSO2 = new List<double>()
        {
            0.00003114428540011519,
            0.00012230999433428957,
            0.00027014928891727483,
            0.00047138484194916754,
            0.0007228098369154083
        },
        ConcentrationsNO = new List<double>()
        {
            0.000001313455805248019,
            0.000005158210247380754,
            0.000011393074114672305,
            0.000019879831860318236,
            0.000030483241602435533
        },
        ConcentrationsNO2 = new List<double>()
        {
            0.000007972440079151828,
            0.0000313094067943494,
            0.00006915390706991411,
            0.00012066699744044517,
            0.00018502778405081932
        },
        ConcentrationsCO2 = new List<double>()
        {
            0.00014495345598457873,
            0.0005692619417154437,
            0.0012573437649075296,
            0.002193945408008094,
            0.0033641415281967153
        },
        ConcentrationsSP = new List<double>()
        {
            0.00046503435266889324,
            0.001826285249748321,
            0.0040337640784380326,
            0.007038535064058618,
            0.0107927152700515
        }
    };
    
    private readonly ConcentrationViewModel _concentrationViewModel = new ConcentrationViewModel()
    {
        Concentrations = new List<double>()
        {
            0.00046503435266889324,
            0.001826285249748321,
            0.0040337640784380326,
            0.007038535064058618,
            0.0107927152700515
        }
    };

    #endregion
}