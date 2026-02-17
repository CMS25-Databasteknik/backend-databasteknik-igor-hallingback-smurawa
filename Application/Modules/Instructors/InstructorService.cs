using Backend.Application.Modules.Instructors.Inputs;
using Backend.Application.Modules.Instructors.Outputs;
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

            if (string.IsNullOrWhiteSpace(instructor.Name))
            {
                return new InstructorResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "Name cannot be empty or whitespace."
                };
            }

            if (instructor.InstructorRoleId < 1)
            {
                return new InstructorResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "Instructor role ID must be greater than zero."
                };
            }

            var role = await _instructorRoleRepository.GetInstructorRoleByIdAsync(instructor.InstructorRoleId, cancellationToken);
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

            var result = await _instructorRepository.CreateInstructorAsync(newInstructor, cancellationToken);

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
            var instructors = await _instructorRepository.GetAllInstructorsAsync(cancellationToken);

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

    public async Task<InstructorResult> GetInstructorByIdAsync(Guid instructorId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (instructorId == Guid.Empty)
            {
                return new InstructorResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Instructor ID cannot be empty."
                };
            }

            var result = await _instructorRepository.GetInstructorByIdAsync(instructorId, cancellationToken);

            if (result == null)
            {
                return new InstructorResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Instructor with ID '{instructorId}' not found."
                };
            }

            return new InstructorResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Instructor retrieved successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new InstructorResult
            {
                Success = false,
                StatusCode = 404,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new InstructorResult
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

            if (instructor.Id == Guid.Empty)
            {
                return new InstructorResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Instructor ID cannot be empty."
                };
            }

            if (string.IsNullOrWhiteSpace(instructor.Name))
            {
                return new InstructorResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Name cannot be empty or whitespace."
                };
            }

            var existingInstructor = await _instructorRepository.GetInstructorByIdAsync(instructor.Id, cancellationToken);
            if (existingInstructor == null)
            {
                return new InstructorResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Instructor with ID '{instructor.Id}' not found."
                };
            }

            if (instructor.InstructorRoleId < 1)
            {
                return new InstructorResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Instructor role ID must be greater than zero."
                };
            }

            var role = await _instructorRoleRepository.GetInstructorRoleByIdAsync(instructor.InstructorRoleId, cancellationToken);
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

            var result = await _instructorRepository.UpdateInstructorAsync(updatedInstructor, cancellationToken);

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

            var existingInstructor = await _instructorRepository.GetInstructorByIdAsync(instructorId, cancellationToken);
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

            var result = await _instructorRepository.DeleteInstructorAsync(instructorId, cancellationToken);

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
