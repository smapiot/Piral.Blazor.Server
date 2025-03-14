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
        var csproj = srcDir.GetProjectFile() ?? throw new InvalidOperationException($"No csproj file found in '{srcDir}'.");
        var buildDirRoot = Path.Combine(srcDir, "bin", "Debug");
        var buildDir = Path.Combine(buildDirRoot, "net8.0", "publish");
        var outDir = Path.Combine(Environment.CurrentDirectory, Output ?? buildDirRoot);

        RunCommand("dotnet", "publish -c Debug", srcDir);

        outDir.CreateDirectoryIfNotExists();

        CreateNuGetPackage(csproj, buildDir, outDir);

        return Task.CompletedTask;
    }

    private static string GetReadme(string name, string version, string description, string sdkVersion)
    {
        var p = "\n\n";

        string code(string lang, string block)
        {
           return $"```{lang}\n{block}\n```";
        }

        string link(string name, string target)
        {
           return $"[{name}]({target})";
        }

        var initialCode = code("xml", $@"<Project Sdk=""Piral.Blazor.Sdk/{sdkVersion}"">
  <PropertyGroup>
    <!-- ... as beforehand -->
  </PropertyGroup>
</Project>");
        var finalCode = code("xml", $@"<Project Sdk=""Piral.Blazor.Sdk/{sdkVersion}"">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Version>0.1.0</Version>
    <AppShell>{name}/{version}</AppShell>
  </PropertyGroup>

  <ItemGroup>
    <Folder Include=""wwwroot\"" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include=""{name}"" Version=""{version}"" PrivateAssets=""all"" />
  </ItemGroup>

</Project>");
        var moduleCode = code("cs", $@"public class Module : IMfModule
{{
    public void Configure(IServiceCollection services)
    {{
        // register your services here
    }}

    public Task Setup(IMfAppService app)
    {{
        // wire up your components / events here
        return Task.CompletedTask;
    }}

    public Task Teardown(IMfAppService app)
    {{
        // don't forget to destroy resources
        return Task.CompletedTask;
    }}
}}");
        var link1 = link("Piral", "https://www.piral.io");
        var link2 = link("Piral for Blazor", "https://blazor.piral.io");
        var link3 = link("Piral Cloud", "https://www.piral.cloud");
        var link4 = link("Consulting and support for Piral", "https://www.smapiot.com");

        return 
            $"# {name}{p}{description}{p}" +
            $"## Usage{p}Create a new Razor Component Library (RCL). Change the csproj to use the SDK:{p}{initialCode}{p}" +
            $"Indicate that you want to reference this version of the emulator:{p}{finalCode}{p}" +
            $"Create a new class for the module. This class needs to implement the `IMfModule` interface.{p}{moduleCode}{p}" +
            $"That's it! Now you can start coding following the concepts outlined for any micro frontend in the `Piral.Blazor.Server` world.{p}" +
            $"## More Documentation{p}You can find more information on the following websites:{p}" +
            $"- {link1}\n- {link2}\n- {link3}\n- {link4}";
    }

    private void CreateNuGetPackage(string csproj, string buildDir, string outDir)
    {
        var project = Project.Load(csproj);
        var projectName = project.GetName() ?? Path.GetFileNameWithoutExtension(csproj);
        var projectVersion = project.GetVersion() ?? "1.0.0";
        var projectAuthors = project.GetAuthor() ?? "Piral";
        var license = project.GetLicense() ?? "MIT";
        var sdkVersion = project.GetSdkVersion() ?? "0.5.0";
        var name = Name ?? $"{projectName}.Emulator";
        var description = $"The emulator for the {projectName} application.";
        var fn = Path.Combine(outDir, $"{name}.{projectVersion}.nupkg");

        File.WriteAllText(Path.Combine(buildDir, "README.md"), GetReadme(name, projectVersion, description, sdkVersion));

        var builder = new PackageBuilder
        {
            Id = name,
            Version = new NuGetVersion(projectVersion),
            RequireLicenseAcceptance = false,
            Readme = "README.md",
            Description = description,
            LicenseMetadata = new LicenseMetadata(LicenseType.Expression, license, NuGetLicenseExpression.Parse(license), null, LicenseMetadata.EmptyVersion),
        };

        builder.Authors.Add(projectAuthors);

        builder.AddFiles(buildDir, "**/*", "");

        using FileStream outputStream = new(fn, FileMode.Create);
        builder.Save(outputStream);

        Console.WriteLine($"[nuget] Package available in '{fn}'.");
    }
}
