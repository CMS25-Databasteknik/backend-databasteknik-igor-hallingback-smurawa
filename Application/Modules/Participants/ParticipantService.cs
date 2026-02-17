using Backend.Application.Modules.Participants.Inputs;
using Backend.Application.Modules.Participants.Outputs;
using Backend.Domain.Modules.Participants.Contracts;
using Backend.Domain.Modules.Participants.Models;

namespace Backend.Application.Modules.Participants;

public class ParticipantService(IParticipantRepository participantRepository) : IParticipantService
{
    private readonly IParticipantRepository _participantRepository = participantRepository ?? throw new ArgumentNullException(nameof(participantRepository));

    public async Task<ParticipantResult> CreateParticipantAsync(CreateParticipantInput participant, CancellationToken cancellationToken = default)
    {
        try
        {
            if (participant == null)
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "Participant cannot be null."
                };
            }

            if (string.IsNullOrWhiteSpace(participant.FirstName))
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "First name cannot be empty or whitespace."
                };
            }

            if (string.IsNullOrWhiteSpace(participant.LastName))
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "Last name cannot be empty or whitespace."
                };
            }

            if (string.IsNullOrWhiteSpace(participant.Email))
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "Email cannot be empty or whitespace."
                };
            }

            if (string.IsNullOrWhiteSpace(participant.PhoneNumber))
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "Phone number cannot be empty or whitespace."
                };
            }

            var newParticipant = new Participant(
                Guid.NewGuid(),
                participant.FirstName,
                participant.LastName,
                participant.Email,
                participant.PhoneNumber
            );

            var result = await _participantRepository.CreateParticipantAsync(newParticipant, cancellationToken);

            return new ParticipantResult
            {
                Success = true,
                StatusCode = 201,
                Result = result,
                Message = "Participant created successfully."
            };
        }
        catch (Exception ex)
        {
            return new ParticipantResult
            {
                Success = false,
                StatusCode = 500,
                Result = null,
                Message = $"An error occurred while creating the participant: {ex.Message}"
            };
        }
    }

    public async Task<ParticipantListResult> GetAllParticipantsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var participants = await _participantRepository.GetAllParticipantsAsync(cancellationToken);

            if (!participants.Any())
            {
                return new ParticipantListResult
                {
                    Success = true,
                    Result = participants,
                    StatusCode = 200,
                    Message = "No participants found."
                };
            }

            return new ParticipantListResult
            {
                Success = true,
                StatusCode = 200,
                Result = participants,
                Message = $"Retrieved {participants.Count} participant(s) successfully."
            };
        }
        catch (Exception ex)
        {
            return new ParticipantListResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving participants: {ex.Message}"
            };
        }
    }

    public async Task<ParticipantResult> GetParticipantByIdAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (participantId == Guid.Empty)
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Participant ID cannot be empty."
                };
            }

            var result = await _participantRepository.GetParticipantByIdAsync(participantId, cancellationToken);

            if (result == null)
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Participant with ID '{participantId}' not found."
                };
            }

            return new ParticipantResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Participant retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new ParticipantResult
            {
                Success = false,
                StatusCode = 500,
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
                    StatusCode = 400,
                    Message = "Participant cannot be null."
                };
            }

            if (participant.Id == Guid.Empty)
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Participant ID cannot be empty."
                };
            }

            if (string.IsNullOrWhiteSpace(participant.FirstName))
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "First name cannot be empty or whitespace."
                };
            }

            if (string.IsNullOrWhiteSpace(participant.LastName))
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Last name cannot be empty or whitespace."
                };
            }

            if (string.IsNullOrWhiteSpace(participant.Email))
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Email cannot be empty or whitespace."
                };
            }

            if (string.IsNullOrWhiteSpace(participant.PhoneNumber))
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Phone number cannot be empty or whitespace."
                };
            }

            var existingParticipant = await _participantRepository.GetParticipantByIdAsync(participant.Id, cancellationToken);
            if (existingParticipant == null)
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Participant with ID '{participant.Id}' not found."
                };
            }

            var updatedParticipant = new Participant(
                participant.Id,
                participant.FirstName,
                participant.LastName,
                participant.Email,
                participant.PhoneNumber
            );

            var result = await _participantRepository.UpdateParticipantAsync(updatedParticipant, cancellationToken);

            if (result == null)
            {
                return new ParticipantResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to update participant."
                };
            }

            return new ParticipantResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Participant updated successfully."
            };
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("modified by another user"))
        {
            return new ParticipantResult
            {
                Success = false,
                StatusCode = 409,
                Message = "The participant was modified by another user. Please refresh and try again."
            };
        }
        catch (Exception ex)
        {
            return new ParticipantResult
            {
                Success = false,
                StatusCode = 500,
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
                    StatusCode = 400,
                    Message = "Participant ID cannot be empty.",
                    Result = false
                };
            }

            // Repository handles cascade delete of registrations in transaction
            var result = await _participantRepository.DeleteParticipantAsync(participantId, cancellationToken);

            return new ParticipantDeleteResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Participant and all associated registrations deleted successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new ParticipantDeleteResult
            {
                Success = false,
                StatusCode = 404,
                Message = ex.Message,
                Result = false
            };
        }
        catch (Exception ex)
        {
            return new ParticipantDeleteResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while deleting the participant: {ex.Message}",
                Result = false
            };
        }
    }
}
