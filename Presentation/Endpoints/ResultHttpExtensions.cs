using Backend.Application.Common;
using Backend.Presentation.API.Models;

namespace Backend.Presentation.API.Endpoints;

public static class ResultHttpExtensions
{
    public static ApiResponse ToApiResponse(this ResultBase result)
        => new(result.Success, null, result.Message, ToCode(result.ErrorType));

    public static ApiResponse<T> ToApiResponse<T>(this ResultCommon<T> result)
        => new(result.Success, result.Result, result.Message, ToCode(result.ErrorType));

    public static IResult ToCreatedResult<T>(this ResultCommon<T> result, string location)
        => Results.Created(location, result.ToApiResponse());

    public static IResult ToHttpResult(this ResultBase result)
        => result.ErrorType switch
        {
            ErrorTypes.NotFound => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status404NotFound),
            ErrorTypes.Validation => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status400BadRequest),
            ErrorTypes.Unauthorized => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status401Unauthorized),
            ErrorTypes.Forbidden => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status403Forbidden),
            ErrorTypes.Conflict => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status409Conflict),
            ErrorTypes.Unprocessable => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status422UnprocessableEntity),
            ErrorTypes.Unexpected => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status500InternalServerError),
            _ => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status500InternalServerError)
        };

    public static IResult ToHttpResult<T>(this ResultCommon<T> result)
        => result.ErrorType switch
        {
            ErrorTypes.NotFound => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status404NotFound),
            ErrorTypes.Validation => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status400BadRequest),
            ErrorTypes.Unauthorized => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status401Unauthorized),
            ErrorTypes.Forbidden => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status403Forbidden),
            ErrorTypes.Conflict => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status409Conflict),
            ErrorTypes.Unprocessable => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status422UnprocessableEntity),
            ErrorTypes.Unexpected => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status500InternalServerError),
            _ => Results.Json(result.ToApiResponse(), statusCode: StatusCodes.Status500InternalServerError)
        };

    private static string? ToCode(ErrorTypes? errorType) => errorType switch
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
