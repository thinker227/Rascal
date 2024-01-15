#r "../src/Rascal/bin/Release/net8.0/Rascal.dll"

using Rascal;
using static Rascal.Prelude;

var name = "Raymond";

// Read console input and try parse it into an int.
// If the input cannot be parsed, the result will be an error.
var age = ParseR<int>(Console.ReadLine()!);

// Map the age to a new person.
// If the age is an error, the person will also be an error.
var person = age.Map(x => new Person(name, x));

Console.WriteLine(person);

record Person(string Name, int Age);
