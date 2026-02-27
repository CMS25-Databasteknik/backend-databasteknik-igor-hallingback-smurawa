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

public sealed record ParticipantResult : ResultBase<Participant>
{
}

public sealed record ParticipantDetailsResult : ResultBase<ParticipantDetails>
{
}

public sealed record ParticipantListResult : ResultBase<IReadOnlyList<Participant>>
{
}

public sealed record ParticipantDeleteResult : ResultBase<bool>
{
}

