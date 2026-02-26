using Backend.Application.Common;
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
                    Error = ResultError.Validation,
                    Message = "Course registration status cannot be null."
                };
            }

            var existingCourseRegistrationStatus = await _repository.GetCourseRegistrationStatusByNameAsync(input.Name, cancellationToken);

            if (existingCourseRegistrationStatus is not null)
                return new CourseRegistrationStatusResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Result = null,
                    Message = "A status with the same name already exists."
                };

            var newStatus = new CourseRegistrationStatus(input.Name);
            var createdStatus = await _repository.AddAsync(newStatus, cancellationToken);
            _cache.ResetEntity(createdStatus);
            _cache.SetEntity(createdStatus);

            return new CourseRegistrationStatusResult
            {
                Success = true,
                                Result = createdStatus,
                Message = "Course registration status created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                Error = ResultError.Validation,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while creating the course registration status: {ex.Message}"
            };
        }
    }

    public async Task<CourseRegistrationStatusListResult> GetAllCourseRegistrationStatusesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var statuses = await _cache.GetAllAsync(
                token => _repository.GetAllAsync(token),
                cancellationToken);

            return new CourseRegistrationStatusListResult
            {
                Success = true,
                                Result = statuses,
                Message = statuses.Any()
                    ? $"Retrieved {statuses.Count} course registration status(es) successfully."
                    : "No course registration statuses found."
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationStatusListResult
            {
                Success = false,
                Error = ResultError.Unexpected,
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
                    Error = ResultError.NotFound,
                    Message = $"Course registration status with ID '{id}' not found."
                };
            }

            return new CourseRegistrationStatusResult
            {
                Success = true,
                                Result = status,
                Message = "Course registration status retrieved successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                Error = ResultError.Validation,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                Error = ResultError.Unexpected,
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
                    Error = ResultError.NotFound,
                    Message = $"Course registration status with name '{name}' not found."
                };
            }

            return new CourseRegistrationStatusResult
            {
                Success = true,
                                Result = status,
                Message = "Course registration status retrieved successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                Error = ResultError.Validation,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                Error = ResultError.Unexpected,
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
                    Error = ResultError.Validation,
                    Message = "Course registration status cannot be null."
                };
            }

            var existingStatus = await _repository.GetByIdAsync(input.Id, cancellationToken);
            if (existingStatus == null)
            {
                return new CourseRegistrationStatusResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
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
                    Error = ResultError.Unexpected,
                    Message = "Failed to update course registration status."
                };
            }

            _cache.ResetEntity(existingStatus);
            _cache.SetEntity(updatedStatus);

            return new CourseRegistrationStatusResult
            {
                Success = true,
                                Result = updatedStatus,
                Message = "Course registration status updated successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                Error = ResultError.Validation,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationStatusResult
            {
                Success = false,
                Error = ResultError.Unexpected,
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
                    Error = ResultError.NotFound,
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
                    Error = ResultError.Conflict,
                    Result = false,
                    Message = $"Cannot delete course registration status with ID '{id}' because it is in use."
                };
            }

            var deleted = await _repository.RemoveAsync(id, cancellationToken);
            if (!deleted)
            {
                return new CourseRegistrationStatusDeleteResult
                {
                    Success = false,
                    Error = ResultError.Unexpected,
                    Result = false,
                    Message = "Failed to delete course registration status."
                };
            }

            _cache.ResetEntity(existingStatus);

            return new CourseRegistrationStatusDeleteResult
            {
                Success = true,
                                Result = true,
                Message = "Course registration status deleted successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationStatusDeleteResult
            {
                Success = false,
                Error = ResultError.Validation,
                Result = false,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationStatusDeleteResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Result = false,
                Message = $"An error occurred while deleting the course registration status: {ex.Message}"
            };
        }
    }
}

