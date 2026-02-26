using Backend.Application.Common;
using Backend.Application.Modules.InstructorRoles.Caching;
using Backend.Application.Modules.InstructorRoles.Inputs;
using Backend.Application.Modules.InstructorRoles.Outputs;
using Backend.Domain.Modules.InstructorRoles.Contracts;
using Backend.Domain.Modules.InstructorRoles.Models;

namespace Backend.Application.Modules.InstructorRoles;

public class InstructorRoleService(IInstructorRoleCache cache, IInstructorRoleRepository repository) : IInstructorRoleService
{
    private readonly IInstructorRoleCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
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
                    Error = ResultError.Validation,
                    Message = "Role cannot be null."
                };
            }

            var role = new InstructorRole(input.RoleName);
            var created = await _repository.AddAsync(role, cancellationToken);
            _cache.ResetEntity(created);
            _cache.SetEntity(created);

            return new InstructorRoleResult
            {
                Success = true,
                Result = created,
                Message = "Instructor role created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new InstructorRoleResult
            {
                Success = false,
                Error = ResultError.Validation,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new InstructorRoleResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while creating the instructor role: {ex.Message}"
            };
        }
    }

    public async Task<InstructorRoleListResult> GetAllInstructorRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var roles = await _cache.GetAllAsync(
                token => _repository.GetAllAsync(token),
                cancellationToken);
            return new InstructorRoleListResult
            {
                Success = true,
                Result = roles,
                Message = roles.Any() ? "Instructor roles retrieved successfully." : "No instructor roles found."
            };
        }
        catch (Exception ex)
        {
            return new InstructorRoleListResult
            {
                Success = false,
                Error = ResultError.Unexpected,
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
                    Error = ResultError.Validation,
                    Message = "Id must be greater than zero."
                };
            }

            var role = await _cache.GetByIdAsync(
                id,
                token => _repository.GetByIdAsync(id, token),
                cancellationToken);
            if (role == null)
            {
                return new InstructorRoleResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Instructor role with ID '{id}' not found."
                };
            }

            return new InstructorRoleResult
            {
                Success = true,
                Result = role,
                Message = "Instructor role retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new InstructorRoleResult
            {
                Success = false,
                Error = ResultError.Unexpected,
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
                    Error = ResultError.Validation,
                    Message = "Role cannot be null."
                };
            }

            if (input.Id < 1)
            {
                return new InstructorRoleResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Id must be greater than zero."
                };
            }

            var existingRole = await _repository.GetByIdAsync(input.Id, cancellationToken);
            if (existingRole == null)
            {
                return new InstructorRoleResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Instructor role with ID '{input.Id}' not found."
                };
            }

            existingRole.Update(input.RoleName);
            var updatedInstructorRole = await _repository.UpdateAsync(existingRole.Id, existingRole, cancellationToken);
            if (updatedInstructorRole == null)
            {
                return new InstructorRoleResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Instructor role with ID '{input.Id}' not found."
                };
            }

            _cache.ResetEntity(existingRole);
            _cache.SetEntity(updatedInstructorRole);

            return new InstructorRoleResult
            {
                Success = true,
                Result = updatedInstructorRole,
                Message = "Instructor role updated successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new InstructorRoleResult
            {
                Success = false,
                Error = ResultError.Validation,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new InstructorRoleResult
            {
                Success = false,
                Error = ResultError.Unexpected,
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
                    Error = ResultError.Validation,
                    Message = "Id must be greater than zero.",
                    Result = false
                };
            }

            var existingRole = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingRole == null)
            {
                return new InstructorRoleDeleteResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Instructor role with ID '{id}' not found.",
                    Result = false
                };
            }

            var isDeleted = await _repository.RemoveAsync(id, cancellationToken);
            if (!isDeleted)
            {
                return new InstructorRoleDeleteResult
                {
                    Success = false,
                    Error = ResultError.Unexpected,
                    Message = "Failed to delete instructor role.",
                    Result = false
                };
            }

            _cache.ResetEntity(existingRole);

            return new InstructorRoleDeleteResult
            {
                Success = true,
                Message = "Instructor role deleted successfully.",
                Result = true
            };
        }
        catch (Exception ex) when (ex.GetType().Name == "DbUpdateException")
        {
            return new InstructorRoleDeleteResult
            {
                Success = false,
                Error = ResultError.Conflict,
                Message = $"Cannot delete instructor role with ID '{id}' because it is in use.",
                Result = false
            };
        }
        catch (Exception ex)
        {
            return new InstructorRoleDeleteResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while deleting the instructor role: {ex.Message}",
                Result = false
            };
        }
    }
}

