using Backend.Application.Models;
using Backend.Domain.Models.Course;

namespace Backend.Application.Interfaces
{
    public interface ICourseService
    {
        Task<ResponseResult<CourseSummaryDto>> CreateCourseAsync(CreateCourseDto course, CancellationToken cancellationToken = default);
        Task<ResponseResult<IEnumerable<CourseSummaryDto>>> GetAllCoursesAsync(CancellationToken cancellationToken = default);
        Task<ResponseResult<CourseDto>> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<ResponseResult<CourseDto>> GetCourseByTitleAsync(string title, CancellationToken cancellationToken = default);
        Task<ResponseResult<CourseSummaryDto>> UpdateCourseAsync(UpdateCourseDto course, CancellationToken cancellationToken = default);
        Task<ResponseResult<bool>> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    }
}
