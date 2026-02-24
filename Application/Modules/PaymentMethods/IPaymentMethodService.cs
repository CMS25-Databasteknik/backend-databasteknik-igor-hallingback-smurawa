using Backend.Application.Modules.PaymentMethods.Inputs;
using Backend.Application.Modules.PaymentMethods.Outputs;

namespace Backend.Application.Modules.PaymentMethods;

public interface IPaymentMethodService
{
    Task<PaymentMethodListResult> GetAllPaymentMethodsAsync(CancellationToken cancellationToken = default);
    Task<PaymentMethodResult> GetPaymentMethodByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PaymentMethodResult> GetPaymentMethodByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<PaymentMethodResult> CreatePaymentMethodAsync(CreatePaymentMethodInput input, CancellationToken cancellationToken = default);
    Task<PaymentMethodResult> UpdatePaymentMethodAsync(UpdatePaymentMethodInput input, CancellationToken cancellationToken = default);
    Task<PaymentMethodDeleteResult> DeletePaymentMethodAsync(int id, CancellationToken cancellationToken = default);
}
