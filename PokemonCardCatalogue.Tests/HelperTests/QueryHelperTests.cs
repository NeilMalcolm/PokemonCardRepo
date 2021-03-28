using Microsoft.VisualStudio.TestTools.UnitTesting;
using PokemonCardCatalogue.Common.Helpers;
using PokemonCardCatalogue.Common.Models;
using System.Collections.Generic;

namespace PokemonCardCatalogue.Tests.HelperTests
{
    [TestClass]
    public class QueryHelperTests : BaseTestClass
    {
        private const string OrderByString = "orderBy=";

        public static IEnumerable<object[]> NoOrderByBuildQueryTestData
        {
            get
            {
                yield return new object[]
                {
                    new QueryParameters
                    {
                        Query = new Dictionary<string, string> { { "name", "bulbasaur" } }
                    }
                };
                yield return new object[]
                {
                    new QueryParameters
                    {
                        Query = new Dictionary<string, string> { { "set.id", "swsh4" } }
                    }
                };
            }
        }
        
        public static IEnumerable<object[]> WithOrderByBuildQueryTestData
        {
            get
            {
                yield return new object[]
                {
                    new QueryParameters
                    {
                        Query = new Dictionary<string, string> { { "name", "bulbasaur" } },
                        OrderBy = "number"
                    }
                };
                yield return new object[]
                {
                    new QueryParameters
                    {
                        Query = new Dictionary<string, string> { { "set.id", "swsh4" } },
                        OrderBy = "name"
                    }
                };
            }
        }

        [TestInitialize]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();
        }

        [TestMethod]
        public void WhenBuildQueryIsCalled_AndQueryParametersIsNull_ThenResultIsEmptyString()
        {
            Assert.AreEqual(string.Empty, QueryHelper.BuildQuery(null));
        }

        [DataTestMethod,
            DynamicData(nameof(NoOrderByBuildQueryTestData))]
        public void WhenBuildQueryIsCalled_AndOrderByIsNullOrEmpty_ThenQueryDoesNotHaveOrderBy(QueryParameters parameters)
        {
            var result = QueryHelper.BuildQuery(parameters);
            Assert.IsFalse(result.Contains(OrderByString));
        }

        [DataTestMethod,
            DynamicData(nameof(WithOrderByBuildQueryTestData))]
        public void WhenBuildQueryIsCalled_AndOrderByIsPresent_ThenQueryDoesHaveOrderBy(QueryParameters parameters)
        {
            var result = QueryHelper.BuildQuery(parameters);
            Assert.IsTrue(result.Contains(OrderByString));
        }

        [DataTestMethod,
            DynamicData(nameof(WithOrderByBuildQueryTestData))]
        public void WhenBuildQueryIsCalled_AndParametersAreSet_ThenQueryIsCorrect(QueryParameters parameters)
        {
            var result = QueryHelper.BuildQuery(parameters);

            foreach (var parameter in parameters.Query)
            {
                Assert.IsTrue(result.Contains($"{parameter.Key}:{parameter.Value}"));
            }

            Assert.IsTrue(result.Contains(OrderByString));
        }

        [TestMethod,
            DataRow(new int[] { 12 })]
        public void WhenGetPokedexNumberQueryIsCalled_AndHasSinglePokedexNumber_ThenOutputIsCorrect(int[] numbers)
        {
            var result = QueryHelper.GetPokedexNumberQuery(numbers);
            List<KeyValuePair<string, string>> expectedResult = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "(nationalPokedexNumbers", $"[{numbers[0]} TO {numbers[0]}])"
                )
            };

            for (int i = 0; i < result.Count; i ++)
            {
                Assert.AreEqual(expectedResult[i].Key, result[i].Key);
                Assert.AreEqual(expectedResult[i].Value, result[i].Value);
            }
        }

        [TestMethod,
            DataRow(new int[] { 12, 20 }),
            DataRow(new int[] { 150, 342, 659 }),
            DataRow(new int[] { 1, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 })]
        public void WhenGetPokedexNumberQueryIsCalled_AndHasMultiplePokedexNumbers_ThenOutputIsCorrect(int[] numbers)
        {
            var result = QueryHelper.GetPokedexNumberQuery(numbers);

            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual("(nationalPokedexNumbers", result[i].Key);
                Assert.IsTrue(result[i].Value.Contains($"[{numbers[i]} TO {numbers[i]}]"));
                if (i < result.Count-1)
                {
                    Assert.IsTrue(result[i].Value.EndsWith(" or"));
                }
            }
        }
    }
}
