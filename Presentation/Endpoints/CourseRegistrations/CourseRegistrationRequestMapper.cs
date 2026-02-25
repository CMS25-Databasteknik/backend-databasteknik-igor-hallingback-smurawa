using Backend.Application.Modules.CourseRegistrations.Inputs;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using Backend.Domain.Modules.PaymentMethod.Models;
using Backend.Presentation.API.Models.CourseRegistration;

namespace Backend.Presentation.API.Endpoints.CourseRegistrations;

public static class CourseRegistrationRequestMapper
{
    public static CourseRegistrationStatus? MapStatus(int statusId)
        => statusId switch
        {
            0 => CourseRegistrationStatus.Pending,
            1 => CourseRegistrationStatus.Paid,
            2 => CourseRegistrationStatus.Cancelled,
            3 => CourseRegistrationStatus.Refunded,
            < 0 => null,
            _ => new CourseRegistrationStatus(statusId, $"Status {statusId}")
        };

    public static CreateCourseRegistrationInput ToCreateInput(CreateCourseRegistrationRequest request, CourseRegistrationStatus status)
        => new(
            request.ParticipantId,
            request.CourseEventId,
            status,
            new PaymentMethod(request.PaymentMethod.Id, request.PaymentMethod.Name));

    public static UpdateCourseRegistrationInput ToUpdateInput(Guid id, UpdateCourseRegistrationRequest request, CourseRegistrationStatus status)
        => new(
            id,
            request.ParticipantId,
            request.CourseEventId,
            status,
            new PaymentMethod(request.PaymentMethod.Id, request.PaymentMethod.Name));
}
