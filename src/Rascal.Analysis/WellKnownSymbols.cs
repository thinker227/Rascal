using System.Diagnostics.CodeAnalysis;

namespace Rascal.Analysis;

public sealed record WellKnownSymbols(
    INamedTypeSymbol ObjectType,
    INamedTypeSymbol Result1Type,
    INamedTypeSymbol ResultExtensionsType,
    IMethodSymbol MapMethod,
    IMethodSymbol ThenMethod,
    IMethodSymbol ToMethod,
    IMethodSymbol UnnestMethod)
{
    private const string Result1TypeName = "Rascal.Result`1";
    private const string ResultExtensionsTypeName = "Rascal.ResultExtensions";
    
    public static bool TryCreate(
        Compilation compilation,
        [NotNullWhen(true)] out WellKnownSymbols? symbols,
        [NotNullWhen(false)] out IReadOnlyCollection<Error>? errors)
    {
        var es = new List<Error>();

        var objectType = compilation.GetSpecialType(SpecialType.System_Object);
        
        INamedTypeSymbol? GetType(string metadataName)
        {
            var type = compilation.GetTypeByMetadataName(metadataName);
            if (type is null) es!.Add(new(metadataName));
            return type;
        }

        var resultType = GetType(Result1TypeName);
        var resultExtensionsType = GetType(ResultExtensionsTypeName);

        if (es is not [])
        {
            symbols = null;
            errors = es;
            return false;
        }

        Func<string, ImmutableArray<ISymbol>> GetMember(ITypeSymbol type, string typeMetadataName) => name =>
        {
            var members = type.GetMembers(name);
            if (members is []) es.Add(new($"{typeMetadataName}.{name}"));
            return members;
        };

        var getResultMember = GetMember(resultType!, Result1TypeName);
        
        ISymbol GetSingleResultMethod(string name) =>
            getResultMember(name).FirstOrDefault()!;
        
        ImmutableArray<ISymbol> GetExtensionMethod(string name) =>
            GetMember(resultExtensionsType!, ResultExtensionsTypeName)(name);

        var mapMethod = (IMethodSymbol?)GetSingleResultMethod("Map");
        var thenMethod = (IMethodSymbol?)GetSingleResultMethod("Then");
        var toMethod = (IMethodSymbol?)GetSingleResultMethod("To");
        var unnestMethod = (IMethodSymbol?)GetExtensionMethod("Unnest").FirstOrDefault();

        if (es is not [])
        {
            symbols = null;
            errors = es;
            return false;
        }

        symbols = new(
            objectType,
            resultType!,
            resultExtensionsType!,
            mapMethod!,
            thenMethod!,
            toMethod!,
            unnestMethod!);
        errors = null;
        return true;
    }

    public readonly record struct Error(string MemberName);
}
