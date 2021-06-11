using Moq;
using NUnit.Framework;
using PokemonCardCatalogue.Common.Context.Interfaces;
using PokemonCardCatalogue.Common.Logic;
using PokemonCardCatalogue.Common.Logic.Interfaces;

namespace PokemonCardCatalogue.Tests.LogicTests
{
    [TestFixture]
    public class CollectionCardLogicTests : BaseTestFixture
    {
        protected Mock<IApi> ApiMock;
        protected Mock<ICardCollection> CardCollectionMock;

        protected CollectionLogic Logic;

        protected override void CreateMocks()
        {
            ApiMock = new Mock<IApi>();
            CardCollectionMock = new Mock<ICardCollection>();
        }

        protected override void SetupMocks()
        {
            base.SetupMocks();
        }

        protected override void SetupData()
        {
            Logic = new CollectionLogic
            (
                ApiMock.Object,
                CardCollectionMock.Object
            );
        }

        public void WhenSetOwnedCountForCardIsCalled_ThenOwnedCountSetToValueForObject(int ownedCount)
        {

        }
    }
}
