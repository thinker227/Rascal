using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Rascal.Analysis.Tests.Verifiers;

public class CodeFixTest<TAnalyzer, TCodeFix> : CSharpCodeFixTest<TAnalyzer, TCodeFix, XUnitVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    public CodeFixTest() => SolutionTransforms.Add((solution, projectId) =>
    {
        var compilationOptions = solution.GetProject(projectId)!.CompilationOptions;
        compilationOptions = compilationOptions!.WithSpecificDiagnosticOptions(
            compilationOptions.SpecificDiagnosticOptions.SetItems(VerifierHelper.NullableWarnings));
        solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

        return solution;
    });
}
