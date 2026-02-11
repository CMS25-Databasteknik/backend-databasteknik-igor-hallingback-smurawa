using Backend.Application.Interfaces;
using Backend.Domain.Modules.Courses.Models;
using Backend.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories
{
    public class CourseRepository(CoursesOnlineDbContext context) : ICoursesRepository
    {
        private readonly CoursesOnlineDbContext _context = context;

        public static Course ToModel(CourseEntity entity)
            => new(entity.Id, entity.Title, entity.Description, entity.DurationInDays);

        public async Task<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken)
        {

            var entity = new CourseEntity
            {
                Id = Guid.NewGuid(),
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
            if (courseId == Guid.Empty)
                throw new ArgumentException("Id is required", nameof(courseId));

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

        public async Task<CourseWithEvents> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("Id is required", nameof(courseId));

            var course = await _context.Courses
                .AsNoTracking()
                .Where(c => c.Id == courseId)
                .Select(c => new CourseWithEvents(
                    c.Id,
                    c.Title,
                    c.Description,
                    c.DurationInDays,
                    c.CourseEvents.Select(ce => new CourseEvent(
                        ce.Id,
                        ce.CourseId,
                        ce.EventDate,
                        ce.Price,
                        ce.Seats,
                        ce.CourseEventTypeId
                    ))
                ))
                .SingleOrDefaultAsync(cancellationToken);

            return course;
        }

        public async Task<Course?> UpdateCourseAsync(Course course, CancellationToken cancellationToken)
        {
            if (course.Id == Guid.Empty)
                throw new ArgumentException("Id is required", nameof(course.Id));

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
    }
}
