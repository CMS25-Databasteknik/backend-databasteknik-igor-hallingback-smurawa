using Backend.Application.Modules.CourseEventTypes.Inputs;
using Backend.Application.Modules.CourseEventTypes.Outputs;

namespace Backend.Application.Modules.CourseEventTypes;

public interface ICourseEventTypeService
{
    Task<CourseEventTypeResult> CreateCourseEventTypeAsync(CreateCourseEventTypeInput courseEventType, CancellationToken cancellationToken = default);
    Task<CourseEventTypeListResult> GetAllCourseEventTypesAsync(CancellationToken cancellationToken = default);
    Task<CourseEventTypeResult> GetCourseEventTypeByIdAsync(int courseEventTypeId, CancellationToken cancellationToken = default);
    Task<CourseEventTypeResult> GetCourseEventTypeByTypeNameAsync(string typeName, CancellationToken cancellationToken = default);
    Task<CourseEventTypeResult> UpdateCourseEventTypeAsync(UpdateCourseEventTypeInput courseEventType, CancellationToken cancellationToken = default);
    Task<CourseEventTypeDeleteResult> DeleteCourseEventTypeAsync(int courseEventTypeId, CancellationToken cancellationToken = default);
}
