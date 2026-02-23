using Backend.Application.Common;
using Backend.Domain.Modules.CourseRegistrations.Models;

namespace Backend.Application.Modules.CourseRegistrations.Outputs;

public sealed record RegistrationLookupItem(int Id, string Name);

public sealed record RegistrationGuidLookupItem(Guid Id, string Name);

public sealed record RegistrationCourseEventItem(Guid Id, DateTime? EventDate);

public sealed record CourseRegistrationDetails(
    Guid Id,
    RegistrationGuidLookupItem Participant,
    RegistrationCourseEventItem CourseEvent,
    DateTime RegistrationDate,
    RegistrationLookupItem Status,
    RegistrationLookupItem PaymentMethod
);

public sealed class CourseRegistrationResult : ResultCommon<CourseRegistration>
{
}

public sealed class CourseRegistrationDetailsResult : ResultCommon<CourseRegistrationDetails>
{
}

public sealed class CourseRegistrationListResult : ResultCommon<IReadOnlyList<CourseRegistration>>
{
}

public sealed class CourseRegistrationDeleteResult : ResultCommon<bool>
{
}
