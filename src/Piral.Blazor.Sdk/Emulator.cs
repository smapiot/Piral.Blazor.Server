using System;
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
        var dir = Path.GetDirectoryName(path)!;
        var fn = Path.GetFileNameWithoutExtension(path)!;

        Environment.SetEnvironmentVariable("Microfrontends__CacheDir", ".cache");
        Environment.SetEnvironmentVariable("Microfrontends__NugetFeeds__Public__Url", "https://api.nuget.org/v3/index.json");

        Environment.SetEnvironmentVariable("ASPNETCORE_APPLICATIONNAME", fn);
        Environment.SetEnvironmentVariable("PIRAL_BLAZOR_DEBUG_ASSEMBLY", assembly.FullName);

        Environment.CurrentDirectory = dir;

        AssemblyLoadContext.Default.Resolving += (alc, assemblyName) =>
        {
            var path = Path.Combine(dir, $"{assemblyName.Name}.dll");
            return alc.LoadFromAssemblyPath(path);
        };

        var ass = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        var prog = ass.DefinedTypes.First(m => m.Name == "Program");
        var method = prog.GetMethods(BindingFlags.Static | BindingFlags.NonPublic).First();

        method.Invoke(null, new[] { args });
    }

    private static string FindPath(Assembly assembly)
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var name = assembly.GetCustomAttribute<Piral.Blazor.Sdk.AppShellAttribute>().Name;
        var path = Path.Combine(userProfile, ".nuget", "packages", name);

        if (Directory.Exists(path))
        {
            var exe = Directory.GetFiles(path, "*.exe").FirstOrDefault();

            if (exe is not null)
            {
                return Path.ChangeExtension(exe, ".dll");
            }
        }
        
        throw new InvalidOperationException("Missing a valid <AppShell> property leading to an app shell emulator NuGet package.");
    }
}
