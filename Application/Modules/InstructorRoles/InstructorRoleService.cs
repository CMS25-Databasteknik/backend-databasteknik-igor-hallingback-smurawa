using Backend.Application.Modules.InstructorRoles.Inputs;
using Backend.Application.Modules.InstructorRoles.Outputs;
using Backend.Domain.Modules.Instructors.Contracts;
using Backend.Domain.Modules.Instructors.Models;

namespace Backend.Application.Modules.InstructorRoles;

public class InstructorRoleService(IInstructorRoleRepository repository) : IInstructorRoleService
{
    private readonly IInstructorRoleRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<InstructorRoleResult> CreateInstructorRoleAsync(CreateInstructorRoleInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
            {
                return new InstructorRoleResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Role cannot be null."
                };
            }

            var role = new InstructorRole(input.RoleName);
            var created = await _repository.CreateInstructorRoleAsync(role, cancellationToken);

            return new InstructorRoleResult
            {
                Success = true,
                StatusCode = 201,
                Result = created,
                Message = "Instructor role created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new InstructorRoleResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new InstructorRoleResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while creating the instructor role: {ex.Message}"
            };
        }
    }

    public async Task<InstructorRoleListResult> GetAllInstructorRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var roles = await _repository.GetAllInstructorRolesAsync(cancellationToken);
            return new InstructorRoleListResult
            {
                Success = true,
                StatusCode = 200,
                Result = roles,
                Message = roles.Any() ? "Instructor roles retrieved successfully." : "No instructor roles found."
            };
        }
        catch (Exception ex)
        {
            return new InstructorRoleListResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving instructor roles: {ex.Message}"
            };
        }
    }

    public async Task<InstructorRoleResult> GetInstructorRoleByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id < 1)
            {
                return new InstructorRoleResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Id must be greater than zero."
                };
            }

            var role = await _repository.GetInstructorRoleByIdAsync(id, cancellationToken);
            if (role == null)
            {
                return new InstructorRoleResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Instructor role with ID '{id}' not found."
                };
            }

            return new InstructorRoleResult
            {
                Success = true,
                StatusCode = 200,
                Result = role,
                Message = "Instructor role retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new InstructorRoleResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving the instructor role: {ex.Message}"
            };
        }
    }

    public async Task<InstructorRoleResult> UpdateInstructorRoleAsync(UpdateInstructorRoleInput input, CancellationToken cancellationToken = default)
    {
        try
        {
            if (input == null)
            {
                return new InstructorRoleResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Role cannot be null."
                };
            }

            if (input.Id < 1)
            {
                return new InstructorRoleResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Id must be greater than zero."
                };
            }

            var updated = await _repository.UpdateInstructorRoleAsync(new InstructorRole(input.Id, input.RoleName), cancellationToken);
            if (updated == null)
            {
                return new InstructorRoleResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Instructor role with ID '{input.Id}' not found."
                };
            }

            return new InstructorRoleResult
            {
                Success = true,
                StatusCode = 200,
                Result = updated,
                Message = "Instructor role updated successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new InstructorRoleResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new InstructorRoleResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while updating the instructor role: {ex.Message}"
            };
        }
    }

    public async Task<InstructorRoleDeleteResult> DeleteInstructorRoleAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            if (id < 1)
            {
                return new InstructorRoleDeleteResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Id must be greater than zero.",
                    Result = false
                };
            }

            var result = await _repository.DeleteInstructorRoleAsync(id, cancellationToken);
            if (!result)
            {
                return new InstructorRoleDeleteResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Instructor role with ID '{id}' not found.",
                    Result = false
                };
            }

            return new InstructorRoleDeleteResult
            {
                Success = true,
                StatusCode = 200,
                Message = "Instructor role deleted successfully.",
                Result = true
            };
        }
        catch (Exception ex)
        {
            return new InstructorRoleDeleteResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while deleting the instructor role: {ex.Message}",
                Result = false
            };
        }
    }
}
