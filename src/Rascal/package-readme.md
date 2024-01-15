Rascal is a simple yet powerful [result type](https://www.youtube.com/watch?v=srQt1NAHYC0&t=1018s) implementation for C#, containing a variety of utilities and standard functions for working with result types and integrating them into the rest of C#.

Rascal is first and foremost an aggregate of the result types I personally find myself implementing in a majority of my own projects, and a competetor other result libraries second. As such, this library implements some things I think other result implementations are lacking, while omitting some features other libraries do implement.

The full library documentation is available [here](https://thinker227.github.io/Rascal/).



## Changelog

### API changes

- Rename `Result<T>.HasValue` to `IsOk`.

- Add `Result<T>.IsError`.

- Move error types into `Rascal.Errors` namespace.

- Add implicit conversion from `Error` to `Result<T>`.
  
- Add implicit conversion from `Exception` to `Error`.

- Add `object? key` parameter and `object? Key` property to `NotFoundError`.

- Add async version of `Prelude.Try`.

- Add async versions of `Result<T>.Match` and `Result<T>.Switch`.

- Rename `Result<T>.ToType` to `To` and allow attempting to convert to non-derived types.

- Add `ToString` extensions supporting `IFormattable`.

- Add `ResultEqualityComparer<T>`.

- Add additional overloads for `Result<T>.Equals` which allow specifying an equality comparer to use for comparing values.

- Rename `GetValueOrDefault(T @default)` and `GetValueOrDefault(Func<T> getDefault)` to `GetValueOr`.

- Add `Parse` extensions for `string` and `ReadOnlySpan<char>`.

### Analyzers and diagnostics

- Add analyzer and code-fix suite.

### Documentation

- Rewrite and update a lot of documentation.

- Create a documentation website.
