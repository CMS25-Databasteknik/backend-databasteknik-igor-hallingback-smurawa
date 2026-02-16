namespace Backend.Application.Modules.Instructors.Inputs;

public sealed record UpdateInstructorInput(
    Guid Id,
    string Name
);
