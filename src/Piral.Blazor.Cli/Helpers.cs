using System.Diagnostics;
using VsTools.Projects;

namespace Piral.Blazor.Cli;

static class Helpers
{
    public static void RunCommand(string cmd, string arguments, string cwd)
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

    public static void CreateDirectoryIfNotExists(this string targetDir)
    {
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }
    }

    public static string GetProjectFile(this string sourceDir)
    {
        return Directory.GetFiles(sourceDir, "*.csproj").First();
    }

    public static string? GetAuthor(this Project project)
    {
        return project.PropertyGroups
            .Select(group => group.GetElement("Authors")?.Value)
            .Where(m => m is not null)
            .FirstOrDefault();
    }

    public static string? GetLicense(this Project project)
    {
        return project.PropertyGroups
            .Select(group => group.GetElement("PackageLicenseExpression")?.Value)
            .Where(m => m is not null)
            .FirstOrDefault();
    }

    public static string? GetSdkVersion(this Project project)
    {
        var packageReference = project.ItemGroups
            .Select(group => group.GetChildElements()?.FirstOrDefault(m => m.ElementName == "PackageReference" && m.GetAttributeValue("Include") == "Piral.Blazor.Orchestrator"))
            .Where(m => m is not null)
            .FirstOrDefault();

        return packageReference?.GetAttributeValue("Version");
    }

    public static string? GetVersion(this Project project)
    {
        return project.PropertyGroups
            .Select(group => group.GetElement("PackageVersion")?.Value ?? group.GetElement("Version")?.Value ?? group.GetElement("VersionPrefix")?.Value)
            .Where(m => m is not null)
            .FirstOrDefault();
    }

    public static string? GetName(this Project project)
    {
        return project.PropertyGroups
            .Select(group =>group.GetElement("PackageId")?.Value ?? group.GetElement("AssemblyName")?.Value ?? group.GetElement("AssemblyTitle")?.Value)
            .Where(m => m is not null)
            .FirstOrDefault();
    }
}
