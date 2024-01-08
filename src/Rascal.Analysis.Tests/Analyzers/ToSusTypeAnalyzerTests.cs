using VerifyCS = Rascal.Analysis.Tests.Verifiers.AnalyzerVerifier<Rascal.Analysis.Analyzers.ToSusTypeAnalyzer>;

namespace Rascal.Analysis.Analyzers.Tests;

public class ToSusTypeAnalyzerTests
{
    [Fact]
    public async Task Reports_SameType_OnSameType() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var x = Ok(2);
            var r = x.To<{|RASCAL004:int|}>();
        }
    }
    """);

    [Fact]
    public async Task Reports_ImpossibleType_OnBothStructTypes() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var x = Ok(2);
            var r = x.To<{|RASCAL005:bool|}>();
        }
    }
    """);

    [Fact]
    public async Task Reports_ImpossibleType_OnBothClassTypes_OutsideHierarchy() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var x = Ok(new B());
            var r = x.To<{|RASCAL005:C|}>();
        }
    }

    class A {}
    class B : A {}
    class C : A {}
    """);

    [Fact]
    public async Task DoesNotReport_OnBothClassTypes_InsideHierarchy_Up() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var x = Ok(new B());
            var r = x.To<A>();
        }
    }

    class A {}
    class B : A {}
    """);

    [Fact]
    public async Task DoesNotReport_OnBothClassTypes_InsideHierarchy_Down() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var x = Ok(new A());
            var r = x.To<B>();
        }
    }

    class A {}
    class B : A {}
    """);

    [Fact]
    public async Task DoesNotReport_OnFromInterfaceType() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var x = Err<I>("error");
            var r = x.To<int>();
        }
    }

    interface I {}
    """);

    [Fact]
    public async Task DoesNotReport_OnToInterfaceType() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar()
        {
            var x = Ok(2);
            var r = x.To<I>();
        }
    }

    interface I {}
    """);

    [Fact]
    public async Task DoesNotReport_OnFromTypeParameter() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar<T>()
        {
            var x = Err<T>("error");
            var r = x.To<int>();
        }
    }
    """);

    [Fact]
    public async Task DoesNotReport_OnToTypeParameter() => await VerifyCS.VerifyAnalyzerAsync("""
    using System;
    using Rascal;
    using static Rascal.Prelude;

    public static class Foo
    {
        public static void Bar<T>()
        {
            var x = Ok(2);
            var r = x.To<T>();
        }
    }
    """);
}
