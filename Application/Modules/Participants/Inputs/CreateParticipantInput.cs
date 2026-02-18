using Backend.Domain.Modules.ParticipantContactTypes.Models;

namespace Backend.Application.Modules.Participants.Inputs;

public sealed record CreateParticipantInput(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    ParticipantContactType ContactType
);
