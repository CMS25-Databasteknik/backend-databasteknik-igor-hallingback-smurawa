namespace Backend.Application.Common;

public enum ErrorTypes
{
    None,
    Validation,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    Unprocessable,
    Unexpected
}

public record ResultBase(
    bool Success = false,
    ErrorTypes ErrorType = ErrorTypes.None,
    string? ErrorMessage = null,
    string? Message = null)
{
    public static ResultBase Ok(string? message = null) => new(true, ErrorTypes.None, null, message);

    public static ResultBase BadRequest(string message) => new(false, ErrorTypes.Validation, message, message);
    public static ResultBase Unauthorized(string message) => new(false, ErrorTypes.Unauthorized, message, message);
    public static ResultBase Forbidden(string message) => new(false, ErrorTypes.Forbidden, message, message);
    public static ResultBase NotFound(string message) => new(false, ErrorTypes.NotFound, message, message);
    public static ResultBase Conflict(string message) => new(false, ErrorTypes.Conflict, message, message);
    public static ResultBase Unprocessable(string message) => new(false, ErrorTypes.Unprocessable, message, message);
    public static ResultBase Unexpected(string message = "An unexpected error occurred.") => new(false, ErrorTypes.Unexpected, message, message);
}

public record ResultBase<T>(
    bool Success = false,
    T? Result = default,
    ErrorTypes ErrorType = ErrorTypes.None,
    string? ErrorMessage = null,
    string? Message = null)
    : ResultBase(Success, ErrorType, ErrorMessage, Message)
{
    public static ResultBase<T> Ok(T value, string? message = null) => new(true, value, ErrorTypes.None, null, message);

    public new static ResultBase<T> BadRequest(string message) => new(false, default, ErrorTypes.Validation, message, message);
    public new static ResultBase<T> Unauthorized(string message) => new(false, default, ErrorTypes.Unauthorized, message, message);
    public new static ResultBase<T> Forbidden(string message) => new(false, default, ErrorTypes.Forbidden, message, message);
    public new static ResultBase<T> NotFound(string message) => new(false, default, ErrorTypes.NotFound, message, message);
    public new static ResultBase<T> Conflict(string message) => new(false, default, ErrorTypes.Conflict, message, message);
    public new static ResultBase<T> Unprocessable(string message) => new(false, default, ErrorTypes.Unprocessable, message, message);
    public new static ResultBase<T> Unexpected(string message = "An unexpected error occurred.") => new(false, default, ErrorTypes.Unexpected, message, message);
}

