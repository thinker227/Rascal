using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Rascal.Analysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ThrowAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        Diagnostics.DoNotThrow);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationCtx =>
        {
            var resultType = compilationCtx.Compilation.GetTypeByMetadataName("Rascal.Result`1");
            if (resultType is null) return;

            compilationCtx.RegisterOperationAction(operationCtx =>
            {
                var operation = (IThrowOperation)operationCtx.Operation;
                
                ISymbol? symbol = GetParentMethodLikeOperation(operation) switch
                {
                    IMethodBodyBaseOperation => operationCtx.ContainingSymbol switch
                    {
                        IMethodSymbol x => x,
                        IPropertySymbol x => x,
                        _ => null,
                    },
                    ILocalFunctionOperation x => x.Symbol,
                    IAnonymousFunctionOperation x => x.Symbol,
                    _ => null,
                };

                if (symbol is null) return;

                var returnType = symbol switch
                {
                    IMethodSymbol x => x.ReturnType,
                    IPropertySymbol x => x.Type,
                    _ => throw new InvalidOperationException(),
                };

                if (!returnType.OriginalDefinition.Equals(resultType, SymbolEqualityComparer.Default)) return;

                var location = operation.Syntax switch
                {
                    ThrowStatementSyntax x => x.ThrowKeyword.GetLocation(),
                    ThrowExpressionSyntax x => x.ThrowKeyword.GetLocation(),
                    _ => throw new InvalidOperationException(
                        "Syntax of throw operation is not a throw statement syntax or throw expression syntax."),
                };

                operationCtx.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.DoNotThrow,
                    location,
                    symbol.Name,
                    returnType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
            }, OperationKind.Throw);
        });
    }

    private static IOperation? GetParentMethodLikeOperation(IOperation operation)
    {
        var current = operation.Parent;

        while (current is not null)
        {
            if (current is IMethodBodyBaseOperation or ILocalFunctionOperation or IAnonymousFunctionOperation)
                return current;
            
            current = current.Parent;
        }

        return null;
    }

    private static TOperation? GetParentOperation<TOperation>(IOperation op)
        where TOperation : class, IOperation
    {
        var current = op;

        while (current is not null)
        {
            if (current is TOperation x) return x;
            current = current.Parent;
        }

        return null;
    }

    private static IMethodSymbol? GetSymbolOfMethodOperation(IMethodBodyBaseOperation operation) =>
        operation.SemanticModel?.GetDeclaredSymbol(operation.Syntax) as IMethodSymbol;
}
