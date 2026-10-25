# SlugHub.SqlServer

SQL Server storage for [SlugHub](https://www.nuget.org/packages/SlugHub).

    var slugGenerator = new SlugGenerator(new SqlServerSlugStore(connectionString));
    var slug = await slugGenerator.GenerateSlugAsync("Some text that needs slugging");

The store creates its schema and table on first use (`[SlugHub].[Slugs]` by default). Change them with `SqlServerSlugStoreOptions`:

    new SqlServerSlugStore(connectionString, new SqlServerSlugStoreOptions { TableSchema = "dbo", TableName = "Slugs" });

Uses `Microsoft.Data.SqlClient`, which encrypts connections by default. Add `TrustServerCertificate=True` to the connection string for servers with self-signed certificates.
