using System.Reflection;
using System.Runtime.Loader;

namespace Piral.Blazor.Orchestrator;

internal class MicrofrontendLoadContext(string name) : AssemblyLoadContext(name, true)
{
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        //string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        //if (assemblyPath != null)
        //{
        //    return LoadFromAssemblyPath(assemblyPath);
        //}
        //return null;
        return base.Load(assemblyName);
    }
}
