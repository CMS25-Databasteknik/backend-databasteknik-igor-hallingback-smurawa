using Backend.Application.Common;

namespace Backend.Presentation.API.Endpoints;

public static class ResultHttpExtensions
{
    public static IResult ToHttpResult(this Result result)
        => result.ErrorType switch
        {
            ErrorTypes.NotFound => Results.NotFound(result.ErrorMessage),
            ErrorTypes.Validation => Results.BadRequest(result.ErrorMessage),
            ErrorTypes.Unauthorized => Results.Unauthorized(),
            ErrorTypes.Forbidden => Results.Forbid(),
            ErrorTypes.Conflict => Results.Conflict(result.ErrorMessage),
            ErrorTypes.Unprocessable => Results.UnprocessableEntity(result.ErrorMessage),
            ErrorTypes.Unexpected => Results.Problem(result.ErrorMessage),
            _ => Results.Problem("An unknown error occurred.")
        };

    public static IResult ToHttpResult<T>(this Result<T> result)
        => result.ErrorType switch
        {
            ErrorTypes.NotFound => Results.NotFound(result.ErrorMessage),
            ErrorTypes.Validation => Results.BadRequest(result.ErrorMessage),
            ErrorTypes.Unauthorized => Results.Unauthorized(),
            ErrorTypes.Forbidden => Results.Forbid(),
            ErrorTypes.Conflict => Results.Conflict(result.ErrorMessage),
            ErrorTypes.Unprocessable => Results.UnprocessableEntity(result.ErrorMessage),
            ErrorTypes.Unexpected => Results.Problem(result.ErrorMessage),
            _ => Results.Problem("An unknown error occurred.")
        };

    public static IResult ToHttpResult(this ResultBase result)
        => result.ErrorType switch
        {
            ErrorTypes.NotFound => Results.NotFound(result),
            ErrorTypes.Validation => Results.BadRequest(result),
            ErrorTypes.Unauthorized => Results.Unauthorized(),
            ErrorTypes.Forbidden => Results.Forbid(),
            ErrorTypes.Conflict => Results.Conflict(result),
            ErrorTypes.Unprocessable => Results.UnprocessableEntity(result),
            ErrorTypes.Unexpected => Results.Problem(detail: result.ErrorMessage),
            _ => Results.Problem("An unknown error occurred.")
        };

    public static IResult ToHttpResult<T>(this ResultCommon<T> result)
        => result.ErrorType switch
        {
            ErrorTypes.NotFound => Results.NotFound(result),
            ErrorTypes.Validation => Results.BadRequest(result),
            ErrorTypes.Unauthorized => Results.Unauthorized(),
            ErrorTypes.Forbidden => Results.Forbid(),
            ErrorTypes.Conflict => Results.Conflict(result),
            ErrorTypes.Unprocessable => Results.UnprocessableEntity(result),
            ErrorTypes.Unexpected => Results.Problem(detail: result.ErrorMessage),
            _ => Results.Problem("An unknown error occurred.")
        };
}
