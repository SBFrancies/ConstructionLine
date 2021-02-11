using System;
using System.Collections.Generic;
using System.Linq;
using ConstructionLine.CodingChallenge.Tests.SampleData;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class SearchEngineTests : SearchEngineTestsBase
    {
        [Test]
        public void Test()
        {
            var shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Blue),
            };

            var searchEngine = new SearchEngine(shirts);

            var searchOptions = new SearchOptions
            {
                Colors = new List<Color> {Color.Red},
                Sizes = new List<Size> {Size.Small}
            };

            var results = searchEngine.Search(searchOptions);

            AssertResults(results.Shirts, searchOptions);
            AssertSizeCounts(shirts, searchOptions, results.SizeCounts);
            AssertColorCounts(shirts, searchOptions, results.ColorCounts);
        }

        [Test]
        public void SearchEngine_Search_NullOptionsThrowsError()
        {
            SearchOptions options = null;
            var sut = this.CreateSystemUnderTest();

            Assert.Throws<ArgumentNullException>(() => sut.Search(options));
        }

        [Test]
        public void SearchEngine_Search_NoCriteriaReturnsAllShirts()
        {
            var options = new SearchOptions();
            var shirts = GetShirts();

            var sut = CreateSystemUnderTest(shirts);

            var result = sut.Search(options);

            Assert.AreEqual(shirts.Count, result.Shirts.Count);
            AssertResults(result.Shirts, options);
            AssertSizeCounts(shirts, options, result.SizeCounts);
            AssertColorCounts(shirts, options, result.ColorCounts);
        }

        [Test]
        public void SearchEngine_Search_NoSizeCriteriaReturnOnlyFiltersByColour()
        {
            var options = new SearchOptions
            {
                Colors = Color.All,
            };

            var shirts = GetShirts();

            var sut = CreateSystemUnderTest(shirts);

            var result = sut.Search(options);

            AssertResults(result.Shirts, options);
            AssertSizeCounts(shirts, options, result.SizeCounts);
            AssertColorCounts(shirts, options, result.ColorCounts);
        }

        [Test]
        public void SearchEngine_Search_NoColourCriteriaReturnOnlyFiltersBySize()
        {
            var options = new SearchOptions
            {
                Sizes = Size.All,
            };

            var shirts = GetShirts();

            var sut = CreateSystemUnderTest(shirts);

            var result = sut.Search(options);

            AssertResults(result.Shirts, options);
            AssertSizeCounts(shirts, options, result.SizeCounts);
            AssertColorCounts(shirts, options, result.ColorCounts);
        }

        [Test]
        public void SearchEngine_Search_OneSizeCriteriaAllThatSize()
        {
            var options = new SearchOptions
            {
                Sizes = new List<Size> { Size.Large },
            };

            var shirts = GetShirts(1000);

            var sut = CreateSystemUnderTest(shirts);

            var result = sut.Search(options);

            Assert.True(result.Shirts.TrueForAll(a => a.Size == Size.Large));
            AssertResults(result.Shirts, options);
            AssertSizeCounts(shirts, options, result.SizeCounts);
            AssertColorCounts(shirts, options, result.ColorCounts);
        }

        [Test]
        public void SearchEngine_Search_OneColourCriteriaAllThatColour()
        {
            var options = new SearchOptions
            {
                Colors = new List<Color> { Color.Black },
            };

            var shirts = GetShirts(1000);

            var sut = CreateSystemUnderTest(shirts);

            var result = sut.Search(options);

            Assert.True(result.Shirts.TrueForAll(a => a.Color == Color.Black));
            AssertResults(result.Shirts, options);
            AssertSizeCounts(shirts, options, result.SizeCounts);
            AssertColorCounts(shirts, options, result.ColorCounts);
        }

        [Test]
        public void SearchEngine_Search_ColourAndSizeCriteriaMatch()
        {
            var options = new SearchOptions
            {
                Colors = new List<Color> { Color.Black },
                Sizes = new List<Size> { Size.Large},
            };

            var shirts = GetShirts(1000);

            var sut = CreateSystemUnderTest(shirts);

            var result = sut.Search(options);

            Assert.True(result.Shirts.TrueForAll(a => a.Color == Color.Black));
            Assert.True(result.Shirts.TrueForAll(a => a.Size == Size.Large));
            AssertResults(result.Shirts, options);
            AssertSizeCounts(shirts, options, result.SizeCounts);
            AssertColorCounts(shirts, options, result.ColorCounts);
        }

        [Test]
        public void SearchEngine_Search_NoMatchsReturnsNoShirts()
        {
            var options = new SearchOptions
            {
                Colors = new List<Color> { Color.Black },
            };

            var shirts = GetShirts(1000);
            shirts.ForEach(a => a.Color = Color.Red);

            var sut = CreateSystemUnderTest(shirts);

            var result = sut.Search(options);

            Assert.False(result.Shirts.Any());
            AssertResults(result.Shirts, options);
            AssertSizeCounts(shirts, options, result.SizeCounts);
            AssertColorCounts(shirts, options, result.ColorCounts);
        }

        private List<Shirt> GetShirts(int count = 10)
        {
            var dataBuilder = new SampleDataBuilder(count);
            return dataBuilder.CreateShirts();
        }

        private SearchEngine CreateSystemUnderTest(List<Shirt> shirts = null)
        {
            return new SearchEngine(shirts ?? GetShirts());
        }
    }
}
