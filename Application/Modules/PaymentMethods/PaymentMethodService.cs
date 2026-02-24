using Backend.Application.Modules.PaymentMethods.Inputs;
using Backend.Application.Modules.PaymentMethods.Outputs;
using Backend.Domain.Modules.PaymentMethod.Contracts;
using PaymentMethodModel = Backend.Domain.Modules.PaymentMethod.Models.PaymentMethod;

namespace Backend.Application.Modules.PaymentMethods;

public sealed class PaymentMethodService(IPaymentMethodRepository repository) : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<PaymentMethodResult> CreatePaymentMethodAsync(CreatePaymentMethodInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
                return new PaymentMethodResult { Success = false, StatusCode = 400, Message = "Payment method cannot be null." };

            var existing = await _repository.GetByNameAsync(input.Name, cancellationToken);
            if (existing is not null)
                return new PaymentMethodResult { Success = false, StatusCode = 400, Message = "A payment method with the same name already exists." };

            var created = await _repository.AddAsync(new PaymentMethodModel(0, input.Name), cancellationToken);
            return new PaymentMethodResult { Success = true, StatusCode = 201, Result = created, Message = "Payment method created successfully." };
        }
        catch (ArgumentException ex)
        {
            return new PaymentMethodResult { Success = false, StatusCode = 400, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new PaymentMethodResult { Success = false, StatusCode = 500, Message = $"An error occurred while creating the payment method: {ex.Message}" };
        }
    }

    public async Task<PaymentMethodListResult> GetAllPaymentMethodsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAllAsync(cancellationToken);
            return new PaymentMethodListResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = result.Any() ? $"Retrieved {result.Count} payment method(s) successfully." : "No payment methods found."
            };
        }
        catch (Exception ex)
        {
            return new PaymentMethodListResult { Success = false, StatusCode = 500, Message = $"An error occurred while retrieving payment methods: {ex.Message}" };
        }
    }

    public async Task<PaymentMethodResult> GetPaymentMethodByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id < 0)
                throw new ArgumentException("Id must be zero or positive.", nameof(id));

            var result = await _repository.GetByIdAsync(id, cancellationToken);
            if (result == null)
                return new PaymentMethodResult { Success = false, StatusCode = 404, Message = $"Payment method with ID '{id}' not found." };

            return new PaymentMethodResult { Success = true, StatusCode = 200, Result = result, Message = "Payment method retrieved successfully." };
        }
        catch (ArgumentException ex)
        {
            return new PaymentMethodResult { Success = false, StatusCode = 400, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new PaymentMethodResult { Success = false, StatusCode = 500, Message = $"An error occurred while retrieving the payment method: {ex.Message}" };
        }
    }

    public async Task<PaymentMethodResult> GetPaymentMethodByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            var result = await _repository.GetByNameAsync(name, cancellationToken);
            if (result == null)
                return new PaymentMethodResult { Success = false, StatusCode = 404, Message = $"Payment method with name '{name}' not found." };

            return new PaymentMethodResult { Success = true, StatusCode = 200, Result = result, Message = "Payment method retrieved successfully." };
        }
        catch (ArgumentException ex)
        {
            return new PaymentMethodResult { Success = false, StatusCode = 400, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new PaymentMethodResult { Success = false, StatusCode = 500, Message = $"An error occurred while retrieving the payment method: {ex.Message}" };
        }
    }

    public async Task<PaymentMethodResult> UpdatePaymentMethodAsync(UpdatePaymentMethodInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
                return new PaymentMethodResult { Success = false, StatusCode = 400, Message = "Payment method cannot be null." };

            var existing = await _repository.GetByIdAsync(input.Id, cancellationToken);
            if (existing == null)
                return new PaymentMethodResult { Success = false, StatusCode = 404, Message = $"Payment method with ID '{input.Id}' not found." };

            existing.Update(input.Name);
            var updated = await _repository.UpdateAsync(existing.Id, existing, cancellationToken);
            if (updated == null)
                return new PaymentMethodResult { Success = false, StatusCode = 500, Message = "Failed to update payment method." };

            return new PaymentMethodResult { Success = true, StatusCode = 200, Result = updated, Message = "Payment method updated successfully." };
        }
        catch (ArgumentException ex)
        {
            return new PaymentMethodResult { Success = false, StatusCode = 400, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new PaymentMethodResult { Success = false, StatusCode = 500, Message = $"An error occurred while updating the payment method: {ex.Message}" };
        }
    }

    public async Task<PaymentMethodDeleteResult> DeletePaymentMethodAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id < 0)
                throw new ArgumentException("Id must be zero or positive.", nameof(id));

            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                return new PaymentMethodDeleteResult { Success = false, StatusCode = 404, Result = false, Message = $"Payment method with ID '{id}' not found." };

            if (await _repository.IsInUseAsync(id, cancellationToken))
                return new PaymentMethodDeleteResult { Success = false, StatusCode = 409, Result = false, Message = $"Cannot delete payment method with ID '{id}' because it is in use." };

            var deleted = await _repository.RemoveAsync(id, cancellationToken);
            return new PaymentMethodDeleteResult { Success = true, StatusCode = 200, Result = deleted, Message = "Payment method deleted successfully." };
        }
        catch (ArgumentException ex)
        {
            return new PaymentMethodDeleteResult { Success = false, StatusCode = 400, Result = false, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new PaymentMethodDeleteResult { Success = false, StatusCode = 500, Result = false, Message = $"An error occurred while deleting the payment method: {ex.Message}" };
        }
    }
}
