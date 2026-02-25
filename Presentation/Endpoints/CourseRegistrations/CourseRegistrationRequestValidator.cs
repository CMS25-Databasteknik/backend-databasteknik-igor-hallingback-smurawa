using Backend.Presentation.API.Models.CourseRegistration;

namespace Backend.Presentation.API.Endpoints.CourseRegistrations;

public static class CourseRegistrationRequestValidator
{
    public static Dictionary<string, string[]> Validate(CreateCourseRegistrationRequest request)
        => ValidateCore(request.ParticipantId, request.CourseEventId, request.StatusId, request.PaymentMethod);

    public static Dictionary<string, string[]> Validate(UpdateCourseRegistrationRequest request)
        => ValidateCore(request.ParticipantId, request.CourseEventId, request.StatusId, request.PaymentMethod);

    private static Dictionary<string, string[]> ValidateCore(
        Guid participantId,
        Guid courseEventId,
        int statusId,
        PaymentMethodRequest paymentMethod)
    {
        var errors = new Dictionary<string, string[]>();

        if (participantId == Guid.Empty)
            errors["participantId"] = ["participantId is required and must be a non-empty GUID."];

        if (courseEventId == Guid.Empty)
            errors["courseEventId"] = ["courseEventId is required and must be a non-empty GUID."];

        if (statusId < 0)
            errors["statusId"] = ["statusId must be zero or positive."];

        if (paymentMethod is null)
            errors["paymentMethod"] = ["paymentMethod is required."];
        else if (paymentMethod.Id < 0)
            errors["paymentMethod.id"] = ["paymentMethod.id is required and must be zero or positive."];

        if (paymentMethod is not null && string.IsNullOrWhiteSpace(paymentMethod.Name))
            errors["paymentMethod.name"] = ["paymentMethod.name is required."];

        return errors;
    }
}
