using Microsoft.AspNetCore.Components;
using NuGet.Frameworks;
using Piral.Blazor.Shared;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json.Nodes;

namespace Piral.Blazor.Orchestrator;

public abstract class MicrofrontendPackage : IDisposable
{
    private readonly RelatedMfAppService _app;
    private readonly IModuleContainerService _container;
    private readonly IPiralConfig _config;
    private readonly MicrofrontendLoadContext _context;
    public event EventHandler? PackageChanged;

    public MicrofrontendPackage(MfPackageMetadata entry, IPiralConfig config, IModuleContainerService container, IEvents events, IData data)
    {
        _app = new (entry, events, data);
        _config = config;
        _container = container;
        _context = new MicrofrontendLoadContext($"{entry.Name}@{entry.Version}", ResolveAssembly);   
    }

    private IMfModule? _module;
    private bool _disabled = false;
    private Assembly? _assembly;

    public IMfAppService Service => _app;

    public string Name => _app.Name;

    public string Version => _app.Version;

    public JsonObject? Config => _app.Config;

    public bool IsDisabled
    {
        get
        {
            return _disabled;
        }
        set
        {
            if (value != _disabled)
            {
                _disabled = value;
                PackageChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public AssemblyLoadContext Context => _context;

    public IEnumerable<string> Scripts => _app.Scripts;

    public IEnumerable<string> Styles => _app.Styles;

    public IEnumerable<string> ComponentNames => _app.Components.Keys;

    public IEnumerable<(string Name, Type Type)> Components => _app.Components.SelectMany(m => m.Value.Select(type => (m.Key, type)));

    public IEnumerable<string> Dependencies =>
        _assembly?.
        GetReferencedAssemblies().
        Select(m => m.Name ?? "").
        Where(m => !string.IsNullOrEmpty(m)) ?? [];

    public IEnumerable<Type> GetComponents(string name)
    {
        if (name is not null && _app.Components.TryGetValue(name, out var result))
        {
            return result;
        }

        return [];
    }

    public async Task Init()
    {
        await OnInitializing();

        var assembly = GetAssembly();

        if (assembly is not null)
        {
            _assembly = assembly;
            _module = await _container.ConfigureModule(assembly, _app);
        }

        _app.PrependStyleSheet(GetCssName());
        await OnInitialized();
    }

    protected abstract Assembly? ResolveAssembly(string dll);

    protected virtual Task OnInitializing()
    {
        foreach (var assembly in _config.IsolatedAssemblies)
        {
            ResolveAssembly(assembly);
        }

        return Task.CompletedTask;
    }

    protected virtual Task OnInitialized() => Task.CompletedTask;

    protected abstract string GetCssName();

    protected abstract Assembly? GetAssembly();

    protected static bool IsCompatible(NuGetFramework framework) => DefaultCompatibilityProvider.Instance.IsCompatible(Constants.CurrentFramework, framework);

    public async Task Destroy()
    {
        if (_module is not null)
        {
            await _module.Teardown(_app);
        }

        _app.Reset();
    }

    public virtual void Dispose() => Context.Unload();

    public abstract Task<Stream?> GetFile(string path);

    sealed class RelatedMfAppService(MfPackageMetadata entry, IEvents events, IData data) : IMfAppService
    {
        private readonly IEvents _events = events;
        private readonly IData _data = data;
        private readonly MfDetails _meta = new () { Name = entry.Name, Version = entry.Version, Config = entry.Config ?? [] };

        public MfDetails Meta => _meta;

        public Dictionary<string, List<Type>> Components { get; } = [];

        public List<string> Styles { get; } = [];

        public List<string> Scripts { get; } = [];

        public string Name { get; } = entry.Name;

        public string Version { get; } = entry.Version;

        public JsonObject? Config { get; } = entry.Config;

        public void AddEventListener<T>(string type, Action<T> handler)
        {
            _events.AddEventListener(type, handler);
        }

        public void AppendScript(string path)
        {
            Scripts.Add(path);
        }

        public void DispatchEvent<T>(string type, T args)
        {
            _events.DispatchEvent(type, args);
        }

        public void PrependStyleSheet(string path)
        {
            Styles.Add(path);
        }

        public void RemoveEventListener<T>(string type, Action<T> handler)
        {
            _events.RemoveEventListener(type, handler);
        }

        public bool TrySetData<T>(string key, T value)
        {
            return _data.TrySetData(Name, key, value);
        }

        public bool TryGetData<T>(string key, out T value)
        {
            return _data.TryGetData(Name, key, out value);
        }

        public void Reset()
        {
            Components.Clear();
            Styles.Clear();
            Scripts.Clear();
        }

        public void MapComponent<T>(string name)
            where T : class, IComponent
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException($"Components registered in 'MapComponent' need to have a non-empty name. No valid name given for '{nameof(T)}'.");
            }

            var components = Components.GetValueOrDefault(name) ?? [];
            components.Add(typeof(T));
            Components[name] = components;
        }

        public void MapRoute<T>()
            where T : class, IComponent
        {
            var attributes = typeof(T).GetCustomAttributes<RouteAttribute>();

            if (!attributes.Any())
            {
                throw new InvalidOperationException($"Components registered in 'MapRoute' need to have at least one '@route' directive. Missing in '{nameof(T)}'.");
            }

            foreach (var attribute in attributes)
            {
                MapComponent<T>($"route:{attribute.Template}");
            }
        }
    }
}
