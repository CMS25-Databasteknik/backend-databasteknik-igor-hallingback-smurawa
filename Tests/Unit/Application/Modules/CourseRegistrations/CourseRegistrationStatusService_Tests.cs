using Backend.Application.Modules.CourseRegistrations;
using Backend.Application.Modules.CourseRegistrations.Inputs;
using Backend.Application.Modules.CourseRegistrations.Outputs;
using Backend.Domain.Modules.CourseRegistrations.Contracts;
using Backend.Domain.Modules.CourseRegistrations.Models;
using NSubstitute;

namespace Backend.Tests.Unit.Application.Modules.CourseRegistrations;

public class CourseRegistrationStatusService_Tests
{
    private static CourseRegistrationStatusService CreateService(out ICourseRegistrationStatusRepository repo)
    {
        repo = Substitute.For<ICourseRegistrationStatusRepository>();
        return new CourseRegistrationStatusService(repo);
    }

    [Fact]
    public async Task GetAll_Should_Return_Success_With_Data()
    {
        var service = CreateService(out var repo);
        repo.GetAllCourseRegistrationStatusesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<CourseRegistrationStatus> { new(1, "Paid"), new(0, "Pending") });

        CourseRegistrationStatusListResult result = await service.GetAllCourseRegistrationStatusesAsync();

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(2, result.Result?.Count());
        Assert.Equal("Retrieved 2 course registration status(es) successfully.", result.Message);
        await repo.Received(1).GetAllCourseRegistrationStatusesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAll_Should_Return_Success_NoData()
    {
        var service = CreateService(out var repo);
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
        var service = CreateService(out var repo);
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
        var service = CreateService(out var _);

        var result = await service.GetCourseRegistrationStatusByIdAsync(-1);

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Id must be zero or positive", result.Message);
    }

    [Fact]
    public async Task GetById_Should_Return_404_When_NotFound()
    {
        var service = CreateService(out var repo);
        repo.GetCourseRegistrationStatusByIdAsync(5, Arg.Any<CancellationToken>()).Returns((CourseRegistrationStatus?)null);

        var result = await service.GetCourseRegistrationStatusByIdAsync(5);

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetById_Should_Return_Status_When_Found()
    {
        var service = CreateService(out var repo);
        repo.GetCourseRegistrationStatusByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new CourseRegistrationStatus(1, "Paid"));

        var result = await service.GetCourseRegistrationStatusByIdAsync(1);

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(1, result.Result.Id);
        Assert.Equal("Paid", result.Result.Name);
    }

    [Fact]
    public async Task Delete_Should_Return_400_For_Negative_Id()
    {
        var service = CreateService(out _);

        var result = await service.DeleteCourseRegistrationStatusAsync(-2);

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_404_When_NotFound()
    {
        var service = CreateService(out var repo);
        repo.GetCourseRegistrationStatusByIdAsync(3, Arg.Any<CancellationToken>()).Returns((CourseRegistrationStatus?)null);

        var result = await service.DeleteCourseRegistrationStatusAsync(3);

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_409_When_InUse()
    {
        var service = CreateService(out var repo);
        repo.GetCourseRegistrationStatusByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new CourseRegistrationStatus(1, "Paid"));
        repo.IsInUseAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        var result = await service.DeleteCourseRegistrationStatusAsync(1);

        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_200_For_Valid_NotInUse()
    {
        var service = CreateService(out var repo);
        repo.GetCourseRegistrationStatusByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new CourseRegistrationStatus(1, "Paid"));
        repo.IsInUseAsync(1, Arg.Any<CancellationToken>()).Returns(false);
        repo.DeleteCourseRegistrationStatusAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        var result = await service.DeleteCourseRegistrationStatusAsync(1);

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.True(result.Result);
    }

    [Fact]
    public async Task Create_Should_Return_400_When_Name_Empty()
    {
        var service = CreateService(out _);

        var result = await service.CreateCourseRegistrationStatusAsync(new CreateCourseRegistrationStatusInput("   "));

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Create_Should_Return_201_When_Name_Valid()
    {
        var service = CreateService(out _);
        var repo = Substitute.For<ICourseRegistrationStatusRepository>();
        repo.CreateCourseRegistrationStatusAsync(Arg.Any<CourseRegistrationStatus>(), Arg.Any<CancellationToken>())
            .Returns(new CourseRegistrationStatus(4, "New"));
        service = new CourseRegistrationStatusService(repo);

        var result = await service.CreateCourseRegistrationStatusAsync(new CreateCourseRegistrationStatusInput("New"));

        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(4, result.Result.Id);
    }

    [Fact]
    public async Task Update_Should_Return_400_When_Name_Empty()
    {
        var repo = Substitute.For<ICourseRegistrationStatusRepository>();
        repo.GetCourseRegistrationStatusByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new CourseRegistrationStatus(1, "Paid"));
        var service = new CourseRegistrationStatusService(repo);

        var result = await service.UpdateCourseRegistrationStatusAsync(new UpdateCourseRegistrationStatusInput(1, " "));

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Update_Should_Return_200_When_Valid()
    {
        var repo = Substitute.For<ICourseRegistrationStatusRepository>();
        repo.GetCourseRegistrationStatusByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new CourseRegistrationStatus(1, "Paid"));
        repo.UpdateCourseRegistrationStatusAsync(Arg.Any<CourseRegistrationStatus>(), Arg.Any<CancellationToken>())
            .Returns(new CourseRegistrationStatus(1, "Paid Updated"));
        var service = new CourseRegistrationStatusService(repo);

        var result = await service.UpdateCourseRegistrationStatusAsync(new UpdateCourseRegistrationStatusInput(1, "Paid"));

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal("Paid Updated", result.Result.Name);
    }
}
