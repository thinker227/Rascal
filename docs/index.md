---
_layout: landing
---

# Rascal

Rascal is a simple and lightweight [result type](https://www.youtube.com/watch?v=srQt1NAHYC0&t=1018s) implementation for C#, containing a variety of utilities and standard functions for working with result types and integrating them into the rest of C#.

Rascal is first and foremost an aggregate of the result types I personally find myself implementing in a majority of my own projects, and a competetor other result libraries second. As such, this library implements some things I think other result implementations are lacking, while omitting some features other libraries do implement.

<br/>

## Installation

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

# [Script environment](#tab/repl)

In environments such as [C# REPL](https://github.com/waf/CSharpRepl) or [RoslynPad](https://roslynpad.net), enter:

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

Add under an `ItemGroup` node in your project file:

```xml
<PackageReference Include="Rascal" Version="1.0.1-pre" />
```

Obviously, replace `1.0.1-pre` with the actual package version you want.

---

<br/>

## Quick start

After installing the package, create a file called `Usings.cs` and add the following:

```cs
global using static Rascal.Prelude;
```

[`Prelude`](/api/Rascal.Prelude.html) includes a variety of utility functions which you can now access from anywhere in your project.

Now, let's pretend you have an ASP.NET Core app with the following method in a service:

```cs
public Task<User?> GetUser(int userId)
{
    var user = db.Users.FirstOrDefaultAsync(user => user.Id == userId);

    return user;
}
```

... you can replace it with:

```cs
public Task<Result<User>> GetUser(int userId) => TryAsync(() =>
{
    var user = db.Users.FirstOrDefaultAsync(user => user.Id == userId);

    if (user is null) return new NotFoundError($"User with ID {userId} does not exist.");

    return user;
});
```

This code will handle catching any exceptions thrown by [`FirstOrDefaultAsync`](https://learn.microsoft.com/en-us/dotnet/api/system.data.entity.queryableextensions.firstordefaultasync) and will return a [`NotFoundError`](/api/Rascal.Errors.NotFoundError.html) if the user isn't found. Now you can use this method as such:

# [Minimal API](#tab/minimal)

```cs
app.MapGet("/users/{id}", async (int id, IUserService userService) =>
{
    var userResult = await userService.GetUser(id);

    return userResult.Match(
        user => Results.Ok(user),
        error => error switch
        {
            NotFoundError => Results.NotFound(),
            _ => Results.Problem(detail: error.Message, statusCode: 500)
        }
    );
});
```

# [Controller](#tab/controller)

```cs
public sealed class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet("/users/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var user = await userService.GetUser(id);

        return user.Match(
            user => this.Ok(user),
            IActionResult (error) => error switch
            {
                NotFoundError => this.NotFound(),
                _ => this.Problem(detail: error.Message, statusCode: 500)
            }
        );
    }
}
```

---

<br/>

## Other great libraries

Some libraries Rascal takes inspiration from:

- Rust's [std::result](https://doc.rust-lang.org/std/result)
- Haskell's [Data.Maybe](https://hackage.haskell.org/package/base-4.19.0.0/docs/Data-Maybe.html) and [Control.Monad](https://hackage.haskell.org/package/base-4.19.0.0/docs/Control-Monad.html)
- [Remora.Results](https://github.com/Remora/Remora.Results)
- [Pidgin](https://github.com/benjamin-hodgson/Pidgin)
- [SuperLinq](https://github.com/viceroypenguin/SuperLinq)
- [HonkSharp](https://github.com/asc-community/HonkSharp)
- [error-or](https://github.com/amantinband/error-or)
