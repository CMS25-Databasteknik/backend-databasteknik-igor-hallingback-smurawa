using Backend.Domain.Modules.Courses.Contracts;
using Backend.Domain.Modules.Courses.Models;
using Backend.Domain.Modules.CourseEvents.Models;
using Backend.Domain.Modules.CourseEventTypes.Models;
using Backend.Domain.Modules.CourseWithEvents.Models;
using Backend.Domain.Modules.VenueTypes.Models;
using Backend.Infrastructure.Persistence.EFC.Context;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories
{
    public class CourseRepository(CoursesOnlineDbContext context) : ICourseRepository
    {
        private readonly CoursesOnlineDbContext _context = context;

        public async Task<Course> AddAsync(Course course, CancellationToken cancellationToken)
        {
            var entity = ToEntity(course);
            _context.Courses.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return ToModel(entity);
        }

        public async Task<IReadOnlyList<CourseWithEvents>> GetAllAsync(CancellationToken cancellationToken)
        {
            var entities = await _context.Courses
                .AsNoTracking()
                .Include(c => c.CourseEvents)
                    .ThenInclude(ce => ce.CourseEventType)
                .Include(c => c.CourseEvents)
                    .ThenInclude(ce => ce.VenueType)
                .OrderByDescending(c => c.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return [.. entities.Select(ToCourseWithEventsModel)];
        }

        public async Task<CourseWithEvents?> GetByIdAsync(Guid courseId, CancellationToken cancellationToken)
        {
            var entity = await _context.Courses
                .AsNoTracking()
                .Include(c => c.CourseEvents)
                    .ThenInclude(ce => ce.CourseEventType)
                .Include(c => c.CourseEvents)
                    .ThenInclude(ce => ce.VenueType)
                .SingleOrDefaultAsync(c => c.Id == courseId, cancellationToken);

            return entity is null ? null : ToCourseWithEventsModel(entity);
        }

        public async Task<Course?> UpdateAsync(Guid id, Course course, CancellationToken cancellationToken)
        {
            var entity = await _context.Courses.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (entity is null)
                throw new KeyNotFoundException($"Course '{course.Id}' not found.");

            entity.Title = course.Title;
            entity.Description = course.Description;
            entity.DurationInDays = course.DurationInDays;
            entity.ModifiedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return ToModel(entity);
        }

        public async Task<bool> RemoveAsync(Guid courseId, CancellationToken cancellationToken)
        {
            var entity = await _context.Courses.SingleOrDefaultAsync(c => c.Id == courseId, cancellationToken);
            if (entity == null)
                throw new KeyNotFoundException($"Course '{courseId}' not found.");

            _context.Courses.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> HasCourseEventsAsync(Guid courseId, CancellationToken cancellationToken)
        {
            return await _context.CourseEvents
                .AsNoTracking()
                .AnyAsync(ce => ce.CourseId == courseId, cancellationToken);
        }

        private static Course ToModel(CourseEntity entity)
            => new(entity.Id, entity.Title, entity.Description, entity.DurationInDays);

        private static CourseEntity ToEntity(Course course)
            => new()
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                DurationInDays = course.DurationInDays
            };

        private static CourseWithEvents ToCourseWithEventsModel(CourseEntity entity)
        {
            var course = ToModel(entity);
            var events = entity.CourseEvents
                .OrderBy(ce => ce.EventDate)
                .Select(ToCourseEventModel)
                .ToList();

            return new CourseWithEvents(course, events);
        }

        private static CourseEvent ToCourseEventModel(CourseEventEntity entity)
        {
            var courseEventType = new CourseEventType(entity.CourseEventType.Id, entity.CourseEventType.TypeName);
            var venueType = new VenueType(entity.VenueTypeId, entity.VenueType.Name);

            return new CourseEvent(
                entity.Id,
                entity.CourseId,
                entity.EventDate,
                entity.Price,
                entity.Seats,
                entity.CourseEventTypeId,
                venueType,
                courseEventType,
                venueType);
        }
    }
}
