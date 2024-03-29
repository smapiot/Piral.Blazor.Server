﻿using CommandLine;
using VsTools.Projects;

namespace Piral.Blazor.Cli;

using static Helpers;

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
        var srcDir = Path.Combine(Environment.CurrentDirectory, Source ?? "");
        var csproj = srcDir.GetProjectFile() ?? throw new InvalidOperationException($"No csproj file found in '{srcDir}'.");
        var buildDirRoot = Path.Combine(srcDir, "bin", "Release");
        var project = Project.Load(csproj);
        var projectName = project.GetName() ?? Path.GetFileNameWithoutExtension(csproj);
        var projectVersion = project.GetVersion() ?? "1.0.0";
        var file = $"{projectName}.{projectVersion}.nupkg";

        RunCommand("dotnet", "build -c Release", srcDir);

        if (!File.Exists(Path.Combine(buildDirRoot, file)))
        {
            var nupkg = Directory.GetFiles(buildDirRoot, "*.nupkg").FirstOrDefault();

            if (nupkg is not null)
            {
                file = Path.GetFileName(nupkg);
            }
            else
            {
                file = null;
            }
        }

        if (file is not null)
        {
            //TODO handle interactive case
            RunCommand("dotnet", $"nuget push {file} --api-key {ApiKey} --source {Url}", buildDirRoot);
        }

        return Task.CompletedTask;
    }
}
