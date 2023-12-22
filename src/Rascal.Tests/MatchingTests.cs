namespace Rascal.Tests;

public class MatchingTests
{
    [Fact]
    public void Match()
    {
        {
            var r = Ok(2);
            var x = r.Match(
                x => x,
                _ => throw new InvalidOperationException()
            );

            x.ShouldBe(2);
        }

        {
            var r = Err<int>("error");
            var x = r.Match(
                _ => throw new InvalidOperationException(),
                e => e
            );

            x.Message.ShouldBe("error");
        }
    }

    [Fact]
    public void Switch()
    {
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
    }

    [Fact]
    public void TryGetValue_1()
    {
        {
            var r = Ok(2);
            r.TryGetValue(out var x).ShouldBeTrue();
            x.ShouldBe(2);
        }

        {
            var r = Err<int>("error");
            r.TryGetValue(out var x).ShouldBeFalse();
            x.ShouldBe(default);
        }
    }

    [Fact]
    public void TryGetValue_2()
    {
        {
            var r = Ok(2);
            r.TryGetValue(out var x, out var e).ShouldBeTrue();
            x.ShouldBe(2);
            e.ShouldBe(default);
        }

        {
            var r = Err<int>("error");
            r.TryGetValue(out var x, out var e).ShouldBeFalse();
            x.ShouldBe(default);
            e.Message.ShouldBe("error");
        }
    }

    [Fact]
    public void TryGetError_1()
    {
        {
            var r = Err<int>("error");
            r.TryGetError(out var e).ShouldBeTrue();
            e.Message.ShouldBe("error");
        }

        {
            var r = Ok(2);
            r.TryGetError(out var e).ShouldBeFalse();
            e.ShouldBe(default);
        }
    }

    [Fact]
    public void TryGetError_2()
    {
        {
            var r = Err<int>("error");
            r.TryGetError(out var e, out var x).ShouldBeTrue();
            e.Message.ShouldBe("error");
            x.ShouldBe(default);
        }
        
        {
            var r = Ok(2);
            r.TryGetError(out var e, out var x).ShouldBeFalse();
            e.ShouldBe(default);
            x.ShouldBe(2);
        }
    }

    [Fact]
    public void GetValueOrDefault_0()
    {
        {
            var r = Ok(2);
            r.GetValueOrDefault().ShouldBe(2);
        }

        {
            var r = Err<int>("error");
            r.GetValueOrDefault().ShouldBe(default);
        }
    }

    [Fact]
    public void GetValueOrDefault_Value()
    {
        {
            var r = Ok(2);
            r.GetValueOrDefault(1).ShouldBe(2);
        }

        {
            var r = Err<int>("error");
            r.GetValueOrDefault(1).ShouldBe(1);
        }
    }

    [Fact]
    public void GetValueOrDefault_Function()
    {
        {
            var r = Ok(2);
            r.GetValueOrDefault(() => 1).ShouldBe(2);
        }

        {
            var r = Err<int>("error");
            r.GetValueOrDefault(() => 1).ShouldBe(1);
        }
    }

    [Fact]
    public void Unwrap()
    {
        {
            var r = Ok(2);
            var x = r.Unwrap();

            x.ShouldBe(2);
        }

        {
            var r = Err<int>("error");
            
            Should.Throw<UnwrapException>(() => r.Unwrap());
        }
    }

    [Fact]
    public void Expect()
    {
        {
            var r = Ok(2);
            var x = r.Expect("uwu");

            x.ShouldBe(2);
        }

        {
            var r = Err<int>("error");

            var ex = Should.Throw<UnwrapException>(() => r.Expect("uwu"));

            ex.Message.ShouldBe("uwu");
        }
    }
}
