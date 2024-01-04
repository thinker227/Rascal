using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Rascal.Analysis;

// Use https://roslynquoter.azurewebsites.net/ for generating syntax code.

internal static class SyntaxInator
{
    public static InvocationExpressionSyntax GetValueOrConstant(
        ExpressionSyntax invocationTarget,
        ExpressionSyntax value) =>
        InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    invocationTarget,
                    IdentifierName("GetValueOr")))
            .WithArgumentList(
                ArgumentList(
                    SingletonSeparatedList(
                        Argument(value))))
            .NormalizeWhitespace();

    public static InvocationExpressionSyntax GetValueOrDiscardLambda(
        ExpressionSyntax invocationTarget,
        LambdaExpressionSyntax lambda) =>
        InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    invocationTarget,
                    IdentifierName("GetValueOr")))
            .WithArgumentList(
                ArgumentList(
                    SingletonSeparatedList(
                        Argument(
                            ParenthesizedLambdaExpression(
                                    lambda.AsyncKeyword,
                                    ParameterList(),
                                    lambda.ArrowToken,
                                    lambda.Body)
                                .WithAttributeLists(lambda.AttributeLists)
                                .WithModifiers(lambda.Modifiers)))))
            .NormalizeWhitespace();

    public static InvocationExpressionSyntax GetValueOrParameterizedLambda(
        ExpressionSyntax invocationTarget,
        LambdaExpressionSyntax lambda) =>
        InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    invocationTarget,
                    IdentifierName("GetValueOr")))
            .WithArgumentList(
                ArgumentList(
                    SingletonSeparatedList(
                        Argument(lambda))))
            .NormalizeWhitespace();
}
