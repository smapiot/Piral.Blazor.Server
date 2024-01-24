using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System.Reflection;

namespace Piral.Blazor.Orchestrator;

internal class CacheManipulatorService : ICacheManipulatorService
{
    private readonly Type _ctiType;
    private readonly Func<Type, Action<IServiceProvider, IComponent>> _createInitializer;
    private readonly FieldInfo _field;
    private readonly MethodInfo _tryGetValue;
    private readonly MethodInfo _tryRemove;
    private readonly MethodInfo _tryAdd;

    public CacheManipulatorService()
    {
        var cf = typeof(Renderer).Assembly.GetType("Microsoft.AspNetCore.Components.ComponentFactory")!;
        _field = cf.GetField("_cachedComponentTypeInfo", BindingFlags.Static | BindingFlags.NonPublic)!;
        _ctiType = cf.GetNestedType("ComponentTypeInfoCacheEntry", BindingFlags.NonPublic)!;
        _createInitializer = cf.GetMethod("CreatePropertyInjector", BindingFlags.NonPublic | BindingFlags.Static)!.CreateDelegate<Func<Type, Action<IServiceProvider, IComponent>>>();
        _tryGetValue = _field.FieldType.GetMethod("TryGetValue")!;
        _tryRemove = _field.FieldType.GetMethods().Where(m => m.Name == "TryRemove").First()!;
        _tryAdd = _field.FieldType.GetMethod("TryAdd")!;
    }

    public void UpdateComponentCache(Type componentType, IScopeResolver resolver)
    {
        var original = _field.GetValue(null)!;
        var initializer = _createInitializer(componentType);
        Action<IServiceProvider, IComponent> propInjection = (scope, cmp) =>
        {
            var provider = resolver.Resolve(scope);
            initializer(provider, cmp);
        };
        object?[] parameters = [componentType, null];
        _tryGetValue.Invoke(original, parameters);
        _tryRemove.Invoke(original, parameters);
        parameters[1] = Activator.CreateInstance(_ctiType, null, propInjection)!;
        _tryAdd.Invoke(original, parameters);
    }
}
