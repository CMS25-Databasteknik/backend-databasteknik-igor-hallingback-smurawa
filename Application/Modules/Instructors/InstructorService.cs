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
                    StatusCode = 400,
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
                    StatusCode = 404,
                    Result = null,
                    Message = $"Instructor role with ID '{instructor.InstructorRoleId}' not found."
                };
            }

            var newInstructor = new Instructor(Guid.NewGuid(), instructor.Name, role);

            var result = await _instructorRepository.AddAsync(newInstructor, cancellationToken);

            return new InstructorResult
            {
                Success = true,
                StatusCode = 201,
                Result = result,
                Message = "Instructor created successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new InstructorResult
            {
                Success = false,
                StatusCode = 404,
                Result = null,
                Message = ex.Message
            };
        }
        catch (ArgumentException ex)
        {
            return new InstructorResult
            {
                Success = false,
                StatusCode = 400,
                Result = null,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new InstructorResult
            {
                Success = false,
                StatusCode = 500,
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
                    StatusCode = 200,
                    Message = "No instructors found."
                };
            }

            return new InstructorListResult
            {
                Success = true,
                StatusCode = 200,
                Result = instructors,
                Message = $"Retrieved {instructors.Count} instructor(s) successfully."
            };
        }
        catch (Exception ex)
        {
            return new InstructorListResult
            {
                Success = false,
                StatusCode = 500,
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
                    StatusCode = 400,
                    Message = "Instructor ID cannot be empty."
                };
            }

            var result = await _instructorRepository.GetByIdAsync(instructorId, cancellationToken);

            if (result == null)
            {
                return new InstructorDetailsResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Instructor with ID '{instructorId}' not found."
                };
            }

            var details = new InstructorDetails(
                result.Id,
                result.Name,
                new InstructorLookupItem(result.Role.Id, result.Role.RoleName)
            );

            return new InstructorDetailsResult
            {
                Success = true,
                StatusCode = 200,
                Result = details,
                Message = "Instructor retrieved successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new InstructorDetailsResult
            {
                Success = false,
                StatusCode = 404,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new InstructorDetailsResult
            {
                Success = false,
                StatusCode = 500,
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
                    StatusCode = 400,
                    Message = "Instructor cannot be null."
                };
            }

            var role = await _instructorRoleRepository.GetByIdAsync(instructor.InstructorRoleId, cancellationToken);
            if (role == null)
            {
                return new InstructorResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Instructor role with ID '{instructor.InstructorRoleId}' not found."
                };
            }

            var updatedInstructor = new Instructor(instructor.Id, instructor.Name, role);

            var existingInstructor = await _instructorRepository.GetByIdAsync(instructor.Id, cancellationToken);
            if (existingInstructor == null)
            {
                return new InstructorResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Instructor with ID '{instructor.Id}' not found."
                };
            }

            var result = await _instructorRepository.UpdateAsync(updatedInstructor.Id, updatedInstructor, cancellationToken);

            if (result == null)
            {
                return new InstructorResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to update instructor."
                };
            }

            return new InstructorResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Instructor updated successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new InstructorResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new InstructorResult
            {
                Success = false,
                StatusCode = 500,
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
                    StatusCode = 400,
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
                    StatusCode = 404,
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
                    StatusCode = 409,
                    Message = $"Cannot delete instructor with ID '{instructorId}' because they are assigned to course events. Please remove the assignments first.",
                    Result = false
                };
            }

            var result = await _instructorRepository.RemoveAsync(instructorId, cancellationToken);

            if (!result)
            {
                return new InstructorDeleteResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to delete instructor.",
                    Result = false
                };
            }

            return new InstructorDeleteResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Instructor deleted successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new InstructorDeleteResult
            {
                Success = false,
                StatusCode = 404,
                Message = ex.Message,
                Result = false
            };
        }
        catch (Exception ex)
        {
            return new InstructorDeleteResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while deleting the instructor: {ex.Message}",
                Result = false
            };
        }
    }
}



