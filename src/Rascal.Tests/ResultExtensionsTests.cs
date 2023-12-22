namespace Rascal.Tests;

public class ResultExtensionsTests
{
    [Fact]
    public void Unnest()
    {
        var r = Ok(Ok(2));
        var x = r.Unnest();

        x.HasValue.ShouldBeTrue();
        x.value.ShouldBe(2);
    }

    [Fact]
    public void NotNull_Class()
    {
        {
            var x = "uwu";
            var r = x.NotNull("error");

            r.HasValue.ShouldBeTrue();
            r.value.ShouldBe("uwu");
        }

        {
            var x = null as string;
            var r = x.NotNull("error");

            r.HasValue.ShouldBeFalse();
            r.error?.Message.ShouldBe("error");
        }
    }

    [Fact]
    public void NotNull_Struct()
    {
        {
            var x = 2 as int?;
            var r = x.NotNull("error");

            r.HasValue.ShouldBeTrue();
            r.value.ShouldBe(2);
        }

        {
            var x = null as int?;
            var r = x.NotNull("error");

            r.HasValue.ShouldBeFalse();
            r.error?.Message.ShouldBe("error");
        }
    }

    [Fact]
    public void TryGetValueR()
    {
        {
            var dict = new Dictionary<string, int>() { ["a"] = 2 };
            var r = dict.TryGetValueR("a", "error");

            r.HasValue.ShouldBeTrue();
            r.value.ShouldBe(2);
        }

        {
            var dict = new Dictionary<string, int>();
            var r = dict.TryGetValueR("a", "error");

            r.HasValue.ShouldBeFalse();
            r.error?.Message.ShouldBe("error");
        }
    }

    [Fact]
    public void Index()
    {
        {
            var xs = new[] { 2 };
            var r = xs.Index(0, "error");

            r.HasValue.ShouldBeTrue();
            r.value.ShouldBe(2);
        }

        {
            var xs = Array.Empty<int>();
            var r = xs.Index(0, "error");

            r.HasValue.ShouldBeFalse();
            r.error?.Message.ShouldBe("error");
        }
    }
}
