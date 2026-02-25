using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.PaymentMethod;

public sealed record CreatePaymentMethodRequest
{
    [Required]
    public required string Name { get; init; }
}
