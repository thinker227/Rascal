using System.Globalization;

namespace Rascal.Tests;

public class PreludeTests
{
    [Fact]
    public void Ok_()
    {
        var r = Ok(2);

        r.HasValue.ShouldBeTrue();
        r.value.ShouldBe(2);
    }

    [Fact]
    public void Err_()
    {
        var r = Err<int>("error");

        r.HasValue.ShouldBeFalse();
        r.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void ParseR_String()
    {
        {
            var r = ParseR<int>("123", CultureInfo.InvariantCulture);
            
            r.HasValue.ShouldBeTrue();
            r.value.ShouldBe(123);
        }

        {
            var r = ParseR<int>("24a", CultureInfo.InvariantCulture);

            r.HasValue.ShouldBeFalse();
            r.error.ShouldNotBeNull();
        }
    }

    [Fact]
    public void ParseR_Span()
    {
        {
            var s = "123".AsSpan();
            var r = ParseR<int>(s, CultureInfo.InvariantCulture);

            r.HasValue.ShouldBeTrue();
            r.value.ShouldBe(123);
        }

        {
            var s = "24a".AsSpan();
            var r = ParseR<int>(s, CultureInfo.InvariantCulture);

            r.HasValue.ShouldBeFalse();
            r.error.ShouldNotBeNull();
        }
    }

    [Fact]
    public void Try_()
    {
        {
            var r = Try(() => 2);

            r.HasValue.ShouldBeTrue();
            r.value.ShouldBe(2);
        }

        {
            var r = Try<int>(() => throw new TestException());

            r.HasValue.ShouldBeFalse();
            var e = r.error.ShouldBeOfType<ExceptionError>();
            e.Exception.ShouldBeOfType<TestException>();
        }
    }

    [Fact]
    public void Iterate_()
    {
        static Result<int> F(int x) =>
            x < 3
                ? Ok(x + 1)
                : Err<int>("error");

        {
            var xs = Iterate(1, F);
            xs.ShouldBe([1, 2, 3]);
        }

        {
            var xs = Iterate(Err<int>("error"), F);
            xs.ShouldBeEmpty();
        }
    }
}
