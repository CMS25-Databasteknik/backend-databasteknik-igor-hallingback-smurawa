namespace Backend.Presentation.API.Models.CourseRegistration;

public sealed class PaymentMethodRequest
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}
