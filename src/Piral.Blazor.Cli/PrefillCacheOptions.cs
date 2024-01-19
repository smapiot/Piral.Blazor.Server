using CommandLine;
using Microsoft.Extensions.Configuration;

namespace Piral.Blazor.Cli;

[Verb("prefill-cache", HelpText = "Populates the .cache folder of an application already.")]
public class PrefillCacheOptions : ICommand
{
    [Option('o', "output", Required = false, HelpText = "Sets where the files should be stored - by default '.cache' relative to the current directory.")]
    public string? Output { get; set; }

    [Option('s', "source", Required = false, HelpText = "The path to the source directory containing the 'appsettings.json' file. By default the current directory is used.")]
    public string? Source { get; set; }

    [Option("secrets-id", Required = false, HelpText = "The user secrets id, if user secrets are used in the configuration.")]
    public string? SecretsId { get; set; }

    public Task Run()
    {
        var source = Path.Combine(Environment.CurrentDirectory, Source ?? "");
        var output = Path.Combine(Environment.CurrentDirectory, Output ?? ".cache");
        var secretsId = SecretsId;
        output.CreateDirectoryIfNotExists();

        var builder = new ConfigurationBuilder()
            .SetBasePath(source)
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json");

        if (secretsId is not null)
        {
            builder.AddUserSecrets(secretsId);
        }

        var config = builder.Build();

        var feeds = config.GetSection("Microfrontends:NugetFeeds").Get<Dictionary<string, NugetFeedConfig>>()!;

        Console.WriteLine("Found the following {0} feeds: {1}", feeds.Count, string.Join(", ", feeds.Keys));

        Console.WriteLine("Not yet implemented.");

        return Task.CompletedTask;
    }

    class NugetFeedConfig
    {
        public string Url { get; set; } = string.Empty;

        public string? User { get; set; }

        public string? Token { get; set; }
    }
}
