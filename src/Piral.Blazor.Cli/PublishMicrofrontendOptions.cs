using CommandLine;

namespace Piral.Blazor.Cli;

[Verb("publish-microfrontend", HelpText = "Publishes a micro frontend package to a micro frontend discovery service.")]
public class PublishMicrofrontendOptions : ICommand
{
    [Option('s', "source", Required = false, HelpText = "The source directory where the csproj of the micro frontend is located.")]
    public string? Source { get; set; }

    [Option('u', "url", Required = true, HelpText = "The URL of the micro frontend discovery service.")]
    public string Url { get; set; } = "";

    [Option('k', "key", Required = false, HelpText = "The optional API key for accessing the micro frontend discovery service.")]
    public string? ApiKey { get; set; }

    [Option('i', "interactive", Required = false, HelpText = "Determines if the authentication to the micro frontend discovery service can be obtained interactively, i.e., via the browser.")]
    public bool IsInteractive { get; set; } = false;

    public Task Run()
    {
        Console.WriteLine("Not yet implemented.");

        return Task.CompletedTask;
    }
}
