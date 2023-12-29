namespace Rascal.Tests;

public class MappingTests
{
    [Fact]
    public void Map_ReturnsOk_ForOk()
    {
        var r = Ok(2);
        var x = r.Map(x => x.ToString());

        x.HasValue.ShouldBeTrue();
        x.value.ShouldBe("2");
    }

    [Fact]
    public void Map_ReturnsErr_ForErr()
    {
        var r = Err<int>("error");
        var x = r.Map(x => x.ToString());

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void Then_ReturnsOk_ForOkAndMappingReturningOk()
    {
        var r = Ok(2);
        var x = r.Then(x => Ok(x.ToString()));

        x.HasValue.ShouldBeTrue();
        x.value.ShouldBe("2");
    }

    [Fact]
    public void Then_ReturnsErr_ForOkAndMappingReturningErr()
    {
        var r = Ok(2);
        var x = r.Then(_ => Err<string>("error"));

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void Then_ReturnsErr_ForErr()
    {
        var r = Err<int>("error");
        var x = r.Then(x => Ok(x.ToString()));

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void SelectMany_2_ReturnsOk_ForOkAndOtherReturningOk()
    {
        var r = Ok(2);
        var x = r.SelectMany(
            x => Ok(x.ToString()),
            (x, s) => (x, s));

        x.HasValue.ShouldBeTrue();
        x.value.ShouldBe((2, "2"));
    }

    [Fact]
    public void SelectMany_2_ReturnsErr_ForOkAndOtherReturningErr()
    {
        var r = Ok(2);
        var x = r.SelectMany(
            x => Err<string>("error"),
            (x, s) => (x, s));

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void SelectMany_2_ReturnsErr_ForErrAndOtherReturningErr()
    {
        var r = Err<int>("error");
        var x = r.SelectMany(
            x => Ok(x.ToString()),
            (x, s) => (x, s));

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void TryMap_ReturnsOk_ForOkWithoutException()
    {
        var r = Ok(2);
        var x = r.TryMap(x => x.ToString());

        x.HasValue.ShouldBeTrue();
        x.value.ShouldBe("2");
    }

    [Fact]
    public void TryMap_ReturnsErr_ForErrWithoutException()
    {
        var r = Err<int>("error");
        var x = r.TryMap(x => x.ToString());

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void TryMap_ReturnsErr_ForOkWithException()
    {
        var r = Ok(2);
        var x = r.TryMap<string>(_ => throw new TestException());

        x.HasValue.ShouldBeFalse();
        var e = x.error.ShouldBeOfType<ExceptionError>();
        e.Exception.ShouldBeOfType<TestException>();
    }

    [Fact]
    public void TryMap_ReturnsErr_ForErrWithException()
    {
        var r = Err<int>("error");
        var x = r.TryMap<string>(_ => throw new TestException());

        x.HasValue.ShouldBeFalse();
        var e = x.error.ShouldBeOfType<StringError>();
        e.Message.ShouldBe("error");
    }

    [Fact]
    public void ThenTry_ReturnsOk_ForOkAndMappingReturningOk()
    {
        var r = Ok(2);
        var x = r.ThenTry(x => Ok(x.ToString()));

        x.HasValue.ShouldBeTrue();
        x.value.ShouldBe("2");
    }

    [Fact]
    public void ThenTry_ReturnsErr_ForOkAndMappingReturningErr()
    {
        var r = Ok(2);
        var x = r.ThenTry(_ => Err<string>("error"));

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void ThenTry_ReturnsErr_ForErrAndMappingReturningErr()
    {
        var r = Err<int>("error");
        var x = r.ThenTry(x => Ok(x.ToString()));

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void ThenTry_ReturnsErr_ForOkAndMappingThrowing()
    {
        var r = Ok(2);
        var x = r.ThenTry<string>(_ => throw new TestException());

        x.HasValue.ShouldBeFalse();
        var e = x.error.ShouldBeOfType<ExceptionError>();
        e.Exception.ShouldBeOfType<TestException>();
    }

    [Fact]
    public void ThenTry_ReturnsErr_ForErrAndMappingThrowing()
    {
        var r = Err<int>("error");
        var x = r.ThenTry<string>(_ => throw new TestException());

        x.HasValue.ShouldBeFalse();
        var e = x.error.ShouldBeOfType<StringError>();
        e.Message.ShouldBe("error");
    }

    [Fact]
    public void MapError_ReturnsOk_ForOk()
    {
        var r = Ok(2);
        var x = r.MapError(_ => new TestError());

        x.HasValue.ShouldBeTrue();
        x.value.ShouldBe(2);
    }

    [Fact]
    public void MapError_ReturnsErr_ForErr()
    {
        var r = Err<int>("error");
        var x = r.MapError(_ => new TestError());

        x.HasValue.ShouldBeFalse();
        x.error.ShouldBeOfType<TestError>();
    }
}
