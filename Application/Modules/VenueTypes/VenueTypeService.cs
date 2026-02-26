using Backend.Application.Common;
using Backend.Application.Modules.VenueTypes.Caching;
using Backend.Application.Modules.VenueTypes.Inputs;
using Backend.Application.Modules.VenueTypes.Outputs;
using Backend.Domain.Modules.VenueTypes.Contracts;
using Backend.Domain.Modules.VenueTypes.Models;

namespace Backend.Application.Modules.VenueTypes;

public sealed class VenueTypeService(IVenueTypeCache cache, IVenueTypeRepository repository) : IVenueTypeService
{
    private readonly IVenueTypeCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly IVenueTypeRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<VenueTypeResult> CreateVenueTypeAsync(CreateVenueTypeInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
                return new VenueTypeResult { Success = false, Error = ResultError.Validation, Message = "Venue type cannot be null." };

            var existing = await _repository.GetByNameAsync(input.Name, cancellationToken);
            if (existing is not null)
                return new VenueTypeResult { Success = false, Error = ResultError.Validation, Message = "A venue type with the same name already exists." };

            var created = await _repository.AddAsync(new VenueType(1, input.Name), cancellationToken);
            _cache.ResetEntity(created);
            _cache.SetEntity(created);
            return new VenueTypeResult { Success = true, Result = created, Message = "Venue type created successfully." };
        }
        catch (ArgumentException ex)
        {
            return new VenueTypeResult { Success = false, Error = ResultError.Validation, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new VenueTypeResult { Success = false, Error = ResultError.Unexpected, Message = $"An error occurred while creating the venue type: {ex.Message}" };
        }
    }

    public async Task<VenueTypeListResult> GetAllVenueTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var venueTypes = await _cache.GetAllAsync(
                token => _repository.GetAllAsync(token),
                cancellationToken);
            return new VenueTypeListResult
            {
                Success = true,
                                Result = venueTypes,
                Message = venueTypes.Any() ? $"Retrieved {venueTypes.Count} venue type(s) successfully." : "No venue types found."
            };
        }
        catch (Exception ex)
        {
            return new VenueTypeListResult { Success = false, Error = ResultError.Unexpected, Message = $"An error occurred while retrieving venue types: {ex.Message}" };
        }
    }

    public async Task<VenueTypeResult> GetVenueTypeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));

            var venueType = await _cache.GetByIdAsync(
                id,
                token => _repository.GetByIdAsync(id, token),
                cancellationToken);
            if (venueType == null)
                return new VenueTypeResult { Success = false, Error = ResultError.NotFound, Message = $"Venue type with ID '{id}' not found." };

            return new VenueTypeResult { Success = true, Result = venueType, Message = "Venue type retrieved successfully." };
        }
        catch (ArgumentException ex)
        {
            return new VenueTypeResult { Success = false, Error = ResultError.Validation, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new VenueTypeResult { Success = false, Error = ResultError.Unexpected, Message = $"An error occurred while retrieving the venue type: {ex.Message}" };
        }
    }

    public async Task<VenueTypeResult> GetVenueTypeByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            var venueType = await _cache.GetByNameAsync(
                name,
                token => _repository.GetByNameAsync(name, token),
                cancellationToken);
            if (venueType == null)
                return new VenueTypeResult { Success = false, Error = ResultError.NotFound, Message = $"Venue type with name '{name}' not found." };

            return new VenueTypeResult { Success = true, Result = venueType, Message = "Venue type retrieved successfully." };
        }
        catch (ArgumentException ex)
        {
            return new VenueTypeResult { Success = false, Error = ResultError.Validation, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new VenueTypeResult { Success = false, Error = ResultError.Unexpected, Message = $"An error occurred while retrieving the venue type: {ex.Message}" };
        }
    }

    public async Task<VenueTypeResult> UpdateVenueTypeAsync(UpdateVenueTypeInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
                return new VenueTypeResult { Success = false, Error = ResultError.Validation, Message = "Venue type cannot be null." };

            var existingVenueType = await _repository.GetByIdAsync(input.Id, cancellationToken);
            if (existingVenueType == null)
                return new VenueTypeResult { Success = false, Error = ResultError.NotFound, Message = $"Venue type with ID '{input.Id}' not found." };

            existingVenueType.Update(input.Name);
            var updatedVenueType = await _repository.UpdateAsync(existingVenueType.Id, existingVenueType, cancellationToken);
            if (updatedVenueType == null)
                return new VenueTypeResult { Success = false, Error = ResultError.Unexpected, Message = "Failed to update venue type." };
            _cache.ResetEntity(existingVenueType);
            _cache.SetEntity(updatedVenueType);

            return new VenueTypeResult { Success = true, Result = updatedVenueType, Message = "Venue type updated successfully." };
        }
        catch (ArgumentException ex)
        {
            return new VenueTypeResult { Success = false, Error = ResultError.Validation, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new VenueTypeResult { Success = false, Error = ResultError.Unexpected, Message = $"An error occurred while updating the venue type: {ex.Message}" };
        }
    }

    public async Task<VenueTypeDeleteResult> DeleteVenueTypeAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));

            var existingVenueType = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingVenueType == null)
                return new VenueTypeDeleteResult { Success = false, Error = ResultError.NotFound, Result = false, Message = $"Venue type with ID '{id}' not found." };

            if (await _repository.IsInUseAsync(id, cancellationToken))
                return new VenueTypeDeleteResult { Success = false, Error = ResultError.Conflict, Result = false, Message = $"Cannot delete venue type with ID '{id}' because it is in use." };

            var isDeleted = await _repository.RemoveAsync(id, cancellationToken);
            if (!isDeleted)
                return new VenueTypeDeleteResult { Success = false, Error = ResultError.Unexpected, Result = false, Message = "Failed to delete venue type." };

            _cache.ResetEntity(existingVenueType);
            return new VenueTypeDeleteResult { Success = true, Result = true, Message = "Venue type deleted successfully." };
        }
        catch (ArgumentException ex)
        {
            return new VenueTypeDeleteResult { Success = false, Error = ResultError.Validation, Result = false, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new VenueTypeDeleteResult { Success = false, Error = ResultError.Unexpected, Result = false, Message = $"An error occurred while deleting the venue type: {ex.Message}" };
        }
    }
}

