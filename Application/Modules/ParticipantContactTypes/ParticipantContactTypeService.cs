using Backend.Application.Modules.ParticipantContactTypes.Inputs;
using Backend.Application.Modules.ParticipantContactTypes.Outputs;
using Backend.Domain.Modules.ParticipantContactTypes.Contracts;
using Backend.Domain.Modules.ParticipantContactTypes.Models;

namespace Backend.Application.Modules.ParticipantContactTypes;

public sealed class ParticipantContactTypeService(IParticipantContactTypeRepository repository) : IParticipantContactTypeService
{
    private readonly IParticipantContactTypeRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<ParticipantContactTypeResult> CreateParticipantContactTypeAsync(CreateParticipantContactTypeInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
                return new ParticipantContactTypeResult { Success = false, StatusCode = 400, Message = "Participant contact type cannot be null." };

            var existing = await _repository.GetByNameAsync(input.Name, cancellationToken);
            if (existing is not null)
                return new ParticipantContactTypeResult { Success = false, StatusCode = 400, Message = "A participant contact type with the same name already exists." };

            var created = await _repository.AddAsync(new ParticipantContactType(1, input.Name), cancellationToken);
            return new ParticipantContactTypeResult { Success = true, StatusCode = 201, Result = created, Message = "Participant contact type created successfully." };
        }
        catch (ArgumentException ex)
        {
            return new ParticipantContactTypeResult { Success = false, StatusCode = 400, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new ParticipantContactTypeResult { Success = false, StatusCode = 500, Message = $"An error occurred while creating the participant contact type: {ex.Message}" };
        }
    }

    public async Task<ParticipantContactTypeListResult> GetAllParticipantContactTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAllAsync(cancellationToken);
            return new ParticipantContactTypeListResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = result.Any() ? $"Retrieved {result.Count} participant contact type(s) successfully." : "No participant contact types found."
            };
        }
        catch (Exception ex)
        {
            return new ParticipantContactTypeListResult { Success = false, StatusCode = 500, Message = $"An error occurred while retrieving participant contact types: {ex.Message}" };
        }
    }

    public async Task<ParticipantContactTypeResult> GetParticipantContactTypeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));

            var result = await _repository.GetByIdAsync(id, cancellationToken);
            if (result == null)
                return new ParticipantContactTypeResult { Success = false, StatusCode = 404, Message = $"Participant contact type with ID '{id}' not found." };

            return new ParticipantContactTypeResult { Success = true, StatusCode = 200, Result = result, Message = "Participant contact type retrieved successfully." };
        }
        catch (ArgumentException ex)
        {
            return new ParticipantContactTypeResult { Success = false, StatusCode = 400, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new ParticipantContactTypeResult { Success = false, StatusCode = 500, Message = $"An error occurred while retrieving the participant contact type: {ex.Message}" };
        }
    }

    public async Task<ParticipantContactTypeResult> GetParticipantContactTypeByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            var result = await _repository.GetByNameAsync(name, cancellationToken);
            if (result == null)
                return new ParticipantContactTypeResult { Success = false, StatusCode = 404, Message = $"Participant contact type with name '{name}' not found." };

            return new ParticipantContactTypeResult { Success = true, StatusCode = 200, Result = result, Message = "Participant contact type retrieved successfully." };
        }
        catch (ArgumentException ex)
        {
            return new ParticipantContactTypeResult { Success = false, StatusCode = 400, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new ParticipantContactTypeResult { Success = false, StatusCode = 500, Message = $"An error occurred while retrieving the participant contact type: {ex.Message}" };
        }
    }

    public async Task<ParticipantContactTypeResult> UpdateParticipantContactTypeAsync(UpdateParticipantContactTypeInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
                return new ParticipantContactTypeResult { Success = false, StatusCode = 400, Message = "Participant contact type cannot be null." };

            var existing = await _repository.GetByIdAsync(input.Id, cancellationToken);
            if (existing == null)
                return new ParticipantContactTypeResult { Success = false, StatusCode = 404, Message = $"Participant contact type with ID '{input.Id}' not found." };

            existing.Update(input.Name);
            var updated = await _repository.UpdateAsync(existing.Id, existing, cancellationToken);
            if (updated == null)
                return new ParticipantContactTypeResult { Success = false, StatusCode = 500, Message = "Failed to update participant contact type." };

            return new ParticipantContactTypeResult { Success = true, StatusCode = 200, Result = updated, Message = "Participant contact type updated successfully." };
        }
        catch (ArgumentException ex)
        {
            return new ParticipantContactTypeResult { Success = false, StatusCode = 400, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new ParticipantContactTypeResult { Success = false, StatusCode = 500, Message = $"An error occurred while updating the participant contact type: {ex.Message}" };
        }
    }

    public async Task<ParticipantContactTypeDeleteResult> DeleteParticipantContactTypeAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));

            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                return new ParticipantContactTypeDeleteResult { Success = false, StatusCode = 404, Result = false, Message = $"Participant contact type with ID '{id}' not found." };

            if (await _repository.IsInUseAsync(id, cancellationToken))
                return new ParticipantContactTypeDeleteResult { Success = false, StatusCode = 409, Result = false, Message = $"Cannot delete participant contact type with ID '{id}' because it is in use." };

            var deleted = await _repository.RemoveAsync(id, cancellationToken);
            return new ParticipantContactTypeDeleteResult { Success = true, StatusCode = 200, Result = deleted, Message = "Participant contact type deleted successfully." };
        }
        catch (ArgumentException ex)
        {
            return new ParticipantContactTypeDeleteResult { Success = false, StatusCode = 400, Result = false, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new ParticipantContactTypeDeleteResult { Success = false, StatusCode = 500, Result = false, Message = $"An error occurred while deleting the participant contact type: {ex.Message}" };
        }
    }
}
