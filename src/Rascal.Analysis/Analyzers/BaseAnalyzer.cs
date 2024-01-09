namespace Rascal.Analysis.Analyzers;

public abstract class BaseAnalyzer : DiagnosticAnalyzer
{
    public sealed override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterCompilationStartAction(compilationCtx =>
        {
            // If any errors are present, they will be reported by MissingSymbolAnalyzer.
            if (WellKnownSymbols.TryCreate(compilationCtx.Compilation, out var symbols, out _))
                Handle(compilationCtx, symbols);
        });
    }

    protected abstract void Handle(CompilationStartAnalysisContext ctx, WellKnownSymbols symbols);
}
