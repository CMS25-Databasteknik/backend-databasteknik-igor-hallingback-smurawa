namespace Backend.Application.Common;

public enum ResultError
{
    None = 0,
    Validation = 1,
    Unauthorized = 2,
    Forbidden = 3,
    NotFound = 4,
    Conflict = 5,
    Unprocessable = 6,
    Unexpected = 7
}

public class ResultBase
{
    public bool Success { get; set; }
    public ErrorTypes? ErrorType { get; set; }
    public string? ErrorMessage { get; set; }

    public ResultError Error
    {
        get => ErrorType switch
        {
            null => ResultError.None,
            ErrorTypes.NotFound => ResultError.NotFound,
            ErrorTypes.Conflict => ResultError.Conflict,
            ErrorTypes.Validation => ResultError.Validation,
            ErrorTypes.Unauthorized => ResultError.Unauthorized,
            ErrorTypes.Forbidden => ResultError.Forbidden,
            ErrorTypes.Unprocessable => ResultError.Unprocessable,
            _ => ResultError.Unexpected
        };

        set => ErrorType = value switch
        {
            ResultError.None => null,
            ResultError.NotFound => ErrorTypes.NotFound,
            ResultError.Conflict => ErrorTypes.Conflict,
            ResultError.Validation => ErrorTypes.Validation,
            ResultError.Unauthorized => ErrorTypes.Unauthorized,
            ResultError.Forbidden => ErrorTypes.Forbidden,
            ResultError.Unprocessable => ErrorTypes.Unprocessable,
            _ => ErrorTypes.Unexpected
        };
    }

    public string? Message
    {
        get => ErrorMessage;
        set => ErrorMessage = value;
    }
}

public abstract class ResultCommon<T> : ResultBase
{
    public T? Result { get; set; }
}

