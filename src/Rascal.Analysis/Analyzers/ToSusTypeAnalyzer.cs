using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Rascal.Analysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ToSusTypeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Diagnostics.ToSameType,
        Diagnostics.ToImpossibleType);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterCompilationStartAction(compilationCtx =>
        {
            var resultType = compilationCtx.Compilation.GetTypeByMetadataName("Rascal.Result`1");
            if (resultType is null) return;
            
            var resultMembers = resultType.GetMembers();
            
            var toMethod = (IMethodSymbol)resultMembers.First(x => x.Name == "To");

            var objectType = compilationCtx.Compilation.GetSpecialType(SpecialType.System_Object);
            
            compilationCtx.RegisterOperationAction(operationCtx =>
            {
                var operation = (IInvocationOperation)operationCtx.Operation;

                var method = operation.TargetMethod;
                if (!method.OriginalDefinition.Equals(toMethod, SymbolEqualityComparer.Default)) return;

                var invocationSyntax = (InvocationExpressionSyntax)operation.Syntax;
                var nameSyntax = (GenericNameSyntax)invocationSyntax.Expression;
                var typeSyntax = nameSyntax.TypeArgumentList.Arguments[0];
                var location = typeSyntax.GetLocation();

                var sourceType = method.ContainingType;
                var targetType = method.TypeArguments[0];

                if (sourceType.Equals(targetType, SymbolEqualityComparer.Default))
                {
                    operationCtx.ReportDiagnostic(Diagnostic.Create(
                        Diagnostics.ToSameType,
                        location,
                        sourceType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
                    return;
                }

                switch (sourceType.TypeKind, targetType.TypeKind)
                {
                case (_, _)
                    when sourceType.Equals(objectType, SymbolEqualityComparer.Default) ||
                         targetType.Equals(objectType, SymbolEqualityComparer.Default): return;
                case (TypeKind.Interface, _) or (_, TypeKind.Interface): return;
                // case (TypeKind.Class, TypeKind.Class) when // inherits
                }
            }, OperationKind.Invocation);
        });
    }
    
    private static bool Inherits(INamedTypeSymbol type, INamedTypeSymbol inherits)
}
