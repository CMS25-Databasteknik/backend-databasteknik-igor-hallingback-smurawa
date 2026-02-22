using Backend.Application.Modules.InstructorRoles;
using Backend.Application.Modules.InstructorRoles.Inputs;
using Backend.Domain.Modules.InstructorRoles.Contracts;
using Backend.Domain.Modules.InstructorRoles.Models;
using NSubstitute;

namespace Backend.Tests.Unit.Application.Modules.InstructorRoles;

public class InstructorRoleService_Tests
{
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
        var repo = CreateRepo();
        var service = new InstructorRoleService(repo);

        var result = await service.CreateInstructorRoleAsync(new CreateInstructorRoleInput("Lead"));

        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.Equal("Lead", result.Result?.RoleName);
    }

    [Fact]
    public async Task Create_Should_Return_400_When_Name_Invalid()
    {
        var repo = CreateRepo();
        var service = new InstructorRoleService(repo);

        var result = await service.CreateInstructorRoleAsync(new CreateInstructorRoleInput("   "));

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task GetById_Should_Return_404_When_Not_Found()
    {
        var repo = CreateRepo();
        var service = new InstructorRoleService(repo);

        var result = await service.GetInstructorRoleByIdAsync(9);

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task Update_Should_Return_404_When_Not_Found()
    {
        var repo = CreateRepo();
        var service = new InstructorRoleService(repo);

        var result = await service.UpdateInstructorRoleAsync(new UpdateInstructorRoleInput(9, "NewName"));

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_200_When_Deleted()
    {
        var repo = CreateRepo();
        var service = new InstructorRoleService(repo);

        var result = await service.DeleteInstructorRoleAsync(1);

        Assert.True(result.Success);
        Assert.True(result.Result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_404_When_NotFound()
    {
        var repo = CreateRepo();
        var service = new InstructorRoleService(repo);

        var result = await service.DeleteInstructorRoleAsync(9);

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
    }
}






