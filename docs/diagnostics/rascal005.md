# 'To' called with impossible type

<br/>

<div class="text-secondary lh-lg" style="font-size: 14px;">
Id: RASCAL005
<br/>
Severity: <span class="text-warning">warning</span>
<br/>
Has code fix: <span class="text-danger">no</span>
<br/>
</div>

<br/>

## Description

*RASCAL005* is reported when [`To`](/api/Rascal.Result-1.html#Rascal_Result_1_To__1_Rascal_Error_) is called with a type which is incompatible with the type of the result. This mainly applies to class types which do not inherit each other, and structs which aren't the same type. Interfaces, type parameters, and `object` may always succeed regardless of which type they are casted to/from.

<br/>

### Example

```cs
// Types added for clarity

Result<int> a = Ok(2);

Result<string> b = a.To<string>(); // RASCAL005
```

```cs
// Types added for clarity

Result<B> a = Ok(new B());

Result<C> b = a.To<C>(); // RASCAL005

class A;
class B : A;
class C : A;
```
