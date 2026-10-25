using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using NUnit.Framework;

namespace SlugHub.CosmosDB.Tests
{
    // Integration tests: set SLUGHUB_COSMOSDB_CONNECTION to a Cosmos DB connection string (the emulator works) to run them
    [TestFixture]
    public class CosmosDbSlugStoreTests
    {
        private CosmosDbSlugStore _store;

        private CosmosClient _client;

        [SetUp]
        public async Task SetUp()
        {
            var connectionString = Environment.GetEnvironmentVariable("SLUGHUB_COSMOSDB_CONNECTION");

            if (string.IsNullOrEmpty(connectionString))
                Assert.Ignore("SLUGHUB_COSMOSDB_CONNECTION not set");

            // the emulator uses a self-signed certificate
            _client = new CosmosClient(connectionString, new CosmosClientOptions
            {
                HttpClientFactory = () => new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = System.Net.Http.HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }),
                ConnectionMode = ConnectionMode.Gateway,
                LimitToEndpoint = true
            });

            _store = await CosmosDbSlugStore.CreateAsync(_client);
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
        }

        [Test]
        public async Task ExistsAsync_returns_false_for_unknown_slug()
        {
            Assert.That(await _store.ExistsAsync(Guid.NewGuid().ToString("N"), null), Is.False);
        }

        [Test]
        public async Task ExistsAsync_returns_true_after_StoreAsync()
        {
            var value = Guid.NewGuid().ToString("N");

            await _store.StoreAsync(new Slug(value, "group1"));

            Assert.That(await _store.ExistsAsync(value, "group1"), Is.True);
            Assert.That(await _store.ExistsAsync(value, "group2"), Is.False);
        }

        [Test]
        public async Task SlugGenerator_iterates_when_slug_is_taken()
        {
            var text = "Some text " + Guid.NewGuid().ToString("N");
            var slugGenerator = new SlugGenerator(_store);

            var slug1 = await slugGenerator.GenerateSlugAsync(text);
            var slug2 = await slugGenerator.GenerateSlugAsync(text);

            Assert.That(slug2, Is.EqualTo(slug1 + "-1"));
        }
    }
}
