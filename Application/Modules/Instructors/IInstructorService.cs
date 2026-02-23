using Backend.Application.Modules.Instructors.Inputs;
using Backend.Application.Modules.Instructors.Outputs;

namespace Backend.Application.Modules.Instructors;

public interface IInstructorService
{
    Task<InstructorResult> CreateInstructorAsync(CreateInstructorInput instructor, CancellationToken cancellationToken = default);
    Task<InstructorListResult> GetAllInstructorsAsync(CancellationToken cancellationToken = default);
    Task<InstructorDetailsResult> GetInstructorByIdAsync(Guid instructorId, CancellationToken cancellationToken = default);
    Task<InstructorResult> UpdateInstructorAsync(UpdateInstructorInput instructor, CancellationToken cancellationToken = default);
    Task<InstructorDeleteResult> DeleteInstructorAsync(Guid instructorId, CancellationToken cancellationToken = default);
}
