using VerifyCS = Rascal.Analysis.Tests.Verifiers.AnalyzerVerifier<Rascal.Analysis.Analyzers.UnnecessaryIdMapAnalyzer>;

namespace Rascal.Analysis.Analyzers.Tests;

public class UnnecessaryIdMapAnalyzerTests
{
    [Fact]
    public async Task DoesNothingUsually() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;
    
    public static class Foo
    {
        public static void Bar()
        {
            var result = Ok(2);
            var v = result.Map(x => x + 1);
        }
    }
    """);
    
    [Fact]
    public async Task ReportsOnMapIdCall() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var result = Ok(2);
            var v = result.{|RASCAL0003:Map(x => x)|};
        }
    }
    """);
}
