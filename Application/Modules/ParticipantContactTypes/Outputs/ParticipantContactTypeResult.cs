using Backend.Application.Common;
using Backend.Domain.Modules.ParticipantContactTypes.Models;

namespace Backend.Application.Modules.ParticipantContactTypes.Outputs;

public sealed record ParticipantContactTypeResult : ResultBase<ParticipantContactType>
{
}

public sealed record ParticipantContactTypeListResult : ResultBase<IReadOnlyList<ParticipantContactType>>
{
}

public sealed record ParticipantContactTypeDeleteResult : ResultBase<bool>
{
}
