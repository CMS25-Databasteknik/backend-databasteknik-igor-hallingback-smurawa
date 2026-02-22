using Backend.Application.Common;

namespace Backend.Tests.Unit.Application.Common;

public class ResultBase_Tests
{
    [Fact]
    public void Constructor_Should_Initialize_With_Default_Values()
    {
        // Act
        var result = new TestResultBase();

        // Assert
        Assert.False(result.Success);
        Assert.Equal(0, result.StatusCode);
        Assert.Null(result.Message);
    }

    [Fact]
    public void Success_Should_Be_Settable()
    {
        // Arrange
        var result = new TestResultBase();

        // Act
        result.Success = true;

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public void StatusCode_Should_Be_Settable()
    {
        // Arrange
        var result = new TestResultBase();

        // Act
        result.StatusCode = 200;

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void Message_Should_Be_Settable()
    {
        // Arrange
        var result = new TestResultBase();

        // Act
        result.Message = "Success message";

        // Assert
        Assert.Equal("Success message", result.Message);
    }

    [Fact]
    public void Should_Support_Success_State()
    {
        // Arrange
        var result = new TestResultBase
        {
            Success = true,
            StatusCode = 200,
            Message = "Operation completed successfully"
        };

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("Operation completed successfully", result.Message);
    }

    [Fact]
    public void Should_Support_Failure_State()
    {
        // Arrange
        var result = new TestResultBase
        {
            Success = false,
            StatusCode = 400,
            Message = "Operation failed"
        };

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Operation failed", result.Message);
    }

    [Theory]
    [InlineData(200, "OK")]
    [InlineData(201, "Created")]
    [InlineData(400, "Bad Request")]
    [InlineData(404, "Not Found")]
    [InlineData(500, "Internal Server Error")]
    public void Should_Support_Various_Status_Codes(int statusCode, string message)
    {
        // Arrange & Act
        var result = new TestResultBase
        {
            StatusCode = statusCode,
            Message = message
        };

        // Assert
        Assert.Equal(statusCode, result.StatusCode);
        Assert.Equal(message, result.Message);
    }

    [Fact]
    public void Message_Should_Accept_Null()
    {
        // Arrange & Act
        var result = new TestResultBase
        {
            Message = null
        };

        // Assert
        Assert.Null(result.Message);
    }

    [Fact]
    public void Message_Should_Accept_Empty_String()
    {
        // Arrange & Act
        var result = new TestResultBase
        {
            Message = string.Empty
        };

        // Assert
        Assert.Equal(string.Empty, result.Message);
    }

    [Fact]
    public void Properties_Should_Be_Independent()
    {
        // Arrange
        var result = new TestResultBase();

        // Act
        result.Success = true;
        result.StatusCode = 200;
        result.Message = "Test";

        // Assert - Each property should be independently settable
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("Test", result.Message);

        // Act - Change one property
        result.Success = false;

        // Assert - Other properties unchanged
        Assert.False(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("Test", result.Message);
    }

    // Helper class for testing abstract ResultBase
    private class TestResultBase : ResultBase
    {
    }
}

public class ResultCommon_Tests
{
    [Fact]
    public void Constructor_Should_Initialize_With_Default_Values()
    {
        // Act
        var result = new TestResultCommon();

        // Assert
        Assert.False(result.Success);
        Assert.Equal(0, result.StatusCode);
        Assert.Null(result.Message);
        Assert.Null(result.Result);
    }

    [Fact]
    public void Result_Should_Be_Settable()
    {
        // Arrange
        var result = new TestResultCommon();
        var testData = "Test Data";

        // Act
        result.Result = testData;

        // Assert
        Assert.Equal(testData, result.Result);
    }

    [Fact]
    public void Should_Support_Success_With_Data()
    {
        // Arrange
        var testData = "Success Data";
        var result = new TestResultCommon
        {
            Success = true,
            StatusCode = 200,
            Message = "Success",
            Result = testData
        };

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("Success", result.Message);
        Assert.Equal(testData, result.Result);
    }

    [Fact]
    public void Should_Support_Failure_With_Null_Data()
    {
        // Arrange
        var result = new TestResultCommon
        {
            Success = false,
            StatusCode = 404,
            Message = "Not Found",
            Result = null
        };

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Not Found", result.Message);
        Assert.Null(result.Result);
    }

    [Fact]
    public void Result_Should_Accept_Complex_Types()
    {
        // Arrange
        var complexData = new { Id = 1, Name = "Test" };
        var result = new TestResultCommonComplex
        {
            Result = complexData
        };

        // Assert
        Assert.NotNull(result.Result);
        Assert.Equal(1, result.Result.Id);
        Assert.Equal("Test", result.Result.Name);
    }

    [Fact]
    public void Generic_Type_Should_Be_Strongly_Typed()
    {
        // Arrange
        var intResult = new TestResultCommonInt { Result = 42 };
        var stringResult = new TestResultCommon { Result = "Test" };

        // Assert
        Assert.Equal(42, intResult.Result);
        Assert.Equal("Test", stringResult.Result);
    }

    [Fact]
    public void Should_Inherit_All_Base_Properties()
    {
        // Arrange
        var result = new TestResultCommon
        {
            Success = true,
            StatusCode = 201,
            Message = "Created",
            Result = "New Item"
        };

        // Assert - Verify inheritance from ResultBase
        Assert.True(result.Success);
        Assert.Equal(201, result.StatusCode);
        Assert.Equal("Created", result.Message);
        Assert.Equal("New Item", result.Result);
    }

    // Helper classes for testing abstract ResultCommon<T>
    private class TestResultCommon : ResultCommon<string>
    {
    }

    private class TestResultCommonInt : ResultCommon<int>
    {
    }

    private class TestResultCommonComplex : ResultCommon<dynamic>
    {
    }
}
