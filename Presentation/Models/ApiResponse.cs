namespace Backend.Presentation.API.Models;

public sealed record ApiResponse(
    bool Success,
    object? Result = null,
    string? Message = null,
    string? Code = null);

public sealed record ApiResponse<T>(
    bool Success,
    T? Result = default,
    string? Message = null,
    string? Code = null);
