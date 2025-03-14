using Microsoft.Extensions.DependencyInjection;
using Piral.Blazor.Shared;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json.Nodes;

namespace Piral.Blazor.Orchestrator;

internal class ModuleContainerService(IServiceProvider globalProvider, IGlobalEvents events, IData data) : IModuleContainerService
{
    private readonly IServiceProvider _globalProvider = globalProvider;
    private readonly IGlobalEvents _events = events;
    private readonly IData _data = data;
    private readonly Dictionary<AssemblyLoadContext, IScopeResolver> _resolvers = [];

    public (IMfModule, IServiceProvider) ConfigureModule(Assembly assembly, JsonObject? config)
    {
        var alc = AssemblyLoadContext.GetLoadContext(assembly);
        var module = EmptyMfModule.Instance;
        var piletServices = new ServiceCollection();

        if (alc is not null && !_resolvers.ContainsKey(alc))
        {
            var assName = assembly.GetName();
            var ModuleType = FindEntryClass(assembly);
            var pilet = new WrappedMfService(_events, _data, new MfDetails
            {
                Name = assName.Name!,
                Version = assName.Version!.ToString(),
                Config = config ?? [],
            });

            alc.Unloading += (context) =>
            {
                if (_resolvers.Remove(context, out var resolver))
                {
                    resolver.Dispose();
                }
            };

            piletServices.AddSingleton<IMfService>(pilet);

            if (ModuleType is not null)
            {
                var moduleInstance = ActivatorUtilities.CreateInstance(_globalProvider, ModuleType) as IMfModule;

                if (moduleInstance is not null)
                {
                    moduleInstance.Configure(piletServices);
                    module = moduleInstance;
                }
            }

            var resolver = new ScopeResolver(piletServices);
            var localProvider = resolver.Resolve(_globalProvider);
            _resolvers.Add(alc, resolver);
            return (module, localProvider);
        }

        return (module, _globalProvider);
    }

    public IScopeResolver? GetProvider(Assembly assembly)
    {
        var alc = AssemblyLoadContext.GetLoadContext(assembly);

        if (alc is not null)
        {
            return _resolvers.GetValueOrDefault(alc);
        }

        return null;
    }

    private static Type? FindEntryClass(Assembly assembly) => 
        assembly.GetTypes().FirstOrDefault(x => x.GetInterfaces().Contains(typeof(IMfModule)));

    sealed class EmptyMfModule : IMfModule
    {
        public static readonly IMfModule Instance = new EmptyMfModule();

        public void Configure(IServiceCollection services)
        {
        }

        public Task Setup(IMfAppService app)
        {
            return Task.CompletedTask;
        }

        public Task Teardown(IMfAppService app)
        {
            return Task.CompletedTask;
        }
    }

    sealed class WrappedMfService(IGlobalEvents events, IData data, MfDetails meta) : IMfService
    {
        private readonly IData _data = data;
        private readonly MfDetails _meta = meta;
        private readonly IGlobalEvents _events = events;

        public MfDetails Meta => _meta;

        public void AddEventListener<T>(string type, Action<T> handler)
        {
            _events.AddEventListener(type, handler);
        }

        public void DispatchEvent<T>(string type, T args)
        {
            _events.DispatchEvent(type, args);
        }

        public void RemoveEventListener<T>(string type, Action<T> handler)
        {
            _events.RemoveEventListener(type, handler);
        }

        public bool TryGetData<T>(string name, out T value)
        {
            return _data.TryGetData(_meta.Name, name, out value);
        }

        public bool TrySetData<T>(string name, T value)
        {
            return _data.TrySetData(_meta.Name, name, value);
        }
    }
}
