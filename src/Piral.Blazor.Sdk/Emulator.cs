﻿// <autogenerated />

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Piral.Blazor.Sdk;

public static class Emulator
{
    public static void Main(string[] args)
    {
        var assembly = Assembly.GetEntryAssembly()!;
        var path = FindPath(assembly);
        var others = FindOtherModules(assembly.Location);
        var dir = Path.GetDirectoryName(path)!;
        var fn = Path.GetFileNameWithoutExtension(path)!;

        Environment.SetEnvironmentVariable("Microfrontends__CacheDir", ".cache");
        Environment.SetEnvironmentVariable("Microfrontends__NugetFeeds__Public__Url", "https://api.nuget.org/v3/index.json");

        Environment.SetEnvironmentVariable("ASPNETCORE_APPLICATIONNAME", fn);
        Environment.SetEnvironmentVariable("PIRAL_BLAZOR_DEBUG_ASSEMBLY", assembly.FullName);
        Environment.SetEnvironmentVariable("PIRAL_BLAZOR_ALL_DEBUG_ASSEMBLIES", string.Join(",", others));

        Environment.CurrentDirectory = dir;

        var root = new AssemblyLoadContext("root");

        root.Resolving += (alc, assemblyName) =>
        {
            var path = Path.Combine(dir, $"{assemblyName.Name}.dll");
            return alc.LoadFromAssemblyPath(path);
        };

        var ass = root.LoadFromAssemblyPath(path);
        var assName = ass.GetName().Name;

        AssemblyLoadContext.Default.Resolving += (_, assemblyName) =>
        {
            // In general we just keep default; but the app shell we also resolve here
            if (assemblyName.Name == assName)
            {
                return ass;
            }

            return null;
        };

        var prog = ass.DefinedTypes.First(m => m.Name == "Program");
        var method = prog.GetMethods(BindingFlags.Static | BindingFlags.NonPublic).First();

        method.Invoke(null, new[] { args });
    }

    private static string[] FindOtherModules(string root)
    {
        var dir = Path.GetDirectoryName(root)!;
        var files = Directory.GetFiles(dir);
        return files
            .Where(path => IsExecutableLib(files, path))
            .ToArray();
    }

    private static void RunCommand(string cmd, string arguments, string cwd)
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

    private static string TryResolveLocalPackageAt(string path)
    {
        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path, "*.csproj");

            if (files.Length == 1)
            {
                var subpath = Path.Combine(path, "bin", "Debug", "net8.0", "publish");
                //TODO check if outdated
                var mustBuild = !Directory.Exists(subpath);

                if (mustBuild)
                {
                    RunCommand("dotnet", "publish -c Debug", path);
                }

                return subpath;
            }
        }

#pragma warning disable CS8603 // Possible null reference return.
        return null;
#pragma warning restore CS8603 // Possible null reference return.
    }

    private static string TryResolveLocalPackage(string root, string name)
    {
        //TODO
        // add more potential combinations incl. inspect CSPROJ / SLN file
        // to find out where the local package (if applicable) really is
        return TryResolveLocalPackageAt(Path.Combine(root, "..", name));
    }

    private static string ResolveGlobalPackage(string name)
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(userProfile, ".nuget", "packages", name.ToLowerInvariant());
    }

    private static string ResolvePath(Assembly assembly)
    {
        var name = assembly.GetCustomAttribute<Piral.Blazor.Sdk.AppShellAttribute>()!.Name;

        if (!name.StartsWith("."))
        {
            var path = assembly.GetCustomAttribute<Piral.Blazor.Sdk.ProjectFolderAttribute>()!.Path;
            return TryResolveLocalPackage(path, name) ?? ResolveGlobalPackage(name);
        }
        
        return Path.Combine(Environment.CurrentDirectory, name);
    }

    private static string FindPath(Assembly assembly)
    {
        var path = ResolvePath(assembly);

        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path);
            // looking for a .exe file (Windows) or the same file without the extension (Linux or MacOS)
            var dll = files.FirstOrDefault(path => IsExecutableLib(files, path));

            if (dll is not null)
            {
                return dll;
            }
        }
        
        throw new InvalidOperationException("Missing a valid <AppShell> property leading to an app shell emulator NuGet package.");
    }

    private static bool IsExecutableLib(string[] files, string path)
    {
        if (Path.GetExtension(path) == ".dll")
        {
            var linuxApp = Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path));
            var windowsExe = Path.ChangeExtension(path, ".exe");
            return files.Contains(windowsExe) || files.Contains(linuxApp);
        }

        return false;
    }
}
