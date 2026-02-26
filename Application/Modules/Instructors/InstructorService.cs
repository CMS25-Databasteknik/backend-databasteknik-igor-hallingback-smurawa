using Backend.Application.Common;
using Backend.Application.Modules.Instructors.Inputs;
using Backend.Application.Modules.Instructors.Outputs;
using Backend.Domain.Modules.InstructorRoles.Contracts;
using Backend.Domain.Modules.Instructors.Contracts;
using Backend.Domain.Modules.Instructors.Models;

namespace Backend.Application.Modules.Instructors;

public class InstructorService(IInstructorRepository instructorRepository, IInstructorRoleRepository instructorRoleRepository) : IInstructorService
{
    private readonly IInstructorRepository _instructorRepository = instructorRepository ?? throw new ArgumentNullException(nameof(instructorRepository));
    private readonly IInstructorRoleRepository _instructorRoleRepository = instructorRoleRepository ?? throw new ArgumentNullException(nameof(instructorRoleRepository));

    public async Task<InstructorResult> CreateInstructorAsync(CreateInstructorInput instructor, CancellationToken cancellationToken = default)
    {
        try
        {
            if (instructor == null)
            {
                return new InstructorResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Result = null,
                    Message = "Instructor cannot be null."
                };
            }

            var role = await _instructorRoleRepository.GetByIdAsync(instructor.InstructorRoleId, cancellationToken);
            if (role == null)
            {
                return new InstructorResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Result = null,
                    Message = $"Instructor role with ID '{instructor.InstructorRoleId}' not found."
                };
            }

            var newInstructor = new Instructor(Guid.NewGuid(), instructor.Name, role);

            var createdInstructor = await _instructorRepository.AddAsync(newInstructor, cancellationToken);

            return new InstructorResult
            {
                Success = true,
                Result = createdInstructor,
                Message = "Instructor created successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new InstructorResult
            {
                Success = false,
                Error = ResultError.NotFound,
                Result = null,
                Message = ex.Message
            };
        }
        catch (ArgumentException ex)
        {
            return new InstructorResult
            {
                Success = false,
                Error = ResultError.Validation,
                Result = null,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new InstructorResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Result = null,
                Message = $"An error occurred while creating the instructor: {ex.Message}"
            };
        }
    }

    public async Task<InstructorListResult> GetAllInstructorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var instructors = await _instructorRepository.GetAllAsync(cancellationToken);

            if (!instructors.Any())
            {
                return new InstructorListResult
                {
                    Success = true,
                    Result = instructors,
                    Message = "No instructors found."
                };
            }

            return new InstructorListResult
            {
                Success = true,
                Result = instructors,
                Message = $"Retrieved {instructors.Count} instructor(s) successfully."
            };
        }
        catch (Exception ex)
        {
            return new InstructorListResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while retrieving instructors: {ex.Message}"
            };
        }
    }

    public async Task<InstructorDetailsResult> GetInstructorByIdAsync(Guid instructorId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (instructorId == Guid.Empty)
            {
                return new InstructorDetailsResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Instructor ID cannot be empty."
                };
            }

            var instructor = await _instructorRepository.GetByIdAsync(instructorId, cancellationToken);

            if (instructor == null)
            {
                return new InstructorDetailsResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Instructor with ID '{instructorId}' not found."
                };
            }

            var details = new InstructorDetails(
                instructor.Id,
                instructor.Name,
                new InstructorLookupItem(instructor.Role.Id, instructor.Role.RoleName)
            );

            return new InstructorDetailsResult
            {
                Success = true,
                Result = details,
                Message = "Instructor retrieved successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new InstructorDetailsResult
            {
                Success = false,
                Error = ResultError.NotFound,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new InstructorDetailsResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while retrieving the instructor: {ex.Message}"
            };
        }
    }

    public async Task<InstructorResult> UpdateInstructorAsync(UpdateInstructorInput instructor, CancellationToken cancellationToken = default)
    {
        try
        {
            if (instructor == null)
            {
                return new InstructorResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Instructor cannot be null."
                };
            }

            if (instructor.Id == Guid.Empty)
            {
                return new InstructorResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Instructor ID cannot be empty."
                };
            }

            var role = await _instructorRoleRepository.GetByIdAsync(instructor.InstructorRoleId, cancellationToken);
            if (role == null)
            {
                return new InstructorResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Instructor role with ID '{instructor.InstructorRoleId}' not found."
                };
            }

            var existingInstructor = await _instructorRepository.GetByIdAsync(instructor.Id, cancellationToken);
            if (existingInstructor == null)
            {
                return new InstructorResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Instructor with ID '{instructor.Id}' not found."
                };
            }

            existingInstructor.Update(instructor.Name, role);

            var updatedInstructor = await _instructorRepository.UpdateAsync(existingInstructor.Id, existingInstructor, cancellationToken);

            if (updatedInstructor == null)
            {
                return new InstructorResult
                {
                    Success = false,
                    Error = ResultError.Unexpected,
                    Message = "Failed to update instructor."
                };
            }

            return new InstructorResult
            {
                Success = true,
                Result = updatedInstructor,
                Message = "Instructor updated successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new InstructorResult
            {
                Success = false,
                Error = ResultError.Validation,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new InstructorResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while updating the instructor: {ex.Message}"
            };
        }
    }

    public async Task<InstructorDeleteResult> DeleteInstructorAsync(Guid instructorId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (instructorId == Guid.Empty)
            {
                return new InstructorDeleteResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Instructor ID cannot be empty.",
                    Result = false
                };
            }

            var existingInstructor = await _instructorRepository.GetByIdAsync(instructorId, cancellationToken);
            if (existingInstructor == null)
            {
                return new InstructorDeleteResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Instructor with ID '{instructorId}' not found.",
                    Result = false
                };
            }

            var hasCourseEvents = await _instructorRepository.HasCourseEventsAsync(instructorId, cancellationToken);
            if (hasCourseEvents)
            {
                return new InstructorDeleteResult
                {
                    Success = false,
                    Error = ResultError.Conflict,
                    Message = $"Cannot delete instructor with ID '{instructorId}' because they are assigned to course events. Please remove the assignments first.",
                    Result = false
                };
            }

            var isDeleted = await _instructorRepository.RemoveAsync(instructorId, cancellationToken);

            if (!isDeleted)
            {
                return new InstructorDeleteResult
                {
                    Success = false,
                    Error = ResultError.Unexpected,
                    Message = "Failed to delete instructor.",
                    Result = false
                };
            }

            return new InstructorDeleteResult
            {
                Success = true,
                Result = isDeleted,
                Message = "Instructor deleted successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new InstructorDeleteResult
            {
                Success = false,
                Error = ResultError.NotFound,
                Message = ex.Message,
                Result = false
            };
        }
        catch (Exception ex)
        {
            return new InstructorDeleteResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while deleting the instructor: {ex.Message}",
                Result = false
            };
        }
    }
}

