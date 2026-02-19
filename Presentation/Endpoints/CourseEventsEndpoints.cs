using Backend.Application.Modules.CourseEvents;
using Backend.Application.Modules.CourseEvents.Inputs;
using Backend.Presentation.API.Models.CourseEvent;

namespace Backend.Presentation.API.Endpoints;

public static class CourseEventsEndpoints
{
    public static RouteGroupBuilder MapCourseEventsEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/course-events")
            .WithTags("Course events");

        group.MapGet("", GetAllCourseEvents).WithName("GetAllCourseEvents");
        group.MapGet("/{id:guid}", GetCourseEventById).WithName("GetCourseEventById");
        api.MapGet("/courses/{courseId:guid}/events", GetCourseEventsByCourseId).WithName("GetCourseEventsByCourseId");
        group.MapPost("", CreateCourseEvent).WithName("CreateCourseEvent");
        group.MapPut("/{id:guid}", UpdateCourseEvent).WithName("UpdateCourseEvent");
        group.MapDelete("/{id:guid}", DeleteCourseEvent).WithName("DeleteCourseEvent");

        return group;
    }

    private static async Task<IResult> GetAllCourseEvents(ICourseEventService service, CancellationToken cancellationToken)
    {
        var response = await service.GetAllCourseEventsAsync(cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> GetCourseEventById(Guid id, ICourseEventService service, CancellationToken cancellationToken)
    {
        var response = await service.GetCourseEventByIdAsync(id, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> GetCourseEventsByCourseId(Guid courseId, ICourseEventService service, CancellationToken cancellationToken)
    {
        var response = await service.GetCourseEventsByCourseIdAsync(courseId, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> CreateCourseEvent(CreateCourseEventRequest request, ICourseEventService service, CancellationToken cancellationToken)
    {
        var input = new CreateCourseEventInput(request.CourseId, request.EventDate, request.Price, request.Seats, request.CourseEventTypeId, request.VenueType);
        var response = await service.CreateCourseEventAsync(input, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Created($"/api/course-events/{response.Result?.Id}", response);
    }

    private static async Task<IResult> UpdateCourseEvent(Guid id, UpdateCourseEventRequest request, ICourseEventService service, CancellationToken cancellationToken)
    {
        var input = new UpdateCourseEventInput(id, request.CourseId, request.EventDate, request.Price, request.Seats, request.CourseEventTypeId, request.VenueType);
        var response = await service.UpdateCourseEventAsync(input, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteCourseEvent(Guid id, ICourseEventService service, CancellationToken cancellationToken)
    {
        var response = await service.DeleteCourseEventAsync(id, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }
}
