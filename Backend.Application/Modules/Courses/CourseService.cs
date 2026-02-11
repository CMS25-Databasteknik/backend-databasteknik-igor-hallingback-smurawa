using Backend.Application.Interfaces;
using Backend.Application.Modules.Courses.Inputs;
using Backend.Application.Modules.Courses.Outputs;
using Backend.Domain.Modules.Courses.Models;

namespace Backend.Application.Modules.Courses
{
    public class CourseService(ICoursesRepository courseRepository) : ICourseService
    {
        private readonly ICoursesRepository _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));

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

                if (string.IsNullOrWhiteSpace(course.Title))
                {
                    return new CourseResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Result = null,
                        Message = "Course title cannot be empty or whitespace."
                    };
                }

                if (string.IsNullOrWhiteSpace(course.Description))
                {
                    return new CourseResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Result = null,
                        Message = "Course description cannot be empty or whitespace."
                    };
                }

                if (course.DurationInDays <= 0)
                {
                    return new CourseResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Result = null,
                        Message = "Course duration must be greater than zero."
                    };
                }

                var newCourse = new Course(
                    id: Guid.NewGuid(),
                    course.Title,
                    course.Description,
                    course.DurationInDays
                );

                var result = await _courseRepository.CreateCourseAsync(newCourse, cancellationToken);

                return new CourseResult
                {
                    Success = true,
                    StatusCode = 201,
                    Result = newCourse,
                    Message = "Course created successfully."
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
                var courses = await _courseRepository.GetAllCoursesAsync(cancellationToken);

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
                    Result = courses,
                    Message = $"Retrieved {courses.Count()} course(s) successfully."
                };
            }
            catch (Exception ex)
            {
                return new CourseListResult
                {
                    Success = false,
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
                        Message = "Course ID cannot be empty."
                    };
                }

                var course = await _courseRepository.GetCourseByIdAsync(courseId, cancellationToken);

                if (course == null)
                {
                    return new CourseWithEventsResult
                    {
                        Success = false,
                        Message = $"Course with ID '{courseId}' not found."
                    };
                }

                return new CourseWithEventsResult
                {
                    Success = true,
                    Result = course,
                    Message = "Course retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                return new CourseWithEventsResult
                {
                    Success = false,
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
                        Message = "Course cannot be null."
                    };
                }

                if (course.Id == Guid.Empty)
                {
                    return new CourseResult
                    {
                        Success = false,
                        Message = "Course ID cannot be empty."
                    };
                }

                if (string.IsNullOrWhiteSpace(course.Title))
                {
                    return new CourseResult
                    {
                        Success = false,
                        Message = "Course title cannot be empty or whitespace."
                    };
                }

                if (string.IsNullOrWhiteSpace(course.Description))
                {
                    return new CourseResult
                    {
                        Success = false,
                        Message = "Course description cannot be empty or whitespace."
                    };
                }

                if (course.DurationInDays <= 0)
                {
                    return new CourseResult
                    {
                        Success = false,
                        Message = "Course duration must be greater than zero."
                    };
                }

                var existingCourse = await _courseRepository.GetCourseByIdAsync(course.Id, cancellationToken);
                if (existingCourse == null)
                {
                    return new CourseResult
                    {
                        Success = false,
                        Message = $"Course with ID '{course.Id}' not found."
                    };
                }

                var updatedCourse = new Course(
                    course.Id,
                    course.Title,
                    course.Description,
                    course.DurationInDays
                    );

                var result = await _courseRepository.UpdateCourseAsync(updatedCourse, cancellationToken);

                if (result == null)
                {
                    return new CourseResult
                    {
                        Success = false,
                        Message = "Failed to update course."
                    };
                }

                return new CourseResult
                {
                    Success = true,
                    Result = result,
                    Message = "Course updated successfully."
                };
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("modified by another user"))
            {
                return new CourseResult
                {
                    Success = false,
                    Message = "The course was modified by another user. Please refresh and try again."
                };
            }
            catch (Exception ex)
            {
                return new CourseResult
                {
                    Success = false,
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
                        Message = "Course ID cannot be empty.",
                        Result = false
                    };
                }

                var existingCourse = await _courseRepository.GetCourseByIdAsync(courseId, cancellationToken);
                if (existingCourse == null)
                {
                    return new CourseDeleteResult
                    {
                        Success = false,
                        Message = $"Course with ID '{courseId}' not found.",
                        Result = false
                    };
                }

                var result = await _courseRepository.DeleteCourseAsync(courseId, cancellationToken);

                if (!result)
                {
                    return new CourseDeleteResult
                    {
                        Success = false,
                        Message = "Failed to delete course.",
                        Result = false
                    };
                }

                return new CourseDeleteResult
                {
                    Success = true,
                    Message = "Course deleted successfully.",
                    Result = true
                };
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("associated course events"))
            {
                return new CourseDeleteResult
                {
                    Success = false,
                    Message = "Cannot delete course because it has associated course events. Please delete the course events first.",
                    Result = false
                };
            }
            catch (Exception ex)
            {
                return new CourseDeleteResult
                {
                    Success = false,
                    Message = $"An error occurred while deleting the course: {ex.Message}",
                    Result = false
                };
            }
        }
    }
}
