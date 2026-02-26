using Backend.Application.Common;
using Backend.Application.Modules.Courses.Outputs;
using Backend.Domain.Modules.CourseEvents.Models;
using Backend.Domain.Modules.Courses.Models;

namespace Backend.Tests.Unit.Application.Modules.Courses.Outputs;

public class CourseResult_Tests
{
    [Fact]
    public void Constructor_Should_Initialize_With_Default_Values()
    {
        // Act
        var result = new CourseResult();

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.Null(result.Message);
        Assert.Null(result.Result);
    }

    [Fact]
    public void Should_Store_Course_In_Result_Property()
    {
        // Arrange
        var course = new Course(Guid.NewGuid(), "Test Course", "Test Description", 10);
        var result = new CourseResult
        {
            Result = course
        };

        // Assert
        Assert.NotNull(result.Result);
        Assert.Equal(course, result.Result);
        Assert.Equal("Test Course", result.Result.Title);
    }

    [Fact]
    public void Should_Support_Success_Response_With_Course()
    {
        // Arrange
        var course = new Course(Guid.NewGuid(), "Test Course", "Test Description", 10);
        var result = new CourseResult
        {
            Success = true,
            Error = ResultError.None,
            Message = "Course retrieved successfully",
            Result = course
        };

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.Equal("Course retrieved successfully", result.Message);
        Assert.NotNull(result.Result);
        Assert.Equal(course.Id, result.Result.Id);
    }

    [Fact]
    public void Should_Support_Failure_Response_Without_Course()
    {
        // Arrange
        var result = new CourseResult
        {
            Success = false,
            Error = ResultError.NotFound,
            Message = "Course not found",
            Result = null
        };

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.NotFound, result.Error);
        Assert.Equal("Course not found", result.Message);
        Assert.Null(result.Result);
    }

    [Fact]
    public void Should_Be_Strongly_Typed_As_Course()
    {
        // Arrange
        var course = new Course(Guid.NewGuid(), "Test", "Description", 5);
        var result = new CourseResult { Result = course };

        // Act - This should compile without casting
        var title = result.Result?.Title;
        var description = result.Result?.Description;

        // Assert
        Assert.Equal("Test", title);
        Assert.Equal("Description", description);
    }
}

public class CourseWithEventsResult_Tests
{
    [Fact]
    public void Constructor_Should_Initialize_With_Default_Values()
    {
        // Act
        var result = new CourseWithEventsResult();

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.Null(result.Message);
        Assert.Null(result.Result);
    }

    [Fact]
    public void Should_Store_CourseWithEvents_In_Result_Property()
    {
        // Arrange
        var course = new Course(Guid.NewGuid(), "Test Course", "Test Description", 10);
        var courseWithEvents = new CourseWithEvents(course, Array.Empty<CourseEvent>());
        var result = new CourseWithEventsResult
        {
            Result = courseWithEvents
        };

        // Assert
        Assert.NotNull(result.Result);
        Assert.Equal(courseWithEvents, result.Result);
        Assert.Equal(course, result.Result.Course);
    }

    [Fact]
    public void Should_Support_Success_Response_With_CourseWithEvents()
    {
        // Arrange
        var course = new Course(Guid.NewGuid(), "Test Course", "Test Description", 10);
        var courseWithEvents = new CourseWithEvents(course, Array.Empty<CourseEvent>());
        var result = new CourseWithEventsResult
        {
            Success = true,
            Error = ResultError.None,
            Message = "Course with events retrieved",
            Result = courseWithEvents
        };

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result.Events);
    }

    [Fact]
    public void Should_Support_Failure_Response_Without_CourseWithEvents()
    {
        // Arrange
        var result = new CourseWithEventsResult
        {
            Success = false,
            Error = ResultError.NotFound,
            Message = "Course not found",
            Result = null
        };

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.NotFound, result.Error);
        Assert.Null(result.Result);
    }
}

public class CourseListResult_Tests
{
    [Fact]
    public void Constructor_Should_Initialize_With_Default_Values()
    {
        // Act
        var result = new CourseListResult();

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.Null(result.Message);
        Assert.Null(result.Result);
    }

    [Fact]
    public void Should_Store_Course_List_In_Result_Property()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course(Guid.NewGuid(), "Course 1", "Description 1", 5),
            new Course(Guid.NewGuid(), "Course 2", "Description 2", 10)
        };
        var result = new CourseListResult
        {
            Result = courses
        };

        // Assert
        Assert.NotNull(result.Result);
        Assert.Equal(2, result.Result.Count());
    }

    [Fact]
    public void Should_Support_Success_Response_With_Multiple_Courses()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course(Guid.NewGuid(), "Course 1", "Description 1", 5),
            new Course(Guid.NewGuid(), "Course 2", "Description 2", 10),
            new Course(Guid.NewGuid(), "Course 3", "Description 3", 15)
        };
        var result = new CourseListResult
        {
            Success = true,
            Error = ResultError.None,
            Message = "Retrieved 3 courses",
            Result = courses
        };

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.NotNull(result.Result);
        Assert.Equal(3, result.Result.Count());
    }

    [Fact]
    public void Should_Support_Success_Response_With_Empty_List()
    {
        // Arrange
        var result = new CourseListResult
        {
            Success = true,
            Error = ResultError.None,
            Message = "No courses found",
            Result = new List<Course>()
        };

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Result);
        Assert.Empty(result.Result);
    }

    [Fact]
    public void Should_Support_Failure_Response_With_Null_Result()
    {
        // Arrange
        var result = new CourseListResult
        {
            Success = false,
            Error = ResultError.Unexpected,
            Message = "Failed to retrieve courses",
            Result = null
        };

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
        Assert.Null(result.Result);
    }

    [Fact]
    public void Should_Be_Enumerable()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course(Guid.NewGuid(), "Course 1", "Description 1", 5),
            new Course(Guid.NewGuid(), "Course 2", "Description 2", 10)
        };
        var result = new CourseListResult { Result = courses };

        // Act
        var count = 0;
        if (result.Result != null)
        {
            foreach (var course in result.Result)
            {
                count++;
                Assert.NotNull(course);
            }
        }

        // Assert
        Assert.Equal(2, count);
    }
}

public class CourseDeleteResult_Tests
{
    [Fact]
    public void Constructor_Should_Initialize_With_Default_Values()
    {
        // Act
        var result = new CourseDeleteResult();

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.Null(result.Message);
        Assert.False(result.Result);
    }

    [Fact]
    public void Should_Support_Success_Response_With_True_Result()
    {
        // Arrange
        var result = new CourseDeleteResult
        {
            Success = true,
            Error = ResultError.None,
            Message = "Course deleted successfully",
            Result = true
        };

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.Equal("Course deleted successfully", result.Message);
        Assert.True(result.Result);
    }

    [Fact]
    public void Should_Support_Failure_Response_With_False_Result()
    {
        // Arrange
        var result = new CourseDeleteResult
        {
            Success = false,
            Error = ResultError.NotFound,
            Message = "Course not found",
            Result = false
        };

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.NotFound, result.Error);
        Assert.Equal("Course not found", result.Message);
        Assert.False(result.Result);
    }

    [Fact]
    public void Result_Should_Be_Boolean_Type()
    {
        // Arrange
        var successResult = new CourseDeleteResult { Result = true };
        var failureResult = new CourseDeleteResult { Result = false };

        // Assert
        Assert.IsType<bool>(successResult.Result);
        Assert.IsType<bool>(failureResult.Result);
        Assert.True(successResult.Result);
        Assert.False(failureResult.Result);
    }

    [Fact]
    public void Should_Support_Internal_Server_Error_Response()
    {
        // Arrange
        var result = new CourseDeleteResult
        {
            Success = false,
            Error = ResultError.Unexpected,
            Message = "Internal server error occurred",
            Result = false
        };

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ResultError.Unexpected, result.Error);
        Assert.False(result.Result);
    }
}

