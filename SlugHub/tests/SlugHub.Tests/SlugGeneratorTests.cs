using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using SlugHub.SlugAlgorithm;
using SlugHub.SlugStore;

namespace SlugHub.Tests
{
    [TestFixture]
    public class SlugGeneratorTests
    {
        private ISlugStore _fakeSlugStore;
        private ISlugAlgorithm _fakeSlugAlgorithm;

        private SlugGenerator _slugGenerator;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _fakeSlugStore = A.Fake<ISlugStore>();
            _fakeSlugAlgorithm = A.Fake<ISlugAlgorithm>();

            _slugGenerator = new SlugGenerator(_fakeSlugStore, _fakeSlugAlgorithm);
        }

        [Test]
        public async Task GenerateSlug_Slugs_given_text_initially()
        {
            var result = await _slugGenerator.GenerateSlugAsync("Some text");

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .MustHaveHappened();
        }

        [Test]
        [TestCase(null)]
        [TestCase("group1")]
        public async Task GenerateSlug_Checks_SlugStore_for_existing_slug(string groupingKey)
        {
            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .Returns("some-text");

            var result = await _slugGenerator.GenerateSlugAsync("Some text", groupingKey);

            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text", groupingKey))
            .MustHaveHappened();
        }

        [Test]
        [TestCase(null)]
        [TestCase("group1")]
        public async Task GenerateSlug_Stores_slug_and_returns_if_it_does_not_already_exist(string groupKey)
        {
            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .Returns("some-text");

            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text", groupKey))
                .Returns(false);

            var result = await _slugGenerator.GenerateSlugAsync("Some text");

            A.CallTo(() => _fakeSlugStore.StoreAsync(A<Slug>.That.Matches(x => x.Value == "some-text")))
                .MustHaveHappened();

            Assert.That(result, Is.EqualTo("some-text"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("group1")]
        public async Task GenerateSlug_Adds_uniquifier_and_re_checks_existence_before_returning(string groupKey)
        {
            //first
            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text", groupKey))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .Returns("some-text");

            //second
            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text-unique", groupKey))
                .Returns(false);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text unique"))
                .Returns("some-text-unique");

            var result = await _slugGenerator.GenerateSlugAsync("Some text", new[] { "unique" });

            A.CallTo(() => _fakeSlugStore.StoreAsync(A<Slug>.That.Matches(x => x.Value == "some-text-unique")))
                .MustHaveHappened();

            Assert.That(result, Is.EqualTo("some-text-unique"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("group1")]
        public async Task GenerateSlug_Iterates_uniquifiers_if_first_one_appended_to_slug_still_exists(string groupKey)
        {
            //first
            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text", groupKey))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .Returns("some-text");

            //second
            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text-uniqueone", groupKey))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text uniqueone"))
                .Returns("some-text-uniqueone");

            //third
            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text-uniquetwo", groupKey))
                .Returns(false);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text uniquetwo"))
                .Returns("some-text-uniquetwo");

            var result = await _slugGenerator.GenerateSlugAsync("Some text", new[] { "uniqueone", "uniquetwo" });

            A.CallTo(() => _fakeSlugStore.StoreAsync(A<Slug>.That.Matches(x => x.Value == "some-text-uniquetwo")))
                .MustHaveHappened();

            Assert.That(result, Is.EqualTo("some-text-uniquetwo"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("group1")]
        public async Task GenerateSlug_Appends_number_to_text_if_original_text_slugged_exists_without_supplying_uniquifiers(string groupKey)
        {
            //first
            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text", groupKey))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .Returns("some-text");

            //second
            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text-1", groupKey))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text 1"))
                .Returns("some-text-1");

            //third
            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text-2", groupKey))
                .Returns(false);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text 2"))
                .Returns("some-text-2");

            var result = await _slugGenerator.GenerateSlugAsync("Some text");

            A.CallTo(() => _fakeSlugStore.StoreAsync(A<Slug>.That.Matches(x => x.Value == "some-text-2")))
                .MustHaveHappened();

            Assert.That(result, Is.EqualTo("some-text-2"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("group1")]
        public async Task GenerateSlug_Appends_number_to_text_and_uniquifier_if_slugged_exists(string groupKey)
        {
            //first
            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text", null))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .Returns("some-text");

            //second
            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text-uniqueone", groupKey))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text uniqueone"))
                .Returns("some-text-uniqueone");

            //third
            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text-uniquetwo", groupKey))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text uniquetwo"))
                .Returns("some-text-uniquetwo");

            //fourth
            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text-uniqueone-1", groupKey))
                .Returns(true);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text uniqueone 1"))
                .Returns("some-text-uniqueone-1");

            //fifth
            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text-uniqueone-2", groupKey))
                .Returns(false);

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text uniqueone 2"))
                .Returns("some-text-uniqueone-2");

            var result = await _slugGenerator.GenerateSlugAsync("Some text", new[] { "uniqueone", "uniquetwo" });

            A.CallTo(() => _fakeSlugStore.StoreAsync(A<Slug>.That.Matches(x => x.Value == "some-text-uniqueone-2")))
                .MustHaveHappened();

            Assert.That(result, Is.EqualTo("some-text-uniqueone-2"));
        }
    }
}
