using Backend.Application.Interfaces;
using Backend.Domain.Models.Course;
using Backend.Domain.Models.CourseEvent;
using Backend.Infrastructure.Persistence.EFC.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.EFC.Repositories
{
    public class CourseRepository(CoursesOnlineDbContext context) : ICoursesRepository
    {
        private readonly CoursesOnlineDbContext _context = context;

        public async Task<CourseSummaryDto> CreateCourseAsync(CreateCourseDto course, CancellationToken cancellationToken)
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

            return new CourseSummaryDto(entity.Id, entity.Title, entity.Description, entity.DurationInDays);
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

        public async Task<IEnumerable<CourseSummaryDto>> GetAllCoursesAsync(CancellationToken cancellationToken)
        {
            var entities = await _context.Courses
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAtUtc)
                .Select(e => new CourseSummaryDto(e.Id, e.Title, e.Description, e.DurationInDays))
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<CourseDto?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("Id is required", nameof(courseId));

            var dto = await _context.Courses
                .AsNoTracking()
                .Where(c => c.Id == courseId)
                .Select(c => new CourseDto(
                    c.Id,
                    c.Title,
                    c.Description,
                    c.DurationInDays,
                    c.CourseEvents.Select(ce => new CourseEventDto(
                        ce.Id,
                        ce.EventDate,
                        ce.Price,
                        ce.Seats,
                        ce.CourseEventTypeId
                    ))
                ))
                .SingleOrDefaultAsync(cancellationToken);

            return dto;
        }

        public async Task<CourseDto?> GetCourseByTitleAsync(string title, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            var dto = await _context.Courses
                .AsNoTracking()
                .Where(c => c.Title == title)
                .Select(c => new CourseDto(
                    c.Id,
                    c.Title,
                    c.Description,
                    c.DurationInDays,
                    c.CourseEvents.Select(ce => new CourseEventDto(
                        ce.Id,
                        ce.EventDate,
                        ce.Price,
                        ce.Seats,
                        ce.CourseEventTypeId
                    ))
                ))
                .SingleOrDefaultAsync(cancellationToken);

            return dto;
        }

        public async Task<CourseSummaryDto?> UpdateCourseAsync(UpdateCourseDto course, CancellationToken cancellationToken)
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

            return new CourseSummaryDto(entity.Id, entity.Title, entity.Description, entity.DurationInDays);
        }
    }
}
