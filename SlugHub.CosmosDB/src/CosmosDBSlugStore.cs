using System;
using SlugHub.SlugStore;

namespace SlugHub.CosmosDB
{
    public class CosmosDbSlugStore : ISlugStore
    {
        private string _keyPrefix;

        public CosmosDbSlugStore(string keyPrefix)
        {
            _keyPrefix = keyPrefix;
        }

        public bool Exists(string slug, string groupingKey)
        {
            throw new NotImplementedException();
        }

        public void Store(Slug slug)
        {
            throw new NotImplementedException();
        }
    }
}