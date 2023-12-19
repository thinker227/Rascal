# Rascal
Rascal is a simple and lightweight [result type](https://www.youtube.com/watch?v=srQt1NAHYC0&t=1018s) implementation for C#, containing a variety of utilities and standard functions for working with result types and integrating them into the rest of C#.

Rascal is first and foremost an aggregate of the result types I personally find myself implementing in a majority of my own projects, and a competetor other result libraries second. As such, this library implements some things I think other result implementations are lacking, while omitting some features other libraries do implement.

## The prelude

Rascal contains a `Prelude` class (named in reference to most functional languages) which contains a wide variety of utility functions. Since this class contains functions which are used very frequently in code heavily utilizing results, this class is meant to be *imported statically*, i.e. through a `using static` statement. For convenience, this can be included as a global using in a `Usings.cs` file containing other global using statements.

## Samples

### Creating a result
```cs
// Through explicit Ok/Error functions
var explicitOk = Ok("uwu");
var explicitErr = Err<int>("An error occured.");

// Through implicit conversion
Result<string> implicitOk = "owo";
```

### Mapping a result

"Mapping" refers to taking a result containing some value of type `T` and *mapping* said value to a value of some other type `TNew`.
```cs
// Read input, parse to int, and apply a function to the value
var x = ParseR<int>(Console.ReadLine()!)
    .Map(x => Enumerable.Repeat("hewwo", x));
```

Another operation, quite similar to mapping, exists, known as a "bind". A bind operation acts like a map, but the function applied to the value of type `T` returns another result, namely a `Result<TNew>`, which is then returned from the bind. This is the fundamental mechanism which allows chaining result operations together, making for a quite powerful tool.
```cs
// Read input, parse to int, and apply a function to the value, which may fail
var num = ParseR<int>(Console.ReadLine()!);
var den = ParseR<int>(Console.ReadLine()!);

var val = num.Then(a => den
    .Map(b => DiveSafe(a, b)));

static Result<int> DivSafe(int a, int b) =>
    b != 0
        ? a / b
        : "Cannot divide by 0.";
```

The above expression for `val` can alternatively be written using query syntax:
```cs
var val =
    from a in num
    from b in den
    from x in DivSafe(a, b)
    select x;
```

### Various utilities

Parse a string or `ReadOnlySpan<char>` to another type, returning a result. `ParseR` (short for `ParseResult`) works for any type implementing `IParsable<TSelf>` or `ISpanParsable<TSelf>`.
```cs
var parsed = ParseR<int>(Console.ReadLine()!);
```

Turn a nullable value into a result.
```cs
var result = F().NotNull();

static int? F();
```

A function for running another function in a `try` block and returning a result containing either the successful value of the function or the thrown exception. Quite useful for functions which provide no good way of checking whether success is expected before running it, such as IO.

> **note** `Try` variants are also available for `Map` and `Bind`.
```cs
var result = Try(() => File.ReadAllText(path));
```

Validate inputs directly inside a result expression chain, replacing the original value with an error if the predicate fails.
```cs
var input = ParseR<int>(Console.ReadLine()!)
    .ErrorIf(
        x => x < 0,
        x => $"Input {x} is less than 0.")
```

### "Unsafe" operations

To not be *too* far out-of-line with the rest of C#, there are also functions for accessing the values inside results in an unsafe manner. "Unsafe" in this context is not referring to the `unsafe` from C#, but rather the fact these functions may throw exceptions, as opposed to most other functions which are pure and should not normally throw exceptions. These functions should be treated with care and only be used in situations where the caller knows without a reasonable shadow of a doubt that the operation is safe or an exception is acceptable to be thrown.
```cs
Result<int> result;

int x = result.Unwrap();
// or
int y = (int)result;

int z = result.Expect("Expected result to be successful.");
```
