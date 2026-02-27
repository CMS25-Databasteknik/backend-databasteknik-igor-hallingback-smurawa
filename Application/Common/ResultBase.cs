namespace Backend.Application.Common;

public enum ResultError
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
    ResultError Error = ResultError.None,
    ErrorTypes? ErrorType = null,
    string? ErrorMessage = null,
    string? Message = null)
{
    public static ResultBase Ok(string? message = null) => new(true, ResultError.None, null, null, message);

    public static ResultBase BadRequest(string message) => new(false, ResultError.Validation, ErrorTypes.Validation, message, message);
    public static ResultBase Unauthorized(string message) => new(false, ResultError.Unauthorized, ErrorTypes.Unauthorized, message, message);
    public static ResultBase Forbidden(string message) => new(false, ResultError.Forbidden, ErrorTypes.Forbidden, message, message);
    public static ResultBase NotFound(string message) => new(false, ResultError.NotFound, ErrorTypes.NotFound, message, message);
    public static ResultBase Conflict(string message) => new(false, ResultError.Conflict, ErrorTypes.Conflict, message, message);
    public static ResultBase Unprocessable(string message) => new(false, ResultError.Unprocessable, ErrorTypes.Unprocessable, message, message);
    public static ResultBase Unexpected(string message = "An unexpected error occurred.") => new(false, ResultError.Unexpected, ErrorTypes.Unexpected, message, message);
}

public record ResultBase<T>(
    bool Success = false,
    T? Result = default,
    ResultError Error = ResultError.None,
    ErrorTypes? ErrorType = null,
    string? ErrorMessage = null,
    string? Message = null)
    : ResultBase(Success, Error, ErrorType, ErrorMessage, Message)
{
    public static ResultBase<T> Ok(T value, string? message = null) => new(true, value, ResultError.None, null, null, message);

    public new static ResultBase<T> BadRequest(string message) => new(false, default, ResultError.Validation, ErrorTypes.Validation, message, message);
    public new static ResultBase<T> Unauthorized(string message) => new(false, default, ResultError.Unauthorized, ErrorTypes.Unauthorized, message, message);
    public new static ResultBase<T> Forbidden(string message) => new(false, default, ResultError.Forbidden, ErrorTypes.Forbidden, message, message);
    public new static ResultBase<T> NotFound(string message) => new(false, default, ResultError.NotFound, ErrorTypes.NotFound, message, message);
    public new static ResultBase<T> Conflict(string message) => new(false, default, ResultError.Conflict, ErrorTypes.Conflict, message, message);
    public new static ResultBase<T> Unprocessable(string message) => new(false, default, ResultError.Unprocessable, ErrorTypes.Unprocessable, message, message);
    public new static ResultBase<T> Unexpected(string message = "An unexpected error occurred.") => new(false, default, ResultError.Unexpected, ErrorTypes.Unexpected, message, message);
}

