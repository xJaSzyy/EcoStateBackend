using System.Text.Json.Serialization;

namespace EcoState.ViewModels.Weather;

public class WeatherResponse
{
    [JsonPropertyName("main")]
    public MainData MainData { get; set; }
    
    [JsonPropertyName("wind")]
    public WindData WindData { get; set; }
    
    [JsonPropertyName("weather")]
    public List<WeatherData> WeatherData { get; set; }
}

public class MainData
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }
}

public class WindData
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
    
    [JsonPropertyName("deg")]
    public int Deg { get; set; }
}

public class WeatherData
{
    [JsonPropertyName("icon")]
    public string Icon { get; set; }
}