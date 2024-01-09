namespace Rascal.Analysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingSymbolAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Diagnostics.MissingSymbol);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterCompilationAction(compilationCtx =>
        {
            if (WellKnownSymbols.TryCreate(compilationCtx.Compilation, out _, out var errors))
                return;
            
            foreach (var error in errors)
            {
                compilationCtx.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.MissingSymbol,
                    null,
                    error.MemberName));
            }
        });
    }
}
