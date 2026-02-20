using Backend.Application.Common;
using Backend.Domain.Modules.Participants.Models;

namespace Backend.Application.Modules.Participants.Outputs;

public sealed class ParticipantResult : ResultCommon<Participant>
{
}

public sealed class ParticipantListResult : ResultCommon<IReadOnlyList<Participant>>
{
}

public sealed class ParticipantDeleteResult : ResultCommon<bool>
{
}

