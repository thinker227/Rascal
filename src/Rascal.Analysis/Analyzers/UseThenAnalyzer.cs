using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
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

                if (!operation.TargetMethod.OriginalDefinition.Equals(unnestMethod, SymbolEqualityComparer.Default))
                    return;

                if (operation.Arguments is not
                [
                    {
                        Value: IInvocationOperation argumentInvocation
                    }
                ]) return;

                if (!argumentInvocation.TargetMethod.OriginalDefinition
                        .Equals(mapMethod, SymbolEqualityComparer.Default))
                    return;

                var invocationSyntax = (InvocationExpressionSyntax)argumentInvocation.Syntax;
                var memberAccessSyntax = (MemberAccessExpressionSyntax)invocationSyntax.Expression;
                var location = memberAccessSyntax.Name.GetLocation();

                operationCtx.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UseThen,
                    location));
            }, OperationKind.Invocation);
        });
    }
}
