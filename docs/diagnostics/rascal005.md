# 'To' called with impossible type

Id: *RASCAL005*

Severity: *warning*

<br/>

## Description

Calling '[To](/api/Rascal.Result-1.html#Rascal_Result_1_To__1_Rascal_Error_)' with a type which no value of the type of the result permits will always fail.

<br/>

### Example

```cs
var a = Ok(2);

var b = a.To<string>(); // RASCAL005
```
