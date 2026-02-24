using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using Backend.Domain.Modules.ParticipantContactTypes.Models;
using Backend.Domain.Modules.PaymentMethod.Models;
using Backend.Domain.Modules.VenueTypes.Models;

namespace Backend.Infrastructure.Common.Repositories;

internal static class DomainValueConverters
{
    public static CourseRegistrationStatus ToCourseRegistrationStatus(int id, string? name = null)
    {
        if (!string.IsNullOrWhiteSpace(name))
            return new CourseRegistrationStatus(id, name);

        return id switch
        {
            0 => CourseRegistrationStatus.Pending,
            1 => CourseRegistrationStatus.Paid,
            2 => CourseRegistrationStatus.Cancelled,
            3 => CourseRegistrationStatus.Refunded,
            _ => new CourseRegistrationStatus(id, $"Status {id}")
        };
    }

    public static PaymentMethod ToPaymentMethod(int id)
    {
        if (!Enum.IsDefined(typeof(PaymentMethod), id))
            throw new InvalidOperationException($"Unsupported payment method id '{id}'.");

        return (PaymentMethod)id;
    }

    public static VenueType ToVenueType(int id, string? name = null)
    {
        return new VenueType(id, string.IsNullOrWhiteSpace(name) ? $"VenueType {id}" : name);
    }

    public static ParticipantContactType ToParticipantContactType(int id)
    {
        if (!Enum.IsDefined(typeof(ParticipantContactType), id))
            throw new InvalidOperationException($"Unsupported participant contact type id '{id}'.");

        return (ParticipantContactType)id;
    }

    public static int ToId(PaymentMethod paymentMethod) => (int)paymentMethod;
    public static int ToId(VenueType venueType) => venueType.Id;
    public static int ToId(ParticipantContactType contactType) => (int)contactType;
}
