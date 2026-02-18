using Backend.Domain.Modules.Instructors.Models;

namespace Backend.Domain.Modules.Instructors.Contracts;

public interface IInstructorRepository
{
    Task<Instructor> CreateInstructorAsync(Instructor instructor, CancellationToken cancellationToken);
    Task<Instructor?> GetInstructorByIdAsync(Guid instructorId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Instructor>> GetAllInstructorsAsync(CancellationToken cancellationToken);
    Task<Instructor?> UpdateInstructorAsync(Instructor instructor, CancellationToken cancellationToken);
    Task<bool> DeleteInstructorAsync(Guid instructorId, CancellationToken cancellationToken);
    Task<bool> HasCourseEventsAsync(Guid instructorId, CancellationToken cancellationToken);
}

