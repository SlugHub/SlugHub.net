using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using SlugHub.SlugAlgorithm;
using SlugHub.SlugStore;

namespace SlugHub.Tests
{
    public class SlugGenerator_UsingOptions_Tests
    {
        private ISlugStore _fakeSlugStore;
        private ISlugAlgorithm _fakeSlugAlgorithm;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _fakeSlugStore = A.Fake<ISlugStore>();
            _fakeSlugAlgorithm = A.Fake<ISlugAlgorithm>();

        }

        [Test]
        [TestCase(null)]
        [TestCase("group1")]
        public async Task GenerateSlug_Appends_number_to_text_if_original_text_slugged_exists_Starting_at_supplied_seed_value(string groupingKey)
        {
            //Arrange
            var slugGenerator = new SlugGenerator(new SlugGeneratorOptions { IterationSeedValue = 3 }, _fakeSlugStore, _fakeSlugAlgorithm); //start at 3

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text"))
                .Returns("some-text");

            A.CallTo(() => _fakeSlugStore.ExistsAsync("some-text", groupingKey))
                .Returns(Task.FromResult(true));

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text 3"))
                .Returns("some-text-3");

            //Act
            var result = await slugGenerator.GenerateSlugAsync("Some text");

            //Assert
            Assert.That(result, Is.EqualTo("some-text-3"));

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text 1"))
                .MustNotHaveHappened();

            A.CallTo(() => _fakeSlugAlgorithm.Slug("Some text 2"))
                .MustNotHaveHappened();
        }
    }
}