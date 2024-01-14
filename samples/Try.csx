#r "../src/Rascal/bin/Release/net8.0/Rascal.dll"

using Rascal;
using static Rascal.Prelude;

// Try read console input and use the input to read the specified file.
// If an exception is thrown, the exception will be returned as an error.
var text = Try(() =>
{
    var path = Console.ReadLine()!;
    return File.ReadAllText(path);
});

Console.WriteLine(text);
