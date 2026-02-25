using Backend.Application.Modules.CourseRegistrations;
using Backend.Application.Modules.CourseRegistrations.Inputs;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using Backend.Domain.Modules.PaymentMethod.Models;
using Backend.Presentation.API.Models.CourseRegistration;

namespace Backend.Presentation.API.Endpoints;

public static class CourseRegistrationsEndpoints
{
    public static RouteGroupBuilder MapCourseRegistrationsEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/course-registrations")
            .WithTags("Course registrations");
        var participantsGroup = api.MapGroup("/participants");
        var courseEventsGroup = api.MapGroup("/course-events");

        group.MapGet("", GetAllCourseRegistrations).WithName("GetAllCourseRegistrations");
        group.MapGet("/{id:guid}", GetCourseRegistrationById).WithName("GetCourseRegistrationById");
        participantsGroup.MapGet("/{participantId:guid}/registrations", GetCourseRegistrationsByParticipantId).WithName("GetCourseRegistrationsByParticipantId");
        courseEventsGroup.MapGet("/{courseEventId:guid}/registrations", GetCourseRegistrationsByCourseEventId).WithName("GetCourseRegistrationsByCourseEventId");
        group.MapPost("", CreateCourseRegistration).WithName("CreateCourseRegistration");
        group.MapPut("/{id:guid}", UpdateCourseRegistration).WithName("UpdateCourseRegistration");
        group.MapDelete("/{id:guid}", DeleteCourseRegistration).WithName("DeleteCourseRegistration");

        return group;
    }

    private static async Task<IResult> GetAllCourseRegistrations(ICourseRegistrationService service, CancellationToken cancellationToken)
    {
        var response = await service.GetAllCourseRegistrationsAsync(cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> GetCourseRegistrationById(Guid id, ICourseRegistrationService service, CancellationToken cancellationToken)
    {
        var response = await service.GetCourseRegistrationByIdAsync(id, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> GetCourseRegistrationsByParticipantId(Guid participantId, ICourseRegistrationService service, CancellationToken cancellationToken)
    {
        var response = await service.GetCourseRegistrationsByParticipantIdAsync(participantId, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> GetCourseRegistrationsByCourseEventId(Guid courseEventId, ICourseRegistrationService service, CancellationToken cancellationToken)
    {
        var response = await service.GetCourseRegistrationsByCourseEventIdAsync(courseEventId, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> CreateCourseRegistration(CreateCourseRegistrationRequest request, ICourseRegistrationService service, CancellationToken cancellationToken)
    {
        var status = MapStatus(request.StatusId);
        if (status is null)
            return Results.BadRequest("Invalid statusId. Value must be zero or positive.");

        var paymentMethod = MapPaymentMethod(request.PaymentMethodId);
        if (paymentMethod is null)
            return Results.BadRequest("Invalid paymentMethodId. Value must be zero or positive.");

        var input = new CreateCourseRegistrationInput(request.ParticipantId, request.CourseEventId, status, paymentMethod);
        var response = await service.CreateCourseRegistrationAsync(input, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Created($"/api/course-registrations/{response.Result?.Id}", response);
    }

    private static async Task<IResult> UpdateCourseRegistration(Guid id, UpdateCourseRegistrationRequest request, ICourseRegistrationService service, CancellationToken cancellationToken)
    {
        var status = MapStatus(request.StatusId);
        if (status is null)
            return Results.BadRequest("Invalid statusId. Value must be zero or positive.");

        var paymentMethod = MapPaymentMethod(request.PaymentMethodId);
        if (paymentMethod is null)
            return Results.BadRequest("Invalid paymentMethodId. Value must be zero or positive.");

        var input = new UpdateCourseRegistrationInput(id, request.ParticipantId, request.CourseEventId, status, paymentMethod);
        var response = await service.UpdateCourseRegistrationAsync(input, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteCourseRegistration(Guid id, ICourseRegistrationService service, CancellationToken cancellationToken)
    {
        var response = await service.DeleteCourseRegistrationAsync(id, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static CourseRegistrationStatus? MapStatus(int statusId)
        => statusId switch
        {
            0 => CourseRegistrationStatus.Pending,
            1 => CourseRegistrationStatus.Paid,
            2 => CourseRegistrationStatus.Cancelled,
            3 => CourseRegistrationStatus.Refunded,
            < 0 => null,
            _ => new CourseRegistrationStatus(statusId, $"Status {statusId}")
        };

    private static PaymentMethod? MapPaymentMethod(int paymentMethodId)
        => paymentMethodId switch
        {
            1 => new PaymentMethod(1, "Card"),
            2 => new PaymentMethod(2, "Invoice"),
            3 => new PaymentMethod(3, "Cash"),
            < 0 => null,
            _ => new PaymentMethod(paymentMethodId, $"PaymentMethod {paymentMethodId}")
        };
}
