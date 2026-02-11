using Backend.Application.Interfaces;
using Backend.Application.Models;
using Backend.Application.Modules.Courses.Inputs;
using Backend.Application.Modules.Courses.Outputs;
using Backend.Domain.Models.Course;
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

                var existingCourse = await _courseRepository.GetCourseByTitleAsync(course.Title, cancellationToken);
                if (existingCourse != null)
                {
                    return new CourseResult
                    {
                        Success = false,
                        StatusCode = 409,
                        Result = null,
                        Message = $"A course with the title '{course.Title}' already exists."
                    };
                }

                var newCourse = new Course(
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

        public async Task<ResponseResult<IEnumerable<CourseSummaryDto>>> GetAllCoursesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var courses = await _courseRepository.GetAllCoursesAsync(cancellationToken);

                if (!courses.Any())
                {
                    return new ResponseResult<IEnumerable<CourseSummaryDto>>
                    {
                        Success = true,
                        Result = courses,
                        Message = "No courses found."
                    };
                }

                return new ResponseResult<IEnumerable<CourseSummaryDto>>
                {
                    Success = true,
                    Result = courses,
                    Message = $"Retrieved {courses.Count()} course(s) successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseResult<IEnumerable<CourseSummaryDto>>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving courses: {ex.Message}"
                };
            }
        }

        public async Task<ResponseResult<CourseDto>> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (courseId == Guid.Empty)
                {
                    return new ResponseResult<CourseDto>
                    {
                        Success = false,
                        Message = "Course ID cannot be empty."
                    };
                }

                var course = await _courseRepository.GetCourseByIdAsync(courseId, cancellationToken);

                if (course == null)
                {
                    return new ResponseResult<CourseDto>
                    {
                        Success = false,
                        Message = $"Course with ID '{courseId}' not found."
                    };
                }

                return new ResponseResult<CourseDto>
                {
                    Success = true,
                    Result = course,
                    Message = "Course retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseResult<CourseDto>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving the course: {ex.Message}"
                };
            }
        }

        public async Task<ResponseResult<CourseDto>> GetCourseByTitleAsync(string title, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    return new ResponseResult<CourseDto>
                    {
                        Success = false,
                        Message = "Course title cannot be empty or whitespace."
                    };
                }

                var course = await _courseRepository.GetCourseByTitleAsync(title, cancellationToken);

                if (course == null)
                {
                    return new ResponseResult<CourseDto>
                    {
                        Success = false,
                        Message = $"Course with title '{title}' not found."
                    };
                }

                return new ResponseResult<CourseDto>
                {
                    Success = true,
                    Result = course,
                    Message = "Course retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseResult<CourseDto>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving the course: {ex.Message}"
                };
            }
        }

        public async Task<ResponseResult<CourseSummaryDto>> UpdateCourseAsync(UpdateCourseDto course, CancellationToken cancellationToken = default)
        {
            try
            {
                if (course == null)
                {
                    return new ResponseResult<CourseSummaryDto>
                    {
                        Success = false,
                        Message = "Course cannot be null."
                    };
                }

                if (course.Id == Guid.Empty)
                {
                    return new ResponseResult<CourseSummaryDto>
                    {
                        Success = false,
                        Message = "Course ID cannot be empty."
                    };
                }

                if (string.IsNullOrWhiteSpace(course.Title))
                {
                    return new ResponseResult<CourseSummaryDto>
                    {
                        Success = false,
                        Message = "Course title cannot be empty or whitespace."
                    };
                }

                if (string.IsNullOrWhiteSpace(course.Description))
                {
                    return new ResponseResult<CourseSummaryDto>
                    {
                        Success = false,
                        Message = "Course description cannot be empty or whitespace."
                    };
                }

                if (course.DurationInDays <= 0)
                {
                    return new ResponseResult<CourseSummaryDto>
                    {
                        Success = false,
                        Message = "Course duration must be greater than zero."
                    };
                }

                var existingCourse = await _courseRepository.GetCourseByIdAsync(course.Id, cancellationToken);
                if (existingCourse == null)
                {
                    return new ResponseResult<CourseSummaryDto>
                    {
                        Success = false,
                        Message = $"Course with ID '{course.Id}' not found."
                    };
                }

                var courseWithSameTitle = await _courseRepository.GetCourseByTitleAsync(course.Title, cancellationToken);
                if (courseWithSameTitle != null && courseWithSameTitle.Id != course.Id)
                {
                    return new ResponseResult<CourseSummaryDto>
                    {
                        Success = false,
                        Message = $"Another course with the title '{course.Title}' already exists."
                    };
                }

                var result = await _courseRepository.UpdateCourseAsync(course, cancellationToken);

                if (result == null)
                {
                    return new ResponseResult<CourseSummaryDto>
                    {
                        Success = false,
                        Message = "Failed to update course."
                    };
                }

                return new ResponseResult<CourseSummaryDto>
                {
                    Success = true,
                    Result = result,
                    Message = "Course updated successfully."
                };
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("modified by another user"))
            {
                return new ResponseResult<CourseSummaryDto>
                {
                    Success = false,
                    Message = "The course was modified by another user. Please refresh and try again."
                };
            }
            catch (Exception ex)
            {
                return new ResponseResult<CourseSummaryDto>
                {
                    Success = false,
                    Message = $"An error occurred while updating the course: {ex.Message}"
                };
            }
        }

        public async Task<ResponseResult<bool>> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (courseId == Guid.Empty)
                {
                    return new ResponseResult<bool>
                    {
                        Success = false,
                        Message = "Course ID cannot be empty.",
                        Result = false
                    };
                }

                var existingCourse = await _courseRepository.GetCourseByIdAsync(courseId, cancellationToken);
                if (existingCourse == null)
                {
                    return new ResponseResult<bool>
                    {
                        Success = false,
                        Message = $"Course with ID '{courseId}' not found.",
                        Result = false
                    };
                }

                var result = await _courseRepository.DeleteCourseAsync(courseId, cancellationToken);

                if (!result)
                {
                    return new ResponseResult<bool>
                    {
                        Success = false,
                        Message = "Failed to delete course.",
                        Result = false
                    };
                }

                return new ResponseResult<bool>
                {
                    Success = true,
                    Message = "Course deleted successfully.",
                    Result = true
                };
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("associated course events"))
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Message = "Cannot delete course because it has associated course events. Please delete the course events first.",
                    Result = false
                };
            }
            catch (Exception ex)
            {
                return new ResponseResult<bool>
                {
                    Success = false,
                    Message = $"An error occurred while deleting the course: {ex.Message}",
                    Result = false
                };
            }
        }
    }
}
