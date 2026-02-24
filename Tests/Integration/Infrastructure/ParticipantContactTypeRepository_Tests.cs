using Backend.Domain.Modules.ParticipantContactTypes.Models;
using Backend.Domain.Modules.Participants.Models;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Infrastructure;

[Collection(SqliteInMemoryCollection.Name)]
public class ParticipantContactTypeRepository_Tests(SqliteInMemoryFixture fixture)
{
    [Fact]
    public async Task CreateParticipantContactTypeAsync_ShouldPersist_And_BeReadableByIdAndName()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new ParticipantContactTypeRepository(context);
        var name = $"Contact-{Guid.NewGuid():N}";

        var created = await repo.AddAsync(new ParticipantContactType(1, name), CancellationToken.None);
        var byId = await repo.GetByIdAsync(created.Id, CancellationToken.None);
        var byName = await repo.GetByNameAsync(name, CancellationToken.None);

        Assert.NotNull(byId);
        Assert.NotNull(byName);
        Assert.Equal(name, byId!.Name);
        Assert.Equal(created.Id, byName!.Id);

        var persisted = await context.ParticipantContactTypes
            .AsNoTracking()
            .SingleAsync(x => x.Id == created.Id, CancellationToken.None);

        Assert.Equal(name, persisted.Name);
    }

    [Fact]
    public async Task IsInUseAsync_ShouldReturnTrue_WhenReferencedByParticipant()
    {
        await using var context = fixture.CreateDbContext();
        var contactTypeRepo = new ParticipantContactTypeRepository(context);
        var contactType = await contactTypeRepo.AddAsync(
            new ParticipantContactType(1, $"Contact-{Guid.NewGuid():N}"),
            CancellationToken.None);
        var participantRepo = new ParticipantRepository(context);

        await participantRepo.AddAsync(
            new Participant(
                Guid.NewGuid(),
                "Test",
                "User",
                $"user-{Guid.NewGuid():N}@example.com",
                "12345",
                new ParticipantContactType(contactType.Id, contactType.Name)),
            CancellationToken.None);

        var inUse = await contactTypeRepo.IsInUseAsync(contactType.Id, CancellationToken.None);

        Assert.True(inUse);
    }
}
