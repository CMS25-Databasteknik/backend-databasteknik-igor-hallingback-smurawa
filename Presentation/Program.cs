using Backend.Application.Extensions;
using Backend.Application.Modules.CourseEvents;
using Backend.Application.Modules.CourseEvents.Inputs;
using Backend.Application.Modules.CourseEventTypes;
using Backend.Application.Modules.CourseEventTypes.Inputs;
using Backend.Application.Modules.CourseRegistrations;
using Backend.Application.Modules.CourseRegistrations.Inputs;
using Backend.Application.Modules.Courses;
using Backend.Application.Modules.Courses.Inputs;
using Backend.Application.Modules.Participants;
using Backend.Application.Modules.Participants.Inputs;
using Backend.Infrastructure.Extensions;
using Backend.Presentation.API.Models.Course;
using Backend.Presentation.API.Models.CourseEvent;
using Backend.Presentation.API.Models.CourseEventType;
using Backend.Presentation.API.Models.CourseRegistration;
using Backend.Presentation.API.Models.Participant;

namespace Backend.Presentation.API;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddCors();

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplication(builder.Configuration, builder.Environment);

        var app = builder.Build();

        app.MapOpenApi();
        app.UseHttpsRedirection();
        app.UseCors(policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());

        app.MapGet("/api/courses", async (ICourseService courseService, CancellationToken cancellationToken) =>
        {
            var response = await courseService.GetAllCoursesAsync(cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetAllCourses");

        app.MapGet("/api/courses/{id:guid}", async (Guid id, ICourseService courseService, CancellationToken cancellationToken) =>
        {
            var response = await courseService.GetCourseByIdAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetCourseById");

        app.MapPost("/api/courses", async (CreateCourseRequest request, ICourseService courseService, CancellationToken cancellationToken) =>
        {
            var input = new CreateCourseInput(request.Title, request.Description, request.DurationInDays);
            var response = await courseService.CreateCourseAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Created($"/api/courses/{response.Result?.Id}", response);
        }).WithName("CreateCourse");

        app.MapPut("/api/courses/{id:guid}", async (Guid id, UpdateCourseRequest request, ICourseService courseService, CancellationToken cancellationToken) =>
        {
            var input = new UpdateCourseInput(id, request.Title, request.Description, request.DurationInDays);
            var response = await courseService.UpdateCourseAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("UpdateCourse");

        app.MapDelete("/api/courses/{id:guid}", async (Guid id, ICourseService courseService, CancellationToken cancellationToken) =>
        {
            var response = await courseService.DeleteCourseAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("DeleteCourse");

        app.MapGet("/api/course-events", async (ICourseEventService courseEventService, CancellationToken cancellationToken) =>
        {
            var response = await courseEventService.GetAllCourseEventsAsync(cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetAllCourseEvents");

        app.MapGet("/api/course-events/{id:guid}", async (Guid id, ICourseEventService courseEventService, CancellationToken cancellationToken) =>
        {
            var response = await courseEventService.GetCourseEventByIdAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetCourseEventById");

        app.MapGet("/api/courses/{courseId:guid}/events", async (Guid courseId, ICourseEventService courseEventService, CancellationToken cancellationToken) =>
        {
            var response = await courseEventService.GetCourseEventsByCourseIdAsync(courseId, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetCourseEventsByCourseId");

        app.MapPost("/api/course-events", async (CreateCourseEventRequest request, ICourseEventService courseEventService, CancellationToken cancellationToken) =>
        {
            var input = new CreateCourseEventInput(request.CourseId, request.EventDate, request.Price, request.Seats, request.CourseEventTypeId);
            var response = await courseEventService.CreateCourseEventAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Created($"/api/course-events/{response.Result?.Id}", response);
        }).WithName("CreateCourseEvent");

        app.MapPut("/api/course-events/{id:guid}", async (Guid id, UpdateCourseEventRequest request, ICourseEventService courseEventService, CancellationToken cancellationToken) =>
        {
            var input = new UpdateCourseEventInput(id, request.CourseId, request.EventDate, request.Price, request.Seats, request.CourseEventTypeId);
            var response = await courseEventService.UpdateCourseEventAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("UpdateCourseEvent");

        app.MapDelete("/api/course-events/{id:guid}", async (Guid id, ICourseEventService courseEventService, CancellationToken cancellationToken) =>
        {
            var response = await courseEventService.DeleteCourseEventAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("DeleteCourseEvent");

        app.MapGet("/api/course-event-types", async (ICourseEventTypeService courseEventTypeService, CancellationToken cancellationToken) =>
        {
            var response = await courseEventTypeService.GetAllCourseEventTypesAsync(cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetAllCourseEventTypes");

        app.MapGet("/api/course-event-types/{id:int}", async (int id, ICourseEventTypeService courseEventTypeService, CancellationToken cancellationToken) =>
        {
            var response = await courseEventTypeService.GetCourseEventTypeByIdAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetCourseEventTypeById");

        app.MapPost("/api/course-event-types", async (CreateCourseEventTypeRequest request, ICourseEventTypeService courseEventTypeService, CancellationToken cancellationToken) =>
        {
            var input = new CreateCourseEventTypeInput(request.TypeName);
            var response = await courseEventTypeService.CreateCourseEventTypeAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Created($"/api/course-event-types/{response.Result?.Id}", response);
        }).WithName("CreateCourseEventType");

        app.MapPut("/api/course-event-types/{id:int}", async (int id, UpdateCourseEventTypeRequest request, ICourseEventTypeService courseEventTypeService, CancellationToken cancellationToken) =>
        {
            var input = new UpdateCourseEventTypeInput(id, request.TypeName);
            var response = await courseEventTypeService.UpdateCourseEventTypeAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("UpdateCourseEventType");

        app.MapDelete("/api/course-event-types/{id:int}", async (int id, ICourseEventTypeService courseEventTypeService, CancellationToken cancellationToken) =>
        {
            var response = await courseEventTypeService.DeleteCourseEventTypeAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("DeleteCourseEventType");

        app.MapGet("/api/course-registrations", async (ICourseRegistrationService courseRegistrationService, CancellationToken cancellationToken) =>
        {
            var response = await courseRegistrationService.GetAllCourseRegistrationsAsync(cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetAllCourseRegistrations");

        app.MapGet("/api/course-registrations/{id:guid}", async (Guid id, ICourseRegistrationService courseRegistrationService, CancellationToken cancellationToken) =>
        {
            var response = await courseRegistrationService.GetCourseRegistrationByIdAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetCourseRegistrationById");

        app.MapGet("/api/participants/{participantId:guid}/registrations", async (Guid participantId, ICourseRegistrationService courseRegistrationService, CancellationToken cancellationToken) =>
        {
            var response = await courseRegistrationService.GetCourseRegistrationsByParticipantIdAsync(participantId, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetCourseRegistrationsByParticipantId");

        app.MapGet("/api/course-events/{courseEventId:guid}/registrations", async (Guid courseEventId, ICourseRegistrationService courseRegistrationService, CancellationToken cancellationToken) =>
        {
            var response = await courseRegistrationService.GetCourseRegistrationsByCourseEventIdAsync(courseEventId, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetCourseRegistrationsByCourseEventId");

        app.MapPost("/api/course-registrations", async (CreateCourseRegistrationRequest request, ICourseRegistrationService courseRegistrationService, CancellationToken cancellationToken) =>
        {
            var input = new CreateCourseRegistrationInput(request.ParticipantId, request.CourseEventId, request.IsPaid);
            var response = await courseRegistrationService.CreateCourseRegistrationAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Created($"/api/course-registrations/{response.Result?.Id}", response);
        }).WithName("CreateCourseRegistration");

        app.MapPut("/api/course-registrations/{id:guid}", async (Guid id, UpdateCourseRegistrationRequest request, ICourseRegistrationService courseRegistrationService, CancellationToken cancellationToken) =>
        {
            var input = new UpdateCourseRegistrationInput(id, request.ParticipantId, request.CourseEventId, request.IsPaid);
            var response = await courseRegistrationService.UpdateCourseRegistrationAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("UpdateCourseRegistration");

        app.MapDelete("/api/course-registrations/{id:guid}", async (Guid id, ICourseRegistrationService courseRegistrationService, CancellationToken cancellationToken) =>
        {
            var response = await courseRegistrationService.DeleteCourseRegistrationAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("DeleteCourseRegistration");

        app.MapGet("/api/participants", async (IParticipantService participantService, CancellationToken cancellationToken) =>
        {
            var response = await participantService.GetAllParticipantsAsync(cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetAllParticipants");

        app.MapGet("/api/participants/{id:guid}", async (Guid id, IParticipantService participantService, CancellationToken cancellationToken) =>
        {
            var response = await participantService.GetParticipantByIdAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetParticipantById");

        app.MapPost("/api/participants", async (CreateParticipantRequest request, IParticipantService participantService, CancellationToken cancellationToken) =>
        {
            var input = new CreateParticipantInput(request.FirstName, request.LastName, request.Email, request.PhoneNumber);
            var response = await participantService.CreateParticipantAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Created($"/api/participants/{response.Result?.Id}", response);
        }).WithName("CreateParticipant");

        app.MapPut("/api/participants/{id:guid}", async (Guid id, UpdateParticipantRequest request, IParticipantService participantService, CancellationToken cancellationToken) =>
        {
            var input = new UpdateParticipantInput(id, request.FirstName, request.LastName, request.Email, request.PhoneNumber);
            var response = await participantService.UpdateParticipantAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("UpdateParticipant");

        app.MapDelete("/api/participants/{id:guid}", async (Guid id, IParticipantService participantService, CancellationToken cancellationToken) =>
        {
            var response = await participantService.DeleteParticipantAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("DeleteParticipant");

        app.Run();
    }
}
