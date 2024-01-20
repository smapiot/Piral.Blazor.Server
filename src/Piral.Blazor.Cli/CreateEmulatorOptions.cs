using CommandLine;
using NuGet.Packaging;
using NuGet.Packaging.Licenses;
using NuGet.Versioning;
using VsTools.Projects;

namespace Piral.Blazor.Cli;

using static Helpers;

[Verb("create-emulator", HelpText = "Creates a new Piral.Blazor.Server-based emulator package.")]
public class CreateEmulatorOptions : ICommand
{
    [Option('s', "source", Required = false, HelpText = "The source directory where the csproj of the app shell is located.")]
    public string? Source { get; set; }

    [Option('o', "output", Required = false, HelpText = "The output directory where the NuGet package should be stored in.")]
    public string? Output { get; set; }

    [Option('n', "name", Required = false, HelpText = "The name of the emulator. Otherwise falls back to the current project name suffixed with '.Emulator'.")]
    public string? Name { get; set; }
    
    [Option('v', "version", Required = false, HelpText = "The version of the emulator. Otherwise falls back to the current project version.")]
    public string? Version { get; set; }

    public Task Run()
    {
        var srcDir = Path.Combine(Environment.CurrentDirectory, Source ?? "");
        var buildDirRoot = Path.Combine(srcDir, "bin", "Release");
        var buildDir = Path.Combine(buildDirRoot, "net8.0", "publish");
        var outDir = Path.Combine(Environment.CurrentDirectory, Output ?? buildDirRoot);

        RunCommand("dotnet", "publish -c Release", srcDir);

        outDir.CreateDirectoryIfNotExists();

        CreateNuGetPackage(srcDir, buildDir, outDir);

        return Task.CompletedTask;
    }

    private void CreateNuGetPackage(string srcDir, string buildDir, string outDir)
    {
        var csproj = srcDir.GetProjectFile();
        var project = Project.Load(csproj);
        var projectName = project.GetName() ?? Path.GetFileNameWithoutExtension(csproj);
        var projectVersion = project.GetVersion() ?? "1.0.0";
        var projectAuthors = project.GetAuthor() ?? "Piral";
        var license = project.GetLicense() ?? "MIT";
        var name = Name ?? $"{projectName}.Emulator";
        var fn = Path.Combine(outDir, $"{name}.nupkg");

        var builder = new PackageBuilder
        {
            Id = name,
            Version = new NuGetVersion(projectVersion),
            RequireLicenseAcceptance = false,
            //Readme = "README.md",
            Description = $"The emulator for the {projectName} application.",
            LicenseMetadata = new LicenseMetadata(LicenseType.Expression, license, NuGetLicenseExpression.Parse(license), null, LicenseMetadata.EmptyVersion),
        };

        builder.Authors.Add(projectAuthors);

        builder.AddFiles(buildDir, "**/*", "");

        using FileStream outputStream = new(fn, FileMode.Create);
        builder.Save(outputStream);

        Console.WriteLine($"[nuget] Package available in '{fn}'.");
    }
}
