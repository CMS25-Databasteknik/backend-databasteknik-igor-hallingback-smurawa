using Backend.Domain.Modules.Participants.Models;

namespace Backend.Domain.Modules.Participants.Contracts;

public interface IParticipantRepository
{
    Task<Participant> CreateParticipantAsync(Participant participant, CancellationToken cancellationToken);
    Task<Participant?> GetParticipantByIdAsync(Guid participantId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Participant>> GetAllParticipantsAsync(CancellationToken cancellationToken);
    Task<Participant?> UpdateParticipantAsync(Participant participant, CancellationToken cancellationToken);
    Task<bool> DeleteParticipantAsync(Guid participantId, CancellationToken cancellationToken);
    Task<bool> HasRegistrationsAsync(Guid participantId, CancellationToken cancellationToken);
}
