using Backend.Domain.Modules.Courses.Models;

namespace Backend.Application.Modules.Courses.Outputs;

public class Result
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
}

public sealed class CourseResult : Result
{
    public Course? Result { get; set; }
    public string? Message { get; set; }
}
