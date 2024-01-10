# 'To' called with same type as result

Id: *RASCAL004*

Severity: *warning*

<br/>

## Description

Calling '[To](/api/Rascal.Result-1.html#Rascal_Result_1_To__1_Rascal_Error_)' with the same type as that of the result will always succeed.

<br/>

### Example

```cs
var a = Ok(2);

var b = a.To<int>(); // RASCAL004
```
