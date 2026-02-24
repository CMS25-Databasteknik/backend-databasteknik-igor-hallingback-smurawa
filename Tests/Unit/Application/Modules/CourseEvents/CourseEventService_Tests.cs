using Backend.Application.Modules.CourseEvents;
using Backend.Application.Modules.CourseEvents.Inputs;
using Backend.Domain.Modules.CourseEvents.Contracts;
using Backend.Domain.Modules.CourseEvents.Models;
using Backend.Domain.Modules.CourseEventTypes.Contracts;
using Backend.Domain.Modules.CourseEventTypes.Models;
using Backend.Domain.Modules.Courses.Contracts;
using Backend.Domain.Modules.Courses.Models;
using Backend.Domain.Modules.VenueTypes.Models;
using NSubstitute;

namespace Backend.Tests.Unit.Application.Modules.CourseEvents;

public class CourseEventService_Tests
{
    private static CourseEventService CreateService(
        ICourseEventRepository? courseEventRepository = null,
        ICourseRepository? courseRepository = null,
        ICourseEventTypeRepository? courseEventTypeRepository = null)
    {
        var eventRepo = courseEventRepository ?? Substitute.For<ICourseEventRepository>();
        var cRepo = courseRepository ?? Substitute.For<ICourseRepository>();
        var typeRepo = courseEventTypeRepository ?? Substitute.For<ICourseEventTypeRepository>();

        if (courseRepository is null)
        {
            cRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    var id = ci.Arg<Guid>();
                    return id == Guid.Empty
                        ? Task.FromResult<Course?>(null)
                        : Task.FromResult<Course?>(new Course(id, "Title", "Desc", 1));
                });
        }

        if (courseEventTypeRepository is null)
        {
            typeRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
                .Returns(ci =>
                {
                    var id = ci.Arg<int>();
                    return id <= 0 ? null : new CourseEventType(id, "Online");
                });
        }

        return new CourseEventService(eventRepo, cRepo, typeRepo);
    }

    #region Create

    [Fact]
    public async Task Create_Should_Return_201_When_Valid()
    {
        var eventRepo = Substitute.For<ICourseEventRepository>();
        var courseId = Guid.NewGuid();
        var date = DateTime.UtcNow.AddDays(10);
        var created = new CourseEvent(Guid.NewGuid(), courseId, date, 100, 10, 1, new VenueType(1, "InPerson"));
        eventRepo.AddAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>()).Returns(created);

        var service = CreateService(eventRepo);
        var input = new CreateCourseEventInput(courseId, date, 100, 10, 1, new VenueType(1, "InPerson"));

        var result = await service.CreateCourseEventAsync(input);

        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.Equal(created, result.Result);
        await eventRepo.Received(1).AddAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_Should_Return_400_When_Input_Null()
    {
        var eventRepo = Substitute.For<ICourseEventRepository>();
        var service = CreateService(eventRepo);

        var result = await service.CreateCourseEventAsync(null!);

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        await eventRepo.DidNotReceive().AddAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_Should_Return_404_When_Course_Missing()
    {
        var eventRepo = Substitute.For<ICourseEventRepository>();
        var courseRepo = Substitute.For<ICourseRepository>();
        courseRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Course?)null);
        var typeRepo = Substitute.For<ICourseEventTypeRepository>();
        typeRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new CourseEventType(1, "Online"));

        var service = new CourseEventService(eventRepo, courseRepo, typeRepo);
        var input = new CreateCourseEventInput(Guid.NewGuid(), DateTime.UtcNow.AddDays(5), 100, 10, 1, new VenueType(1, "InPerson"));

        var result = await service.CreateCourseEventAsync(input);

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Course with ID", result.Message);
        await eventRepo.DidNotReceive().AddAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_Should_Return_404_When_EventType_Missing()
    {
        var eventRepo = Substitute.For<ICourseEventRepository>();
        var typeRepo = Substitute.For<ICourseEventTypeRepository>();
        typeRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((CourseEventType?)null);
        var courseRepo = Substitute.For<ICourseRepository>();
        courseRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new Course(Guid.NewGuid(), "Title", "Desc", 1));

        var service = new CourseEventService(eventRepo, courseRepo, typeRepo);
        var input = new CreateCourseEventInput(Guid.NewGuid(), DateTime.UtcNow.AddDays(5), 100, 10, 99, new VenueType(1, "InPerson"));

        var result = await service.CreateCourseEventAsync(input);

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Course event type with ID", result.Message);
        await eventRepo.DidNotReceive().AddAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_Should_Return_400_When_Domain_Validation_Fails()
    {
        var eventRepo = Substitute.For<ICourseEventRepository>();
        var service = CreateService(eventRepo);
        var input = new CreateCourseEventInput(Guid.NewGuid(), DateTime.UtcNow.AddDays(1), -1, 10, 1, new VenueType(1, "InPerson"));

        var result = await service.CreateCourseEventAsync(input);

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Price cannot be negative", result.Message);
        await eventRepo.DidNotReceive().AddAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_Should_Return_500_When_Repository_Throws()
    {
        var eventRepo = Substitute.For<ICourseEventRepository>();
        eventRepo.AddAsync(Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CourseEvent>(new Exception("db fail")));

        var service = CreateService(eventRepo);
        var input = new CreateCourseEventInput(Guid.NewGuid(), DateTime.UtcNow.AddDays(1), 100, 10, 1, new VenueType(1, "InPerson"));

        var result = await service.CreateCourseEventAsync(input);

        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("db fail", result.Message);
    }

    #endregion

    #region GetAll / GetById / GetByCourseId

    [Fact]
    public async Task GetAll_Should_Return_Events()
    {
        var eventRepo = Substitute.For<ICourseEventRepository>();
        var events = new List<CourseEvent>
        {
            new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), 10, 5, 1, new VenueType(1, "InPerson")),
            new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(2), 20, 10, 1, new VenueType(1, "InPerson"))
        };
        eventRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns(events);

        var service = CreateService(eventRepo);

        var result = await service.GetAllCourseEventsAsync();

        Assert.True(result.Success);
        Assert.Equal(2, result.Result.Count());
    }

    [Fact]
    public async Task GetById_Should_Return_400_When_Empty_Id()
    {
        var service = CreateService();
        var result = await service.GetCourseEventByIdAsync(Guid.Empty);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task GetByCourseId_Should_Return_400_When_Empty()
    {
        var service = CreateService();
        var result = await service.GetCourseEventsByCourseIdAsync(Guid.Empty);
        Assert.Equal(400, result.StatusCode);
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_Should_Return_200_When_Valid()
    {
        var eventRepo = Substitute.For<ICourseEventRepository>();
        var eventId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var existing = new CourseEvent(eventId, courseId, DateTime.UtcNow.AddDays(1), 10, 5, 1, new VenueType(1, "InPerson"));
        var updated = new CourseEvent(eventId, courseId, DateTime.UtcNow.AddDays(2), 20, 8, 2, new VenueType(1, "InPerson"));

        eventRepo.GetByIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(existing);
        eventRepo.UpdateAsync(Arg.Any<Guid>(), Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>()).Returns(updated);

        var service = CreateService(eventRepo);
        var input = new UpdateCourseEventInput(eventId, courseId, updated.EventDate, updated.Price, updated.Seats, updated.CourseEventTypeId, new VenueType(1, "InPerson"));

        var result = await service.UpdateCourseEventAsync(input);

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(updated.EventDate, result.Result!.EventDate);
    }

    [Fact]
    public async Task Update_Should_Return_404_When_Event_Not_Found()
    {
        var eventRepo = Substitute.For<ICourseEventRepository>();
        eventRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((CourseEvent?)null);

        var service = CreateService(eventRepo);
        var input = new UpdateCourseEventInput(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), 10, 5, 1, new VenueType(1, "InPerson"));

        var result = await service.UpdateCourseEventAsync(input);

        Assert.Equal(404, result.StatusCode);
        await eventRepo.DidNotReceive().UpdateAsync(Arg.Any<Guid>(), Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_Should_Return_404_When_Course_Missing()
    {
        var eventRepo = Substitute.For<ICourseEventRepository>();
        var courseRepo = Substitute.For<ICourseRepository>();
        courseRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Course?)null);
        eventRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), 10, 5, 1, new VenueType(1, "InPerson")));

        var typeRepo = Substitute.For<ICourseEventTypeRepository>();
        typeRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new CourseEventType(1, "Online"));

        var service = new CourseEventService(eventRepo, courseRepo, typeRepo);
        var input = new UpdateCourseEventInput(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(2), 20, 8, 1, new VenueType(1, "InPerson"));

        var result = await service.UpdateCourseEventAsync(input);

        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task Update_Should_Return_404_When_EventType_Missing()
    {
        var eventRepo = Substitute.For<ICourseEventRepository>();
        var typeRepo = Substitute.For<ICourseEventTypeRepository>();
        typeRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((CourseEventType?)null);

        eventRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new CourseEvent(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), 10, 5, 1, new VenueType(1, "InPerson")));

        var courseRepo = Substitute.For<ICourseRepository>();
        courseRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new Course(Guid.NewGuid(), "Title", "Desc", 1));

        var service = new CourseEventService(eventRepo, courseRepo, typeRepo);
        var input = new UpdateCourseEventInput(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(2), 20, 8, 2, new VenueType(1, "InPerson"));

        var result = await service.UpdateCourseEventAsync(input);

        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task Update_Should_Return_400_When_Domain_Invalid()
    {
        var eventRepo = Substitute.For<ICourseEventRepository>();
        var courseId = Guid.NewGuid();
        eventRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new CourseEvent(Guid.NewGuid(), courseId, DateTime.UtcNow.AddDays(1), 10, 5, 1, new VenueType(1, "InPerson")));

        var service = CreateService(eventRepo);
        var input = new UpdateCourseEventInput(Guid.NewGuid(), courseId, DateTime.UtcNow.AddDays(1), -1, 5, 1, new VenueType(1, "InPerson"));

        var result = await service.UpdateCourseEventAsync(input);

        Assert.Equal(400, result.StatusCode);
        await eventRepo.DidNotReceive().UpdateAsync(Arg.Any<Guid>(), Arg.Any<CourseEvent>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Delete

    [Fact]
    public async Task Delete_Should_Return_400_When_Id_Empty()
    {
        var service = CreateService();
        var result = await service.DeleteCourseEventAsync(Guid.Empty);
        Assert.Equal(400, result.StatusCode);
    }

    #endregion
}
