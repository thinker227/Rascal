using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Rascal.Analysis.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class RemoveMapIdCallCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        Diagnostics.UnnecessaryIdMap.Id);
    
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext ctx)
    {
        var document = ctx.Document;
        
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
        
        var codeAction = CodeAction.Create(
            "Remove Map call",
            _ => Task.FromResult(ExecuteFix(
                document,
                root,
                invocation,
                invocationTarget)),
            nameof(RemoveMapIdCallCodeFix));
        
        ctx.RegisterCodeFix(codeAction, ctx.Diagnostics);
    }

    private static Document ExecuteFix(
        Document document,
        SyntaxNode root,
        InvocationExpressionSyntax invocation,
        ExpressionSyntax invocationTarget)
    {
        var newRoot = root.ReplaceNode(invocation, invocationTarget);
        return document.WithSyntaxRoot(newRoot);
    }
}
