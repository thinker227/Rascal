using Microsoft.CodeAnalysis;

namespace Rascal.Analysis;

public static class SymbolExtensions
{
    public static bool Inherits(this ITypeSymbol x, ITypeSymbol type)
    {
        var t = x;

        while (t is not null)
        {
            if (t.Equals(type, SymbolEqualityComparer.Default)) return true;
            t = t.BaseType;
        }

        return false;
    }
}
