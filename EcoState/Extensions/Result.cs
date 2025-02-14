using System.Text.Json.Serialization;

namespace EcoState.Extensions;

[Serializable]
public class Result
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ErrorMessage { get; set; } = null!;

    public int ReturnCode { get; set; }
    
    public bool IsSuccess => this.ErrorMessage == null;
}

[Serializable]
public class Result<T> : Result
{
    /// <summary>Конструктор по умолчанию.</summary>
    public Result()
    {
    }

    /// <summary>Конструктор результата с содержанием.</summary>
    /// <param name="content">Объекта в содержании.</param>
    public Result(T content) => this.Content = content;

    /// <summary>Содержание.</summary>
    public T Content { get; set; } = default!;
}