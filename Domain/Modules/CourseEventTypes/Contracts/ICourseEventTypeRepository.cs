using Backend.Domain.Modules.CourseEventTypes.Models;

namespace Backend.Domain.Modules.CourseEventTypes.Contracts;

public interface ICourseEventTypeRepository
{
    Task<CourseEventType> CreateCourseEventTypeAsync(CourseEventType courseEventType, CancellationToken cancellationToken);
    Task<CourseEventType?> GetCourseEventTypeByIdAsync(int courseEventTypeId, CancellationToken cancellationToken);
    Task<CourseEventType?> GetCourseEventTypeByTypeNameAsync(string typeName, CancellationToken cancellationToken);
    Task<IReadOnlyList<CourseEventType>> GetAllCourseEventTypesAsync(CancellationToken cancellationToken);
    Task<CourseEventType?> UpdateCourseEventTypeAsync(CourseEventType courseEventType, CancellationToken cancellationToken);
    Task<bool> DeleteCourseEventTypeAsync(int courseEventTypeId, CancellationToken cancellationToken);
    Task<bool> IsInUseAsync(int courseEventTypeId, CancellationToken cancellationToken);
}
