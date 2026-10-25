# SlugHub.net

Highly configurable URL slug generator for .NET, with pluggable persistent storage.

| Package | Folder | Description |
|---|---|---|
| [SlugHub](https://www.nuget.org/packages/SlugHub) | [`SlugHub/`](SlugHub/) | Slug generator with in-memory storage |
| [SlugHub.SqlServer](https://www.nuget.org/packages/SlugHub.SqlServer) | [`SlugHub.SqlServer/`](SlugHub.SqlServer/) | SQL Server slug store |
| [SlugHub.CosmosDB](https://www.nuget.org/packages/SlugHub.CosmosDB) | [`SlugHub.CosmosDB/`](SlugHub.CosmosDB/) | Azure Cosmos DB slug store |

All packages target `net8.0` and `net10.0`.

## Building

    dotnet build SlugHub.slnx
    dotnet test SlugHub.slnx

The SqlServer and CosmosDB tests are integration tests. They are skipped unless `SLUGHUB_SQLSERVER_CONNECTION` / `SLUGHUB_COSMOSDB_CONNECTION` are set.

## Releasing

Each package has its own GitHub Actions workflow, which only runs when that package (or something it depends on) changes. Versions live in each project's `VersionPrefix`.

To release, bump `VersionPrefix`, merge, then push a tag named `<PackageId>-v<version>`:

    git tag SlugHub-v3.0.0 && git push origin SlugHub-v3.0.0
    git tag SlugHub.SqlServer-v1.0.0 && git push origin SlugHub.SqlServer-v1.0.0

The workflow checks the tag matches the project version and publishes to NuGet.org with [trusted publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing), so no API key is stored. Each workflow file (`slughub.yml`, `slughub-sqlserver.yml`, `slughub-cosmosdb.yml`) needs a matching policy on nuget.org (environment `nuget`), and the `NUGET_USER` repository variable holds the nuget.org username. Release `SlugHub` before the store packages that depend on a new version of it.

## History

`SlugHub.SqlServer` and `SlugHub.CosmosDB` used to live in separate repositories (`SlugHub.net.SqlServer`, `SlugHub.net.CosmosDB`). Their full history was merged into this repository.
