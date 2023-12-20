namespace Rascal.Tests;

public class UtilityTests
{
    [Fact]
    public void ImplicitConversion()
    {
        Result<int> x = 2;

        x.value.ShouldBe(2);
        x.hasValue.ShouldBeTrue();
    }

    [Fact]
    public void Combine()
    {
        {
            var a = Ok(2);
            var b = Ok("uwu");
            var r = a.Combine(b);

            r.hasValue.ShouldBeTrue();
            r.value.ShouldBe((2, "uwu"));
        }

        {
            var a = Ok(2);
            var b = Err<string>("error");
            var r = a.Combine(b);

            r.hasValue.ShouldBeFalse();
            r.error?.Message.ShouldBe("error");
        }

        {
            var a = Err<int>("error");
            var b = Ok("uwu");
            var r = a.Combine(b);

            r.hasValue.ShouldBeFalse();
            r.error?.Message.ShouldBe("error");
        }

        {
            var a = Err<int>("error a");
            var b = Err<string>("error b");
            var r = a.Combine(b);

            r.hasValue.ShouldBeFalse();
            var error = r.error.ShouldBeOfType<AggregateError>();
            error.Errors.Length.ShouldBe(2);
            error.Errors[0].Message.ShouldBe("error a");
            error.Errors[1].Message.ShouldBe("error b");
        }
    }

    [Fact]
    public void OfType()
    {
        {
            var r = Ok<object>("uwu").ToType<string>();

            r.hasValue.ShouldBeTrue();
            r.value.ShouldBe("uwu");
        }

        {
            var r = Ok<object>("uwu").ToType<int>();

            r.hasValue.ShouldBeFalse();
            r.error?.Message.ShouldNotBeNull();
        }
    }

    [Fact]
    public void ErrorIf()
    {
        {
            var r = Ok(2).Validate(_ => true, _ => "error");
            
            r.hasValue.ShouldBeFalse();
            r.error?.Message.ShouldBe("error");
        }

        {
            var r = Ok(2).Validate(_ => false, _ => "error");

            r.hasValue.ShouldBeTrue();
            r.value.ShouldBe(2);
        }

        {
            var r = Err<int>("error a").Validate(_ => true, _ => "error b");

            r.hasValue.ShouldBeFalse();
            r.error?.Message.ShouldBe("error a");
        }
    }

    [Fact]
    public void ToEnumerable()
    {
        {
            var r = Ok(2).ToEnumerable();

            r.ShouldHaveSingleItem().ShouldBe(2);
        }

        {
            var r = Err<int>("error").ToEnumerable();

            r.ShouldBeEmpty();
        }
    }
}
