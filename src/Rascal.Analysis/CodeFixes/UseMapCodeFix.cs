using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Operations;

namespace Rascal.Analysis.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class UseMapCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        Diagnostics.UseMap.Id);

    public override FixAllProvider? GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext ctx)
    {
        var document = ctx.Document;
        var semanticModel = await document.GetSemanticModelAsync(ctx.CancellationToken);
        if (semanticModel is null) return;
        
        var root = await document.GetSyntaxRootAsync(ctx.CancellationToken);
        if (root is null) return;
        
        var invocation = root.FindNode(ctx.Span).FirstAncestorOrSelf<InvocationExpressionSyntax>();
        if (invocation is not
            {
                Expression: MemberAccessExpressionSyntax
                {
                    Expression: var invocationTarget
                }
            }) return;

        var invocationOperation = (IInvocationOperation)semanticModel.GetOperation(invocation)!;

        if (GetReturnExpression(invocationOperation) is not var (argumentExpression, returnExpression, subExpression)) return;

        var codeAction = CodeAction.Create(
            "Use Map",
            _ => Task.FromResult(ExecuteFix(
                document,
                root,
                invocation,
                invocationTarget,
                argumentExpression,
                returnExpression,
                subExpression)),
            nameof(UseMapCodeFix));
        
        ctx.RegisterCodeFix(codeAction, ctx.Diagnostics);
    }

    private static (ExpressionSyntax, ExpressionSyntax, ExpressionSyntax)? GetReturnExpression(IInvocationOperation invocation)
    {
        if (invocation.Arguments is not
            [
                {
                    Value: IDelegateCreationOperation
                    {
                        Target: IAnonymousFunctionOperation
                        {
                            Body.Operations:
                            [
                                IReturnOperation
                                {
                                    ReturnedValue: var returnValue
                                } returnOperation
                            ]
                        } expressionOperation
                    }
                }
            ]) return null;

        var argumentExpression = (ExpressionSyntax)expressionOperation.Syntax;
        var returnExpression = (ExpressionSyntax)returnOperation.Syntax;
        
        if (returnValue is IInvocationOperation returnInvocation)
            return (argumentExpression, returnExpression, (ExpressionSyntax)returnInvocation.Arguments[0].Value.Syntax);

        if (returnValue is IObjectCreationOperation returnObjectCreation)
            return (argumentExpression, returnExpression, (ExpressionSyntax)returnObjectCreation.Arguments[0].Value.Syntax);

        if (returnValue is IConversionOperation conversion)
        {
            if (conversion is { IsImplicit: true, Operand: IObjectCreationOperation targetTypeNew })
                return (argumentExpression, returnExpression, (ExpressionSyntax)targetTypeNew.Arguments[0].Value.Syntax);

            return (argumentExpression, returnExpression, (ExpressionSyntax)conversion.Operand.Syntax);
        }

        return null;
    }

    private static Document ExecuteFix(Document document,
        SyntaxNode root,
        InvocationExpressionSyntax invocation,
        ExpressionSyntax invocationTarget,
        ExpressionSyntax argumentExpression,
        ExpressionSyntax returnExpression,
        ExpressionSyntax subExpression)
    {
        var newArgumentExpression = argumentExpression.ReplaceNode(returnExpression, subExpression);
        var newInvocation = SyntaxInator.MapFrom(invocationTarget, newArgumentExpression);

        var newRoot = root.ReplaceNode(invocation, newInvocation);
        return document.WithSyntaxRoot(newRoot);
    }
}
