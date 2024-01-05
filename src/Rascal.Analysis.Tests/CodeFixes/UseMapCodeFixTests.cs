using VerifyCS = Rascal.Analysis.Tests.Verifiers.CodeFixVerifier<Rascal.Analysis.Analyzers.UseMapAnalyzer, Rascal.Analysis.CodeFixes.UseMapCodeFix>;

namespace Rascal.Analysis.CodeFixes.Tests;

public class UseMapCodeFixTests
{
    [Fact]
    public async Task RemovesMap() => await VerifyCS.VerifyCodeFixAsync("""
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
    """, """
    using System;
    using Rascal;
    using static Rascal.Prelude;
    
    public static class Foo
    {
        public static void Bar()
        {
            var result = Ok(2);
            var v = result.Map(x => x);
        }
    }
    """);
}
