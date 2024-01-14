This article contains a variety of code samples demonstrating common usages for various parts of the Rascal library. All the samples source code can be found [here](https://github.com/thinker227/Rascal/tree/main/samples).

<br/>

## Creating results

Results in Rascal can be created in a variety of ways, the two most common of which are through the [`Ok`](/api/Rascal.Prelude.html#Rascal_Prelude_Ok__1___0_) and [`Err`](/api/Rascal.Prelude.html#Rascal_Prelude_Err__1_Rascal_Error_) methods defined in the prelude, or through implicitly converting ok values or errors into results.

[!code-csharp[](../../samples/Construction.csx#L7-L13)]

<br/>

## Mapping

"Mapping" refers to taking a result containing some value some type (`T`) and *mapping* said value to a new value of some other type (`TNew`). The principal method of mapping is the aptly named [`Map`](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__).

[!code-csharp[](../../samples/Map.csx#L6-L14)]

<br/>

Another operation, commonly referred to as "bind" or "chaining", exists, which looks quite similar to mapping, the only difference being that the lambda you supply to the method returns a *new* result rather than a plain value. The principal method of chaining is [`Then`](/api/Rascal.Result-1.html#Rascal_Result_1_Then__1_System_Func__0_Rascal_Result___0___), which can be read as "a, then b, then c".

[!code-csharp[](../../samples/Then.csx#L6-L19)]

<br/>

[`Map`](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__) and [`Then`](/api/Rascal.Result-1.html#Rascal_Result_1_Then__1_System_Func__0_Rascal_Result___0___) together make up the core of the [`Result<T>`](/api/Rascal.Result-1.html) type, allowing for chaining multiple operations on a single result. In functional terms, these are what makes [`Result<T>`](/api/Rascal.Result-1.html) a functor and monad respectively (although not an applicative).

<br/>

> [!TIP]
> The aliases [`Select`](/api/Rascal.Result-1.html#Rascal_Result_1_Select__1_System_Func__0___0__) and [`SelectMany`](/api/Rascal.Result-1.html#Rascal_Result_1_SelectMany__1_System_Func__0_Rascal_Result___0___) are available for [`Map`](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__) and [`Then`](/api/Rascal.Result-1.html#Rascal_Result_1_Then__1_System_Func__0_Rascal_Result___0___) respectively. These exist to supply support for *query expressions* as an alternative to method chaining. Query syntax can in specific situations be more readable than the method chaining alternative, although in *most* scenarios, method chaning is better. [`Select`](/api/Rascal.Result-1.html#Rascal_Result_1_Select__1_System_Func__0___0__) and [`SelectMany`](/api/Rascal.Result-1.html#Rascal_Result_1_SelectMany__1_System_Func__0_Rascal_Result___0___) should ***not*** be used outside query syntax.

<br/>

### Combine

[`Combine`](/api/Rascal.Result-1.html#Rascal_Result_1_Combine__1_Rascal_Result___0__) is an addition to [`Map`](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__) and [`Then`](/api/Rascal.Result-1.html#Rascal_Result_1_Then__1_System_Func__0_Rascal_Result___0___) which streamlines the specific case where you have two results and want to *combine* them into a single result only if both results are ok.

[!code-csharp[](../../samples/Combine.csx#L6-L15)]

<br/>

## Validation

Rascal supports a simple way of validating the value of a result, returning an error in case the validation fails.

[!code-csharp[](../../samples/Validation.csx#L8-L24)]

<br/>

## Exception handling

One of the major kinks of adapting C# into a more functional style (such as using results) is the already existing standard of using exceptions for error-handling. Exceptions have *many* flaws, and result types explicitly exist to provide a better alternative to exceptions, but Rascal nontheless provides a way to interoperate with traditional exception-based error handling.

The [`Try`](/api/Rascal.Prelude.html#Rascal_Prelude_Try__1_System_Func___0__) method in the prelude is the premiere exception-handling method, which runs another function inside a `try`-`catch` block, and returns an [`ExceptionError`](/api/Rascal.Errors.ExceptionError.html) in case an exception is thrown.

[!code-csharp[](../../samples/Try.csx#L6-L11)]

<br/>

`Try` variants also exist for [`Map`](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__) and [`Then`](/api/Rascal.Result-1.html#Rascal_Result_1_Then__1_System_Func__0_Rascal_Result___0___), namely [`TryMap`](/api/Rascal.Result-1.html#Rascal_Result_1_TryMap__1_System_Func__0___0__) and [`ThenTry`](/api/Rascal.Result-1.html#Rascal_Result_1_ThenTry__1_System_Func__0_Rascal_Result___0___).

[!code-csharp[](../../samples/TryMap.csx#L6-L11)]
