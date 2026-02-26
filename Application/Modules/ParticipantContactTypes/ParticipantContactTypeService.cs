using Backend.Application.Common;
using Backend.Application.Modules.ParticipantContactTypes.Caching;
using Backend.Application.Modules.ParticipantContactTypes.Inputs;
using Backend.Application.Modules.ParticipantContactTypes.Outputs;
using Backend.Domain.Modules.ParticipantContactTypes.Contracts;
using Backend.Domain.Modules.ParticipantContactTypes.Models;

namespace Backend.Application.Modules.ParticipantContactTypes;

public sealed class ParticipantContactTypeService(IParticipantContactTypeCache cache, IParticipantContactTypeRepository repository) : IParticipantContactTypeService
{
    private readonly IParticipantContactTypeCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly IParticipantContactTypeRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<ParticipantContactTypeResult> CreateParticipantContactTypeAsync(CreateParticipantContactTypeInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
                return new ParticipantContactTypeResult { Success = false, Error = ResultError.Validation, Message = "Participant contact type cannot be null." };

            var existing = await _repository.GetByNameAsync(input.Name, cancellationToken);
            if (existing is not null)
                return new ParticipantContactTypeResult { Success = false, Error = ResultError.Validation, Message = "A participant contact type with the same name already exists." };

            var created = await _repository.AddAsync(new ParticipantContactType(1, input.Name), cancellationToken);
            _cache.ResetEntity(created);
            _cache.SetEntity(created);
            return new ParticipantContactTypeResult { Success = true, Result = created, Message = "Participant contact type created successfully." };
        }
        catch (ArgumentException ex)
        {
            return new ParticipantContactTypeResult { Success = false, Error = ResultError.Validation, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new ParticipantContactTypeResult { Success = false, Error = ResultError.Unexpected, Message = $"An error occurred while creating the participant contact type: {ex.Message}" };
        }
    }

    public async Task<ParticipantContactTypeListResult> GetAllParticipantContactTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var participantContactTypes = await _cache.GetAllAsync(
                token => _repository.GetAllAsync(token),
                cancellationToken);
            return new ParticipantContactTypeListResult
            {
                Success = true,
                Result = participantContactTypes,
                Message = participantContactTypes.Any() ? $"Retrieved {participantContactTypes.Count} participant contact type(s) successfully." : "No participant contact types found."
            };
        }
        catch (Exception ex)
        {
            return new ParticipantContactTypeListResult { Success = false, Error = ResultError.Unexpected, Message = $"An error occurred while retrieving participant contact types: {ex.Message}" };
        }
    }

    public async Task<ParticipantContactTypeResult> GetParticipantContactTypeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));

            var participantContactType = await _cache.GetByIdAsync(
                id,
                token => _repository.GetByIdAsync(id, token),
                cancellationToken);
            if (participantContactType == null)
                return new ParticipantContactTypeResult { Success = false, Error = ResultError.NotFound, Message = $"Participant contact type with ID '{id}' not found." };

            return new ParticipantContactTypeResult { Success = true, Result = participantContactType, Message = "Participant contact type retrieved successfully." };
        }
        catch (ArgumentException ex)
        {
            return new ParticipantContactTypeResult { Success = false, Error = ResultError.Validation, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new ParticipantContactTypeResult { Success = false, Error = ResultError.Unexpected, Message = $"An error occurred while retrieving the participant contact type: {ex.Message}" };
        }
    }

    public async Task<ParticipantContactTypeResult> GetParticipantContactTypeByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            var participantContactType = await _cache.GetByNameAsync(
                name,
                token => _repository.GetByNameAsync(name, token),
                cancellationToken);
            if (participantContactType == null)
                return new ParticipantContactTypeResult { Success = false, Error = ResultError.NotFound, Message = $"Participant contact type with name '{name}' not found." };

            return new ParticipantContactTypeResult { Success = true, Result = participantContactType, Message = "Participant contact type retrieved successfully." };
        }
        catch (ArgumentException ex)
        {
            return new ParticipantContactTypeResult { Success = false, Error = ResultError.Validation, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new ParticipantContactTypeResult { Success = false, Error = ResultError.Unexpected, Message = $"An error occurred while retrieving the participant contact type: {ex.Message}" };
        }
    }

    public async Task<ParticipantContactTypeResult> UpdateParticipantContactTypeAsync(UpdateParticipantContactTypeInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
                return new ParticipantContactTypeResult { Success = false, Error = ResultError.Validation, Message = "Participant contact type cannot be null." };

            var existingParticipantContactType = await _repository.GetByIdAsync(input.Id, cancellationToken);
            if (existingParticipantContactType == null)
                return new ParticipantContactTypeResult { Success = false, Error = ResultError.NotFound, Message = $"Participant contact type with ID '{input.Id}' not found." };

            existingParticipantContactType.Update(input.Name);
            var updatedParticipantContactType = await _repository.UpdateAsync(existingParticipantContactType.Id, existingParticipantContactType, cancellationToken);
            if (updatedParticipantContactType == null)
                return new ParticipantContactTypeResult { Success = false, Error = ResultError.Unexpected, Message = "Failed to update participant contact type." };
            _cache.ResetEntity(existingParticipantContactType);
            _cache.SetEntity(updatedParticipantContactType);

            return new ParticipantContactTypeResult { Success = true, Result = updatedParticipantContactType, Message = "Participant contact type updated successfully." };
        }
        catch (ArgumentException ex)
        {
            return new ParticipantContactTypeResult { Success = false, Error = ResultError.Validation, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new ParticipantContactTypeResult { Success = false, Error = ResultError.Unexpected, Message = $"An error occurred while updating the participant contact type: {ex.Message}" };
        }
    }

    public async Task<ParticipantContactTypeDeleteResult> DeleteParticipantContactTypeAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than zero.", nameof(id));

            var existingParticipantContactType = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingParticipantContactType == null)
                return new ParticipantContactTypeDeleteResult { Success = false, Error = ResultError.NotFound, Result = false, Message = $"Participant contact type with ID '{id}' not found." };

            if (await _repository.IsInUseAsync(id, cancellationToken))
                return new ParticipantContactTypeDeleteResult { Success = false, Error = ResultError.Conflict, Result = false, Message = $"Cannot delete participant contact type with ID '{id}' because it is in use." };

            var isDeleted = await _repository.RemoveAsync(id, cancellationToken);
            if (!isDeleted)
                return new ParticipantContactTypeDeleteResult { Success = false, Error = ResultError.Unexpected, Result = false, Message = "Failed to delete participant contact type." };

            _cache.ResetEntity(existingParticipantContactType);
            return new ParticipantContactTypeDeleteResult { Success = true, Result = true, Message = "Participant contact type deleted successfully." };
        }
        catch (ArgumentException ex)
        {
            return new ParticipantContactTypeDeleteResult { Success = false, Error = ResultError.Validation, Result = false, Message = ex.Message };
        }
        catch (Exception ex)
        {
            return new ParticipantContactTypeDeleteResult { Success = false, Error = ResultError.Unexpected, Result = false, Message = $"An error occurred while deleting the participant contact type: {ex.Message}" };
        }
    }
}

