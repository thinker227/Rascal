using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Rascal.Analysis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseMapAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Diagnostics.UseMap);
    
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterCompilationStartAction(compilationCtx =>
        {
            var resultType = compilationCtx.Compilation.GetTypeByMetadataName("Rascal.Result`1");
            if (resultType is null) return;

            var preludeType = compilationCtx.Compilation.GetTypeByMetadataName("Rascal.Prelude");
            if (preludeType is null) return;

            var resultMembers = resultType.GetMembers();
            var preludeMembers = preludeType.GetMembers();

            var thenMethod = (IMethodSymbol)resultMembers.First(x => x.Name == "Then");
            var okMethod = (IMethodSymbol)preludeMembers.First(x => x.Name == "Ok");
            var okCtor = resultType.InstanceConstructors
                .First(x => x.Parameters is [{ Type: ITypeParameterSymbol }]);
            
            compilationCtx.RegisterOperationAction(operationCtx =>
            {
                var operation = (IInvocationOperation)operationCtx.Operation;

                // Check that it is Then being called.
                if (!operation.TargetMethod.OriginalDefinition.Equals(thenMethod, SymbolEqualityComparer.Default)) return;

                // Check that the first argument is a lambda.
                if (operation.Arguments is not
                [
                    {
                        Value: IDelegateCreationOperation
                        {
                            Target: IAnonymousFunctionOperation
                            {
                                Body: var body
                            }
                        }
                    }
                ]) return;

                // Check that the body is a single return operation.
                if (body.Operations is not [IReturnOperation returnOperation]) return;

                // Check that the returned expression is an invocation.
                if (returnOperation.ReturnedValue is not IInvocationOperation returnInvocation) return;

                // Check that the return invocation expression is calling either Ok or new(T).
                if (!returnInvocation.TargetMethod.OriginalDefinition.Equals(okMethod, SymbolEqualityComparer.Default) &&
                    !returnInvocation.TargetMethod.OriginalDefinition.Equals(okCtor, SymbolEqualityComparer.Default)) return;

                // Get the syntax of the method name.
                var invocationSyntax = (InvocationExpressionSyntax)operation.Syntax;
                var memberAccess = (MemberAccessExpressionSyntax)invocationSyntax.Expression;
                var location = memberAccess.Name.GetLocation();
                
                // Report the diagnostic.
                operationCtx.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.UseMap,
                    location));
            }, OperationKind.Invocation);
        });
    }
}
