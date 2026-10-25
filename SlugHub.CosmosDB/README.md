# SlugHub.CosmosDB

Azure Cosmos DB storage for [SlugHub](https://www.nuget.org/packages/SlugHub).

    using var client = new CosmosClient(connectionString);
    var store = await CosmosDbSlugStore.CreateAsync(client);
    var slugGenerator = new SlugGenerator(store);

`CreateAsync` creates the database and container if needed (`SlugHub` / `Slugs` by default, change them with `CosmosDbSlugStoreOptions`). To use an existing container, pass it to `new CosmosDbSlugStore(container)`. It must be partitioned on `/groupingKey`.
