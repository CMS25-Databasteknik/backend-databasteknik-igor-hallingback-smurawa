using Backend.Application.Modules.CourseEventTypes.Caching;
using Backend.Application.Modules.CourseEventTypes.Inputs;
using Backend.Application.Modules.CourseEventTypes.Outputs;
using Backend.Domain.Modules.CourseEventTypes.Contracts;
using Backend.Domain.Modules.CourseEventTypes.Models;

namespace Backend.Application.Modules.CourseEventTypes;

public class CourseEventTypeService(ICourseEventTypeCache cache, ICourseEventTypeRepository courseEventTypeRepository) : ICourseEventTypeService
{
    private readonly ICourseEventTypeCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
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

            var existingCourseEventType = await _courseEventTypeRepository.GetCourseEventTypeByTypeNameAsync(courseEventType.TypeName, cancellationToken);

            if (existingCourseEventType is not null)
                return new CourseEventTypeResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "A typename with the same name already exists."
                };

            var newCourseEventType = new CourseEventType(courseEventType.TypeName);

            var createdCourseEventType = await _courseEventTypeRepository.AddAsync(newCourseEventType, cancellationToken);
            _cache.ResetEntity(createdCourseEventType);
            _cache.SetEntity(createdCourseEventType);

            return new CourseEventTypeResult
            {
                Success = true,
                StatusCode = 201,
                Result = createdCourseEventType,
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
            var courseEventTypes = await _cache.GetAllAsync(
                token => _courseEventTypeRepository.GetAllAsync(token),
                cancellationToken);

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

            var courseEventType = await _cache.GetByIdAsync(
                courseEventTypeId,
                token => _courseEventTypeRepository.GetByIdAsync(courseEventTypeId, token),
                cancellationToken);

            if (courseEventType == null)
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
                Result = courseEventType,
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

    public async Task<CourseEventTypeResult> GetCourseEventTypeByTypeNameAsync(string typeName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return new CourseEventTypeResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course event type name is required."
                };
            }

            var courseEventType = await _courseEventTypeRepository.GetCourseEventTypeByTypeNameAsync(typeName, cancellationToken);

            if (courseEventType == null)
            {
                return new CourseEventTypeResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course event type with name '{typeName}' not found."
                };
            }

            return new CourseEventTypeResult
            {
                Success = true,
                StatusCode = 200,
                Result = courseEventType,
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

            var existingCourseEventType = await _courseEventTypeRepository.GetByIdAsync(courseEventType.Id, cancellationToken);

            if (existingCourseEventType == null)
            {
                return new CourseEventTypeResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course event type with ID '{courseEventType.Id}' not found."
                };
            }

            existingCourseEventType.Update(courseEventType.TypeName);

            var updatedCourseEventType = await _courseEventTypeRepository.UpdateAsync(existingCourseEventType.Id, existingCourseEventType, cancellationToken);

            if (updatedCourseEventType == null)
            {
                return new CourseEventTypeResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to update course event type."
                };
            }

            _cache.ResetEntity(existingCourseEventType);
            _cache.SetEntity(updatedCourseEventType);

            return new CourseEventTypeResult
            {
                Success = true,
                StatusCode = 200,
                Result = updatedCourseEventType,
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

            var existingCourseEventType = await _courseEventTypeRepository.GetByIdAsync(courseEventTypeId, cancellationToken);
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

            var isDeleted = await _courseEventTypeRepository.RemoveAsync(courseEventTypeId, cancellationToken);
            if (!isDeleted)
            {
                return new CourseEventTypeDeleteResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to delete course event type.",
                    Result = false
                };
            }

            _cache.ResetEntity(existingCourseEventType);

            return new CourseEventTypeDeleteResult
            {
                Success = true,
                StatusCode = 200,
                Result = true,
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

