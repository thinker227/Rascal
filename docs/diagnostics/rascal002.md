# Use Then instead of 'Map(...).Unnest()'

<br/>

<div class="text-secondary lh-lg" style="font-size: 14px;">
Id: RASCAL002
<br/>
Severity: <span class="text-warning">warning</span>
<br/>
Has code fix: <span class="text-success">yes</span>
<br/>
</div>

<br/>

## Description

*RASCAL002* is reported when [`Unnest`](/api/Rascal.ResultExtensions.html#Rascal_ResultExtensions_Unnest__1_Rascal_Result_Rascal_Result___0___) is called immediately after a [`Map`](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__) call. This operation is equivalent to calling [`Then`](/api/Rascal.Result-1.html#Rascal_Result_1_Then__1_System_Func__0_Rascal_Result___0___) with the same mapping function, which improves clarity and performance. The warning can be resolved by removing the [`Unnest`](/api/Rascal.ResultExtensions.html#Rascal_ResultExtensions_Unnest__1_Rascal_Result_Rascal_Result___0___) call and replacing the [`Then`](/api/Rascal.Result-1.html#Rascal_Result_1_Then__1_System_Func__0_Rascal_Result___0___) call with a [`Map`](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__).

<br/>

### Example

```cs
var a = Ok(2);

var b = a.Map(x => F(x)).Unnest(); // RASCAL002

// Fix:
var b = a.Then(x => F(x));
```
