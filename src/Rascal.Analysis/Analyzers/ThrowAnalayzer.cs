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

            compilationCtx.RegisterSymbolStartAction(symbolCtx =>
            {
                var method = (IMethodSymbol)symbolCtx.Symbol;

                var returnType = method.ReturnType;
                if (!returnType.OriginalDefinition.Equals(resultType, SymbolEqualityComparer.Default)) return;

                symbolCtx.RegisterOperationAction(operationCtx =>
                {
                    var operation = (IThrowOperation)operationCtx.Operation;
                    
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
                        method.Name,
                        returnType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
                }, OperationKind.Throw);
            }, SymbolKind.Method);
        });
    }
}
