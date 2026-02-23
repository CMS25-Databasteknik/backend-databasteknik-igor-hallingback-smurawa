using Backend.Application.Modules.CourseEvents.Inputs;
using Backend.Application.Modules.CourseEvents.Outputs;
using Backend.Domain.Modules.CourseEvents.Contracts;
using Backend.Domain.Modules.CourseEvents.Models;
using Backend.Domain.Modules.CourseEventTypes.Contracts;
using Backend.Domain.Modules.Courses.Contracts;

namespace Backend.Application.Modules.CourseEvents;

public class CourseEventService(
    ICourseEventRepository courseEventRepository,
    ICourseRepository courseRepository,
    ICourseEventTypeRepository courseEventTypeRepository) : ICourseEventService
{
    private readonly ICourseEventRepository _courseEventRepository = courseEventRepository ?? throw new ArgumentNullException(nameof(courseEventRepository));
    private readonly ICourseRepository _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
    private readonly ICourseEventTypeRepository _courseEventTypeRepository = courseEventTypeRepository ?? throw new ArgumentNullException(nameof(courseEventTypeRepository));

    public async Task<CourseEventResult> CreateCourseEventAsync(CreateCourseEventInput courseEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            if (courseEvent == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Course event cannot be null."
                };
            }

            var newCourseEvent = new CourseEvent(
                Guid.NewGuid(),
                courseEvent.CourseId,
                courseEvent.EventDate,
                courseEvent.Price,
                courseEvent.Seats,
                courseEvent.CourseEventTypeId,
                courseEvent.VenueType);

            var existingCourse = await _courseRepository.GetByIdAsync(newCourseEvent.CourseId, cancellationToken);
            if (existingCourse == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course with ID '{newCourseEvent.CourseId}' not found."
                };
            }

            var existingCourseEventType = await _courseEventTypeRepository.GetByIdAsync(newCourseEvent.CourseEventTypeId, cancellationToken);
            if (existingCourseEventType == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course event type with ID '{newCourseEvent.CourseEventTypeId}' not found."
                };
            }

            var result = await _courseEventRepository.AddAsync(newCourseEvent, cancellationToken);

            return new CourseEventResult
            {
                Success = true,
                StatusCode = 201,
                Result = result,
                Message = "Course event created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseEventResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseEventResult
            {
                Success = false,
                StatusCode = 500,
                Message = $"An error occurred while creating the course event: {ex.Message}"
            };
        }
    }

    public async Task<CourseEventListResult> GetAllCourseEventsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _courseEventRepository.GetAllAsync(cancellationToken);

            return new CourseEventListResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = result.Any()
                    ? $"Retrieved {result.Count} course event(s) successfully."
                    : "No course events found."
            };
        }
        catch (Exception ex)
        {
            return new CourseEventListResult
            {
                Success = false,
                StatusCode = 500,
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
                    StatusCode = 400,
                    Message = "Course event ID cannot be empty."
                };
            }

            var result = await _courseEventRepository.GetByIdAsync(courseEventId, cancellationToken);
            if (result == null)
            {
                return new CourseEventDetailsResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course event with ID '{courseEventId}' not found."
                };
            }

            var details = new CourseEventDetails(
                result.Id,
                result.CourseId,
                result.EventDate,
                result.Price,
                result.Seats,
                new CourseEventLookupItem(result.CourseEventType.Id, result.CourseEventType.TypeName),
                new CourseEventLookupItem((int)result.VenueType, result.VenueTypeName)
            );

            return new CourseEventDetailsResult
            {
                Success = true,
                StatusCode = 200,
                Result = details,
                Message = "Course event retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            return new CourseEventDetailsResult
            {
                Success = false,
                StatusCode = 500,
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
                    StatusCode = 400,
                    Message = "Course ID cannot be empty."
                };
            }

            var result = await _courseEventRepository.GetCourseEventsByCourseIdAsync(courseId, cancellationToken);

            return new CourseEventListResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = result.Any()
                    ? $"Retrieved {result.Count} course event(s) successfully."
                    : "No course events found for this course."
            };
        }
        catch (Exception ex)
        {
            return new CourseEventListResult
            {
                Success = false,
                StatusCode = 500,
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
                    StatusCode = 400,
                    Message = "Course event cannot be null."
                };
            }

            var existingCourseEvent = await _courseEventRepository.GetByIdAsync(courseEvent.Id, cancellationToken);
            if (existingCourseEvent == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course event with ID '{courseEvent.Id}' not found."
                };
            }

            var existingCourse = await _courseRepository.GetByIdAsync(courseEvent.CourseId, cancellationToken);
            if (existingCourse == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course with ID '{courseEvent.CourseId}' not found."
                };
            }

            var existingCourseEventType = await _courseEventTypeRepository.GetByIdAsync(courseEvent.CourseEventTypeId, cancellationToken);
            if (existingCourseEventType == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    StatusCode = 404,
                    Message = $"Course event type with ID '{courseEvent.CourseEventTypeId}' not found."
                };
            }

            var updatedCourseEvent = new CourseEvent(
                courseEvent.Id,
                courseEvent.CourseId,
                courseEvent.EventDate,
                courseEvent.Price,
                courseEvent.Seats,
                courseEvent.CourseEventTypeId,
                courseEvent.VenueType);

            var result = await _courseEventRepository.UpdateAsync(updatedCourseEvent.Id, updatedCourseEvent, cancellationToken);
            if (result == null)
            {
                return new CourseEventResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = "Failed to update course event."
                };
            }

            return new CourseEventResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Course event updated successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new CourseEventResult
            {
                Success = false,
                StatusCode = 404,
                Message = ex.Message
            };
        }
        catch (ArgumentException ex)
        {
            return new CourseEventResult
            {
                Success = false,
                StatusCode = 400,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseEventResult
            {
                Success = false,
                StatusCode = 500,
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
                    StatusCode = 400,
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
                    StatusCode = 404,
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
                    StatusCode = 409,
                    Result = false,
                    Message = $"Cannot delete course event with ID '{courseEventId}' because it has registrations."
                };
            }

            var result = await _courseEventRepository.RemoveAsync(courseEventId, cancellationToken);
            return new CourseEventDeleteResult
            {
                Success = true,
                StatusCode = 200,
                Result = result,
                Message = "Course event deleted successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new CourseEventDeleteResult
            {
                Success = false,
                StatusCode = 404,
                Result = false,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new CourseEventDeleteResult
            {
                Success = false,
                StatusCode = 500,
                Result = false,
                Message = $"An error occurred while deleting the course event: {ex.Message}"
            };
        }
    }
}


