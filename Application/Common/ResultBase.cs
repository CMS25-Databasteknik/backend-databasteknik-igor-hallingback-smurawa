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

public record ResultBase(bool Success = false, ErrorTypes ErrorType = ErrorTypes.None, string? Message = null)
{
    public static ResultBase Ok(string? message = null) => new(true, ErrorTypes.None, message);
    public static ResultBase BadRequest(string message) => new(false, ErrorTypes.Validation, message);
    public static ResultBase Unauthorized(string message) => new(false, ErrorTypes.Unauthorized, message);
    public static ResultBase Forbidden(string message) => new(false, ErrorTypes.Forbidden, message);
    public static ResultBase NotFound(string message) => new(false, ErrorTypes.NotFound, message);
    public static ResultBase Conflict(string message) => new(false, ErrorTypes.Conflict, message);
    public static ResultBase Unprocessable(string message) => new(false, ErrorTypes.Unprocessable, message);
    public static ResultBase Unexpected(string message = "An unexpected error occurred.") => new(false, ErrorTypes.Unexpected, message);
}

public record ResultBase<T>(bool Success = false, T? Result = default, ErrorTypes ErrorType = ErrorTypes.None, string? Message = null)
    : ResultBase(Success, ErrorType, Message)
{
    public static ResultBase<T> Ok(T value, string? message = null) => new(true, value, ErrorTypes.None, message);
    public new static ResultBase<T> BadRequest(string message) => new(false, default, ErrorTypes.Validation, message);
    public new static ResultBase<T> Unauthorized(string message) => new(false, default, ErrorTypes.Unauthorized, message);
    public new static ResultBase<T> Forbidden(string message) => new(false, default, ErrorTypes.Forbidden, message);
    public new static ResultBase<T> NotFound(string message) => new(false, default, ErrorTypes.NotFound, message);
    public new static ResultBase<T> Conflict(string message) => new(false, default, ErrorTypes.Conflict, message);
    public new static ResultBase<T> Unprocessable(string message) => new(false, default, ErrorTypes.Unprocessable, message);
    public new static ResultBase<T> Unexpected(string message = "An unexpected error occurred.") => new(false, default, ErrorTypes.Unexpected, message);
}

