using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Piral.Blazor.Orchestrator;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMicrofrontends(this IApplicationBuilder app)
    {
        RewireBlazorToHandleLocalScopedMicrofrontendComponents();
        return app.UseMiddleware<MicrofrontendsMiddleware>();
    }

    #region Evil Reflection Hack

    private static readonly Type cf = typeof(Renderer).Assembly.GetType("Microsoft.AspNetCore.Components.ComponentFactory")!;
    private static readonly FieldInfo field = cf.GetField("_cachedComponentTypeInfo", BindingFlags.Static | BindingFlags.NonPublic)!;
    private static readonly Type ctiType = cf.GetNestedType("ComponentTypeInfoCacheEntry", BindingFlags.NonPublic)!;
    private static readonly Func<Type, Action<IServiceProvider, IComponent>> createInitializer = cf.GetMethod("CreatePropertyInjector", BindingFlags.NonPublic | BindingFlags.Static)!.CreateDelegate<Func<Type, Action<IServiceProvider, IComponent>>>();
    private static readonly MethodInfo tryGetValue = field.FieldType.GetMethod("TryGetValue")!;
    private static readonly MethodInfo tryRemove = field.FieldType.GetMethods().Where(m => m.Name == "TryRemove").First()!;
    private static readonly MethodInfo tryAdd = field.FieldType.GetMethod("TryAdd")!;

    private static void RewireBlazorToHandleLocalScopedMicrofrontendComponents()
    {
        var methodToReplace = cf.GetMethod("GetComponentTypeInfo", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)!;
        var methodToInject = typeof(ApplicationBuilderExtensions).GetMethod("OverwriteGetComponentTypeInfo", BindingFlags.Static | BindingFlags.NonPublic)!;

        RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
        RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);

        SwapImplementations(methodToInject, methodToReplace);
    }

    private static void SwapImplementations(MethodInfo methodToInject, MethodInfo methodToReplace)
    {
        unsafe
        {
            if (IntPtr.Size == 4)
            {
                int* inj = (int*)methodToInject.MethodHandle.Value.ToPointer() + 2;
                int* tar = (int*)methodToReplace.MethodHandle.Value.ToPointer() + 2;

                #if DEBUG
                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;
                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);
                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
                #else
                    *tar = *inj;
                #endif
            }
            else
            {
                long* inj = (long*)methodToInject.MethodHandle.Value.ToPointer() + 1;
                long* tar = (long*)methodToReplace.MethodHandle.Value.ToPointer() + 1;

                #if DEBUG
                    byte* injInst = (byte*)*inj;
                    byte* tarInst = (byte*)*tar;
                    int* injSrc = (int*)(injInst + 1);
                    int* tarSrc = (int*)(tarInst + 1);
                    *tarSrc = (((int)injInst + 5) + *injSrc) - ((int)tarInst + 5);
                #else
                    *tar = *inj;
                #endif
            }
        }
    }

    private static object? OverwriteGetComponentTypeInfo(Type componentType)
    {
        var original = field.GetValue(null)!;
        object?[] parameters = [componentType, null];
        
        if ((bool)tryGetValue.Invoke(original, parameters)! == false)
        {
            var componentTypeRenderMode = componentType.GetCustomAttribute<RenderModeAttribute>()?.Mode;
            var initializer = createInitializer(componentType);
            var origin = componentType.Assembly;

            Action<IServiceProvider, IComponent> propInjection = (scope, cmp) =>
            {
                var container = scope.GetService<IModuleContainerService>()!;
                var mfProvider = container.GetProvider(origin);

                // local DI is found - use this one instead
                if (mfProvider is not null)
                {
                    var scopeProvider = mfProvider.Resolve(scope);
                    initializer(scopeProvider, cmp);
                }
                else
                {
                    initializer(scope, cmp);
                }
            };

            parameters[1] = Activator.CreateInstance(ctiType, [componentTypeRenderMode, propInjection])!;
            tryAdd.Invoke(original, parameters);
        }

        return parameters[1];
    }

    #endregion
}
