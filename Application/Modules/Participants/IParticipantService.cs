using Backend.Application.Modules.Participants.Inputs;
using Backend.Application.Modules.Participants.Outputs;

namespace Backend.Application.Modules.Participants;

public interface IParticipantService
{
    Task<ParticipantResult> CreateParticipantAsync(CreateParticipantInput participant, CancellationToken cancellationToken = default);
    Task<ParticipantListResult> GetAllParticipantsAsync(CancellationToken cancellationToken = default);
    Task<ParticipantResult> GetParticipantByIdAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task<ParticipantResult> UpdateParticipantAsync(UpdateParticipantInput participant, CancellationToken cancellationToken = default);
    Task<ParticipantDeleteResult> DeleteParticipantAsync(Guid participantId, CancellationToken cancellationToken = default);
}
