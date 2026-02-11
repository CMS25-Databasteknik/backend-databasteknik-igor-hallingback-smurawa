using Backend.Application.Models;
using Backend.Application.Modules.Courses.Inputs;
using Backend.Application.Modules.Courses.Outputs;
using Backend.Domain.Models.Course;

namespace Backend.Application.Modules.Courses
{
    public interface ICourseService
    {
        Task<CourseResult> CreateCourseAsync(CreateCourseInput course, CancellationToken cancellationToken = default);
        Task<ResponseResult<IEnumerable<CourseSummaryDto>>> GetAllCoursesAsync(CancellationToken cancellationToken = default);
        Task<ResponseResult<CourseDto>> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<ResponseResult<CourseDto>> GetCourseByTitleAsync(string title, CancellationToken cancellationToken = default);
        Task<ResponseResult<CourseSummaryDto>> UpdateCourseAsync(UpdateCourseDto course, CancellationToken cancellationToken = default);
        Task<ResponseResult<bool>> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default);

    }
}
