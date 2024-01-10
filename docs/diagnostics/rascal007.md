# Missing symbol required for analysis

Id: *RASCAL007*

Severity: *warning*

<br/>

## Description

Cannot find type or member '*member*' which is required for analysis. No analysis will be performed. Verify that the version of the analyzer package matches that of the library, or report this as a bug.

<br/>

### Cause

This diagnostic will only occur if the analyzer assembly is referenced without the main Rascal assembly being present. This is most probably the result of a bug.
