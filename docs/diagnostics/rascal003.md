# Unnecessary 'Map' call with identity function

Id: *RASCAL003*

Severity: *warning*

> [!TIP]
> This diagnostic has an associated automatic code fix.

<br/>

## Description

Calling '[Map](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__)' with an identity function returns the same result as the input. Remove this call to '[Map](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__)'.

<br/>

### Example

```cs
var a = Ok(2);

var b = a.Map(x => x); // RASCAL003

// Fix:
var b = a;
```
