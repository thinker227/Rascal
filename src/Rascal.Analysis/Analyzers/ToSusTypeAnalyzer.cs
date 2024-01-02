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

                // Check that the target method is To
                var method = operation.TargetMethod;
                if (!method.OriginalDefinition.Equals(toMethod, SymbolEqualityComparer.Default)) return;

                // Get the location of the type argument
                if (operation.Syntax is not InvocationExpressionSyntax
                {
                    Expression: MemberAccessExpressionSyntax
                    {
                        Name: GenericNameSyntax
                        {
                            TypeArgumentList.Arguments: [var typeSyntax]
                        }
                    }
                }) return;
                var location = typeSyntax.GetLocation();

                // Get the source and target type for the conversion
                var sourceType = method.ContainingType.TypeArguments[0];
                var targetType = method.TypeArguments[0];

                // Report diagnostic if both source and target types are the same
                if (sourceType.Equals(targetType, SymbolEqualityComparer.Default))
                {
                    operationCtx.ReportDiagnostic(Diagnostic.Create(
                        Diagnostics.ToSameType,
                        location,
                        sourceType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
                    return;
                }

                // Return early if the types are compatible
                switch (sourceType.TypeKind, targetType.TypeKind)
                {
                // One of the types is an interface
                case (TypeKind.Interface, _) or (_, TypeKind.Interface): return;
                // One of the types is a type parameter
                case (TypeKind.TypeParameter, _) or (_, TypeKind.TypeParameter): return;
                // One of the types is object
                case (_, _) when
                    sourceType.Equals(objectType, SymbolEqualityComparer.Default) ||
                    targetType.Equals(objectType, SymbolEqualityComparer.Default): return;
                // Both types are classes and one inherits the other
                case (TypeKind.Class, TypeKind.Class) when
                    sourceType.Inherits(targetType) ||
                    targetType.Inherits(sourceType): return;
                }

                // The types are sus
                operationCtx.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.ToImpossibleType,
                    location,
                    sourceType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                    targetType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
            }, OperationKind.Invocation);
        });
    }
}
