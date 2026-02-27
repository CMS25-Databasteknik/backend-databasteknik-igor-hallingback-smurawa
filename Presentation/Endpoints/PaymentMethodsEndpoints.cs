using Backend.Application.Modules.PaymentMethods;
using Backend.Application.Modules.PaymentMethods.Inputs;
using Backend.Presentation.API.Models.PaymentMethod;

namespace Backend.Presentation.API.Endpoints;

public static class PaymentMethodsEndpoints
{
    public static RouteGroupBuilder MapPaymentMethodsEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/payment-methods")
            .WithTags("Payment methods");

        group.MapGet("", GetPaymentMethods).WithName("GetPaymentMethods");
        group.MapGet("/{id:int}", GetPaymentMethodById).WithName("GetPaymentMethodById");
        group.MapGet("/by-name/{name}", GetPaymentMethodByName).WithName("GetPaymentMethodByName");
        group.MapPost("", CreatePaymentMethod).WithName("CreatePaymentMethod");
        group.MapPut("/{id:int}", UpdatePaymentMethod).WithName("UpdatePaymentMethod");
        group.MapDelete("/{id:int}", DeletePaymentMethod).WithName("DeletePaymentMethod");

        return group;
    }

    private static async Task<IResult> GetPaymentMethods(IPaymentMethodService service, CancellationToken cancellationToken)
    {
        var response = await service.GetAllPaymentMethodsAsync(cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> GetPaymentMethodById(int id, IPaymentMethodService service, CancellationToken cancellationToken)
    {
        var response = await service.GetPaymentMethodByIdAsync(id, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> GetPaymentMethodByName(string name, IPaymentMethodService service, CancellationToken cancellationToken)
    {
        var response = await service.GetPaymentMethodByNameAsync(name, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> CreatePaymentMethod(CreatePaymentMethodRequest request, IPaymentMethodService service, CancellationToken cancellationToken)
    {
        var input = new CreatePaymentMethodInput(request.Name);
        var response = await service.CreatePaymentMethodAsync(input, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Created($"/api/payment-methods/{response.Result?.Id}", response);
    }

    private static async Task<IResult> UpdatePaymentMethod(int id, UpdatePaymentMethodRequest request, IPaymentMethodService service, CancellationToken cancellationToken)
    {
        var updateInput = new UpdatePaymentMethodInput(id, request.Name);
        var response = await service.UpdatePaymentMethodAsync(updateInput, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }

    private static async Task<IResult> DeletePaymentMethod(int id, IPaymentMethodService service, CancellationToken cancellationToken)
    {
        var response = await service.DeletePaymentMethodAsync(id, cancellationToken);
        if (!response.Success)
            return response.ToHttpResult();

        return Results.Ok(response);
    }
}



