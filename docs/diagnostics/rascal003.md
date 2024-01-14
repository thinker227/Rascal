# Unnecessary 'Map' call with identity function

<br/>

<div class="text-secondary lh-lg" style="font-size: 14px;">
Id: RASCAL003
<br/>
Severity: <span class="text-warning">warning</span>
<br/>
Has code fix: <span class="text-success">yes</span>
<br/>
</div>

<br/>

## Description

*RASCAL003* is reported when [`Map`](~/api/Rascal.Result-1.yml#Rascal_Result_1_Map__1_System_Func__0___0__) is used with an *identity function*, i.e. a lambda in the form `x => x`. Because [`Map`](~/api/Rascal.Result-1.yml#Rascal_Result_1_Map__1_System_Func__0___0__) transforms the ok value of a result, applying an identity function onto the value does nothing and returns the same result as the input, and the call is completely useless. The warning can be resolved by removing the call altogether.

<br/>

### Example

```cs
var a = Ok(2);

var b = a.Map(x => x); // RASCAL003

// Fix:
var b = a;
```
