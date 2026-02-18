using Backend.Domain.Modules.CourseEvents.Contracts;
using Backend.Domain.Modules.CourseEvents.Models;
using Backend.Domain.Modules.VenueTypes.Models;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories
{
    public class CourseEventRepository(CoursesOnlineDbContext context) : ICourseEventRepository
    {
        private readonly CoursesOnlineDbContext _context = context;

        private static CourseEvent ToModel(CourseEventEntity entity)
            => new(
                entity.Id,
                entity.CourseId,
                entity.EventDate,
                entity.Price,
                entity.Seats,
                entity.CourseEventTypeId,
                (VenueType)entity.VenueTypeId);

        public async Task<CourseEvent> CreateCourseEventAsync(CourseEvent courseEvent, CancellationToken cancellationToken)
        {
            var entity = new CourseEventEntity
            {
                Id = courseEvent.Id,
                CourseId = courseEvent.CourseId,
                EventDate = courseEvent.EventDate,
                Price = courseEvent.Price,
                Seats = courseEvent.Seats,
                CourseEventTypeId = courseEvent.CourseEventTypeId,
                VenueTypeId = (int)courseEvent.VenueType
            };

            _context.CourseEvents.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return ToModel(entity);
        }

        public async Task<bool> DeleteCourseEventAsync(Guid courseEventId, CancellationToken cancellationToken)
        {
            using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var exists = await _context.CourseEvents
                    .AnyAsync(ce => ce.Id == courseEventId, cancellationToken);

                if (!exists)
                    throw new KeyNotFoundException($"Course event '{courseEventId}' not found.");

                await _context.Database.ExecuteSqlAsync(
                    $"DELETE FROM CourseRegistrations WHERE CourseEventId = {courseEventId}",
                    cancellationToken);

                await _context.Database.ExecuteSqlAsync(
                    $"DELETE FROM CourseEventInstructors WHERE CourseEventId = {courseEventId}",
                    cancellationToken);

                await _context.Database.ExecuteSqlAsync(
                    $"DELETE FROM InPlaceEventLocations WHERE CourseEventId = {courseEventId}",
                    cancellationToken);

                await _context.Database.ExecuteSqlAsync(
                    $"DELETE FROM CourseEvents WHERE Id = {courseEventId}",
                    cancellationToken);

                await tx.CommitAsync(cancellationToken);
                return true;
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<bool> HasRegistrationsAsync(Guid courseEventId, CancellationToken cancellationToken)
        {
            return await _context.CourseRegistrations
                .AsNoTracking()
                .AnyAsync(cr => cr.CourseEventId == courseEventId, cancellationToken);
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
            var entity = await _context.CourseEvents
                .AsNoTracking()
                .SingleOrDefaultAsync(ce => ce.Id == courseEventId, cancellationToken);

            return entity == null ? null : ToModel(entity);
        }

        public async Task<IReadOnlyList<CourseEvent>> GetCourseEventsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
        {
            var entities = await _context.CourseEvents
                .AsNoTracking()
                .Where(ce => ce.CourseId == courseId)
                .OrderBy(ce => ce.EventDate)
                .ToListAsync(cancellationToken);

            return [.. entities.Select(ToModel)];
        }

        public async Task<CourseEvent?> UpdateCourseEventAsync(CourseEvent courseEvent, CancellationToken cancellationToken)
        {
            var entity = await _context.CourseEvents.SingleOrDefaultAsync(ce => ce.Id == courseEvent.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Course event '{courseEvent.Id}' not found.");

            entity.CourseId = courseEvent.CourseId;
            entity.EventDate = courseEvent.EventDate;
            entity.Price = courseEvent.Price;
            entity.Seats = courseEvent.Seats;
            entity.CourseEventTypeId = courseEvent.CourseEventTypeId;
            entity.VenueTypeId = (int)courseEvent.VenueType;
            entity.ModifiedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return ToModel(entity);
        }
    }
}




