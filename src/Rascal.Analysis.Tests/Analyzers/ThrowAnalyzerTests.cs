using VerifyCS = Rascal.Analysis.Tests.Verifiers.AnalyzerVerifier<Rascal.Analysis.Analyzers.ThrowAnalyzer>;

namespace Rascal.Analysis.Analyzers.Tests;

public class ThrowAnalyzerTests
{
    [Fact]
    public async Task DoesNothingUsually() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;

    public static class Foo
    {
        public static Result<int> Bar()
        {
            return new(2);
        }
    }
    """);

    [Fact]
    public async Task ThrowInsideResultMethod() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;

    public static class Foo
    {
        public static Result<int> Bar()
        {
            {|RASCAL0001:throw|} new Exception();
        }
    }
    """);

    [Fact]
    public async Task ThrowInsideLocalFunction() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;

    public static class Foo
    {
        public static void Bar()
        {
            Baz();

            static Result<int> Baz()
            {
                {|RASCAL0001:throw|} new Exception();
            }
        }
    }
    """);

    [Fact]
    public async Task ThrowInsideLambda() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;

    public static class Foo
    {
        public static Func<Result<int>> func = () =>
        {
            {|RASCAL0001:throw|} new Exception();
        };
    }
    """);

    [Fact]
    public async Task ThrowInsideExpressionProperty() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;

    public static class Foo
    {
        public static Result<int> X => {|RASCAL0001:throw|} new Exception();
    }
    """);

    [Fact]
    public async Task ThrowInsideExplicitProperty() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;

    public static class Foo
    {
        public static Result<int> X
        {
            get
            {
                {|RASCAL0001:throw|} new Exception();
            }
        }
    }
    """);
}
