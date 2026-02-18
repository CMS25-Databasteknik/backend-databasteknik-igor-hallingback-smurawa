using Backend.Application.Extensions;
using Backend.Application.Modules.CourseEvents;
using Backend.Application.Modules.CourseEvents.Inputs;
using Backend.Application.Modules.CourseEventTypes;
using Backend.Application.Modules.CourseEventTypes.Inputs;
using Backend.Application.Modules.CourseRegistrations;
using Backend.Application.Modules.CourseRegistrations.Inputs;
using Backend.Application.Modules.Courses;
using Backend.Application.Modules.Courses.Inputs;
using Backend.Application.Modules.InPlaceLocations;
using Backend.Application.Modules.InPlaceLocations.Inputs;
using Backend.Application.Modules.InstructorRoles;
using Backend.Application.Modules.InstructorRoles.Inputs;
using Backend.Application.Modules.Instructors;
using Backend.Application.Modules.Instructors.Inputs;
using Backend.Application.Modules.Locations;
using Backend.Application.Modules.Locations.Inputs;
using Backend.Application.Modules.Participants;
using Backend.Application.Modules.Participants.Inputs;
using Backend.Infrastructure.Extensions;
using Backend.Presentation.API.Models.Course;
using Backend.Presentation.API.Models.CourseEvent;
using Backend.Presentation.API.Models.CourseEventType;
using Backend.Presentation.API.Models.CourseRegistration;
using Backend.Presentation.API.Models.InPlaceLocation;
using Backend.Presentation.API.Models.Instructor;
using Backend.Presentation.API.Models.InstructorRole;
using Backend.Presentation.API.Models.Location;
using Backend.Presentation.API.Models.Participant;

namespace Backend.Presentation.API;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddCors();

        builder.Services.AddMemoryCache();

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
            var input = new CreateCourseEventInput(request.CourseId, request.EventDate, request.Price, request.Seats, request.CourseEventTypeId, request.VenueType);
            var response = await courseEventService.CreateCourseEventAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Created($"/api/course-events/{response.Result?.Id}", response);
        }).WithName("CreateCourseEvent");

        app.MapPut("/api/course-events/{id:guid}", async (Guid id, UpdateCourseEventRequest request, ICourseEventService courseEventService, CancellationToken cancellationToken) =>
        {
            var input = new UpdateCourseEventInput(id, request.CourseId, request.EventDate, request.Price, request.Seats, request.CourseEventTypeId, request.VenueType);
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
            var input = new CreateCourseRegistrationInput(request.ParticipantId, request.CourseEventId, request.Status, request.PaymentMethod);
            var response = await courseRegistrationService.CreateCourseRegistrationAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Created($"/api/course-registrations/{response.Result?.Id}", response);
        }).WithName("CreateCourseRegistration");

        app.MapPut("/api/course-registrations/{id:guid}", async (Guid id, UpdateCourseRegistrationRequest request, ICourseRegistrationService courseRegistrationService, CancellationToken cancellationToken) =>
        {
            var input = new UpdateCourseRegistrationInput(id, request.ParticipantId, request.CourseEventId, request.Status, request.PaymentMethod);
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
            var input = new CreateParticipantInput(request.FirstName, request.LastName, request.Email, request.PhoneNumber, request.ContactType);
            var response = await participantService.CreateParticipantAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Created($"/api/participants/{response.Result?.Id}", response);
        }).WithName("CreateParticipant");

        app.MapPut("/api/participants/{id:guid}", async (Guid id, UpdateParticipantRequest request, IParticipantService participantService, CancellationToken cancellationToken) =>
        {
            var input = new UpdateParticipantInput(id, request.FirstName, request.LastName, request.Email, request.PhoneNumber, request.ContactType);
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

        app.MapGet("/api/locations", async (ILocationService locationService, CancellationToken cancellationToken) =>
        {
            var response = await locationService.GetAllLocationsAsync(cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetAllLocations");

        app.MapGet("/api/locations/{id:int}", async (int id, ILocationService locationService, CancellationToken cancellationToken) =>
        {
            var response = await locationService.GetLocationByIdAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetLocationById");

        app.MapPost("/api/locations", async (CreateLocationRequest request, ILocationService locationService, CancellationToken cancellationToken) =>
        {
            var input = new CreateLocationInput(request.StreetName, request.PostalCode, request.City);
            var response = await locationService.CreateLocationAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Created($"/api/locations/{response.Result?.Id}", response);
        }).WithName("CreateLocation");

        app.MapPut("/api/locations/{id:int}", async (int id, UpdateLocationRequest request, ILocationService locationService, CancellationToken cancellationToken) =>
        {
            var input = new UpdateLocationInput(id, request.StreetName, request.PostalCode, request.City);
            var response = await locationService.UpdateLocationAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("UpdateLocation");

        app.MapDelete("/api/locations/{id:int}", async (int id, ILocationService locationService, CancellationToken cancellationToken) =>
        {
            var response = await locationService.DeleteLocationAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("DeleteLocation");

        app.MapGet("/api/in-place-locations", async (IInPlaceLocationService inPlaceLocationService, CancellationToken cancellationToken) =>
        {
            var response = await inPlaceLocationService.GetAllInPlaceLocationsAsync(cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetAllInPlaceLocations");

        app.MapGet("/api/in-place-locations/{id:int}", async (int id, IInPlaceLocationService inPlaceLocationService, CancellationToken cancellationToken) =>
        {
            var response = await inPlaceLocationService.GetInPlaceLocationByIdAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetInPlaceLocationById");

        app.MapGet("/api/locations/{locationId:int}/in-place-locations", async (int locationId, IInPlaceLocationService inPlaceLocationService, CancellationToken cancellationToken) =>
        {
            var response = await inPlaceLocationService.GetInPlaceLocationsByLocationIdAsync(locationId, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetInPlaceLocationsByLocationId");

        app.MapPost("/api/in-place-locations", async (CreateInPlaceLocationRequest request, IInPlaceLocationService inPlaceLocationService, CancellationToken cancellationToken) =>
        {
            var input = new CreateInPlaceLocationInput(request.LocationId, request.RoomNumber, request.Seats);
            var response = await inPlaceLocationService.CreateInPlaceLocationAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Created($"/api/in-place-locations/{response.Result?.Id}", response);
        }).WithName("CreateInPlaceLocation");

        app.MapPut("/api/in-place-locations/{id:int}", async (int id, UpdateInPlaceLocationRequest request, IInPlaceLocationService inPlaceLocationService, CancellationToken cancellationToken) =>
        {
            var input = new UpdateInPlaceLocationInput(id, request.LocationId, request.RoomNumber, request.Seats);
            var response = await inPlaceLocationService.UpdateInPlaceLocationAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("UpdateInPlaceLocation");

        app.MapDelete("/api/in-place-locations/{id:int}", async (int id, IInPlaceLocationService inPlaceLocationService, CancellationToken cancellationToken) =>
        {
            var response = await inPlaceLocationService.DeleteInPlaceLocationAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("DeleteInPlaceLocation");

        app.MapGet("/api/instructors", async (IInstructorService instructorService, CancellationToken cancellationToken) =>
        {
            var response = await instructorService.GetAllInstructorsAsync(cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetAllInstructors");

        app.MapGet("/api/instructors/{id:guid}", async (Guid id, IInstructorService instructorService, CancellationToken cancellationToken) =>
        {
            var response = await instructorService.GetInstructorByIdAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("GetInstructorById");

        app.MapPost("/api/instructors", async (CreateInstructorRequest request, IInstructorService instructorService, CancellationToken cancellationToken) =>
        {
            var input = new CreateInstructorInput(request.Name, request.InstructorRoleId);
            var response = await instructorService.CreateInstructorAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Created($"/api/instructors/{response.Result?.Id}", response);
        }).WithName("CreateInstructor");

        app.MapPut("/api/instructors/{id:guid}", async (Guid id, UpdateInstructorRequest request, IInstructorService instructorService, CancellationToken cancellationToken) =>
        {
            var input = new UpdateInstructorInput(id, request.Name, request.InstructorRoleId);
            var response = await instructorService.UpdateInstructorAsync(input, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("UpdateInstructor");

        app.MapDelete("/api/instructors/{id:guid}", async (Guid id, IInstructorService instructorService, CancellationToken cancellationToken) =>
        {
            var response = await instructorService.DeleteInstructorAsync(id, cancellationToken);

            if (!response.Success)
                return Results.BadRequest(response);

            return Results.Ok(response);
        }).WithName("DeleteInstructor");

        app.MapGet("/api/instructor-roles", async (IInstructorRoleService roleService, CancellationToken cancellationToken) =>
        {
            var response = await roleService.GetAllInstructorRolesAsync(cancellationToken);
            return Results.Ok(response);
        }).WithName("GetAllInstructorRoles");

        app.MapGet("/api/instructor-roles/{id:int}", async (int id, IInstructorRoleService roleService, CancellationToken cancellationToken) =>
        {
            var response = await roleService.GetInstructorRoleByIdAsync(id, cancellationToken);
            if (!response.Success)
                return Results.BadRequest(response);
            return Results.Ok(response);
        }).WithName("GetInstructorRoleById");

        app.MapPost("/api/instructor-roles", async (CreateInstructorRoleRequest request, IInstructorRoleService roleService, CancellationToken cancellationToken) =>
        {
            var input = new CreateInstructorRoleInput(request.RoleName);
            var response = await roleService.CreateInstructorRoleAsync(input, cancellationToken);
            if (!response.Success)
                return Results.BadRequest(response);
            return Results.Created($"/api/instructor-roles/{response.Result?.Id}", response);
        }).WithName("CreateInstructorRole");

        app.MapPut("/api/instructor-roles/{id:int}", async (int id, UpdateInstructorRoleRequest request, IInstructorRoleService roleService, CancellationToken cancellationToken) =>
        {
            var input = new UpdateInstructorRoleInput(id, request.RoleName);
            var response = await roleService.UpdateInstructorRoleAsync(input, cancellationToken);
            if (!response.Success)
                return Results.BadRequest(response);
            return Results.Ok(response);
        }).WithName("UpdateInstructorRole");

        app.MapDelete("/api/instructor-roles/{id:int}", async (int id, IInstructorRoleService roleService, CancellationToken cancellationToken) =>
        {
            var response = await roleService.DeleteInstructorRoleAsync(id, cancellationToken);
            if (!response.Success)
                return Results.BadRequest(response);
            return Results.Ok(response);
        }).WithName("DeleteInstructorRole");

        app.MapGet("/api/course-registration-statuses", async (ICourseRegistrationStatusService statusService, CancellationToken cancellationToken) =>
        {
            var response = await statusService.GetAllCourseRegistrationStatusesAsync(cancellationToken);
            if (!response.Success)
                return Results.BadRequest(response);
            return Results.Ok(response);
        }).WithName("GetCourseRegistrationStatuses");

        app.MapGet("/api/course-registration-statuses/{id:int}", async (int id, ICourseRegistrationStatusService statusService, CancellationToken cancellationToken) =>
        {
            var response = await statusService.GetCourseRegistrationStatusByIdAsync(id, cancellationToken);
            if (!response.Success)
                return Results.BadRequest(response);
            return Results.Ok(response);
        }).WithName("GetCourseRegistrationStatusById");

        app.MapPost("/api/course-registration-statuses", async (CreateCourseRegistrationStatusInput input, ICourseRegistrationStatusService statusService, CancellationToken cancellationToken) =>
        {
            var response = await statusService.CreateCourseRegistrationStatusAsync(input, cancellationToken);
            if (!response.Success)
                return Results.BadRequest(response);
            return Results.Created($"/api/course-registration-statuses/{response.Result?.Id}", response);
        }).WithName("CreateCourseRegistrationStatus");

        app.MapPut("/api/course-registration-statuses/{id:int}", async (int id, UpdateCourseRegistrationStatusInput input, ICourseRegistrationStatusService statusService, CancellationToken cancellationToken) =>
        {
            var updateInput = new UpdateCourseRegistrationStatusInput(id, input.Name);
            var response = await statusService.UpdateCourseRegistrationStatusAsync(updateInput, cancellationToken);
            if (!response.Success)
                return Results.BadRequest(response);
            return Results.Ok(response);
        }).WithName("UpdateCourseRegistrationStatus");

        app.MapDelete("/api/course-registration-statuses/{id:int}", async (int id, ICourseRegistrationStatusService statusService, CancellationToken cancellationToken) =>
        {
            var response = await statusService.DeleteCourseRegistrationStatusAsync(id, cancellationToken);
            if (!response.Success)
                return Results.BadRequest(response);
            return Results.Ok(response);
        }).WithName("DeleteCourseRegistrationStatus");

        app.Run();
    }
}
