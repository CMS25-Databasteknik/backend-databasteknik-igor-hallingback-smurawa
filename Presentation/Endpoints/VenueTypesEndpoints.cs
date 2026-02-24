using Backend.Application.Modules.VenueTypes;
using Backend.Application.Modules.VenueTypes.Inputs;

namespace Backend.Presentation.API.Endpoints;

public static class VenueTypesEndpoints
{
    public static RouteGroupBuilder MapVenueTypesEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/venue-types")
            .WithTags("Venue types");

        group.MapGet("", GetVenueTypes).WithName("GetVenueTypes");
        group.MapGet("/{id:int}", GetVenueTypeById).WithName("GetVenueTypeById");
        group.MapGet("/by-name/{name}", GetVenueTypeByName).WithName("GetVenueTypeByName");
        group.MapPost("", CreateVenueType).WithName("CreateVenueType");
        group.MapPut("/{id:int}", UpdateVenueType).WithName("UpdateVenueType");
        group.MapDelete("/{id:int}", DeleteVenueType).WithName("DeleteVenueType");

        return group;
    }

    private static async Task<IResult> GetVenueTypes(IVenueTypeService service, CancellationToken cancellationToken)
    {
        var response = await service.GetAllVenueTypesAsync(cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> GetVenueTypeById(int id, IVenueTypeService service, CancellationToken cancellationToken)
    {
        var response = await service.GetVenueTypeByIdAsync(id, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> GetVenueTypeByName(string name, IVenueTypeService service, CancellationToken cancellationToken)
    {
        var response = await service.GetVenueTypeByNameAsync(name, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> CreateVenueType(CreateVenueTypeInput input, IVenueTypeService service, CancellationToken cancellationToken)
    {
        var response = await service.CreateVenueTypeAsync(input, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Created($"/api/venue-types/{response.Result?.Id}", response);
    }

    private static async Task<IResult> UpdateVenueType(int id, UpdateVenueTypeInput input, IVenueTypeService service, CancellationToken cancellationToken)
    {
        var updateInput = new UpdateVenueTypeInput(id, input.Name);
        var response = await service.UpdateVenueTypeAsync(updateInput, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteVenueType(int id, IVenueTypeService service, CancellationToken cancellationToken)
    {
        var response = await service.DeleteVenueTypeAsync(id, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }
}
