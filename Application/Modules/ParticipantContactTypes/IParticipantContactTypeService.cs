using Backend.Application.Modules.ParticipantContactTypes.Inputs;
using Backend.Application.Modules.ParticipantContactTypes.Outputs;

namespace Backend.Application.Modules.ParticipantContactTypes;

public interface IParticipantContactTypeService
{
    Task<ParticipantContactTypeListResult> GetAllParticipantContactTypesAsync(CancellationToken cancellationToken = default);
    Task<ParticipantContactTypeResult> GetParticipantContactTypeByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ParticipantContactTypeResult> GetParticipantContactTypeByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<ParticipantContactTypeResult> CreateParticipantContactTypeAsync(CreateParticipantContactTypeInput input, CancellationToken cancellationToken = default);
    Task<ParticipantContactTypeResult> UpdateParticipantContactTypeAsync(UpdateParticipantContactTypeInput input, CancellationToken cancellationToken = default);
    Task<ParticipantContactTypeDeleteResult> DeleteParticipantContactTypeAsync(int id, CancellationToken cancellationToken = default);
}
