using System.Linq;
using System.Threading.Tasks;
using SlugHub.SlugAlgorithm;
using SlugHub.SlugStore;

namespace SlugHub
{
    public class SlugGenerator : ISlugGenerator
    {
        private readonly ISlugStore _slugStore;
        private readonly ISlugAlgorithm _slugAlgorithm;
        private readonly SlugGeneratorOptions _slugGeneratorOptions;

        public SlugGenerator()
            : this(new SlugGeneratorOptions(), new InMemorySlugStore(), new DefaultSlugAlgorithm())
        { }

        public SlugGenerator(SlugGeneratorOptions slugGeneratorOptions)
            : this(slugGeneratorOptions, new InMemorySlugStore(), new DefaultSlugAlgorithm())
        { }

        public SlugGenerator(SlugGeneratorOptions slugGeneratorOptions, ISlugStore slugStore)
            : this(slugGeneratorOptions, slugStore, new DefaultSlugAlgorithm())
        { }

        public SlugGenerator(SlugGeneratorOptions slugGeneratorOptions, ISlugAlgorithm slugAlgorithm)
            : this(slugGeneratorOptions, new InMemorySlugStore(), slugAlgorithm)
        { }

        public SlugGenerator(ISlugStore slugStore)
            : this(slugStore, new DefaultSlugAlgorithm())
        { }

        public SlugGenerator(ISlugStore slugStore, ISlugAlgorithm slugAlgorithm)
            : this(new SlugGeneratorOptions(), slugStore, slugAlgorithm)
        { }

        public SlugGenerator(SlugGeneratorOptions slugGeneratorOptions, ISlugStore slugStore, ISlugAlgorithm slugAlgorithm)
        {
            _slugStore = slugStore;
            _slugAlgorithm = slugAlgorithm;
            _slugGeneratorOptions = slugGeneratorOptions;
        }

        public Task<string> GenerateSlugAsync(string text, string[] uniquifiers = null)
        {
            return GenerateSlugAsync(text, null, uniquifiers);
        }

        public async Task<string> GenerateSlugAsync(string text, string groupingKey, string[] uniquifiers = null)
        {
            var initialSlug = _slugAlgorithm.Slug(text);

            //we might get lucky on first try
            if (!await _slugStore.ExistsAsync(initialSlug, groupingKey))
            {
                await _slugStore.StoreAsync(new Slug(initialSlug, groupingKey));
                return initialSlug;
            }

            //if we've got uniquifiers, iterate them
            if (uniquifiers != null && uniquifiers.Any())
            {
                string slugWithUniquifier = null;

                var uniquifierIterationIndex = 0;

                while (string.IsNullOrEmpty(slugWithUniquifier) && uniquifierIterationIndex < uniquifiers.Length)
                {
                    var uniquifier = uniquifiers.ElementAt(uniquifierIterationIndex);

                    slugWithUniquifier = _slugAlgorithm.Slug(text + " " + uniquifier);

                    if (await _slugStore.ExistsAsync(slugWithUniquifier, groupingKey))
                        slugWithUniquifier = null;

                    uniquifierIterationIndex++;
                }

                //we couldn't generate a unique slug with any of the uniquifiers supplied
                //so now use the first uniquifier in the list, and append an incremental number until we find the unique
                if (string.IsNullOrEmpty(slugWithUniquifier))
                    return await GenerateAndStoreSlugWithIncrementedNumberAppendage(text + " " + uniquifiers.First(), groupingKey);

                await _slugStore.StoreAsync(new Slug(slugWithUniquifier, groupingKey));

                return slugWithUniquifier;
            }
            //no uniquifiers so add number on the end until we find a unique value
            return await GenerateAndStoreSlugWithIncrementedNumberAppendage(text, groupingKey);
        }

        private async Task<string> GenerateAndStoreSlugWithIncrementedNumberAppendage(string text, string groupingKey)
        {
            // this could be a bit slow if there's a load of slugs with the same precident:
            //slug-1
            //slug-2
            //slug-3
            //etc.... 

            string slugWithNumber = null;
            var number = _slugGeneratorOptions.IterationSeedValue ?? 1;

            while (slugWithNumber == null)
            {
                slugWithNumber = _slugAlgorithm.Slug(text + " " + number);

                if (await _slugStore.ExistsAsync(slugWithNumber, groupingKey))
                    slugWithNumber = null;

                number++;
            }

            await _slugStore.StoreAsync(new Slug(slugWithNumber, groupingKey));

            return slugWithNumber;
        }
    }
}