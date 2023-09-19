﻿using NuGet.Packaging;

namespace Piral.Blazor.Orchestrator;

internal class MfPackageService : IMfPackageService
{
    private readonly IModuleContainerService _container;
    private readonly ISnapshotService _snapshot;
    private readonly IEvents _events;

    public MfPackageService(IModuleContainerService container, ISnapshotService snapshot, IEvents events)
    {
        _container = container;
        _snapshot = snapshot;
        _events = events;
    }

    public async Task<MicrofrontendPackage> LoadMicrofrontend(string name, string version)
    {
        var packages = await CollectPackages(name, version);
        return new NugetMicrofrontendPackage(name, version, packages, _container, _events);
    }

    private async Task<List<PackageArchiveReader>> CollectPackages(string name, string version)
    {
        var mf = new NugetEntry
        {
            Name = name,
            Version = version,
        };
        var id = mf.MakePackageId();
        var dependencies = await _snapshot.ListDependencies(id);
        var result = new List<PackageArchiveReader>();

        foreach (var dependency in dependencies)
        {
            var depId = dependency.MakePackageId();
            var reader = await _snapshot.LoadPackage(depId);

            if (reader is not null)
            {
                result.Add(reader);
            }
        }

        return result;
    }
}

