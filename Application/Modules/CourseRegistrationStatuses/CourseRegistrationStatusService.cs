using Backend.Application.Modules.CourseRegistrationStatuses.Caching;
using Backend.Application.Modules.CourseRegistrationStatuses.Inputs;
using Backend.Application.Modules.CourseRegistrationStatuses.Outputs;
using Backend.Domain.Modules.CourseRegistrationStatuses.Contracts;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;

namespace Backend.Application.Modules.CourseRegistrationStatuses;

public class CourseRegistrationStatusService(ICourseRegistrationStatusCache cache, ICourseRegistrationStatusRepository repository) : ICourseRegistrationStatusService
{
    private readonly ICourseRegistrationStatusCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ICourseRegistrationStatusRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<CourseRegistrationStatusResult> CreateCourseRegistrationStatusAsync(CreateCourseRegistrationStatusInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
            {
                return new CourseRegistrationStatusResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course registration status cannot be null."
                };
            }

            var existingCourseRegistrationStatus = await _repository.GetCourseRegistrationStatusByNameAsync(input.Name, cancellationToken);

            if (existingCourseRegistrationStatus is not null)
                return new CourseRegistrationStatusResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "A status with the same name already exists."
                };

            var newStatus = new CourseRegistrationStatus(input.Name);
            var result = await _repository.AddAsync(newStatus, cancellationToken);
            _cache.ResetEntity(result);
            _cache.SetEntity(result);

            return new CourseRegistrationStatusResult
            {
                Success = true,
                StatusCode = 201,
                Result = result,
                Message = "Course registration status created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while creating the course registration status: {ex.Message}"
            };
        }
    }

    public async Task<CourseRegistrationStatusListResult> GetAllCourseRegistrationStatusesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cache.GetAllAsync(
                token => _repository.GetAllAsync(token),
                cancellationToken);

            return new CourseRegistrationStatusListResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = result.Any()
                    ? $"Retrieved {result.Count} course registration status(es) successfully."
                    : "No course registration statuses found."
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationStatusListResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving course registration statuses: {ex.Message}"
            };
        }
    }

    public async Task<CourseRegistrationStatusResult> GetCourseRegistrationStatusByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id < 0)
                throw new ArgumentException("Id must be zero or positive.", nameof(id));

            var status = await _cache.GetByIdAsync(
                id,
                token => _repository.GetByIdAsync(id, token),
                cancellationToken);
            if (status == null)
            {
                return new CourseRegistrationStatusResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course registration status with ID '{id}' not found."
                };
            }

            return new CourseRegistrationStatusResult
            {
                Success = true,
                StatusCode = 200,
                Result = status,
                Message = "Course registration status retrieved successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving the course registration status: {ex.Message}"
            };
        }
    }

    public async Task<CourseRegistrationStatusResult> GetCourseRegistrationStatusByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            var status = await _repository.GetCourseRegistrationStatusByNameAsync(name, cancellationToken);

            if (status == null)
            {
                return new CourseRegistrationStatusResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course registration status with name '{name}' not found."
                };
            }

            return new CourseRegistrationStatusResult
            {
                Success = true,
                StatusCode = 200,
                Result = status,
                Message = "Course registration status retrieved successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving the course registration status: {ex.Message}"
            };
        }
    }

    public async Task<CourseRegistrationStatusResult> UpdateCourseRegistrationStatusAsync(UpdateCourseRegistrationStatusInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
            {
                return new CourseRegistrationStatusResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course registration status cannot be null."
                };
            }

            var existingStatus = await _repository.GetByIdAsync(input.Id, cancellationToken);
            if (existingStatus == null)
            {
                return new CourseRegistrationStatusResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course registration status with ID '{input.Id}' not found."
                };
            }

            existingStatus.Update(input.Name);
            var updatedStatus = await _repository.UpdateAsync(existingStatus.Id, existingStatus, cancellationToken);

            if (updatedStatus == null)
            {
                return new CourseRegistrationStatusResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to update course registration status."
                };
            }

            _cache.ResetEntity(existingStatus);
            _cache.SetEntity(updatedStatus);

            return new CourseRegistrationStatusResult
            {
                Success = true,
                StatusCode = 200,
                Result = updatedStatus,
                Message = "Course registration status updated successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while updating the course registration status: {ex.Message}"
            };
        }
    }

    public async Task<CourseRegistrationStatusDeleteResult> DeleteCourseRegistrationStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id < 0)
                throw new ArgumentException("Id must be zero or positive.", nameof(id));

            var existingStatus = await _repository.GetByIdAsync(id, cancellationToken);

            if (existingStatus == null)
            {
                return new CourseRegistrationStatusDeleteResult
                {
                    Success = false,
                    StatusCode = 404,
                    Result = false,
                    Message = $"Course registration status with ID '{id}' not found."
                };
            }

            var isStatusInUse = await _repository.IsInUseAsync(id, cancellationToken);

            if (isStatusInUse)
            {
                return new CourseRegistrationStatusDeleteResult
                {
                    Success = false,
                    StatusCode = 409,
                    Result = false,
                    Message = $"Cannot delete course registration status with ID '{id}' because it is in use."
                };
            }

            var deleted = await _repository.RemoveAsync(id, cancellationToken);
            _cache.ResetEntity(existingStatus);

            return new CourseRegistrationStatusDeleteResult
            {
                Success = true,
                StatusCode = 200,
                Result = deleted,
                Message = "Course registration status deleted successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationStatusDeleteResult
            {
                Success = false,
                StatusCode = 400,
                Result = false,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationStatusDeleteResult
            {
                Success = false,
                StatusCode = 500,
                Result = false,
                Message = $"An error occurred while deleting the course registration status: {ex.Message}"
            };
        }
    }
}

