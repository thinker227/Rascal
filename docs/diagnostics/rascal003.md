# Unnecessary 'Map' call with identity function

Id: *RASCAL003*

Severity: *warning*

> [!TIP]
> This diagnostic has an associated automatic code fix.

<br/>

## Description

*RASCAL003* is reported when [`Map`](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__) is used with an *identity function*, i.e. a lambda in the form `x => x`. Because [`Map`](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__) transforms the ok value of a result, applying an identity function onto the value does nothing and returns the same result as the input, and the call is completely useless. The warning can be resolved by removing the call altogether.

<br/>

### Example

```cs
var a = Ok(2);

var b = a.Map(x => x); // RASCAL003

// Fix:
var b = a;
```
