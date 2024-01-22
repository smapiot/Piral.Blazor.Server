using CommandLine;
using Microsoft.Extensions.Configuration;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json;

namespace Piral.Blazor.Cli;

[Verb("prefill-cache", HelpText = "Populates the .cache folder of an application already.")]
public class PrefillCacheOptions : ICommand
{
    [Option('o', "output", Required = false, HelpText = "Sets where the files should be stored - by default '.cache' relative to the current directory.")]
    public string? Output { get; set; }

    [Option('s', "source", Required = false, HelpText = "The path to the source directory containing the 'appsettings.json' file. By default the current directory is used.")]
    public string? Source { get; set; }

    [Option("secrets-id", Required = false, HelpText = "The user secrets id, if user secrets are used in the configuration.")]
    public string? SecretsId { get; set; }

    public async Task Run()
    {
        var source = Path.Combine(Environment.CurrentDirectory, Source ?? "");
        var output = Path.Combine(Environment.CurrentDirectory, Output ?? ".cache");
        var secretsId = SecretsId;
        output.CreateDirectoryIfNotExists();

        var builder = new ConfigurationBuilder()
            .SetBasePath(source)
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json");

        if (secretsId is not null)
        {
            builder.AddUserSecrets(secretsId);
        }

        var config = builder.Build();

        var feeds = config.GetSection("Microfrontends:NugetFeeds").Get<Dictionary<string, NugetFeedConfig>>();
        var feedUrl = config.GetValue<string>("Microfrontends:DiscoveryInfoUrl") ?? "https://feed.piral.cloud/api/v1/pilet/empty";

        var repositories = feeds?.Values.Select(m =>
        {
            var repo = Repository.Factory.GetCoreV3(m.Url);

            if (m.Token is not null)
            {
                repo.PackageSource.Credentials = new PackageSourceCredential(m.Url, m.User, m.Token, true, null);
            }

            return repo;
        }).ToList() ?? Enumerable.Repeat(Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json"), 1);

        var cache = new SourceCacheContext();

        await LoadMicrofrontends(repositories, cache, output, feedUrl);
    }

    private async Task LoadMicrofrontends(IEnumerable<SourceRepository> repositories, SourceCacheContext cache, string cacheDir, string feedUrl)
    {
        if (!string.IsNullOrEmpty(feedUrl))
        {
            var client = new HttpClient();
            var response = await client.GetFromJsonAsync<MfDiscoveryServiceResponse>(feedUrl);

            if (response?.MicroFrontends is not null)
            {
                var mfs = new List<NugetEntry>();

                foreach (var item in response.MicroFrontends)
                {
                    var data = item.Value.FirstOrDefault();
                    var version = data?.Metadata?.Version;

                    if (version is not null)
                    {
                        var mf = new NugetEntry
                        {
                            Name = item.Key,
                            Version = version,
                        };
                        await CollectPackages(repositories, cache, cacheDir, mf);
                        mfs.Add(mf);
                    }
                }

                await StoreMicrofrontendsSnapshot(cacheDir, mfs);
            }
        }
    }

    private async Task CollectPackages(IEnumerable<SourceRepository> repositories, SourceCacheContext cache, string cacheDir, NugetEntry mf)
    {
        var dependencies = await RetrieveDependencies(repositories, cache, mf);

        foreach (var dependency in dependencies)
        {
            var result = await DownloadPackage(repositories, cache, dependency);

            if (result is not null)
            {
                await StorePackageSnapshot(cacheDir, $"{dependency.Name}@{dependency.Version}", result);
            }
        }

        await StoreDependenciesSnapshot(cacheDir, $"{mf.Name}@{mf.Version}", dependencies);
    }

    private static async Task StoreMicrofrontendsSnapshot(string cacheDir, IEnumerable<NugetEntry> microfrontends)
    {
        var fn = $"_.json";
        var target = Path.Combine(cacheDir, fn);
        using var fs = File.OpenWrite(target);
        await JsonSerializer.SerializeAsync(fs, microfrontends.ToList());
    }

    private static async Task StoreDependenciesSnapshot(string cacheDir, string id, IEnumerable<NugetEntry> dependencies)
    {
        var fn = $"{id}.json";
        var target = Path.Combine(cacheDir, fn);
        using var fs = File.OpenWrite(target);
        await JsonSerializer.SerializeAsync(fs, dependencies.ToList());
    }

    private static async Task StorePackageSnapshot(string cacheDir, string id, PackageArchiveReader reader)
    {
        var fn = $"{id}.nupkg";
        var target = Path.Combine(cacheDir, fn);
        await reader.CopyNupkgAsync(target, CancellationToken.None);
    }

    private async Task<PackageArchiveReader?> DownloadPackage(IEnumerable<SourceRepository> repositories, SourceCacheContext cache, NugetEntry package)
    {
        var version = NuGetVersion.Parse(package.Version);

        foreach (var repository in repositories)
        {
            var resource = await repository.GetResourceAsync<FindPackageByIdResource>()!;
            var exists = await resource.DoesPackageExistAsync(package.Name, version, cache, NullLogger.Instance, CancellationToken.None);

            if (exists)
            {
                var stream = new MemoryStream();
                await resource.CopyNupkgToStreamAsync(package.Name, version, stream, cache, NullLogger.Instance, CancellationToken.None);
                return new PackageArchiveReader(stream);
            }
        }
        return null;
    }

    private async Task<IEnumerable<NugetEntry>> RetrieveDependencies(IEnumerable<SourceRepository> repositories, SourceCacheContext cache, NugetEntry package)
    {
        var resolver = new PackageResolver();

        // Find all potential dependencies
        var packages = new ConcurrentDictionary<PackageIdentity, int>(PackageIdentityComparer.Default);

        await ListAllPackageDependencies(repositories, cache, new PackageIdentity(package.Name, NuGetVersion.Parse(package.Version)), packages);

        // Find the best version for each package
        var resolverContext = new PackageResolverContext(
            dependencyBehavior: DependencyBehavior.Lowest,
            targetIds: [package.Name],
            requiredPackageIds: [],
            packagesConfig: [],
            preferredVersions: [],
            availablePackages: packages.Keys.OfType<SourcePackageDependencyInfo>(),
            repositories.Select(r => r.PackageSource),
            NullLogger.Instance);

        return resolver.Resolve(resolverContext, CancellationToken.None).Select(m => new NugetEntry
        {
            Name = m.Id,
            Version = m.Version.ToNormalizedString(),
        });
    }

    private async Task ListAllPackageDependencies(IEnumerable<SourceRepository> repositories, SourceCacheContext cache, PackageIdentity package, ConcurrentDictionary<PackageIdentity, int> dependencies)
    {
        if (!dependencies.TryGetValue(package, out _))
        {
            await Parallel.ForEachAsync(repositories, async (repository, cancellationToken) =>
            {
                var currentFramework = NuGetFramework.Parse("net8.0");
                var dependencyInfoResource = await repository.GetResourceAsync<DependencyInfoResource>();
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(package, currentFramework, cache, NullLogger.Instance, cancellationToken);

                if (dependencyInfo is not null && dependencies.TryAdd(dependencyInfo, 1))
                {
                    await Parallel.ForEachAsync(dependencyInfo.Dependencies, async (dependency, _) =>
                    {
                        var packageInfo = new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion);
                        await ListAllPackageDependencies(repositories, cache, packageInfo, dependencies);
                    });
                }
            });
        }
    }

    class NugetFeedConfig
    {
        public string Url { get; set; } = string.Empty;

        public string? User { get; set; }

        public string? Token { get; set; }
    }

    class NugetEntry
    {
        public string Name { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;
    }
    
    class MfDiscoveryServiceResponse
    {
        public Dictionary<string, List<MfDiscoveryServiceItem>>? MicroFrontends { get; set; }
    }

    class MfDiscoveryServiceItem
    {
        public string? Url { get; set; }

        public MfDiscoveryServiceItemMetadata? Metadata { get; set; }

        public MfDiscoveryServiceItemExtras? Extras { get; set; }
    }

    class MfDiscoveryServiceItemMetadata
    {
        public string? Integrity { get; set; }

        public string? Version { get; set; }
    }

    class MfDiscoveryServiceItemExtras
    {
        public List<string>? Assemblies { get; set; }

        public Dictionary<string, string>? Dependencies { get; set; }
    }
}
