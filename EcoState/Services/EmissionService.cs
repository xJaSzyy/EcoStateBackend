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
    private double H; 
    private double F; 
    private double D; 
    private double A; 
    private double w0; 
    private double Tgam; 
    private double Ta; 

    private double deltaT;
    private double V1;
    private double vm;
    private double vm_s;
    private double f;
    private double f_e;
    
    public void Setup(EmissionCalculateModel model)
    {
        H = model.H;
        F = (int)model.F;
        D = model.D;
        A = (int)model.A;
        w0 = model.w0;
        Tgam = model.Tgam;
        Ta = model.Ta;
        
        deltaT = Tgam - Ta;
        V1 = (Math.PI * Math.Pow(D, 2) / 4 * w0); 
        vm = 0.65 * Math.Pow(V1 * deltaT / H, 1.0 / 3.0);
        vm_s = 1.3 * w0 * D / H;
        f = 1000 * (Math.Pow(w0, 2) * D) / (Math.Pow(H, 2) * deltaT);
        f_e = 800 * Math.Pow(vm_s, 3);
    }

    public List<double> GetNormalSurfaceConcentration(List<double> x, double M)
    {
        List<double> c = new List<double>();

        double s1 = 0;
        double c_m = GetMaximumSingleSurfaceConcentration(H, M, V1, D);

        double x_m = GetDistanceFromEmissionSourceSingle();

        foreach (var t in x)
        {
            double x_div = t / x_m;

            switch (x_div)
            {
                case <= 1:
                    s1 = 3 * Math.Pow(x_div, 4) - 8 * Math.Pow(x_div, 3) + 6 * Math.Pow(x_div, 2);
                    break;
                case <= 8:
                    s1 = 1.13f / (0.13f * Math.Pow(x_div, 2) + 1);
                    break;
                case <= 100 when F <= 1.5f:
                    s1 = x_div / (3.556f * Math.Pow(x_div, 2) - 35.2f * x_div + 120);
                    break;
                case <= 100:
                    s1 = 1 / (0.1f * Math.Pow(x_div, 2) + 2.456f * x_div - 17.8f);
                    break;
                case > 100 when F <= 1.5f:
                    s1 = 144.3f * Math.Pow(x_div, -7.0 / 3.0);
                    break;
                case > 100:
                    s1 = 37.76f * Math.Pow(x_div, -7.0 / 3.0);
                    break;
            }

            if (H <= 10 && x_div < 1)
            {
                double s1_h = 0.125f * (10 - H) + 0.125f * (H - 2) * s1;
                c.Add(s1_h * c_m);
                return c;
            }

            c.Add(s1 * c_m);
        }

        return c;
    }

    public double GetMaximumSingleSurfaceConcentration(double H, double M, double V1, double D)
    {
        double c_m = 0;

        double nu = 1;//GetReliefCorrectionFactor();

        double m = 0;
        double n = 0;
        if (f < 100)
        {
            m = 1 / (0.67f + 0.1f * Math.Sqrt(f) + 0.34f * Math.Pow(f, 1.0 / 3.0));
            if (vm < 0.5f)
            {
                n = 4.4 * vm;
                double m_s = 2.86 * m;
                c_m = A * M * F * m_s * nu / Math.Pow(H, 7.0 / 3.0);
                return c_m;
            }
            else if (vm < 2)
            {
                n = 0.532f * Math.Pow(vm, 2) - 2.13f * vm + 3.13f;
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
                double K = D / 8 * V1;
                K = 1 / 7.1f * Math.Sqrt(w0 * V1);
                c_m = A * M * F * n * nu * K / Math.Pow(H, 4.0 / 3.0);
                return c_m;
            }
            else
            {
                double m_s = 0.9f;
                c_m = A * M * F * m_s * nu / Math.Pow(H, 7.0 / 3.0);
                return c_m;
            }
        }


        c_m = (A * M * F * m * n * nu / (Math.Pow(H, 2) * Math.Pow(V1 * deltaT, 1.0 / 3.0)));
        return c_m;
    }

    public double GetDistanceFromEmissionSourceSingle()
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
            x_m = 5.7f * H;
            return x_m;
        }

        x_m = ((5 - F) / 4) * d * H;
        return x_m;
    }
    
    public EmissionViewModel CalculateEmission()
    {
        var maxDistance = 1025;

        var x = new List<double>();
        for (var i = 5; i <= maxDistance; i += 5)
        {
            x.Add(i);
        }

        ConcentrationMasses().TryGetValue(1, out var mSO2);
        ConcentrationMasses().TryGetValue(2, out var mNO);
        ConcentrationMasses().TryGetValue(3, out var mNO2);
        ConcentrationMasses().TryGetValue(4, out var mCO2);
        ConcentrationMasses().TryGetValue(5, out var mSP);
        
        var concentrationsSO2 = GetNormalSurfaceConcentration(x, mSO2); 
        var concentrationsNO = GetNormalSurfaceConcentration(x, mNO); 
        var concentrationsNO2 = GetNormalSurfaceConcentration(x, mNO2); 
        var concentrationsCO2 = GetNormalSurfaceConcentration(x, mCO2); 
        var concentrationsSP = GetNormalSurfaceConcentration(x, mSP);

        var result = new EmissionViewModel
        {
            Concentrations = new List<Concentration>()
            {
                new Concentration()
                    { Type = ConcentrationType.SO2, Concentrations = concentrationsSO2 },
                new Concentration()
                    { Type = ConcentrationType.NO, Concentrations = concentrationsNO },
                new Concentration()
                    { Type = ConcentrationType.NO2, Concentrations = concentrationsNO2 },
                new Concentration()
                    { Type = ConcentrationType.CO2, Concentrations = concentrationsCO2 },
                new Concentration()
                    { Type = ConcentrationType.SP, Concentrations = concentrationsSP }
            }
        };

        return result;
    }
    
    public ConcentrationViewModel CalculateConcentration(ConcentrationType type)
    {
        var maxDistance = 10000;

        var x = new List<double>();
        for (var i = 5; i <= maxDistance; i += 5)
        {
            x.Add(i);
        }

        ConcentrationMasses().TryGetValue((int)type, out var m);

        var concentrations = GetNormalSurfaceConcentration(x, m);
        
        var result = new ConcentrationViewModel()
        {
            Type = type,
            Concentrations = concentrations
        };

        return result;
    }

    private Dictionary<int, double> ConcentrationMasses()
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