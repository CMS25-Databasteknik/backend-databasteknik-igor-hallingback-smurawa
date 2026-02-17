namespace Backend.Presentation.API.Models.Instructor;

public sealed record CreateInstructorRequest(
    string Name,
    int InstructorRoleId
);
