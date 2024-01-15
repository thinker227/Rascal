#r "../src/Rascal/bin/Release/net8.0/Rascal.dll"

using Rascal;
using static Rascal.Prelude;

// Read console input and assert that it isn't null.
var name = Console.ReadLine().NotNull();

// Read console input, assert that it isn't null, then try parse it into an int.
var age = Console.ReadLine().NotNull()
    .Then(str => ParseR<int>(str));

// Combine the name and age results together, then map them into a person.
var person = name.Combine(age)
    .Map(v => new Person(v.first, v.second));

Console.WriteLine(person);

record Person(string Name, int Age);
