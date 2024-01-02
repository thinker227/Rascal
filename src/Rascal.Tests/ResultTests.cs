using Rascal.Errors;

namespace Rascal.Tests;

public class ResultTests
{
    [Fact]
    public void New_T_HasValue()
    {
        var r = new Result<int>(2);
        
        r.IsOk.ShouldBeTrue();
        r.IsError.ShouldBeFalse();
        r.value.ShouldBe(2);
        r.error.ShouldBeNull();
    }

    [Fact]
    public void New_Error_HasError()
    {
        var r = new Result<int>(new TestError());

        r.IsOk.ShouldBeFalse();
        r.IsError.ShouldBeTrue();
        r.value.ShouldBe(default);
        r.error.ShouldBeOfType<TestError>();
    }

    [Fact]
    public void Default_IsDefault()
    {
        var r = default(Result<int>);

        r.IsOk.ShouldBeFalse();
        r.IsError.ShouldBeTrue();
        r.value.ShouldBe(default);
        r.error.ShouldBe(default);
        r.Error.ShouldBe(Error.DefaultValueError);
    }

    [Fact]
    public void ToString_ReturnsOkString()
    {
        var r = Ok(2);
        r.ToString().ShouldBe("Ok { 2 }");
    }

    [Fact]
    public void ToString_ReturnsErrorString()
    {
        var r = Err<int>(new StringError("error"));
        r.ToString().ShouldBe("Error { error }");
    }
}
