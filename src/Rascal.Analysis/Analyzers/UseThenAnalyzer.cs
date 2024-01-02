using Microsoft.CodeAnalysis.Operations;

namespace Rascal.Analysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseThenAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Diagnostics.UseThen);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterCompilationStartAction(compilationCtx =>
        {
            var resultType = compilationCtx.Compilation.GetTypeByMetadataName("Rascal.Result`1");
            if (resultType is null) return;

            var resultExtensionsType = compilationCtx.Compilation.GetTypeByMetadataName("Rascal.ResultExtensions");
            if (resultExtensionsType is null) return;
            
            var resultMembers = resultType.GetMembers();
            var resultExtensionsMembers = resultExtensionsType.GetMembers();

            var mapMethod = (IMethodSymbol)resultMembers.First(x => x.Name == "Map");
            var unnestMethod = (IMethodSymbol)resultExtensionsMembers.First(x => x.Name == "Unnest");
            
            compilationCtx.RegisterOperationAction(operationCtx =>
            {
                var operation = (IInvocationOperation)operationCtx.Operation;

                // Check that it is Unnest being called.
                if (!operation.TargetMethod.OriginalDefinition.Equals(unnestMethod, SymbolEqualityComparer.Default))
                    return;

                // Check that the first argument is an invocation.
                if (operation.Arguments is not
                [
                    {
                        Value: IInvocationOperation argumentInvocation
                    }
                ]) return;

                // Check that the invoked method is Map.
                if (!argumentInvocation.TargetMethod.OriginalDefinition
                        .Equals(mapMethod, SymbolEqualityComparer.Default))
                    return;

                // Get the location of the method name.
                if (argumentInvocation.Syntax is not InvocationExpressionSyntax
                {
                    Expression: MemberAccessExpressionSyntax memberAccessExpression
                }) return;
                var location = memberAccessExpression.GetLocation();

                // Report the diagnostic.
                operationCtx.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UseThen,
                    location));
            }, OperationKind.Invocation);
        });
    }
}
