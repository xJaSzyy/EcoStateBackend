using EcoState.Enums;

namespace EcoState.ViewModels.Concentration;

/// <summary>
/// Модель получения концентрации по её типу
/// </summary>
public class ConcentrationGetByTypeModel
{
    /// <summary>
    /// Вид частиц
    /// </summary>
    public ConcentrationType Type { get; set; }
}