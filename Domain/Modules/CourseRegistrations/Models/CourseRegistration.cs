using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using PaymentMethodModel = Backend.Domain.Modules.PaymentMethod.Models.PaymentMethod;
using System.Diagnostics.CodeAnalysis;

namespace Backend.Domain.Modules.CourseRegistrations.Models;

public sealed class CourseRegistration
{
    public Guid Id { get; }
    public Guid ParticipantId { get; private set; }
    public Guid CourseEventId { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public CourseRegistrationStatus Status { get; private set; }
    public PaymentMethodModel PaymentMethod { get; private set; }

    public CourseRegistration(
        Guid id,
        Guid participantId,
        Guid courseEventId,
        DateTime registrationDate,
        CourseRegistrationStatus status,
        PaymentMethodModel paymentMethod)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID cannot be empty.", nameof(id));

        Id = id;
        SetValues(participantId, courseEventId, registrationDate, status, paymentMethod);
    }

    public void Update(
        Guid participantId,
        Guid courseEventId,
        DateTime registrationDate,
        CourseRegistrationStatus status,
        PaymentMethodModel paymentMethod)
    {
        SetValues(participantId, courseEventId, registrationDate, status, paymentMethod);
    }

    [MemberNotNull(nameof(Status))]
    private void SetValues(
        Guid participantId,
        Guid courseEventId,
        DateTime registrationDate,
        CourseRegistrationStatus status,
        PaymentMethodModel paymentMethod)
    {
        if (participantId == Guid.Empty)
            throw new ArgumentException("Participant ID cannot be empty.", nameof(participantId));

        if (courseEventId == Guid.Empty)
            throw new ArgumentException("Course event ID cannot be empty.", nameof(courseEventId));

        if (registrationDate == default)
            throw new ArgumentException("Registration date must be specified.", nameof(registrationDate));

        ArgumentNullException.ThrowIfNull(status);

        if (!Enum.IsDefined(typeof(PaymentMethodModel), paymentMethod))
            throw new ArgumentException("Payment method is invalid.", nameof(paymentMethod));

        ParticipantId = participantId;
        CourseEventId = courseEventId;
        RegistrationDate = registrationDate;
        Status = status;
        PaymentMethod = paymentMethod;
    }
}
