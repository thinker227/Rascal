<div align="center">
    <h1>Rascal</h1>
    <img alt="Build and test status" src="https://img.shields.io/github/actions/workflow/status/thinker227/Rascal/build-test.yml?style=for-the-badge&label=Build%20%26%20Test">
    <img alt="Nuget" src="https://img.shields.io/nuget/vpre/Rascal?style=for-the-badge&label=Nuget%3A%20Rascal">
</div>

<br></br>

Rascal is a simple yet powerful [result type](https://www.youtube.com/watch?v=srQt1NAHYC0&t=1018s) implementation for C#, containing a variety of utilities and standard functions for working with result types and integrating them into the rest of C#.

Rascal is first and foremost an aggregate of the result types I personally find myself implementing in a majority of my own projects, and a competetor other result libraries second. As such, this library implements some things I think other result implementations are lacking, while omitting some features other libraries do implement.

**TODO**: Link to the Github Pages docfx documentation.

<br/>

# Installation

<details>
<summary>.NET CLI</summary>

Run in a terminal in the root of your project:

```ps1
dotnet add package Rascal --prerelease
```

</details>

<details>
<summary>Package manager console</summary>

Run from the Visual Studio Package Manager console:

```ps1
NuGet\Install-Package Rascal -IncludePrerelease
```

</details>

<details>
<summary>Script environment</summary>

In environments such as [C# REPL](https://github.com/waf/CSharpRepl) or [RoslynPad](https://roslynpad.net), enter:

```cs
#r "nuget: Rascal"
```

If you wish to install a specific version of the package, specify the package version:

```cs
#r "nuget: Rascal, 1.0.1-pre"
```

</details>

<details>
<summary><code>PackageReference</code></summary>

Add under an `ItemGroup` node in your project file:

```xml
<PackageReference Include="Rascal" Version="1.0.1-pre" />
```

Obviously, replace `1.0.1-pre` with the actual package version you want.

</details>

<br/>

# Samples

## Creating results

<!-- Github annoyingly does not allow embedding code snippets from other files in markdown, so this entire section is copy-pasted from ./docs/samples/index.md and the ./samples/ folder. REMEMBER TO UPDATE THIS WHEN EDITING THE CORRESPONDING SAMPLES. -->

Results in Rascal can be created in a variety of ways, the two most common of which are through the `Ok` and `Err` methods defined in the prelude, or through implicitly converting ok values or errors into results.

```cs
// You can create a result either through explicit Ok/Error functions...
var explicitOk = Ok(new Person("Melody", 27));
var explicitError = Err<Person>("Could not find person");

// ... or through implicit conversions...
Result<Person> implicitOk = new Person("Edwin", 32);
Result<Person> implicitError = new StringError("Failed to find person");
```

## Mapping

"Mapping" refers to taking a result containing some value some type (`T`) and *mapping* said value to a new value of some other type (`TNew`). The principal method of mapping is the aptly named `Map`.

```cs
var name = "Raymond";

// Read console input and try parse it into an int.
// If the input cannot be parsed, the result will be an error.
var age = ParseR<int>(Console.ReadLine()!);

// Map the age to a new person.
// If the age is an error, the person will also be an error.
var person = age.Map(x => new Person(name, x));
```

<br/>

Another operation, commonly referred to as "bind" or "chaining", exists, which looks quite similar to mapping, the only difference being that the lambda you supply to the method returns a *new* result rather than a plain value. The principal method of chaining is `Then`, which can be read as "a, then b, then c".

```cs
// Read console input and assert that it isn't null.
// If the input is null, the value will be an error.
var name = Console.ReadLine().NotNull();

// Chain an operation on the name which will only execute if the name is ok.
var person = name.Then(n =>
{
    // Read console input, assert that it isn't null, then try parse it into an int.
    var age = Console.ReadLine().NotNull()
        .Then(str => ParseR<int>(str));

    // Map the age into a new person.
    return age.Map(a => new Person(n, a));
});
```

<br/>

`Map` and `Then` together make up the core of the `Result<T>` type, allowing for chaining multiple operations on a single result. In functional terms, these are what makes `Result<T>` a functor and monad respectively (although not an applicative).

### Combine

`Combine` is an addition to `Map` and `Then` which streamlines the specific case where you have two results and want to *combine* them into a single result only if both results are ok.

```cs
// Read console input and assert that it isn't null.
var name = Console.ReadLine().NotNull();

// Read console input, assert that it isn't null, then try parse it into an int.
var age = Console.ReadLine().NotNull()
    .Then(str => ParseR<int>(str));

// Combine the name and age results together, then map them into a person.
var person = name.Combine(age)
    .Map(v => new Person(v.first, v.second));
```

## Validation

Rascal supports a simple way of validating the value of a result, returning an error in case the validation fails.

```cs
// Read console input, assert that it isn't null, and validate that it matches the regex.
var name = Console.ReadLine().NotNull()
    .Validate(
        str => Regex.IsMatch(str, "[A-Z][a-z]*"),
        _ => "Name can only contain characters a-z and has to start with a capital letter.");

var person = name.Then(n =>
{
    // Read console input, assert that it isn't null, try parse it into an int, then validate that it is greater than 0.
    var age = Console.ReadLine().NotNull()
        .Then(str => ParseR<int>(str))
        .Validate(
            x => x > 0,
            _ => "Age has to be greater than 0.");

    return age.Map(a => new Person(n, a));
});
```

## Exception handling

One of the major kinks of adapting C# into a more functional style (such as using results) is the already existing standard of using exceptions for error-handling. Exceptions have *many* flaws, and result types explicitly exist to provide a better alternative to exceptions, but Rascal nontheless provides a way to interoperate with traditional exception-based error handling.

The `Try` method in the prelude is the premiere exception-handling method, which runs another function inside a `try`-`catch` block, and returns an `ExceptionError` in case an exception is thrown.

```cs
// Try read console input and use the input to read the specified file.
// If an exception is thrown, the exception will be returned as an error.
var text = Try(() =>
{
    var path = Console.ReadLine()!;
    return File.ReadAllText(path);
});
```

`Try` variants also exist for `Map` and `Then`, namely `TryMap` and `ThenTry`.

```cs
// Read console input and assert that it isn't null.
var path = Console.ReadLine().NotNull();

// Try to map the input by reading a file specified by the input.
// If ReadAllText throws an exception, the exception will be returned as an error.
var text = path.TryMap(p => File.ReadAllText(p));
```
