namespace Backend.Application.Common;

public class ResultBase
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
}

public abstract class ResultCommon<T> : ResultBase
{
    public T? Result { get; set; }
}