using Backend.Application.Common;
using PaymentMethodModel = Backend.Domain.Modules.PaymentMethod.Models.PaymentMethod;

namespace Backend.Application.Modules.PaymentMethods.Outputs;

public sealed record PaymentMethodResult : ResultBase<PaymentMethodModel>
{
}

public sealed record PaymentMethodListResult : ResultBase<IReadOnlyList<PaymentMethodModel>>
{
}

public sealed record PaymentMethodDeleteResult : ResultBase<bool>
{
}
