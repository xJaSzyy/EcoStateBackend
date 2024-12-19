using EcoState.Enums;
using EcoState.Extensions;
using EcoState.Interfaces;
using EcoState.ViewModels.Concentration;

namespace EcoState.Services;

public class EmissionService : IEmissionService
{
    private double H; 
    private double F; 
    //private double u;
    private double D; 
    private double A; 
    private double w0; 
    private double Tgam; 
    private double Ta; 
    //private double x; 
    //private double y;

    private double deltaT;
    private double V1;
    private double vm;
    private double vm_s;
    private double f;
    private double f_e;

    private double h0;
    private double a0;
    private double ridge;
    private double hollow;
    private double ledge;
    private bool upperPlateau;
    private Landform landform;
    
    public void Setup(ConcentrationListCalculateModel model)
    {
        H = model.H;
        F = (int)model.F;
        //u = model.u;
        D = model.D;
        A = (int)model.A;
        w0 = model.w0;
        Tgam = model.Tgam;
        Ta = model.Ta;
        //x = model.x;
        //y = model.y;
        a0 = model.a0;
        h0 = model.h0;
        ridge = model.ridge;
        hollow = model.hollow;
        ledge = model.ledge;
        upperPlateau = model.upperPlateau;
        landform = model.landform;
        
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

        double nu = GetReliefCorrectionFactor();

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
    
    public ConcentrationListViewModel CalculateConcentrationList()
    {
        var maxDistance = 1025;

        var x = new List<double>();
        for (var i = 5; i <= maxDistance; i += 5)
        {
            x.Add(i);
        }

        ConcentrationMasses().TryGetValue("SO2", out var mSO2);
        ConcentrationMasses().TryGetValue("NO", out var mNO);
        ConcentrationMasses().TryGetValue("NO2", out var mNO2);
        ConcentrationMasses().TryGetValue("CO2", out var mCO2);
        ConcentrationMasses().TryGetValue("SP", out var mSP);
        
        var concentrationsSO2 = GetNormalSurfaceConcentration(x, mSO2); 
        var concentrationsNO = GetNormalSurfaceConcentration(x, mNO); 
        var concentrationsNO2 = GetNormalSurfaceConcentration(x, mNO2); 
        var concentrationsCO2 = GetNormalSurfaceConcentration(x, mCO2); 
        var concentrationsSP = GetNormalSurfaceConcentration(x, mSP); 
        
        var result = new ConcentrationListViewModel
        {
            ConcentrationsCO2 = concentrationsCO2,
            ConcentrationsNO = concentrationsNO,
            ConcentrationsNO2 = concentrationsNO2,
            ConcentrationsSP = concentrationsSP,
            ConcentrationsSO2 = concentrationsSO2
        };

        return result;
    }
    
    public ConcentrationViewModel CalculateConcentration(string concentration)
    {
        var maxDistance = 1025;

        var x = new List<double>();
        for (var i = 5; i <= maxDistance; i += 5)
        {
            x.Add(i);
        }

        ConcentrationMasses().TryGetValue(concentration, out var m);

        var concentrations = GetNormalSurfaceConcentration(x, m);

        var result = new ConcentrationViewModel
        {
            Concentrations = concentrations
        };

        return result;
    }
    
    public double GetReliefCorrectionFactor()
    {
        double fi1 = GetFi1();
        double nu_m = GetNuM();
        double nu = 1 + fi1 * (nu_m - 1);
        return nu;
    }
    
    public double GetFi1()
    {
        List<double> current = new List<double>() { ridge, hollow, ledge };

        if (GetTable1().TryGetValue(current, out var value))
        {
            if (upperPlateau)
            {
                return -value;
            }

            return value;
        }

        return -1;
    }

    private double GetNuM()
        {
            var array_ridge = new double[4, 5];
            array_ridge[0, 0] = 3.0f;
            array_ridge[0, 1] = 2.2f;
            array_ridge[0, 2] = 1.4f;
            array_ridge[0, 3] = 1.2f;
            array_ridge[0, 4] = 1.0f;
            array_ridge[1, 0] = 1.5f;
            array_ridge[1, 1] = 1.4f;
            array_ridge[1, 2] = 1.3f;
            array_ridge[1, 3] = 1.2f;
            array_ridge[1, 4] = 1.0f;
            array_ridge[2, 0] = 1.4f;
            array_ridge[2, 1] = 1.3f;
            array_ridge[2, 2] = 1.2f;
            array_ridge[2, 3] = 1.1f;
            array_ridge[2, 4] = 1.0f;
            array_ridge[3, 0] = 1.2f;
            array_ridge[3, 1] = 1.0f;
            array_ridge[3, 2] = 1.0f;
            array_ridge[3, 3] = 1.0f;
            array_ridge[3, 4] = 1.0f;

            var array_hollow = new double[4, 5];
            array_hollow[0, 0] = 4.0f;
            array_hollow[0, 1] = 3.0f;
            array_hollow[0, 2] = 1.8f;
            array_hollow[0, 3] = 1.4f;
            array_hollow[0, 4] = 1.0f;
            array_hollow[1, 0] = 2.0f;
            array_hollow[1, 1] = 1.6f;
            array_hollow[1, 2] = 1.5f;
            array_hollow[1, 3] = 1.3f;
            array_hollow[1, 4] = 1.0f;
            array_hollow[2, 0] = 1.6f;
            array_hollow[2, 1] = 1.5f;
            array_hollow[2, 2] = 1.4f;
            array_hollow[2, 3] = 1.2f;
            array_hollow[2, 4] = 1.0f;
            array_hollow[3, 0] = 1.3f;
            array_hollow[3, 1] = 1.2f;
            array_hollow[3, 2] = 1.1f;
            array_hollow[3, 3] = 1.0f;
            array_hollow[3, 4] = 1.0f;

            var array_ledge = new double[4, 5];
            array_ledge[0, 0] = 3.5f;
            array_ledge[0, 1] = 2.7f;
            array_ledge[0, 2] = 1.6f;
            array_ledge[0, 3] = 1.3f;
            array_ledge[0, 4] = 1.0f;
            array_ledge[1, 0] = 1.8f;
            array_ledge[1, 1] = 1.5f;
            array_ledge[1, 2] = 1.4f;
            array_ledge[1, 3] = 1.2f;
            array_ledge[1, 4] = 1.0f;
            array_ledge[2, 0] = 1.5f;
            array_ledge[2, 1] = 1.3f;
            array_ledge[2, 2] = 1.2f;
            array_ledge[2, 3] = 1.1f;
            array_ledge[2, 4] = 1.0f;
            array_ledge[3, 0] = 1.2f;
            array_ledge[3, 1] = 1.2f;
            array_ledge[3, 2] = 1.1f;
            array_ledge[3, 3] = 1.0f;
            array_ledge[3, 4] = 1.0f;

            double n1 = H / h0;
            double n2 = a0 / h0;

            int i, j = 0;
            if (n1 <= 0.55f)
            {
                j = 0;
            }
            else if (n1 < 1.05f)
            {
                j = 1;
            }
            else if (n1 <= 2.95f)
            {
                j = 2;
            }
            else if (n1 <= 5)
            {
                j = 3;
            }
            else
            {
                j = 4;
            }

            if (n2 > 4f && n2 <= 5.5f)
            {
                i = 0;
            }
            else if (n2 <= 9.5f)
            {
                i = 1;
            }
            else if (n2 <= 15.5f)
            {
                i = 2;
            }
            else
            {
                i = 3;
            }

            if (n2 > 20)
            {
                //MessageBox.Show("n2 больше 20");
            }

            if (landform == Landform.Ridge)
            {
                return array_ridge[i, j];
            }

            if (landform == Landform.Hollow)
            {
                return array_hollow[i, j];
            }

            return array_ledge[i, j];
        }

    private Dictionary<string, double> ConcentrationMasses()
    {
        return new Dictionary<string, double>()
        {
            { "SO2", 1.0528 },
            { "NO", 0.0444 },
            { "NO2", 0.2695 },
            { "CO2", 4.9 },
            { "SP", 15.72 },
        };
    }

    private Dictionary<List<double>, double> GetTable1()
    {
        return new Dictionary<List<double>, double>(new ListComparer())
        {
            { new List<double>() { 0.025, 0.000, 0.000 }, -4.00 },
            { new List<double>() { 0.050, 0.000, 0.000 }, -3.50 },
            { new List<double>() { 0.100, 0.000, 0.000 }, -3.00 },
            { new List<double>() { 0.150, 0.000, 0.000 }, -2.50 },
            { new List<double>() { 0.250, 0.000, 0.000 }, -2.00 },
            { new List<double>() { 0.300, 0.000, 0.000 }, -1.75 },
            { new List<double>() { 0.500, 0.000, 0.000 }, -1.50 },
            { new List<double>() { 0.800, 0.000, 0.000 }, -1.25 },
            { new List<double>() { 1.000, 0.000, 0.000 }, -1.00 },
            { new List<double>() { 0.800, 0.250, 0.000 }, -0.75 },
            { new List<double>() { 0.400, 0.600, 0.000 }, -0.50 },
            { new List<double>() { 0.100, 0.900, 0.000 }, -0.25 },
            { new List<double>() { 0.000, 1.000, 0.000 }, 0.00 },
            { new List<double>() { 0.100, 0.900, 0.100 }, 0.25 },
            { new List<double>() { 0.400, 0.600, 0.400 }, 0.50 },
            { new List<double>() { 0.800, 0.250, 0.800 }, 0.75 },
            { new List<double>() { 1.000, 0.000, 1.000 }, 1.00 },
            { new List<double>() { 0.800, 0.000, 0.800 }, 1.25 },
            { new List<double>() { 0.500, 0.000, 0.500 }, 1.50 },
            { new List<double>() { 0.300, 0.000, 0.350 }, 1.75 },
            { new List<double>() { 0.250, 0.000, 0.250 }, 2.00 },
            { new List<double>() { 0.150, 0.000, 0.150 }, 2.50 },
            { new List<double>() { 0.100, 0.000, 0.100 }, 3.00 },
            { new List<double>() { 0.050, 0.000, 0.075 }, 3.50 },
            { new List<double>() { 0.025, 0.000, 0.075 }, 4.00 }
        };
    }
}