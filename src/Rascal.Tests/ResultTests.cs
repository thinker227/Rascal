namespace Rascal.Tests;

public class ResultTests
{
    [Fact]
    public void New_T_HasValue()
    {
        var r = new Result<int>(2);
        
        r.HasValue.ShouldBeTrue();
        r.value.ShouldBe(2);
        r.error.ShouldBeNull();
    }

    [Fact]
    public void New_Error_HasError()
    {
        var r = new Result<int>(new TestError());

        r.HasValue.ShouldBeFalse();
        r.value.ShouldBe(default);
        r.error.ShouldBeOfType<TestError>();
    }

    [Fact]
    public void Default_IsDefault()
    {
        var r = default(Result<int>);

        r.HasValue.ShouldBeFalse();
        r.value.ShouldBe(default);
        r.error.ShouldBe(default);
        r.Error.ShouldBe(StringError.DefaultError);
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
