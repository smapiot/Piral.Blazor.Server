﻿using Microsoft.Extensions.DependencyInjection;
using Piral.Blazor.Shared;
using System.Reflection;
using System.Runtime.Loader;

namespace Piral.Blazor.Orchestrator;

internal class ModuleContainerService : IModuleContainerService
{
    private readonly IServiceProvider _globalProvider;
    private readonly Dictionary<AssemblyLoadContext, IScopeResolver> _resolvers;

    public ModuleContainerService(IServiceProvider globalProvider)
    {
        _globalProvider = globalProvider;
        _resolvers = new Dictionary<AssemblyLoadContext, IScopeResolver>();
    }

    public async Task<IMfModule> ConfigureModule(Assembly assembly, IMfAppService app)
    {
        var alc = AssemblyLoadContext.GetLoadContext(assembly);
        var module = EmptyMfModule.Instance;

        if (alc is not null && !_resolvers.ContainsKey(alc))
        {
            var piletServices = new ServiceCollection();
            var ModuleType = FindEntryClass(assembly);
            var pilet = new WrappedMfService(app);

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

                    await moduleInstance.Setup(app);

                    module = moduleInstance;
                }
            }

            _resolvers.Add(alc, new ScopeResolver(piletServices));
        }

        return module;
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

    sealed class WrappedMfService : IMfService
    {
        private readonly IMfAppService _app;

        public WrappedMfService(IMfAppService app)
        {
            _app = app;
        }

        public void AddEventListener<T>(string type, Action<T> handler)
        {
            _app.AddEventListener(type, handler);
        }

        public void DispatchEvent<T>(string type, T args)
        {
            _app.DispatchEvent(type, args);
        }

        public void RemoveEventListener<T>(string type, Action<T> handler)
        {
            _app.RemoveEventListener(type, handler);
        }
    }
}