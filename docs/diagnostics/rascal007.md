# Missing symbol required for analysis

<br/>

<div class="text-secondary lh-lg" style="font-size: 14px;">
Id: RASCAL007
<br/>
Severity: <span class="text-warning">warning</span>
<br/>
Has code fix: <span class="text-danger">no</span>
<br/>
</div>

<br/>

## Description

*RASCAL007* is reported if any type or member which is required by the analysis suite to perform analysis is found to be missing. The most likely cause of this is referencing the analysis assembly without referencing the core Rascal assembly, or referencing a higher version of the analysis assembly than that of the core assembly. This may also be the result of a bug in the analysis suite.

> [!IMPORTANT]
> If *RASCAL007* occurs, all analyzers from the analysis suite will completely stop working until the warning is resolved.
