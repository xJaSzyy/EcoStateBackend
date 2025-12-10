using System.Text.Json.Serialization;

namespace EcoState.ViewModels.Weather;

/// <summary>
/// Ответ от Open-Meteo API
/// </summary>
public class OpenMeteoResponse
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; } = null!;

    [JsonPropertyName("timezone_abbreviation")]
    public string TimezoneAbbreviation { get; set; } = null!;

    [JsonPropertyName("elevation")]
    public double Elevation { get; set; }

    [JsonPropertyName("current_weather")]
    public CurrentWeather CurrentWeather { get; set; } = null!;

    [JsonPropertyName("current_weather_units")]
    public CurrentWeatherUnits CurrentWeatherUnits { get; set; } = null!;
}

public class CurrentWeather
{
    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("windspeed")]
    public double WindSpeed { get; set; }

    [JsonPropertyName("winddirection")]
    public double WindDirection { get; set; }

    [JsonPropertyName("weathercode")]
    public int WeatherCode { get; set; }

    [JsonPropertyName("is_day")]
    public int IsDay { get; set; }
}

public class CurrentWeatherUnits
{
    [JsonPropertyName("temperature")]
    public string TemperatureUnit { get; set; } = null!;

    [JsonPropertyName("windspeed")]
    public string WindSpeedUnit { get; set; } = null!;
}