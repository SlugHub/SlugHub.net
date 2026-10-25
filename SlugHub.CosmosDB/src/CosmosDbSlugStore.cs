using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using SlugHub.SlugStore;

namespace SlugHub.CosmosDB
{
    public class CosmosDbSlugStore : ISlugStore
    {
        private const string DefaultGroupingKey = "Default";
        private readonly Container _container;

        /// <summary>
        /// Uses an existing container, which must be partitioned on <c>/groupingKey</c>
        /// </summary>
        public CosmosDbSlugStore(Container container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <summary>
        /// Creates the database and container (partitioned on <c>/groupingKey</c>) if they don't exist
        /// </summary>
        public static async Task<CosmosDbSlugStore> CreateAsync(CosmosClient client, CosmosDbSlugStoreOptions options = null)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            options = options ?? new CosmosDbSlugStoreOptions();

            Database database = await client.CreateDatabaseIfNotExistsAsync(options.DatabaseId);
            Container container = await database.CreateContainerIfNotExistsAsync(options.ContainerId, "/groupingKey");

            return new CosmosDbSlugStore(container);
        }

        public async Task<bool> ExistsAsync(string slug, string groupingKey)
        {
            using (var response = await _container.ReadItemStreamAsync(ToId(slug), ToPartitionKey(groupingKey)))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    return false;

                response.EnsureSuccessStatusCode();
                return true;
            }
        }

        public async Task StoreAsync(Slug slug)
        {
            var groupingKey = string.IsNullOrEmpty(slug.GroupingKey)
                ? DefaultGroupingKey
                : slug.GroupingKey;

            var document = new SlugDocument
            {
                Id = ToId(slug.Value),
                Slug = slug.Value,
                GroupingKey = groupingKey,
                Created = slug.Created
            };

            await _container.CreateItemAsync(document, new PartitionKey(groupingKey));
        }

        // cosmos ids can't contain / \ ? or #
        private static string ToId(string slug) => Uri.EscapeDataString(slug);

        private static PartitionKey ToPartitionKey(string groupingKey) =>
            new PartitionKey(string.IsNullOrEmpty(groupingKey) ? DefaultGroupingKey : groupingKey);

        private class SlugDocument
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("slug")]
            public string Slug { get; set; }

            [JsonProperty("groupingKey")]
            public string GroupingKey { get; set; }

            [JsonProperty("created")]
            public DateTime Created { get; set; }
        }
    }
}
