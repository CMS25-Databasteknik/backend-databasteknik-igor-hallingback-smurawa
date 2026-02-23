using Backend.Application.Common;
using Backend.Domain.Modules.Participants.Models;

namespace Backend.Application.Modules.Participants.Outputs;

public sealed record ParticipantLookupItem(int Id, string Name);

public sealed record ParticipantDetails(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    ParticipantLookupItem ContactType
);

public sealed class ParticipantResult : ResultCommon<Participant>
{
}

public sealed class ParticipantDetailsResult : ResultCommon<ParticipantDetails>
{
}

public sealed class ParticipantListResult : ResultCommon<IReadOnlyList<Participant>>
{
}

public sealed class ParticipantDeleteResult : ResultCommon<bool>
{
}

