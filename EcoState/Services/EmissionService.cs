using EcoState.Domain;
using EcoState.Enums;
using EcoState.Interfaces;
using EcoState.ViewModels.Concentration;

namespace EcoState.Services;

/// <summary>
/// Сервис выбросов
/// </summary>
public class EmissionService : IEmissionService
{
    private const double WindAverageSpeed = 3;

    private double _heightSource; 
    private double _sedimentationRateRatio; 
    private double _diameterSource; 
    private double _tempStratificationRatio; 
    private double _avgExitSpeed; 
    private double _ejectedTemp; 
    private double _airTemp; 
    private double _windSpeed;

    private double deltaT;
    private double V1;
    private double vm;
    private double vm_s;
    private double f;
    private double f_e;

    /// <inheritdoc />
    public void Setup(EmissionCalculateModel model)
    {
        _heightSource = model.H;
        _sedimentationRateRatio = (int)model.F;
        _diameterSource = model.D;
        _tempStratificationRatio = (int)model.A;
        _avgExitSpeed = model.w0;
        _ejectedTemp = model.Tgam;
        _airTemp = model.Ta;
        _windSpeed = model.WindSpeed;
        
        deltaT = _ejectedTemp - _airTemp;
        V1 = (Math.PI * Math.Pow(_diameterSource, 2) / 4 * _avgExitSpeed); 
        vm = 0.65 * Math.Pow(V1 * deltaT / _heightSource, 1.0 / 3.0);
        vm_s = 1.3 * _avgExitSpeed * _diameterSource / _heightSource;
        f = 1000 * (Math.Pow(_avgExitSpeed, 2) * _diameterSource) / (Math.Pow(_heightSource, 2) * deltaT);
        f_e = 800 * Math.Pow(vm_s, 3);
    }

    private List<double> GetNormalSurfaceConcentration(List<double> x, double m)
    {
        List<double> c = new List<double>();

        double s1 = 0;
        var c_m = GetMaximumSingleSurfaceConcentration(_heightSource, m, V1, _diameterSource);

        var x_m = GetDistanceFromEmissionSourceSingle();

        foreach (var t in x)
        {
            var x_div = t / x_m;

            switch (x_div)
            {
                case <= 1:
                    s1 = 3 * Math.Pow(x_div, 4) - 8 * Math.Pow(x_div, 3) + 6 * Math.Pow(x_div, 2);
                    break;
                case <= 8:
                    s1 = 1.13f / (0.13f * Math.Pow(x_div, 2) + 1);
                    break;
                case <= 100 when _sedimentationRateRatio <= 1.5f:
                    s1 = x_div / (3.556f * Math.Pow(x_div, 2) - 35.2f * x_div + 120);
                    break;
                case <= 100:
                    s1 = 1 / (0.1f * Math.Pow(x_div, 2) + 2.456f * x_div - 17.8f);
                    break;
                case > 100 when _sedimentationRateRatio <= 1.5f:
                    s1 = 144.3f * Math.Pow(x_div, -7.0 / 3.0);
                    break;
                case > 100:
                    s1 = 37.76f * Math.Pow(x_div, -7.0 / 3.0);
                    break;
            }

            if (_heightSource <= 10 && x_div < 1)
            {
                var s1_h = 0.125f * (10 - _heightSource) + 0.125f * (_heightSource - 2) * s1;
                c.Add(s1_h * c_m);
                return c;
            }

            c.Add(s1 * c_m);
        }

        return c;
    }

    private double GetMaximumSingleSurfaceConcentration(double H, double M, double V1, double D)
    {
        double c_m = 0;

        const double nu = 1; //GetReliefCorrectionFactor();

        double m = 0;
        double n = 0;
        if (f < 100)
        {
            m = 1 / (0.67f + 0.1f * Math.Sqrt(f) + 0.34f * Math.Pow(f, 1.0 / 3.0));
            
            if (vm < 2)
            {
                n = 0.532f * Math.Pow(vm, 2) - 2.13f * vm + 3.13f;
            }
            else if (vm < 0.5f)
            {
                n = 4.4 * vm;
                var m_s = 2.86 * m;
                c_m = _tempStratificationRatio * M * _sedimentationRateRatio * m_s * nu / Math.Pow(H, 7.0 / 3.0);
                return c_m;
            }
            else
            {
                n = 1;
            }
        }
        else if (f >= 100 || (deltaT >= 0 && deltaT < 0.5f))
        {
            if (f >= 100)
            {
                m = 1.47f / Math.Pow(f, 1.0 / 3.0);
            }

            if (vm_s >= 0.5f)
            {
                var K = D / 8 * V1;
                K = 1 / 7.1f * Math.Sqrt(_avgExitSpeed * V1);
                c_m = _tempStratificationRatio * M * _sedimentationRateRatio * n * nu * K / Math.Pow(H, 4.0 / 3.0);
                return c_m;
            }
            else
            {
                double m_s = 0.9f;
                c_m = _tempStratificationRatio * M * _sedimentationRateRatio * m_s * nu / Math.Pow(H, 7.0 / 3.0);
                return c_m;
            }
        }


        c_m = (_tempStratificationRatio * M * _sedimentationRateRatio * m * n * nu / (Math.Pow(H, 2) * Math.Pow(V1 * deltaT, 1.0 / 3.0)));
        return c_m;
    }

    private double GetDistanceFromEmissionSourceSingle()
    {
        double x_m = 0;

        double d = 0;
        if (f < 100)
        {
            if (vm <= 0.5f)
            {
                d = 2.48f * (1 + 0.28f * Math.Pow(f_e, 1.0 / 3.0));
            }
            else if (vm <= 2)
            {
                d = 4.95f * vm * (1 + 0.28f * Math.Pow(f, 1.0 / 3.0));
            }
            else
            {
                d = 7 * Math.Sqrt(vm) * (1 + 0.28f * Math.Pow(f, 1.0 / 3.0));
            }
        }
        else if (f >= 100 || (deltaT >= 0 && deltaT < 0.5f))
        {
            if (vm_s <= 0.5f)
            {
                d = 5.7f;
            }
            else if (vm_s <= 2)
            {
                d = 11.4f * vm_s;
            }
            else
            {
                d = 16 * Math.Sqrt(vm_s);
            }
        }

        if (vm_s >= 0 && vm_s < 0.5f && deltaT >= -0.5f && deltaT <= 0)
        {
            x_m = 5.7f * _heightSource;
            return x_m;
        }

        x_m = ((5 - _sedimentationRateRatio) / 4) * d * _heightSource;
        return x_m;
    }

    /// <inheritdoc />
    public EmissionViewModel CalculateEmission()
    {
        const int distance = 10000;

        var x = new List<double>();
        for (var i = 5; i <= distance; i += 5)
        {
            x.Add(i);
        }

        ConcentrationMasses().TryGetValue(1, out var massSo2);
        ConcentrationMasses().TryGetValue(2, out var massNo);
        ConcentrationMasses().TryGetValue(3, out var massNo2);
        ConcentrationMasses().TryGetValue(4, out var massCo2);
        ConcentrationMasses().TryGetValue(5, out var massSp);
        
        var concentrationsSo2 = GetNormalSurfaceConcentration(x, massSo2); 
        var concentrationsNo = GetNormalSurfaceConcentration(x, massNo); 
        var concentrationsNo2 = GetNormalSurfaceConcentration(x, massNo2); 
        var concentrationsCo2 = GetNormalSurfaceConcentration(x, massCo2); 
        var concentrationsSp = GetNormalSurfaceConcentration(x, massSp);
        
        var dangerZoneParametersSo2 = CalculateDangerZoneParameters(concentrationsSo2);
        var dangerZoneParametersNo = CalculateDangerZoneParameters(concentrationsNo);
        var dangerZoneParametersNo2 = CalculateDangerZoneParameters(concentrationsNo2);
        var dangerZoneParametersCo2 = CalculateDangerZoneParameters(concentrationsCo2);
        var dangerZoneParametersSp = CalculateDangerZoneParameters(concentrationsSp);

        var result = new EmissionViewModel
        {
            Concentrations = new List<Concentration>()
            {
                new Concentration()
                {
                    Type = ConcentrationType.SO2, Concentrations = concentrationsSo2, 
                    DangerZoneWidth = dangerZoneParametersSo2.DangerZoneWidth, 
                    DangerZoneLength = dangerZoneParametersSo2.DangerZoneLength
                },
                new Concentration()
                {
                    Type = ConcentrationType.NO, Concentrations = concentrationsNo, 
                    DangerZoneWidth = dangerZoneParametersNo.DangerZoneWidth, 
                    DangerZoneLength = dangerZoneParametersNo.DangerZoneLength
                },
                new Concentration()
                { 
                        Type = ConcentrationType.NO2, Concentrations = concentrationsNo2, 
                        DangerZoneWidth = dangerZoneParametersNo2.DangerZoneWidth, 
                        DangerZoneLength = dangerZoneParametersNo2.DangerZoneLength
                },
                new Concentration()
                {
                    Type = ConcentrationType.CO2, Concentrations = concentrationsCo2, 
                    DangerZoneWidth = dangerZoneParametersCo2.DangerZoneWidth, 
                    DangerZoneLength = dangerZoneParametersCo2.DangerZoneLength
                },
                new Concentration()
                {
                    Type = ConcentrationType.SP, Concentrations = concentrationsSp, 
                    DangerZoneWidth = dangerZoneParametersSp.DangerZoneWidth, 
                    DangerZoneLength = dangerZoneParametersSp.DangerZoneLength
                }
            }
        };

        return result;
    }

    /// <inheritdoc />
    public ConcentrationViewModel CalculateConcentration(ConcentrationType type)
    {
        const int distance = 10000;

        var x = new List<double>();
        for (var i = 5; i <= distance; i += 5)
        {
            x.Add(i);
        }

        ConcentrationMasses().TryGetValue((int)type, out var m);

        var concentrations = GetNormalSurfaceConcentration(x, m);

        var dangerZoneParameters = CalculateDangerZoneParameters(concentrations);
        
        var result = new ConcentrationViewModel()
        {
            Type = type,
            DangerZoneLength = dangerZoneParameters.DangerZoneLength,
            DangerZoneWidth = dangerZoneParameters.DangerZoneWidth,
            Concentrations = concentrations
        };

        return result;
    }

    private DangerZoneParameters CalculateDangerZoneParameters(List<double> concentrations)
    {
        var maxIndex = int.MinValue;
        var maxDistance = double.MinValue;
        var minDistance = double.MinValue;

        var valuesUpMax = new List<double>();
        
        var maxConcentration = concentrations.Max();
        for (var i = 0; i < concentrations.Count; i++)
        {
            valuesUpMax.Add(concentrations[i]);

            if (maxConcentration == concentrations[i])
            {
                maxIndex = i;
                maxDistance = (i + 1) * 5;
                break;
            }
        }

        double med;

        valuesUpMax.Sort();
        
        if (valuesUpMax.Count % 2 == 0)
        {
            med = (valuesUpMax[(valuesUpMax.Count / 2)] + valuesUpMax[(valuesUpMax.Count / 2) - 1]) / 2;
        }
        else
        {
            med = valuesUpMax[(valuesUpMax.Count / 2)];
        }

        for (var i = maxIndex; i < concentrations.Count; i++)
        {
            if (concentrations[i] < med)
            {
                minDistance = (i + 1) * 5;
                break;
            }
        }
        
        var windSpeedCoeff =  WindAverageSpeed / _windSpeed; // коэффицент скорости ветра, влияющий на ширину выброса
        
        var dangerZoneLength = minDistance;
        var dangerZoneWidth = Math.Round((minDistance - maxDistance) * 2 * windSpeedCoeff, 2);
        
        return new DangerZoneParameters()
        {
            DangerZoneLength = dangerZoneLength,
            DangerZoneWidth = dangerZoneWidth
        };
    }

    private static Dictionary<int, double> ConcentrationMasses()
    {
        return new Dictionary<int, double>()
        {
            { 1, 1.0528 },
            { 2, 0.0444 },
            { 3, 0.2695 },
            { 4, 4.9 },
            { 5, 15.72 },
        };
    }
}

/// <summary>
/// Параметры зоны выброса
/// </summary>
public class DangerZoneParameters
{
    /// <summary>
    /// Длина зоны выброса
    /// </summary>
    public double DangerZoneLength { get; set; }
    
    /// <summary>
    /// Ширина зоны выброса
    /// </summary>
    public double DangerZoneWidth { get; set; }
}