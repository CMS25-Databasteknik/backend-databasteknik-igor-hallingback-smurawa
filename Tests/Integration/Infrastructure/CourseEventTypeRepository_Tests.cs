using Backend.Domain.Modules.CourseEventTypes.Models;
using Backend.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration.Infrastructure;

[Collection(SqliteInMemoryCollection.Name)]
public class CourseEventTypeRepository_Tests(SqliteInMemoryFixture fixture)
{
    [Fact]
    public async Task CreateCourseEventTypeAsync_ShouldPersist_And_BeReadableByIdAndName()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseEventTypeRepository(context);
        var typeName = $"Lecture-{Guid.NewGuid():N}";

        var created = await repo.CreateCourseEventTypeAsync(new CourseEventType(typeName), CancellationToken.None);
        var byId = await repo.GetCourseEventTypeByIdAsync(created.Id, CancellationToken.None);
        var byName = await repo.GetCourseEventTypeByTypeNameAsync(typeName, CancellationToken.None);

        Assert.True(created.Id > 0);
        Assert.NotNull(byId);
        Assert.NotNull(byName);
        Assert.Equal(typeName, created.TypeName);
        Assert.Equal(created.Id, byId!.Id);
        Assert.Equal(typeName, byId.TypeName);
        Assert.Equal(created.Id, byName!.Id);
        Assert.Equal(typeName, byName.TypeName);

        var persisted = await context.CourseEventTypes
            .AsNoTracking()
            .SingleAsync(x => x.Id == created.Id, CancellationToken.None);

        Assert.Equal(created.Id, persisted.Id);
        Assert.Equal(typeName, persisted.TypeName);
    }

    [Fact]
    public async Task GetAllCourseEventTypesAsync_ShouldIncludeCreatedType()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseEventTypeRepository(context);
        var created = await repo.CreateCourseEventTypeAsync(new CourseEventType($"Type-{Guid.NewGuid():N}"), CancellationToken.None);

        var all = await repo.GetAllCourseEventTypesAsync(CancellationToken.None);

        Assert.Contains(all, x => x.Id == created.Id);
    }

    [Fact]
    public async Task UpdateCourseEventTypeAsync_ShouldPersistNewTypeName()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseEventTypeRepository(context);
        var created = await repo.CreateCourseEventTypeAsync(new CourseEventType($"Type-{Guid.NewGuid():N}"), CancellationToken.None);

        var updated = await repo.UpdateCourseEventTypeAsync(new CourseEventType(created.Id, "UpdatedType"), CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal("UpdatedType", updated!.TypeName);

        var persisted = await context.CourseEventTypes
            .AsNoTracking()
            .SingleAsync(x => x.Id == created.Id, CancellationToken.None);

        Assert.Equal("UpdatedType", persisted.TypeName);
    }

    [Fact]
    public async Task IsInUseAsync_ShouldReturnTrueWhenReferencedByCourseEvent()
    {
        await using var context = fixture.CreateDbContext();
        var type = await RepositoryTestDataHelper.CreateCourseEventTypeAsync(context);
        await RepositoryTestDataHelper.CreateCourseEventAsync(context, typeId: type.Id);
        var repo = new CourseEventTypeRepository(context);

        var inUse = await repo.IsInUseAsync(type.Id, CancellationToken.None);

        Assert.True(inUse);
    }

    [Fact]
    public async Task DeleteCourseEventTypeAsync_ShouldRemoveType()
    {
        await using var context = fixture.CreateDbContext();
        var repo = new CourseEventTypeRepository(context);
        var created = await repo.CreateCourseEventTypeAsync(new CourseEventType($"Type-{Guid.NewGuid():N}"), CancellationToken.None);

        var deleted = await repo.DeleteCourseEventTypeAsync(created.Id, CancellationToken.None);
        var loaded = await repo.GetCourseEventTypeByIdAsync(created.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Null(loaded);
    }
}
