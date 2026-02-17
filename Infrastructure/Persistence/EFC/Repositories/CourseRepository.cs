using Backend.Domain.Modules.CourseEvents.Models;
using Backend.Domain.Modules.Courses.Contracts;
using Backend.Domain.Modules.Courses.Models;
using Backend.Infrastructure.Persistence.Entities;
using Backend.Infrastructure.Persistence.EFC.Context;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories
{
    public class CourseRepository(CoursesOnlineDbContext context) : ICourseRepository
    {
        private readonly CoursesOnlineDbContext _context = context;

        public static Course ToModel(CourseEntity entity)
            => new(entity.Id, entity.Title, entity.Description, entity.DurationInDays);

        public async Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken)
        {
            var entity = new CourseEntity
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                DurationInDays = course.DurationInDays
            };

            _context.Courses.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return ToModel(entity);
        }

        public async Task<bool> DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken)
        {
            var entity = await _context.Courses.SingleOrDefaultAsync(c => c.Id == courseId, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Course '{courseId}' not found.");

            _context.Courses.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IReadOnlyList<Course>> GetAllCoursesAsync(CancellationToken cancellationToken)
        {
            var entities = await _context.Courses
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return [.. entities.Select(ToModel)];
        }

        public async Task<CourseWithEvents?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
        {
            var entity = await _context.Courses
                .AsNoTracking()
                .Include(c => c.CourseEvents)
                .SingleOrDefaultAsync(c => c.Id == courseId, cancellationToken);

            if (entity == null)
                return null;

            var course = ToModel(entity);
            var events = entity.CourseEvents
                .Select(ce => new CourseEvent(
                    ce.Id,
                    ce.CourseId,
                    ce.EventDate,
                    ce.Price,
                    ce.Seats,
                    ce.CourseEventTypeId))
                .ToList();

            return new CourseWithEvents(course, events);
        }

        public async Task<Course?> UpdateCourseAsync(Course course, CancellationToken cancellationToken)
        {
            var entity = await _context.Courses.SingleOrDefaultAsync(c => c.Id == course.Id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Course '{course.Id}' not found.");

            entity.Title = course.Title;
            entity.Description = course.Description;
            entity.DurationInDays = course.DurationInDays;
            entity.ModifiedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return ToModel(entity);
        }

        public async Task<bool> HasCourseEventsAsync(Guid courseId, CancellationToken cancellationToken)
        {
            return await _context.CourseEvents
                .AsNoTracking()
                .AnyAsync(ce => ce.CourseId == courseId, cancellationToken);
        }
    }
}




