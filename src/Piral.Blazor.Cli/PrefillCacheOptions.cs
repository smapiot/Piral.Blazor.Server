using CommandLine;

namespace Piral.Blazor.Cli;

[Verb("prefill-cache", HelpText = "Populates the .cache folder of an application already.")]
public class PrefillCacheOptions : ICommand
{

    [Option('j', "joke", Required = false, HelpText = "Output joke")]
    public bool Joke { get; set; }

    public Task Run()
    {
        Console.WriteLine("Not yet implemented.");
        return Task.CompletedTask;
    }
}
