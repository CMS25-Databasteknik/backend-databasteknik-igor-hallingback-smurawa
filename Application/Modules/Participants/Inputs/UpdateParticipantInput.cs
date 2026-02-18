using Backend.Domain.Modules.Participants.Models;

namespace Backend.Application.Modules.Participants.Inputs;

public sealed record UpdateParticipantInput(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    ParticipantContactType ContactType
);
