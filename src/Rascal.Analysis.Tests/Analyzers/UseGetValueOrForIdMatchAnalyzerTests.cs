using VerifyCS = Rascal.Analysis.Tests.Verifiers.AnalyzerVerifier<Rascal.Analysis.Analyzers.UseGetValueOrForIdMatchAnalyzer>;

namespace Rascal.Analysis.Analyzers.Tests;

public class UseGetValueOrForIdMatchAnalyzerTests
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
            var r = Ok(2);
            var x = r.Match(x => x + 1, _ => 0);
        }
    }
    """);
    
    [Fact]
    public async Task ReportsOnIdMatchCall() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var r = Ok(2);
            var x = r.{|RASCAL0006:Match|}(x => x, _ => 0);
        }
    }
    """);
}
