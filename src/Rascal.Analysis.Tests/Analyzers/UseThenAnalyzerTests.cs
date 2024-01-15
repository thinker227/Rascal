using VerifyCS = Rascal.Analysis.Tests.Verifiers.AnalyzerVerifier<Rascal.Analysis.Analyzers.UseThenAnalyzer>;

namespace Rascal.Analysis.Analyzers.Tests;

public class UseThenAnalyzerTests
{
    [Fact]
    public async Task DoesNotReport_OnMapWithoutUnnest() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;
    
    public static class Foo
    {
        public static void Bar()
        {
            var result = Ok(2);
            var v = result.Map(x => Ok(2));
        }
    }
    """);
    
    [Fact]
    public async Task Report_OnMapUnnest_ExtensionForm() => await VerifyCS.VerifyAnalyzerAsync("""
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
    """);

    [Fact]
    public async Task Reports_OnMapUnnest_MethodForm() => await VerifyCS.VerifyAnalyzerAsync("""
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
    """);
}
