namespace Rascal.Tests;

public class MappingTests
{
    [Fact]
    public void Map()
    {
        {
            var r = Ok(2);
            var x = r.Map(x => x.ToString());

            x.HasValue.ShouldBeTrue();
            x.value.ShouldBe("2");
        }

        {
            var r = Err<int>("error");
            var x = r.Map(x => x.ToString());

            x.HasValue.ShouldBeFalse();
            x.error?.Message.ShouldBe("error");
        }
    }

    [Fact]
    public void Then()
    {
        {
            var r = Ok(2);
            var x = r.Then(x => Ok(x.ToString()));

            x.HasValue.ShouldBeTrue();
            x.value.ShouldBe("2");
        }

        {
            var r = Ok(2);
            var x = r.Then(_ => Err<string>("error"));

            x.HasValue.ShouldBeFalse();
            x.error?.Message.ShouldBe("error");
        }

        {
            var r = Err<int>("error");
            var x = r.Then(x => Ok(x.ToString()));

            x.HasValue.ShouldBeFalse();
            x.error?.Message.ShouldBe("error");
        }
    }

    [Fact]
    public void SelectMany_2()
    {
        {
            var r = Ok(2);
            var x = r.SelectMany(
                x => Ok(x.ToString()),
                (x, s) => (x, s));

            x.HasValue.ShouldBeTrue();
            x.value.ShouldBe((2, "2"));
        }

        {
            var r = Ok(2);
            var x = r.SelectMany(
                x => Err<string>("error"),
                (x, s) => (x, s));

            x.HasValue.ShouldBeFalse();
            x.error?.Message.ShouldBe("error");
        }

        {
            var r = Err<int>("error");
            var x = r.SelectMany(
                x => Ok(x.ToString()),
                (x, s) => (x, s));

            x.HasValue.ShouldBeFalse();
            x.error?.Message.ShouldBe("error");
        }
    }

    [Fact]
    public void TryMap()
    {
        {
            var r = Ok(2);
            var x = r.TryMap(x => x.ToString());

            x.HasValue.ShouldBeTrue();
            x.value.ShouldBe("2");
        }

        {
            var r = Err<int>("error");
            var x = r.TryMap(x => x.ToString());

            x.HasValue.ShouldBeFalse();
            x.error?.Message.ShouldBe("error");
        }

        {
            var r = Ok(2);
            var x = r.TryMap<string>(_ => throw new TestException());

            x.HasValue.ShouldBeFalse();
            var e = x.error.ShouldBeOfType<ExceptionError>();
            e.Exception.ShouldBeOfType<TestException>();
        }

        {
            var r = Err<int>("error");
            var x = r.TryMap<string>(_ => throw new TestException());

            x.HasValue.ShouldBeFalse();
            var e = x.error.ShouldBeOfType<StringError>();
            e.Message.ShouldBe("error");
        }
    }

    [Fact]
    public void ThenTry()
    {
        {
            var r = Ok(2);
            var x = r.ThenTry(x => Ok(x.ToString()));

            x.HasValue.ShouldBeTrue();
            x.value.ShouldBe("2");
        }

        {
            var r = Ok(2);
            var x = r.ThenTry(_ => Err<string>("error"));

            x.HasValue.ShouldBeFalse();
            x.error?.Message.ShouldBe("error");
        }

        {
            var r = Err<int>("error");
            var x = r.ThenTry(x => Ok(x.ToString()));

            x.HasValue.ShouldBeFalse();
            x.error?.Message.ShouldBe("error");
        }

        {
            var r = Ok(2);
            var x = r.ThenTry<string>(_ => throw new TestException());

            x.HasValue.ShouldBeFalse();
            var e = x.error.ShouldBeOfType<ExceptionError>();
            e.Exception.ShouldBeOfType<TestException>();
        }

        {
            var r = Err<int>("error");
            var x = r.ThenTry<string>(_ => throw new TestException());

            x.HasValue.ShouldBeFalse();
            var e = x.error.ShouldBeOfType<StringError>();
            e.Message.ShouldBe("error");
        }
    }

    [Fact]
    public void MapError()
    {
        {
            var r = Ok(2);
            var x = r.MapError(_ => new TestError());

            x.HasValue.ShouldBeTrue();
            x.value.ShouldBe(2);
        }

        {
            var r = Err<int>("error");
            var x = r.MapError(_ => new TestError());

            x.HasValue.ShouldBeFalse();
            x.error.ShouldBeOfType<TestError>();
        }
    }
}
