#r "../src/Rascal/bin/Release/net8.0/Rascal.dll"

using Rascal;
using static Rascal.Prelude;

// Read console input and assert that it isn't null.
var path = Console.ReadLine().NotNull();

// Try to map the input by reading a file specified by the input.
// If ReadAllText throws an exception, the exception will be returned as an error.
var text = path.TryMap(p => File.ReadAllText(p));

Console.WriteLine(text);
