#r "../src/Rascal/bin/Release/net8.0/Rascal.dll"

using Rascal;
using static Rascal.Prelude;

// Read console input and assert that it isn't null.
// If the input is null, the value will be an error.
var name = Console.ReadLine().NotNull();

// Chain an operation on the name which will only execute if the name is ok.
var person = name.Then(n =>
{
    // Read console input, assert that it isn't null, then try parse it into an int.
    var age = Console.ReadLine().NotNull()
        .Then(str => ParseR<int>(str));

    // Map the age into a new person.
    return age.Map(a => new Person(n, a));
});

Console.WriteLine(person);

record Person(string Name, int Age);
