using System.Globalization;
using Rascal.Errors;

namespace Rascal.Tests;

public class ResultExtensionsTests
{
    [Fact]
    public void ToString_UsesFormat_ForOk()
    {
        var result = Ok(15);
        var str = result.ToString("b", CultureInfo.InvariantCulture);

        str.ShouldBe("Ok { 1111 }");
    }

    [Fact]
    public void ToString_ReturnsErrorMessage_ForErr()
    {
        var result = Err<int>("error");
        var str = result.ToString("b", CultureInfo.InvariantCulture);

        str.ShouldBe("Error { error }");
    }
    
    [Fact]
    public void Unnest()
    {
        var r = Ok(Ok(2));
        var x = r.Unnest();

        x.IsOk.ShouldBeTrue();
        x.value.ShouldBe(2);
    }

    [Fact]
    public void NotNull_Class_ReturnsOk_ForNotNull()
    {
        var x = "uwu";
        var r = x.NotNull("error");

        r.IsOk.ShouldBeTrue();
        r.value.ShouldBe("uwu");
    }

    [Fact]
    public void NotNull_Class_ReturnsErr_ForNull()
    {
        var x = null as string;
        var r = x.NotNull("error");

        r.IsOk.ShouldBeFalse();
        r.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void NotNull_Struct_ReturnsOk_ForNotNull()
    {
        var x = 2 as int?;
        var r = x.NotNull("error");

        r.IsOk.ShouldBeTrue();
        r.value.ShouldBe(2);
    }

    [Fact]
    public void NotNull_Struct_ReturnsErr_ForNull()
    {
        var x = null as int?;
        var r = x.NotNull("error");

        r.IsOk.ShouldBeFalse();
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
    public void Sequence_ReturnsAllOk_ForSequenceContainingAllOks()
    {
        IEnumerable<Result<int>> xs = [1, 2, 3, 4, 5];

        var result = xs.Sequence();
        
        result.IsOk.ShouldBeTrue();
        result.value.ShouldBe([1, 2, 3, 4, 5]);
    }

    [Fact]
    public void Sequence_ReturnsFirstError_ForSequenceContainingErrors()
    {
        IEnumerable<Result<int>> xs = [Ok(1), Ok(2), Err<int>("error 1"), Ok(4), Err<int>("error 2")];

        var result = xs.Sequence();

        result.IsOk.ShouldBeFalse();
        result.error?.Message.ShouldBe("error 1");
    }

    [Fact]
    public void Sequence_ReturnsOk_ForEmptySequence()
    {
        IEnumerable<Result<int>> xs = [];

        var result = xs.Sequence();

        result.IsOk.ShouldBeTrue();
        result.value.ShouldBeEmpty();
    }

    [Fact]
    public void TryGetValueR_ReturnsOk_IfKeyExists()
    {
        var dict = new Dictionary<string, int>() { ["a"] = 2 };
        var r = dict.TryGetValueR("a", "error");

        r.IsOk.ShouldBeTrue();
        r.value.ShouldBe(2);
    }

    [Fact]
    public void TryGetValueR_ReturnsErr_IfKeyIsMissing()
    {
        var dict = new Dictionary<string, int>();
        var r = dict.TryGetValueR("a", "error");

        r.IsOk.ShouldBeFalse();
        r.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void Index_ReturnsOk_IfWithinRange()
    {
        var xs = new[] { 2 };
        var r = xs.Index(0, "error");

        r.IsOk.ShouldBeTrue();
        r.value.ShouldBe(2);
    }

    [Fact]
    public void Index_ReturnsErr_IfOutsideRange()
    {
        var xs = Array.Empty<int>();
        var r = xs.Index(0, "error");

        r.IsOk.ShouldBeFalse();
        r.error?.Message.ShouldBe("error");
    }

    [Fact]
    public async Task CatchCancellation_ReturnsOk_IfFinished()
    {
        var r = await Task.FromResult(2).CatchCancellation();

        r.IsOk.ShouldBeTrue();
        r.value.ShouldBe(2);
    }

    [Fact]
    public async Task CatchCancellation_ReturnsErr_IfCanceled()
    {
        var cts = new CancellationTokenSource();

        var task = GetTask(cts.Token);
        await cts.CancelAsync();
        var r = await task.CatchCancellation();

        r.IsOk.ShouldBeFalse();
        r.error.ShouldBeOfType<CancellationError>();
        return;

        static async Task<int> GetTask(CancellationToken cancellationToken)
        {
            await Task.Delay(-1, cancellationToken);
            return 0;
        }
    }
}
