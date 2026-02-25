using Backend.Application.Modules.PaymentMethods;
using Backend.Application.Modules.PaymentMethods.Caching;
using Backend.Domain.Modules.PaymentMethod.Contracts;
using NSubstitute;
using PaymentMethodModel = Backend.Domain.Modules.PaymentMethod.Models.PaymentMethod;

namespace Tests.Unit.Application.Modules.PaymentMethods;

public class PaymentMethodService_Tests
{
    [Fact]
    public async Task GetAll_Should_Return_Items()
    {
        var repo = Substitute.For<IPaymentMethodRepository>();
        var cache = Substitute.For<IPaymentMethodCache>();
        cache.GetAllAsync(Arg.Any<Func<CancellationToken, Task<IReadOnlyList<PaymentMethodModel>>>>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Func<CancellationToken, Task<IReadOnlyList<PaymentMethodModel>>>>()(ci.Arg<CancellationToken>()));
        repo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns([new PaymentMethodModel(1, "Card"), new PaymentMethodModel(2, "Invoice")]);
        var service = new PaymentMethodService(cache, repo);

        var result = await service.GetAllPaymentMethodsAsync();

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Result);
        Assert.Equal(2, result.Result!.Count);
    }
}
