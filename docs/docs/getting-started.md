# Getting started

A quick setup guide for using Rascal.

## Installing the package

In order to use Rascal, you should first install the [Rascal Nuget package](https://www.nuget.org/packages/Rascal).

# [.NET CLI](#tab/cli)

Run in a terminal in the root of your project:

```ps1
dotnet add package Rascal --prerelease
```

# [Package manager console](#tab/pm)

Run from the Visual Studio Package Manager console:

```ps1
NuGet\Install-Package Rascal -IncludePrerelease
```

# [REPL environment](#tab/repl)

In environments such as [C# REPL](https://github.com/waf/CSharpRepl) or [F# Interactive](https://learn.microsoft.com/en-us/dotnet/fsharp/tools/fsharp-interactive/), enter:

```cs
#r "nuget: Rascal"
```

If you wish to install a pre-release version of the package, specify the package version:

```cs
#r "nuget: Rascal, 1.0.1-pre"
```

For the special flavor of C# script mode used by the [MODiX REPL](https://github.com/discord-csharp/CSharpRepl):

```cs
#nuget Rascal
```

> [!NOTE]
> The MODiX REPL currently doesn't support referencing specific package versions and therefore does not support pre-release versions of the package.

# [`PackageReference`](#tab/csproj)

Add under an `ItemGroup` node in the `.csproj` file of your project:

```xml
<PackageReference Include="Rascal" Version="1.0.1-pre" />
```

---

## Using the package

Once you've installed the package, import the `Rascal` namespace containing the import types for Rascal:

```cs
using Rascal;
```

You can now access the [`Result<T>`](/api/Rascal.Result-1.html) type and fail away to your heart's content. See [Using `Result<T>`](/docs/using-result.html) for a more detailed introduction to the `Result<T>` type.

### Importing the prelude

Rascal features a [`Prelude`](/api/Rascal.Prelude.html) class which contains various utility methods which are meant to be imported *statically*. Create a new file named `Usings.cs` and add the following:

```cs
global using static Rascal.Prelude;
```

This imports `Prelude` *globally* and *statically* such that all its methods are available everywhere within your project without having to explicitly reference the `Prelude` class.

```cs
var result = Ok("some value");
var parsed = ParseR<int>(Console.ReadLine()!);
```
