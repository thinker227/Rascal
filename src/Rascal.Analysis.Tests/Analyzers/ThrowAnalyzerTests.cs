using VerifyCS = Rascal.Analysis.Tests.Verifiers.AnalyzerVerifier<Rascal.Analysis.Analyzers.ThrowAnalyzer>;

namespace Rascal.Analysis.Analyzers.Tests;

public class ThrowAnalyzerTests
{
    [Fact]
    public Task DoesNothingUsually() => VerifyCS.VerifyAnalyzerAsync("""
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
    public Task ThrowInsideResultMethod() => VerifyCS.VerifyAnalyzerAsync("""
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
    public Task ThrowInsideLocalFunction() => VerifyCS.VerifyAnalyzerAsync("""
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
}
