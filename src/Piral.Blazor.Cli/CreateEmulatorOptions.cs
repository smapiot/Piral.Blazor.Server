using CommandLine;

namespace Piral.Blazor.Cli;

[Verb("create-emulator", HelpText = "Creates a new Piral.Blazor.Server-based emulator package.")]
public class CreateEmulatorOptions
{

    [Option('j', "joke", Required = false, HelpText = "Output joke")]
    public bool Joke { get; set; }
}
