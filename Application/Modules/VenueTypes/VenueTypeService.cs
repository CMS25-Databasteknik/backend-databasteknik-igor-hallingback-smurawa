using Backend.Application.Modules.VenueTypes.Inputs;
using Backend.Application.Modules.VenueTypes.Outputs;
using Backend.Domain.Modules.VenueTypes.Contracts;
using Backend.Domain.Modules.VenueTypes.Models;

namespace Backend.Application.Modules.VenueTypes;

public sealed class VenueTypeService(IVenueTypeRepository repository) : IVenueTypeService
{
    private readonly IVenueTypeRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<VenueTypeResult> CreateVenueTypeAsync(CreateVenueTypeInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
                return new VenueTypeResult { Success = false, StatusCode = 400, Message = "Venue type cannot be null." };

            var existing = await _repository.GetByNameAsync(input.Name, cancellationToken);
            if (existing is not null)
                return new VenueTypeResult { Success = false, StatusCode = 400, Message = "A venue type with the same name already exists." };

            var created = await _repository.AddAsync(new VenueType(1, input.Name), cancellationToken);
            return new VenueTypeResult { Success = true, StatusCode = 201, Result = created, Message = "Venue type created successfully." };
        }
        catch (ArgumentException ex)
        {
            return new VenueTypeResult { Success = false, StatusCode = 400, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new VenueTypeResult { Success = false, StatusCode = 500, Message = $"An error occurred while creating the venue type: {ex.Message}" };
        }
    }

    public async Task<VenueTypeListResult> GetAllVenueTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAllAsync(cancellationToken);
            return new VenueTypeListResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = result.Any() ? $"Retrieved {result.Count} venue type(s) successfully." : "No venue types found."
            };
        }
        catch (Exception ex)
        {
            return new VenueTypeListResult { Success = false, StatusCode = 500, Message = $"An error occurred while retrieving venue types: {ex.Message}" };
        }
    }

    public async Task<VenueTypeResult> GetVenueTypeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));

            var result = await _repository.GetByIdAsync(id, cancellationToken);
            if (result == null)
                return new VenueTypeResult { Success = false, StatusCode = 404, Message = $"Venue type with ID '{id}' not found." };

            return new VenueTypeResult { Success = true, StatusCode = 200, Result = result, Message = "Venue type retrieved successfully." };
        }
        catch (ArgumentException ex)
        {
            return new VenueTypeResult { Success = false, StatusCode = 400, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new VenueTypeResult { Success = false, StatusCode = 500, Message = $"An error occurred while retrieving the venue type: {ex.Message}" };
        }
    }

    public async Task<VenueTypeResult> GetVenueTypeByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            var result = await _repository.GetByNameAsync(name, cancellationToken);
            if (result == null)
                return new VenueTypeResult { Success = false, StatusCode = 404, Message = $"Venue type with name '{name}' not found." };

            return new VenueTypeResult { Success = true, StatusCode = 200, Result = result, Message = "Venue type retrieved successfully." };
        }
        catch (ArgumentException ex)
        {
            return new VenueTypeResult { Success = false, StatusCode = 400, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new VenueTypeResult { Success = false, StatusCode = 500, Message = $"An error occurred while retrieving the venue type: {ex.Message}" };
        }
    }

    public async Task<VenueTypeResult> UpdateVenueTypeAsync(UpdateVenueTypeInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
                return new VenueTypeResult { Success = false, StatusCode = 400, Message = "Venue type cannot be null." };

            var existing = await _repository.GetByIdAsync(input.Id, cancellationToken);
            if (existing == null)
                return new VenueTypeResult { Success = false, StatusCode = 404, Message = $"Venue type with ID '{input.Id}' not found." };

            existing.Update(input.Name);
            var updated = await _repository.UpdateAsync(existing.Id, existing, cancellationToken);
            if (updated == null)
                return new VenueTypeResult { Success = false, StatusCode = 500, Message = "Failed to update venue type." };

            return new VenueTypeResult { Success = true, StatusCode = 200, Result = updated, Message = "Venue type updated successfully." };
        }
        catch (ArgumentException ex)
        {
            return new VenueTypeResult { Success = false, StatusCode = 400, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new VenueTypeResult { Success = false, StatusCode = 500, Message = $"An error occurred while updating the venue type: {ex.Message}" };
        }
    }

    public async Task<VenueTypeDeleteResult> DeleteVenueTypeAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));

            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                return new VenueTypeDeleteResult { Success = false, StatusCode = 404, Result = false, Message = $"Venue type with ID '{id}' not found." };

            if (await _repository.IsInUseAsync(id, cancellationToken))
                return new VenueTypeDeleteResult { Success = false, StatusCode = 409, Result = false, Message = $"Cannot delete venue type with ID '{id}' because it is in use." };

            var deleted = await _repository.RemoveAsync(id, cancellationToken);
            return new VenueTypeDeleteResult { Success = true, StatusCode = 200, Result = deleted, Message = "Venue type deleted successfully." };
        }
        catch (ArgumentException ex)
        {
            return new VenueTypeDeleteResult { Success = false, StatusCode = 400, Result = false, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new VenueTypeDeleteResult { Success = false, StatusCode = 500, Result = false, Message = $"An error occurred while deleting the venue type: {ex.Message}" };
        }
    }
}
