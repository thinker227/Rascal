namespace Rascal.Tests;

public class MatchingTests
{
    [Fact]
    public void Match_ReturnsOkFunc_ForOk()
    {
        var r = Ok(2);
        var x = r.Match(
            x => x,
            _ => throw new InvalidOperationException()
        );

        x.ShouldBe(2);
    }

    [Fact]
    public void Match_CallsErrFunc_ForErr()
    {
        var r = Err<int>("error");
        var x = r.Match(
            _ => throw new InvalidOperationException(),
            e => e
        );

        x.Message.ShouldBe("error");
    }

    [Fact]
    public void Switch_CallsOkAction_ForOk()
    {
        var called = false;

        var r = Ok(2);
        r.Switch(
            x =>
            {
                x.ShouldBe(2);
                called = true;
            },
            _ => throw new InvalidOperationException()
        );

        called.ShouldBeTrue();
    }

    [Fact]
    public void Switch_CallsErrAction_ForErr()
    {
        var called = false;

        var r = Err<int>("error");
        r.Switch(
            _ => throw new InvalidOperationException(),
            e =>
            {
                called = true;
                e.Message.ShouldBe("error");
            }
        );

        called.ShouldBeTrue();
    }
    
    [Fact]
    public async Task MatchAsync_ReturnsOkFunc_ForOk()
    {
        var r = Ok(2);
        var x = await r.MatchAsync(
            Task.FromResult,
            _ => throw new InvalidOperationException()
        );

        x.ShouldBe(2);
    }

    [Fact]
    public async Task MatchAsync_CallsErrFunc_ForErr()
    {
        var r = Err<int>("error");
        var x = await r.MatchAsync(
            _ => throw new InvalidOperationException(),
            Task.FromResult
        );

        x.Message.ShouldBe("error");
    }

    [Fact]
    public async Task SwitchAsync_CallsOkAction_ForOk()
    {
        var called = false;

        var r = Ok(2);
        await r.SwitchAsync(
            x =>
            {
                x.ShouldBe(2);
                called = true;
                return Task.CompletedTask;
            },
            _ => throw new InvalidOperationException()
        );

        called.ShouldBeTrue();
    }

    [Fact]
    public async Task SwitchAsync_CallsErrAction_ForErr()
    {
        var called = false;

        var r = Err<int>("error");
        await r.SwitchAsync(
            _ => throw new InvalidOperationException(),
            e =>
            {
                called = true;
                e.Message.ShouldBe("error");
                return Task.CompletedTask;
            }
        );

        called.ShouldBeTrue();
    }

    [Fact]
    public void TryGetValue_1_ReturnsTrueAndSetsValue_ForOk()
    {
        var r = Ok(2);
        r.TryGetValue(out var x).ShouldBeTrue();
        x.ShouldBe(2);
    }

    [Fact]
    public void TryGetValue_1_ReturnsFalse_ForErr()
    {
        var r = Err<int>("error");
        r.TryGetValue(out var x).ShouldBeFalse();
        x.ShouldBe(default);
    }

    [Fact]
    public void TryGetValue_2_ReturnsTrueAndSetsValue_ForOk()
    {
        var r = Ok(2);
        r.TryGetValue(out var x, out var e).ShouldBeTrue();
        x.ShouldBe(2);
        e.ShouldBe(default);
    }

    [Fact]
    public void TryGetValue_2_ReturnsFalseAndSetsError_ForErr()
    {
        var r = Err<int>("error");
        r.TryGetValue(out var x, out var e).ShouldBeFalse();
        x.ShouldBe(default);
        e?.Message.ShouldBe("error");
    }

    [Fact]
    public void TryGetError_1_ReturnsTrueAndSetsError_ForErr()
    {
        var r = Err<int>("error");
        r.TryGetError(out var e).ShouldBeTrue();
        e?.Message.ShouldBe("error");
    }

    [Fact]
    public void TryGetError_1_ReturnsFalse_ForOk()
    {
        var r = Ok(2);
        r.TryGetError(out var e).ShouldBeFalse();
        e.ShouldBe(default);
    }

    [Fact]
    public void TryGetError_2_ReturnsTrueAndSetsError_ForErr()
    {
        var r = Err<int>("error");
        r.TryGetError(out var e, out var x).ShouldBeTrue();
        e?.Message.ShouldBe("error");
        x.ShouldBe(default);
    }

    [Fact]
    public void TryGetError_2_ReturnsFalseAndSetsValue_ForOk()
    {
        var r = Ok(2);
        r.TryGetError(out var e, out var x).ShouldBeFalse();
        e.ShouldBe(default);
        x.ShouldBe(2);
    }

    [Fact]
    public void GetValueOrDefault_0_ReturnsValue_ForOk()
    {
        var r = Ok(2);
        r.GetValueOrDefault().ShouldBe(2);
    }

    [Fact]
    public void GetValueOrDefault_0_ReturnsDefault_ForErr()
    {
        var r = Err<int>("error");
        r.GetValueOrDefault().ShouldBe(default);
    }

    [Fact]
    public void GetValueOrDefault_Value_ReturnsValue_ForOk()
    {
        var r = Ok(2);
        r.GetValueOrDefault(1).ShouldBe(2);
    }

    [Fact]
    public void GetValueOrDefault_Value_ReturnsDefaultValue_ForErr()
    {
        var r = Err<int>("error");
        r.GetValueOrDefault(1).ShouldBe(1);
    }

    [Fact]
    public void GEtValueOrDefault_Function_ReturnsValue_ForOk()
    {
        var r = Ok(2);
        r.GetValueOrDefault(() => 1).ShouldBe(2);
    }

    [Fact]
    public void GetValueOrDefault_Function_CallsFunction_ForErr()
    {
        var r = Err<int>("error");
        r.GetValueOrDefault(() => 1).ShouldBe(1);
    }

    [Fact]
    public void Unwrap_ReturnsValue_ForOk()
    {
        var r = Ok(2);
        var x = r.Unwrap();

        x.ShouldBe(2);
    }

    [Fact]
    public void Unwrap_Throws_ForErr()
    {
        var r = Err<int>("error");
        
        Should.Throw<UnwrapException>(() => r.Unwrap());
    }

    [Fact]
    public void Expect_ReturnsValue_ForOk()
    {
        var r = Ok(2);
        var x = r.Expect("uwu");

        x.ShouldBe(2);
    }

    [Fact]
    public void Expect_Throws_ForErr()
    {
        var r = Err<int>("error");

        var ex = Should.Throw<UnwrapException>(() => r.Expect("uwu"));

        ex.Message.ShouldBe("uwu");
    }
}
