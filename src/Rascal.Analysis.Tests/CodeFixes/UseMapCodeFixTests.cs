using VerifyCS = Rascal.Analysis.Tests.Verifiers.CodeFixVerifier<Rascal.Analysis.Analyzers.UseMapAnalyzer, Rascal.Analysis.CodeFixes.UseMapCodeFix>;

namespace Rascal.Analysis.CodeFixes.Tests;

public class UseMapCodeFixTests
{
    [Fact]
    public async Task RemovesMap_Lambda() => await VerifyCS.VerifyCodeFixAsync("""
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
    
    [Fact]
    public async Task RemovesMap_Ctor_Explicit() => await VerifyCS.VerifyCodeFixAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var result = Ok(2);
            var v = result.{|RASCAL0001:Then|}(x => new Result<int>(x));
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
    
    [Fact]
    public async Task RemovesMap_Ctor_Implicit() => await VerifyCS.VerifyCodeFixAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;
    
    public static class Foo
    {
        public static void Bar()
        {
            var result = Ok(2);
            var v = result.{|RASCAL0001:Then<int>|}(x => new(x));
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
    
    [Fact]
    public async Task RemovesMap_Conversion() => await VerifyCS.VerifyCodeFixAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;
    
    public static class Foo
    {
        public static void Bar()
        {
            var result = Ok(2);
            var v = result.{|RASCAL0001:Then|}(x => (Result<int>)x);
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
