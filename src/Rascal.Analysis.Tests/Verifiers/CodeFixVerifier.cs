using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Rascal.Analysis.Tests.Verifiers;

public static partial class CodeFixVerifier<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, string)"/>
    public static async Task VerifyCodeFixAsync(string source, string fixedSource)
    {
        var test = new CodeFixTest<TAnalyzer, TCodeFix>
        {
            // Replace line endings because the formatter uses the environment's newline.
            TestCode = source,
            FixedCode = fixedSource,
        };

        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile("./Rascal.dll"));

        await test.RunAsync(CancellationToken.None);
    }
}
