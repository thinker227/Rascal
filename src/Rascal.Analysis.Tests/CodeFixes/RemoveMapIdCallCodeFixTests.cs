using VerifyCS = Rascal.Analysis.Tests.Verifiers.CodeFixVerifier<Rascal.Analysis.Analyzers.UnnecessaryIdMapAnalyzer, Rascal.Analysis.CodeFixes.RemoveMapIdCallCodeFix>;

public class RemoveMapIdCallCodeFixProviderTests
{
    [Fact]
    public async Task FixesMapIdCall() => await VerifyCS.VerifyCodeFixAsync("""
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
