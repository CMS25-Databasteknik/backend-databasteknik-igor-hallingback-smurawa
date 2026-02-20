using Backend.Application.Modules.CourseRegistrationStatuses;
using Backend.Application.Modules.CourseRegistrationStatuses.Caching;
using Backend.Application.Modules.CourseRegistrationStatuses.Inputs;
using Backend.Application.Modules.CourseRegistrationStatuses.Outputs;
using Backend.Domain.Modules.CourseRegistrationStatuses.Contracts;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using NSubstitute;

namespace Backend.Tests.Unit.Application.Modules.CourseRegistrations;

public class CourseRegistrationStatusService_Tests
{
    private static CourseRegistrationStatusService CreateService(
        out ICourseRegistrationStatusRepository repo,
        out ICourseRegistrationStatusCache cache)
    {
        repo = Substitute.For<ICourseRegistrationStatusRepository>();
        cache = Substitute.For<ICourseRegistrationStatusCache>();

        cache.GetAllAsync(Arg.Any<Func<CancellationToken, Task<IReadOnlyList<CourseRegistrationStatus>>>>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Func<CancellationToken, Task<IReadOnlyList<CourseRegistrationStatus>>>>()(ci.Arg<CancellationToken>()));

        cache.GetByIdAsync(Arg.Any<int>(), Arg.Any<Func<CancellationToken, Task<CourseRegistrationStatus?>>>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Func<CancellationToken, Task<CourseRegistrationStatus?>>>()(ci.Arg<CancellationToken>()));

        return new CourseRegistrationStatusService(cache, repo);
    }

    [Fact]
    public async Task GetAll_Should_Return_Success_With_Data()
    {
        var service = CreateService(out var repo, out var cache);
        repo.GetAllCourseRegistrationStatusesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<CourseRegistrationStatus> { new(1, "Paid"), new(0, "Pending") });

        CourseRegistrationStatusListResult result = await service.GetAllCourseRegistrationStatusesAsync();

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(2, result.Result?.Count());
        Assert.Equal("Retrieved 2 course registration status(es) successfully.", result.Message);
        await repo.Received(1).GetAllCourseRegistrationStatusesAsync(Arg.Any<CancellationToken>());
        await cache.Received(1).GetAllAsync(
            Arg.Any<Func<CancellationToken, Task<IReadOnlyList<CourseRegistrationStatus>>>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAll_Should_Return_Success_NoData()
    {
        var service = CreateService(out var repo, out _);
        repo.GetAllCourseRegistrationStatusesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<CourseRegistrationStatus>());

        var result = await service.GetAllCourseRegistrationStatusesAsync();

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
        Assert.Equal("No course registration statuses found.", result.Message);
    }

    [Fact]
    public async Task GetAll_Should_Handle_Exception()
    {
        var service = CreateService(out var repo, out _);
        repo.GetAllCourseRegistrationStatusesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IReadOnlyList<CourseRegistrationStatus>>(new Exception("db failure")));

        var result = await service.GetAllCourseRegistrationStatusesAsync();

        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("db failure", result.Message);
    }

    [Fact]
    public async Task GetById_Should_Return_400_When_Id_Negative()
    {
        var service = CreateService(out _, out _);

        var result = await service.GetCourseRegistrationStatusByIdAsync(-1);

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Id must be zero or positive", result.Message);
    }

    [Fact]
    public async Task GetById_Should_Return_404_When_NotFound()
    {
        var service = CreateService(out var repo, out var cache);
        repo.GetCourseRegistrationStatusByIdAsync(5, Arg.Any<CancellationToken>()).Returns((CourseRegistrationStatus?)null);

        var result = await service.GetCourseRegistrationStatusByIdAsync(5);

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        await cache.Received(1).GetByIdAsync(
            5,
            Arg.Any<Func<CancellationToken, Task<CourseRegistrationStatus?>>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetById_Should_Return_Status_When_Found()
    {
        var service = CreateService(out var repo, out var cache);
        repo.GetCourseRegistrationStatusByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new CourseRegistrationStatus(1, "Paid"));

        var result = await service.GetCourseRegistrationStatusByIdAsync(1);

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(1, result.Result.Id);
        Assert.Equal("Paid", result.Result.Name);
        await cache.Received(1).GetByIdAsync(
            1,
            Arg.Any<Func<CancellationToken, Task<CourseRegistrationStatus?>>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByName_Should_Return_400_When_Name_Empty()
    {
        var service = CreateService(out _, out _);

        var result = await service.GetCourseRegistrationStatusByNameAsync(" ");

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Name is required", result.Message);
    }

    [Fact]
    public async Task GetByName_Should_Return_404_When_NotFound()
    {
        var service = CreateService(out var repo, out var cache);
        repo.GetCourseRegistrationStatusByNameAsync("Unknown", Arg.Any<CancellationToken>())
            .Returns((CourseRegistrationStatus?)null);

        var result = await service.GetCourseRegistrationStatusByNameAsync("Unknown");

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        await repo.Received(1).GetCourseRegistrationStatusByNameAsync("Unknown", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByName_Should_Return_Status_When_Found()
    {
        var service = CreateService(out var repo, out var cache);
        repo.GetCourseRegistrationStatusByNameAsync("Paid", Arg.Any<CancellationToken>())
            .Returns(new CourseRegistrationStatus(1, "Paid"));

        var result = await service.GetCourseRegistrationStatusByNameAsync("Paid");

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(1, result.Result.Id);
        Assert.Equal("Paid", result.Result.Name);
        await repo.Received(1).GetCourseRegistrationStatusByNameAsync("Paid", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_Should_Return_400_For_Negative_Id()
    {
        var service = CreateService(out _, out _);

        var result = await service.DeleteCourseRegistrationStatusAsync(-2);

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_404_When_NotFound()
    {
        var service = CreateService(out var repo, out var cache);
        repo.GetCourseRegistrationStatusByIdAsync(3, Arg.Any<CancellationToken>()).Returns((CourseRegistrationStatus?)null);

        var result = await service.DeleteCourseRegistrationStatusAsync(3);

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        cache.DidNotReceive().ResetEntity(Arg.Any<CourseRegistrationStatus>());
    }

    [Fact]
    public async Task Delete_Should_Return_409_When_InUse()
    {
        var service = CreateService(out var repo, out var cache);
        repo.GetCourseRegistrationStatusByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new CourseRegistrationStatus(1, "Paid"));
        repo.IsInUseAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        var result = await service.DeleteCourseRegistrationStatusAsync(1);

        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        cache.DidNotReceive().ResetEntity(Arg.Any<CourseRegistrationStatus>());
    }

    [Fact]
    public async Task Delete_Should_Return_200_For_Valid_NotInUse()
    {
        var service = CreateService(out var repo, out var cache);
        repo.GetCourseRegistrationStatusByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new CourseRegistrationStatus(1, "Paid"));
        repo.IsInUseAsync(1, Arg.Any<CancellationToken>()).Returns(false);
        repo.DeleteCourseRegistrationStatusAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        var result = await service.DeleteCourseRegistrationStatusAsync(1);

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.True(result.Result);
        cache.Received(1).ResetEntity(Arg.Is<CourseRegistrationStatus>(s => s.Id == 1));
    }

    [Fact]
    public async Task Create_Should_Return_400_When_Name_Empty()
    {
        var service = CreateService(out _, out _);

        var result = await service.CreateCourseRegistrationStatusAsync(new CreateCourseRegistrationStatusInput("   "));

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Create_Should_Return_201_When_Name_Valid()
    {
        var service = CreateService(out _, out var cache);
        var repo = Substitute.For<ICourseRegistrationStatusRepository>();
        repo.CreateCourseRegistrationStatusAsync(Arg.Any<CourseRegistrationStatus>(), Arg.Any<CancellationToken>())
            .Returns(new CourseRegistrationStatus(4, "New"));
        service = new CourseRegistrationStatusService(cache, repo);

        var result = await service.CreateCourseRegistrationStatusAsync(new CreateCourseRegistrationStatusInput("New"));

        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(4, result.Result.Id);
        cache.Received(1).ResetEntity(Arg.Is<CourseRegistrationStatus>(s => s.Id == 4));
        cache.Received(1).SetEntity(Arg.Is<CourseRegistrationStatus>(s => s.Id == 4));
    }

    [Fact]
    public async Task Update_Should_Return_400_When_Name_Empty()
    {
        var repo = Substitute.For<ICourseRegistrationStatusRepository>();
        var cache = Substitute.For<ICourseRegistrationStatusCache>();
        cache.GetByIdAsync(Arg.Any<int>(), Arg.Any<Func<CancellationToken, Task<CourseRegistrationStatus?>>>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Func<CancellationToken, Task<CourseRegistrationStatus?>>>()(ci.Arg<CancellationToken>()));
        repo.GetCourseRegistrationStatusByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new CourseRegistrationStatus(1, "Paid"));
        var service = new CourseRegistrationStatusService(cache, repo);

        var result = await service.UpdateCourseRegistrationStatusAsync(new UpdateCourseRegistrationStatusInput(1, " "));

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        cache.DidNotReceive().ResetEntity(Arg.Any<CourseRegistrationStatus>());
        cache.DidNotReceive().SetEntity(Arg.Any<CourseRegistrationStatus>());
    }

    [Fact]
    public async Task Update_Should_Return_200_When_Valid()
    {
        var repo = Substitute.For<ICourseRegistrationStatusRepository>();
        var cache = Substitute.For<ICourseRegistrationStatusCache>();
        cache.GetByIdAsync(Arg.Any<int>(), Arg.Any<Func<CancellationToken, Task<CourseRegistrationStatus?>>>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Func<CancellationToken, Task<CourseRegistrationStatus?>>>()(ci.Arg<CancellationToken>()));
        repo.GetCourseRegistrationStatusByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new CourseRegistrationStatus(1, "Paid"));
        repo.UpdateCourseRegistrationStatusAsync(Arg.Any<CourseRegistrationStatus>(), Arg.Any<CancellationToken>())
            .Returns(new CourseRegistrationStatus(1, "Paid Updated"));
        var service = new CourseRegistrationStatusService(cache, repo);

        var result = await service.UpdateCourseRegistrationStatusAsync(new UpdateCourseRegistrationStatusInput(1, "Paid"));

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("Paid Updated", result.Result.Name);
        cache.Received(1).ResetEntity(Arg.Is<CourseRegistrationStatus>(s => s.Id == 1));
        cache.Received(1).SetEntity(Arg.Is<CourseRegistrationStatus>(s => s.Id == 1 && s.Name == "Paid Updated"));
    }

    [Fact]
    public void Constructor_Should_Throw_When_Dependencies_Are_Null()
    {
        var cache = Substitute.For<ICourseRegistrationStatusCache>();
        var repo = Substitute.For<ICourseRegistrationStatusRepository>();

        Assert.Throws<ArgumentNullException>(() => new CourseRegistrationStatusService(null!, repo));
        Assert.Throws<ArgumentNullException>(() => new CourseRegistrationStatusService(cache, null!));
    }
}





