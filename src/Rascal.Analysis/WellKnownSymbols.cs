using System.Diagnostics.CodeAnalysis;

namespace Rascal.Analysis;

public sealed record WellKnownSymbols(
    INamedTypeSymbol ObjectType,
    INamedTypeSymbol Result1Type,
    ITypeParameterSymbol TypeParameterT,
    INamedTypeSymbol ResultExtensionsType,
    INamedTypeSymbol PreludeType,
    IMethodSymbol MapMethod,
    IMethodSymbol ThenMethod,
    IMethodSymbol ToMethod,
    IMethodSymbol MatchMethod,
    IMethodSymbol UnnestMethod,
    IMethodSymbol OkMethod,
    IMethodSymbol OkCtor,
    IMethodSymbol OkConversion)
{
    private const string Result1TypeName = "Rascal.Result`1";
    private const string ResultExtensionsTypeName = "Rascal.ResultExtensions";
    private const string PreludeTypeName = "Rascal.Prelude";
    
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
            if (type is null) es.Add(new(metadataName));
            return type;
        }

        var resultType = GetType(Result1TypeName);
        var resultExtensionsType = GetType(ResultExtensionsTypeName);
        var preludeType = GetType(PreludeTypeName);

        if (es is not [])
        {
            symbols = null;
            errors = es;
            return false;
        }

        var typeParameterT = resultType!.TypeParameters[0];

        Func<string, ImmutableArray<ISymbol>> GetMember(ITypeSymbol type, string typeMetadataName) => name =>
        {
            var members = type.GetMembers(name);
            if (members is []) es.Add(new($"{typeMetadataName}.{name}"));
            return members;
        };

        var getResultMember = GetMember(resultType!, Result1TypeName);
        var getResultExtensionsMember = GetMember(resultExtensionsType!, ResultExtensionsTypeName);
        var getPreludeMember = GetMember(preludeType!, PreludeTypeName);

        IMethodSymbol? GetSingleResultMethod(string name) =>
            (IMethodSymbol?)getResultMember!(name).FirstOrDefault();

        IMethodSymbol? GetSinglePreludeMethod(string name) =>
            (IMethodSymbol?)getPreludeMember!(name).FirstOrDefault();

        var mapMethod = GetSingleResultMethod("Map");
        var thenMethod = GetSingleResultMethod("Then");
        var toMethod = GetSingleResultMethod("To");
        var unnestMethod = (IMethodSymbol?)getResultExtensionsMember("Unnest").FirstOrDefault();
        var matchMethod = GetSingleResultMethod("Match");
        var okMethod = GetSinglePreludeMethod("Ok");
        var okCtor = resultType!.InstanceConstructors
            .FirstOrDefault(ctor =>
                ctor.Parameters is [{ Type: ITypeParameterSymbol t }] &&
                t.Equals(typeParameterT, SymbolEqualityComparer.Default));
        var okConversion = resultType.GetMembers("op_Implicit")
            .OfType<IMethodSymbol>()
            .FirstOrDefault(method =>
                method.Parameters is [{ Type: ITypeParameterSymbol t }] &&
                t.Equals(typeParameterT, SymbolEqualityComparer.Default));

        if (es is not [])
        {
            symbols = null;
            errors = es;
            return false;
        }

        symbols = new(
            objectType,
            resultType!,
            typeParameterT!,
            resultExtensionsType!,
            preludeType!,
            mapMethod!,
            thenMethod!,
            toMethod!,
            matchMethod!,
            unnestMethod!,
            okMethod!,
            okCtor!,
            okConversion!);
        errors = null;
        return true;
    }

    public readonly record struct Error(string MemberName);
}
