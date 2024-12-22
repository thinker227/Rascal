#r "../src/Rascal/bin/Release/net8.0/Rascal.dll"

using System.Text.RegularExpressions;
using Rascal;
using Rascal.Errors;
using static Rascal.Prelude;

// Read console input, assert that it isn't null, and validate that it matches the regex.
var name = Console.ReadLine().NotNull()
    .Validate(
        str => Regex.IsMatch(str, "[A-Z][a-z]*"),
        _ => "Name can only contain characters a-z and has to start with a capital letter.");

var person = name.Then(n =>
{
    // Read console input, assert that it isn't null, try parse it into an int, then validate that it is greater than 0.
    var age = Console.ReadLine().NotNull()
        .Then(str => ParseR<int>(str))
        .Validate(
            x => x > 0,
            _ => "Age has to be greater than 0.");

    return age.Map(a => new Person(n, a));
});

Console.WriteLine(person);

record Person(string Name, int Age);
