using System.ComponentModel.DataAnnotations;

namespace EcoState.Domain;

public class Weather
{
    [Key] 
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public float Temperature { get; set; }
    
    public int WindDirection { get; set; }
    
    public float WindSpeed { get; set; }
}