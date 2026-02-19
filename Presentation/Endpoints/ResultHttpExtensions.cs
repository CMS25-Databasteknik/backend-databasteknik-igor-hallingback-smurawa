using Backend.Application.Common;

namespace Backend.Presentation.API.Endpoints;

public static class ResultHttpExtensions
{
    public static IResult ToHttpResult(this ResultBase response)
    {
        return response.StatusCode switch
        {
            400 => Results.BadRequest(response),
            401 => Results.Unauthorized(),
            403 => Results.Forbid(),
            404 => Results.NotFound(response),
            409 => Results.Conflict(response),
            422 => Results.UnprocessableEntity(response),
            500 => Results.InternalServerError(response),
            _ => Results.Json(response, statusCode: response.StatusCode is >= 400 and <= 599 ? response.StatusCode : 500)
        };
    }
}
