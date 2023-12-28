using System.Globalization;

namespace Rascal.Tests;

public class PreludeTests
{
    [Fact]
    public void Ok_ReturnsOk()
    {
        var r = Ok(2);

        r.IsOk.ShouldBeTrue();
        r.value.ShouldBe(2);
    }

    [Fact]
    public void Err_ReturnsErr()
    {
        var r = Err<int>("error");

        r.IsOk.ShouldBeFalse();
        r.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void ParseR_String_ReturnsOk_ForValidString()
    {
        var r = ParseR<int>("123", CultureInfo.InvariantCulture);
        
        r.IsOk.ShouldBeTrue();
        r.value.ShouldBe(123);
    }

    [Fact]
    public void ParseR_String_ReturnsErr_ForInvalidString()
    {
        var r = ParseR<int>("24a", CultureInfo.InvariantCulture);

        r.IsOk.ShouldBeFalse();
        r.error.ShouldNotBeNull();
    }

    [Fact]
    public void ParseR_Span_ReturnsOk_ForValidString()
    {
        var s = "123".AsSpan();
        var r = ParseR<int>(s, CultureInfo.InvariantCulture);
        
        r.IsOk.ShouldBeTrue();
        r.value.ShouldBe(123);
    }

    [Fact]
    public void ParseR_Span_ReturnsErr_ForInvalidString()
    {
        var s = "24a".AsSpan();
        var r = ParseR<int>(s, CultureInfo.InvariantCulture);

        r.IsOk.ShouldBeFalse();
        r.error.ShouldNotBeNull();
    }

    [Fact]
    public void Try_ReturnsOk_ForNoException()
    {
        var r = Try(() => 2);

        r.IsOk.ShouldBeTrue();
        r.value.ShouldBe(2);
    }

    [Fact]
    public void Try_ReturnsErr_ForException()
    {
        var r = Try<int>(() => throw new TestException());

        r.IsOk.ShouldBeFalse();
        var e = r.error.ShouldBeOfType<ExceptionError>();
        e.Exception.ShouldBeOfType<TestException>();
    }

    private static Result<int> IterateFunc(int x) =>
        x < 3
            ? Ok(x + 1)
            : Err<int>("error");

    [Fact]
    public void Iterate_ReturnsUntilError()
    {
        var xs = Iterate(1, IterateFunc);
        xs.ShouldBe([1, 2, 3]);
    }

    [Fact]
    public void Iterate_ReturnsEmpty_ForErr()
    {
        var xs = Iterate(Err<int>("error"), IterateFunc);
        xs.ShouldBeEmpty();
    }
}
