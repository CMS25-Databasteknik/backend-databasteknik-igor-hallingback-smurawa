using Backend.Domain.Modules.CourseRegistrations.Models;

namespace Backend.Tests.Unit.Domain.Modules.CourseRegistrations.Models;

public class PaymentMethod_Tests
{
    [Theory]
    [InlineData(PaymentMethod.Card)]
    [InlineData(PaymentMethod.Invoice)]
    [InlineData(PaymentMethod.Cash)]
    [InlineData(PaymentMethod.Unknown)]
    public void Enum_Should_Define_Known_Values(PaymentMethod paymentMethod)
    {
        Assert.True(Enum.IsDefined(typeof(PaymentMethod), paymentMethod));
    }

    [Fact]
    public void Enum_Should_Reject_Invalid_Value()
    {
        Assert.False(Enum.IsDefined(typeof(PaymentMethod), (PaymentMethod)999));
    }
}
