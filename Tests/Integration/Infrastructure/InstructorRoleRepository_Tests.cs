using Backend.Domain.Modules.InstructorRoles.Models;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests.Integration.Infrastructure;

[Collection(SqliteInMemoryCollection.Name)]
public class InstructorRoleRepository_Tests(SqliteInMemoryFixture fixture)
{
    [Fact]
    public async Task CreateInstructorRoleAsync_ShouldPersist_And_BeReadableById()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new InstructorRoleRepository(context);
        var roleName = $"Trainer-{Guid.NewGuid():N}";

        var created = await repo.AddAsync(new InstructorRole(roleName), CancellationToken.None);
        var loaded = await repo.GetByIdAsync(created.Id, CancellationToken.None);

        Assert.NotNull(loaded);
        Assert.Equal(created.Id, loaded!.Id);
        Assert.Equal(roleName, loaded!.RoleName);

        var persisted = await context.InstructorRoles
            .AsNoTracking()
            .SingleAsync(x => x.Id == created.Id, CancellationToken.None);

        Assert.Equal(created.Id, persisted.Id);
        Assert.Equal(roleName, persisted.RoleName);
    }

    [Fact]
    public async Task GetAllInstructorRolesAsync_ShouldContainCreatedRole()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new InstructorRoleRepository(context);
        var created = await repo.AddAsync(new InstructorRole($"Role-{Guid.NewGuid():N}"), CancellationToken.None);

        var all = await repo.GetAllAsync(CancellationToken.None);

        Assert.Contains(all, x => x.Id == created.Id);
    }

    [Fact]
    public async Task GetAllInstructorRolesAsync_ShouldReturnDescendingById()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new InstructorRoleRepository(context);
        var first = await repo.AddAsync(new InstructorRole($"RoleA-{Guid.NewGuid():N}"), CancellationToken.None);
        var second = await repo.AddAsync(new InstructorRole($"RoleB-{Guid.NewGuid():N}"), CancellationToken.None);

        var all = await repo.GetAllAsync(CancellationToken.None);
        var firstIndex = all.ToList().FindIndex(x => x.Id == first.Id);
        var secondIndex = all.ToList().FindIndex(x => x.Id == second.Id);

        Assert.True(firstIndex >= 0);
        Assert.True(secondIndex >= 0);
        Assert.True(secondIndex < firstIndex);
    }

    [Fact]
    public async Task UpdateInstructorRoleAsync_ShouldPersistChanges()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new InstructorRoleRepository(context);
        var created = await repo.AddAsync(new InstructorRole($"Role-{Guid.NewGuid():N}"), CancellationToken.None);

        var updated = await repo.UpdateAsync(created.Id, new InstructorRole(created.Id, "UpdatedRole"), CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal("UpdatedRole", updated!.RoleName);

        var persisted = await context.InstructorRoles
            .AsNoTracking()
            .SingleAsync(x => x.Id == created.Id, CancellationToken.None);

        Assert.Equal("UpdatedRole", persisted.RoleName);
    }

    [Fact]
    public async Task DeleteInstructorRoleAsync_ShouldRemoveRole()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new InstructorRoleRepository(context);
        var created = await repo.AddAsync(new InstructorRole($"Role-{Guid.NewGuid():N}"), CancellationToken.None);

        var deleted = await repo.RemoveAsync(created.Id, CancellationToken.None);
        var loaded = await repo.GetByIdAsync(created.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Null(loaded);
    }
}

