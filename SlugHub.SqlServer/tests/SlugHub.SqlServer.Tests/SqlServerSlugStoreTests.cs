using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SlugHub.SqlServer.Tests
{
    // Integration tests: set SLUGHUB_SQLSERVER_CONNECTION to a SQL Server connection string to run them
    [TestFixture]
    public class SqlServerSlugStoreTests
    {
        private SqlServerSlugStore _store;

        [SetUp]
        public void SetUp()
        {
            var connectionString = Environment.GetEnvironmentVariable("SLUGHUB_SQLSERVER_CONNECTION");

            if (string.IsNullOrEmpty(connectionString))
                Assert.Ignore("SLUGHUB_SQLSERVER_CONNECTION not set");

            _store = new SqlServerSlugStore(connectionString);
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
