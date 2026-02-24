using Backend.Domain.Modules.CourseRegistrations.Models;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using PaymentMethodModel = Backend.Domain.Modules.PaymentMethod.Models.PaymentMethod;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Infrastructure;

[Collection(SqliteInMemoryCollection.Name)]
public class PaymentMethodRepository_Tests(SqliteInMemoryFixture fixture)
{
    [Fact]
    public async Task CreatePaymentMethodAsync_ShouldPersist_And_BeReadableByIdAndName()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new PaymentMethodRepository(context);
        var name = $"Method-{Guid.NewGuid():N}";

        var created = await repo.AddAsync(new PaymentMethodModel(0, name), CancellationToken.None);
        var byId = await repo.GetByIdAsync(created.Id, CancellationToken.None);
        var byName = await repo.GetByNameAsync(name, CancellationToken.None);

        Assert.NotNull(byId);
        Assert.NotNull(byName);
        Assert.Equal(name, byId!.Name);
        Assert.Equal(created.Id, byName!.Id);

        var persisted = await context.PaymentMethods
            .AsNoTracking()
            .SingleAsync(x => x.Id == created.Id, CancellationToken.None);

        Assert.Equal(name, persisted.Name);
    }

    [Fact]
    public async Task IsInUseAsync_ShouldReturnTrue_WhenReferencedByCourseRegistration()
    {
        await using var context = fixture.CreateDbContext();
        var paymentMethodRepo = new PaymentMethodRepository(context);
        var paymentMethod = await paymentMethodRepo.AddAsync(
            new PaymentMethodModel(0, $"Method-{Guid.NewGuid():N}"),
            CancellationToken.None);
        var participant = await RepositoryTestDataHelper.CreateParticipantAsync(context);
        var courseEvent = await RepositoryTestDataHelper.CreateCourseEventAsync(context);
        var registrationRepo = new CourseRegistrationRepository(context);

        await registrationRepo.AddAsync(
            new CourseRegistration(
                Guid.NewGuid(),
                participant.Id,
                courseEvent.Id,
                DateTime.UtcNow,
                CourseRegistrationStatus.Pending,
                new PaymentMethodModel(paymentMethod.Id, paymentMethod.Name)),
            CancellationToken.None);

        var inUse = await paymentMethodRepo.IsInUseAsync(paymentMethod.Id, CancellationToken.None);

        Assert.True(inUse);
    }
}
