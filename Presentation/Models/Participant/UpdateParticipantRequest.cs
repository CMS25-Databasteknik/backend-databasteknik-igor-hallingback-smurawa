namespace Backend.Presentation.API.Models.Participant;

public sealed record UpdateParticipantRequest(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber
);
