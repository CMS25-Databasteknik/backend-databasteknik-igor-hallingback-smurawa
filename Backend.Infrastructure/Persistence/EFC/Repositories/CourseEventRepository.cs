using Backend.Application.Interfaces;
using Backend.Domain.Models.CourseEvent;
using Backend.Infrastructure.Persistence.EFC.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories
{
    public class CourseEventRepository(CoursesOnlineDbContext context) : ICourseEventsRepository
    {
        private readonly CoursesOnlineDbContext _context = context;

        public async Task<CourseEventSummaryDto> CreateCourseEventAsync(CreateCourseEventDto courseEvent, CancellationToken cancellationToken)
        {
            var entity = new CourseEventEntity
            {
                Id = Guid.NewGuid(),
                CourseId = courseEvent.CourseId,
                EventDate = courseEvent.EventDate,
                Price = courseEvent.Price,
                Seats = courseEvent.Seats,
                CourseEventTypeId = courseEvent.CourseEventTypeId
            };

            _context.CourseEvents.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new CourseEventSummaryDto(
                entity.Id,
                entity.CourseId,
                entity.EventDate,
                entity.Price,
                entity.Seats,
                entity.CourseEventTypeId
            );
        }

        public async Task<bool> DeleteCourseEventAsync(Guid courseEventId, CancellationToken cancellationToken)
        {
            if (courseEventId == Guid.Empty)
                throw new ArgumentException("Id is required", nameof(courseEventId));

            var entity = await _context.CourseEvents.SingleOrDefaultAsync(ce => ce.Id == courseEventId, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Course event '{courseEventId}' not found.");

            _context.CourseEvents.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IEnumerable<CourseEventSummaryDto>> GetAllCourseEventsAsync(CancellationToken cancellationToken)
        {
            var entities = await _context.CourseEvents
                .AsNoTracking()
                .OrderByDescending(ce => ce.CreatedAtUtc)
                .Select(ce => new CourseEventSummaryDto(
                    ce.Id,
                    ce.CourseId,
                    ce.EventDate,
                    ce.Price,
                    ce.Seats,
                    ce.CourseEventTypeId
                ))
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<CourseEventDto?> GetCourseEventByIdAsync(Guid courseEventId, CancellationToken cancellationToken)
        {
            if (courseEventId == Guid.Empty)
                throw new ArgumentException("Id is required", nameof(courseEventId));

            var dto = await _context.CourseEvents
                .AsNoTracking()
                .Where(ce => ce.Id == courseEventId)
                .Select(ce => new CourseEventDto(
                    ce.Id,
                    ce.EventDate,
                    ce.Price,
                    ce.Seats,
                    ce.CourseEventTypeId
                ))
                .SingleOrDefaultAsync(cancellationToken);

            return dto;
        }

        public async Task<IEnumerable<CourseEventSummaryDto>> GetCourseEventsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("CourseId is required", nameof(courseId));

            var entities = await _context.CourseEvents
                .AsNoTracking()
                .Where(ce => ce.CourseId == courseId)
                .OrderBy(ce => ce.EventDate)
                .Select(ce => new CourseEventSummaryDto(
                    ce.Id,
                    ce.CourseId,
                    ce.EventDate,
                    ce.Price,
                    ce.Seats,
                    ce.CourseEventTypeId
                ))
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<CourseEventSummaryDto?> UpdateCourseEventAsync(UpdateCourseEventDto courseEvent, CancellationToken cancellationToken)
        {
            if (courseEvent.Id == Guid.Empty)
                throw new ArgumentException("Id is required", nameof(courseEvent.Id));

            var entity = await _context.CourseEvents.SingleOrDefaultAsync(ce => ce.Id == courseEvent.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Course event '{courseEvent.Id}' not found.");

            entity.CourseId = courseEvent.CourseId;
            entity.EventDate = courseEvent.EventDate;
            entity.Price = courseEvent.Price;
            entity.Seats = courseEvent.Seats;
            entity.CourseEventTypeId = courseEvent.CourseEventTypeId;
            entity.ModifiedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new CourseEventSummaryDto(
                entity.Id,
                entity.CourseId,
                entity.EventDate,
                entity.Price,
                entity.Seats,
                entity.CourseEventTypeId
            );
        }
    }
}
