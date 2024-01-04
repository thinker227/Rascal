using VerifyCS = Rascal.Analysis.Tests.Verifiers.CodeFixVerifier<Rascal.Analysis.Analyzers.UseDefaultOrForIdMatchAnalyzer, Rascal.Analysis.CodeFixes.UseDefaultOrForIdMatchCodeFix>;

namespace Rascal.Analysis.CodeFixes.Tests;

public class UseDefaultOrForIdMatchCodeFixTests
{
    [Fact]
    public async Task Fixes_MatchIdCall_WithError() => await VerifyCS.VerifyCodeFixAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;
    
    public static class Foo
    {
        public static void Bar()
        {
            var r = Ok("uwu");
            var x = r.{|RASCAL0006:Match|}(x => x, e => e.Message);
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
            var r = Ok("uwu");
            var x = r.GetValueOr(e => e.Message);
        }
    }
    """);
    
    [Fact]
    public async Task Fixes_MatchIdCall_WithDiscard_NonConstant() => await VerifyCS.VerifyCodeFixAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var r = Ok("uwu");
            var v = "owo";
            var x = r.{|RASCAL0006:Match|}(x => x, _ => v);
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
            var r = Ok("uwu");
            var v = "owo";
            var x = r.GetValueOr(() => v);
        }
    }
    """);
    
    [Fact]
    public async Task Fixes_MatchIdCall_WithDiscard_Constant() => await VerifyCS.VerifyCodeFixAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var r = Ok("uwu");
            var x = r.{|RASCAL0006:Match|}(x => x, _ => "owo");
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
            var r = Ok("uwu");
            var x = r.GetValueOr("owo");
        }
    }
    """);
}
