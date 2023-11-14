using Microsoft.AspNetCore.Components;
using Piral.Blazor.Shared;
using System.Reflection;

namespace Piral.Blazor.Orchestrator;

public abstract class MicrofrontendPackage : IDisposable
{
    private readonly RelatedMfAppService _app;
    private readonly IModuleContainerService _container;
    public event EventHandler? PackageChanged;

    private IMfModule? _module;
    private bool _disabled;

    public MicrofrontendPackage(string name, string version, IModuleContainerService container, IEvents events)
    {
        _app = new RelatedMfAppService(name, version, events);
        _container = container;
        _disabled = false;
    }

    public string Name => _app.Name;

    public string Version => _app.Version;

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

    public IEnumerable<string> Scripts => _app.Scripts;

    public IEnumerable<string> Styles => _app.Styles;

    public IEnumerable<string> ComponentNames => _app.Components.Keys;

    public IEnumerable<(string Name, Type Type)> Components => _app.Components.SelectMany(m => m.Value.Select(type => (m.Key, type)));

    public IEnumerable<string> Dependencies =>
        GetAssembly()?.
        GetReferencedAssemblies().
        Select(m => m.Name ?? "").
        Where(m => !string.IsNullOrEmpty(m)) ?? Enumerable.Empty<string>();

    public IEnumerable<Type> GetComponents(string name)
    {
        if (_app.Components.TryGetValue(name, out var result))
        {
            return result;
        }

        return Enumerable.Empty<Type>();
    }

    public async Task Init()
    {
        var assembly = GetAssembly();

        if (assembly is not null)
        {
            _module = await _container.ConfigureModule(assembly, _app);
        }

        _app.PrependStyleSheet(GetCssName());
        await OnInitialized();
    }

    protected virtual Task OnInitialized() => Task.CompletedTask;

    protected abstract string GetCssName();

    protected abstract Assembly? GetAssembly();

    public async Task Destroy()
    {
        if (_module is not null)
        {
            await _module.Teardown(_app);
        }

        _app.Reset();
    }

    public virtual void Dispose()
    {
        // Empty on purpose
    }

    public abstract Stream? GetFile(string path);

    sealed class RelatedMfAppService : IMfAppService
    {
        private readonly IEvents _events;

        public Dictionary<string, List<Type>> Components { get; } = new();

        public List<string> Styles { get; } = new();

        public List<string> Scripts { get; } = new();

        public string Name { get; }

        public string Version { get; }

        public RelatedMfAppService(string name, string version, IEvents events)
        {
            Name = name;
            Version = version;
            _events = events;
        }

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

            var components = Components.GetValueOrDefault(name) ?? new List<Type>();
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
