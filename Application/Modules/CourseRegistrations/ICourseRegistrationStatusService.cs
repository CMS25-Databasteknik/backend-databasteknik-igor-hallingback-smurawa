using Backend.Application.Modules.CourseRegistrations.Inputs;
using Backend.Application.Modules.CourseRegistrations.Outputs;

namespace Backend.Application.Modules.CourseRegistrations;

public interface ICourseRegistrationStatusService
{
    Task<CourseRegistrationStatusListResult> GetAllCourseRegistrationStatusesAsync(CancellationToken cancellationToken = default);
    Task<CourseRegistrationStatusResult> GetCourseRegistrationStatusByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CourseRegistrationStatusResult> CreateCourseRegistrationStatusAsync(CreateCourseRegistrationStatusInput input, CancellationToken cancellationToken = default);
    Task<CourseRegistrationStatusResult> UpdateCourseRegistrationStatusAsync(UpdateCourseRegistrationStatusInput input, CancellationToken cancellationToken = default);
    Task<CourseRegistrationStatusDeleteResult> DeleteCourseRegistrationStatusAsync(int id, CancellationToken cancellationToken = default);
}
