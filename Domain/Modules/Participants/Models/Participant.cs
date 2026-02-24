using Backend.Domain.Modules.ParticipantContactTypes.Models;

namespace Backend.Domain.Modules.Participants.Models;

public sealed class Participant
{
    public Guid Id { get; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public ParticipantContactType ContactType { get; private set; } = null!;

    public Participant(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        ParticipantContactType? contactType = null)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty.", nameof(id));

        Id = id;
        SetValues(firstName, lastName, email, phoneNumber, contactType);
    }

    public void Update(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        ParticipantContactType? contactType = null)
    {
        SetValues(firstName, lastName, email, phoneNumber, contactType);
    }

    private void SetValues(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        ParticipantContactType? contactType)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty or whitespace.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty or whitespace.", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty or whitespace.", nameof(email));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty or whitespace.", nameof(phoneNumber));

        var resolvedContactType = contactType ?? new ParticipantContactType(1, "Primary");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
        PhoneNumber = phoneNumber.Trim();
        ContactType = resolvedContactType;
    }
}
