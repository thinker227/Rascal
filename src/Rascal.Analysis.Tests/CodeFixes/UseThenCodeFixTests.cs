using VerifyCS = Rascal.Analysis.Tests.Verifiers.CodeFixVerifier<Rascal.Analysis.Analyzers.UseThenAnalyzer, Rascal.Analysis.CodeFixes.UseThenCodeFix>;

namespace Rascal.Analysis.CodeFixes.Tests;

public class UseThenCodeFixTests
{
    [Fact]
    public async Task FixesExtensionForm() => await VerifyCS.VerifyCodeFixAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var result = Ok(2);
            var v = result.{|RASCAL002:Map|}(x => Ok(x)).Unnest();
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
            var v = result.Then(x => Ok(x));
        }
    }
    """);
    
    [Fact]
    public async Task FixesCallForm() => await VerifyCS.VerifyCodeFixAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var result = Ok(2);
            var v = ResultExtensions.Unnest(result.{|RASCAL002:Map|}(x => Ok(x)));
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
            var v = result.Then(x => Ok(x));
        }
    }
    """);
}
