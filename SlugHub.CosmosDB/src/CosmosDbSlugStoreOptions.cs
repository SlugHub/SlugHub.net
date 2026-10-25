namespace SlugHub.CosmosDB
{
    public class CosmosDbSlugStoreOptions
    {
        public string DatabaseId { get; set; } = "SlugHub";

        public string ContainerId { get; set; } = "Slugs";
    }
}
