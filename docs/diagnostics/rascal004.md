# 'To' called with same type as result

<br/>

<div class="text-secondary lh-lg" style="font-size: 14px;">
Id: RASCAL004
<br/>
Severity: <span class="text-warning">warning</span>
<br/>
Has code fix: <span class="text-danger">no</span>
<br/>
</div>

<br/>

## Description

*RASCAL004* is reported when [`To`](/api/Rascal.Result-1.html#Rascal_Result_1_To__1_Rascal_Error_) is called with the same type as that of the result it is called on. Because `.To<T>()` is equivalent to an `is T` pattern, calling `.To<T>` on a `Result<T>` where both `T`s are the same type will always succeed if the result is ok, and the call is redundant. The warning can be resolved by removing the call altogether.

<br/>

### Example

```cs
// Types added for clarity

Result<int> a = Ok(2);

Result<int> b = a.To<int>(); // RASCAL004
```
