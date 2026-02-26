using Backend.Application.Common;
using Backend.Application.Modules.Participants.Inputs;
using Backend.Application.Modules.Participants.Outputs;
using Backend.Domain.Modules.ParticipantContactTypes.Contracts;
using Backend.Domain.Modules.ParticipantContactTypes.Models;
using Backend.Domain.Modules.Participants.Contracts;
using Backend.Domain.Modules.Participants.Models;

namespace Backend.Application.Modules.Participants;

public class ParticipantService(
    IParticipantRepository participantRepository,
    IParticipantContactTypeRepository participantContactTypeRepository) : IParticipantService
{
    private readonly IParticipantRepository _participantRepository = participantRepository ?? throw new ArgumentNullException(nameof(participantRepository));
    private readonly IParticipantContactTypeRepository _participantContactTypeRepository = participantContactTypeRepository ?? throw new ArgumentNullException(nameof(participantContactTypeRepository));

    public async Task<ParticipantResult> CreateParticipantAsync(CreateParticipantInput participant, CancellationToken cancellationToken = default)
    {
        try
        {
            if (participant == null)
            {
                return new ParticipantResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Result = null,
                    Message = "Participant cannot be null."
                };
            }

            var contactType = await _participantContactTypeRepository.GetByIdAsync(participant.ContactTypeId, cancellationToken);
            if (contactType == null)
            {
                return new ParticipantResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Result = null,
                    Message = $"Participant contact type with ID '{participant.ContactTypeId}' not found."
                };
            }

            var newParticipant = new Participant(
                Guid.NewGuid(),
                participant.FirstName,
                participant.LastName,
                participant.Email,
                participant.PhoneNumber,
                contactType
            );

            var result = await _participantRepository.AddAsync(newParticipant, cancellationToken);

            return new ParticipantResult
            {
                Success = true,
                                Result = result,
                Message = "Participant created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new ParticipantResult
            {
                Success = false,
                Error = ResultError.Validation,
                Result = null,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new ParticipantResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Result = null,
                Message = $"An error occurred while creating the participant: {ex.Message}"
            };
        }
    }

    public async Task<ParticipantListResult> GetAllParticipantsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var participants = await _participantRepository.GetAllAsync(cancellationToken);

            if (!participants.Any())
            {
                return new ParticipantListResult
                {
                    Success = true,
                    Result = participants,
                                        Message = "No participants found."
                };
            }

            return new ParticipantListResult
            {
                Success = true,
                                Result = participants,
                Message = $"Retrieved {participants.Count} participant(s) successfully."
            };
        }
        catch (Exception ex)
        {
            return new ParticipantListResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while retrieving participants: {ex.Message}"
            };
        }
    }

    public async Task<ParticipantDetailsResult> GetParticipantByIdAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (participantId == Guid.Empty)
            {
                return new ParticipantDetailsResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Participant ID cannot be empty."
                };
            }

            var existingParticipant = await _participantRepository.GetByIdAsync(participantId, cancellationToken);

            if (existingParticipant == null)
            {
                return new ParticipantDetailsResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Participant with ID '{participantId}' not found."
                };
            }

            var details = new ParticipantDetails(
                existingParticipant.Id,
                existingParticipant.FirstName,
                existingParticipant.LastName,
                existingParticipant.Email,
                existingParticipant.PhoneNumber,
                new ParticipantLookupItem(
                    existingParticipant.ContactType.Id,
                    existingParticipant.ContactType.Name)
            );

            return new ParticipantDetailsResult
            {
                Success = true,
                                Result = details,
                Message = "Participant retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new ParticipantDetailsResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while retrieving the participant: {ex.Message}"
            };
        }
    }

    public async Task<ParticipantResult> UpdateParticipantAsync(UpdateParticipantInput participant, CancellationToken cancellationToken = default)
    {
        try
        {
            if (participant == null)
            {
                return new ParticipantResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Participant cannot be null."
                };
            }

            if (participant.Id == Guid.Empty)
            {
                return new ParticipantResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Participant ID cannot be empty."
                };
            }

            var existingParticipant = await _participantRepository.GetByIdAsync(participant.Id, cancellationToken);
            if (existingParticipant == null)
            {
                return new ParticipantResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Participant with ID '{participant.Id}' not found."
                };
            }

            var contactType = await _participantContactTypeRepository.GetByIdAsync(participant.ContactTypeId, cancellationToken);
            if (contactType == null)
            {
                return new ParticipantResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Participant contact type with ID '{participant.ContactTypeId}' not found."
                };
            }

            existingParticipant.Update(
                participant.FirstName,
                participant.LastName,
                participant.Email,
                participant.PhoneNumber,
                contactType
            );

            var updatedParticipant = await _participantRepository.UpdateAsync(existingParticipant.Id, existingParticipant, cancellationToken);

            if (updatedParticipant == null)
            {
                return new ParticipantResult
                {
                    Success = false,
                    Error = ResultError.Unexpected,
                    Message = "Failed to update participant."
                };
            }

            return new ParticipantResult
            {
                Success = true,
                                Result = updatedParticipant,
                Message = "Participant updated successfully."
            };
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("modified by another user"))
        {
            return new ParticipantResult
            {
                Success = false,
                Error = ResultError.Conflict,
                Message = "The participant was modified by another user. Please refresh and try again."
            };
        }
        catch (ArgumentException ex)
        {
            return new ParticipantResult
            {
                Success = false,
                Error = ResultError.Validation,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new ParticipantResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while updating the participant: {ex.Message}"
            };
        }
    }

    public async Task<ParticipantDeleteResult> DeleteParticipantAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (participantId == Guid.Empty)
            {
                return new ParticipantDeleteResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Participant ID cannot be empty.",
                    Result = false
                };
            }

            var existingParticipant = await _participantRepository.GetByIdAsync(participantId, cancellationToken);
            if (existingParticipant == null)
            {
                return new ParticipantDeleteResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Participant with ID '{participantId}' not found.",
                    Result = false
                };
            }

            var hasRegistrations = await _participantRepository.HasRegistrationsAsync(participantId, cancellationToken);
            if (hasRegistrations)
            {
                return new ParticipantDeleteResult
                {
                    Success = false,
                    Error = ResultError.Conflict,
                    Message = "Cannot delete participant because they have course registrations.",
                    Result = false
                };
            }

            var result = await _participantRepository.RemoveAsync(participantId, cancellationToken);

            if (!result)
            {
                return new ParticipantDeleteResult
                {
                    Success = false,
                    Error = ResultError.Unexpected,
                    Message = "Failed to delete participant.",
                    Result = false
                };
            }

            return new ParticipantDeleteResult
            {
                Success = true,
                                Result = result,
                Message = "Participant deleted successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new ParticipantDeleteResult
            {
                Success = false,
                Error = ResultError.NotFound,
                Message = ex.Message,
                Result = false
            };
        }
        catch (Exception ex)
        {
            return new ParticipantDeleteResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while deleting the participant: {ex.Message}",
                Result = false
            };
        }
    }

}

