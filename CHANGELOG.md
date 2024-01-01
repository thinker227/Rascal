# Version `1.1.0-pre`

- Rewrite and update a lot of documentation.

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
