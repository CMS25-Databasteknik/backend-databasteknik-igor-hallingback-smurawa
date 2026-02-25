using Backend.Domain.Modules.PaymentMethod.Models;

namespace Tests.Unit.Domain.Modules.CourseRegistrations.Models;

public class PaymentMethod_Tests
{
    public static IEnumerable<object[]> ValidPaymentMethods()
    {
        yield return [new PaymentMethod(1, "Card")];
        yield return [new PaymentMethod(2, "Invoice")];
        yield return [new PaymentMethod(3, "Cash")];
        yield return [new PaymentMethod(0, "Unknown")];
    }

    [Theory]
    [MemberData(nameof(ValidPaymentMethods))]
    public void Constructor_Should_Create_PaymentMethod_When_Valid(PaymentMethod paymentMethod)
    {
        Assert.True(paymentMethod.Id >= 0);
        Assert.False(string.IsNullOrWhiteSpace(paymentMethod.Name));
    }

    [Fact]
    public void Constructor_Should_Throw_When_Id_Is_Negative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PaymentMethod(-1, "Invalid"));
    }
}
