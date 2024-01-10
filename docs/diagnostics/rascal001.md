# Use 'Map' instead of 'Then(x => Ok(...))'

Id: *RASCAL001*

Severity: *warning*

> [!TIP]
> This diagnostic has an associated automatic code fix.

<br/>

## Description

Calling '[Ok](/api/Rascal.Prelude.html#Rascal_Prelude_Ok__1___0_)' directly inside a '[Then](/api/Rascal.Result-1.html#Rascal_Result_1_Then__1_System_Func__0_Rascal_Result___0___)' call is equivalent to calling '[Map](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__)'. Use '[Map](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__)' instead for clarity and performance.

<br/>

### Example

```cs
var a = Ok(2);

var b = a.Then(x => Ok(F(x))); // RASCAL001

// Fix:
var b = a.Then(x => Ok(F(x)));
```
