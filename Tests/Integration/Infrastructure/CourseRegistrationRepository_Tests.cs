using Backend.Domain.Modules.CourseRegistrations.Models;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using Backend.Domain.Modules.PaymentMethod.Models;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Infrastructure;

[Collection(SqliteInMemoryCollection.Name)]
public class CourseRegistrationRepository_Tests(SqliteInMemoryFixture fixture)
{
    [Fact]
    public async Task CreateCourseRegistrationAsync_ShouldPersist_And_BeReadableById()
    {
        await using var context = fixture.CreateDbContext();
        var participant = await RepositoryTestDataHelper.CreateParticipantAsync(context);
        var courseEvent = await RepositoryTestDataHelper.CreateCourseEventAsync(context);
        var repo = new CourseRegistrationRepository(context);

        var input = new CourseRegistration(
            Guid.NewGuid(),
            participant.Id,
            courseEvent.Id,
            DateTime.UtcNow,
            CourseRegistrationStatus.Pending,
            PaymentMethod.Card);

        var created = await repo.AddAsync(input, CancellationToken.None);
        var byId = await repo.GetByIdAsync(created.Id, CancellationToken.None);

        Assert.NotNull(byId);
        Assert.Equal(input.Id, created.Id);
        Assert.Equal(participant.Id, byId!.ParticipantId);
        Assert.Equal(courseEvent.Id, byId.CourseEventId);
        Assert.Equal(CourseRegistrationStatus.Pending.Id, byId.Status.Id);
        Assert.Equal(PaymentMethod.Card, byId.PaymentMethod);

        var persisted = await context.CourseRegistrations
            .AsNoTracking()
            .SingleAsync(x => x.Id == created.Id, CancellationToken.None);

        Assert.Equal(input.Id, persisted.Id);
        Assert.Equal(participant.Id, persisted.ParticipantId);
        Assert.Equal(courseEvent.Id, persisted.CourseEventId);
        Assert.Equal(CourseRegistrationStatus.Pending.Id, persisted.CourseRegistrationStatusId);
        Assert.Equal((int)PaymentMethod.Card, persisted.PaymentMethodId);
    }

    [Fact]
    public async Task CreateRegistrationWithSeatCheckAsync_ShouldReturnNullWhenNoSeats()
    {
        await using var context = fixture.CreateDbContext();
        var participant = await RepositoryTestDataHelper.CreateParticipantAsync(context);
        var courseEvent = await RepositoryTestDataHelper.CreateCourseEventAsync(context, seats: 1);
        await RepositoryTestDataHelper.CreateCourseRegistrationAsync(context, participantId: participant.Id, courseEventId: courseEvent.Id);
        var secondParticipant = await RepositoryTestDataHelper.CreateParticipantAsync(context);
        var repo = new CourseRegistrationRepository(context);

        var second = await repo.CreateRegistrationWithSeatCheckAsync(
            new CourseRegistration(
                Guid.NewGuid(),
                secondParticipant.Id,
                courseEvent.Id,
                DateTime.UtcNow,
                CourseRegistrationStatus.Pending,
                PaymentMethod.Card),
            CancellationToken.None);

        Assert.Null(second);

        var registrationCount = await context.CourseRegistrations
            .AsNoTracking()
            .CountAsync(x => x.CourseEventId == courseEvent.Id, CancellationToken.None);

        Assert.Equal(1, registrationCount);
    }

    [Fact]
    public async Task GetAllCourseRegistrationsAsync_ShouldContainCreatedRegistration()
    {
        await using var context = fixture.CreateDbContext();
        var created = await RepositoryTestDataHelper.CreateCourseRegistrationAsync(context);
        var repo = new CourseRegistrationRepository(context);

        var all = await repo.GetAllAsync(CancellationToken.None);

        Assert.Contains(all, x => x.Id == created.Id);
    }

    [Fact]
    public async Task GetCourseRegistrationsByParticipantIdAsync_ShouldReturnParticipantRegistrations()
    {
        await using var context = fixture.CreateDbContext();
        var participant = await RepositoryTestDataHelper.CreateParticipantAsync(context);
        var created = await RepositoryTestDataHelper.CreateCourseRegistrationAsync(context, participantId: participant.Id);
        var repo = new CourseRegistrationRepository(context);

        var byParticipant = await repo.GetCourseRegistrationsByParticipantIdAsync(participant.Id, CancellationToken.None);

        Assert.Contains(byParticipant, x => x.Id == created.Id);
    }

    [Fact]
    public async Task GetCourseRegistrationsByCourseEventIdAsync_ShouldReturnEventRegistrations()
    {
        await using var context = fixture.CreateDbContext();
        var courseEvent = await RepositoryTestDataHelper.CreateCourseEventAsync(context);
        var created = await RepositoryTestDataHelper.CreateCourseRegistrationAsync(context, courseEventId: courseEvent.Id);
        var repo = new CourseRegistrationRepository(context);

        var byCourseEvent = await repo.GetCourseRegistrationsByCourseEventIdAsync(courseEvent.Id, CancellationToken.None);

        Assert.Contains(byCourseEvent, x => x.Id == created.Id);
    }

    [Fact]
    public async Task UpdateCourseRegistrationAsync_ShouldPersistChanges()
    {
        await using var context = fixture.CreateDbContext();
        var participant = await RepositoryTestDataHelper.CreateParticipantAsync(context);
        var courseEvent = await RepositoryTestDataHelper.CreateCourseEventAsync(context);
        var created = await RepositoryTestDataHelper.CreateCourseRegistrationAsync(context, participant.Id, courseEvent.Id);
        var repo = new CourseRegistrationRepository(context);

        var updated = await repo.UpdateAsync(
            created.Id,
            new CourseRegistration(
                created.Id,
                created.ParticipantId,
                created.CourseEventId,
                created.RegistrationDate,
                CourseRegistrationStatus.Paid,
                PaymentMethod.Invoice),
            CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal(CourseRegistrationStatus.Paid.Id, updated!.Status.Id);

        var persisted = await context.CourseRegistrations
            .AsNoTracking()
            .SingleAsync(x => x.Id == created.Id, CancellationToken.None);

        Assert.Equal(CourseRegistrationStatus.Paid.Id, persisted.CourseRegistrationStatusId);
        Assert.Equal((int)PaymentMethod.Invoice, persisted.PaymentMethodId);
    }

    [Fact]
    public async Task DeleteCourseRegistrationAsync_ShouldRemoveRegistration()
    {
        await using var context = fixture.CreateDbContext();
        var created = await RepositoryTestDataHelper.CreateCourseRegistrationAsync(context);
        var repo = new CourseRegistrationRepository(context);

        var deleted = await repo.RemoveAsync(created.Id, CancellationToken.None);
        var loaded = await repo.GetByIdAsync(created.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Null(loaded);
    }
}
