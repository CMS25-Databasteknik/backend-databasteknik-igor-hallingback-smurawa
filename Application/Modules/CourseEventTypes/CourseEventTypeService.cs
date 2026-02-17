using Backend.Application.Modules.CourseEventTypes.Inputs;
using Backend.Application.Modules.CourseEventTypes.Outputs;
using Backend.Domain.Modules.CourseEventTypes.Contracts;
using Backend.Domain.Modules.CourseEventTypes.Models;

namespace Backend.Application.Modules.CourseEventTypes;

public class CourseEventTypeService(ICourseEventTypeRepository courseEventTypeRepository) : ICourseEventTypeService
{
    private readonly ICourseEventTypeRepository _courseEventTypeRepository = courseEventTypeRepository ?? throw new ArgumentNullException(nameof(courseEventTypeRepository));

    public async Task<CourseEventTypeResult> CreateCourseEventTypeAsync(CreateCourseEventTypeInput courseEventType, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseEventType == null)
            {
                return new CourseEventTypeResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "Course event type cannot be null."
                };
            }

            var newCourseEventType = new CourseEventType(1, courseEventType.TypeName);

            var result = await _courseEventTypeRepository.CreateCourseEventTypeAsync(newCourseEventType, cancellationToken);

            return new CourseEventTypeResult
            {
                Success = true,
                StatusCode = 201,
                Result = result,
                Message = "Course event type created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseEventTypeResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseEventTypeResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseEventTypeResult
            {
                Success = false,
                StatusCode = 500,
                Result = null,
                Message = $"An error occurred while creating the course event type: {ex.Message}"
            };
        }
    }

    public async Task<CourseEventTypeListResult> GetAllCourseEventTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var courseEventTypes = await _courseEventTypeRepository.GetAllCourseEventTypesAsync(cancellationToken);

            if (!courseEventTypes.Any())
            {
                return new CourseEventTypeListResult
                {
                    Success = true,
                    Result = courseEventTypes,
                    StatusCode = 200,
                    Message = "No course event types found."
                };
            }

            return new CourseEventTypeListResult
            {
                Success = true,
                StatusCode = 200,
                Result = courseEventTypes,
                Message = $"Retrieved {courseEventTypes.Count} course event type(s) successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseEventTypeListResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving course event types: {ex.Message}"
            };
        }
    }

    public async Task<CourseEventTypeResult> GetCourseEventTypeByIdAsync(int courseEventTypeId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseEventTypeId <= 0)
            {
                return new CourseEventTypeResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course event type ID must be greater than zero."
                };
            }

            var result = await _courseEventTypeRepository.GetCourseEventTypeByIdAsync(courseEventTypeId, cancellationToken);

            if (result == null)
            {
                return new CourseEventTypeResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course event type with ID '{courseEventTypeId}' not found."
                };
            }

            return new CourseEventTypeResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Course event type retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseEventTypeResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving the course event type: {ex.Message}"
            };
        }
    }

    public async Task<CourseEventTypeResult> UpdateCourseEventTypeAsync(UpdateCourseEventTypeInput courseEventType, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseEventType == null)
            {
                return new CourseEventTypeResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course event type cannot be null."
                };
            }

            var existingCourseEventType = await _courseEventTypeRepository.GetCourseEventTypeByIdAsync(courseEventType.Id, cancellationToken);
            if (existingCourseEventType == null)
            {
                return new CourseEventTypeResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course event type with ID '{courseEventType.Id}' not found."
                };
            }

            var updatedCourseEventType = new CourseEventType(courseEventType.Id, courseEventType.TypeName);

            var result = await _courseEventTypeRepository.UpdateCourseEventTypeAsync(updatedCourseEventType, cancellationToken);

            if (result == null)
            {
                return new CourseEventTypeResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to update course event type."
                };
            }

            return new CourseEventTypeResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Course event type updated successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseEventTypeResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseEventTypeResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while updating the course event type: {ex.Message}"
            };
        }
    }

    public async Task<CourseEventTypeDeleteResult> DeleteCourseEventTypeAsync(int courseEventTypeId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseEventTypeId <= 0)
            {
                return new CourseEventTypeDeleteResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course event type ID must be greater than zero.",
                    Result = false
                };
            }

            var existingCourseEventType = await _courseEventTypeRepository.GetCourseEventTypeByIdAsync(courseEventTypeId, cancellationToken);
            if (existingCourseEventType == null)
            {
                return new CourseEventTypeDeleteResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course event type with ID '{courseEventTypeId}' not found.",
                    Result = false
                };
            }

            var isInUse = await _courseEventTypeRepository.IsInUseAsync(courseEventTypeId, cancellationToken);
            if (isInUse)
            {
                return new CourseEventTypeDeleteResult
                {
                    Success = false,
                    StatusCode = 409,
                    Message = $"Cannot delete course event type with ID '{courseEventTypeId}' because it is being used by one or more course events.",
                    Result = false
                };
            }

            var result = await _courseEventTypeRepository.DeleteCourseEventTypeAsync(courseEventTypeId, cancellationToken);

            return new CourseEventTypeDeleteResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Course event type deleted successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseEventTypeDeleteResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while deleting the course event type: {ex.Message}",
                Result = false
            };
        }
    }
}
