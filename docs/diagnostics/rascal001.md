# Use 'Map' instead of 'Then(x => Ok(...))'

<br/>

<div class="text-secondary lh-lg" style="font-size: 14px;">
Id: RASCAL001
<br/>
Severity: <span class="text-warning">warning</span>
<br/>
Has code fix: <span class="text-success">yes</span>
<br/>
</div>

<br/>

## Description

*RASCAL001* is reported when [`Ok`](~/api/Rascal.Prelude.yml#Rascal_Prelude_Ok__1___0_) or any other form of result construction is used as the immediate return from the lambda inside a [`Then`](~/api/Rascal.Result-1.yml#Rascal_Result_1_Then__1_System_Func__0_Rascal_Result___0___) call. Because [`Then`](~/api/Rascal.Result-1.yml#Rascal_Result_1_Then__1_System_Func__0_Rascal_Result___0___) chains results, this is equivalent to a much simpler [`Map`](~/api/Rascal.Result-1.yml#Rascal_Result_1_Map__1_System_Func__0___0__) call. The warning can be resolved by replacing the [`Then`](~/api/Rascal.Result-1.yml#Rascal_Result_1_Then__1_System_Func__0_Rascal_Result___0___) call with a [`Map`](~/api/Rascal.Result-1.yml#Rascal_Result_1_Map__1_System_Func__0___0__) which maps to the expression inside the [`Ok`](~/api/Rascal.Prelude.yml#Rascal_Prelude_Ok__1___0_).

<br/>

### Example

```cs
var a = Ok(2);

var b = a.Then(x => Ok(F(x))); // RASCAL001

// Fix:
var b = a.Map(x => F(x));
```
