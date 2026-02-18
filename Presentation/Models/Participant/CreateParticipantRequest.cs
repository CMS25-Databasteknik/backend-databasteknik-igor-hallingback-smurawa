using Backend.Domain.Modules.Participants.Models;

namespace Backend.Presentation.API.Models.Participant;

public sealed record CreateParticipantRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    ParticipantContactType ContactType
);
