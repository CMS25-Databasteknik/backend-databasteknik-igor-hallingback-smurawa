using System.Text.Json.Serialization;

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

public sealed record Result(
    bool Success,
    [property: JsonIgnore]
    ErrorTypes? ErrorType = null,
    [property: JsonIgnore]
    string? ErrorMessage = null)
{
    public static Result Ok() => new(true);

    public static Result Validation(string message) => new(false, ErrorTypes.Validation, message);
    public static Result Unauthorized(string message = "Unauthorized.") => new(false, ErrorTypes.Unauthorized, message);
    public static Result Forbidden(string message = "Forbidden.") => new(false, ErrorTypes.Forbidden, message);
    public static Result NotFound(string message) => new(false, ErrorTypes.NotFound, message);
    public static Result Conflict(string message) => new(false, ErrorTypes.Conflict, message);
    public static Result Unprocessable(string message) => new(false, ErrorTypes.Unprocessable, message);
    public static Result Error(string message = "An unexpected error occurred.") => new(false, ErrorTypes.Unexpected, message);

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message => ErrorMessage;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Code => ErrorType switch
    {
        ErrorTypes.Validation => "validation_error",
        ErrorTypes.Unauthorized => "unauthorized",
        ErrorTypes.Forbidden => "forbidden",
        ErrorTypes.NotFound => "not_found",
        ErrorTypes.Conflict => "conflict",
        ErrorTypes.Unprocessable => "unprocessable",
        ErrorTypes.Unexpected => "unexpected_error",
        _ => null
    };
}

public sealed record Result<T>(
    bool Success,
    T? Value = default,
    [property: JsonIgnore]
    ErrorTypes? ErrorType = null,
    [property: JsonIgnore]
    string? ErrorMessage = null)
{
    public static Result<T> Ok(T value) => new(true, value);

    public static Result<T> Validation(string message) => new(false, default, ErrorTypes.Validation, message);
    public static Result<T> Unauthorized(string message = "Unauthorized.") => new(false, default, ErrorTypes.Unauthorized, message);
    public static Result<T> Forbidden(string message = "Forbidden.") => new(false, default, ErrorTypes.Forbidden, message);
    public static Result<T> NotFound(string message) => new(false, default, ErrorTypes.NotFound, message);
    public static Result<T> Conflict(string message) => new(false, default, ErrorTypes.Conflict, message);
    public static Result<T> Unprocessable(string message) => new(false, default, ErrorTypes.Unprocessable, message);
    public static Result<T> Error(string message = "An unexpected error occurred.") => new(false, default, ErrorTypes.Unexpected, message);

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message => ErrorMessage;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Code => ErrorType switch
    {
        ErrorTypes.Validation => "validation_error",
        ErrorTypes.Unauthorized => "unauthorized",
        ErrorTypes.Forbidden => "forbidden",
        ErrorTypes.NotFound => "not_found",
        ErrorTypes.Conflict => "conflict",
        ErrorTypes.Unprocessable => "unprocessable",
        ErrorTypes.Unexpected => "unexpected_error",
        _ => null
    };
}

public class ResultBase
{
    public bool Success { get; set; }
    [JsonIgnore]
    public ErrorTypes? ErrorType { get; set; }
    [JsonIgnore]
    public string? ErrorMessage { get; set; }

    [JsonIgnore]
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

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Code => ErrorType switch
    {
        ErrorTypes.Validation => "validation_error",
        ErrorTypes.Unauthorized => "unauthorized",
        ErrorTypes.Forbidden => "forbidden",
        ErrorTypes.NotFound => "not_found",
        ErrorTypes.Conflict => "conflict",
        ErrorTypes.Unprocessable => "unprocessable",
        ErrorTypes.Unexpected => "unexpected_error",
        _ => null
    };

    public Result ToResult() => new(Success, ErrorType, ErrorMessage);
}

public abstract class ResultCommon<T> : ResultBase
{
    public T? Result { get; set; }

    public new Result<T> ToResult() => new(Success, Result, ErrorType, ErrorMessage);
}

