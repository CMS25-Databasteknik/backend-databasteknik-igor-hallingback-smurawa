using Backend.Application.Modules.CourseEvents.Inputs;
using Backend.Application.Modules.CourseEvents.Outputs;
using Backend.Domain.Modules.CourseEvents.Contracts;
using Backend.Domain.Modules.CourseEvents.Models;

namespace Backend.Application.Modules.CourseEvents
{
    public class CourseEventService(ICourseEventRepository courseEventRepository) : ICourseEventService
    {
        private readonly ICourseEventRepository _courseEventRepository = courseEventRepository ?? throw new ArgumentNullException(nameof(courseEventRepository));

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
                        Result = null,
                        Message = "Course event cannot be null."
                    };
                }

                if (courseEvent.CourseId == Guid.Empty)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Result = null,
                        Message = "Course ID cannot be empty."
                    };
                }

                if (courseEvent.EventDate == default)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Result = null,
                        Message = "Event date must be specified."
                    };
                }

                if (courseEvent.Price < 0)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Result = null,
                        Message = "Price cannot be negative."
                    };
                }

                if (courseEvent.Seats <= 0)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Result = null,
                        Message = "Seats must be greater than zero."
                    };
                }

                if (courseEvent.CourseEventTypeId <= 0)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Result = null,
                        Message = "Course event type ID must be greater than zero."
                    };
                }

                var newCourseEvent = new CourseEvent(
                    id: Guid.NewGuid(),
                    courseEvent.CourseId,
                    courseEvent.EventDate,
                    courseEvent.Price,
                    courseEvent.Seats,
                    courseEvent.CourseEventTypeId
                );

                var result = await _courseEventRepository.CreateCourseEventAsync(newCourseEvent, cancellationToken);

                return new CourseEventResult
                {
                    Success = true,
                    StatusCode = 201,
                    Result = result,
                    Message = "Course event created successfully."
                };
            }
            catch (Exception ex)
            {
                return new CourseEventResult
                {
                    Success = false,
                    StatusCode = 500,
                    Result = null,
                    Message = $"An error occurred while creating the course event: {ex.Message}"
                };
            }
        }

        public async Task<CourseEventListResult> GetAllCourseEventsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var courseEvents = await _courseEventRepository.GetAllCourseEventsAsync(cancellationToken);

                if (!courseEvents.Any())
                {
                    return new CourseEventListResult
                    {
                        Success = true,
                        Result = courseEvents,
                        StatusCode = 200,
                        Message = "No course events found."
                    };
                }

                return new CourseEventListResult
                {
                    Success = true,
                    StatusCode = 200,
                    Result = courseEvents,
                    Message = $"Retrieved {courseEvents.Count} course event(s) successfully."
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

        public async Task<CourseEventResult> GetCourseEventByIdAsync(Guid courseEventId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (courseEventId == Guid.Empty)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Course event ID cannot be empty."
                    };
                }

                var result = await _courseEventRepository.GetCourseEventByIdAsync(courseEventId, cancellationToken);

                if (result == null)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = $"Course event with ID '{courseEventId}' not found."
                    };
                }

                return new CourseEventResult
                {
                    Success = true,
                    StatusCode = 200,
                    Result = result,
                    Message = "Course event retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                return new CourseEventResult
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

                var courseEvents = await _courseEventRepository.GetCourseEventsByCourseIdAsync(courseId, cancellationToken);

                if (!courseEvents.Any())
                {
                    return new CourseEventListResult
                    {
                        Success = true,
                        Result = courseEvents,
                        StatusCode = 200,
                        Message = "No course events found for this course."
                    };
                }

                return new CourseEventListResult
                {
                    Success = true,
                    StatusCode = 200,
                    Result = courseEvents,
                    Message = $"Retrieved {courseEvents.Count} course event(s) for the course successfully."
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

                if (courseEvent.Id == Guid.Empty)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Course event ID cannot be empty."
                    };
                }

                if (courseEvent.CourseId == Guid.Empty)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Course ID cannot be empty."
                    };
                }

                if (courseEvent.EventDate == default)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Event date must be specified."
                    };
                }

                if (courseEvent.Price < 0)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Price cannot be negative."
                    };
                }

                if (courseEvent.Seats <= 0)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Seats must be greater than zero."
                    };
                }

                if (courseEvent.CourseEventTypeId <= 0)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Course event type ID must be greater than zero."
                    };
                }

                var existingCourseEvent = await _courseEventRepository.GetCourseEventByIdAsync(courseEvent.Id, cancellationToken);
                if (existingCourseEvent == null)
                {
                    return new CourseEventResult
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = $"Course event with ID '{courseEvent.Id}' not found."
                    };
                }

                var updatedCourseEvent = new CourseEvent(
                    courseEvent.Id,
                    courseEvent.CourseId,
                    courseEvent.EventDate,
                    courseEvent.Price,
                    courseEvent.Seats,
                    courseEvent.CourseEventTypeId
                );

                var result = await _courseEventRepository.UpdateCourseEventAsync(updatedCourseEvent, cancellationToken);

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
            catch (InvalidOperationException ex) when (ex.Message.Contains("modified by another user"))
            {
                return new CourseEventResult
                {
                    Success = false,
                    StatusCode = 409,
                    Message = "The course event was modified by another user. Please refresh and try again."
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
                        Message = "Course event ID cannot be empty.",
                        Result = false
                    };
                }

                var existingCourseEvent = await _courseEventRepository.GetCourseEventByIdAsync(courseEventId, cancellationToken);
                if (existingCourseEvent == null)
                {
                    return new CourseEventDeleteResult
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = $"Course event with ID '{courseEventId}' not found.",
                        Result = false
                    };
                }

                var result = await _courseEventRepository.DeleteCourseEventAsync(courseEventId, cancellationToken);

                return new CourseEventDeleteResult
                {
                    Success = true,
                    StatusCode = 200,
                    Result = result,
                    Message = "Course event deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new CourseEventDeleteResult
                {
                    Success = false,
                    StatusCode = 500,
                    Message = $"An error occurred while deleting the course event: {ex.Message}",
                    Result = false
                };
            }
        }
    }
}
