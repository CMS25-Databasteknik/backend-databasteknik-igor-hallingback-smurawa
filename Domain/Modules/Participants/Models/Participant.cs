namespace Backend.Domain.Modules.Participants.Models;

public sealed class Participant
{
    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public ParticipantContactType ContactType { get; }

    public Participant(Guid id, string firstName, string lastName, string email, string phoneNumber, ParticipantContactType contactType = ParticipantContactType.Primary)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty.", nameof(id));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty or whitespace.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty or whitespace.", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty or whitespace.", nameof(email));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty or whitespace.", nameof(phoneNumber));

        if (!Enum.IsDefined(typeof(ParticipantContactType), contactType))
            throw new ArgumentException("Participant contact type is invalid.", nameof(contactType));

        Id = id;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
        PhoneNumber = phoneNumber.Trim();
        ContactType = contactType;
    }
}
