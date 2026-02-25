using Backend.Application.Modules.CourseEvents.Inputs;
using Backend.Domain.Modules.VenueTypes.Models;
using Backend.Presentation.API.Models.CourseEvent;

namespace Backend.Presentation.API.Endpoints.CourseEvents;

public static class CourseEventRequestMapper
{
    public static CreateCourseEventInput ToCreateInput(CreateCourseEventRequest request)
        => new(
            request.CourseId,
            request.EventDate,
            request.Price,
            request.Seats,
            request.CourseEventTypeId,
            new VenueType(request.VenueType.Id, request.VenueType.Name));

    public static UpdateCourseEventInput ToUpdateInput(Guid id, UpdateCourseEventRequest request)
        => new(
            id,
            request.CourseId,
            request.EventDate,
            request.Price,
            request.Seats,
            request.CourseEventTypeId,
            new VenueType(request.VenueType.Id, request.VenueType.Name));
}
