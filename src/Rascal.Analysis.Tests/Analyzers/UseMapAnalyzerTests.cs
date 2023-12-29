using VerifyCS = Rascal.Analysis.Tests.Verifiers.AnalyzerVerifier<Rascal.Analysis.Analyzers.UseMapAnalyzer>;

namespace Rascal.Analysis.Analyzers.Tests;

public class UseMapAnalyzerTests
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
            var v = result.Then(x => F(x));
        }
        
        public static Result<int> F(int x) => Ok(x + 1);
    }
    """);
    
    [Fact]
    public async Task ReportsOnLambda() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;
    
    public static class Foo
    {
        public static void Bar()
        {
            var result = Ok(2);
            var v = result.{|RASCAL0001:Then|}(x => Ok(x));
        }
    }
    """);
    
    [Fact]
    public async Task ReportOnBlockBody() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;
    
    public static class Foo
    {
        public static void Bar()
        {
            var result = Ok(2);
            var v = result.{|RASCAL0001:Then|}(x =>
            {
                return Ok(x);
            });
        }
    }
    """);
}
