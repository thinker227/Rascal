namespace Rascal.Tests;

public class EqualityTests
{
    [Fact]
    public void Equals_T()
    {
        {
            var a = Ok(2);
            var b = 2;

            a.Equals(b).ShouldBeTrue();
        }

        {
            var a = Ok(2);
            var b = 3;

            a.Equals(b).ShouldBeFalse();
        }

        {
            var a = Err<int>("error");
            var b = 2;

            a.Equals(b).ShouldBeFalse();
        }
    }

    [Fact]
    public void Equals_Result()
    {
        {
            var a = Ok(2);
            var b = Ok(2);

            a.Equals(b).ShouldBeTrue();
        }

        {
            var a = Ok(2);
            var b = Err<int>("error");

            a.Equals(b).ShouldBeFalse();
        }

        {
            var a = Err<int>("error");
            var b = Ok(2);

            a.Equals(b).ShouldBeFalse();
        }

        {
            var a = Err<int>("error 1");
            var b = Err<int>("error 2");

            a.Equals(b).ShouldBeTrue();
        }
    }

    [Fact]
    public void GetHashCode_()
    {
        {
            var r = Ok(2);

            r.GetHashCode().ShouldBe(2.GetHashCode());
        }

        {
            var r = Ok<string?>(null);

            r.GetHashCode().ShouldBe(0);
        }

        {
            var r = Err<int>("error");

            r.GetHashCode().ShouldBe(0);
        }
    }
}
