using EcoState.Domain;
using EcoState.Enums;
using EcoState.Interfaces;
using EcoState.Services;
using EcoState.ViewModels.Concentration;
using FluentAssertions;

namespace EcoState.Test;

public class EmissionServiceTest
{
    private readonly IEmissionService _service = new EmissionService();
    
    [Test]
    public void CalculateConcentrationList_ShouldCalculateCorrectly()
    {
        //Act
        _service.Setup(_calculateModel);
        var actual = _service.CalculateEmission();

        //Assert
        actual.Concentrations.Should().HaveCount(4);
        foreach (var concentration in actual.Concentrations)
        {
            concentration.Concentrations.Should().HaveCount(2000);
            concentration.AverageConcentration.Should().BeGreaterThan(0);
            concentration.DangerZoneAngle.Should().BeInRange(-6, 8);
            concentration.DangerZoneColorHex.Should().NotBeEmpty();
            concentration.DangerZoneColorHex.Should().Contain("#");
            concentration.DangerZoneLength.Should().BeGreaterThan(0);
            concentration.DangerZoneWidth.Should().BeGreaterThan(0);
            
            PDKValues.TryGetValue(concentration.Type, out var pdk);
            concentration.PDK.Should().Be(pdk);
        }
    }

    [Test]
    public void CalculateConcentration_WithConcentrationType_ShouldCalculateCorrectly()
    {
        //Act
        _service.Setup(_calculateModel);
        var actual = _service.CalculateConcentration(ConcentrationType.SP);

        //Assert
        actual.Concentrations.Should().HaveCount(2000);
        actual.AverageConcentration.Should().BeGreaterThan(0);
        actual.DangerZoneAngle.Should().Be(0);
        actual.DangerZoneColorHex.Should().NotBeEmpty();
        actual.DangerZoneColorHex.Should().Contain("#");
        actual.DangerZoneLength.Should().BeGreaterThan(0);
        actual.DangerZoneWidth.Should().BeGreaterThan(0);
        actual.PDK.Should().Be(0.5);
    }

    private readonly EmissionCalculateModel _calculateModel = new EmissionCalculateModel()
    {
        EjectedTemp = 235,
        AirTemp = 10,
        AvgExitSpeed = 15,
        HeightSource = 13,
        DiameterSource = 2,
        TempStratificationRatio = CoefficientRegion.NorthernPart,
        SedimentationRateRatio = CoefficientDegreePurification.High,
        WindSpeed = 10
    };
    
    private static readonly Dictionary<ConcentrationType, double> PDKValues = new()
    {
        { ConcentrationType.SO2, 0.5 },
        { ConcentrationType.NO, 0.4 },
        { ConcentrationType.NO2, 0.085 },
        { ConcentrationType.CO2, 5.0 },
        { ConcentrationType.SP, 0.5 }
    };
}