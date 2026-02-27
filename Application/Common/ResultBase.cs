namespace Backend.Application.Common;

public record Result(
    bool Success,
    ErrorTypes? ErrorType = null,
    string? ErrorMessage = null,
    string? Message = null)
{
    public static Result Ok() => new(true);

    public static Result BadRequest(string message) => new(false, ErrorTypes.Validation, message, message);
    public static Result Validation(string message) => new(false, ErrorTypes.Validation, message, message);
    public static Result Unauthorized(string message) => new(false, ErrorTypes.Unauthorized, message, message);
    public static Result Forbidden(string message) => new(false, ErrorTypes.Forbidden, message, message);
    public static Result NotFound(string message) => new(false, ErrorTypes.NotFound, message, message);
    public static Result Conflict(string message) => new(false, ErrorTypes.Conflict, message, message);
    public static Result Unprocessable(string message) => new(false, ErrorTypes.Unprocessable, message, message);
    public static Result Error(string message = "An unexpected error occurred.") => new(false, ErrorTypes.Unexpected, message, message);
}

public record Result<T>(
    bool Success,
    T? Value = default,
    ErrorTypes? ErrorType = null,
    string? ErrorMessage = null,
    string? Message = null) : Result(Success, ErrorType, ErrorMessage, Message)
{
    public static Result<T> Ok(T value) => new(true, value);

    public new static Result<T> BadRequest(string message) => new(false, default, ErrorTypes.Validation, message, message);
    public new static Result<T> Validation(string message) => new(false, default, ErrorTypes.Validation, message, message);
    public new static Result<T> Unauthorized(string message) => new(false, default, ErrorTypes.Unauthorized, message, message);
    public new static Result<T> Forbidden(string message) => new(false, default, ErrorTypes.Forbidden, message, message);
    public new static Result<T> NotFound(string message) => new(false, default, ErrorTypes.NotFound, message, message);
    public new static Result<T> Conflict(string message) => new(false, default, ErrorTypes.Conflict, message, message);
    public new static Result<T> Unprocessable(string message) => new(false, default, ErrorTypes.Unprocessable, message, message);
    public new static Result<T> Error(string message = "An unexpected error occurred.") => new(false, default, ErrorTypes.Unexpected, message, message);
}

// Backwards-compatible aliases to ease transition
public record ResultBase(
    bool Success = false,
    ErrorTypes? ErrorType = null,
    string? ErrorMessage = null,
    string? Message = null) : Result(Success, ErrorType, ErrorMessage, Message);

public record ResultCommon<T>(
    bool Success = false,
    T? Result = default,
    ErrorTypes? ErrorType = null,
    string? ErrorMessage = null,
    string? Message = null)
    : Result<T>(Success, Result, ErrorType, ErrorMessage, Message);
