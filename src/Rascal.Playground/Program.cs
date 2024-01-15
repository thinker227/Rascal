using Rascal;
using static Rascal.Prelude;

var num = Console.ReadLine()
    .NotNull("Failed to read from stdin.")
    .Then(s => ParseR<int>(s, null, $"{s} is not a number!"))
    .Validate(x => x >= 0, _ => "Number cannot be negative!");

num.Switch(
    x => Console.WriteLine($"You entered {x}!"),
    e => Console.WriteLine(e.Message)
);
