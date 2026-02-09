namespace Backend.Domain.Models.Course
{
    public sealed record CourseSummaryDto(
        Guid Id,
        string Title,
        string Description,
        int DurationInDays
    );
}
