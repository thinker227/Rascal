using System.Runtime.CompilerServices;

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
    public void NotNull_Class_ReturnsOk_ForNotNull()
    {
        var x = "uwu";
        var r = x.NotNull("error");

        r.HasValue.ShouldBeTrue();
        r.value.ShouldBe("uwu");
    }

    [Fact]
    public void NotNull_Class_ReturnsErr_ForNull()
    {
        var x = null as string;
        var r = x.NotNull("error");

        r.HasValue.ShouldBeFalse();
        r.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void NotNull_Struct_ReturnsOk_ForNotNull()
    {
        var x = 2 as int?;
        var r = x.NotNull("error");

        r.HasValue.ShouldBeTrue();
        r.value.ShouldBe(2);
    }

    [Fact]
    public void NotNull_Struct_ReturnsErr_ForNull()
    {
        var x = null as int?;
        var r = x.NotNull("error");

        r.HasValue.ShouldBeFalse();
        r.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void GeTValueOrNull_ReturnsValue_ForOk()
    {
        var r = Ok(2);
        var x = r.GetValueOrNull();

        x.ShouldBe(2);
    }

    [Fact]
    public void GetValueOrNull_ReturnsNull_ForErr()
    {
        var r = Err<int>("error");
        var x = r.GetValueOrNull();

        x.ShouldBe(null);
    }

    [Fact]
    public void AsRef_ReturnsRefToValue_ForOk()
    {
        var r = Ok(2);
        ref readonly var v = ref r.value;

        ref readonly var x = ref r.AsRef();

        Unsafe.AreSame(in x, in v).ShouldBeTrue();
        x.ShouldBe(2);
    }

    [Fact]
    public void AsRef_ReturnsRefToDefault_ForErr()
    {
        var r = Err<int>("error");
        ref readonly var v = ref r.value;

        ref readonly var x = ref r.AsRef();

        Unsafe.AreSame(in x, in v).ShouldBeTrue();
        x.ShouldBe(default);
    }

    [Fact]
    public void TryGetValueR_ReturnsOk_IfKeyExists()
    {
        var dict = new Dictionary<string, int>() { ["a"] = 2 };
        var r = dict.TryGetValueR("a", "error");

        r.HasValue.ShouldBeTrue();
        r.value.ShouldBe(2);
    }

    [Fact]
    public void TryGetValueR_ReturnsErr_IfKeyIsMissing()
    {
        var dict = new Dictionary<string, int>();
        var r = dict.TryGetValueR("a", "error");

        r.HasValue.ShouldBeFalse();
        r.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void Index_ReturnsOk_IfWithinRange()
    {
        var xs = new[] { 2 };
        var r = xs.Index(0, "error");

        r.HasValue.ShouldBeTrue();
        r.value.ShouldBe(2);
    }

    [Fact]
    public void Index_ReturnsErr_IfOutsideRange()
    {
        var xs = Array.Empty<int>();
        var r = xs.Index(0, "error");

        r.HasValue.ShouldBeFalse();
        r.error?.Message.ShouldBe("error");
    }
}
