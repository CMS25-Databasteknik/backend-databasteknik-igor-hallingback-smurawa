using Backend.Application.Common;
using Backend.Application.Modules.CourseEvents.Inputs;
using Backend.Application.Modules.CourseEvents.Outputs;
using Backend.Domain.Modules.CourseEvents.Contracts;
using Backend.Domain.Modules.CourseEvents.Models;
using Backend.Domain.Modules.CourseEventTypes.Contracts;
using Backend.Domain.Modules.Courses.Contracts;
using Backend.Domain.Modules.VenueTypes.Contracts;

namespace Backend.Application.Modules.CourseEvents;

public class CourseEventService(
    ICourseEventRepository courseEventRepository,
    ICourseRepository courseRepository,
    ICourseEventTypeRepository courseEventTypeRepository,
    IVenueTypeRepository venueTypeRepository) : ICourseEventService
{
    private readonly ICourseEventRepository _courseEventRepository = courseEventRepository ?? throw new ArgumentNullException(nameof(courseEventRepository));
    private readonly ICourseRepository _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
    private readonly ICourseEventTypeRepository _courseEventTypeRepository = courseEventTypeRepository ?? throw new ArgumentNullException(nameof(courseEventTypeRepository));
    private readonly IVenueTypeRepository _venueTypeRepository = venueTypeRepository ?? throw new ArgumentNullException(nameof(venueTypeRepository));

    public async Task<CourseEventResult> CreateCourseEventAsync(CreateCourseEventInput courseEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseEvent == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Course event cannot be null."
                };
            }

            var existingCourse = await _courseRepository.GetByIdAsync(courseEvent.CourseId, cancellationToken);
            if (existingCourse == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Course with ID '{courseEvent.CourseId}' not found."
                };
            }

            var existingCourseEventType = await _courseEventTypeRepository.GetByIdAsync(courseEvent.CourseEventTypeId, cancellationToken);
            if (existingCourseEventType == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Course event type with ID '{courseEvent.CourseEventTypeId}' not found."
                };
            }

            var venueType = await _venueTypeRepository.GetByIdAsync(courseEvent.VenueTypeId, cancellationToken);
            if (venueType == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Venue type with ID '{courseEvent.VenueTypeId}' not found."
                };
            }

            var newCourseEvent = new CourseEvent(
                Guid.NewGuid(),
                courseEvent.CourseId,
                courseEvent.EventDate,
                courseEvent.Price,
                courseEvent.Seats,
                courseEvent.CourseEventTypeId,
                venueType);

            var createdCourseEvent = await _courseEventRepository.AddAsync(newCourseEvent, cancellationToken);

            return new CourseEventResult
            {
                Success = true,
                Result = createdCourseEvent,
                Message = "Course event created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseEventResult
            {
                Success = false,
                Error = ResultError.Validation,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseEventResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while creating the course event: {ex.Message}"
            };
        }
    }

    public async Task<CourseEventListResult> GetAllCourseEventsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var courseEvents = await _courseEventRepository.GetAllAsync(cancellationToken);

            return new CourseEventListResult
            {
                Success = true,
                Result = courseEvents,
                Message = courseEvents.Any()
                    ? $"Retrieved {courseEvents.Count} course event(s) successfully."
                    : "No course events found."
            };
        }
        catch (Exception ex)
        {
            return new CourseEventListResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while retrieving course events: {ex.Message}"
            };
        }
    }

    public async Task<CourseEventDetailsResult> GetCourseEventByIdAsync(Guid courseEventId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseEventId == Guid.Empty)
            {
                return new CourseEventDetailsResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Course event ID cannot be empty."
                };
            }

            var courseEvent = await _courseEventRepository.GetByIdAsync(courseEventId, cancellationToken);
            if (courseEvent == null)
            {
                return new CourseEventDetailsResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Course event with ID '{courseEventId}' not found."
                };
            }

            var details = new CourseEventDetails(
                courseEvent.Id,
                courseEvent.CourseId,
                courseEvent.EventDate,
                courseEvent.Price,
                courseEvent.Seats,
                new CourseEventLookupItem(courseEvent.CourseEventType.Id, courseEvent.CourseEventType.TypeName),
                new CourseEventLookupItem(courseEvent.VenueTypeId, courseEvent.VenueType.Name)
            );

            return new CourseEventDetailsResult
            {
                Success = true,
                Result = details,
                Message = "Course event retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseEventDetailsResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while retrieving the course event: {ex.Message}"
            };
        }
    }

    public async Task<CourseEventListResult> GetCourseEventsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseId == Guid.Empty)
            {
                return new CourseEventListResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Course ID cannot be empty."
                };
            }

            var courseEvents = await _courseEventRepository.GetCourseEventsByCourseIdAsync(courseId, cancellationToken);

            return new CourseEventListResult
            {
                Success = true,
                Result = courseEvents,
                Message = courseEvents.Any()
                    ? $"Retrieved {courseEvents.Count} course event(s) successfully."
                    : "No course events found for this course."
            };
        }
        catch (Exception ex)
        {
            return new CourseEventListResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while retrieving course events by course ID: {ex.Message}"
            };
        }
    }

    public async Task<CourseEventResult> UpdateCourseEventAsync(UpdateCourseEventInput courseEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseEvent == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Message = "Course event cannot be null."
                };
            }

            var existingCourseEvent = await _courseEventRepository.GetByIdAsync(courseEvent.Id, cancellationToken);
            if (existingCourseEvent == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Course event with ID '{courseEvent.Id}' not found."
                };
            }

            var existingCourse = await _courseRepository.GetByIdAsync(courseEvent.CourseId, cancellationToken);
            if (existingCourse == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Course with ID '{courseEvent.CourseId}' not found."
                };
            }

            var existingCourseEventType = await _courseEventTypeRepository.GetByIdAsync(courseEvent.CourseEventTypeId, cancellationToken);
            if (existingCourseEventType == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Course event type with ID '{courseEvent.CourseEventTypeId}' not found."
                };
            }

            var venueType = await _venueTypeRepository.GetByIdAsync(courseEvent.VenueTypeId, cancellationToken);
            if (venueType == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Message = $"Venue type with ID '{courseEvent.VenueTypeId}' not found."
                };
            }

            existingCourseEvent.Update(
                courseEvent.CourseId,
                courseEvent.EventDate,
                courseEvent.Price,
                courseEvent.Seats,
                courseEvent.CourseEventTypeId,
                venueType,
                existingCourseEventType);

            var updatedCourseEvent = await _courseEventRepository.UpdateAsync(existingCourseEvent.Id, existingCourseEvent, cancellationToken);
            if (updatedCourseEvent == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    Error = ResultError.Unexpected,
                    Message = "Failed to update course event."
                };
            }

            return new CourseEventResult
            {
                Success = true,
                Result = updatedCourseEvent,
                Message = "Course event updated successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new CourseEventResult
            {
                Success = false,
                Error = ResultError.NotFound,
                Message = ex.Message
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseEventResult
            {
                Success = false,
                Error = ResultError.Validation,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseEventResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Message = $"An error occurred while updating the course event: {ex.Message}"
            };
        }
    }

    public async Task<CourseEventDeleteResult> DeleteCourseEventAsync(Guid courseEventId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseEventId == Guid.Empty)
            {
                return new CourseEventDeleteResult
                {
                    Success = false,
                    Error = ResultError.Validation,
                    Result = false,
                    Message = "Course event ID cannot be empty."
                };
            }

            var existingCourseEvent = await _courseEventRepository.GetByIdAsync(courseEventId, cancellationToken);
            if (existingCourseEvent == null)
            {
                return new CourseEventDeleteResult
                {
                    Success = false,
                    Error = ResultError.NotFound,
                    Result = false,
                    Message = $"Course event with ID '{courseEventId}' not found."
                };
            }

            var hasRegistrations = await _courseEventRepository.HasRegistrationsAsync(courseEventId, cancellationToken);
            if (hasRegistrations)
            {
                return new CourseEventDeleteResult
                {
                    Success = false,
                    Error = ResultError.Conflict,
                    Result = false,
                    Message = $"Cannot delete course event with ID '{courseEventId}' because it has registrations."
                };
            }

            var isDeleted = await _courseEventRepository.RemoveAsync(courseEventId, cancellationToken);
            return new CourseEventDeleteResult
            {
                Success = true,
                Result = isDeleted,
                Message = "Course event deleted successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new CourseEventDeleteResult
            {
                Success = false,
                Error = ResultError.NotFound,
                Result = false,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseEventDeleteResult
            {
                Success = false,
                Error = ResultError.Unexpected,
                Result = false,
                Message = $"An error occurred while deleting the course event: {ex.Message}"
            };
        }
    }

}

