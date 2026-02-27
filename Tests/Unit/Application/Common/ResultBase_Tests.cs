using Backend.Application.Common;
using System.Text.Json;

namespace Backend.Tests.Unit.Application.Common;

public class ResultBase_Tests
{
    [Fact]
    public void Constructor_Should_Initialize_With_Default_Values()
    {
        var result = new TestResultBase
        {
            Success = false,
            ErrorType = ErrorTypes.None,
            Message = null
        };

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.None, result.ErrorType);
        Assert.Null(result.Message);
    }

    [Fact]
    public void Properties_Should_Be_Settable()
    {
        var result = new TestResultBase
        {
            Success = false,
            ErrorType = ErrorTypes.Validation,
            Message = "Validation failed"
        };

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.Validation, result.ErrorType);
        Assert.Equal("Validation failed", result.Message);
    }

    [Fact]
    public void Message_Should_Accept_Null()
    {
        var result = new TestResultBase
        {
            Success = false,
            ErrorType = ErrorTypes.None,
            Message = null
        };

        Assert.Null(result.Message);
    }

    private sealed record TestResultBase : ResultBase;
}

public class ResultBaseGeneric_Tests
{
    [Fact]
    public void Constructor_Should_Initialize_With_Default_Values()
    {
        var result = new TestResultBaseGeneric
        {
            Success = false,
            ErrorType = ErrorTypes.None,
            Message = null,
            Result = null
        };

        Assert.False(result.Success);
        Assert.Equal(ErrorTypes.None, result.ErrorType);
        Assert.Null(result.Message);
        Assert.Null(result.Result);
    }

    [Fact]
    public void Result_Should_Be_Settable()
    {
        var result = new TestResultBaseGeneric
        {
            Success = false,
            ErrorType = ErrorTypes.None,
            Message = null,
            Result = "Test"
        };

        Assert.Equal("Test", result.Result);
    }

    [Fact]
    public void Should_Inherit_Base_Properties()
    {
        var result = new TestResultBaseGeneric
        {
            Success = true,
            ErrorType = ErrorTypes.None,
            Message = "ok",
            Result = "payload"
        };

        Assert.True(result.Success);
        Assert.Equal(ErrorTypes.None, result.ErrorType);
        Assert.Equal("ok", result.Message);
        Assert.Equal("payload", result.Result);
    }

    [Fact]
    public void Serialization_Should_Include_All_Fields_Even_When_Null()
    {
        var result = new TestResultBaseGeneric
        {
            Success = true,
            ErrorType = ErrorTypes.None,
            Message = null,
            Result = null
        };

        var json = JsonSerializer.Serialize(result);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("Success", out var successProp));
        Assert.True(successProp.GetBoolean());

        Assert.True(root.TryGetProperty("ErrorType", out var errorTypeProp));
        Assert.Equal(JsonValueKind.Number, errorTypeProp.ValueKind);

        Assert.True(root.TryGetProperty("Message", out var messageProp));
        Assert.Equal(JsonValueKind.Null, messageProp.ValueKind);

        Assert.True(root.TryGetProperty("Result", out var resultProp));
        Assert.Equal(JsonValueKind.Null, resultProp.ValueKind);
    }

    private sealed record TestResultBaseGeneric : ResultBase<string>;
}
