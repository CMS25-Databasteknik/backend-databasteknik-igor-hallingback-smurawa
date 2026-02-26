using Backend.Application.Common;

namespace Backend.Tests.Unit.Application.Common;

public class ResultBase_Tests
{
    [Fact]
    public void Constructor_Should_Initialize_With_Default_Values()
    {
        var result = new TestResultBase();

        Assert.False(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.Null(result.Message);
    }

    [Fact]
    public void Properties_Should_Be_Settable()
    {
        var result = new TestResultBase
        {
            Success = false,
            Error = ResultError.Validation,
            Message = "Validation failed"
        };

        Assert.False(result.Success);
        Assert.Equal(ResultError.Validation, result.Error);
        Assert.Equal("Validation failed", result.Message);
    }

    [Fact]
    public void Message_Should_Accept_Null()
    {
        var result = new TestResultBase { Message = null };

        Assert.Null(result.Message);
    }

    private sealed class TestResultBase : ResultBase;
}

public class ResultCommon_Tests
{
    [Fact]
    public void Constructor_Should_Initialize_With_Default_Values()
    {
        var result = new TestResultCommon();

        Assert.False(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.Null(result.Message);
        Assert.Null(result.Result);
    }

    [Fact]
    public void Result_Should_Be_Settable()
    {
        var result = new TestResultCommon { Result = "Test" };

        Assert.Equal("Test", result.Result);
    }

    [Fact]
    public void Should_Inherit_Base_Properties()
    {
        var result = new TestResultCommon
        {
            Success = true,
            Error = ResultError.None,
            Message = "ok",
            Result = "payload"
        };

        Assert.True(result.Success);
        Assert.Equal(ResultError.None, result.Error);
        Assert.Equal("ok", result.Message);
        Assert.Equal("payload", result.Result);
    }

    private sealed class TestResultCommon : ResultCommon<string>;
}
