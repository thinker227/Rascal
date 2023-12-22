namespace Rascal.Tests;

public class ResultTests
{
    [Fact]
    public void New_T()
    {
        var r = new Result<int>(2);
        
        r.HasValue.ShouldBeTrue();
        r.value.ShouldBe(2);
        r.error.ShouldBeNull();
    }

    [Fact]
    public void New_Error()
    {
        var r = new Result<int>(new TestError());

        r.HasValue.ShouldBeFalse();
        r.value.ShouldBe(default);
        r.error.ShouldBeOfType<TestError>();
    }

    [Fact]
    public void Error()
    {
        var r = default(Result<int>);

        r.HasValue.ShouldBeFalse();
        r.value.ShouldBe(default);
        r.error.ShouldBe(default);
        r.Error.ShouldBe(StringError.DefaultError);
    }

    [Fact]
    public void ToString_()
    {
        {
            var r = Ok(2);
            r.ToString().ShouldBe("Ok { 2 }");
        }

        {
            var r = Err<int>(new StringError("error"));
            r.ToString().ShouldBe("Error { error }");
        }
    }
}
