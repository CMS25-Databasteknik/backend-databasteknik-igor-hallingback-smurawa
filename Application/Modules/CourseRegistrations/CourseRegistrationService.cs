using Backend.Application.Modules.CourseRegistrations.Inputs;
using Backend.Application.Modules.CourseRegistrations.Outputs;
using Backend.Domain.Modules.CourseEvents.Contracts;
using Backend.Domain.Modules.CourseRegistrationStatuses.Contracts;
using Backend.Domain.Modules.CourseRegistrationStatuses.Models;
using Backend.Domain.Modules.CourseRegistrations.Contracts;
using Backend.Domain.Modules.CourseRegistrations.Models;
using Backend.Domain.Modules.PaymentMethod.Contracts;
using Backend.Domain.Modules.PaymentMethod.Models;
using Backend.Domain.Modules.Participants.Contracts;

namespace Backend.Application.Modules.CourseRegistrations;

public class CourseRegistrationService(
    ICourseRegistrationRepository courseRegistrationRepository,
    IParticipantRepository participantRepository,
    ICourseEventRepository courseEventRepository,
    ICourseRegistrationStatusRepository statusRepository,
    IPaymentMethodRepository paymentMethodRepository) : ICourseRegistrationService
{
    private readonly ICourseRegistrationRepository _courseRegistrationRepository = courseRegistrationRepository ?? throw new ArgumentNullException(nameof(courseRegistrationRepository));
    private readonly IParticipantRepository _participantRepository = participantRepository ?? throw new ArgumentNullException(nameof(participantRepository));
    private readonly ICourseEventRepository _courseEventRepository = courseEventRepository ?? throw new ArgumentNullException(nameof(courseEventRepository));
    private readonly ICourseRegistrationStatusRepository _statusRepository = statusRepository ?? throw new ArgumentNullException(nameof(statusRepository));
    private readonly IPaymentMethodRepository _paymentMethodRepository = paymentMethodRepository ?? throw new ArgumentNullException(nameof(paymentMethodRepository));

    public async Task<CourseRegistrationResult> CreateCourseRegistrationAsync(CreateCourseRegistrationInput courseRegistration, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseRegistration == null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "Course registration cannot be null."
                };
            }

            if (courseRegistration.ParticipantId == Guid.Empty)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "Participant ID cannot be empty."
                };
            }

            if (courseRegistration.CourseEventId == Guid.Empty)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Result = null,
                    Message = "Course event ID cannot be empty."
                };
            }

            var participant = await _participantRepository.GetByIdAsync(courseRegistration.ParticipantId, cancellationToken);
            if (participant is null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 404,
                    Result = null,
                    Message = $"Participant with ID '{courseRegistration.ParticipantId}' not found."
                };
            }

            var courseEvent = await _courseEventRepository.GetByIdAsync(courseRegistration.CourseEventId, cancellationToken);
            if (courseEvent is null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 404,
                    Result = null,
                    Message = $"Course event with ID '{courseRegistration.CourseEventId}' not found."
                };
            }

            var status = await _statusRepository.GetByIdAsync(courseRegistration.StatusId, cancellationToken);
            if (status is null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 404,
                    Result = null,
                    Message = $"Course registration status with ID '{courseRegistration.StatusId}' not found."
                };
            }

            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(courseRegistration.PaymentMethodId, cancellationToken);
            if (paymentMethod is null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 404,
                    Result = null,
                    Message = $"Payment method with ID '{courseRegistration.PaymentMethodId}' not found."
                };
            }

            var newCourseRegistration = new CourseRegistration(
                Guid.NewGuid(),
                courseRegistration.ParticipantId,
                courseRegistration.CourseEventId,
                DateTime.UtcNow,
                status,
                paymentMethod
            );

            var result = await _courseRegistrationRepository.AddAsync(newCourseRegistration, cancellationToken);

            return new CourseRegistrationResult
            {
                Success = true,
                StatusCode = 201,
                Result = result,
                Message = "Course registration created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationResult
            {
                Success = false,
                StatusCode = 400,
                Result = null,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationResult
            {
                Success = false,
                StatusCode = 500,
                Result = null,
                Message = $"An error occurred while creating the course registration: {ex.Message}"
            };
        }
    }

    public async Task<CourseRegistrationListResult> GetAllCourseRegistrationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var courseRegistrations = await _courseRegistrationRepository.GetAllAsync(cancellationToken);

            if (!courseRegistrations.Any())
            {
                return new CourseRegistrationListResult
                {
                    Success = true,
                    Result = courseRegistrations,
                    StatusCode = 200,
                    Message = "No course registrations found."
                };
            }

            return new CourseRegistrationListResult
            {
                Success = true,
                StatusCode = 200,
                Result = courseRegistrations,
                Message = $"Retrieved {courseRegistrations.Count} course registration(s) successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationListResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving course registrations: {ex.Message}"
            };
        }
    }

    public async Task<CourseRegistrationDetailsResult> GetCourseRegistrationByIdAsync(Guid courseRegistrationId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseRegistrationId == Guid.Empty)
            {
                return new CourseRegistrationDetailsResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course registration ID cannot be empty."
                };
            }

            var result = await _courseRegistrationRepository.GetByIdAsync(courseRegistrationId, cancellationToken);

            if (result == null)
            {
                return new CourseRegistrationDetailsResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course registration with ID '{courseRegistrationId}' not found."
                };
            }

            var participant = await _participantRepository.GetByIdAsync(result.ParticipantId, cancellationToken);
            var participantName = participant == null
                ? result.ParticipantId.ToString()
                : $"{participant.FirstName} {participant.LastName}".Trim();

            var courseEvent = await _courseEventRepository.GetByIdAsync(result.CourseEventId, cancellationToken);

            var details = new CourseRegistrationDetails(
                result.Id,
                new RegistrationGuidLookupItem(
                    result.ParticipantId,
                    participantName),
                new RegistrationCourseEventItem(
                    result.CourseEventId,
                    courseEvent?.EventDate),
                result.RegistrationDate,
                new RegistrationLookupItem(result.Status.Id, result.Status.Name),
                new RegistrationLookupItem(result.PaymentMethod.Id, result.PaymentMethod.Name)
            );

            return new CourseRegistrationDetailsResult
            {
                Success = true,
                StatusCode = 200,
                Result = details,
                Message = "Course registration retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationDetailsResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving the course registration: {ex.Message}"
            };
        }
    }

    public async Task<CourseRegistrationListResult> GetCourseRegistrationsByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (participantId == Guid.Empty)
            {
                return new CourseRegistrationListResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Participant ID cannot be empty."
                };
            }

            var courseRegistrations = await _courseRegistrationRepository.GetCourseRegistrationsByParticipantIdAsync(participantId, cancellationToken);

            if (!courseRegistrations.Any())
            {
                return new CourseRegistrationListResult
                {
                    Success = true,
                    Result = courseRegistrations,
                    StatusCode = 200,
                    Message = "No course registrations found for this participant."
                };
            }

            return new CourseRegistrationListResult
            {
                Success = true,
                StatusCode = 200,
                Result = courseRegistrations,
                Message = $"Retrieved {courseRegistrations.Count} course registration(s) for the participant successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationListResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving course registrations: {ex.Message}"
            };
        }
    }

    public async Task<CourseRegistrationListResult> GetCourseRegistrationsByCourseEventIdAsync(Guid courseEventId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseEventId == Guid.Empty)
            {
                return new CourseRegistrationListResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course event ID cannot be empty."
                };
            }

            var courseRegistrations = await _courseRegistrationRepository.GetCourseRegistrationsByCourseEventIdAsync(courseEventId, cancellationToken);

            if (!courseRegistrations.Any())
            {
                return new CourseRegistrationListResult
                {
                    Success = true,
                    Result = courseRegistrations,
                    StatusCode = 200,
                    Message = "No course registrations found for this course event."
                };
            }

            return new CourseRegistrationListResult
            {
                Success = true,
                StatusCode = 200,
                Result = courseRegistrations,
                Message = $"Retrieved {courseRegistrations.Count} course registration(s) for the course event successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationListResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while retrieving course registrations: {ex.Message}"
            };
        }
    }

    public async Task<CourseRegistrationResult> UpdateCourseRegistrationAsync(UpdateCourseRegistrationInput courseRegistration, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseRegistration == null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course registration cannot be null."
                };
            }

            if (courseRegistration.Id == Guid.Empty)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course registration ID cannot be empty."
                };
            }

            if (courseRegistration.ParticipantId == Guid.Empty)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Participant ID cannot be empty."
                };
            }

            if (courseRegistration.CourseEventId == Guid.Empty)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course event ID cannot be empty."
                };
            }

            var existingCourseRegistration = await _courseRegistrationRepository.GetByIdAsync(courseRegistration.Id, cancellationToken);
            if (existingCourseRegistration == null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course registration with ID '{courseRegistration.Id}' not found."
                };
            }

            var participant = await _participantRepository.GetByIdAsync(courseRegistration.ParticipantId, cancellationToken);
            if (participant is null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Participant with ID '{courseRegistration.ParticipantId}' not found."
                };
            }

            var courseEvent = await _courseEventRepository.GetByIdAsync(courseRegistration.CourseEventId, cancellationToken);
            if (courseEvent is null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course event with ID '{courseRegistration.CourseEventId}' not found."
                };
            }

            var status = await _statusRepository.GetByIdAsync(courseRegistration.StatusId, cancellationToken);
            if (status is null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course registration status with ID '{courseRegistration.StatusId}' not found."
                };
            }

            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(courseRegistration.PaymentMethodId, cancellationToken);
            if (paymentMethod is null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Payment method with ID '{courseRegistration.PaymentMethodId}' not found."
                };
            }

            existingCourseRegistration.Update(
                courseRegistration.ParticipantId,
                courseRegistration.CourseEventId,
                existingCourseRegistration.RegistrationDate,
                status,
                paymentMethod
            );

            var updatedCourseRegistration = await _courseRegistrationRepository.UpdateAsync(existingCourseRegistration.Id, existingCourseRegistration, cancellationToken);

            if (updatedCourseRegistration == null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to update course registration."
                };
            }

            return new CourseRegistrationResult
            {
                Success = true,
                StatusCode = 200,
                Result = updatedCourseRegistration,
                Message = "Course registration updated successfully."
            };
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("modified by another user"))
        {
            return new CourseRegistrationResult
            {
                Success = false,
                StatusCode = 409,
                Message = "The course registration was modified by another user. Please refresh and try again."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while updating the course registration: {ex.Message}"
            };
        }
    }

    public async Task<CourseRegistrationDeleteResult> DeleteCourseRegistrationAsync(Guid courseRegistrationId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseRegistrationId == Guid.Empty)
            {
                return new CourseRegistrationDeleteResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course registration ID cannot be empty.",
                    Result = false
                };
            }

            var existingCourseRegistration = await _courseRegistrationRepository.GetByIdAsync(courseRegistrationId, cancellationToken);
            if (existingCourseRegistration == null)
            {
                return new CourseRegistrationDeleteResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course registration with ID '{courseRegistrationId}' not found.",
                    Result = false
                };
            }

            var result = await _courseRegistrationRepository.RemoveAsync(courseRegistrationId, cancellationToken);

            if (!result)
            {
                return new CourseRegistrationDeleteResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to delete course registration.",
                    Result = false
                };
            }

            return new CourseRegistrationDeleteResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Course registration deleted successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationDeleteResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while deleting the course registration: {ex.Message}",
                Result = false
            };
        }
    }

}



