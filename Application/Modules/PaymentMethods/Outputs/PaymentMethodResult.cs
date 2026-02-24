using Backend.Application.Common;
using PaymentMethodModel = Backend.Domain.Modules.PaymentMethod.Models.PaymentMethod;

namespace Backend.Application.Modules.PaymentMethods.Outputs;

public sealed class PaymentMethodResult : ResultCommon<PaymentMethodModel>
{
}

public sealed class PaymentMethodListResult : ResultCommon<IReadOnlyList<PaymentMethodModel>>
{
}

public sealed class PaymentMethodDeleteResult : ResultCommon<bool>
{
}
