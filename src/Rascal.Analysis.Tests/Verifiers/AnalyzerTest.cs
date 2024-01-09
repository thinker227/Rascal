using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Rascal.Analysis.Tests.Verifiers;

public class AnalyzerTest<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public AnalyzerTest() => SolutionTransforms.Add((solution, projectId) =>
    {
        var compilationOptions = solution.GetProject(projectId)!.CompilationOptions;
        compilationOptions = compilationOptions!.WithSpecificDiagnosticOptions(
            compilationOptions.SpecificDiagnosticOptions.SetItems(VerifierHelper.NullableWarnings));
        solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

        return solution;
    });
}
