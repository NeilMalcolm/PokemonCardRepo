using NUnit.Framework;
using Moq;
using PokemonCardCatalogue.Common.Constants;
using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Logic;
using PokemonCardCatalogue.Common.Logic.Interfaces;
using PokemonCardCatalogue.Common.Models;
using PokemonCardCatalogue.Common.Models.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonCardCatalogue.Tests.LogicTests
{
    [TestFixture]
    public class AllSetsLogicTests : BaseTestFixture
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

        protected override void CreateMocks()
        {
            CardCollectionMock = new Mock<ICardCollection>();
            ApiMock = new Mock<IApi>();
        }

        protected override void SetupMocks()
        {
            ApiMock.Setup(m => m.GetSetsAsync(It.IsAny<QueryParameters>(), It.IsAny<bool>()))
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

        [Test]
        public async Task WhenGetSetsAsyncIsCalled_AndThereAreMatchingSetsInCollection_ThenAllApiSetResultsAreReturned()
        {
            var allSets = await Logic.GetSetsAsync();

            Assert.AreEqual(SetsApiResponse.Count, allSets.Count);
        }

        [Test]
        public async Task WhenGetSetsAsyncIsCalled_AndThereAreMatchingSetsInCollection_ThenIsInCollectionIsSet()
        {
            var allSets = await Logic.GetSetsAsync();

            Assert.AreEqual(CollectionSetIds.Count, allSets.Where(x => x.IsInCollection).Count());
        }
        [Test]
        public async Task WhenGetSetsOrderedByMostRecentAsyncIsCalled_AndThereAreMatchingSetsInCollection_ThenAllApiSetResultsAreReturned()
        {
            var allSets = await Logic.GetSetsAsync();

            Assert.AreEqual(SetsApiResponse.Count, allSets.Count);
        }

        [Test]
        public async Task WhenGetSetsOrderedByMostRecentAsyncIsCalled_AndThereAreMatchingSetsInCollection_ThenIsInCollectionIsSet()
        {
            var allSets = await Logic.GetSetsAsync();

            Assert.AreEqual(CollectionSetIds.Count, allSets.Where(x => x.IsInCollection).Count());
        }
    }
}
