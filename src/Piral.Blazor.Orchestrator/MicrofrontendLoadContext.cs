using System.Reflection;
using System.Runtime.Loader;

namespace Piral.Blazor.Orchestrator;

internal class MicrofrontendLoadContext(string name, Func<string, Assembly?> resolve) : AssemblyLoadContext(name, true)
{
    private readonly AssemblyLoadContext _root = All.FirstOrDefault(m => m.Name == "root") ?? Default;

    public readonly Func<string, Assembly?> _resolve = resolve;

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var existing = GetExisting(_root.Assemblies, assemblyName);

        if (existing is not null)
        {
            return existing;
        }

        // root and default are different; we must be in an emulator
        if (_root != Default)
        {
            // Let's see if we find this in the default ALC
            var globalAssembly = GetExisting(Default.Assemblies, assemblyName);

            if (globalAssembly is not null)
            {
                // In case we found it we return null to trigger the not found flow
                return null;
            }
        }

        // Let's find it via the custom resolve
        return _resolve($"{assemblyName.Name}.dll");
    }

    private static Assembly? GetExisting(IEnumerable<Assembly> assemblies, AssemblyName name)
    {
        return assemblies.FirstOrDefault(m => AssemblyName.ReferenceMatchesDefinition(m.GetName(), name));
    }
}
