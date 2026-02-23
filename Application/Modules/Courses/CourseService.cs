using Backend.Application.Modules.Courses.Inputs;
using Backend.Application.Modules.Courses.Outputs;
using Backend.Domain.Modules.Courses.Contracts;
using Backend.Domain.Modules.Courses.Models;

namespace Backend.Application.Modules.Courses;

public class CourseService(ICourseRepository courseRepository) : ICourseService
{
    private readonly ICourseRepository _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));

    public async Task<CourseResult> CreateCourseAsync(CreateCourseInput course, CancellationToken cancellationToken = default)
    {
        try
        {
            if (course == null)
            {
                return new CourseResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "Course cannot be null."
                };
            }

            var newCourse = new Course(
                id: Guid.NewGuid(),
                course.Title,
                course.Description,
                course.DurationInDays
            );

            _ = await _courseRepository.AddAsync(newCourse, cancellationToken);

            return new CourseResult
            {
                Success = true,
                StatusCode = 201,
                Result = newCourse,
                Message = "Course created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseResult
            {
                Success = false,
                StatusCode = 400,
                Result = null,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseResult
            {
                Success = false,
                StatusCode = 500,
                Result = null,
                Message = $"An error occurred while creating the course: {ex.Message}"
            };
        }
    }

    public async Task<CourseListResult> GetAllCoursesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var courses = await _courseRepository.GetAllAsync(cancellationToken);

            if (!courses.Any())
            {
                return new CourseListResult
                {
                    Success = true,
                    Result = courses,
                    StatusCode = 200,
                    Message = "No courses found."
                };
            }

            return new CourseListResult
            {
                Success = true,
                StatusCode = 200,
                Result = courses,
                Message = $"Retrieved {courses.Count()} course(s) successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseListResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving courses: {ex.Message}"
            };
        }
    }

    public async Task<CourseWithEventsResult> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseId == Guid.Empty)
            {
                return new CourseWithEventsResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course ID cannot be empty."
                };
            }

            var result = await _courseRepository.GetCourseByIdAsync(courseId, cancellationToken);

            if (result == null)
            {
                return new CourseWithEventsResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course with ID '{courseId}' not found."
                };
            }

            return new CourseWithEventsResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Course retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseWithEventsResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving the course: {ex.Message}"
            };
        }
    }

    public async Task<CourseResult> UpdateCourseAsync(UpdateCourseInput course, CancellationToken cancellationToken = default)
    {
        try
        {
            if (course == null)
            {
                return new CourseResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course cannot be null."
                };
            }

            if (course.Id == Guid.Empty)
            {
                return new CourseResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course ID cannot be empty."
                };
            }

            var existingCourse = await _courseRepository.GetByIdAsync(course.Id, cancellationToken);
            if (existingCourse == null)
            {
                return new CourseResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course with ID '{course.Id}' not found."
                };
            }

            existingCourse.Update(
                course.Title,
                course.Description,
                course.DurationInDays
            );

            var updatedCourse = await _courseRepository.UpdateAsync(existingCourse.Id, existingCourse, cancellationToken);

            if (updatedCourse == null)
            {
                return new CourseResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to update course."
                };
            }

            return new CourseResult
            {
                Success = true,
                StatusCode = 200,
                Result = updatedCourse,
                Message = "Course updated successfully."
            };
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("modified by another user"))
        {
            return new CourseResult
            {
                Success = false,
                StatusCode = 409,
                Message = "The course was modified by another user. Please refresh and try again."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while updating the course: {ex.Message}"
            };
        }
    }

    public async Task<CourseDeleteResult> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseId == Guid.Empty)
            {
                return new CourseDeleteResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course ID cannot be empty.",
                    Result = false
                };
            }

            var existingCourse = await _courseRepository.GetByIdAsync(courseId, cancellationToken);
            if (existingCourse == null)
            {
                return new CourseDeleteResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course with ID '{courseId}' not found.",
                    Result = false
                };
            }

            var hasCourseEvents = await _courseRepository.HasCourseEventsAsync(courseId, cancellationToken);
            if (hasCourseEvents)
            {
                return new CourseDeleteResult
                {
                    Success = false,
                    StatusCode = 409,
                    Message = $"Cannot delete course with ID '{courseId}' because it has associated course events. Please delete the course events first.",
                    Result = false
                };
            }

            var result = await _courseRepository.RemoveAsync(courseId, cancellationToken);

            if (!result)
            {
                return new CourseDeleteResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to delete course.",
                    Result = false
                };
            }

            return new CourseDeleteResult
            {
                Success = true,
                StatusCode = 200,
                Message = "Course deleted successfully.",
                Result = true
            };
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("associated course events"))
        {
            return new CourseDeleteResult
            {
                Success = false,
                StatusCode = 409,
                Message = "Cannot delete course because it has associated course events. Please delete the course events first.",
                Result = false
            };
        }
        catch (Exception ex)
        {
            return new CourseDeleteResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while deleting the course: {ex.Message}",
                Result = false
            };
        }
    }
}


