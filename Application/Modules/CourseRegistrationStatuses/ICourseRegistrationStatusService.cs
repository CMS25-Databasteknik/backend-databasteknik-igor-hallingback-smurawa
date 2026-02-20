using Backend.Application.Modules.CourseRegistrationStatuses.Inputs;
using Backend.Application.Modules.CourseRegistrationStatuses.Outputs;

namespace Backend.Application.Modules.CourseRegistrationStatuses;

public interface ICourseRegistrationStatusService
{
    Task<CourseRegistrationStatusListResult> GetAllCourseRegistrationStatusesAsync(CancellationToken cancellationToken = default);
    Task<CourseRegistrationStatusResult> GetCourseRegistrationStatusByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CourseRegistrationStatusResult> GetCourseRegistrationStatusByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<CourseRegistrationStatusResult> CreateCourseRegistrationStatusAsync(CreateCourseRegistrationStatusInput input, CancellationToken cancellationToken = default);
    Task<CourseRegistrationStatusResult> UpdateCourseRegistrationStatusAsync(UpdateCourseRegistrationStatusInput input, CancellationToken cancellationToken = default);
    Task<CourseRegistrationStatusDeleteResult> DeleteCourseRegistrationStatusAsync(int id, CancellationToken cancellationToken = default);
}
