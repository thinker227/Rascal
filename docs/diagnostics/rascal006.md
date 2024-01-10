# Use 'GetValueOr' instead of 'Match(x => x, ...)'

Id: *RASCAL006*

Severity: *warning*

> [!TIP]
> This diagnostic has an associated automatic code fix.

<br/>

## Description

Calling '[Match](/api/Rascal.Result-1.html#Rascal_Result_1_Match__1_System_Func__0___0__System_Func_Rascal_Error___0__)' with an identity function for the 'ifOk' parameter is equivalent to '[DefaultOr](/api/Rascal.Result-1.html#Rascal_Result_1_GetValueOr_System_Func_Rascal_Error__0__)'.

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
