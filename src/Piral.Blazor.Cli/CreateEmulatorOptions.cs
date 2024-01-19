using System.Diagnostics;
using CommandLine;

namespace Piral.Blazor.Cli;

[Verb("create-emulator", HelpText = "Creates a new Piral.Blazor.Server-based emulator package.")]
public class CreateEmulatorOptions : ICommand
{
    [Option('d', "directory", Required = false, HelpText = "The directory where the csproj of the app shell is located.")]
    public string? Directory { get; set; }
    
    [Option('n', "name", Required = false, HelpText = "The name of the emulator. Otherwise falls back to the current project name suffixed with '.Emulator'.")]
    public string? Name { get; set; }
    
    [Option('v', "version", Required = false, HelpText = "The version of the emulator. Otherwise falls back to the current project version.")]
    public string? Version { get; set; }

    public Task Run()
    {
        var cwd = Path.Combine(Environment.CurrentDirectory, Directory ?? "");
        var outDir = Path.Combine(cwd, "bin", "Release", "net8.0", "publish");

        RunCommand("dotnet", "publish -c Release", cwd);

        CreateNuspec(cwd, outDir);

        RunCommand("nuget", "pack", outDir);

        return Task.CompletedTask;
    }

    private void CreateNuspec(string sourceDir, string outDir)
    {
        var projectName = "My";
        var projectVersion = "1.0.0";
        var projectAuthors = "tbd.";
        var name = Name ?? $"{projectName}.Emulator";
        var version = Version ?? projectVersion;
        var fn = Path.Combine(outDir, $"{name}.nuspec");
        var content = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<package xmlns=""http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"">
    <metadata>
        <id>{name}</id>
        <version>{version}</version>
        <description>The emulator for the {projectName} application.</description>
        <authors>{projectAuthors}</authors>
    </metadata>
    <files>
        <file src=""**/*"" target="""" />
    </files>
</package>";

        File.WriteAllText(fn, content);
    }

    private void RunCommand(string cmd, string arguments, string cwd)
    {
        var proc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = cmd,
                WorkingDirectory = cwd,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            },
        };

        proc.OutputDataReceived += (sender, e) => Console.WriteLine($"[{cmd}] {e.Data}");
        proc.ErrorDataReceived += (sender, e) => Console.WriteLine($"[{cmd}] {e.Data}");

        proc.Start();
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();
        proc.WaitForExit();

        if (proc.ExitCode != 0)
        {
            throw new Exception($"The '{cmd}' application exited with an error.");
        }
    }
}
