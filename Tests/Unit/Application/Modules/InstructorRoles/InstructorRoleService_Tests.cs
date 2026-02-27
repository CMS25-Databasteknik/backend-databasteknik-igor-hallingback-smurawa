using Backend.Application.Common;
using Backend.Application.Modules.InstructorRoles;
using Backend.Application.Modules.InstructorRoles.Caching;
using Backend.Application.Modules.InstructorRoles.Inputs;
using Backend.Domain.Modules.InstructorRoles.Contracts;
using Backend.Domain.Modules.InstructorRoles.Models;
using NSubstitute;

namespace Backend.Tests.Unit.Application.Modules.InstructorRoles;

public class InstructorRoleService_Tests
{
    private static IInstructorRoleCache CreateCache()
    {
        var cache = Substitute.For<IInstructorRoleCache>();
        cache.GetAllAsync(Arg.Any<Func<CancellationToken, Task<IReadOnlyList<InstructorRole>>>>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Func<CancellationToken, Task<IReadOnlyList<InstructorRole>>>>()(ci.Arg<CancellationToken>()));
        cache.GetByIdAsync(Arg.Any<int>(), Arg.Any<Func<CancellationToken, Task<InstructorRole?>>>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Func<CancellationToken, Task<InstructorRole?>>>()(ci.Arg<CancellationToken>()));
        return cache;
    }

    private static IInstructorRoleRepository CreateRepo()
    {
        var repo = Substitute.For<IInstructorRoleRepository>();
        repo.AddAsync(Arg.Any<InstructorRole>(), Arg.Any<CancellationToken>())
            .Returns(ci => new InstructorRole(1, ci.Arg<InstructorRole>().RoleName));
        repo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<int>() == 9 ? null : new InstructorRole(ci.Arg<int>(), $"Role{ci.Arg<int>()}"));
        repo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<InstructorRole>>(new List<InstructorRole> { new(1, "Lead") }));
        repo.UpdateAsync(Arg.Any<int>(), Arg.Any<InstructorRole>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<InstructorRole>().Id == 9 ? null : ci.Arg<InstructorRole>());
        repo.RemoveAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<int>() != 9);
        return repo;
    }

    [Fact]
    public async Task Create_Should_Return_201_When_Valid()
    {
        var cache = CreateCache();
        var repo = CreateRepo();
        var service = new InstructorRoleService(cache, repo);

        var result = await service.CreateInstructorRoleAsync(new CreateInstructorRoleInput("Lead"));

        Assert.True(result.Success);
        Assert.Equal(ErrorTypes.None, result.ErrorType);
        Assert.Equal("Lead", result.Result?.RoleName);
    }

    [Fact]
    public async Task Create_Should_Return_400_When_Name_Invalid()
    {
        var cache = CreateCache();
        var repo = CreateRepo();
        var service = new InstructorRoleService(cache, repo);

        var result = await service.CreateInstructorRoleAsync(new CreateInstructorRoleInput("   "));

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.Validation, result.ErrorType);
    }

    [Fact]
    public async Task GetById_Should_Return_404_When_Not_Found()
    {
        var cache = CreateCache();
        var repo = CreateRepo();
        var service = new InstructorRoleService(cache, repo);

        var result = await service.GetInstructorRoleByIdAsync(9);

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Update_Should_Return_404_When_Not_Found()
    {
        var cache = CreateCache();
        var repo = CreateRepo();
        var service = new InstructorRoleService(cache, repo);

        var result = await service.UpdateInstructorRoleAsync(new UpdateInstructorRoleInput(9, "NewName"));

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Delete_Should_Return_200_When_Deleted()
    {
        var cache = CreateCache();
        var repo = CreateRepo();
        var service = new InstructorRoleService(cache, repo);

        var result = await service.DeleteInstructorRoleAsync(1);

        Assert.True(result.Success);
        Assert.True(result.Result);
        Assert.Equal(ErrorTypes.None, result.ErrorType);
    }

    [Fact]
    public async Task Delete_Should_Return_404_When_NotFound()
    {
        var cache = CreateCache();
        var repo = CreateRepo();
        var service = new InstructorRoleService(cache, repo);

        var result = await service.DeleteInstructorRoleAsync(9);

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetById_Should_Return_From_Cache_Without_Repo()
    {
        var cache = CreateCache();
        var repo = CreateRepo();
        var cached = new InstructorRole(11, "CachedRole");
        cache.GetByIdAsync(11, Arg.Any<Func<CancellationToken, Task<InstructorRole?>>>(), Arg.Any<CancellationToken>())
            .Returns(cached);
        var service = new InstructorRoleService(cache, repo);

        var result = await service.GetInstructorRoleByIdAsync(11);

        Assert.True(result.Success);
        Assert.Equal(cached, result.Result);
        await repo.DidNotReceive().GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_Should_Reset_And_Set_Cache()
    {
        var cache = CreateCache();
        var repo = CreateRepo();
        var existing = new InstructorRole(2, "Lead");
        var updated = new InstructorRole(2, "Senior Lead");
        repo.GetByIdAsync(existing.Id, Arg.Any<CancellationToken>()).Returns(existing);
        repo.UpdateAsync(existing.Id, Arg.Any<InstructorRole>(), Arg.Any<CancellationToken>()).Returns(updated);
        var service = new InstructorRoleService(cache, repo);

        var result = await service.UpdateInstructorRoleAsync(new UpdateInstructorRoleInput(existing.Id, "Senior Lead"));

        Assert.True(result.Success);
        Assert.Equal(updated, result.Result);
        cache.Received(1).ResetEntity(existing);
        cache.Received(1).SetEntity(updated);
    }
}

