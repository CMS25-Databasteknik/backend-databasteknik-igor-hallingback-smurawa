using Backend.Application.Modules.InstructorRoles;
using Backend.Application.Modules.InstructorRoles.Inputs;
using Backend.Presentation.API.Models.InstructorRole;

namespace Backend.Presentation.API.Endpoints;

public static class InstructorRolesEndpoints
{
    public static RouteGroupBuilder MapInstructorRolesEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/instructor-roles")
            .WithTags("Instructor roles");

        group.MapGet("", GetAllInstructorRoles).WithName("GetAllInstructorRoles");
        group.MapGet("/{id:int}", GetInstructorRoleById).WithName("GetInstructorRoleById");
        group.MapPost("", CreateInstructorRole).WithName("CreateInstructorRole");
        group.MapPut("/{id:int}", UpdateInstructorRole).WithName("UpdateInstructorRole");
        group.MapDelete("/{id:int}", DeleteInstructorRole).WithName("DeleteInstructorRole");

        return group;
    }

    private static async Task<IResult> GetAllInstructorRoles(IInstructorRoleService roleService, CancellationToken cancellationToken)
    {
        var response = await roleService.GetAllInstructorRolesAsync(cancellationToken);
        return Results.Ok(response);
    }

    private static async Task<IResult> GetInstructorRoleById(int id, IInstructorRoleService roleService, CancellationToken cancellationToken)
    {
        var response = await roleService.GetInstructorRoleByIdAsync(id, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> CreateInstructorRole(CreateInstructorRoleRequest request, IInstructorRoleService roleService, CancellationToken cancellationToken)
    {
        var input = new CreateInstructorRoleInput(request.RoleName);
        var response = await roleService.CreateInstructorRoleAsync(input, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Created($"/api/instructor-roles/{response.Result?.Id}", response);
    }

    private static async Task<IResult> UpdateInstructorRole(int id, UpdateInstructorRoleRequest request, IInstructorRoleService roleService, CancellationToken cancellationToken)
    {
        var input = new UpdateInstructorRoleInput(id, request.RoleName);
        var response = await roleService.UpdateInstructorRoleAsync(input, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteInstructorRole(int id, IInstructorRoleService roleService, CancellationToken cancellationToken)
    {
        var response = await roleService.DeleteInstructorRoleAsync(id, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }
}



