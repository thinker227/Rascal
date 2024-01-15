namespace Rascal.Tests;

public class EqualityCoreTests
{
    [Fact]
    public void Equals_T_ReturnsTrue_ForOkAndEqualValue()
    {
        var a = Ok(2);
        var b = 2;

        EqualityCore.Equals(a, b, EqualityComparer<int>.Default).ShouldBeTrue();
    }

    [Fact]
    public void Equals_T_ReturnsFalse_ForOkAndUnequalValue()
    {
        var a = Ok(2);
        var b = 3;

        EqualityCore.Equals(a, b, EqualityComparer<int>.Default).ShouldBeFalse();
    }

    [Fact]
    public void Equals_T_ReturnsFalse_ForErr()
    {
        var a = Err<int>("error");
        var b = 2;

        EqualityCore.Equals(a, b, EqualityComparer<int>.Default).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Result_ReturnsTrue_ForOkAndOkAndEqualValue()
    {
        var a = Ok(2);
        var b = Ok(2);

        EqualityCore.Equals(a, b, EqualityComparer<int>.Default).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Result_ReturnsFalse_ForOkAndOkAndUnequalValue()
    {
        var a = Ok(2);
        var b = Ok(3);

        EqualityCore.Equals(a, b, EqualityComparer<int>.Default).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Result_ReturnsFalse_ForOkAndErr()
    {
        var a = Ok(2);
        var b = Err<int>("error");

        EqualityCore.Equals(a, b, EqualityComparer<int>.Default).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Result_ReturnsFalse_ForErrAndOk()
    {
        var a = Err<int>("error");
        var b = Ok(2);

        EqualityCore.Equals(a, b, EqualityComparer<int>.Default).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Result_ReturnsTrue_ForErrAndErr()
    {
        var a = Err<int>("error 1");
        var b = Err<int>("error 2");

        EqualityCore.Equals(a, b, EqualityComparer<int>.Default).ShouldBeTrue();
    }

    [Fact]
    public void GetHashCode_ReturnsHashCode_ForOk()
    {
        var r = Ok(2);

        EqualityCore.GetHashCode(r, EqualityComparer<int>.Default).ShouldBe(2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_Returns0_ForNull()
    {
        var r = Ok<string?>(null);

        EqualityCore.GetHashCode(r, EqualityComparer<string?>.Default).ShouldBe(0);
    }

    [Fact]
    public void GetHashCode_Returns0_ForErr()
    {
        var r = Err<int>("error");

        EqualityCore.GetHashCode(r, EqualityComparer<int>.Default).ShouldBe(0);
    }
}
