using Backend.Application.Common;
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
                    ErrorType = ErrorTypes.Validation,
                    Result = null,
                    Message = "Course registration cannot be null."
                };
            }

            if (courseRegistration.ParticipantId == Guid.Empty)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    ErrorType = ErrorTypes.Validation,
                    Result = null,
                    Message = "Participant ID cannot be empty."
                };
            }

            if (courseRegistration.CourseEventId == Guid.Empty)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    ErrorType = ErrorTypes.Validation,
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
                    ErrorType = ErrorTypes.NotFound,
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
                    ErrorType = ErrorTypes.NotFound,
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
                    ErrorType = ErrorTypes.NotFound,
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
                    ErrorType = ErrorTypes.NotFound,
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

            var createdCourseRegistration = await _courseRegistrationRepository.AddAsync(newCourseRegistration, cancellationToken);

            return new CourseRegistrationResult
            {
                Success = true,
                Result = createdCourseRegistration,
                Message = "Course registration created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationResult
            {
                Success = false,
                ErrorType = ErrorTypes.Validation,
                Result = null,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationResult
            {
                Success = false,
                ErrorType = ErrorTypes.Unexpected,
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
                    Message = "No course registrations found."
                };
            }

            return new CourseRegistrationListResult
            {
                Success = true,
                Result = courseRegistrations,
                Message = $"Retrieved {courseRegistrations.Count} course registration(s) successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationListResult
            {
                Success = false,
                ErrorType = ErrorTypes.Unexpected,
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
                    ErrorType = ErrorTypes.Validation,
                    Message = "Course registration ID cannot be empty."
                };
            }

            var courseRegistration = await _courseRegistrationRepository.GetByIdAsync(courseRegistrationId, cancellationToken);

            if (courseRegistration == null)
            {
                return new CourseRegistrationDetailsResult
                {
                    Success = false,
                    ErrorType = ErrorTypes.NotFound,
                    Message = $"Course registration with ID '{courseRegistrationId}' not found."
                };
            }

            var participant = await _participantRepository.GetByIdAsync(courseRegistration.ParticipantId, cancellationToken);
            var participantName = participant == null
                ? courseRegistration.ParticipantId.ToString()
                : $"{participant.FirstName} {participant.LastName}".Trim();

            var courseEvent = await _courseEventRepository.GetByIdAsync(courseRegistration.CourseEventId, cancellationToken);

            var details = new CourseRegistrationDetails(
                courseRegistration.Id,
                new RegistrationGuidLookupItem(
                    courseRegistration.ParticipantId,
                    participantName),
                new RegistrationCourseEventItem(
                    courseRegistration.CourseEventId,
                    courseEvent?.EventDate),
                courseRegistration.RegistrationDate,
                new RegistrationLookupItem(courseRegistration.Status.Id, courseRegistration.Status.Name),
                new RegistrationLookupItem(courseRegistration.PaymentMethod.Id, courseRegistration.PaymentMethod.Name)
            );

            return new CourseRegistrationDetailsResult
            {
                Success = true,
                Result = details,
                Message = "Course registration retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationDetailsResult
            {
                Success = false,
                ErrorType = ErrorTypes.Unexpected,
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
                    ErrorType = ErrorTypes.Validation,
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
                    Message = "No course registrations found for this participant."
                };
            }

            return new CourseRegistrationListResult
            {
                Success = true,
                Result = courseRegistrations,
                Message = $"Retrieved {courseRegistrations.Count} course registration(s) for the participant successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationListResult
            {
                Success = false,
                ErrorType = ErrorTypes.Unexpected,
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
                    ErrorType = ErrorTypes.Validation,
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
                    Message = "No course registrations found for this course event."
                };
            }

            return new CourseRegistrationListResult
            {
                Success = true,
                Result = courseRegistrations,
                Message = $"Retrieved {courseRegistrations.Count} course registration(s) for the course event successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationListResult
            {
                Success = false,
                ErrorType = ErrorTypes.Unexpected,
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
                    ErrorType = ErrorTypes.Validation,
                    Message = "Course registration cannot be null."
                };
            }

            if (courseRegistration.Id == Guid.Empty)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    ErrorType = ErrorTypes.Validation,
                    Message = "Course registration ID cannot be empty."
                };
            }

            if (courseRegistration.ParticipantId == Guid.Empty)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    ErrorType = ErrorTypes.Validation,
                    Message = "Participant ID cannot be empty."
                };
            }

            if (courseRegistration.CourseEventId == Guid.Empty)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    ErrorType = ErrorTypes.Validation,
                    Message = "Course event ID cannot be empty."
                };
            }

            var existingCourseRegistration = await _courseRegistrationRepository.GetByIdAsync(courseRegistration.Id, cancellationToken);
            if (existingCourseRegistration == null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    ErrorType = ErrorTypes.NotFound,
                    Message = $"Course registration with ID '{courseRegistration.Id}' not found."
                };
            }

            var participant = await _participantRepository.GetByIdAsync(courseRegistration.ParticipantId, cancellationToken);
            if (participant is null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    ErrorType = ErrorTypes.NotFound,
                    Message = $"Participant with ID '{courseRegistration.ParticipantId}' not found."
                };
            }

            var courseEvent = await _courseEventRepository.GetByIdAsync(courseRegistration.CourseEventId, cancellationToken);
            if (courseEvent is null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    ErrorType = ErrorTypes.NotFound,
                    Message = $"Course event with ID '{courseRegistration.CourseEventId}' not found."
                };
            }

            var status = await _statusRepository.GetByIdAsync(courseRegistration.StatusId, cancellationToken);
            if (status is null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    ErrorType = ErrorTypes.NotFound,
                    Message = $"Course registration status with ID '{courseRegistration.StatusId}' not found."
                };
            }

            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(courseRegistration.PaymentMethodId, cancellationToken);
            if (paymentMethod is null)
            {
                return new CourseRegistrationResult
                {
                    Success = false,
                    ErrorType = ErrorTypes.NotFound,
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
                    ErrorType = ErrorTypes.Unexpected,
                    Message = "Failed to update course registration."
                };
            }

            return new CourseRegistrationResult
            {
                Success = true,
                Result = updatedCourseRegistration,
                Message = "Course registration updated successfully."
            };
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("modified by another user"))
        {
            return new CourseRegistrationResult
            {
                Success = false,
                ErrorType = ErrorTypes.Conflict,
                Message = "The course registration was modified by another user. Please refresh and try again."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseRegistrationResult
            {
                Success = false,
                ErrorType = ErrorTypes.Validation,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationResult
            {
                Success = false,
                ErrorType = ErrorTypes.Unexpected,
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
                    ErrorType = ErrorTypes.Validation,
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
                    ErrorType = ErrorTypes.NotFound,
                    Message = $"Course registration with ID '{courseRegistrationId}' not found.",
                    Result = false
                };
            }

            var isDeleted = await _courseRegistrationRepository.RemoveAsync(courseRegistrationId, cancellationToken);

            if (!isDeleted)
            {
                return new CourseRegistrationDeleteResult
                {
                    Success = false,
                    ErrorType = ErrorTypes.Unexpected,
                    Message = "Failed to delete course registration.",
                    Result = false
                };
            }

            return new CourseRegistrationDeleteResult
            {
                Success = true,
                Result = isDeleted,
                Message = "Course registration deleted successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseRegistrationDeleteResult
            {
                Success = false,
                ErrorType = ErrorTypes.Unexpected,
                Message = $"An error occurred while deleting the course registration: {ex.Message}",
                Result = false
            };
        }
    }

}

