using Backend.Domain.Models.Course;

namespace Backend.Application.Interfaces
{
    public interface ICoursesRepository
    {
        Task<CourseDto> CreateCourseAsync(CreateCourseDto course, CancellationToken cancellationToken);
        Task<CourseDto> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<CourseDto> GetCourseByTitleAsync(string title, CancellationToken cancellationToken);
        Task<CourseDto> UpdateCourseAsync(UpdateCourseDto course, CancellationToken cancellationToken);
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync(CancellationToken cancellationToken);
        Task<bool> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken);
    }
}
