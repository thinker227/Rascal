using VerifyCS = Rascal.Analysis.Tests.Verifiers.CodeFixVerifier<Rascal.Analysis.Analyzers.UnnecessaryIdMapAnalyzer, Rascal.Analysis.CodeFixes.RemoveMapIdCallCodeFix>;

namespace Rascal.Analysis.CodeFixes.Tests;

public class RemoveMapIdCallCodeFixProviderTests
{
    [Fact]
    public async Task Fixes_MapIdCall() => await VerifyCS.VerifyCodeFixAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;
    
    public static class Foo
    {
        public static void Bar()
        {
            var result = Ok(2);
            var v = result.{|RASCAL003:Map(x => x)|};
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
            var v = result;
        }
    }
    """);
}
