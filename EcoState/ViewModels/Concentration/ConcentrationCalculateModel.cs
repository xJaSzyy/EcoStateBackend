using System.ComponentModel.DataAnnotations;
using EcoState.Enums;

namespace EcoState.ViewModels.Concentration;

public class ConcentrationCalculateModel : ConcentrationListCalculateModel
{
    /// <summary>
    /// Вид частиц
    /// </summary>
    public ConcentrationType Concentration { get; set; }
}