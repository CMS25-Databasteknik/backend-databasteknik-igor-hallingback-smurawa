using Backend.Application.Modules.PaymentMethods;
using Backend.Domain.Modules.PaymentMethod.Contracts;
using PaymentMethodModel = Backend.Domain.Modules.PaymentMethod.Models.PaymentMethod;
using NSubstitute;

namespace Backend.Tests.Unit.Application.Modules.PaymentMethods;

public class PaymentMethodService_Tests
{
    [Fact]
    public async Task GetAll_Should_Return_Items()
    {
        var repo = Substitute.For<IPaymentMethodRepository>();
        repo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns([new PaymentMethodModel(1, "Card"), new PaymentMethodModel(2, "Invoice")]);
        var service = new PaymentMethodService(repo);

        var result = await service.GetAllPaymentMethodsAsync();

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(2, result.Result!.Count);
    }
}
