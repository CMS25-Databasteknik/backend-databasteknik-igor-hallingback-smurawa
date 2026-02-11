using Backend.Application.Interfaces;
using Backend.Domain.Modules.Courses.Models;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories
{
    public class CourseEventRepository(CoursesOnlineDbContext context) : ICourseEventsRepository
    {
        private readonly CoursesOnlineDbContext _context = context;

        private static CourseEvent ToModel(CourseEventEntity entity)
            => new(
                entity.Id,
                entity.CourseId,
                entity.EventDate,
                entity.Price,
                entity.Seats,
                entity.CourseEventTypeId);

        public async Task<CourseEvent> CreateCourseEventAsync(CourseEvent courseEvent, CancellationToken cancellationToken)
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

            return ToModel(entity);
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

        public async Task<IReadOnlyList<CourseEvent>> GetAllCourseEventsAsync(CancellationToken cancellationToken)
        {
            var entities = await _context.CourseEvents
                .AsNoTracking()
                .OrderByDescending(ce => ce.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return [.. entities.Select(ToModel)];
        }

        public async Task<CourseEvent?> GetCourseEventByIdAsync(Guid courseEventId, CancellationToken cancellationToken)
        {
            if (courseEventId == Guid.Empty)
                throw new ArgumentException("Id is required", nameof(courseEventId));

            var entity = await _context.CourseEvents
                .AsNoTracking()
                .SingleOrDefaultAsync(ce => ce.Id == courseEventId, cancellationToken);

            return entity == null ? null : ToModel(entity);
        }

        public async Task<IReadOnlyList<CourseEvent>> GetCourseEventsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("CourseId is required", nameof(courseId));

            var entities = await _context.CourseEvents
                .AsNoTracking()
                .Where(ce => ce.CourseId == courseId)
                .OrderBy(ce => ce.EventDate)
                .ToListAsync(cancellationToken);

            return [.. entities.Select(ToModel)];
        }

        public async Task<CourseEvent?> UpdateCourseEventAsync(CourseEvent courseEvent, CancellationToken cancellationToken)
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

            return ToModel(entity);
        }
    }
}
