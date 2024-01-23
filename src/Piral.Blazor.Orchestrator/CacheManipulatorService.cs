using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System.Reflection;

namespace Piral.Blazor.Orchestrator;

internal class CacheManipulatorService : ICacheManipulatorService
{
    private readonly Type _ctiType;
    private readonly Func<Type, Action<IServiceProvider, IComponent>> _createInitializer;
    private readonly object _original;
    private readonly IModuleContainerService _container;

    public CacheManipulatorService(IModuleContainerService container)
    {
        var cf = typeof(Renderer).Assembly.GetType("Microsoft.AspNetCore.Components.ComponentFactory")!;
        var field = cf.GetField("_cachedComponentTypeInfo", BindingFlags.Static | BindingFlags.NonPublic)!;
        _ctiType = cf.GetNestedType("ComponentTypeInfoCacheEntry", BindingFlags.NonPublic)!;
        _createInitializer = cf.GetMethod("CreatePropertyInjector", BindingFlags.NonPublic | BindingFlags.Static)!.CreateDelegate<Func<Type, Action<IServiceProvider, IComponent>>>();
        _original = field.GetValue(null)!;
        _container = container;
    }

    public void UpdateComponentCache(Assembly assembly)
    {
        var components = assembly!.GetExportedTypes().Where(m => typeof(IComponent).IsAssignableFrom(m));
        var resolver = _container.GetProvider(assembly)!;

        foreach (var componentType in components)
        {
            var initializer = _createInitializer(componentType);
            Action<IServiceProvider, IComponent> propInjection = (scope, cmp) =>
            {
                var provider = resolver.Resolve(scope);
                initializer(provider, cmp);
            };
            var entry = Activator.CreateInstance(_ctiType, null, propInjection)!;
            object?[] parameters = [componentType, entry];
            _original.GetType().GetMethod("TryAdd")!.Invoke(_original, parameters);
        }
    }
}
