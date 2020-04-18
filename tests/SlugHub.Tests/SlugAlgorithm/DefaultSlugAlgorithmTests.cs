using System;
using FakeItEasy;
using NUnit.Framework;
using SlugHub.SlugAlgorithm;

namespace SlugHub.Tests.SlugAlgorithm
{
    [TestFixture]
    public class DefaultSlugAlgorithmTests
    {
        private DefaultSlugAlgorithm _defaultSlugAlgorithm;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _defaultSlugAlgorithm = new DefaultSlugAlgorithm();
        }

        [Test]
        public void GenerateSlug_replaces_spaces_with_hyphens()
        {
            var result = _defaultSlugAlgorithm.Slug("some text");

            Assert.That(result, Is.EqualTo("some-text"));
        }

        [Test]
        public void GenerateSlug_removes_special_characters()
        {
            var result = _defaultSlugAlgorithm.Slug("sometext@!#$%^&");

            Assert.That(result, Is.EqualTo("sometext"));
        }

        public void GenerateSlug_replaces_accented_characters()
        {
            var result = _defaultSlugAlgorithm.Slug("this téxt is àçé");

            Assert.That(result, Is.EqualTo("this-text-is-ace"));
        }

        [Test]
        public void GenerateSlug_returns_in_lowercase()
        {
            var result = _defaultSlugAlgorithm.Slug("SOMETEXT");

            Assert.That(result, Is.EqualTo("sometext"));
        }

        [Test]
        public void GenerateSlug_WithOptions_applies_manipulators_in_order()
        {
            Func<string, string> func1 = s => s + "a";
            Func<string, string> func2 = s => s + "b";
            Func<string, string> func3 = s => s + "c";

            var result = _defaultSlugAlgorithm.Slug("input", new SlugAlgorithmOptions(func1, func2, func3));

            Assert.AreEqual("inputabc", result);
        }
    }
}