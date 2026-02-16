namespace Backend.Domain.Modules.CourseRegistrations.Models;

public sealed class CourseRegistration
{
    public Guid Id { get; }
    public Guid ParticipantId { get; }
    public Guid CourseEventId { get; }
    public DateTime RegistrationDate { get; }
    public bool IsPaid { get; }

    public CourseRegistration(Guid id, Guid participantId, Guid courseEventId, DateTime registrationDate, bool isPaid)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty.", nameof(id));

        if (participantId == Guid.Empty)
            throw new ArgumentException("Participant ID cannot be empty.", nameof(participantId));

        if (courseEventId == Guid.Empty)
            throw new ArgumentException("Course event ID cannot be empty.", nameof(courseEventId));

        if (registrationDate == default)
            throw new ArgumentException("Registration date must be specified.", nameof(registrationDate));

        Id = id;
        ParticipantId = participantId;
        CourseEventId = courseEventId;
        RegistrationDate = registrationDate;
        IsPaid = isPaid;
    }
}
