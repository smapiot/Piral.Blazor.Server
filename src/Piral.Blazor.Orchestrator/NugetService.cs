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

namespace Piral.Blazor.Orchestrator;

internal class NugetService : INugetService
{
    private readonly ILogger _logger = NullLogger.Instance;
    private readonly NuGetFramework _currentFramework = NuGetFramework.Parse("net8.0");
    private readonly FrameworkReducer _frameworkReducer = new();

    private readonly SourceCacheContext _cache = new();
    private readonly IEnumerable<SourceRepository> _repositories;

    public NugetService(IConfiguration configuration)
    {
        var feeds = configuration.GetSection("Microfrontends:NugetFeeds").Get<Dictionary<string, NugetFeedConfig>>();
        var feedUrl = configuration.GetValue<string>("Microfrontends:DiscoveryInfoUrl");

        _repositories = feeds?.Values.Select(m =>
        {
            var repo = Repository.Factory.GetCoreV3(m.Url);

            if (m.Token is not null)
            {
                repo.PackageSource.Credentials = new PackageSourceCredential(m.Url, m.User, m.Token, true, null);
            }

            return repo;
        }).ToList() ?? Enumerable.Repeat(Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json"), 1);

        // Special case: The feed URL is actually a NuGet feed.
        if (feedUrl is not null && feedUrl.EndsWith("/index.json"))
        {
            var current = new List<SourceRepository>(_repositories);
            current.Insert(0, Repository.Factory.GetCoreV3(feedUrl));
            _repositories = current;
        }
    }

    public IEnumerable<PackageDependency> ListDependencies(PackageArchiveReader reader)
    {
        var dependencyGroups = reader.GetPackageDependencies();
        var frameworks = dependencyGroups.Select(m => m.TargetFramework);
        var selectedFramework = _frameworkReducer.GetNearest(_currentFramework, frameworks);

        if (selectedFramework is not null)
        {
            var dependencyGroup = dependencyGroups.First(m => m.TargetFramework == selectedFramework);
            return dependencyGroup.Packages;
        }

        return [];
    }

    public async Task<PackageArchiveReader?> DownloadPackage(string packageName, string packageVersion)
    {
        var version = NuGetVersion.Parse(packageVersion);

        foreach (var repository in _repositories)
        {
            var resource = await repository.GetResourceAsync<FindPackageByIdResource>()!;
            var exists = await resource.DoesPackageExistAsync(packageName, version, _cache, _logger, CancellationToken.None);

            if (exists)
            {
                var stream = new MemoryStream();
                await resource.CopyNupkgToStreamAsync(packageName, version, stream, _cache, _logger, CancellationToken.None);
                return new PackageArchiveReader(stream);
            }
        }

        return null;
    }

    public async Task<IEnumerable<NugetEntry>> RetrieveDependencies(string packageName, string packageVersion)
    {
        var resolver = new PackageResolver();

        // Find all potential dependencies
        var packages = new ConcurrentDictionary<PackageIdentity, int>(PackageIdentityComparer.Default);

        await ListAllPackageDependencies(new PackageIdentity(packageName, NuGetVersion.Parse(packageVersion)), packages);

        // Find the best version for each package
        var resolverContext = new PackageResolverContext(
            dependencyBehavior: DependencyBehavior.Lowest,
            targetIds: [packageName],
            requiredPackageIds: [],
            packagesConfig: [],
            preferredVersions: [],
            availablePackages: packages.Keys.OfType<SourcePackageDependencyInfo>(),
            _repositories.Select(r => r.PackageSource),
            _logger);

        return resolver.Resolve(resolverContext, CancellationToken.None).Select(m => new NugetEntry
        {
            Name = m.Id,
            Version = m.Version.ToNormalizedString(),
        });
    }

    private async Task ListAllPackageDependencies(PackageIdentity package, ConcurrentDictionary<PackageIdentity, int> dependencies)
    {
        if (!dependencies.TryGetValue(package, out _))
        {
            await Parallel.ForEachAsync(_repositories, async (repository, cancellationToken) =>
            {
                var dependencyInfoResource = await repository.GetResourceAsync<DependencyInfoResource>();
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(package, _currentFramework, _cache, _logger, cancellationToken);

                if (dependencyInfo is not null && dependencies.TryAdd(dependencyInfo, 1))
                {
                    await Parallel.ForEachAsync(dependencyInfo.Dependencies, async (dependency, _) =>
                    {
                        var packageInfo = new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion);
                        await ListAllPackageDependencies(packageInfo, dependencies);
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
}
