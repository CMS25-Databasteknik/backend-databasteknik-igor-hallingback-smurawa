using Backend.Presentation.API.Models.CourseEvent;

namespace Backend.Presentation.API.Endpoints.CourseEvents;

public static class CourseEventRequestValidator
{
    public static Dictionary<string, string[]> Validate(
        Guid courseId,
        DateTime eventDate,
        decimal price,
        int seats,
        int courseEventTypeId,
        VenueTypeRequest? venueType)
    {
        var errors = new Dictionary<string, string[]>();

        if (courseId == Guid.Empty)
            errors["courseId"] = ["courseId is required and must be a non-empty GUID."];

        if (eventDate == default)
            errors["eventDate"] = ["eventDate is required."];

        if (price < 0)
            errors["price"] = ["price cannot be negative."];

        if (seats <= 0)
            errors["seats"] = ["seats must be greater than zero."];

        if (courseEventTypeId <= 0)
            errors["courseEventTypeId"] = ["courseEventTypeId must be greater than zero."];

        if (venueType is null)
            errors["venueType"] = ["venueType is required."];
        else
        {
            if (venueType.Id <= 0)
                errors["venueType.id"] = ["venueType.id must be greater than zero."];

            if (string.IsNullOrWhiteSpace(venueType.Name))
                errors["venueType.name"] = ["venueType.name is required."];
        }

        return errors;
    }
}
