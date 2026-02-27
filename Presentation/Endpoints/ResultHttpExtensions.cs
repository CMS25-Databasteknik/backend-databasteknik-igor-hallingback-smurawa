using Backend.Application.Common;

namespace Backend.Presentation.API.Endpoints;

public static class ResultHttpExtensions
{
    public static IResult ToHttpResult(this ResultBase result)
        => result.ErrorType switch
        {
            ErrorTypes.NotFound => Results.NotFound(result.ErrorMessage),
            ErrorTypes.Validation => Results.BadRequest(result.ErrorMessage),
            ErrorTypes.Unauthorized => Results.StatusCode(StatusCodes.Status401Unauthorized),
            ErrorTypes.Forbidden => Results.StatusCode(StatusCodes.Status403Forbidden),
            ErrorTypes.Conflict => Results.Conflict(result.ErrorMessage),
            ErrorTypes.Unprocessable => Results.UnprocessableEntity(result.ErrorMessage),
            ErrorTypes.Unexpected => Results.Problem(result.ErrorMessage),
            _ => Results.Problem("An unknown error occurred.")
        };

    public static IResult ToHttpResult<T>(this ResultCommon<T> result)
        => result.ErrorType switch
        {
            ErrorTypes.NotFound => Results.NotFound(result.ErrorMessage),
            ErrorTypes.Validation => Results.BadRequest(result.ErrorMessage),
            ErrorTypes.Unauthorized => Results.StatusCode(StatusCodes.Status401Unauthorized),
            ErrorTypes.Forbidden => Results.StatusCode(StatusCodes.Status403Forbidden),
            ErrorTypes.Conflict => Results.Conflict(result.ErrorMessage),
            ErrorTypes.Unprocessable => Results.UnprocessableEntity(result.ErrorMessage),
            ErrorTypes.Unexpected => Results.Problem(result.ErrorMessage),
            _ => Results.Problem("An unknown error occurred.")
        };
}
