using Tickefy.Domain.Common.Errors;
using Tickefy.Domain.Common.Results;

namespace Tickefy.Domain.Tests.Common.Results;

public class ResultPatternTests
{
    [Fact]
    public void Result_Success_Should_Set_Success_State()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void Result_Failure_Should_Set_Failure_State_And_Error()
    {
        var error = new NotFoundError("entity");

        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ResultOfT_Success_Should_Set_Value_And_Success_State()
    {
        var result = Result<int>.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void ResultOfT_Failure_Should_Set_Failure_State_And_Error()
    {
        var error = new ForbiddenError("denied");

        var result = Result<int>.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void ResultOfT_Match_Should_Invoke_Success_Branch_On_Success()
    {
        var result = Result<string>.Success("ok");
        var failureCalled = false;

        var output = result.Match(
            onSuccess: value => value.ToUpperInvariant(),
            onFailure: _ =>
            {
                failureCalled = true;
                return "fail";
            });

        output.Should().Be("OK");
        failureCalled.Should().BeFalse();
    }

    [Fact]
    public void ResultOfT_Match_Should_Invoke_Failure_Branch_On_Failure()
    {
        var error = new InvalidArgumentError("bad");
        var result = Result<string>.Failure(error);
        var successCalled = false;

        var output = result.Match(
            onSuccess: _ =>
            {
                successCalled = true;
                return "ok";
            },
            onFailure: e => e.Message);

        output.Should().Be("bad");
        successCalled.Should().BeFalse();
    }
}
