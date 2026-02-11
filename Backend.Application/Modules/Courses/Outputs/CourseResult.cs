using Backend.Domain.Modules.Courses.Models;
using Backend.Domain.Modules.CourseEvents.Models;

namespace Backend.Application.Modules.Courses.Outputs;

public class Result
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
}

public sealed class CourseResult : Result
{
    public Course? Result { get; set; }

}
public sealed class CourseWithEventsResult : Result
{
    public Course? Course { get; set; }
    public IEnumerable<CourseEvent> Events { get; set; } = [];
}

public sealed class CourseListResult : Result
{
    public IEnumerable<Course> Result { get; set; } = [];

}

public sealed class CourseDeleteResult : Result
{
    public bool Result { get; set; }

}
