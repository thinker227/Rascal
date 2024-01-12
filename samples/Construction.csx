#r "../src/Rascal/bin/Release/net8.0/Rascal.dll"

using Rascal;
using Rascal.Errors;
using static Rascal.Prelude;

// You can create a result either through explicit Ok/Error functions...
var explicitOk = Ok(new Person("Melody", 27));
var explicitError = Err<Person>("Could not find person");

// ... or through implicit conversions...
Result<Person> implicitOk = new Person("Edwin", 32);
Result<Person> implicitError = new StringError("Failed to find person");

record Person(string Name, int Age);
