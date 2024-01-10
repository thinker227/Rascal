# Use Then instead of 'Map(...).Unnest()'

Id: *RASCAL002*

Severity: *warning*

> [!TIP]
> This diagnostic has an associated automatic code fix.

<br/>

## Description

Calling '[Unnest](/api/Rascal.ResultExtensions.html#Rascal_ResultExtensions_Unnest__1_Rascal_Result_Rascal_Result___0___)' directly after a '[Map](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__)' call is equivalent to calling '[Then](/api/Rascal.Result-1.html#Rascal_Result_1_Then__1_System_Func__0_Rascal_Result___0___)'. Use '[Then](/api/Rascal.Result-1.html#Rascal_Result_1_Then__1_System_Func__0_Rascal_Result___0___)' instead for clarity and performance.

<br/>

### Example

```cs
var a = Ok(2);

var b = a.Map(x => F(x)).Unnest(); // RASCAL002

// Fix:
var b = a.Then(x => F(x));
```
