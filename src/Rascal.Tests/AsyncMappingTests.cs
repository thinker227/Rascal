namespace Rascal.Tests;

public class AsyncMappingTests
{
    [Fact]
    public async Task MapAsync_ReturnsOk_ForOk()
    {
        var r = Ok(2);
        var x = await r.MapAsync(x => Task.FromResult(x.ToString()));

        x.HasValue.ShouldBeTrue();
        x.value.ShouldBe("2");
    }

    [Fact]
    public async Task MapAsync_ReturnsErr_ForErr()
    {
        var r = Err<int>("error");
        var x = await r.MapAsync(x => Task.FromResult(x.ToString()));

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }

    [Fact]
    public async Task ThenAsync_ReturnsOk_ForOkAndMappingReturningOk()
    {
        var r = Ok(2);
        var x = await r.ThenAsync(x => Task.FromResult(Ok(x.ToString())));

        x.HasValue.ShouldBeTrue();
        x.value.ShouldBe("2");
    }

    [Fact]
    public async Task ThenAsync_ReturnsErr_ForOkAndMappingReturningErr()
    {
        var r = Ok(2);
        var x = await r.ThenAsync(_ => Task.FromResult(Err<string>("error")));

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }

    [Fact]
    public async Task ThenAsync_ReturnsErr_ForErr()
    {
        var r = Err<int>("error");
        var x = await r.ThenAsync(x => Task.FromResult(Ok(x.ToString())));

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }
    
    [Fact]
    public async Task TryMapAsync_ReturnsOk_ForOkWithoutException()
    {
        var r = Ok(2);
        var x = await r.TryMapAsync(x => Task.FromResult(x.ToString()));

        x.HasValue.ShouldBeTrue();
        x.value.ShouldBe("2");
    }

    [Fact]
    public async Task TryMapAsync_ReturnsErr_ForErrWithoutException()
    {
        var r = Err<int>("error");
        var x = r.TryMap(x => Task.FromResult(x.ToString()));

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }

    [Fact]
    public async Task TryMapAsync_ReturnsErr_ForOkWithException()
    {
        var r = Ok(2);
        var x = await r.TryMapAsync<string>(_ => throw new TestException());

        x.HasValue.ShouldBeFalse();
        var e = x.error.ShouldBeOfType<ExceptionError>();
        e.Exception.ShouldBeOfType<TestException>();
    }

    [Fact]
    public async Task TryMapAsync_ReturnsErr_ForErrWithException()
    {
        var r = Err<int>("error");
        var x = await r.TryMapAsync<string>(_ => throw new TestException());

        x.HasValue.ShouldBeFalse();
        var e = x.error.ShouldBeOfType<StringError>();
        e.Message.ShouldBe("error");
    }

    [Fact]
    public async Task ThenTryAsync_ReturnsOk_ForOkAndMappingReturningOk()
    {
        var r = Ok(2);
        var x = await r.ThenTryAsync(x => Task.FromResult(Ok(x.ToString())));

        x.HasValue.ShouldBeTrue();
        x.value.ShouldBe("2");
    }

    [Fact]
    public async Task ThenTryAsync_ReturnsErr_ForOkAndMappingReturningErr()
    {
        var r = Ok(2);
        var x = await r.ThenTryAsync(_ => Task.FromResult(Err<string>("error")));

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }

    [Fact]
    public async Task ThenTryAsync_ReturnsErr_ForErrAndMappingReturningErr()
    {
        var r = Err<int>("error");
        var x = await r.ThenTryAsync(x => Task.FromResult(Ok(x.ToString())));

        x.HasValue.ShouldBeFalse();
        x.error?.Message.ShouldBe("error");
    }

    [Fact]
    public async Task ThenTryAsync_ReturnsErr_ForOkAndMappingThrowing()
    {
        var r = Ok(2);
        var x = await r.ThenTryAsync<string>(_ => throw new TestException());

        x.HasValue.ShouldBeFalse();
        var e = x.error.ShouldBeOfType<ExceptionError>();
        e.Exception.ShouldBeOfType<TestException>();
    }

    [Fact]
    public async Task ThenTryAsync_ReturnsErr_ForErrAndMappingThrowing()
    {
        var r = Err<int>("error");
        var x = await r.ThenTryAsync<string>(_ => throw new TestException());

        x.HasValue.ShouldBeFalse();
        var e = x.error.ShouldBeOfType<StringError>();
        e.Message.ShouldBe("error");
    }
}
