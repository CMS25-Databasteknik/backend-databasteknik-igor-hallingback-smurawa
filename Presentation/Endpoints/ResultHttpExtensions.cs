using Backend.Application.Common;

namespace Backend.Presentation.API.Endpoints;

public static class ResultHttpExtensions
{
    public static IResult ToHttpResult(this ResultBase result)
        => result.ErrorType switch
        {
            ErrorTypes.NotFound => Results.NotFound(result),
            ErrorTypes.Validation => Results.BadRequest(result),
            ErrorTypes.Conflict => Results.Conflict(result),
            ErrorTypes.Unexpected => Results.Problem(result.Message),
            _ => Results.Problem("An unknown error occurred.")
        };

    public static IResult ToHttpResult<T>(this ResultBase<T> result)
        => result.ErrorType switch
        {
            ErrorTypes.NotFound => Results.NotFound(result),
            ErrorTypes.Validation => Results.BadRequest(result),
            ErrorTypes.Conflict => Results.Conflict(result),
            ErrorTypes.Unexpected => Results.Problem(result.Message),
            _ => Results.Problem("An unknown error occurred.")
        };
}
