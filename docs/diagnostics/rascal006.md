# Use 'GetValueOr' instead of 'Match(x => x, ...)'

Id: *RASCAL006*

Severity: *warning*

> [!TIP]
> This diagnostic has an associated automatic code fix.

<br/>

## Description

*RASCAL006* is reported when [`Match`](/api/Rascal.Result-1.html#Rascal_Result_1_Match__1_System_Func__0___0__System_Func_Rascal_Error___0__) is called with an *identity function* as its first argument, i.e. a lambda with the form `x => x`. Such a call is equivalent to [`GetValueOr`](/api/Rascal.Result-1.html#Rascal_Result_1_GetValueOr_System_Func_Rascal_Error__0__), with the remaining argument being a function which returns a value based on the result's error. Note that [`GetValueOr`](/api/Rascal.Result-1.html#Rascal_Result_1_GetValueOr_System_Func_Rascal_Error__0__) has three different overloads which are suitable depending on whether the result's error is needed when retrieving the value. The warning can be resolved by removing the first argument and replacing the call to [`Match`](/api/Rascal.Result-1.html#Rascal_Result_1_Match__1_System_Func__0___0__System_Func_Rascal_Error___0__) with [`GetValueOr`](/api/Rascal.Result-1.html#Rascal_Result_1_GetValueOr_System_Func_Rascal_Error__0__).

<br/>

### Example

```cs
var a = Ok(2);

var b = a.Match(x => x, e => 0); // RASCAL006

// Fix:
var b = a.DefaultOr(0);
```

```cs
var a = Ok(2);

var b = a.Match(x => x, e => F(e)); // RASCAL006

// Fix:
var b = a.DefaultOr(e => F(e));
```
