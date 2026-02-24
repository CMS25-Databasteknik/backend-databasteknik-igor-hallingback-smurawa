using Backend.Application.Common;
using Backend.Domain.Modules.ParticipantContactTypes.Models;

namespace Backend.Application.Modules.ParticipantContactTypes.Outputs;

public sealed class ParticipantContactTypeResult : ResultCommon<ParticipantContactType>
{
}

public sealed class ParticipantContactTypeListResult : ResultCommon<IReadOnlyList<ParticipantContactType>>
{
}

public sealed class ParticipantContactTypeDeleteResult : ResultCommon<bool>
{
}
