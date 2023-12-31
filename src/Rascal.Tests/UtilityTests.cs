using System.Runtime.CompilerServices;
using Rascal.Errors;

namespace Rascal.Tests;

public class UtilityTests
{
    [Fact]
    public void ImplicitConversion()
    {
        Result<int> x = 2;

        x.value.ShouldBe(2);
        x.IsOk.ShouldBeTrue();
    }

    [Fact]
    public void Combine_ReturnsOk_ForOkAndOk()
    {
        var a = Ok(2);
        var b = Ok("uwu");
        var r = a.Combine(b);

        r.IsOk.ShouldBeTrue();
        r.value.ShouldBe((2, "uwu"));
    }

    [Fact]
    public void Combine_ReturnsErr_ForOkAndErr()
    {
        var a = Ok(2);
        var b = Err<string>("error");
        var r = a.Combine(b);

        r.IsOk.ShouldBeFalse();
        r.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void Combine_ReturnsErr_ForErrAndOk()
    {
        var a = Err<int>("error");
        var b = Ok("uwu");
        var r = a.Combine(b);

        r.IsOk.ShouldBeFalse();
        r.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void Combine_ReturnsErr_ForErrAndErr()
    {
        var a = Err<int>("error a");
        var b = Err<string>("error b");
        var r = a.Combine(b);

        r.IsOk.ShouldBeFalse();
        var error = r.error.ShouldBeOfType<AggregateError>();
        error.Errors.Select(x => x.Message).ShouldBe(["error a", "error b"]);
    }

    [Fact]
    public void OfType_ReturnsOk_ForOkToValidType()
    {
        var r = Ok<object>("uwu").ToType<string>();

        r.IsOk.ShouldBeTrue();
        r.value.ShouldBe("uwu");
    }

    [Fact]
    public void OfType_ReturnsErr_ForOkToInvalidType()
    {
        var r = Ok<object>("uwu").ToType<int>();

        r.IsOk.ShouldBeFalse();
        r.error?.Message.ShouldNotBeNull();
    }

    [Fact]
    public void OfType_ReturnsErr_ForErr()
    {
        var r = Err<object>("error").ToType<int>();

        r.IsOk.ShouldBeFalse();
        r.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void Validate_ReturnsErrWithDefaultError_ForOkAndFalse()
    {
        var r = Ok(2).Validate(_ => false);

        r.IsOk.ShouldBeFalse();
        r.error?.Message.ShouldNotBeNull();
    }

    [Fact]
    public void Validate_ReturnsErrWithError_ForOkAndFalse()
    {
        var r = Ok(2).Validate(_ => false, _ => "error");
        
        r.IsOk.ShouldBeFalse();
        r.error?.Message.ShouldBe("error");
    }
    
    [Fact]
    public void Validate_ReturnsOk_ForOk()
    {
        var r = Ok(2).Validate(_ => true, _ => "error");

        r.IsOk.ShouldBeTrue();
        r.value.ShouldBe(2);
    }

    [Fact]
    public void Validate_ReturnsErr_ForErr()
    {
        var r = Err<int>("error a").Validate(_ => false, _ => "error b");

        r.IsOk.ShouldBeFalse();
        r.error?.Message.ShouldBe("error a");
    }

    [Fact]
    public void ToEnumerable_ReturnsSingleItem_ForOk()
    {
        var r = Ok(2).ToEnumerable();

        r.ShouldHaveSingleItem().ShouldBe(2);
    }

    [Fact]
    public void ToEnumerable_ReturnsNoItems_ForErr()
    {
        var r = Err<int>("error").ToEnumerable();

        r.ShouldBeEmpty();
    }[Fact]
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
}
