using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Logic;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using PokemonCardCatalogue.Constants;
using PokemonCardCatalogue.Models.Collection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Tests.LogicTests
{
    [TestClass]
    public class AllSetsLogicTests : BaseTestClass
    {
        protected Mock<ICardCollection> CardCollectionMock;
        protected Mock<IApi> ApiMock;

        protected AllSetsLogic Logic;

        public List<Set> SetsApiResponse = new List<Set>
        {
            new Set { Id = "1" },
            new Set { Id = "2" },
            new Set { Id = "3" },
            new Set { Id = "4" },
            new Set { Id = "5" }
        };

        public List<IdResult> CollectionSetIds = new List<IdResult>
        {
            new IdResult { Id = "1" },
            new IdResult { Id = "2" },
            new IdResult { Id = "3" }
        };

        [TestInitialize]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();
        }

        public override void AfterEachTest()
        {
            base.AfterEachTest();
        }

        protected override void CreateMocks()
        {
            CardCollectionMock = new Mock<ICardCollection>();
            ApiMock = new Mock<IApi>();
        }

        protected override void SetupMocks()
        {
            ApiMock.Setup(m => m.GetSetsAsync(It.IsAny<QueryParameters>()))
                .Returns(Task.FromResult(SetsApiResponse));

            CardCollectionMock.Setup(m => m.QueryAsync<IdResult>(Queries.GetAllSetIdsInCollection))
                .Returns(Task.FromResult(CollectionSetIds));
        }

        protected override void SetupData()
        {
            Logic = new AllSetsLogic
            (
                ApiMock.Object,
                CardCollectionMock.Object
            );
        }

        [TestMethod]
        public async Task WhenGetSetsAsyncIsCalled_AndThereAreMatchingSetsInCollection_ThenAllApiSetResultsAreReturned()
        {
            var allSets = await Logic.GetSetsAsync();

            Assert.AreEqual(SetsApiResponse.Count, allSets.Count);
        }

        [TestMethod]
        public async Task WhenGetSetsAsyncIsCalled_AndThereAreMatchingSetsInCollection_ThenIsInCollectionIsSet()
        {
            var allSets = await Logic.GetSetsAsync();

            Assert.AreEqual(CollectionSetIds.Count, allSets.Where(x => x.IsInCollection).Count());
        }
        [TestMethod]
        public async Task WhenGetSetsOrderedByMostRecentAsyncIsCalled_AndThereAreMatchingSetsInCollection_ThenAllApiSetResultsAreReturned()
        {
            var allSets = await Logic.GetSetsAsync();

            Assert.AreEqual(SetsApiResponse.Count, allSets.Count);
        }

        [TestMethod]
        public async Task WhenGetSetsOrderedByMostRecentAsyncIsCalled_AndThereAreMatchingSetsInCollection_ThenIsInCollectionIsSet()
        {
            var allSets = await Logic.GetSetsAsync();

            Assert.AreEqual(CollectionSetIds.Count, allSets.Where(x => x.IsInCollection).Count());
        }
    }
}
